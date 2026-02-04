using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
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
    public bool _isOpen = false;

    public event Action<Vector3> OnStageSet;
    public event Action<PlayerScript> OnGrab;
    public event Action<Jar> OnGrabTargetJar;
    public event Action<Jar> OnGrabOrPut;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        SoundManager.Instance.SoundPlay(Sound.InGameBGM);

        StartCoroutine(WaitForPhotonAndInit());
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

        if (props.ContainsKey("SoundVolum") == true)
        {
            SoundManager.Instance.audioSource.volume = (float)props["SoundVolum"];
        }
    }

    private IEnumerator WaitForPhotonAndInit()
    {
        yield return new WaitUntil(() =>
            PhotonNetwork.InRoom &&
            GameManager.Instance != null
        );

        _gameManager = GameManager.Instance;
        Init();
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
        $"Player",
        repawnPos[_actorNum].transform.position,
        transform.rotation
        );
        player.name = $"Player{PhotonNetwork.LocalPlayer.ActorNumber - 1}";
        player.GetComponent<PlayerScript>().playerNum = _actorNum;

    }

    public void ReSpawnPlayer(GameObject player, GameObject jar)
    {
        SoundManager.Instance.SoundPlay(Sound.PlayerRespwan);
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
        _gameManager.UpdateStageClearInfo(
            stageMap.StageNumber, 
            presenter.view.rotateSlider.value,
            presenter.view.soundSlider.value
            );
        
        Debug.Log($"스테이지 클리어 : {stageMap.StageNumber}");
    }   

    public void Restart()
    {
        if (!PhotonNetwork.IsMasterClient) return;

        photonView.RPC(
                nameof(RPC_Restart),
                RpcTarget.All
                );
    }

    public void BeforeGotoRoom()
    {
        Debug.Log($"스테이지 정보 저장");
        SoundManager.Instance.isPlayBGM = false;
        _gameManager.SaveFirebase();
        _gameManager.MouseSpeed(presenter.view.rotateSlider.value);
    }

    [PunRPC]
    public void RPC_Restart()
    {
        //우물 초기화.
        presenter.wellModel.CurrentWater = 0;
        //깨진 항아리 초기화
        destroyJarNumber = 0;
        presenter.view.Init();

        PlayerScript myPlayer = null;

        PlayerScript[] players = FindObjectsByType<PlayerScript>(FindObjectsSortMode.None);
        foreach(PlayerScript player in players)
        {
            if(player.photonView.IsMine == true)
            {
                myPlayer = player;
                break;
            }
        }

        if(myPlayer != null)
        {
            myPlayer.Restert();
        }
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
