using System;
using UnityEngine;

public class Well : MonoBehaviour
{
    [Header("우물 목표 L")]
    [SerializeField, Range(5f, 100f)] private float _goalWater = 5f;
    [Header("우물 목표 L")]
    [SerializeField, Range(0f, 10f)] private float _currentWater = 0f;
    private float _plusWater;
    private GameSceneManager _gameSceneManager;

    public float CurrentWater { get { return _currentWater; } set { _currentWater = value; } }
    public event Action<float, float> OnAddWater;

    private void Start()
    {
        _gameSceneManager = FindAnyObjectByType<GameSceneManager>();
    }
    public void WellWaterPlus(float jarWater)
    {
        SoundManager.Instance.SoundPlay(Sound.WellWaterFill);
        _currentWater += jarWater;
        OnAddWater.Invoke(_currentWater, _goalWater);
        Debug.Log($"우물 물 량 추가 / 현재: {_currentWater}");
        if (_currentWater >= _goalWater)
        {
            SatisfyGoal();
        }
    }

    public void SatisfyGoal()
    {
        _gameSceneManager.StageClear();
    }
}
