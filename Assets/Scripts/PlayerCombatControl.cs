using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlayerCombatControl : MonoBehaviour
{
    public PlayerCombatState playerCombatState;
    public bool isAttacking;
    public bool AttackingEnabled = true;
    public float attackTime;


    PlayerInventarManager _playerInventarManager;
    Animator _animator;
    float _attackStartTime;


    // Start is called before the first frame update
    void Start()
    {
        _animator = GetComponent<Animator>();
        _playerInventarManager = GetComponent<PlayerInventarManager>();
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
                if (!(_playerInventarManager.MainHandEquipGO is var weaponGO)) return;
                Weapon weapon = weaponGO.GetComponent<Weapon>();
                weapon.StartAttack();
                _attackStartTime = Time.time;
                isAttacking = true;
                _animator.SetTrigger("StartAttack");
            }
        }
        else if (Time.time > _attackStartTime + attackTime)
        {
            isAttacking = false;
            Weapon weapon = _playerInventarManager.MainHandEquipGO.GetComponent<Weapon>();
            weapon.EndAttack();
        }
    }

    public void OnWeaponChange(EquipmentItem equipmentItem)
    {
        switch (equipmentItem.itemTag)
        {
            case "Sword":
                _animator.SetTrigger("ToSword");
                break;
            case "Hand":
                _animator.SetTrigger("ToHand");
                break;
        }
    }
}