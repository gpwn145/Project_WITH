using Photon.Pun;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class RoomManager : MonoBehaviourPunCallbacks
{
    [SerializeField] GameObject _playerListPanel;
    [SerializeField] Text _playerNamePrefab;
    [SerializeField] InputField _roomIDField;

    [SerializeField] GameObject _stageListPanel;
    [SerializeField] StageManager stageManager;

    [SerializeField] int _totalPlayerNum;

    IEnumerator Start()
    {
        yield return new WaitUntil(() => PhotonNetwork.InRoom);

        RoomID();
        PlayerListSetting();
    }

    public void StageSetting()
    {
        
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
        _roomIDField.text = $"{PhotonNetwork.CurrentRoom.Name}";
    }

    public void OnClickStartButton()
    {
        if(stageManager.ChoiceStage != null)
        {
            PhotonNetwork.LoadLevel("TestScene");
        }
    }
}
