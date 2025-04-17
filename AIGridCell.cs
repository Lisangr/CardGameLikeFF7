using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class AIGridCell : MonoBehaviour, IDropHandler
{
    public AIGridCell leftNeighbor;
    public AIGridCell rightNeighbor;
    public AIGridCell topNeighbor;
    public AIGridCell bottomNeighbor;

    public Vector2Int gridPosition;
    public Card currentCard;
    private Color customRed;
    private Color customGreen;

    // Добавляем аналогичные события для подсчета захваченных карт
    public delegate void CounterForPlayer();
    public static event CounterForPlayer OnCardTookByPlayer1;
    public static event CounterForPlayer OnCardTookByPlayer2;
    void Start()
    {
        // Инициализация цветов
        if (!ColorUtility.TryParseHtmlString("#FF9494", out customRed))
        {
            Debug.LogError("Invalid color string for #FF9494");
        }

        if (!ColorUtility.TryParseHtmlString("#A3FF86", out customGreen))
        {
            Debug.LogError("Invalid color string for #A3FF86");
        }
    }

    public void OnDrop(PointerEventData eventData)
    {
        // Проверяем, что в ячейке еще нет карты
        if (currentCard != null)
        {
            Debug.LogWarning("Ячейка уже занята картой.");
            return;
        }

        GameObject droppedObject = eventData.pointerDrag;

        if (droppedObject != null && droppedObject.GetComponent<DraggableCard>())
        {
            DraggableCard draggableCard = droppedObject.GetComponent<DraggableCard>();

            // Проверяем, принадлежит ли карта текущему игроку
            if ((draggableCard.card.isPlayer1Card && !ChangeTurn.IsPlayer1Turn) ||
                (!draggableCard.card.isPlayer1Card && ChangeTurn.IsPlayer1Turn))
            {
                Debug.LogWarning("Нельзя положить карту другого игрока.");
                return;
            }

            // Размещаем карту в эту ячейку
            droppedObject.transform.SetParent(transform);
            droppedObject.transform.localPosition = Vector3.zero;

            // Сохраняем ссылку на карту
            currentCard = draggableCard.card;

            // Блокируем карту и меняем ход
            draggableCard.LockCard();
            ChangeTurn.Instance.SwitchTurn(gridPosition);

            // Сравниваем карту с соседями
            CompareWithNeighbors(draggableCard.card);
        }
    }
    public void CompareWithNeighbors(Card newCard)
    {
        // С левым соседом
        if (leftNeighbor != null && leftNeighbor.HasCard())
        {
            Card leftCard = leftNeighbor.GetCard();
            if (CanBeatCard(newCard, leftCard, newCard.leftValue, leftCard.rightValue))
            {
                if (!newCard.isPlayer1Card)
                {
                    leftNeighbor.ChangeCardColor(customGreen);
                    OnCardTookByPlayer2?.Invoke(); // Вызываем событие
                }
                else
                {
                    leftNeighbor.ChangeCardColor(customRed);
                    OnCardTookByPlayer1?.Invoke(); // Вызываем событие
                }
            }
        }

        // С правым соседом
        if (rightNeighbor != null && rightNeighbor.HasCard())
        {
            Card rightCard = rightNeighbor.GetCard();
            if (CanBeatCard(newCard, rightCard, newCard.rightValue, rightCard.leftValue))
            {
                if (!newCard.isPlayer1Card)
                {
                    rightNeighbor.ChangeCardColor(customGreen);
                    OnCardTookByPlayer2?.Invoke(); // Вызываем событие
                }
                else
                {
                    rightNeighbor.ChangeCardColor(customRed);
                    OnCardTookByPlayer1?.Invoke(); // Вызываем событие
                }
            }
        }

        // С верхним соседом
        if (topNeighbor != null && topNeighbor.HasCard())
        {
            Card topCard = topNeighbor.GetCard();
            if (CanBeatCard(newCard, topCard, newCard.topValue, topCard.bottomValue))
            {
                if (!newCard.isPlayer1Card)
                {
                    topNeighbor.ChangeCardColor(customGreen);
                    OnCardTookByPlayer2?.Invoke(); // Вызываем событие
                }
                else
                {
                    topNeighbor.ChangeCardColor(customRed);
                    OnCardTookByPlayer1?.Invoke(); // Вызываем событие
                }
            }
        }

        // С нижним соседом
        if (bottomNeighbor != null && bottomNeighbor.HasCard())
        {
            Card bottomCard = bottomNeighbor.GetCard();
            if (CanBeatCard(newCard, bottomCard, newCard.bottomValue, bottomCard.topValue))
            {
                if (!newCard.isPlayer1Card)
                {
                    bottomNeighbor.ChangeCardColor(customGreen);
                    OnCardTookByPlayer2?.Invoke(); // Вызываем событие
                }
                else
                {
                    bottomNeighbor.ChangeCardColor(customRed);
                    OnCardTookByPlayer1?.Invoke(); // Вызываем событие
                }
            }
        }
    }

    // Метод для проверки, может ли карта захватить соседа
    private bool CanBeatCard(Card newCard, Card neighborCard, int newCardValue, int neighborCardValue)
    {
        // Если карта уже захвачена (изменен цвет), пропускаем проверку, иначе проверяем карты одной команды
        bool wasBeaten = IsCardColored(neighborCard);

        // Если карты обе от одного игрока и не было захвата
        if (newCard.isPlayer1Card == neighborCard.isPlayer1Card && !wasBeaten)
        {
            return false;
        }

        // Сравниваем значения карт
        return newCardValue > neighborCardValue;
    }

    // Проверяем, была ли карта уже захвачена
    private bool IsCardColored(Card neighborCard)
    {
        Image neighborCardImage = neighborCard.GetComponent<Image>();
        if (neighborCardImage != null)
        {
            return neighborCardImage.color == customRed || neighborCardImage.color == customGreen;
        }
        return false;
    }

    public bool HasCard()
    {
        return currentCard != null;
    }

    public Card GetCard()
    {
        return currentCard;
    }

    public void ChangeCardColor(Color color)
    {
        if (HasCard())
        {
            Image cardImage = transform.GetChild(0).GetComponent<Image>();
            if (cardImage != null)
            {
                Debug.Log($"Changing card color to {color}.");
                cardImage.color = color;
            }
            else
            {
                Debug.LogError("Image component not found on card!");
            }
        }
        else
        {
            Debug.LogError("No card found in this cell!");
        }
    }
}