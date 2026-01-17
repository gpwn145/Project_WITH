using Photon.Pun;
using System.IO;
using UnityEngine;

public partial class Jar : MonoBehaviourPunCallbacks, IPunObservable
{
    private int _destroyJarNumber;

    [PunRPC]
    void RPC_JarState(JarState jarState)
    {
        _jarState = jarState;
    }

    void Master_GoalWater(GameObject jar)
    {
        if (PhotonNetwork.IsMasterClient == false)
        { 
            return;
        }
        GameManager.Instance.JarList.Remove(jar);
        PhotonNetwork.Destroy(jar);
        Debug.Log("우물 - 항아리 제거");
    }

    void Master_DestroyJar(GameObject jar)
    {
        if (PhotonNetwork.IsMasterClient == false) 
        {
            return;
        }
        _presenter.CurrentDstroyJar(++_destroyJarNumber);
        GameManager.Instance.JarList.Remove(jar);
        PhotonNetwork.Destroy(jar);
        Debug.Log("내구도바닥 - 항아리 제거");
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(_currentWaterLv);
            stream.SendNext(_destroyJarNumber);
        }
        else // 클라이언트
        {
            float currentWaterLv = _currentWaterLv;
            _currentWaterLv = (float)stream.ReceiveNext();
            _destroyJarNumber = (int)stream.ReceiveNext();

            // 값이 실제로 바뀌었을 때만 UI 갱신
            if (!Mathf.Approximately(currentWaterLv, _currentWaterLv))
            {
                OnWaterLV?.Invoke(this);
            }
        }
    }

}
