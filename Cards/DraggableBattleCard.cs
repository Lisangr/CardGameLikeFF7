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
            Debug.LogError("Не удалось найти Canvas для родительского объекта.");
        }
        card = GetComponent<CardsForTakeCardsAndCollection>();
    }
    // Начало перетаскивания
    public void OnBeginDrag(PointerEventData eventData)
    {
        if (isLocked)
        {
            Debug.LogWarning("Карта заблокирована и не может быть перемещена.");
            return; // Если карта заблокирована, остановить перетаскивание
        }

        cloneCard = Instantiate(gameObject, parentCanvas.transform);

        if (cloneCard == null)
        {
            Debug.LogError("Не удалось создать клон карты.");
            return;
        }

        cloneRectTransform = cloneCard.GetComponent<RectTransform>();

        if (cloneRectTransform == null)
        {
            Debug.LogError("Не удалось найти RectTransform у клона карты.");
            return;
        }

        cloneRectTransform.sizeDelta = rectTransform.sizeDelta;

        // Логи для отладки позиции клона
        Debug.Log("Клон создан на позиции: " + cloneRectTransform.position);

        cloneRectTransform.anchoredPosition = rectTransform.anchoredPosition; // Приведение клона к позиции оригинала

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

        // Проверка финальной позиции клона
        Debug.Log("Финальная позиция клона: " + cloneRectTransform.position);

        cloneCard.GetComponent<CanvasGroup>().alpha = 1f; // Устанавливаем полную видимость
        cloneCard.layer = LayerMask.NameToLayer("UI"); // Убедитесь, что клон находится в правильном слое
    }

    // Процесс перетаскивания
    public void OnDrag(PointerEventData eventData)
    {
        if (RectTransformUtility.ScreenPointToWorldPointInRectangle(
                parentCanvas.transform as RectTransform,
                eventData.position,
                parentCanvas.worldCamera,
                out var globalMousePos))
        {
            cloneRectTransform.position = globalMousePos; // Перемещаем клон за мышью
        }
    }

    // Окончание перетаскивания
    public void OnEndDrag(PointerEventData eventData)
    {
        if (cloneCard != null)
        {
            Destroy(cloneCard); // Уничтожаем клон при завершении перетаскивания
        }

        // Восстанавливаем карту
        canvasGroup.alpha = 1.0f;
        canvasGroup.blocksRaycasts = true; // Возвращаем блокировку рейкастов
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
