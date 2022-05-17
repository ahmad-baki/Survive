using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "Item", menuName = "ScriptableObjects/Item")]
public class Item : ScriptableObject
{
    public Sprite icon;
    public string itemTitle;
    public string description;
    public string itemTag;
    public bool isStackable;
    public string prefabPath;

    public GameObject Prefab
    {
        get { return Resources.Load<GameObject>(prefabPath); }
    }
}
