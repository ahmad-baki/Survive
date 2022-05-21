using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class EquipSlot : MonoBehaviour, IDropHandler, IPointerDownHandler
{
    public Sprite defaultIcon;
    public BodyPart bodyPart;
    public EquipmentItem equipmentItem;

    Image _image;
    TextMeshProUGUI tmpPro;

    private void Awake()
    {
        _image = GetComponent<Image>();
        tmpPro = GetComponent<TextMeshProUGUI>();
    }

    public void ChangeEquipment(EquipmentItem equipItem)
    {
        if (_image != null)
        {
            _image.sprite = equipItem.icon;
        }
        if (tmpPro != null)
        {
            string itemName = equipItem.itemTitle;
            tmpPro.text = (equipItem.isStackable) ? equipItem.ToString() : "";
        }
    }

    public void Clear()
    {
        if (defaultIcon)
        {
            _image.sprite = defaultIcon;
        }
        else
        {
            _image.sprite = null;
        }
    }

    public void OnDrop(PointerEventData eventData)
    {
        ItemSlotHover itemSlotHover = eventData.pointerDrag.GetComponent<ItemSlotHover>();
        if (itemSlotHover is not null)
        {
            PlayerInventarManager playerInventarManager = UIInventarManager.Singelton.playerInventarManager;
            Item item = playerInventarManager.inventar[itemSlotHover.index];
            if (item is EquipmentItem) {
                EquipmentItem equipmentItem = (EquipmentItem)item;
                playerInventarManager.Equip(equipmentItem, bodyPart);
                this.equipmentItem = equipmentItem;
            }
            else
            {
                Debug.LogWarning($"Referenced item at {itemSlotHover.index} isnt Equipment");
            }
            Destroy(eventData.pointerDrag);
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        PlayerInventarManager playerInventarManager = UIInventarManager.Singelton.playerInventarManager;
        int index = playerInventarManager.inventar.IndexOf(equipmentItem);
        if(index == -1)
        {
            Debug.LogWarning($"Couldnt find item {equipmentItem.itemTitle} in inventar");
            return;
        }
        UIInventarManager.Singelton.OnClickedItem(index, eventData);
    }
}
