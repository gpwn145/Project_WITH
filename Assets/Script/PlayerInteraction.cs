using Photon.Pun;
using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Windows;

public partial class PlayerScript : MonoBehaviourPunCallbacks
{
    private GameObject _target;
    private GameObject _hand;
    private GameObject _WaterButton;
    private float _input;
    private bool _beforeInput;

    public static event Action<PlayerScript> OnGrab;

    public GameObject Hand => _hand;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Jar" && _hand == null)
        {
            Debug.Log("타겟 항아리 있음");
            _target = other.gameObject;
        }

        if (other.gameObject.tag == "Respawn")
        {
            gameObject.transform.position = _rewpawnPos.transform.position;
        }

        if (other.gameObject.tag == "Button")
        {
            Debug.Log("누를버튼 있음");
            _WaterButton = other.gameObject;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Jar")
        {
            Debug.Log("타겟 항아리 없음");
            _target = null;
        }

        if (other.gameObject.tag == "Button")
        {
            Debug.Log("누를버튼 없음");
            _WaterButton = null;
        }
    }

    private void OnInteraction(InputAction.CallbackContext ctx)
    {
        if (!photonView.IsMine) return;

        _input = ctx.ReadValue<float>();
        bool pressed = (_input > 0.5f);

        if (pressed == true && _beforeInput == false)
        {
            if (_target != null && _hand == null)
            {
                _hand = _target.transform.parent.gameObject;
                _target = null;

                GrapJar();
                Debug.Log("항아리 잡음");
                OnGrab.Invoke(this);
            }

            else if (_target == null && _hand != null)
            {
                PutJar();
                Debug.Log("항아리 놓기");

                _hand = null;
            }
        }

        if (_WaterButton != null && _hand == null)
        {
            Debug.Log($"버튼 누름 키 눌림 {pressed}");
            _presenter.IsFillStart = (pressed == true);
        }

        _beforeInput = pressed;
    }

    private void GrapJar()
    {
        Rigidbody jarRigid = _hand.transform.GetComponent<Rigidbody>();
        Collider jarColl = _hand.transform.GetComponent<Collider>();

        jarRigid.isKinematic = true;
        jarRigid.useGravity = false;
        jarRigid.detectCollisions = false;
        jarRigid.freezeRotation = false;

        jarColl.gameObject.SetActive(false);
        jarColl.gameObject.SetActive(true);

        _hand.transform.SetParent(gameObject.transform, false);
        _hand.transform.position = _jarPos.transform.position;
        _hand.transform.rotation = gameObject.transform.rotation;
    }

    private void PutJar()
    {
        Rigidbody jarRigid = _hand.transform.GetComponent<Rigidbody>();
        Collider jarColl = _hand.transform.GetComponent<Collider>();

        jarRigid.isKinematic = false;
        jarRigid.useGravity = true;
        jarRigid.detectCollisions = true;
        jarRigid.freezeRotation = false;

        jarColl.gameObject.SetActive(false);
        jarColl.gameObject.SetActive(true);

        _hand.transform.SetParent(null);

        Vector3 putPos = gameObject.transform.position + transform.forward * _putPos;
        _hand.transform.position = putPos;
    }
}
