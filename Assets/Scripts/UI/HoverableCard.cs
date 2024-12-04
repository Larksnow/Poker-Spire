using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI; 

public class HoverableCard : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private GameObject cardInstance;
    private Image imageComponent;
    private Color originalColor;
    private int originalIndex;
    private float offset = 50f;
    private Transform cardTransform;
    public Vector3 origionalPosition;

    public void Initialize(GameObject card, int index)
    {
        originalIndex = index;
        cardInstance = card;
        cardTransform = cardInstance.transform;
        imageComponent = cardInstance.GetComponent<Image>();
        if (imageComponent != null)
        {
            originalColor = imageComponent.color; // Store the original color
        }
        origionalPosition = cardTransform.position;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (CardDragHandler.IsDragging) return; 
        if (cardInstance != null)
        {
            UpdateIndex();
            DeckManager.main.BringToFront(cardInstance);
            cardTransform.localPosition += new Vector3(0, offset, 0);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (CardDragHandler.IsDragging) return; 
        if (cardInstance != null)
        {
            cardTransform.position = origionalPosition;
            DeckManager.main.BringToOrigin(cardInstance, originalIndex);
        }
    }

    public void UpdateIndex()
    {
        int newIndex = cardTransform.GetSiblingIndex();
        // Debug.Log("Updating Origional Index: " + originalIndex + " to newIndex: " + newIndex);
        originalIndex = newIndex;
    }

}
