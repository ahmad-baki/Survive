using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class EquipSlot : MonoBehaviour, IDropHandler
{
    [SerializeField]
    BodyPart bodyPart;

    Image _image;
    TextMeshProUGUI tmpPro;

    private void Awake()
    {
        _image = GetComponent<Image>();
        tmpPro = GetComponent<TextMeshProUGUI>();
    }

    public void ChangeItem(Item item)
    {

        if (_image != null)
        {
            _image.sprite = item.icon;
        }
        if (tmpPro != null)
        {
            string itemName = item.itemTitle;
            tmpPro.text = (item.isStackable) ? item.ToString() : "";
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
                playerInventarManager.Equip(item as EquipmentItem, bodyPart);
            }
            else
            {
                Debug.LogWarning($"Referenced item at {itemSlotHover.index} isnt Equipment");
            }
            Destroy(eventData.pointerDrag);
        }
    }
}
