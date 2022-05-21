using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[CreateAssetMenu(fileName = "Equipment", menuName = "ScriptableObjects/Equipment")]
public class EquipmentItem : Item
{
    public EquipType equipType;

    public override void Use()
    {
        base.Use();
        PlayerInventarManager playerInventarManager = UIInventarManager.Singelton.playerInventarManager;
        if (playerInventarManager.IsEquiped(this))
        {
            playerInventarManager.UnEquip(this);
        }
        else
        {
            playerInventarManager.Equip(this, BodyPart.Null);
        }
    }
}
