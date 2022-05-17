using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum PlayerAttackType
{
    NormalRightPunch, NormalLeftPunch, NormalRightSlash, NormalLeftSlash, StrongRightSlash, StrongLeftSlash, StrongSlash
}

public class PlayerCombatControl : MonoBehaviour
{
    public PlayerCombatState playerCombatState;
    public bool isAttacking;
    public bool AttackingEnabled = true;
    public float attackTime;


    InventarManager _inventarManager;
    Animator _animator;
    float _attackStartTime;


    // Start is called before the first frame update
    void Start()
    {
        _animator = GetComponent<Animator>();
        _inventarManager = GetComponent<InventarManager>();
        //playerCombatState = PlayerCombatState.Hand;
    }

    // Update is called once per frame
    void Update()
    {
        Attack();
    }

    void Attack()
    {
        if (!isAttacking && AttackingEnabled)
        {
            if (Input.GetKeyDown(KeyCode.Mouse0))
            {
                if (!(_inventarManager.weaponGO is var weaponGO)) return;
                Weapon weapon = weaponGO.GetComponent<Weapon>();
                if (weapon.playerCombatState == PlayerCombatState.Hand)
                {
                    weapon.StartAttack();
                    _attackStartTime = Time.time;
                    isAttacking = true;
                    _animator.SetTrigger("PunchRight");
                }
            }
        }
        else if (Time.time > _attackStartTime + attackTime)
        {
            isAttacking = false;
            Weapon weapon = _inventarManager.weaponGO.GetComponent<Weapon>();
            weapon.EndAttack();
        }
    }
}