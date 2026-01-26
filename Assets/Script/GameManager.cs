using Photon.Pun;
using Photon.Pun.Demo.PunBasics;
using UnityEngine;

public class GameManager : MonoBehaviourPunCallbacks
{
    public static GameManager Instance;
    
    public int choiceStage;

    //스테이지 클리어 정보
    public int stageClearInfo;


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


    public void SetStage()
    {
        var props = PhotonNetwork.CurrentRoom.CustomProperties;

        Debug.Log($"열린 스테이지 개수 : {stageClearInfo}");
        if (props.ContainsKey("StageClearInfo") == false)
        {
            stageClearInfo = 1;
            props["StageClearInfo"] = stageClearInfo;
            PhotonNetwork.CurrentRoom.SetCustomProperties(props);
            Debug.Log($"열린 스테이지 개수 불러오기 : {stageClearInfo}");
            Debug.Log($"열린 스테이지 저장정보 : {(int)props["StageClearInfo"]}");
        }
        else
        {
            stageClearInfo = (int)props["StageClearInfo"];
            Debug.Log($"열린 스테이지 개수 불러오기 : {stageClearInfo}");
        }
    }

    public void UpdateStageClearInfo(int clearStage)
    {
        if (PhotonNetwork.IsMasterClient == false)
        {
            return;
        }
        stageClearInfo = clearStage + 1;
        var props = PhotonNetwork.CurrentRoom.CustomProperties;
        props["StageClearInfo"] = stageClearInfo;
        props["ChoiceStage"] = choiceStage + 1;
        Debug.Log($"다음 스테이지 : {props["StageClearInfo"]}");
        Debug.Log($"다음 스테이지 세팅 : {props["ChoiceStage"]}");

        PhotonNetwork.CurrentRoom.SetCustomProperties(props);
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

        Debug.Log($"열린 스테이지 저장정보 : {(int)props["ChoiceStage"]}");
    }
}
