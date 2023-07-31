using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;

using Photon.Pun;

public class UIManager : MonoBehaviourPunCallbacks
{
    #region Public Variables

    public static UIManager Instance;

    #endregion

    #region Private Variables

    [SerializeField] private Text nameText;
    [SerializeField] private Text roomText;

    #endregion

    #region MonoBehavior Methods

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        if (PhotonNetwork.IsConnected)
        {
            string hostString = PhotonNetwork.IsMasterClient ? " (Host)" : "";
            nameText.text = "Name: " + PhotonNetwork.NickName + hostString;
            roomText.text = "Room: " + PhotonNetwork.CurrentRoom.Name;
        }
    }

    #endregion
}
