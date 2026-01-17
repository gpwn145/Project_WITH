using NUnit.Framework;
using Photon.Pun;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviourPunCallbacks
{
    [Header("리스폰 지역")]
    [SerializeField] public GameObject reSpawnPos;
    [Header("플레이어 프리팹")]
    [SerializeField] public GameObject playerPrefab;
    [Header("항아리 프리팹")]
    [SerializeField] public GameObject jarPrefab;
    [Header("항아리 스폰지역")]
    [SerializeField] public GameObject jarSpawnPos;
    [Header("프레젠터")]
    [SerializeField] public Presenter presenter;

    public List<GameObject> JarList = new List<GameObject>();
    public List<GameObject> IngamePlayerList = new List<GameObject>();
    private Collider jarSpawnCollider;

    public static GameManager Instance;

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
        jarSpawnCollider = jarSpawnPos.GetComponent<Collider>();
    }

    private void Init()
    {
        JarList.Clear();
        JarSetting();
        //플레이어 소환
        GameObject player = PhotonNetwork.Instantiate(
            playerPrefab.name, 
            reSpawnPos.transform.position, 
            transform.rotation
            );
        IngamePlayerList.Add(player);

        //우물 초기화
    }

    public void JarSetting()
    {
        if (!PhotonNetwork.IsMasterClient) return;

        GameObject jar = PhotonNetwork.Instantiate(
            jarPrefab.name,
            jarSpawnPos.transform.position,
            transform.rotation
            );
        JarList.Add(jar);
    }

    public void ReSpawnPlayer(GameObject player)
    {
        player.transform.position = reSpawnPos.transform.position;
    }
}
