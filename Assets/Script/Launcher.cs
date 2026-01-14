using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;

public class Launcher : MonoBehaviourPunCallbacks
{
    [SerializeField] private Button _startButton;
    [SerializeField] private Text _startButtonText;

    private string _gameVersion = "1";

    private void Awake()
    {
        _startButtonText.text = "서버연결중";
        _startButton.enabled = false;
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
        PhotonNetwork.JoinLobby();
    }

    public override void OnJoinedLobby()
    {
        Debug.Log($"Region={PhotonNetwork.CloudRegion}");
        Debug.Log($"로비 연결 성공");
        _startButtonText.text = "게임시작";
        _startButton.enabled = true;
    }

    public void OnStartButton()
    {
        PhotonNetwork.LoadLevel("LobbyScene");
    }
}
