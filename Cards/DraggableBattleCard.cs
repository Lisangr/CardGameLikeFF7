using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class DraggableBattleCard : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;
    private Canvas parentCanvas;
    private GameObject cloneCard;
    private RectTransform cloneRectTransform;
    public CardsForTakeCardsAndCollection card;

    private bool isLocked = false;
    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
        parentCanvas = GetComponentInParent<Canvas>();
        if (parentCanvas == null)
        {
            Debug.LogError("�� ������� ����� Canvas ��� ������������� �������.");
        }
        card = GetComponent<CardsForTakeCardsAndCollection>();
    }
    // ������ ��������������
    public void OnBeginDrag(PointerEventData eventData)
    {
        if (isLocked)
        {
            Debug.LogWarning("����� ������������� � �� ����� ���� ����������.");
            return; // ���� ����� �������������, ���������� ��������������
        }

        cloneCard = Instantiate(gameObject, parentCanvas.transform);

        if (cloneCard == null)
        {
            Debug.LogError("�� ������� ������� ���� �����.");
            return;
        }

        cloneRectTransform = cloneCard.GetComponent<RectTransform>();

        if (cloneRectTransform == null)
        {
            Debug.LogError("�� ������� ����� RectTransform � ����� �����.");
            return;
        }

        cloneRectTransform.sizeDelta = rectTransform.sizeDelta;

        // ���� ��� ������� ������� �����
        Debug.Log("���� ������ �� �������: " + cloneRectTransform.position);

        cloneRectTransform.anchoredPosition = rectTransform.anchoredPosition; // ���������� ����� � ������� ���������

        DraggableBattleCard cloneDraggable = cloneCard.GetComponent<DraggableBattleCard>();
        cloneDraggable.enabled = false;

        CardsForTakeCardsAndCollection cloneCardComponent = cloneCard.GetComponent<CardsForTakeCardsAndCollection>();
        cloneCardComponent.enabled = false;

        cloneCardComponent.leftValue = card.leftValue;
        cloneCardComponent.rightValue = card.rightValue;
        cloneCardComponent.topValue = card.topValue;
        cloneCardComponent.bottomValue = card.bottomValue;

        cloneCardComponent.SetCard();
        cloneCard.GetComponent<CanvasGroup>().blocksRaycasts = false;

        cloneRectTransform.SetAsLastSibling();

        // �������� ��������� ������� �����
        Debug.Log("��������� ������� �����: " + cloneRectTransform.position);

        cloneCard.GetComponent<CanvasGroup>().alpha = 1f; // ������������� ������ ���������
        cloneCard.layer = LayerMask.NameToLayer("UI"); // ���������, ��� ���� ��������� � ���������� ����
    }

    // ������� ��������������
    public void OnDrag(PointerEventData eventData)
    {
        if (RectTransformUtility.ScreenPointToWorldPointInRectangle(
                parentCanvas.transform as RectTransform,
                eventData.position,
                parentCanvas.worldCamera,
                out var globalMousePos))
        {
            cloneRectTransform.position = globalMousePos; // ���������� ���� �� �����
        }
    }

    // ��������� ��������������
    public void OnEndDrag(PointerEventData eventData)
    {
        if (cloneCard != null)
        {
            Destroy(cloneCard); // ���������� ���� ��� ���������� ��������������
        }

        // ��������������� �����
        canvasGroup.alpha = 1.0f;
        canvasGroup.blocksRaycasts = true; // ���������� ���������� ���������
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
}
