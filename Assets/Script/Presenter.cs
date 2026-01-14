using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class Presenter : MonoBehaviour
{
    [Header("항아리 모델")]
    [SerializeField] private Jar _jarScript;
    [Header("플레이어 모델")]
    [SerializeField] private Player _player;
    [Header("뷰")]
    [SerializeField] private InGameView _view;

    GameObject _targetJar;
    public bool IsFillStart { get; set; }

    private void Start()
    {
        Player.OnGrab += WhoGrap;
    }

    private void FixedUpdate()
    {
        if (_targetJar != null)
        {
            _jarScript = _targetJar.GetComponent<Jar>();
            _jarScript.FillWater(IsFillStart);
        }
    }

    private void WhoGrap(Player player)
    {
        _targetJar = player.Hand;
    }
}
