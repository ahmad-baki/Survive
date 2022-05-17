using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class InventarManager : MonoBehaviour
{
    public GameObject rightHand;
    public GameObject leftHand;
    public GameObject weaponGO;
    public int maxItemAmount;
    public List<Item> inventar;
    public Dictionary<string, int> itemCounter;
    public UnityEvent<int> onChangeSlot;
    public UnityEvent<int> onRemoveSlot;
    public UnityEvent<int> onAddSlot;

    // Start is called before the first frame update
    void Awake()
    {
        if (onChangeSlot is null)
        {
            onChangeSlot = new();
        }

        inventar = new();
        itemCounter = new();
    }

    
    public bool AddItem(Item addItem, int count = 1)
    {
        if(!addItem.isStackable && count > 1)
        {
            Debug.LogWarning($"Tried to add {count} of {addItem.name} to inventar, despite them being non-stackable");
            return false;
        }

        if (itemCounter.ContainsKey(addItem.itemTitle))
        {
            itemCounter[addItem.itemTitle] += count;

            if (addItem.isStackable) {
                for(int i = 0; i < inventar.Count; i++)
                {
                    if(inventar[i].itemTitle == addItem.itemTitle)
                    {
                        onChangeSlot?.Invoke(i);
                        return true;
                    }
                }
                itemCounter[addItem.itemTitle] = count;
                Debug.LogWarning($"itemCounter contains {addItem.name}, despite it being not in the inventar");
            }
        }
        else
        {
            itemCounter.Add(addItem.itemTitle, count);
        }
        inventar.Add(addItem);
        onAddSlot?.Invoke(inventar.Count - 1);
        return true;


        //if (itemCounter.ContainsKey(addItem.itemName))
        //{
        //    itemCounter[addItem.itemName] += count;
        //    for (int i = 0; i < inventar.Count; i++)
        //    {
        //        if (inventar[i].itemName == addItem.itemName)
        //        {
        //            onChangeSlot?.Invoke(i);
        //            return true;
        //        }

        //        itemCounter.Remove(addItem.itemName);
        //        Debug.LogWarning($"itemCounter contains {addItem.name}, despite it being not in the inventar, removed item from itemCounter and proceeded normaly");
        //    }
        //}
        //else
        //{
        //    for (int i = 0; i < inventar.Count; i++)
        //    {
        //        if (inventar[i] is null)
        //        {
        //            inventar[i] = addItem;

        //            if (addItem.isStackable)
        //            {
        //                itemCounter.Add(addItem.itemName, count);
        //            }
        //            else if (itemCounter.ContainsKey(addItem.itemName))
        //            {
        //                itemCounter[addItem.itemName] += count;
        //            }
        //            else
        //            {
        //                itemCounter.Add(addItem.itemName, count);
        //            }
        //            onChangeSlot?.Invoke(i);
        //            return true;
        //        }
        //    }
        //    Debug.LogWarning($"Cant add {addItem.itemName}, becouse inventar is full");
        //}
        //return false;
    }

    public bool RemoveItem(Item remItem, int count = 1)
    {
        for (int i = 0; i < inventar.Count; i++)
        {
            if(inventar[i].itemTitle == remItem.itemTitle)
            {
                RemoveItem(i, count);
            }
        }

        Debug.LogWarning($"Cant remove {remItem.itemTitle}, becouse item cant be found in inventar");
        if(itemCounter.ContainsKey(remItem.itemTitle)) itemCounter.Remove(remItem.itemTitle);
        return false;
    }

    public bool RemoveItem(int index, int count = 1)
    {
        if(index >= inventar.Count)
        {
            Debug.LogWarning($"Out of bounds: tried to acces element {count} of inventar, despite it having a size of {inventar.Count}");
            return false;
        }

        Item item = inventar[index];
        if (!item.isStackable && count > 1)
        {
            Debug.LogWarning($"Tried to remove {count} of {item.name} to inventar, despite them being non-stackable");
            return false;
        }

        if (item.isStackable)
        {
            if (itemCounter.ContainsKey(item.itemTitle))
            {
                if (itemCounter[item.itemTitle] < count)
                {
                    Debug.LogWarning($"Cant remove {count} Elements of {item.itemTitle}, becouse inventar only has {itemCounter[item.itemTitle]}");
                    return false;
                }

                itemCounter[item.itemTitle] -= count;
                if (itemCounter[item.itemTitle] == 0)
                {
                    inventar.RemoveAt(index);
                    itemCounter.Remove(item.itemTitle);
                    onRemoveSlot?.Invoke(index);
                }
                else
                {
                    onChangeSlot?.Invoke(index);
                }
                return true;
            }

            else
            {
                Debug.LogWarning($"itemCounter doesnt contain, for whatever reason, {item.itemTitle}, so it was removed from inventar");
                inventar.RemoveAt(index);
                onRemoveSlot?.Invoke(index);
                return false;
            }
        }
        else
        {
            itemCounter[item.itemTitle]--;
            inventar.RemoveAt(index);
            onRemoveSlot?.Invoke(index);
            return true;
        }
    }

    public bool DropItem(int index, int count = 1)
    {
        GameObject prefab = inventar[index].Prefab;
        if(prefab is null) return false;
        if (!RemoveItem(index, count)) return false;

        GameObject dropedItem = Instantiate(prefab, transform.position, Quaternion.identity);
        dropedItem.name = prefab.name;
        if(dropedItem.GetComponent<Pickable>() is var pickable)
        {
            pickable.amount = count;
        }
        else
        {
            Debug.LogWarning($"{dropedItem.name} doesnt have Pickable-Component");
        }
        return true;
    }

    public int GetAmount(int index)
    {
        if(index < 0 || index >= inventar.Count)
        {
            Debug.LogWarning($"Out of bounds: tried to acces element {index} of inventar, despite it having a size of {inventar.Count}");
            return 0;
        }

        Item item = inventar[index];
        if (item.isStackable) return itemCounter[item.itemTitle];
        return 1;

    }
}
