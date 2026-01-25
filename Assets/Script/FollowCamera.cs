using Photon.Pun;
using UnityEngine;

public class FollowCamera : MonoBehaviourPunCallbacks
{
    private Transform _target;
    [SerializeField] public Vector3 offset = new Vector3(8,20,-10);
    [SerializeField] public GameSceneManager gameSceneManager;
    //[SerializeField] private Quaternion cameraRo = new Quaternion(50,-50,0,0);

    private void Awake()
    {
        gameSceneManager.OnStageSet += CameraPos;
    }

    public void CameraPos(Vector3 pos)
    {
        offset = pos;
        Debug.Log($"카메라 위치 설정");
    }

    private void LateUpdate()
    {
        if(null == _target)
        {
            return;
        }
        //transform.rotation = cameraRo;
        transform.LookAt(_target);
    }

    public void SetTarget(Transform player)
    {
        _target = player;
        transform.position = offset;
    }
}
