using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class GridCell : MonoBehaviour, IDropHandler
{
    public GridCell leftNeighbor;
    public GridCell rightNeighbor;
    public GridCell topNeighbor;
    public GridCell bottomNeighbor;

    public Vector2Int gridPosition;
    public CardsForTakeCardsAndCollection currentCardsForTakeCardsAndCollection;
    public Card currentCard;
    private Color customRed;
    private Color customGreen;

    public delegate void CardPlacedHandler(CardsForTakeCardsAndCollection card, GridCell cell, Vector2Int position);
    public static event CardPlacedHandler OnCardPlaced;

    void Start()
    {
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
        GameObject droppedObject = eventData.pointerDrag;

        if (droppedObject != null && droppedObject.GetComponent<DraggableBattleCard>())
        {
            DraggableBattleCard draggableCard = droppedObject.GetComponent<DraggableBattleCard>();

            // Проверка на то, что карта уже размещена
            if (draggableCard.IsLocked())
            {
                Debug.LogWarning("Эта карта уже размещена и не может быть перемещена.");
                return;
            }

            // Проверка на то, что в ячейке нет карты
            if (HasCard())
            {
                Debug.LogWarning("Ячейка уже занята, невозможно разместить карту.");
                return;
            }

            // Логика размещения карты
            droppedObject.transform.SetParent(transform);
            droppedObject.transform.localPosition = Vector3.zero;

            currentCardsForTakeCardsAndCollection = draggableCard.card;

            draggableCard.LockCard();
            ChangeTurn.Instance.SwitchTurn(gridPosition);

            CompareWithNeighbors(draggableCard.card);

            OnCardPlaced?.Invoke(currentCardsForTakeCardsAndCollection, this, gridPosition);
        }
    }

    public void CompareWithNeighbors(CardsForTakeCardsAndCollection newCard)
    {
        CompareNeighbor(leftNeighbor, newCard, "left");
        CompareNeighbor(rightNeighbor, newCard, "right");
        CompareNeighbor(topNeighbor, newCard, "top");
        CompareNeighbor(bottomNeighbor, newCard, "bottom");
    }

    private void CompareNeighbor(GridCell neighbor, CardsForTakeCardsAndCollection newCard, string direction)
    {
        if (neighbor != null && neighbor.HasCard())
        {
            // Проверяем наличие карт обоих типов
            CardsForTakeCardsAndCollection neighborPlayerCard = neighbor.currentCardsForTakeCardsAndCollection;
            Card neighborEnemyCard = neighbor.currentCard;

            // Сравнение с картой игрока
            if (neighborPlayerCard != null)
            {
                CompareCardValues(newCard, neighborPlayerCard, direction);
            }

            // Сравнение с картой врага
            if (neighborEnemyCard != null)
            {
                CompareCardValues(newCard, neighborEnemyCard, direction);
            }
        }
    }
    //
    private void CompareCardValues(CardsForTakeCardsAndCollection newCard, CardsForTakeCardsAndCollection neighborCard, string direction)
    {
        if (direction == "left")
        {
            Debug.Log($"Comparing right value of new card: {newCard.leftValue} vs left value of neighbor card: {neighborCard.rightValue}");
            if (!newCard.isPlayer1Card && newCard.leftValue < neighborCard.rightValue)
            {
                ChangeCardColor(customGreen);
            }
            else if (newCard.isPlayer1Card && newCard.leftValue < neighborCard.rightValue)
            {
                ChangeCardColor(customRed);
            }
        }
        // Аналогично для других направлений
        if (direction == "right")
        {
            Debug.Log($"Comparing left value of new card: {newCard.rightValue} vs right value of neighbor card: {neighborCard.leftValue}");
            if (!newCard.isPlayer1Card && newCard.rightValue < neighborCard.leftValue)
            {
                ChangeCardColor(customGreen);
            }
            else if (newCard.isPlayer1Card && newCard.rightValue < neighborCard.leftValue)
            {
                ChangeCardColor(customRed);
            }
        }
        if (direction == "top")
        {
            Debug.Log($"Comparing bottom value of new card: {newCard.bottomValue} vs top value of neighbor card: {neighborCard.topValue}");
            if (!newCard.isPlayer1Card && newCard.bottomValue < neighborCard.topValue)
            {
                ChangeCardColor(customGreen);
            }
            else if (newCard.isPlayer1Card && newCard.bottomValue < neighborCard.topValue)
            {
                ChangeCardColor(customRed);
            }
        }
        if (direction == "bottom")
        {
            Debug.Log($"Comparing top value of new card: {newCard.topValue} vs bottom value of neighbor card: {neighborCard.bottomValue}");
            if (!newCard.isPlayer1Card && newCard.topValue < neighborCard.bottomValue)
            {
                ChangeCardColor(customGreen);
            }
            else if (newCard.isPlayer1Card && newCard.topValue < neighborCard.bottomValue)
            {
                ChangeCardColor(customRed);
            }
        }
    }

    private void CompareCardValues(CardsForTakeCardsAndCollection newCard, Card neighborCard, string direction)
    {
        if (direction == "left")
        {
            Debug.Log($"Comparing right value of new card: {newCard.leftValue} vs left value of enemy card: {neighborCard.rightValue}");
            if (!newCard.isPlayer1Card && newCard.leftValue < neighborCard.rightValue)
            {
                ChangeCardColor(customGreen);
            }
            else if (newCard.isPlayer1Card && newCard.leftValue < neighborCard.rightValue)
            {
                ChangeCardColor(customRed);
            }
        }
        // Аналогично для других направлений
        if (direction == "right")
        {
            Debug.Log($"Comparing left value of new card: {newCard.rightValue} vs right value of enemy card: {neighborCard.leftValue}");
            if (!newCard.isPlayer1Card && newCard.rightValue < neighborCard.leftValue)
            {
                ChangeCardColor(customGreen);
            }
            else if (newCard.isPlayer1Card && newCard.rightValue < neighborCard.leftValue)
            {
                ChangeCardColor(customRed);
            }
        }
        if (direction == "top")
        {
            Debug.Log($"Comparing bottom value of new card: {newCard.bottomValue} vs top value of enemy card: {neighborCard.topValue}");
            if (!newCard.isPlayer1Card && newCard.bottomValue < neighborCard.topValue)
            {
                ChangeCardColor(customGreen);
            }
            else if (newCard.isPlayer1Card && newCard.bottomValue < neighborCard.topValue)
            {
                ChangeCardColor(customRed);
            }
        }
        if (direction == "bottom")
        {
            Debug.Log($"Comparing top value of new card: {newCard.topValue} vs bottom value of enemy card: {neighborCard.bottomValue}");
            if (!newCard.isPlayer1Card && newCard.topValue < neighborCard.bottomValue)
            {
                ChangeCardColor(customGreen);
            }
            else if (newCard.isPlayer1Card && newCard.topValue < neighborCard.bottomValue)
            {
                ChangeCardColor(customRed);
            }
        }
    }

    public bool HasEnemyCard()
    {
        return currentCard != null && !currentCard.isPlayer1Card;
    }

    public bool HasCard()
    {
        return currentCardsForTakeCardsAndCollection != null || currentCard != null;
    }

    public Card GetCard()
    {
        return currentCard;
    }
    
    private void ChangeCardColor(Color color)
    {
        // Предположим, что у карты есть Image компонент
        Image cardImage = GetComponentInChildren<Image>();
        if (cardImage != null)
        {
            Debug.Log($"Изменение цвета карты на {color}");
            cardImage.color = color;  // Устанавливаем новый цвет
        }
        else
        {
            Debug.LogError("Image component not found on card!");
        }
    }
}





















