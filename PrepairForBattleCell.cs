using UnityEngine;
using UnityEngine.EventSystems;

public class PrepairForBattleCell : MonoBehaviour, IDropHandler
{
    public CardsForTakeCardsAndCollection currentCard; // ������� ����� � ������   

    public void OnDrop(PointerEventData eventData)
    {
        // �������� ������, ������� ��� ���������
        GameObject droppedObject = eventData.pointerDrag;

        if (droppedObject != null && droppedObject.GetComponent<DraggableBattleCard>())
        {
            DraggableBattleCard draggableCard = droppedObject.GetComponent<DraggableBattleCard>();

            // ������������� ������ � ������
            droppedObject.transform.SetParent(transform);
            droppedObject.transform.localPosition = Vector3.zero;

            // ������������� ������� ����� � ������
            currentCard = draggableCard.card;
        }
    }
}
