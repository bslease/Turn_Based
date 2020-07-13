using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class NetworkManager : MonoBehaviourPunCallbacks
{
    // instance
    public static NetworkManager instance;
    void Awake()
    {
        //instance = this;
        //DontDestroyOnLoad(gameObject);

        // Make this a proper singleton to avoid errors on returning to menu after game finish
        // if an instance already exists and it's not this one - destroy us
        if (instance != null && instance != this)
            gameObject.SetActive(false);
        else
        {
            // set the instance
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        // connect to the master server
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("Connected to server");
    }

    // joins a random room or creates a new room
    // called when user presses the play button
    public void CreateOrJoinRoom()
    {
        // if there are available rooms, join a random one
        if (PhotonNetwork.CountOfRooms > 0)
            PhotonNetwork.JoinRandomRoom();
        // otherwise, create a new room
        else
        {
            RoomOptions options = new RoomOptions();
            options.MaxPlayers = 2;
            PhotonNetwork.CreateRoom(null, options);
        }
    }

    // changes the scene using Photon's system
    [PunRPC]
    public void ChangeScene(string sceneName)
    {
        PhotonNetwork.LoadLevel(sceneName);
    }
}
