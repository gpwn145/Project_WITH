using NUnit.Framework;
using Photon.Pun;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviourPunCallbacks
{
    [Header("리스폰 지역")]
    [SerializeField] public GameObject rewpawnPos;
    [Header("플레이어 프리팹")]
    [SerializeField] public GameObject playerPrefab;
    [Header("항아리 프리팹")]
    [SerializeField] public GameObject jarPrefab;
    [Header("항아리 스폰지역")]
    [SerializeField] public GameObject jarSpawnPos;
    [Header("프레젠터")]
    [SerializeField] public Presenter presenter;

    public List<GameObject> JarList = new List<GameObject>();

    public static GameManager Instance;
    public int DestroyJarNumber {  get; private set; }

    private void Awake()
    {
        #region 싱글톤
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this);
        }
        else
        {
            Destroy(this);
        }
        #endregion
        Init();
    }

    private void Start()
    {
        Jar.OnDestroyJar += CountJar;
    }

    public void CountJar(GameObject gameObject)
    {
        DestroyJarNumber++;
        JarList.Remove(gameObject);
    }
    private void Init()
    {
        JarList.Clear();
        JarSetting();
        DestroyJarNumber = 0;
        //플레이어 소환
        GameObject player = PhotonNetwork.Instantiate(
            playerPrefab.name, 
            rewpawnPos.transform.position, 
            transform.rotation
            );

        //우물 초기화
    }

    private void JarSetting()
    {

        GameObject jar = PhotonNetwork.Instantiate(
            jarPrefab.name,
            jarSpawnPos.transform.position,
            transform.rotation
            );
        JarList.Add(jar);
    }
}
