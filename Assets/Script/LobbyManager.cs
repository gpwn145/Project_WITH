using UnityEngine;
using Photon.Pun;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using Photon.Realtime;

public class LobbyManager : MonoBehaviourPunCallbacks
{
    [SerializeField,Range(2,4)] private int _maxPlayers = 2;
    [SerializeField] private Text _inputRoomID;
    private List<RoomInfo> _roomList = new List<RoomInfo>();
    public void OnCreateRoomButton()
    {
        PhotonNetwork.CreateRoom(
            null, 
            new Photon.Realtime.RoomOptions { MaxPlayers = _maxPlayers}
            );
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        Debug.Log($"로비 갱신됨");

    }

    //방 생성시 호출
    public override void OnCreatedRoom()
    {
        Debug.Log($"방 생성 성공 {PhotonNetwork.CurrentRoom.Name}");
    }

    //방 생성 실패
    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        Debug.Log($"생성 실패 {returnCode} : {message}");
    }

    public override void OnJoinedRoom()
    {
        Debug.Log($"방에 입장");
        PhotonNetwork.LoadLevel("RoomScene");
    }

    //참여하기 버튼
    public void OnJoinRoomButton()
    {
        PhotonNetwork.JoinRoom(_inputRoomID.text);
        Debug.Log($"{_inputRoomID.text}");
    }

    //참여하기 실패
    public override void OnJoinRoomFailed(short returnCode, string message) 
    {
        Debug.Log($"참여 실패 {returnCode} : {message}");
    }
}
