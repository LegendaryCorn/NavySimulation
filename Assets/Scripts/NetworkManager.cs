using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.SceneManagement;

using Photon.Pun;
using Photon.Realtime;

public class NetworkManager : MonoBehaviourPunCallbacks
{
    #region Public Variables

    public static NetworkManager Instance;

    #endregion

    #region Private Variables


    #endregion

    #region MonoBehavior Methods

    private void Awake()
    {
        Instance = this;
    }

    #endregion

    #region Pun Methods

    public override void OnPlayerEnteredRoom(Player other)
    {
        Debug.LogFormat("Player {0} has entered the room.", other.NickName);
    }

    public override void OnPlayerLeftRoom(Player other)
    {
        Debug.LogFormat("Player {0} has left the room.", other.NickName);

        if (PhotonNetwork.IsMasterClient)
        {
            LeaveRoom();
        }
    }

    public override void OnLeftRoom()
    {
        SceneManager.LoadScene("ConnectScene");
    }

    #endregion

    #region Public Methods

    public void LeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
        PhotonNetwork.Disconnect();
    }

    #endregion
}
