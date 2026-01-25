using Photon.Pun;
using Photon.Pun.Demo.PunBasics;
using System;
using System.Collections;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

public partial class PlayerScript : MonoBehaviourPunCallbacks
{
    //공통
    [Header("이동 힘")]
    [SerializeField, Range(5f, 20f)] private float _moveSpeed = 10f;
    [Header("회전속도")]
    [SerializeField, Range(0f, 10f)] private float _rotateSpeed = 5f;
    [Header("내려놓는 거리")]
    [SerializeField, Range(0f, 10f)] private float _putPos = 1f;
    [Header("던지는 힘")]
    [SerializeField, Range(10f, 30f)] private float _throwForce = 10f;
    [Header("밀려나는 힘")]
    [SerializeField, Range(0f, 10f)] private float _backForce = 3f;
    [SerializeField] GameObject _jarPos;

    //개인
    [Header("플레이어 색")]
    [SerializeField] private string _colorCode = "#FF8686";
    private GameObject _rewpawnPos;
    public int playerNum;

    //상호작용
    private bool isBack;
    private bool isWaiting = false;
    private Rigidbody _rigidbody;

    //참조
    private Presenter _presenter;
    private GameSceneManager _gameSceneManager;

    //카메라
    private FollowCamera playerCamera;

    //조작
    private Vector3 moveDirection;
    private Vector2 input;
    private PlayerInput _playerInput;
    private InputActionMap _playerMap;
    private InputAction _moveAction;
    private InputAction _holdAction;
    private InputAction _thorwAction;
    private InputAction _waterButtonAction;
    private InputAction _ESCAction;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();

        _playerInput = GetComponent<PlayerInput>();
        _playerMap = _playerInput.actions.FindActionMap("Player");
        _moveAction = _playerMap.FindAction("Move");
        _holdAction = _playerMap.FindAction("Hold");
        _thorwAction = _playerMap.FindAction("Throw");
        _waterButtonAction = _playerMap.FindAction("Water");
        _ESCAction = _playerMap.FindAction("ESC");

        _presenter = GameObject.Find("InGameUI").GetComponent<Presenter>();
        _gameSceneManager = _presenter._gameSceneManager;
        _rewpawnPos = _presenter._gameSceneManager.repawnPos[0];
    }

    private void Start()
    {
        if (!photonView.IsMine) return;
        Renderer renderer = GetComponent<Renderer>();

        ColorUtility.TryParseHtmlString(_colorCode, out var color);
        renderer.material.color = color;
        gameObject.tag = "Player";

        playerCamera = FindAnyObjectByType<FollowCamera>();

        if (playerCamera != null)
        {
            Debug.Log("카메라 스크립트 찾음");
            playerCamera.SetTarget(this.transform);
        }
        else
        {
            Debug.Log("카메라 스크립트 못찾음");
        }
    }

    public override void OnEnable()
    {
        if(photonView.IsMine == true)
        {
            _playerInput.ActivateInput();
        }
        else
        {
            _playerInput.DeactivateInput();
        }

        _playerMap.Enable();

        _moveAction.performed += OnMove;
        _moveAction.canceled += OnMove;

        _holdAction.performed += OnHoldAction;

        _ESCAction.performed += OnMenu;

        _thorwAction.performed += OnThorowAction;

        _waterButtonAction.started += OnWaterButtonAction;
        _waterButtonAction.canceled += OnWaterButtonAction;
    }

    public override void OnDisable()
    {
        _moveAction.performed -= OnMove;
        _moveAction.canceled -= OnMove;

        _holdAction.performed -= OnHoldAction;

        _ESCAction.performed -= OnMenu;

        _thorwAction.performed -= OnThorowAction;

        _waterButtonAction.started -= OnWaterButtonAction;
        _waterButtonAction.canceled -= OnWaterButtonAction;

        _playerMap.Disable();
    }

    private void FixedUpdate()
    {
        if (!photonView.IsMine) return;

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

        Vector3 cameraForward = playerCamera.transform.forward;
        Vector3 cameraRight = playerCamera.transform.right;
        
        cameraForward.y = 0f;
        cameraRight.y = 0f;

        cameraForward.Normalize();
        cameraRight.Normalize();

        moveDirection =
            cameraRight * input.x +
            cameraForward * input.y
            ;

        if(moveDirection.sqrMagnitude > 0.0001f)
        {
            _rigidbody.MoveRotation(Quaternion.Slerp(
            _rigidbody.rotation,
            Quaternion.LookRotation(moveDirection),
            _rotateSpeed * Time.fixedDeltaTime
            ));
        }
        //_rigidbody.MovePosition(_rigidbody.position + _rigidbody.transform.forward * _moveSpeed * Time.deltaTime);
        _rigidbody.AddForce(moveDirection * _moveSpeed * Time.deltaTime, ForceMode.Impulse);
    }

    private void OnMenu(InputAction.CallbackContext ctx)
    {
        if (!photonView.IsMine) return;

        if (ctx.performed)
        {
            if(_presenter.menuPanel.activeSelf == true)
            { 
                _presenter.menuPanel.SetActive(false);
            }
            if(_presenter.menuPanel.activeSelf == false)
            { 
                _presenter.menuPanel.SetActive(true);
            }
        }
    }

    private void OnMove(InputAction.CallbackContext ctx)
    {
        if (!photonView.IsMine) return;

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
        if (!photonView.IsMine) return;

        Debug.Log($"플레이어 콜라이더 : {collision.gameObject.name}");

        if (collision.gameObject.tag == "Player" || collision.gameObject.tag == "Hurdle" || collision.gameObject.tag == "Jar")
        {
            if (collision.gameObject.layer == LAYER_JarPlayer)
            {
                return;
            }

            isBack = true;
            Debug.Log("입력 비활성화");
            if (isWaiting == true)
            {
                return;
            }

            BackMove(collision);
            StartCoroutine(WaitTime());

            if (gameObject.layer == LAYER_JarPlayer)
            {
                _hand.gameObject.GetComponent<Jar>().Damaged();
            }
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

    public void Restert()
    {
        if (!photonView.IsMine) return;
        _gameSceneManager.ReSpawnPlayer(gameObject, _hand);
    }
}
