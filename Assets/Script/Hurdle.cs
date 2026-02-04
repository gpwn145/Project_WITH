using Photon.Pun;
using System.Collections;
using UnityEngine;

public class Hurdle : MonoBehaviourPunCallbacks
{
    [Header("이동 힘")]
    [SerializeField, Range(5f, 20f)] private float _moveSpeed = 10f;
    [Header("밀려나는 힘")]
    [SerializeField, Range(0f, 10f)] private float _backForce = 3f;

    private Rigidbody _rigidbody;
    private bool isBack;
    private bool isWaiting = false;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
    }

    //private void FixedUpdate()
    //{
    //    if (!photonView.IsMine) return;

    //    if (isBack == true)
    //    {
    //        if (_rigidbody.linearVelocity.magnitude < 0.1f)
    //        {
    //            isBack = false;
    //            _rigidbody.angularVelocity = Vector3.zero;
    //            Debug.Log("입력 활성화");
    //        }
    //        return;
    //    }
    //}

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Player" || collision.gameObject.tag == "Hurdle" || collision.gameObject.tag == "Jar")
        {
            isBack = true;
            Debug.Log("입력 비활성화");
            if (isWaiting == true)
            {
                return;
            }

            BackMove(collision);
            StartCoroutine(WaitTime());
        }
    }

    private void BackMove(Collision collision)
    {
        Vector3 backward = collision.contacts[0].normal;

        _rigidbody.linearVelocity = Vector3.zero;
        _rigidbody.AddForce(backward * _backForce, ForceMode.Impulse);
        isWaiting = true;
    }

    IEnumerator WaitTime()
    {
        yield return new WaitForSeconds(1f);
        isWaiting = false;
    }
}
