using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class GridCellForPlayerVsPlayer : MonoBehaviour, IDropHandler
{
    public GridCellForPlayerVsPlayer leftNeighbor;
    public GridCellForPlayerVsPlayer rightNeighbor;
    public GridCellForPlayerVsPlayer topNeighbor;
    public GridCellForPlayerVsPlayer bottomNeighbor;
    //public CardManager cardManager;

    public Vector2Int gridPosition;
    public Card currentCard;
    private Color customRed;
    private Color customGreen;

    public delegate void CounterForPlayer();
    public static event CounterForPlayer OnCardTookByPlayer1;
    public static event CounterForPlayer OnCardTookByPlayer2;

    void Start()
    {
        // Преобразуем строковый цвет в объект Color
        if (!ColorUtility.TryParseHtmlString("#FF9494", out customRed))
        {
            Debug.LogError("Invalid color string for #FF9494");
        }

        // Преобразуем строковый цвет в объект Color
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

            // Проверяем, принадлежит ли карта активному игроку
            if ((draggableCard.card.isPlayer1Card && !ChangeTurn.IsPlayer1Turn) ||
                (!draggableCard.card.isPlayer1Card && ChangeTurn.IsPlayer1Turn))
            {
                Debug.LogWarning("Нельзя сбросить карту другого игрока.");
                return;
            }

            // Перемещаем карту в эту ячейку
            droppedObject.transform.SetParent(transform);
            droppedObject.transform.localPosition = Vector3.zero;

            // Обновляем состояние ячейки, устанавливаем текущую карту
            currentCard = draggableCard.card;

            // Блокируем карту и меняем ход
            draggableCard.LockCard();
            ChangeTurn.Instance.SwitchTurn(gridPosition);

            // Сравниваем новую карту с соседями
            CompareWithNeighbors(draggableCard.card);
        }
    }


    public void CompareWithNeighbors(Card newCard)
    {
        // С левой картой
        if (leftNeighbor != null && leftNeighbor.HasCard())
        {
            Card leftCard = leftNeighbor.GetCard();
            if (CanBeatCard(newCard, leftCard, newCard.leftValue, leftCard.rightValue))
            {
                if (!newCard.isPlayer1Card)
                {
                    leftNeighbor.ChangeCardColor(customGreen);
                    OnCardTookByPlayer2?.Invoke();
                }
                else
                {
                    leftNeighbor.ChangeCardColor(customRed);
                    OnCardTookByPlayer1?.Invoke();
                }
            }
        }

        // С правой картой
        if (rightNeighbor != null && rightNeighbor.HasCard())
        {
            Card rightCard = rightNeighbor.GetCard();
            if (CanBeatCard(newCard, rightCard, newCard.rightValue, rightCard.leftValue))
            {
                if (!newCard.isPlayer1Card)
                {
                    rightNeighbor.ChangeCardColor(customGreen);
                    OnCardTookByPlayer2?.Invoke();
                }
                else
                {
                    rightNeighbor.ChangeCardColor(customRed);
                    OnCardTookByPlayer1?.Invoke();
                }
            }
        }

        // С верхней картой
        if (topNeighbor != null && topNeighbor.HasCard())
        {
            Card topCard = topNeighbor.GetCard();
            if (CanBeatCard(newCard, topCard, newCard.bottomValue, topCard.topValue))
            {
                if (!newCard.isPlayer1Card)
                {
                    topNeighbor.ChangeCardColor(customGreen);
                    OnCardTookByPlayer2?.Invoke();
                }
                else
                {
                    topNeighbor.ChangeCardColor(customRed);
                    OnCardTookByPlayer1?.Invoke();
                }
            }
        }

        // С нижней картой
        if (bottomNeighbor != null && bottomNeighbor.HasCard())
        {
            Card bottomCard = bottomNeighbor.GetCard();
            if (CanBeatCard(newCard, bottomCard, newCard.topValue, bottomCard.bottomValue))
            {
                if (!newCard.isPlayer1Card)
                {
                    bottomNeighbor.ChangeCardColor(customGreen);
                    OnCardTookByPlayer2?.Invoke();
                }
                else
                {
                    bottomNeighbor.ChangeCardColor(customRed);
                    OnCardTookByPlayer1?.Invoke();
                }
            }
        }
    }

    // Метод для проверки, может ли новая карта побить соседнюю
    private bool CanBeatCard(Card newCard, Card neighborCard, int newCardValue, int neighborCardValue)
    {
        // Если карта была побеждена (изменила цвет), разрешаем перебить, даже если карта того же игрока
        bool wasBeaten = IsCardColored(neighborCard);

        // Если карта того же игрока и не была побеждена — не перебиваем
        if (newCard.isPlayer1Card == neighborCard.isPlayer1Card && !wasBeaten)
        {
            return false;
        }

        // Проверяем значения карт, можно ли перебить
        return newCardValue > neighborCardValue;
    }

    // Проверяет, была ли карта уже побеждена (окрашена)
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
                cardImage.color = color; // Изменяем цвет карты
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