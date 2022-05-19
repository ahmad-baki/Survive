using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Equipment", menuName = "ScriptableObjects/Equipment")]
public class EquipmentItem : Item
{
    public EquipType equipType;

    public override void Use()
    {
        base.Use();

        UIInventarManager.Singelton.playerInventarManager.Equip(this, BodyPart.Null);
    }
}