/*

using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class GridCell : MonoBehaviour, IDropHandler
{
    public GridCell leftNeighbor;
    public GridCell rightNeighbor;
    public GridCell topNeighbor;
    public GridCell bottomNeighbor;

    public Vector2Int gridPosition;
    public CardsForTakeCardsAndCollection currentCardsForTakeCardsAndCollection;
    public Card currentCard;
    private Color customRed;
    private Color customGreen;

    public delegate void CardPlacedHandler(CardsForTakeCardsAndCollection card, GridCell cell, Vector2Int position);
    public static event CardPlacedHandler OnCardPlaced;

    void Start()
    {
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
        GameObject droppedObject = eventData.pointerDrag;

        if (droppedObject != null && droppedObject.GetComponent<DraggableBattleCard>())
        {
            DraggableBattleCard draggableCard = droppedObject.GetComponent<DraggableBattleCard>();

            // Проверка на то, что карта уже размещена
            if (draggableCard.IsLocked())
            {
                Debug.LogWarning("Эта карта уже размещена и не может быть перемещена.");
                return;
            }

            // Проверка на то, что в ячейке нет карты
            if (HasCard())
            {
                Debug.LogWarning("Ячейка уже занята, невозможно разместить карту.");
                return;
            }

            // Остальная логика размещения карты
            droppedObject.transform.SetParent(transform);
            droppedObject.transform.localPosition = Vector3.zero;

            currentCardsForTakeCardsAndCollection = draggableCard.card;

            draggableCard.LockCard();
            ChangeTurn.Instance.SwitchTurn(gridPosition);

            CompareWithNeighbors(draggableCard.card);

            OnCardPlaced?.Invoke(currentCardsForTakeCardsAndCollection, this, gridPosition);
        }
    }

    public void CompareWithNeighbors(CardsForTakeCardsAndCollection newCard)
    {
        if (leftNeighbor != null && leftNeighbor.HasCard())
        {
            CardsForTakeCardsAndCollection leftCard = leftNeighbor.GetPlayerCard();
            if (leftCard != null)  // Добавляем проверку на null
            {
                Debug.Log($"Comparing right value of new card: {newCard.leftValue} vs left value of neighbor card: {leftCard.rightValue}");
                if (!newCard.isPlayer1Card && newCard.leftValue > leftCard.rightValue)
                {
                    leftNeighbor.ChangeCardColor(customGreen);
                }
                else if (newCard.isPlayer1Card && newCard.leftValue > leftCard.rightValue)
                {
                    leftNeighbor.ChangeCardColor(customRed);
                }
            }
        }

        if (rightNeighbor != null && rightNeighbor.HasCard())
        {
            CardsForTakeCardsAndCollection rightCard = rightNeighbor.GetPlayerCard();
            if (rightCard != null)  // Добавляем проверку на null
            {
                Debug.Log($"Comparing left value of new card: {newCard.rightValue} vs right value of neighbor card: {rightCard.leftValue}");
                if (!newCard.isPlayer1Card && newCard.rightValue > rightCard.leftValue)
                {
                    rightNeighbor.ChangeCardColor(customGreen);
                }
                else if (newCard.isPlayer1Card && newCard.rightValue > rightCard.leftValue)
                {
                    rightNeighbor.ChangeCardColor(customRed);
                }
            }
        }

        if (topNeighbor != null && topNeighbor.HasCard())
        {
            CardsForTakeCardsAndCollection topCard = topNeighbor.GetPlayerCard();
            if (topCard != null)  // Добавляем проверку на null
            {
                Debug.Log($"Comparing bottom value of new card: {newCard.bottomValue} vs top value of neighbor card: {topCard.topValue}");
                if (!newCard.isPlayer1Card && newCard.bottomValue > topCard.topValue)
                {
                    topNeighbor.ChangeCardColor(customGreen);
                }
                else if (newCard.isPlayer1Card && newCard.bottomValue > topCard.topValue)
                {
                    topNeighbor.ChangeCardColor(customRed);
                }
            }
        }

        if (bottomNeighbor != null && bottomNeighbor.HasCard())
        {
            CardsForTakeCardsAndCollection bottomCard = bottomNeighbor.GetPlayerCard();
            if (bottomCard != null)  // Добавляем проверку на null
            {
                Debug.Log($"Comparing top value of new card: {newCard.topValue} vs bottom value of neighbor card: {bottomCard.bottomValue}");
                if (!newCard.isPlayer1Card && newCard.topValue > bottomCard.bottomValue)
                {
                    bottomNeighbor.ChangeCardColor(customGreen);
                }
                else if (newCard.isPlayer1Card && newCard.topValue > bottomCard.bottomValue)
                {
                    bottomNeighbor.ChangeCardColor(customRed);
                }
            }
        }
    }

    public bool HasEnemyCard()
    {
        return currentCard != null && !currentCard.isPlayer1Card;
    }

    public bool HasCard()
    {
        return currentCard != null;
    }

    public Card GetCard()
    {
        return currentCard;
    }
    public CardsForTakeCardsAndCollection GetPlayerCard()
    {
        return currentCardsForTakeCardsAndCollection;
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
}*/