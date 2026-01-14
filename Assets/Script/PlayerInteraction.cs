using UnityEngine;
using UnityEngine.InputSystem;

public partial class Player : MonoBehaviour
{
    private GameObject _target;
    private GameObject _hand;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Jar" && _hand == null)
        {
            Debug.Log("타겟 항아리 있음");
            _target = other.gameObject;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Jar")
        {
            Debug.Log("타겟 항아리 없음");
            _target = null;
        }
    }

    private void OnInteraction(InputAction.CallbackContext ctx)
    {
        if (ctx.performed && _target != null && _hand == null)
        {
            _hand = _target.transform.parent.gameObject;
            _target = null;

            GrapJar();
            Debug.Log("항아리 잡음");
        }

        else if (ctx.performed && _target == null && _hand != null)
        {
            PutJar();
            Debug.Log("항아리 놓기");

            _hand = null;
        }
    }

    private void GrapJar()
    {
        Rigidbody jarRigid = _hand.transform.GetComponent<Rigidbody>();
        jarRigid.isKinematic = true;
        jarRigid.useGravity = false;
        jarRigid.detectCollisions = false;

        _hand.transform.SetParent(gameObject.transform, false);
        _hand.transform.position = _jarPos.transform.position;
        _hand.transform.rotation = gameObject.transform.rotation;
    }

    private void PutJar()
    {
        Rigidbody jarRigid = _hand.transform.GetComponent<Rigidbody>();
        jarRigid.isKinematic = false;
        jarRigid.useGravity = true;
        jarRigid.detectCollisions = true;

        _hand.transform.SetParent(null);

        Vector3 putPos = gameObject.transform.position + transform.forward * _putPos;
        _hand.transform.position = putPos;
    }
}
