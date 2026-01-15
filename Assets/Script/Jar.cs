using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem.LowLevel;

public class Jar : MonoBehaviour
{
    [Header("내구도")]
    [SerializeField] private int _maxHp = 5;
    [Header("용량(ml)")]
    [SerializeField] private float _maxWaterLv = 5000f;
    private int _currentHP;
    private float _currentWaterLv;
    private Presenter _presenter;
    private bool _isLeakTime;

    public static event Action<GameObject> OnDestroyJar;
    public static event Action<Jar> OnWaterLV;

    public float MaxWaterLv => _maxWaterLv;
    public float CurrentWaterLv => _currentWaterLv;

    private void Awake()
    {
        _maxHp = 5;
        _maxWaterLv = 5000f;
        _currentHP = _maxHp;
        _currentWaterLv = 0;
        _presenter = GameObject.Find("Canvas").GetComponent<Presenter>();
    }

    private void FixedUpdate()
    {
        if (_isLeakTime == true)
        {
            LeakWater();
        }

    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Floor" || collision.gameObject.tag == "Hurdle")
        {
            DestroyJar(collision.gameObject);
            _currentHP -= 1;
            Debug.Log($"항아리 현재 내구도 {_currentHP}/{_maxHp}");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Respawn")
        {
            _currentHP = 0;
            DestroyJar(other.gameObject);
        }
    }

    private void DestroyJar(GameObject gameObject)
    {
        if (_currentHP <= 1)
        {
            Destroy(gameObject);
            OnDestroyJar.Invoke(gameObject);
        }
    }



    public void LeakWater()
    {
        if (_currentWaterLv >= 1000f)
        {
            _currentWaterLv -= 4000f * Time.deltaTime;
        }
    }

    IEnumerator LeakWaterTimer(bool isPressed)
    {
        yield return new WaitForSeconds(2f);
        _isLeakTime = isPressed;
    }

    public void FillWater(bool isPressed)
    {
        if (isPressed == true)
        {
            if (_currentWaterLv == _maxWaterLv)
            {
                Debug.Log($"항아리 물 {_currentWaterLv}/{_maxWaterLv}");
                return;
            }
            _currentWaterLv += 5000f * Time.deltaTime;
            Debug.Log($"항아리 물 {_currentWaterLv}/{_maxWaterLv}");
            OnWaterLV.Invoke(this);

            //물샘 없음으로
            _isLeakTime = false;
        }
        //물 공급 중단되면 물새기 타이머 시작
        else if (isPressed == false)
        {
            StartCoroutine(LeakWaterTimer(true));
        }
    }
}
