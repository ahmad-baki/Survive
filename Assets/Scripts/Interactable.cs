using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public abstract class Interactable : MonoBehaviour
{
    public string interactionText;
    public Vector3 hUIOffset;
    public GameObject interactHUIPrefab;

    public GameObject _hUIParent;
    bool _isActive;
    GameObject _hUI;

    protected void Start()
    {
       Canvas canvas = FindObjectOfType<Canvas>();
        if(canvas is null) canvas = new GameObject("Canvas").AddComponent<Canvas>();

        foreach (Transform childTrans in canvas.GetComponentsInChildren<Transform>())
        {
            if (childTrans.name == "HUI")
            {
                _hUIParent = childTrans.gameObject;
            }
        }
        if(_hUIParent is null)
        {
            _hUIParent = new GameObject();
            _hUIParent.transform.parent = canvas.transform;
        }

        _hUI = Instantiate(interactHUIPrefab, _hUIParent.transform);
        _hUI.SetActive(false);
    }

    protected void Update()
    {
        if(_isActive && _hUIParent)
        {
            _hUI.transform.position = Camera.main.WorldToScreenPoint(transform.position + hUIOffset);
        }
    }

    public void Activate()
    {
        _isActive = true;
        _hUI.SetActive(true);

    }

    public void DeActivate()
    {
        _isActive = false;
        _hUI.SetActive(false);
    }

    public abstract void Interact(GameObject player);

    protected void OnDestroy()
    {
        Destroy(_hUI);
    }
}
