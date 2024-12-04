
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DropZone : MonoBehaviour, IDropHandler
{
    public Color highlightColor = Color.yellow; // Color to highlight the drop zone
    private Color originalColor; // Store the original color

    private Image imageComponent; // Reference to the Image component

    private void Start()
    {
    }

    public void OnDrop(PointerEventData eventData)
    {
        GameObject droppedCard = eventData.pointerDrag;
        // Check if the dragged object is a card
        CardDragHandler cardHandler  = eventData.pointerDrag.GetComponent<CardDragHandler>();
        if (cardHandler  != null)
        {
            
            // Debug.Log("Card dropped in DropZone: " + droppedCard.GetComponent<CardDisplay>().cardID);
            DeckManager.main.PlayCard(droppedCard, PlayerManager.main, EnemyManager.main.currentEnemy.GetComponent<BaseEnemy>());
            cardHandler.OnEndDrag(eventData);
        }
        else
        {
            Debug.LogWarning("Dropped object is not a card.");
        }
    }
}
