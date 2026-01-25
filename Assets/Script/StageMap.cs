using System.Collections.Generic;
using UnityEngine;

//public enum Stage
//{
//    normal = 0,
//    ice,
//    Slope
//}

public class StageMap
{
    private int _stageNumber;
    private string _stageName;
    private int _jarSpawnPosNumber;
    private Vector3 _stageCamera;
    private GameObject _stagePrefab;

    public int StageNumber { get { return _stageNumber; } set { _stageNumber = value; } }
    public string StageName => _stageName;
    public int JarSpawnPosNumber => _jarSpawnPosNumber;
    public Vector3 StageCamera => _stageCamera;
    public GameObject StagePrefab => _stagePrefab;

    public StageMap(int stageNum)
    {
        OnStageChoice(stageNum);
}
    //버튼
    public void OnStageChoice(int choiceStage)
    {
        _stageNumber = choiceStage;

        switch (choiceStage)
        {
            case 0:
                //프리팹 설정
                _jarSpawnPosNumber = 1;
                _stageName = "Stage1";
                _stageCamera = new Vector3(8, 20, -10);
                Debug.Log($"스테이지1 설정");
                break;
            case 1:
                _jarSpawnPosNumber = 2;
                _stageName = "Stage2";
                _stageCamera = new Vector3(-4, 15, 25);
                Debug.Log($"스테이지2 설정");
                break;
            case 2:
                _jarSpawnPosNumber = 1;
                _stageName = "TestMap";
                _stageCamera = new Vector3(2, 13, 0);
                Debug.Log($"테스트맵 설정");
                break;
        }
    }
}
