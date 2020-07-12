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
            unit.usedThisTurn = false;

        // update the UI
    }

    void Update()
    {
        if (!photonView.IsMine)
            return;

        if (Input.GetMouseButtonDown(0) && GameManager.instance.curPlayer == this)
        {
            Vector3 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            TrySelect(new Vector3(Mathf.RoundToInt(pos.x), Mathf.RoundToInt(pos.y), 0));
        }
    }

    void TrySelect(Vector3 selectPos)
    {
        // are we selecting our unit?
        Unit unit = units.Find(x => x.transform.position == selectPos);

        if (unit != null)
        {
            SelectUnit(unit);
            return;
        }

        if (!selectedUnit)
            return;

        // are we selecting an enemy unit?
        Unit enemyUnit = enemy.units.Find(x => x.transform.position == selectPos);

        if (enemyUnit != null)
        {
            TryAttack(enemyUnit);
            return;
        }

        TryMove(selectPos);
    }

    void SelectUnit(Unit unitToSelect)
    {
        // can we select the unit
        if (!unitToSelect.CanSelect())
            return;

        // un-select the current unit
        if (selectedUnit != null)
            selectedUnit.ToggleSelect(false);

        // select the new unit
        selectedUnit = unitToSelect;
        selectedUnit.ToggleSelect(true);

        // set the unit info text
    }

    void DeSelectUnit()
    {
        selectedUnit.ToggleSelect(false);
        selectedUnit = null;

        // disable the unit info text
    }

    void SelectNextAvailableUnit()
    {
        Unit availableUnit = units.Find(x => x.CanSelect());

        if (availableUnit != null)
            SelectUnit(availableUnit);
        else
            DeSelectUnit();
    }

    void TryAttack(Unit enemyUnit)
    {
        if (selectedUnit.CanAttack(enemyUnit.transform.position))
        {
            selectedUnit.Attack(enemyUnit);
            SelectNextAvailableUnit();

            // update the UI
        }
    }

    void TryMove(Vector3 movePos)
    {
        if (selectedUnit.CanMove(movePos))
        {
            selectedUnit.Move(movePos);
            SelectNextAvailableUnit();

            // update the UI
        }
    }

    public void EndTurn()
    {
        // de-select the unit
        if (selectedUnit != null)
            DeSelectUnit();

        // start the next turn
        GameManager.instance.photonView.RPC("SetNextTurn", RpcTarget.All);
    }
}
