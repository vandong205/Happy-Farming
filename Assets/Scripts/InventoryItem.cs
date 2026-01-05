using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventoryItem : MonoBehaviour,IDragHandler,IBeginDragHandler,IEndDragHandler
{
    [Header("UI")]
    public Image icon;
    [HideInInspector] public Transform parentAfterDrag;
    public void OnBeginDrag(PointerEventData eventData)
    {
        icon.raycastTarget = false;
        parentAfterDrag = transform.parent;
        transform.SetParent(transform.root);


    }
    public void OnDrag(PointerEventData eventData)
    {
        transform.position = eventData.position;
    }
    public void OnEndDrag(PointerEventData eventData)
    {
        icon.raycastTarget = true;
        transform.SetParent(parentAfterDrag);
    }
}
