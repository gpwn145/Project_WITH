using Photon.Pun;
using System.IO;
using UnityEngine;

public partial class Jar : MonoBehaviourPunCallbacks, IPunObservable
{
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

        if(_isDestroyed == true)
        {
            return;
        }
        _presenter.wellModel.WellWaterPlus(_currentWaterLv);
        PhotonNetwork.Destroy(jar);
        _isDestroyed = true;
        Debug.Log("우물 - 항아리 제거");
    }

    public void Master_DestroyJar(GameObject jar)
    {
        if (_isDestroyed == true)
        {
            return;
        }

        if (PhotonNetwork.IsMasterClient == false) 
        {
            return;
        }
        _gameSceneManager.JarCout(jar);
        PhotonNetwork.Destroy(jar);
        _isDestroyed = true;
    }


    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (_rigid == null) return;

        if (stream.IsWriting)
        {
            stream.SendNext(_currentWaterLv);
            stream.SendNext(_rigid.position);
        }
        else // 클라이언트
        {
            float currentWaterLv = _currentWaterLv;
            _currentWaterLv = (float)stream.ReceiveNext();
            _rigid.position = (Vector3)stream.ReceiveNext();

            // 값이 실제로 바뀌었을 때만 UI 갱신
            if (!Mathf.Approximately(currentWaterLv, _currentWaterLv))
            {
                
                    OnWaterLV?.Invoke(this);
            }
        }
    }

}
