using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class Presenter : MonoBehaviour
{
    [Header("플레이어 모델")]
    [SerializeField] private PlayerScript _player;
    [Header("뷰")]
    [SerializeField] private InGameView _view;

    private Jar _jarScript;
    private GameObject _targetJar;
    public bool IsFillStart { get; set; }
    public Jar JarScript { get; set; }

    private void Start()
    {
        PlayerScript.OnGrab += WhoGrap;
    }

    private void FixedUpdate()
    {
        if (_targetJar != null)
        {
            _jarScript = _targetJar.GetComponent<Jar>();
            _jarScript.FillWater(IsFillStart);
        }
    }

    private void WhoGrap(PlayerScript player)
    {
        _targetJar = player.Hand;
    }
}
