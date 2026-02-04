using Photon.Pun;
using UnityEngine;

public class MenuScript : MonoBehaviourPunCallbacks
{
    [Header("게임매니저")]
    [SerializeField] private GameSceneManager _gameSceneManager;

    [Header("세팅 버튼")]
    [SerializeField] private GameObject _restart;
    [SerializeField] private GameObject _goToRoom;

    [Header("클리어 버튼")]
    [SerializeField] private GameObject _nextStage;
    [SerializeField] private GameObject _cGoToRoom;

    private void Awake()
    {
        _restart.SetActive(false);
        _goToRoom.SetActive(false);
        _nextStage.SetActive(false);
        _cGoToRoom.SetActive(false);
    }

    public override void OnEnable()
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
        SoundManager.Instance.SoundPlay(Sound.BaseButtonClick);
        _gameSceneManager.Restart();
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = false;
        _gameSceneManager._isOpen = false;
    }
    public void OnContinueButton()
    {
        SoundManager.Instance.SoundPlay(Sound.SettingPanelClose);
        GameManager.Instance.MouseSpeed(_gameSceneManager.presenter.view.rotateSlider.value);
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = false;
        _gameSceneManager._isOpen = false;
    }
    public void OnKeySettingButton()
    {
        SoundManager.Instance.SoundPlay(Sound.BaseButtonClick);
        _gameSceneManager._isOpen = false;
        //ESC키
    }

    public void OnGoToRoomButton()
    {
        SoundManager.Instance.SoundPlay(Sound.BaseButtonClick);
        _gameSceneManager._isOpen = false;

        if (!PhotonNetwork.IsMasterClient) return;
        _gameSceneManager._isSceneChanging = true;
        _gameSceneManager.BeforeGotoRoom();
        PhotonNetwork.LoadLevel("RoomScene");
    }

    public void OnGoToNextStageButton()
    {
        SoundManager.Instance.SoundPlay(Sound.BaseButtonClick);
        _gameSceneManager._isOpen = false;
        if (!PhotonNetwork.IsMasterClient) return;
        _gameSceneManager._isSceneChanging = true;
        _gameSceneManager.NextStage();
    }
}
