using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class Presenter : MonoBehaviour
{
    [Header("물버튼 오브젝트")]
    [SerializeField] public WaterButton _waterButton;

    [Header("항아리 스폰 위치")]
    [SerializeField] private JarSpwaner _jarSpawner;

    //private PlayerScript _player;
    [Header("뷰")]
    [SerializeField] private InGameView _view;

    [Header("우물모델")]
    [SerializeField] public Well wellModel;

    private Jar _jarScript;
    private GameObject _targetJar;
    

    private void Start()
    {
        PlayerScript.OnGrab += WhoGrap;
        PlayerScript.OnGrabTargetJar += TargetJar;
        PlayerScript.OnGrabOrPut += InitJar;
        PlayerScript.OnGrabOrPut += CurrentJarWater;
        wellModel.OnAddWater += CurrentGoalWaterLv;
    }

    private void TargetJar(Jar jarScript)
    {
        _jarScript = jarScript;
        _jarScript.OnWaterLV += CurrentJarWater;
    }
    

    private void WhoGrap(PlayerScript player)
    {
        _targetJar = player.Hand;
    }

    public void InitJar(Jar jar)
    {
        _view.UpdateJarWater(0, jar.MaxWaterLv);
    }

    private void CurrentGoalWaterLv(float water, float golaWater)
    {
        int persent = Mathf.RoundToInt((water / golaWater) * 100);
        _view.CurrentWellLV(persent);
    }

    public void CurrentDstroyJar(int brokenJar)
    {
        _view.UpdateJar(brokenJar);
    }

    private void CurrentJarWater(Jar jar)
    {
        _view.UpdateJarWater(jar.CurrentWaterLv, jar.MaxWaterLv);
    }

    public void JarTaken()
    {
        if (_jarSpawner.hasJar == true)
        {
            _jarSpawner.hasJar = false;
            GameManager.Instance.JarSetting();
        }
    }
}
