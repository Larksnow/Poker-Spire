using UnityEngine;
using UnityEngine.EventSystems;

public class CardDragHandler : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private CanvasGroup canvasGroup;
    private HoverableCard hoverableCard;
    public static bool IsDragging { get; private set; } // Static flag to indicate dragging state
    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        hoverableCard = GetComponent<HoverableCard>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        canvasGroup.blocksRaycasts = false;    // Prevent the card from intercepting raycasts
        IsDragging = true; // Set dragging flag to true
    }

    public void OnDrag(PointerEventData eventData)
    {
        // Move card with mouse
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            transform.parent as RectTransform, // Use the parent RectTransform
            Input.mousePosition,
            null,
            out Vector2 localPoint
        );
        transform.localPosition = localPoint; // Set the card's local position
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        // Restore raycast blocking for the card
        canvasGroup.blocksRaycasts = true;
        IsDragging = false;
        hoverableCard?.OnPointerExit(eventData);
        // Reset card to original position if not dropped in a drop zone
        if (eventData.pointerEnter != null && eventData.pointerEnter.CompareTag("DropZone"))
        {
            // Debug.Log("Card played in drop zone!");
            // Perform card effect here
        }
        else
        {
            transform.position = hoverableCard.origionalPosition;
        }
    }
}
