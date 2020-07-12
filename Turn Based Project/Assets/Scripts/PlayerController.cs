using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class PlayerController : MonoBehaviourPun
{
    public Player photonPlayer;
    public string[] unitsToSpawn;
    public Transform[] spawnPoints;

    public List<Unit> units = new List<Unit>();
    private Unit selectedUnit;

    public static PlayerController me;
    public static PlayerController enemy;

    [PunRPC]
    void Initialize(Player player)
    {
        photonPlayer = player;

        // if this is our local player, spawn the units
        if (player.IsLocal)
        {
            me = this;
            SpawnUnits();
        }
        else
        {
            enemy = this;
        }

        // set the UI player text
    }

    void SpawnUnits()
    {
        for (int x = 0; x < unitsToSpawn.Length; ++x)
        {
            GameObject unit = PhotonNetwork.Instantiate(unitsToSpawn[x], spawnPoints[x].position, Quaternion.identity);
            unit.GetPhotonView().RPC("Initialize", RpcTarget.Others, false);
            unit.GetPhotonView().RPC("Initialize", photonPlayer, true); // could we just: unit.Initialize(true)?
        }
    }

    public void BeginTurn()
    {
        foreach (Unit unit in units)
            unit.usedThisTurn = true;

        // update the UI
    }
}
