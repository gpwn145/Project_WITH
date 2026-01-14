using UnityEngine;

public class Jar : MonoBehaviour
{
    [SerializeField] private int _maxHp = 5;
    [SerializeField] private float _maxWaterLv = 5f;
    private int _currentHP;
    private float _currentWaterLv;
    private bool _isLeakStart;

    private void Awake()
    {
        _maxHp = 5;
        _maxWaterLv = 5f;
        _currentHP = _maxHp;
        _currentWaterLv = 0;
        _isLeakStart = false;
    }

    private void FixedUpdate()
    {
        
    }

    private void OnCollisionEnter(Collision collision)
    {
        DestroyJar();
        _currentHP -= 1;
        Debug.Log($"항아리 현재 내구도 {_currentHP}/{_maxHp}");
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Water")
        {
            if (_currentWaterLv == _maxWaterLv)
            {
                Debug.Log($"항아리 물 {_currentWaterLv}/{_maxWaterLv}");
                _isLeakStart = true;
                return;
            }
            _currentWaterLv += 1f * Time.deltaTime;
            Debug.Log($"항아리 물 {_currentWaterLv}/{_maxWaterLv}");
        }
    }

    private void DestroyJar()
    {
        if (_currentHP <= 0)
        {
            gameObject.SetActive(false);
        }
    }

    private void LeakWater()
    {
        
    }
}
