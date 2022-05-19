using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ItemSlotHover : MonoBehaviour, IDragHandler, IEndDragHandler
{
    public int index;

    Canvas _canvas;
    RectTransform _rectTransform;
    CanvasGroup _canvasGroup;

    public void OnBeginDrag()
    {
        _rectTransform = GetComponent<RectTransform>();
        _canvasGroup = GetComponent<CanvasGroup>();
        _canvas = UIManager.Singelton.mainCanvas;
        _canvasGroup.blocksRaycasts = false;
        _canvasGroup.alpha = 0.7f;
    }

    public void OnDrag(PointerEventData eventData)
    {
        _rectTransform.anchoredPosition += eventData.delta / _canvas.scaleFactor;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        Destroy(gameObject);
    }
}
