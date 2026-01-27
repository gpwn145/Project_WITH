using Photon.Pun;
using System;
using UnityEngine;

enum ActionType
{
    Grab,
    Put,
    Throw,
    WaterFillStart,
    WaterFillEnd
}
public partial class PlayerScript : MonoBehaviourPunCallbacks
{
    bool isFilling = false;
    public static event Action OnFillStop;

    [PunRPC]
    void RPC_MasterAction(ActionType Action, int jarViewId)
    {
        if (PhotonNetwork.IsMasterClient == false)
        {
            return;
        }

        switch (Action)
        {
            case ActionType.WaterFillEnd:
                {
                    //내려놓기 : 버튼 못누름, 
                    if (isFilling == false)
                    {
                        return;
                    }
                    photonView.RPC(
                   nameof(RPC_ApplyAction),
                    RpcTarget.All,
                    ActionType.WaterFillEnd,
                    0,
                    Vector3.zero
                   );
                    isFilling = false;
                }
                break;

            case ActionType.WaterFillStart:
                {
                    if (isFilling == true)
                    {
                        return;
                    }
                    //버튼
                    photonView.RPC(
                       nameof(RPC_ApplyAction),
                        RpcTarget.All,
                        ActionType.WaterFillStart,
                        0,
                        Vector3.zero
                       );
                    isFilling = true;
                }
                break;

        }

        PhotonView jarPV = PhotonView.Find(jarViewId);

        if (jarPV == null)
        {
            return;
        }



        switch (Action)
        {
            case ActionType.Grab:
                {
                    if (isFilling == true)
                    {
                        photonView.RPC(
                            nameof(RPC_ApplyAction),
                            RpcTarget.All,
                            ActionType.WaterFillEnd,
                            0,
                            Vector3.zero
                        );

                        isFilling = false;
                        Debug.Log($"[RPC_MasterAction] 물 켜져있음 -1전달");
                    }

                    //버튼 항아리 있을때 항아리 먼저 잡을 수 있음
                    if (jarPV.Owner != null && jarPV.IsMine == false)
                    {
                        return;
                    }

                    int grabPlayerNum = photonView.OwnerActorNr;

                    jarPV.TransferOwnership(PhotonNetwork.MasterClient);

                    photonView.RPC(
                       nameof(RPC_ApplyAction),
                        RpcTarget.All,
                        ActionType.Grab,
                        jarViewId,
                        Vector3.zero
                        );

                    isFilling = false;
                }
                break;

            case ActionType.Throw:
                {
                    if (isFilling == true)
                    {
                        photonView.RPC(
                        nameof(RPC_ApplyAction),
                        RpcTarget.All,
                        ActionType.WaterFillEnd,
                        jarViewId,
                        Vector3.zero
                       );
                    }

                    Vector3 throwWay = (_rigidbody.transform.forward * 1f) + (Vector3.up * 0.4f);
                    throwWay = throwWay.normalized * _throwForce;
                    photonView.RPC(
                    nameof(RPC_ApplyAction),
                    RpcTarget.All,
                    ActionType.Throw,
                    jarViewId,
                    throwWay
                    );
                    isFilling = false;
                }
                break;

            case ActionType.Put:
                {
                    if (isFilling == true)
                    {
                        photonView.RPC(
                        nameof(RPC_ApplyAction),
                        RpcTarget.All,
                        ActionType.WaterFillEnd,
                        jarViewId,
                        Vector3.zero
                       );
                    }

                    photonView.RPC(
                    nameof(RPC_ApplyAction),
                    RpcTarget.All,
                    ActionType.Put,
                    jarViewId,
                    Vector3.zero
                    );
                    isFilling = false;
                }
                break;
        }
    }


    [PunRPC]
    void RPC_ApplyAction(ActionType Action, int jarViewId, Vector3 throwWay)
    {
        if (_gameSceneManager._isSceneChanging) return;

        Debug.Log($"[WaterFillStart] presenter = {_presenter}, waterButton = {_presenter?._waterButton}");

        switch (Action)
        {
            case ActionType.WaterFillStart:
                {
                    WaterButtonInteraction(true);
                    Debug.Log($"[RPC_ApplyAction] 물 오브젝트 활성화");
                    return;
                }
            //버튼
            case ActionType.WaterFillEnd:
                {
                    WaterButtonInteraction(false);
                    OnFillStop?.Invoke();
                    Debug.Log($"[RPC_ApplyAction] 물 오브젝트 비활성화");
                    return;
                }
        }

        if (jarViewId <= 0)
            return;

        PhotonView jarPV = PhotonView.Find(jarViewId);
        if (jarPV == null)
            return;

        _hand = jarPV.gameObject;
        _target = null;

        Jar jar = jarPV.GetComponent<Jar>();
        if (jar == null)
            return;

        switch (Action)
        {
            case ActionType.Grab:
                {
                    GrabJar(_hand.GetComponent<Jar>().jarSpwaner);
                    _gameSceneManager.GSGrab(this);
                    _gameSceneManager.GSGrabTargetJar(_hand.GetComponent<Jar>());
                    Debug.Log($"[RPC_ApplyAction] 항아리 잡았음 ");
                    _gameSceneManager.GSGrabOrPut(_hand.GetComponent<Jar>());
                }

                break;
            case ActionType.Throw:
                {
                    jarPV.TransferOwnership(PhotonNetwork.MasterClient);

                    _hand.gameObject.GetComponent<Jar>().GodMode(false);
                    ThrowJar(throwWay);

                    _gameSceneManager.GSGrabOrPut(_hand.GetComponent<Jar>());

                    _hand = null;
                    Debug.Log($"[RPC_ApplyAction] 항아리 던졋음 ");
                }
                break;
            case ActionType.Put:
                {
                    jarPV.TransferOwnership(PhotonNetwork.MasterClient);

                    _hand.gameObject.GetComponent<Jar>().GodMode(true);
                    PutJar();

                    _gameSceneManager.GSGrabOrPut(_hand.GetComponent<Jar>());
                    _hand = null;
                    Debug.Log($"[RPC_ApplyAction] 플레이어 항아리 내려놓았음 ");
                }
                break;
        }
    }
}
