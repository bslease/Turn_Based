using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class GameManager : MonoBehaviourPun
{
    public PlayerController leftPlayer;
    public PlayerController rightPlayer;

    public PlayerController curPlayer;

    public float postGameTime;
    
    // instance
    public static GameManager instance;
    void Awake()
    {
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        // the master client will set the players
        if (PhotonNetwork.IsMasterClient)
            SetPlayers();
    }

   void SetPlayers()
    {
        // set the owners of the two player's photon views
        leftPlayer.photonView.TransferOwnership(1);
        rightPlayer.photonView.TransferOwnership(2);

        // initialize the players
        leftPlayer.photonView.RPC("Initialize", RpcTarget.AllBuffered, PhotonNetwork.CurrentRoom.GetPlayer(1));
        rightPlayer.photonView.RPC("Initialize", RpcTarget.AllBuffered, PhotonNetwork.CurrentRoom.GetPlayer(2));

        photonView.RPC("SetNextTurn", RpcTarget.AllBuffered);
    }

    [PunRPC]
    void SetNextTurn()
    {
        if (curPlayer == null)
            curPlayer = leftPlayer;
        else
            curPlayer = curPlayer == leftPlayer ? rightPlayer : leftPlayer;

        if (curPlayer == PlayerController.me)
            PlayerController.me.BeginTurn();

        // toggle the end turn button
        GameUI.instance.ToggleEndTurnButton(curPlayer == PlayerController.me);
    }

    public PlayerController GetOtherPlayer(PlayerController player)
    {
        return player == leftPlayer ? rightPlayer : leftPlayer;
    }

    public void CheckWinCondition()
    {
        if (PlayerController.me.units.Count == 0)
        {
            photonView.RPC("WinGame", RpcTarget.All, PlayerController.enemy == leftPlayer ? 0 : 1);
        }
    }

    [PunRPC]
    void WinGame(int winner)
    {
        PlayerController player = winner == 0 ? leftPlayer : rightPlayer;
        GameUI.instance.SetWinText(player.photonPlayer.NickName);
        Invoke("GoBackToMenu", postGameTime);
    }

    void GoBackToMenu()
    {
        PhotonNetwork.LeaveRoom();
        NetworkManager.instance.ChangeScene("Menu");
    }
}
