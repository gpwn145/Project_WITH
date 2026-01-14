using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public enum Stage
{
    normal = 0,
    ice,
    Slope
}

public class StageManager : MonoBehaviour
{
    [SerializeField] private List<GameObject> _stagePrefab = new List<GameObject>();
    private GameObject _choiceStage;

    public GameObject ChoiceStage => _choiceStage;
    

    //버튼
    public void OnStageChoice(int stage)
    {
        _choiceStage = _stagePrefab[0];

        switch (stage)
        {
            case (int)Stage.normal:
                //프리팹 설정
                Debug.Log($"노말 맵 선택");
                break;
            case (int)Stage.ice:
                Debug.Log($"얼음 맵 선택");
                break;
            case (int)Stage.Slope:
                Debug.Log($"경사 맵 선택");
                break;
        }

        _choiceStage = _stagePrefab[stage];
    }
}
