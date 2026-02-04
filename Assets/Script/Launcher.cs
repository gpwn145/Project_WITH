using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;
using Firebase.Auth;
using System.Collections;

public class Launcher : MonoBehaviourPunCallbacks
{
    [SerializeField] private Button _startButton;
    [SerializeField] private Text _startButtonText;
    private FirebaseAuth _auth;

    private string _gameVersion = "1";

    private void Awake()
    {
        _startButtonText.text = "서버연결중";
        _startButton.interactable = false;
        //호스트가 씬 변경 시, 다른 유저의 씬 정보 갱신됨.
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    private void Start()
    {
        //빌드 정보 저장
        PhotonNetwork.GameVersion = _gameVersion;
        //서버연결시도
        PhotonNetwork.ConnectUsingSettings();
    }

    //펀 마스터 연결 시 실행됨
    public override void OnConnectedToMaster()
    {
        Debug.Log($"펀 마스터 연결 성공");
        //로비로 연결 시도

        _startButtonText.text = "로그인 중";
        StartCoroutine(Login());

        PhotonNetwork.JoinLobby();
    }

    public override void OnJoinedLobby()
    {
        Debug.Log($"Region={PhotonNetwork.CloudRegion}");
        Debug.Log($"로비 연결 성공");
    }

    public void OnStartButton()
    {
        SoundManager.Instance.SoundPlay(Sound.BaseButtonClick);
        PhotonNetwork.LoadLevel("LobbyScene");
    }

    private IEnumerator Login()
    {
        yield return new WaitUntil(() => BackendManager.IsReady);
        _auth = BackendManager.Auth;


        if (_auth == null)
        {
            Debug.LogError($"파이어베이스 초기화 안됨");
            yield break;
        }

        if (_auth.CurrentUser != null)
        {
            SuccessLogin();
            yield break;
        }


        _auth.SignInAnonymouslyAsync().ContinueWith(task =>
        {
            if (task.IsFaulted)
            {
                Debug.Log($"로그인 실패, 재접속 해주세요");
                return;
            }

            if (task.IsCompleted)
            {
                SuccessLogin();
            }
        });
    }

    private void SuccessLogin()
    {
        PhotonNetwork.NickName = _auth.CurrentUser.UserId.Substring(0, 6);
        Debug.Log($"{_auth.CurrentUser.UserId} 로그인 성공 ");

        _startButtonText.text = "게임시작";
        _startButton.interactable = true;
    }
}
