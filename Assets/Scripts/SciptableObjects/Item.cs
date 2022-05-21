using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

[CreateAssetMenu(fileName = "Item", menuName = "ScriptableObjects/Item")]
public class Item : ScriptableObject
{
    public Sprite icon;
    public string itemTitle;
    public string description;
    public string itemTag;
    public bool isStackable;
    public GameObject prefab;

    public virtual void Use() { }
}
