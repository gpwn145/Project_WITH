using Photon.Pun;
using UnityEngine;
using UnityEngine.InputSystem;

public class FollowCamera : MonoBehaviourPunCallbacks
{
    [SerializeField] private GameSceneManager gameSceneManager;
    private Transform _target;
    [SerializeField] public Vector3 offset = new Vector3(8, 20, -10);
    [Header("거리")]
    [SerializeField, Range(5f, 20f)] private float _distance = 5f;

    [Header("회전-감도")]
    private float _rotateSpeed;
    [SerializeField, Range(0.1f, 0.5f)] private float rotationSmoothTime = 0.1f;
    [SerializeField] private float rotate;

    [Header("높이")]
    [SerializeField] private float heightSmoothTime = 0.1f;
    [SerializeField, Range(5f, 10f)] private float _height = 10f;
    [SerializeField, Range(5f, 10f)] private float _maxHeight = 10f;
    [SerializeField, Range(0f, 5f)] private float _minHeight = 1f;
    private float heightVelocity;
    //[SerializeField] private Quaternion cameraRo = new Quaternion(50,-50,0,0);

    //public float RotateSpeed { get { return _rotateSpeed; } set { _rotateSpeed = value; } }

    private void Awake()
    {
        gameSceneManager.OnStageSet += CameraPos;
    }

    public void SpeedSet(float speed)
    {
        GameManager.Instance.MouseSpeed(speed);
        _rotateSpeed = GameManager.Instance.mouseSpeed;
    }

    public void CameraPos(Vector3 pos)
    {
        offset = pos;
        Debug.Log($"카메라 위치 설정");
    }

    private void LateUpdate()
    {
        if (null == _target)
        {
            return;
        }
        if (gameSceneManager._isOpen) return;

        Vector2 mouse = Mouse.current.delta.ReadValue();
        rotate += mouse.x * _rotateSpeed * 0.01f;

         _height -= mouse.y * _rotateSpeed * 0.01f;
        _height = Mathf.Clamp(_height, _minHeight, _maxHeight);

        Quaternion rotation = Quaternion.Euler(0f, rotate, 0f);
        Vector3 direction = rotation * Vector3.back;

        Vector3 targetPos = _target.position;
        Vector3 cameraPos = targetPos + direction * _distance + Vector3.up * _height;

        transform.position = cameraPos;
        transform.LookAt(_target);
    }

    public void SetTarget(Transform player)
    {
        _target = player;
        transform.position = offset;
    }
}
