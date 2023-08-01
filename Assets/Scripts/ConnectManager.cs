using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Photon.Pun;
using Photon.Realtime;

public class ConnectManager : MonoBehaviourPunCallbacks
{
    #region Public Variables

    public static ConnectManager Instance;

    #endregion

    #region Private Variables

    [SerializeField] private byte maxPlayersPerRoom = 8;
    [SerializeField] private string version = "1";
    

    bool isHost = false;
    bool isConnecting = false;
    string roomName = "";

    #endregion

    #region MonoBehavior Methods

    private void Awake()
    {
        Instance = this;
    }

    #endregion

    #region Pun Methods

    public override void OnConnectedToMaster()
    {
        if (isConnecting)
        {
            if (isHost)
            {
                PhotonNetwork.CreateRoom(roomName, new RoomOptions { MaxPlayers = maxPlayersPerRoom, CleanupCacheOnLeave = false });
            }
            else
            {
                PhotonNetwork.JoinRoom(roomName);
            }
            isConnecting = false;
        }
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        MenuManager.Instance.SetPanel("home");
        Debug.LogWarningFormat("OnDisconnected was called with reason {0}", cause);
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        MenuManager.Instance.SetPanel("host");
        Debug.LogWarningFormat("Failed to create room.");
        PhotonNetwork.Disconnect();
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        MenuManager.Instance.SetPanel("join");
        Debug.LogWarningFormat("Failed to join room.");
        PhotonNetwork.Disconnect();
    }

    public override void OnJoinedRoom()
    {
        PhotonNetwork.LoadLevel("SampleScene");
    }

    #endregion

    #region Public Methods

    public void CreateRoom()
    {
        if(MenuManager.Instance.hostNameInputField.text.Equals("") || MenuManager.Instance.hostRoomInputField.text.Equals(""))
        {
            return;
        }

        PhotonNetwork.NickName = MenuManager.Instance.hostNameInputField.text;
        roomName = MenuManager.Instance.hostRoomInputField.text;
        MenuManager.Instance.SetPanel("connect");
        isHost = true;
        isConnecting = PhotonNetwork.ConnectUsingSettings();
        PhotonNetwork.GameVersion = version;
        
    }

    public void ConnectToRoom()
    {
        if (MenuManager.Instance.joinNameInputField.text.Equals("") || MenuManager.Instance.joinRoomInputField.text.Equals(""))
        {
            return;
        }

        PhotonNetwork.NickName = MenuManager.Instance.joinNameInputField.text;
        roomName = MenuManager.Instance.joinRoomInputField.text;
        MenuManager.Instance.SetPanel("connect");
        isHost = false;
        isConnecting = PhotonNetwork.ConnectUsingSettings();
        PhotonNetwork.GameVersion = version;
    }

    #endregion
}
