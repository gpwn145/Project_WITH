using Photon.Pun;
using System.Collections.Generic;
using UnityEngine;

public class JarSpwaner : MonoBehaviourPunCallbacks
{
    [Header("게임씬매니저")]
    [SerializeField] private GameSceneManager _gameSceneManager;

    public bool hasJar = false;

    private void Awake()
    {
        _gameSceneManager = FindAnyObjectByType<GameSceneManager>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Jar")
        {
            hasJar = true;
            Debug.Log("항아리 있음");
        }
    }

    public void JarSetting()
    {
        if (!PhotonNetwork.IsMasterClient) return;

        GameObject jar = PhotonNetwork.Instantiate(
            "Jar",
            transform.position,
            transform.rotation
            );
        Debug.Log("항아리 생성");

        Jar jarScript = jar.GetComponent<Jar>();
        System.Random random = new System.Random();

        jarScript.MaxWaterLv = random.Next(2, 8);
        jarScript.jarSpwaner = this;
        jarScript.jarSpwaner.hasJar = true;

        //for (int i = 0; i < jarSpawnPos.Count; i++)
        //{
        //    GameObject jar = PhotonNetwork.Instantiate(
        //    "Jar",
        //    jarSpawnPos[i].transform.position,
        //    transform.rotation
        //    );
        //    Debug.Log("항아리 생성");
        //}
    }

    //public void GJarTaken()
    //{
    //    if (PhotonNetwork.IsMasterClient == false) return;

    //    if (jarSpawnPos[i].GetComponent<JarSpwaner>().hasJar == true)
    //    {
    //        jarSpawnPos[i].GetComponent<JarSpwaner>().hasJar = false;
    //        JarSetting();
    //    }
    //}
}
