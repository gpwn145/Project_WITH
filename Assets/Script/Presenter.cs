using Photon.Pun;
using System;
using UnityEngine;

public class Presenter : MonoBehaviour
{
    [Header("물버튼 오브젝트")]
    [SerializeField] public WaterButton _waterButton;

    [Header("게임매니저")]
    [SerializeField] public GameSceneManager _gameSceneManager;

    //private PlayerScript _player;
    [Header("뷰")]
    [SerializeField] public InGameView view;

    [Header("우물모델")]
    [SerializeField] public Well wellModel;

    [Header("클리어 판넬")]
    [SerializeField] private GameObject _clearPanel;

    [Header("메뉴")]
    [SerializeField] public GameObject menuPanel;


    private Jar _jarScript;
    private GameObject _targetJar;
    private GameObject _stageObject;

    private void Awake()
    {
        if(_gameSceneManager == null)
        {
            Debug.Log("<color=red>게임씬매니저 비었음</color>");
            return;
        }
        _gameSceneManager.OnStageSet += PresenterStart;
        Debug.Log("OnStageSet 구독");
    }

    private void Start()
    {
        _gameSceneManager.OnGrab += WhoGrap;
        _gameSceneManager.OnGrabTargetJar += TargetJar;
        _gameSceneManager.OnGrabOrPut += InitJar;
    }

    private void PresenterStart(Vector3 unUsed)
    {
        Debug.Log("프레젠터 스타트 시작");
        _stageObject = _gameSceneManager.parant;

        GetScript();
        _clearPanel.SetActive(false);
        wellModel.OnAddWater += _gameSceneManager.CurrentGoalWaterLv;
    }

    private void GetScript()
    {
        GetWellModelScript();
        GetWaterButtonScript();
    }

    private void GetWellModelScript()
    {
        Transform wellObj = _stageObject.transform.Find("Well");
        if (wellObj == null)
        {
            Debug.Log("우물 오브젝트 없음");
            return;
        }

        Transform waterObj = wellObj.transform.Find("Water");
        if (waterObj == null)
        {
            Debug.Log("우물-물 오브젝트 없음");
            return;
        }

        wellModel = waterObj.GetComponent<Well>();
        if (waterObj == null)
        {
            Debug.Log("Well 스크립트 없음");
            return;
        }
        Debug.Log("우물 스크립트 가져옴");
    }

    private void GetWaterButtonScript()
    {
        Transform faucetObj = _stageObject.transform.Find("Faucet");
        if (faucetObj == null)
        {
            Debug.Log("수도 오브젝트 없음");
            return;
        }

        Transform buttonObj = faucetObj.transform.Find("FaucetButton");
        if (buttonObj == null)
        {
            Debug.Log("수도꼭지버튼 오브젝트 없음");
            return;
        }

        _waterButton = buttonObj.GetComponent<WaterButton>();
        if (wellModel == null)
        {
            Debug.Log("WaterButton 스크립트 없음");
            return;
        }
        Debug.Log("우물 스크립트 가져옴");
    }

    public void BindView(InGameView view)
    {
        this.view = view;
    }

    private void TargetJar(Jar jar)
    {
        if (_targetJar == null || _targetJar.GetComponent<Jar>() != jar)
            return;

        if (_jarScript != null)
            _jarScript.OnWaterLV -= CurrentJarWater;

        _jarScript = jar;
        _jarScript.OnWaterLV += CurrentJarWater;
    }

    private void WhoGrap(PlayerScript player)
    {
        if (!player.photonView.IsMine)
            return;

        _targetJar = player.Hand;
    }

    //현재 항아리 물 수위 > 각자 체크
    public void InitJar(Jar jar)
    {
        if (view == null || view.gameObject == null)
        {
            return;
        }
        if (jar == null || jar != _jarScript)
            return;

        view.UpdateJarWater(jar.CurrentWaterLv, jar.MaxWaterLv);
    }

    //실시간 항아리 물 수위 > 각자
    private void CurrentJarWater(Jar jar)
    {
        if(jar != null && jar == _jarScript)
        {
            view.UpdateJarWater(jar.CurrentWaterLv, jar.MaxWaterLv);
        }
    }

    public void JarTaken()
    {
        _gameSceneManager.GJarTaken();
        Debug.Log($"항아리 가져간거 매니저에 전달");
    }

    //스테이지 클리어 > 각자
    public void ClearStage()
    {
        _clearPanel.SetActive(true);
    }

    public void SetStageInfo(int stageNum, string stageName)
    {
        view.UpdateStage(stageNum, stageName);
    }
}
