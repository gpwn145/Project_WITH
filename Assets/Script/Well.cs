using System;
using UnityEngine;

public class Well : MonoBehaviour
{
    [Header("우물 목표 L")]
    [SerializeField, Range(10f, 100f)] private float _goalWater = 10f;
    private float _currentWater = 0f;

    public event Action<float, float> OnAddWater;
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Jar")
        {
            Jar targetScript = other.gameObject.GetComponent<Jar>();
            if (targetScript == null)
            {
                Debug.Log($"Jar 스크립트 못찾음");
                return;
            }
            float target = targetScript.CurrentWaterLv;
            _currentWater += target;
            OnAddWater.Invoke(_currentWater, _goalWater);
        }
    }
}
