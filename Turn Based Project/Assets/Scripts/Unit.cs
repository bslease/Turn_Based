using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class Unit : MonoBehaviourPun
{
    public int curHp;
    public int maxHp;
    public float moveSpeed;
    public int minDamage;
    public int maxDamage;

    public int maxMoveDistance;
    public int maxAttackDistance;

    public bool usedThisTurn;

    public GameObject selectedVisual;
    public SpriteRenderer spriteVisual;

    [Header("UI")]
    public Image healthFillImage;

    [Header("Sprite Variants")]
    public Sprite leftPlayerSprite;
    public Sprite rightPlayerSprite;

    public bool CanSelect()
    {
        if (usedThisTurn)
            return false;
        else
            return true;
    }

    // can we move to this position?
    public bool CanMove(Vector3 movePos)
    {
        if (Vector3.Distance(transform.position, movePos) <= maxMoveDistance)
            return true;
        else
            return false;
    }

    // can we attack this position?
    public bool CanAttack(Vector3 attackPos)
    {
        if (Vector3.Distance(transform.position, attackPos) <= maxAttackDistance)
            return true;
        else
            return false;
    }

    // called when we either select or deselect the unit
    public void ToggleSelect(bool selected)
    {
        selectedVisual.SetActive(selected);
    }

    public void Move(Vector3 targetPos)
    {
        usedThisTurn = true;

        //rotate the sprite
        Vector3 dir = (transform.position - targetPos).normalized;
        spriteVisual.transform.up = dir;

        StartCoroutine(MoveOverTime());

        IEnumerator MoveOverTime()
        {
            while (transform.position != targetPos)
            {
                transform.position = Vector3.MoveTowards(transform.position, targetPos, moveSpeed * Time.deltaTime);
                yield return null;
            }
        }
    }

    [PunRPC]
    void TakeDamage(int damage)
    {
        curHp -= damage;

        if (curHp <= 0)
            photonView.RPC("Die", RpcTarget.All);
        else
        {
            // update the health UI
            photonView.RPC("UpdateHealthBar", RpcTarget.All, (float)curHp / (float)maxHp);
        }
    }

    [PunRPC]
    void Die()
    {

    }

    [PunRPC]
    void UpdateHealthBar(float fillAmount)
    {
        healthFillImage.fillAmount = fillAmount;
    }

    [PunRPC]
    public void Attack(Unit unitToAttack)
    {
        usedThisTurn = true;
    }

    [PunRPC]
    void Initialize(bool isMine)
    {
        if (isMine)
            PlayerController.me.units.Add(this);
        else
            GameManager.instance.GetOtherPlayer(PlayerController.me).units.Add(this);

        healthFillImage.fillAmount = 1.0f;

        // set sprite variant
        spriteVisual.sprite = transform.position.x < 0 ? leftPlayerSprite : rightPlayerSprite;

        // rotate the unit
        spriteVisual.transform.up = transform.position.x < 0 ? Vector3.left : Vector3.right;
    }
}
