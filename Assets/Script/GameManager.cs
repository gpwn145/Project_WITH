using Firebase.Database;
using Firebase.Extensions;
using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviourPunCallbacks
{
    public static GameManager Instance;
    
    public int choiceStage;
    public bool isRoomMgrReady = false;
    public event Action<int> OnSetStage;
    public event Action OnFirebaseCallback;
    //스테이지 클리어 정보
    public int stageClearInfo;
    public float mouseSpeed;


    private void Awake()
    {
        #region 싱글톤
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        #endregion
    }

    private void Start()
    {
        mouseSpeed = 5f;
    }

    public IEnumerator SetStage()
    {
        yield return new WaitUntil(() => isRoomMgrReady);

        if (!PhotonNetwork.InRoom)
        {
            Debug.LogError("[GameManager] 방 입장 전 SetStage 호출됨");
            yield break;
        }

        if (PhotonNetwork.CurrentRoom == null)
        {
            Debug.LogError("[GameManager] CurrentRoom null");
            yield break;
        }

            var props = PhotonNetwork.CurrentRoom.CustomProperties;

        Debug.Log($"열린 스테이지 개수 : {stageClearInfo}");
        

        if (props.ContainsKey("StageClearInfo") == false)
        {
            Debug.Log($"파이어베이스 데이터 읽어오기");
            LoadFirebase();
        }
        else
        {
            stageClearInfo = (int)props["StageClearInfo"];
            Debug.Log($"열린 스테이지 개수 불러오기 : {stageClearInfo}");
            if(OnSetStage == null)
            {
                Debug.Log($"온셋스테이지 널");
            }
            OnSetStage?.Invoke(stageClearInfo);
            OnFirebaseCallback?.Invoke();
        }
    }

    public void UpdateStageClearInfo(int clearStage, float mouseSpeed, float soundVolum)
    {
        if (PhotonNetwork.IsMasterClient == false)
        {
            return;
        }
        stageClearInfo = clearStage + 1;
        var props = PhotonNetwork.CurrentRoom.CustomProperties;
        props["StageClearInfo"] = stageClearInfo;
        props["ChoiceStage"] = choiceStage + 1;
        props["MouseSpeed"] = mouseSpeed;
        Debug.Log($"다음 스테이지 : {props["StageClearInfo"]}");
        Debug.Log($"다음 스테이지 세팅 : {props["ChoiceStage"]}");
        Debug.Log($"설정 감도 : {props["MouseSpeed"]}");

        PhotonNetwork.CurrentRoom.SetCustomProperties(props);
        SaveFirebase();
    }    

    //스테이지 클리어 정보

    public void StageSave(int stage)
    {
        if (!PhotonNetwork.IsMasterClient) return;
        var props = PhotonNetwork.CurrentRoom.CustomProperties;

        choiceStage = stage;
        Debug.Log($"선택 스테이지 : {choiceStage}");

        props["ChoiceStage"] = choiceStage;
        PhotonNetwork.CurrentRoom.SetCustomProperties(props);
    }

    public void SaveFirebase()
    {
        if (!PhotonNetwork.IsMasterClient) return;

        DatabaseReference root = BackendManager.Database.RootReference;
        var props = PhotonNetwork.CurrentRoom.CustomProperties;

        Dictionary<string, object> data = new Dictionary<string, object>()
        {
            {"nickname", PhotonNetwork.NickName },
            {"StageClearInfo", stageClearInfo }
        };
        root.Child(BackendManager.Auth.CurrentUser.UserId).SetValueAsync(data);
    }

    public void saveNickName(string nickName)
    {
        DatabaseReference root = BackendManager.Database.RootReference;
        root.Child(BackendManager.Auth.CurrentUser.UserId).Child("nickname").SetValueAsync(nickName);
    }

    public void LoadFirebase()
    {
        DatabaseReference root = BackendManager.Database.RootReference;
        DatabaseReference data = root.Child(BackendManager.Auth.CurrentUser.UserId);

        data.GetValueAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted)
            {
                Debug.Log($"데이터 읽기 오류 : {task.Exception}");
                return;
            }

            if (task.Result.Exists == false)
            {
                Debug.Log($"저장 데이터 없음");
                stageClearInfo = 1;
                OnSetStage?.Invoke(stageClearInfo);
                return;
            }

            DataSnapshot snapshot = task.Result;

            stageClearInfo = int.Parse(snapshot.Child("StageClearInfo").Value.ToString());
            PhotonNetwork.NickName = snapshot.Child("nickname").Value.ToString();

            var props = PhotonNetwork.CurrentRoom.CustomProperties;
            props["StageClearInfo"] = stageClearInfo;
            PhotonNetwork.CurrentRoom.SetCustomProperties(props);
            Debug.Log($"열린 스테이지 저장정보 : {(int)props["StageClearInfo"]}");
            if(OnSetStage == null)
            {
                Debug.Log($"온셋스테이지 널");
            }
            OnSetStage?.Invoke(stageClearInfo);
            OnFirebaseCallback?.Invoke();
        });
    }

    public void MouseSpeed(float speed)
    {
        mouseSpeed = speed;
    }
}
