using Photon.Pun;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class MenuScript : MonoBehaviourPunCallbacks
{
    [Header("게임매니저")]
    [SerializeField] public GameSceneManager _gameSceneManager;

    [Header("세팅 버튼")]
    [SerializeField] public GameObject _restart;
    [SerializeField] public GameObject _goToRoom;

    [Header("클리어 버튼")]
    [SerializeField] public GameObject _nextStage;
    [SerializeField] public GameObject _cGoToRoom;

    private void Awake()
    {
        _restart.SetActive(false);
        _goToRoom.SetActive(false);
        _nextStage.SetActive(false);
        _cGoToRoom.SetActive(false);
    }

    private void Start()
    {
        if (PhotonNetwork.IsMasterClient == false)
            return;

        _restart.SetActive(true);
        _goToRoom.SetActive(true);
        _nextStage.SetActive(true);
        _cGoToRoom.SetActive(true);
    }

    public void OnRestartButton()
    {
        _gameSceneManager.Restart();
    }
    public void OnReSoundButton()
    {
        //판넬 따로 준비
    }
    public void OnKeySettingButton()
    {
        //ESC키
    }

    public void OnGoToRoomButton()
    {
        if (!PhotonNetwork.IsMasterClient) return;
        _gameSceneManager._isSceneChanging = true;
        PhotonNetwork.LoadLevel("RoomScene");
    }

    public void OnGoToNextStageButton()
    {
        if (!PhotonNetwork.IsMasterClient) return;
        _gameSceneManager._isSceneChanging = true;
        _gameSceneManager.NextStage();
    }
}
