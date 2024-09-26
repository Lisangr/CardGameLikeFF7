using UnityEngine;
using UnityEngine.EventSystems;

public class PrepairForBattleCell : MonoBehaviour, IDropHandler
{
    public CardsForTakeCardsAndCollection currentCard; // Текущая карта в ячейке   

    public void OnDrop(PointerEventData eventData)
    {
        // Получаем объект, который был перетащен
        GameObject droppedObject = eventData.pointerDrag;

        if (droppedObject != null && droppedObject.GetComponent<DraggableBattleCard>())
        {
            DraggableBattleCard draggableCard = droppedObject.GetComponent<DraggableBattleCard>();

            // Устанавливаем объект в ячейку
            droppedObject.transform.SetParent(transform);
            droppedObject.transform.localPosition = Vector3.zero;

            // Устанавливаем текущую карту в ячейке
            currentCard = draggableCard.card;
        }
    }
}
