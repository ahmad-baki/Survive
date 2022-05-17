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
    public Button dropButton;
    public Button abortDropButton;

    InventarManager _playerInventarManager;
    TextMeshProUGUI _extraTitle;
    Image _extraIcon;
    TextMeshProUGUI _extraDesc;
    TextMeshProUGUI _extraTag;
    bool _didDeactivateAttacking;

    private void Start()
    {
        _playerInventarManager = FindObjectOfType<InventarManager>();
        _playerInventarManager.onChangeSlot.AddListener(ReloadSlot);
        _playerInventarManager.onAddSlot.AddListener(AddSlot);
        _playerInventarManager.onRemoveSlot.AddListener(RemoveSlot);
        Cursor.lockState = CursorLockMode.Locked;

        foreach(Transform transform in extraInformation.GetComponentInChildren<Transform>())
        {
            switch (transform.name)
            {
                case "Title":
                    _extraTitle = transform.GetComponent<TextMeshProUGUI>();
                    break;
                case "Desc":
                    _extraDesc = transform.GetComponent<TextMeshProUGUI>();
                    break;
                case "Icon":
                    _extraIcon = transform.GetComponent<Image>();
                    break;
                case "Tag":
                    _extraTag = transform.GetComponent<TextMeshProUGUI>();
                    break;
            }
        }

        abortDropButton.onClick.AddListener(CloseDropPopUp);
    }


    private void Update()
    {
        if (inventarUI)
        {
            if (Input.GetKeyDown(KeyCode.Tab) || Input.GetKeyDown(KeyCode.I))
            {
                if (inventarUI.activeInHierarchy)
                {
                    inventarUI.SetActive(false);
                    extraInformation.SetActive(false);
                    Cursor.lockState = CursorLockMode.Locked;

                    PlayerCombatControl playerCombatControl = _playerInventarManager.GetComponent<PlayerCombatControl>();
                    if (_didDeactivateAttacking)
                    {
                        playerCombatControl.AttackingEnabled = true;
                        _didDeactivateAttacking = false;
                    }
                }
                else
                {
                    inventarUI.SetActive(true);
                    Cursor.lockState = CursorLockMode.None;

                    PlayerCombatControl playerCombatControl = _playerInventarManager.GetComponent<PlayerCombatControl>();
                    if (playerCombatControl.AttackingEnabled)
                    {
                        playerCombatControl.AttackingEnabled = false;
                        _didDeactivateAttacking = true;
                    }
                }
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

        List<Item> inventar = _playerInventarManager.inventar;
        Dictionary<string, int> itemCounter = _playerInventarManager.itemCounter;
        foreach (Transform slotTrans in itemSlots.transform)
        {
            if(slotTrans.name == $"Slot{index}")
            {
                //if (index >= inventar.Count || inventar[index] is null)
                //{
                //    Destroy(slotTrans.gameObject);
                //    return;
                //}
                //else
                //{
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
                //}
            }
        }
    }

    public void AddSlot(int index)
    {
        List<Item> inventar = _playerInventarManager.inventar;
        Dictionary<string, int> itemCounter = _playerInventarManager.itemCounter;
        if (index + 1 > itemSlots.transform.childCount)
        {
            for (int i = itemSlots.transform.childCount; i < index + 1; i++)
            {
                GameObject slot = Instantiate(slotPrefab, itemSlots.transform);
                slot.GetComponent<ItemSlot>().index = index;
                slot.name = $"Slot{i}";
                Image image = slot.GetComponentInChildren<Image>();
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


    public void OnClickedItem(int index, PointerEventData eventData)
    {
        List<Item> inventar = _playerInventarManager.inventar;
        if (inventar.Count <= index) return;

        _extraTitle.text = inventar[index].itemTitle.ToString();
        _extraDesc.text = inventar[index].description.ToString();
        _extraTag.text = inventar[index].itemTag.ToString();
        _extraIcon.sprite = inventar[index].icon;
        extraDropButton.onClick.AddListener(() => OpenDropPopUp(index));
        extraInformation.SetActive(true);
    }

    public void OpenDropPopUp(int index)
    {
        TMP_InputField inputField = dropPopUp.GetComponentInChildren<TMP_InputField>();
        dropButton.onClick.AddListener(() => {
            if (int.TryParse(inputField.text, out int amount) && amount > 0)
            {
                _playerInventarManager.DropItem(index, amount);
                CloseDropPopUp();
            }
        });

        inputField.onEndEdit.AddListener((string text) =>
        {
            int maxAmount = _playerInventarManager.GetAmount(index);
            int inputAmount;
            if(Int32.TryParse(text, out inputAmount))
            {
                if(inputAmount < 0)
                {
                    inputField.text = "0";
                }
                else if(inputAmount > maxAmount)
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