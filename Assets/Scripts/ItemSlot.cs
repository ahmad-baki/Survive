using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ItemSlot : MonoBehaviour, IPointerDownHandler
{
    public int index;

    UIInventarManager _uIInventarManager;

    void Start()
    {
        _uIInventarManager = UIInventarManager.Singelton;        
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        _uIInventarManager.OnClickedItem(index, eventData);
    }
}
