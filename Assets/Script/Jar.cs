using System;
using UnityEngine;

public class Jar : MonoBehaviour
{
    [Header("내구도")]
    [SerializeField] private int _maxHp = 5;
    [Header("용량(ml)")]
    [SerializeField] private float _maxWaterLv = 5000f;
    private int _currentHP;
    private float _currentWaterLv;

    public static event Action OnDestroyJar;
    public event Action<Jar> OnWaterLV;

    public float MaxWaterLv => _maxWaterLv;
    public float CurrentWaterLv => _currentWaterLv;




    private void Awake()
    {
        _maxHp = 5;
        _maxWaterLv = 5000f;
        _currentHP = _maxHp;
        _currentWaterLv = 0;
    }

    private void FixedUpdate()
    {
        LeakWater();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag != "Player")
        {
            DestroyJar();
            _currentHP -= 1;
            Debug.Log($"항아리 현재 내구도 {_currentHP}/{_maxHp}");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Respawn")
        {
            _currentHP = 0;
            DestroyJar();
        }
    }

    private void DestroyJar()
    {
        if (_currentHP <= 1)
        {
            Destroy(gameObject);
            OnDestroyJar.Invoke();
        }
    }

    private void LeakWater()
    {
        if (_currentWaterLv >= 1000f )
        {
            _currentWaterLv -= 4000f * Time.deltaTime;
        }
    }

    public void FillWater(bool isPressed)
    {
        if (isPressed)
        {
            if (_currentWaterLv == _maxWaterLv)
            {
                Debug.Log($"항아리 물 {_currentWaterLv}/{_maxWaterLv}");
                return;
            }
            _currentWaterLv += 5000f * Time.deltaTime;
            Debug.Log($"항아리 물 {_currentWaterLv}/{_maxWaterLv}");
            OnWaterLV.Invoke(this);

        }
    }
}
