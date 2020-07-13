using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameUI : MonoBehaviour
{
    public Button endTurnButton;
    public TextMeshProUGUI leftPlayerText;
    public TextMeshProUGUI rightPlayerText;
    public TextMeshProUGUI waitingUnitsText;
    public TextMeshProUGUI unitInfoText;
    public TextMeshProUGUI winText;
    
    // instance
    public static GameUI instance;
    void Awake()
    {
        instance = this;
    }

    public void OnEndTurnButton()
    {
        PlayerController.me.EndTurn();
    }

    public void ToggleEndTurnButton(bool toggle)
    {
        endTurnButton.interactable = toggle;
        waitingUnitsText.gameObject.SetActive(toggle);
    }

    public void UpdateWaitingUnitsText(int waitingUnits)
    {
        waitingUnitsText.text = waitingUnits + " Units Waiting";
    }

    public void SetPlayerText(PlayerController player)
    {
        TextMeshProUGUI text = player == GameManager.instance.leftPlayer ? leftPlayerText : rightPlayerText;
        text.text = player.photonPlayer.NickName;
    }

    public void SetUnitInfoText(Unit unit)
    {
        unitInfoText.gameObject.SetActive(true);
        unitInfoText.text = "";
        unitInfoText.text += string.Format("<b>Hp:</b> {0} / {1}", unit.curHp, unit.maxHp);
        unitInfoText.text += string.Format("\n<b>Damage:</b> {0} - {1}", unit.minDamage, unit.maxDamage);
        unitInfoText.text += string.Format("\n<b>Move Range:</b> {0}", unit.maxMoveDistance);
        unitInfoText.text += string.Format("\n<b>Attack Range:</b> {0}", unit.maxAttackDistance);
    }

    public void SetWinText(string winnerName)
    {
        winText.gameObject.SetActive(true);
        winText.text = winnerName + " Wins!";
    }
}
