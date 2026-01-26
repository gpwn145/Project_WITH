using Photon.Pun;
using System;
using UnityEngine;
using UnityEngine.InputSystem;

public partial class PlayerScript : MonoBehaviourPunCallbacks
{
    private GameObject _target;
    private GameObject _hand;
    //private float _input;
    private bool _beforeInput;

    const int LAYER_JarSpawn = 6;
    const int LAYER_JarPlayer = 7;
    const int LAYER_JarPutDown = 8;

    public GameObject Hand => _hand;

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log($"플레이어 트리거 : {other.gameObject.name}");

        if (other.gameObject.tag == "Button" && _hand == null)
        {
            _target = other.gameObject;
            Debug.Log("누를버튼 있음");
        }

        else if (other.gameObject.tag == "JarGrabRange" && _hand == null)
        {
            _target = other.gameObject;
            Debug.Log("타겟 항아리 있음");
        }

        if (other.gameObject.tag == "Out")
        {
            _gameSceneManager.ReSpawnPlayer(gameObject, _hand);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Button")
        {
            Debug.Log("누를버튼 없음");
            _target = null;

            if (photonView.IsMine)
            {
                photonView.RPC(
                    nameof(RPC_MasterAction),
                    RpcTarget.MasterClient,
                    ActionType.WaterFillEnd,
                    0
                );
            }
        }
        else if (other.gameObject.tag == "JarGrabRange")
        {
            Debug.Log("타겟 항아리 없음");
            _target = null;
        }
    }

    private void OnHoldAction(InputAction.CallbackContext ctx)
    {
        if (!photonView.IsMine) return;

        if (_target != null && _hand == null)
        {
            if (ctx.performed && _target.tag == "JarGrabRange")
            {
                Debug.Log("항아리 잡음");

                PhotonView jarPV = _target.transform.parent.GetComponent<PhotonView>();

                photonView.RPC(
                    nameof(RPC_MasterAction), 
                    RpcTarget.MasterClient,
                    ActionType.Grab,
                    jarPV.ViewID);
            }
        }

        else if (_target == null && _hand != null)
        {
            if (ctx.performed)
            {
                Debug.Log("항아리 놓기");

                PhotonView jarPV = _hand.GetComponent<PhotonView>();

                photonView.RPC(
                    nameof(RPC_MasterAction),
                    RpcTarget.MasterClient,
                    ActionType.Put,
                    jarPV.ViewID);
            }
        }
    }

    private void OnWaterButtonAction(InputAction.CallbackContext ctx)
    {
        if (!photonView.IsMine) return;

        if (_target != null && _hand == null && _target.tag == "Button")
        {
            if (ctx.started)
            {
                Debug.Log($"물 버튼 클릭 됨");
                photonView.RPC(
                   nameof(RPC_MasterAction),
                    RpcTarget.MasterClient,
                    ActionType.WaterFillStart,
                    0
                   );
            }
            else if (ctx.canceled)
            {
                Debug.Log($"물 버튼 꺼짐");
                photonView.RPC(
                   nameof(RPC_MasterAction),
                    RpcTarget.MasterClient,
                    ActionType.WaterFillEnd,
                    0
                   );
            }
        }
    }

    private void OnThorowAction(InputAction.CallbackContext ctx)
    {
        if (!photonView.IsMine) return;

        if (ctx.performed)
        {
            if (_target == null && _hand != null)
            {
                Debug.Log("항아리 던지기");

                PhotonView jarPV = _hand.GetComponent<PhotonView>();

                photonView.RPC(
                    nameof(RPC_MasterAction),
                    RpcTarget.MasterClient,
                    ActionType.Throw,
                    jarPV.ViewID
                    );
            }
        }
    }


    private void ThrowJar(Vector3 throwWay)
    {
        Rigidbody jarRigid = _hand.transform.GetComponent<Rigidbody>();
        Collider jarColl = _hand.transform.GetComponent<Collider>();


        _hand.transform.SetParent(null);
        _hand.GetComponent<Jar>()._jarState = JarState.None;

        jarColl.enabled = true;

        jarRigid.isKinematic = false;
        jarRigid.useGravity = true;
        jarRigid.detectCollisions = true;

        jarRigid.linearVelocity = Vector3.zero;
        jarRigid.angularVelocity = Vector3.zero;

        _hand.layer = LAYER_JarPutDown;
        gameObject.layer = 0;
        
        jarRigid.AddForce(throwWay, ForceMode.Impulse);
    }

    private void GrabJar(JarSpwaner jarSpwaner)
    {
        if(_hand.gameObject.layer == LAYER_JarSpawn)
        {
            _presenter.JarTaken(jarSpwaner); 
            Debug.Log($"항아리 가져감");
        }

        Rigidbody jarRigid = _hand.transform.GetComponent<Rigidbody>();
        Collider jarColl = _hand.transform.GetComponent<Collider>();
        gameObject.layer = LAYER_JarPlayer; 
        _hand.layer = LAYER_JarPlayer;
        _hand.GetComponent<Jar>()._jarState = JarState.None;

        jarRigid.isKinematic = true;
        jarRigid.useGravity = false;
        jarRigid.detectCollisions = true;

        _hand.transform.SetParent(gameObject.transform, false);
        _hand.transform.position = _jarPos.transform.position;
        _hand.transform.rotation = gameObject.transform.rotation;
    }

    private void PutJar()
    {
        Rigidbody jarRigid = _hand.transform.GetComponent<Rigidbody>();
        Collider jarColl = _hand.transform.GetComponent<Collider>();
        gameObject.layer = 0;
        _hand.GetComponent<Jar>()._jarState = JarState.None;

        jarRigid.isKinematic = false;
        jarRigid.useGravity = true;
        jarRigid.detectCollisions = true;


        _hand.transform.SetParent(null);

        _hand.gameObject.GetComponent<Jar>().GodMode(true);
        _hand.layer = LAYER_JarPutDown;
        Vector3 putPos = gameObject.transform.position + transform.forward * _putPos;
        _hand.transform.position = putPos;
    }

    private void WaterButtonInteraction(bool isOpen)
    {
        _presenter._waterButton.WaterOpen(isOpen);
    }
}
