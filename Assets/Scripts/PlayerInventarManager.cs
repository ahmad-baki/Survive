using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerInventarManager : InventarManager
{
    public GameObject rightHand;
    public GameObject leftHand;
    public UnityEvent<EquipmentItem> onEquipChange;

    EquipmentItem mainHandEquip;
    EquipmentItem offHandEquip;


    public GameObject MainHandEquipGO
    {
        get { return rightHand.transform.GetChild(0).gameObject; }
    }

    public GameObject OffHandEquipGO
    {
        get { return leftHand.transform.GetChild(0).gameObject; }
    }

    public EquipmentItem MainHandEquip
    {
        get { return mainHandEquip; }
        set { Equip(value, BodyPart.RightHand); }
    }

    public EquipmentItem OffHandEquip
    {
        get { return offHandEquip; }
        set { Equip(value, BodyPart.LeftHand); }
    }


    // Start is called before the first frame update
    protected new void Start()
    {
        base.Start();
        onEquipChange.AddListener(SpawnItem);
    }

    void SpawnItem(EquipmentItem equipment) {
        GameObject spawnTarget;
        switch (equipment.equipType)
        {
            case EquipType.MainHand:
                spawnTarget = rightHand;
                break;
            case EquipType.OffHand:
                spawnTarget = leftHand;
                break;
            case EquipType.BothHand:
                spawnTarget = rightHand;
                break;
            default:
                return;
        }
        if (equipment.prefab is null) return;

        Instantiate(equipment.prefab, spawnTarget.transform);
    }


    public void Equip(EquipmentItem equipment, BodyPart prefTarget)
    {
        switch (equipment.equipType)
        {
            case EquipType.MainHand:
                mainHandEquip = equipment;
                break;
            case EquipType.OffHand:
                offHandEquip = equipment;
                break;
            case EquipType.BothHand:
                mainHandEquip = equipment;
                offHandEquip = equipment;
                break;
            default:
                return;
        }

        onEquipChange?.Invoke(equipment);
    }
}
