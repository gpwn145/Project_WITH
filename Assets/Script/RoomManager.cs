using Photon.Pun;
using Photon.Pun.Demo.PunBasics;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class RoomManager : MonoBehaviourPunCallbacks
{
    [Header("접속자 리스트 패널")]
    [SerializeField] private GameObject _playerListPanel;
    [Header("접속자 이름표 오브젝트")]
    [SerializeField] private Text _playerNamePrefab;
    [Header("룸아이디필드")]
    [SerializeField] private InputField _roomIDField;
    private string _roomID;

    [Header("스테이지 리스트 패널")]
    [SerializeField] private GameObject _stageListPanel;
    [Header("스테이지 버튼")]
    [SerializeField] private Button[] _stageButton = new Button[3];

    [Header("전체 플레이어 수")]
    [SerializeField] private int _totalPlayerNum;

    [Header("시작하기 버튼")]
    [SerializeField] private Button _startButton;

    private GameManager gameManager;

    private void Awake()
    {
        //_startButton.enabled = false;
        //for (int i = 0; i < _stageButton.Length; i++)
        //{
        //    _stageButton[i].enabled = false;
        //}
    }

    IEnumerator Start()
    {
        yield return new WaitUntil(() => PhotonNetwork.InRoom);
        gameManager = GameManager.Instance;
        if (gameManager == null)
        {
            Debug.Log($"게임 매니저 없음");
            yield break;
        }
        Debug.Log($"[RoomManager] 방에 참여됨");

        RoomID();
        gameManager.SetStage();
        PlayerListSetting();
        RoomSetting();
    }



    public void RoomSetting()
    {
        if (!PhotonNetwork.IsMasterClient) return;

        _startButton.enabled = true;


        for (int i = 0; i < gameManager.stageClearInfo; i++)
        {
            _stageButton[i].enabled = true;
        }
    }

    //참여자 아이디 띄워주기
    public void PlayerListSetting()
    {
        _totalPlayerNum = PhotonNetwork.PlayerList.Length;
        //처음 세팅 : 호스트 이름 띄워주기
        for (int i = 0; i < _totalPlayerNum; i++)
        {
            Text name = Instantiate(_playerNamePrefab, _playerListPanel.transform);
            name.text = PhotonNetwork.PlayerList[i].NickName;
            Debug.Log($"{name.text} 참여");
        }
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
}
