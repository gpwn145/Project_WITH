using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class RoomManager : MonoBehaviourPunCallbacks
{
    [Header("접속자 리스트 패널")]
    [SerializeField] private GameObject _playerListPanel;
    [Header("접속자 이름표 오브젝트")]
    [SerializeField] private GameObject _playerNamePrefab;
    private InputField _myNmeBoxText;

    [Header("룸아이디필드")]
    [SerializeField] private InputField _roomIDField;
    private string _roomID;

    [Header("스테이지 리스트 패널")]
    [SerializeField] private GameObject _stageListPanel;
    [Header("스테이지 버튼")]
    [SerializeField] private Button[] _stageButton = new Button[3];
    //[Header("스테이지 자물쇠 이미지")]
    //[SerializeField] private GameObject[] _stageLockImage = new GameObject[3];

    [Header("전체 플레이어 수")]
    [SerializeField] private int _totalPlayerNum;

    [Header("시작하기 버튼")]
    [SerializeField] private Button _startButton;
    [Header("로비가기 버튼")]
    [SerializeField] private Button _backButton;

    private GameManager gameManager;
    private string _nickName;
    private IEnumerator co;


    private void Awake()
    {
        Debug.Log($"[RoomManager] 어웨이크 실행");
        _startButton.interactable = false;
        _backButton.interactable = false;
        for (int i = 0; i < _stageButton.Length; i++)
        {
            _stageButton[i].interactable = false;
            //_stageLockImage[i].SetActive(true);
        }
    }

    IEnumerator Start()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        if(SoundManager.Instance.isPlayBGM == false )
        {
            SoundManager.Instance.SoundPlay(Sound.LobbyBGM);
        }

        yield return new WaitUntil(() => PhotonNetwork.InRoom);
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        if (gameManager == null)
        {
            Debug.Log($"게임 매니저 없음");
            yield break;
        }
        Debug.Log($"[RoomManager] 방에 참여됨");

        co = gameManager.SetStage();
        StartCoroutine(co);

        gameManager.OnSetStage += RoomSetting;
        gameManager.OnSetStage += PlayerListSetting;
        gameManager.OnFirebaseCallback += ActiveBackButton;
        Debug.Log($"[RoomManager] 게임매니저이벤트 구독완료");

        gameManager.isRoomMgrReady = true;
        Debug.Log($"[RoomManager] 룸매니저 준비완료");

        RoomID();
    }

    private void ActiveBackButton()
    {
        _backButton = GameObject.Find("Back")?.GetComponent<Button>();
        _backButton.interactable = true;
        Debug.Log($"[RoomManager] 로비로 가기 버튼 활성화");
    }


    public void RoomSetting(int stage)
    {
        if (!PhotonNetwork.IsMasterClient) return;

        _startButton.interactable = true;

        Debug.Log($"열린 스테이지 개수 불러오기 : {stage}");

        for (int i = 0; i < stage; i++)
        {
            _stageButton[i].interactable = true;
            //_stageLockImage[i].SetActive(false);
        }

    }

    //참여자 아이디 띄워주기
    public void PlayerListSetting(int none)
    {
        if (_myNmeBoxText != null &&
        !string.IsNullOrEmpty(_myNmeBoxText.text) && PhotonNetwork.NickName != _myNmeBoxText.text)
        {
            PhotonNetwork.NickName = _myNmeBoxText.text;
        }
        
        _playerListPanel = GameObject.Find("PlayerList")?.gameObject;

        foreach (Transform child in _playerListPanel.transform)
        {
            Destroy(child.gameObject);
        }

        _totalPlayerNum = PhotonNetwork.PlayerList.Length;

        //처음 세팅 : 호스트 이름 띄워주기
        for (int i = 0; i < _totalPlayerNum; i++)
        {
            GameObject nameBox = Instantiate(_playerNamePrefab, _playerListPanel.transform);

            if (PhotonNetwork.PlayerList[i].IsLocal)
            {
                _myNmeBoxText = nameBox.GetComponentInChildren<InputField>();
                _myNmeBoxText.text = PhotonNetwork.PlayerList[i].NickName;

                _myNmeBoxText.interactable = true;

                Text text = _myNmeBoxText.textComponent;
                text.fontStyle = FontStyle.Bold;
            }
            else
            {
                
                InputField nameBoxText = nameBox.GetComponentInChildren<InputField>();
                nameBoxText.interactable = false;
                nameBoxText.text = PhotonNetwork.PlayerList[i].NickName;
            }
            Debug.Log($"{PhotonNetwork.PlayerList[i].NickName} 참여");
        }
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        PlayerListSetting(0);
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        if (otherPlayer.IsMasterClient)
        {
            Debug.Log("마스터 방 나감");
            PhotonNetwork.LeaveRoom();
        }
        PlayerListSetting(0);
    }

    private void RoomID()
    {
        _roomIDField.readOnly = true;
        _roomID = PhotonNetwork.CurrentRoom.Name;
        _roomIDField.text = _roomID;
    }

    public void OnStageChoice(int stage)
    {
        if (PhotonNetwork.IsMasterClient == false)
        {
            return;
        }

        SoundManager.Instance.SoundPlay(Sound.BaseButtonClick);

        switch (stage)
        {
            case 0:
                //프리팹 설정
                Debug.Log($"스테이지1 선택");
                break;
            case 1:
                Debug.Log($"스테이지2 선택");
                break;
            case 2:
                Debug.Log($"테스트맵 선택");
                break;
        }
        GameManager.Instance.StageSave(stage);
    }

    public void OnClickStartButton()
    {
        SoundManager.Instance.SoundPlay(Sound.BaseButtonClick);
        gameManager.OnSetStage -= RoomSetting;
        PhotonNetwork.NickName = _myNmeBoxText.text;
        gameManager.saveNickName(PhotonNetwork.NickName);

        if (!PhotonNetwork.IsMasterClient) return;

        var props = PhotonNetwork.CurrentRoom.CustomProperties;

        if (props.ContainsKey("ChoiceStage") == false)
        {
            gameManager.choiceStage = 0;
            props["ChoiceStage"] = gameManager.choiceStage;
            Debug.Log($"선택 스테이지 : {props["ChoiceStage"]}");
        }

        photonView.RPC(
                nameof(RPC_OnClickStartButton),
                RpcTarget.Others,
                gameManager.choiceStage
                );
        PhotonNetwork.LoadLevel("GameScene");
    }

    [PunRPC]
    public void RPC_OnClickStartButton(int stage)
    {
        var props = PhotonNetwork.CurrentRoom.CustomProperties;

        if (props.ContainsKey("ChoiceStage") == false)
        {
            props["ChoiceStage"] = stage;
            Debug.Log($"선택 스테이지 : {props["ChoiceStage"]}");
        }
    }

    public void OnGoToLobby()
    {
        SoundManager.Instance.SoundPlay(Sound.BackButtonClick);
        gameManager.choiceStage = 0;
        gameManager.isRoomMgrReady = false;
        StopCoroutine(co);
        gameManager.OnSetStage -= RoomSetting;
        gameManager.OnSetStage -= PlayerListSetting;
        gameManager.OnFirebaseCallback -= ActiveBackButton;
        PhotonNetwork.LeaveRoom();
        Debug.Log($"룸 나감");
    }
    public override void OnLeftRoom()
    {
        Debug.Log($"Region={PhotonNetwork.CloudRegion}");
        Debug.Log($"로비 연결 성공");
        PhotonNetwork.LoadLevel("LobbyScene");
    }
}
