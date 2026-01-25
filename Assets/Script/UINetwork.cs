using UnityEngine;
using Photon.Pun;

public partial class GameSceneManager : MonoBehaviourPunCallbacks
{
    //실시간 우물 수위 > 다같이

    public void CurrentGoalWaterLv(float water, float golaWater)
    {
        if (PhotonNetwork.IsMasterClient == false)
        {
            return;
        }
        photonView.RPC(
                nameof(RPC_CurrentGoalWaterLv),
                RpcTarget.All,
                water,
                golaWater
                );
    }

    [PunRPC]
    private void RPC_CurrentGoalWaterLv(float water, float golaWater)
    {
        int persent = Mathf.RoundToInt((water / golaWater) * 100);
        presenter.view.CurrentWellLV(persent);
        Debug.Log("우물 물 갱신");
    }

    //실시간 부서진 항아리 개수 > 다같이
    public void CurrentDstroyJar(int brokenJar)
    {
        if (PhotonNetwork.IsMasterClient == false)
        {
            return;
        }

        photonView.RPC(
                nameof(RPC_CurrentDstroyJar),
                RpcTarget.All,
                brokenJar
                );
    }

    [PunRPC]
    private void RPC_CurrentDstroyJar(int brokenJar)
    {
        presenter.view.UpdateJar(brokenJar);
    }

    [PunRPC]
    private void GClearStage()
    {
        presenter.ClearStage();
    }
}
