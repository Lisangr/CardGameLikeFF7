using UnityEngine;
using UnityEngine.EventSystems;

public class DraggableCard : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;
    private Canvas parentCanvas;
    private GameObject cloneCard;
    private RectTransform cloneRectTransform;
    private bool isDropped = false;
    private bool isLocked = false;
    public Card card;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
        parentCanvas = GetComponentInParent<Canvas>();
        card = GetComponent<Card>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (isDropped || isLocked || !CanDragCard()) return;

        // �������� �� ����������� �������������� � ����������� �� �������� ����
        if ((card.isPlayer1Card && !ChangeTurn.IsPlayer1Turn) || (!card.isPlayer1Card && ChangeTurn.IsPlayer1Turn))
        {
            Debug.Log("�� ������ ������������� ����� ������� ������.");
            return; // ��������� �������������� �����, ���� ��� �� ��� ������
        }

        cloneCard = Instantiate(gameObject, parentCanvas.transform);
        cloneRectTransform = cloneCard.GetComponent<RectTransform>();
        cloneRectTransform.sizeDelta = rectTransform.sizeDelta;

        DraggableCard cloneDraggable = cloneCard.GetComponent<DraggableCard>();
        cloneDraggable.enabled = false;

        Card cloneCardComponent = cloneCard.GetComponent<Card>();
        cloneCardComponent.enabled = false;

        cloneCardComponent.leftValue = card.leftValue;
        cloneCardComponent.rightValue = card.rightValue;
        cloneCardComponent.topValue = card.topValue;
        cloneCardComponent.bottomValue = card.bottomValue;

        cloneCardComponent.SetCard();
        cloneCard.GetComponent<CanvasGroup>().blocksRaycasts = false;

        cloneRectTransform.SetAsLastSibling();
    }

    public void OnDrag(PointerEventData eventData)
{
    if (isDropped || isLocked || !gameObject || !gameObject.activeInHierarchy) 
    {
        return;
    }

    // Проверяем, не уничтожен ли объект
    if (!cloneCard || !parentCanvas)
    {
        Debug.LogWarning("Попытка перетащить карту с уничтоженными объектами");
        return;
    }

    // Проверяем на возможность перетаскивания в зависимости от текущего хода
    if ((card.isPlayer1Card && !ChangeTurn.IsPlayer1Turn) || (!card.isPlayer1Card && ChangeTurn.IsPlayer1Turn))
    {
        Debug.Log("Не можете перетаскивать карты другого игрока.");
        return;
    }

    try
    {
        RectTransformUtility.ScreenPointToWorldPointInRectangle(
            parentCanvas.transform as RectTransform,
            eventData.position,
            parentCanvas.worldCamera,
            out var globalMousePos);

        cloneRectTransform.position = globalMousePos;
    }
    catch (System.Exception e)
    {
        Debug.LogError($"Ошибка при перетаскивании карты: {e.Message}");
        if (cloneCard != null)
        {
            Destroy(cloneCard);
            cloneCard = null;
        }
    }
}

    public void OnEndDrag(PointerEventData eventData)
    {
        if (cloneCard != null)
        {
            Destroy(cloneCard);
        }

        if (isDropped || isLocked) return;

        // �������� �� ����������� �������������� � ����������� �� �������� ����
        if ((card.isPlayer1Card && !ChangeTurn.IsPlayer1Turn) || (!card.isPlayer1Card && ChangeTurn.IsPlayer1Turn))
        {
            Debug.Log("�� ������ ������������� ����� ������� ������.");
            return;
        }

        // ������� ����� �� ������ � CardManager
    }

    public void LockCard()
    {
        isLocked = true;
        canvasGroup.blocksRaycasts = false;
    }

    public bool IsLocked()
    {
        return isLocked;
    }

    private bool CanDragCard()
    {
        // �������� �� ��, ��� ���, � � ������ ������ ��������� �����
        if (card.isPlayer1Card && !ChangeTurn.IsPlayer1Turn)
        {
            Debug.Log("����� 1 �� ����� ������������� ����� �� ����� ���� ������ 2.");
            return false;
        }
        if (!card.isPlayer1Card && ChangeTurn.IsPlayer1Turn)
        {
            Debug.Log("����� 2 �� ����� ������������� ����� �� ����� ���� ������ 1.");
            return false;
        }
        return true;
    }
}
