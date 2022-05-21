using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerInventarManager : InventarManager
{
    public GameObject rightHand;
    public GameObject leftHand;
    // the default
    public GameObject bareHandPrefab;
    public UnityEvent<EquipmentItem, BodyPart> onEquip;
    public UnityEvent<BodyPart> onUnEquip;


    EquipmentItem rightHandEquip;
    EquipmentItem leftHandEquip;


    public GameObject MainHandEquipGO
    {
        get
        {
            if (rightHand.transform.childCount > 0) return rightHand.transform.GetChild(0).gameObject;
            return null;
        }
    }

    public GameObject OffHandEquipGO
    {
        get { return leftHand.transform.GetChild(0).gameObject; }
    }

    public EquipmentItem MainHandEquip
    {
        get { return rightHandEquip; }
        set { Equip(value, BodyPart.RightHand); }
    }

    public EquipmentItem OffHandEquip
    {
        get { return leftHandEquip; }
        set { Equip(value, BodyPart.LeftHand); }
    }


    // Start is called before the first frame update
    protected new void Start()
    {
        base.Start();
        onEquip.AddListener(SpawnEquipment);
    }

    void SpawnEquipment(EquipmentItem equipment, BodyPart bodyPart) {
        GameObject spawnTarget;
        switch (bodyPart)
        {
            case BodyPart.RightHand:
                spawnTarget = rightHand;
                break;
            case BodyPart.LeftHand:
                spawnTarget = leftHand;
                break;
            case BodyPart.BothHand:
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
        BodyPart target = prefTarget;
        switch (equipment.equipType)
        { 
            case EquipType.MainHand:
                UnEquip(BodyPart.RightHand);
                target = BodyPart.RightHand;
                rightHandEquip = equipment;
                break;
            case EquipType.OffHand:
                UnEquip(BodyPart.LeftHand);
                target = BodyPart.LeftHand;
                leftHandEquip = equipment;
                break;
            case EquipType.BothHand:
                UnEquip(BodyPart.RightHand);
                UnEquip(BodyPart.LeftHand);
                target = BodyPart.BothHand;
                rightHandEquip = equipment;
                leftHandEquip = equipment;
                break;
            case EquipType.SingleHand:
                switch (prefTarget)
                {
                    case BodyPart.LeftHand:
                        UnEquip(BodyPart.LeftHand);
                        leftHandEquip = equipment;
                        break;
                    case BodyPart.RightHand:
                        UnEquip(BodyPart.RightHand);
                        rightHandEquip = equipment;
                        break;
                    default:
                        return;
                }
                break;
            default:
                return;
        }
        onEquip?.Invoke(equipment, target);
    }


    public void UnEquip(BodyPart bodyPart)
    {
        switch (bodyPart)
        {
            case BodyPart.RightHand:
                foreach (Transform childTrans in rightHand.transform)
                {
                    Destroy(childTrans.gameObject);
                }
                break;
            case BodyPart.LeftHand:
                foreach (Transform childTrans in leftHand.transform)
                {
                    Destroy(childTrans.gameObject);
                }
                break;

        }
    }

    public void UnEquip(EquipmentItem equipmentItem)
    {
        if (equipmentItem == null) return;
        if (rightHandEquip.Equals(equipmentItem))
        {
            foreach(Transform childTrans in rightHand.transform)
            {
                Destroy(childTrans.gameObject);
            }
            Instantiate(bareHandPrefab, rightHand.transform);
            rightHandEquip = null;
            onUnEquip?.Invoke(BodyPart.RightHand);
        }
        if (leftHandEquip == equipmentItem)
        {
            foreach (Transform childTrans in leftHand.transform)
            {
                Destroy(childTrans.gameObject);
            }
            Instantiate(bareHandPrefab, leftHand.transform);
            leftHandEquip = null;
            onUnEquip?.Invoke(BodyPart.LeftHand);
        }
    }

    public bool IsEquiped(EquipmentItem equipmentItem)
    {
        if(equipmentItem == null) return false;
        if (rightHandEquip == equipmentItem) return true;
        if(leftHandEquip == equipmentItem) return true;
        return false;
    }
}
