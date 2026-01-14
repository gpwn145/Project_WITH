using System;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;

public partial class Player : MonoBehaviour
{
    [Header("이동 힘")]
    [SerializeField, Range(5f, 20f)] private float _moveSpeed = 10f;
    [Header("회전속도")]
    [SerializeField, Range(0f, 10f)] private float _rotateSpeed = 5f;
    [Header("내려놓는 거리")]
    [SerializeField, Range(0f, 10f)] private float _putPos = 1f;
    [Header("밀려나는 힘")]
    [SerializeField, Range(0f, 10f)] private float _backForce = 3f;
    [Header("리스폰 지역")]
    [SerializeField] private GameObject _rewpawnPos;
    [Header("프레젠터 버튼")]
    [SerializeField] private Presenter _presenter;
    [SerializeField] GameObject _jarPos;

    private Vector3 moveDirection;
    private Vector2 input;
    private bool isBack;

    private PlayerInput _playerInput;
    private InputActionMap _playerMap;
    private InputAction _moveAction;
    private InputAction _HoldAction;
    private Rigidbody _rigidbody;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();

        _playerInput = GetComponent<PlayerInput>();
        _playerMap = _playerInput.actions.FindActionMap("Player");
        _moveAction = _playerMap.FindAction("Move");
        _HoldAction = _playerMap.FindAction("Hold");
    }

    private void OnEnable()
    {
        _playerMap.Enable();

        _moveAction.performed += OnMove;
        _moveAction.canceled += OnMove;
        _HoldAction.performed += OnInteraction;
    }

    private void OnDisable()
    {
        _moveAction.performed -= OnMove;
        _moveAction.canceled -= OnMove;
        _HoldAction.performed -= OnInteraction;

        _playerMap.Disable();
    }

    private void FixedUpdate()
    {

        if (isBack == true)
        {
            if (_rigidbody.linearVelocity.magnitude < 0.1f)
            {
                isBack = false;
                _rigidbody.angularVelocity = Vector3.zero;
                _moveAction.Enable();
                Debug.Log("입력 활성화");
            }
            return;
        }

        if (input == Vector2.zero)
        {
            return;
        }

        moveDirection = new Vector3(input.x, 0f, input.y);

        _rigidbody.MoveRotation(Quaternion.Slerp(
            _rigidbody.rotation,
            Quaternion.LookRotation(moveDirection),
            _rotateSpeed * Time.fixedDeltaTime
            ));

        //_rigidbody.MovePosition(_rigidbody.position + _rigidbody.transform.forward * _moveSpeed * Time.deltaTime);
        _rigidbody.AddForce(_rigidbody.transform.forward * _moveSpeed * Time.deltaTime, ForceMode.Impulse);
    }

    private void OnMove(InputAction.CallbackContext ctx)
    {
        if (ctx.performed)
        {
            input = ctx.ReadValue<Vector2>();
        }
        else if (ctx.canceled)
        {
            input = Vector2.zero;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            isBack = true;
            Debug.Log("입력 비활성화");
            BackMove();
        }
    }

    private void BackMove()
    {
        Vector3 backward = -transform.forward;
        _rigidbody.AddForce(backward * _backForce, ForceMode.Impulse);
    }
}
