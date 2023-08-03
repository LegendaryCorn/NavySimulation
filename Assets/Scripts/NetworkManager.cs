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
    public GameObject playerRoot;

    #endregion

    #region Private Variables

    [SerializeField] private PlayerManager playerManagerPrefab;
    [SerializeField] private PlayerCommand playerCommandPrefab;


    #endregion

    #region MonoBehavior Methods

    private void Awake()
    {
        Instance = this;

        if (PhotonNetwork.IsMasterClient) // and no playermanager exists
        {
            GameObject pM = PhotonNetwork.Instantiate(playerManagerPrefab.name, Vector3.zero, Quaternion.identity, 0);
        }

        if(true) // if I don't have an object already
        {
            GameObject pC = PhotonNetwork.Instantiate(playerCommandPrefab.name, Vector3.zero, Quaternion.identity, 0);
        }

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
