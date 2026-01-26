using System;
using System.Collections;
using UnityEngine;
using Photon.Pun;

public enum JarState
{
    None,
    WaterFilling,
    LeakTime
}

public partial class Jar : MonoBehaviourPunCallbacks, IPunObservable
{
    [Header("내구도")]
    [SerializeField] private int _maxHp = 5;
    [Header("용량(L)")]
    [SerializeField] private float _maxWaterLv = 5f;
    [Header("상태")]
    [SerializeField] public JarState _jarState = JarState.None;
    private Presenter _presenter;
    private GameSceneManager _gameSceneManager;
    private int _currentHP;
    [Header("현재 물")]
    [SerializeField, Range(00f, 5f)] private float _currentWaterLv;

    //private bool _isLeakTime = false;
    //private bool _isWater = false;
    private bool _isGodMode;
    //private bool _isCoroutineStart = false;
    private Rigidbody _rigid;
    private Coroutine prograssCor;
    private bool _isDestroyed = false;
    public JarSpwaner jarSpwaner;

    const int LAYER_JarSpawn = 6;
    const int LAYER_JarPlayer = 7;
    const int LAYER_JarPutDown = 8;

    public event Action<Jar> OnWaterLV;

    public float MaxWaterLv => _maxWaterLv;
    public float CurrentWaterLv => _currentWaterLv;
    public int MaxHp => _maxHp;

    private void Awake()
    {
        _maxHp = 5;
        _maxWaterLv = 5f;
        _currentHP = _maxHp;
        _currentWaterLv = 0;
        gameObject.tag = "Jar";
        gameObject.layer = LAYER_JarSpawn;
        GodMode(true);
        _rigid = GetComponent<Rigidbody>();
        gameObject.GetComponent<PhotonView>().TransferOwnership(PhotonNetwork.MasterClient);
        //_presenter = GameObject.Find("InGameUI").GetComponent<Presenter>();
    }

    private void Start()
    {
        _presenter = GameObject.Find("InGameUI").GetComponent<Presenter>();
        _gameSceneManager = _presenter._gameSceneManager;
        PlayerScript.OnFillStop += FillStop;
    }

    private void Update()
    {
        if (PhotonNetwork.IsMasterClient == true)
        {
            if (_jarState == JarState.LeakTime)
            {
                LeakWater();
            }

            if (_jarState == JarState.WaterFilling)
            {
                FillWater();
            }
        }
    }

    public void FillStop()
    {
        if (_isDestroyed) return;
        if (_gameSceneManager._isSceneChanging) return;

        SendStateRPC(JarState.None);
    }

    public void SendStateRPC(JarState state)
    {
        if (_gameSceneManager._isSceneChanging) return;
        if (_isDestroyed) return;
        if (photonView == null) return;
        if (PhotonNetwork.IsMasterClient == false)
        {
            return;
        }

        _jarState = state;

        photonView.RPC(
                    nameof(RPC_JarState),
                    RpcTarget.Others,
                    state
                    );
    }

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log($"항아리 콜라이더 : {collision.gameObject.name}");

        if (collision.gameObject.tag == "Floor" || collision.gameObject.tag == "Hurdle" || collision.gameObject.tag == "Player")
        {
            if (collision.gameObject.layer == LAYER_JarPlayer)
            {
                return;
            }

            Debug.Log($"항아리 무적상태 : {_isGodMode}");
            Debug.Log($"항아리 레이어상태 : {gameObject.layer}");
            //바닥에 내려져 있을 때, 무적이 아닐때
            if (gameObject.layer == LAYER_JarPutDown && _isGodMode == false)
            {
                Damaged();
            }
        }
    }

    public void Damaged()
    {
        if (PhotonNetwork.IsMasterClient == false)
        {
            return;
        }

        _currentHP -= 1;

        if (_currentHP < 1)
        {
            Master_DestroyJar(gameObject);
            return;
        }

        Debug.Log($"항아리 현재 내구도 {_currentHP}/{_maxHp}");
        GodMode(true);
        StartCoroutine(JarGodModeTimer());
        Debug.Log("무적타이머 시작");
    }

    public void GodMode(bool isGad)
    {
        _isGodMode = isGad;
        Debug.Log($"항아리 무적상태 : {_isGodMode} 설정");
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log($"항아리 트리거 : {other.gameObject.name}");

        if (other.gameObject.tag == "Out")
        {
            Debug.Log("항아리 파괴");
            Master_DestroyJar(gameObject);
            return;
        }
        else if (other.gameObject.tag == "Goal")
        {
            Master_GoalWater(gameObject);
            Debug.Log("우물에 던짐");
            return;
        }

        if (other.gameObject.tag == "Water")
        {
            if (PhotonNetwork.IsMasterClient == false)
                return;

            if (prograssCor != null)
            {
                StopCoroutine(prograssCor);
            }
            SendStateRPC(JarState.WaterFilling);
            Debug.Log("물받기 시작");
        }
    }



    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Water")
        {
            if (PhotonNetwork.IsMasterClient == false)
                return;

            if (_jarState == JarState.WaterFilling)
            {
                SendStateRPC(JarState.None);
                Debug.Log("물받기 중단");

                if (prograssCor != null)
                {
                    StopCoroutine(prograssCor);
                }
                prograssCor = StartCoroutine(LeakWaterTimer());
            }
        }
    }

    IEnumerator LeakWaterTimer()
    {
        yield return new WaitForSeconds(2f);
        SendStateRPC(JarState.LeakTime);
        Debug.Log("물새기 시작");
        prograssCor = null;
    }

    public IEnumerator JarGodModeTimer()
    {
        yield return new WaitForSeconds(1f);
        GodMode(false);
    }

    public void FillWater()
    {
        if (PhotonNetwork.IsMasterClient == false)
            return;

        if (gameObject.layer != LAYER_JarPlayer)
            return;

        if (_currentWaterLv >= _maxWaterLv)
        {
            Debug.Log($"항아리 물 {_currentWaterLv}/{_maxWaterLv}");
            return;
        }
        _currentWaterLv += 1f * Time.deltaTime;
        _currentWaterLv = Mathf.Clamp(_currentWaterLv, 0f, _maxWaterLv);
        OnWaterLV?.Invoke(this);
    }
    public void LeakWater()
    {
        if (PhotonNetwork.IsMasterClient == false)
            return;

        _currentWaterLv -= 1f * Time.deltaTime;
        _currentWaterLv = Mathf.Clamp(_currentWaterLv, 0f, _maxWaterLv);

        if (_currentWaterLv <= 1f)
        {
            SendStateRPC(JarState.None);
        }
    }

}
