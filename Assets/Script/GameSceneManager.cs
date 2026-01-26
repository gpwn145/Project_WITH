using Photon.Pun;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public partial class GameSceneManager : MonoBehaviourPunCallbacks
{
    [Header("항아리 스폰지역")]
    [SerializeField] public List<JarSpwaner> jarSpawnScript = new List<JarSpwaner>();
    //항아리 콜라이더
    private List<Collider> jarSpawnCollider = new List<Collider>();

    [Header("리스폰 지역")]
    [SerializeField] public GameObject[] repawnPos = new GameObject[4];

    [Header("스테이지 프리팹")]
    [SerializeField] private List<GameObject> _stagePrefabs = new List<GameObject>();


    [Header("프레젠터")]
    [SerializeField] public Presenter presenter;

    //게임에 존재하는 플레이어 리스트-필요함..
    private List<GameObject> IngamePlayerList = new List<GameObject>();

    //저장 정보 불러오기
    private GameManager _gameManager;
    public bool _isSceneChanging = false;

    //한 스테이지 저장 변수
    public int destroyJarNumber;
    private int _actorNum;
    public GameObject parant;
    public StageMap stageMap;

    //스테이지 정보 변수
    private string roomID;
    private int joinPlayerNum;

    public event Action<Vector3> OnStageSet;
    public event Action<PlayerScript> OnGrab;
    public event Action<Jar> OnGrabTargetJar;
    public event Action<Jar> OnGrabOrPut;

    private void Awake()
    {
        _gameManager = GameManager.Instance;
    }

    private void Start()
    {
        Init();
    }

    public void Init()
    {
        var props = PhotonNetwork.CurrentRoom.CustomProperties;
        GameManager.Instance.choiceStage = (int)props["ChoiceStage"];
        stageMap = new StageMap(GameManager.Instance.choiceStage);

        GetStage();
        GetSpawnPoint();
        GetJarSpawnPoint();
        for(int i = 0; i < jarSpawnScript.Count; i++)
        {
            jarSpawnScript[i].JarSetting();
        }
        PlayerSpawn();
    }

    private void GetStage()
    {
        GameObject currentStage = Instantiate(_stagePrefabs[stageMap.StageNumber], new Vector3(0, 0, 0), transform.rotation);
        currentStage.name = stageMap.StageName;

        Debug.Log($"{stageMap.StageName} 스테이지 불러옴");

        currentStage.SetActive(true);
        parant = GameObject.Find($"{stageMap.StageName}");
        presenter.SetStageInfo(stageMap.StageNumber + 1, currentStage.name);
        OnStageSet?.Invoke(stageMap.StageCamera);
    }

    private void GetSpawnPoint()
    {
        if (parant.transform.Find($"SpwanPoint0") != null)
        {
            for (int i = 0; i < 4; i++)
            {
                repawnPos[i] = parant.transform.Find($"SpwanPoint{i}").gameObject;
                Debug.Log("플레이어 리스폰 불러오기");
            }
        }
    }
    

    private void PlayerSpawn()
    {
        _actorNum = PhotonNetwork.LocalPlayer.ActorNumber - 1;
        GameObject player = PhotonNetwork.Instantiate(
        "Player",
        repawnPos[_actorNum].transform.position,
        transform.rotation
        );
        IngamePlayerList.Add(player);
        player.GetComponent<PlayerScript>().playerNum = _actorNum;
    }


    public void ReSpawnPlayer(GameObject player, GameObject jar)
    {
        player.transform.position = repawnPos[_actorNum].transform.position;
        if (jar != null)
        {
            jar.GetComponent<Jar>().Master_DestroyJar(jar);
        }
    }

    public void GetJarSpawnPoint()
    {
        if (PhotonNetwork.IsMasterClient == true)
        {
            jarSpawnScript.Clear();
            jarSpawnCollider.Clear();

            for (int i = 0; i < stageMap.JarSpawnPosNumber; i++)
            {
                if (parant == null)
                {
                    Debug.Log($"스테이지 오브젝트 못찾음");
                    return;
                }

                if (parant.transform.Find($"JarSpawnPoint{i + 1}") == null)
                {
                    Debug.Log("항아리 스폰 포인트 못찾음");
                    return;
                }

                GameObject jarObject = parant.transform.Find($"JarSpawnPoint{i + 1}").gameObject;

                jarSpawnScript.Add(jarObject.GetComponent<JarSpwaner>());
                jarSpawnCollider.Add(jarSpawnScript[i].GetComponent<Collider>());

                Debug.Log("항아리 소환 포인트 읽어옴");
            }
        }
    }

    public void JarCout(GameObject jar)
    {
        destroyJarNumber += 1;
        CurrentDstroyJar(destroyJarNumber);
        Debug.Log($"내구도바닥 - 항아리 제거 {destroyJarNumber} 개");
    }

    public void StageClear()
    {
        //스테이지 클리어는 같이
        photonView.RPC(
                nameof(GClearStage),
                RpcTarget.All
                );

        stageMap.StageNumber++;
        _gameManager.UpdateStageClearInfo(stageMap.StageNumber);
        
        Debug.Log($"스테이지 클리어 : {stageMap.StageNumber}");
    }   

    public void Restart()
    {
        //우물 초기화.
        presenter.wellModel.CurrentWater = 0;
        //깨진 항아리 초기화
        destroyJarNumber = 0;
        //플레이어 위치 초기화
        for (int i = 0; i < IngamePlayerList.Count; i++)
        {
            IngamePlayerList[i].GetComponent<PlayerScript>().Restert();
        }
        presenter.view.Init();

        //시간 초기화
    }
    public void NextStage()
    {
        if (!PhotonNetwork.IsMasterClient) return;

        switch(SceneManager.GetActiveScene().name)
        {
            case "GameScene":
                PhotonNetwork.LoadLevel("GameScene1");
                break;
            case "GameScene1":
                PhotonNetwork.LoadLevel("GameScene");
                break;
        }
        
    }

    public void GSGrab(PlayerScript playerScript)
    {
        OnGrab?.Invoke(playerScript);
    }
    public void GSGrabTargetJar(Jar jar)
    {
        OnGrabTargetJar?.Invoke(jar);
    }
    public void GSGrabOrPut(Jar jar)
    {
        OnGrabOrPut?.Invoke(jar);
    }
}
