using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIInventarManager : SzeneSingelton<UIInventarManager>
{
    public GameObject inventarUI;
    public GameObject itemSlots;
    public GameObject extraInformation;
    public Button extraUseButton;
    public Button extraDropButton;
    public GameObject slotPrefab;
    public GameObject dropPopUp;
    public Button popDropButton;
    public Button popAbortDropButton;
    public EquipSlot rightHandSlot;
    public EquipSlot leftHandSlot;
    public PlayerInventarManager playerInventarManager;

    public TextMeshProUGUI extraTitle;
    public Image extraIcon;
    public TextMeshProUGUI extraDesc;
    public TextMeshProUGUI extraTag;
    bool _didDeactivateAttacking;

    private void Start()
    {
        //playerInventarManager.onChangeSlot.AddListener(ReloadSlot);
        //playerInventarManager.onAddSlot.AddListener(AddSlot);
        //playerInventarManager.onRemoveSlot.AddListener(RemoveSlot);
        Cursor.lockState = CursorLockMode.Locked;

        foreach(Transform transform in extraInformation.GetComponentInChildren<Transform>())
        {
            switch (transform.name)
            {
                case "Title":
                    extraTitle = transform.GetComponent<TextMeshProUGUI>();
                    break;
                case "Desc":
                    extraDesc = transform.GetComponent<TextMeshProUGUI>();
                    break;
                case "Icon":
                    extraIcon = transform.GetComponent<Image>();
                    break;
                case "Tag":
                    extraTag = transform.GetComponent<TextMeshProUGUI>();
                    break;
            }
        }

        popAbortDropButton.onClick.AddListener(CloseDropPopUp);
    }


    private void Update()
    {
        if (inventarUI)
        {
            if (Input.GetKeyDown(KeyCode.Tab) || Input.GetKeyDown(KeyCode.I))
            {
                if (inventarUI.activeInHierarchy)
                {
                    CloseInventar();
                }
                else
                {
                    OpenInventar();
                }
            }
            else if (Input.GetKeyDown(KeyCode.Escape) && inventarUI.activeInHierarchy)
            {
                CloseInventar();
            }
        }
    }


    public void ReloadSlot(int index)
    {
        if (index < 0 || index >= itemSlots.transform.childCount)
        {
            Debug.LogWarning($"Tried to acces element {index} of inventar with {itemSlots.transform.childCount} items");
            return;
        }

        List<Item> inventar = playerInventarManager.inventar;
        Dictionary<string, int> itemCounter = playerInventarManager.itemCounter;
        foreach (Transform slotTrans in itemSlots.transform)
        {
            if(slotTrans.name == $"Slot{index}")
            {
                Image image = slotTrans.GetComponentInChildren<Image>();
                if (image != null)
                {
                    image.sprite = inventar[index].icon;
                }
                TextMeshProUGUI tmpPro = slotTrans.GetComponentInChildren<TextMeshProUGUI>();
                if (tmpPro != null)
                {
                    string itemName = inventar[index].itemTitle;
                    tmpPro.text = (inventar[index].isStackable) ? itemCounter[itemName].ToString() : "";
                }
            }
        }
    }

    public void AddSlot(int index)
    {
        List<Item> inventar = playerInventarManager.inventar;
        Dictionary<string, int> itemCounter = playerInventarManager.itemCounter;
        if (index + 1 > itemSlots.transform.childCount)
        {
            for (int i = itemSlots.transform.childCount; i < index + 1; i++)
            {
                GameObject slot = Instantiate(slotPrefab, itemSlots.transform);
                slot.GetComponent<ItemSlot>().index = index;
                slot.name = $"Slot{i}";
                Image image = null;
                foreach (Image img in slot.GetComponentsInChildren<Image>())
                {
                    if (!img.gameObject.Equals(slot))
                    {
                        image = img;
                        break;
                    }
                }
                if (image is null) return;

                if (image != null)
                {
                    image.sprite = inventar[i].icon;
                }
                TextMeshProUGUI tmpPro = slot.GetComponentInChildren<TextMeshProUGUI>();
                if (tmpPro != null)
                {
                    string itemTitle = inventar[i].itemTitle;
                    tmpPro.text = (inventar[i].isStackable) ? itemCounter[itemTitle].ToString() : "";
                }
            }
            return;
        }
    }

    public void RemoveSlot(int index) 
    {
        if (index < 0 || index >= itemSlots.transform.childCount)
        {
            Debug.LogWarning($"Tried to remove element {index} of inventar with { itemSlots.transform.childCount} items");
            return;
        }

        foreach (Transform child in itemSlots.transform)
        {
            ItemSlot itemSlot = child.GetComponent<ItemSlot>();
            if (itemSlot is not null)
            {
                if (itemSlot.index == index)
                {
                    Destroy(child.gameObject);
                }
                else if (itemSlot.index > index)
                {
                    itemSlot.index--;
                }
            }
        }
    }

    public void ReloadEquipSlot(EquipmentItem equipment)
    {
        switch (equipment.equipType)
        {
            case EquipType.MainHand:
                rightHandSlot.ChangeItem(equipment);
                break;
            case EquipType.OffHand:
                leftHandSlot.ChangeItem(equipment);
                break;
            case EquipType.BothHand:
                rightHandSlot.ChangeItem(equipment);
                leftHandSlot.ChangeItem(equipment);
                break;
            default:
                return;
        }
    }

    public void OnClickedItem(int index, PointerEventData eventData)
    {
        int count = playerInventarManager.inventar.Count;
        if (index >= count || index < 0)
        {
            Debug.LogWarning($"Cant acces element {index} of inventar with size {count}");
        }

        Item item = playerInventarManager.inventar[index];
        extraTitle.text = item.itemTitle;
        extraDesc.text = item.description;
        extraTag.text = item.itemTag;
        extraIcon.sprite = item.icon;
        extraUseButton.GetComponentInChildren<TextMeshProUGUI>().text = item.useTitle;
        extraUseButton.onClick.AddListener(item.Use);
        extraDropButton.onClick.AddListener(() => OpenDropPopUp(index));
        extraInformation.SetActive(true);
    }

    public void CloseInventar() {
        inventarUI.SetActive(false);
        extraInformation.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;

        PlayerCombatControl playerCombatControl = playerInventarManager.GetComponent<PlayerCombatControl>();
        if (_didDeactivateAttacking)
        {
            playerCombatControl.AttackingEnabled = true;
            _didDeactivateAttacking = false;
        }
    }

    public void OpenInventar() {
        inventarUI.SetActive(true);
        Cursor.lockState = CursorLockMode.None;

        PlayerCombatControl playerCombatControl = playerInventarManager.GetComponent<PlayerCombatControl>();
        if (playerCombatControl.AttackingEnabled)
        {
            playerCombatControl.AttackingEnabled = false;
            _didDeactivateAttacking = true;
        }
    }

    public void OpenDropPopUp(int index)
    {
        TMP_InputField inputField = dropPopUp.GetComponentInChildren<TMP_InputField>();
        popDropButton.onClick.AddListener(() => {
            if (int.TryParse(inputField.text, out int amount) && amount > 0)
            {
                playerInventarManager.DropItem(index, amount);
                CloseDropPopUp();
            }
        });

        inputField.onEndEdit.AddListener((string text) =>
        {
            int maxAmount = playerInventarManager.GetAmount(index);
            int inputAmount;
            if (Int32.TryParse(text, out inputAmount))
            {
                if (inputAmount < 0)
                {
                    inputField.text = "0";
                }
                else if (inputAmount > maxAmount)
                {
                    inputField.text = maxAmount.ToString();
                }
            }
            else
            {
                inputField.text = "1";
            }
        });

        dropPopUp.SetActive(true);
    }

    public void CloseDropPopUp()
    {
        dropPopUp.SetActive(false);
    }
}