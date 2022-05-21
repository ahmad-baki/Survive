using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ItemSlot : MonoBehaviour, IPointerDownHandler, IBeginDragHandler, IDragHandler
{
    public int index; 
    UIInventarManager _uIInventarManager;
    RectTransform _rectTransform;

    protected void Start()
    {
        _uIInventarManager = UIInventarManager.Singelton;
        _rectTransform = GetComponent<RectTransform>();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        _uIInventarManager.OnClickedItem(index, eventData);
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        GameObject hover = new("Hover");
        hover.transform.parent = transform.parent.parent.parent;
        RectTransform rectTransform = hover.AddComponent<RectTransform>();
        rectTransform.position = transform.position;
        rectTransform.sizeDelta = GetComponent<RectTransform>().sizeDelta;

        Image image = null;
        foreach (Image img in GetComponentsInChildren<Image>())
        {
            if (!img.gameObject.Equals(gameObject))
            {
                image = img;
                break;
            }
        }
        if (image is null) return;


        hover.AddComponent<Image>().sprite = image.sprite;
        hover.AddComponent<CanvasGroup>();

        ItemSlotHover itemSlotHover = hover.AddComponent<ItemSlotHover>();
        itemSlotHover.index = index;
        itemSlotHover.OnBeginDrag();

        eventData.pointerDrag = hover;
    }

    public void OnDrag(PointerEventData eventData)
    {
        throw new System.NotImplementedException();
    }
}
