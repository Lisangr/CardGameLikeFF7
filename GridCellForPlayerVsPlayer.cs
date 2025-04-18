using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections.Generic;

public class GridCellForPlayerVsPlayer : MonoBehaviour, IDropHandler
{
    public GridCellForPlayerVsPlayer leftNeighbor;
    public GridCellForPlayerVsPlayer rightNeighbor;
    public GridCellForPlayerVsPlayer topNeighbor;
    public GridCellForPlayerVsPlayer bottomNeighbor;

    public Vector2Int gridPosition;
    public Card currentCard;

    // Новые цвета для лучшей визуализации
    private Color playerColor;     // Голубой для карт игрока
    private Color aiColor;         // Зеленый для карт ИИ
    private Color neutralColor;    // Белый для нейтральных карт

    public delegate void CounterForPlayer();
    public static event CounterForPlayer OnCardTookByPlayer1;
    public static event CounterForPlayer OnCardTookByPlayer2;

    void Start()
    {
        // Устанавливаем новые, более наглядные цвета
        if (!ColorUtility.TryParseHtmlString("#4FC3F7", out playerColor))  // Голубой для игрока
        {
            Debug.LogError("Invalid color string for #4FC3F7");
            playerColor = Color.blue;
        }

        if (!ColorUtility.TryParseHtmlString("#81C784", out aiColor))  // Зеленый для ИИ
        {
            Debug.LogError("Invalid color string for #81C784");
            aiColor = Color.green;
        }

        neutralColor = Color.white;  // Белый для нейтральных карт
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

            // Устанавливаем цвет карты в соответствии с принадлежностью
            SetInitialCardColor(draggableCard.card);

            // Блокируем карту и меняем ход
            draggableCard.LockCard();
            ChangeTurn.Instance.SwitchTurn(gridPosition);

            // Сравниваем карту с соседями
            CompareWithNeighbors(draggableCard.card);
        }
    }

    // Новый метод для установки начального цвета карты
    private void SetInitialCardColor(Card card)
    {
        Image cardImage = card.GetComponent<Image>();
        if (cardImage != null)
        {
            cardImage.color = neutralColor; // Изначально карты нейтрального цвета
        }
    }

    public void CompareWithNeighbors(Card newCard)
    {
        // С левым соседом
        if (leftNeighbor != null && leftNeighbor.HasCard())
        {
            Card leftCard = leftNeighbor.GetCard();
            // Для левого соседа мы сравниваем левое значение нашей карты с правым значением соседа
            if (CanBeatCard(newCard, leftCard, newCard.leftValue, leftCard.rightValue))
            {
                if (!newCard.isPlayer1Card)
                {
                    leftNeighbor.ChangeCardColor(aiColor); // Зеленый для карт ИИ
                    OnCardTookByPlayer2?.Invoke();
                    Debug.Log($"ИИ побил карту слева своим значением {newCard.leftValue} > {leftCard.rightValue}");
                }
                else
                {
                    leftNeighbor.ChangeCardColor(playerColor); // Голубой для карт игрока
                    OnCardTookByPlayer1?.Invoke();
                }
            }
        }

        // С правым соседом
        if (rightNeighbor != null && rightNeighbor.HasCard())
        {
            Card rightCard = rightNeighbor.GetCard();
            // Для правого соседа мы сравниваем правое значение нашей карты с левым значением соседа
            if (CanBeatCard(newCard, rightCard, newCard.rightValue, rightCard.leftValue))
            {
                if (!newCard.isPlayer1Card)
                {
                    rightNeighbor.ChangeCardColor(aiColor); // Зеленый для карт ИИ
                    OnCardTookByPlayer2?.Invoke();
                    Debug.Log($"ИИ побил карту справа своим значением {newCard.rightValue} > {rightCard.leftValue}");
                }
                else
                {
                    rightNeighbor.ChangeCardColor(playerColor); // Голубой для карт игрока
                    OnCardTookByPlayer1?.Invoke();
                }
            }
        }

        // С верхним соседом
        if (topNeighbor != null && topNeighbor.HasCard())
        {
            Card topCard = topNeighbor.GetCard();
            // ВАЖНО: Для верхнего соседа мы сравниваем верхнее значение нашей карты с нижним значением соседа
            if (CanBeatCard(newCard, topCard, newCard.bottomValue, topCard.topValue))
            {
                if (!newCard.isPlayer1Card)
                {
                    topNeighbor.ChangeCardColor(aiColor); // Зеленый для карт ИИ
                    OnCardTookByPlayer2?.Invoke();
                    Debug.Log($"ИИ побил карту сверху своим значением {newCard.bottomValue} > {topCard.topValue}");
                }
                else
                {
                    topNeighbor.ChangeCardColor(playerColor); // Голубой для карт игрока
                    OnCardTookByPlayer1?.Invoke();
                }
            }
        }

        // С нижним соседом
        if (bottomNeighbor != null && bottomNeighbor.HasCard())
        {
            Card bottomCard = bottomNeighbor.GetCard();
            // ВАЖНО: Для нижнего соседа мы сравниваем нижнее значение нашей карты с верхним значением соседа
            if (CanBeatCard(newCard, bottomCard, newCard.topValue, bottomCard.bottomValue))
            {
                if (!newCard.isPlayer1Card)
                {
                    bottomNeighbor.ChangeCardColor(aiColor); // Зеленый для карт ИИ
                    OnCardTookByPlayer2?.Invoke();
                    Debug.Log($"ИИ побил карту снизу своим значением {newCard.topValue} > {bottomCard.bottomValue}");
                }
                else
                {
                    bottomNeighbor.ChangeCardColor(playerColor); // Голубой для карт игрока
                    OnCardTookByPlayer1?.Invoke();
                }
            }
        }
    }

    // Метод для проверки, может ли карта побить другую карту
    private bool CanBeatCard(Card newCard, Card neighborCard, int newCardValue, int neighborCardValue)
    {
        // Проверяем, была ли карта уже захвачена
        bool wasBeaten = IsCardColored(neighborCard);

        // Если карты принадлежат одному игроку и она не была захвачена ранее, то побить нельзя
        if (newCard.isPlayer1Card == neighborCard.isPlayer1Card && !wasBeaten)
        {
            return false;
        }

        // Сравниваем значения карт
        return newCardValue > neighborCardValue;
    }

    // Проверяем, была ли карта уже захвачена (окрашена)
    private bool IsCardColored(Card neighborCard)
    {
        Image neighborCardImage = neighborCard.GetComponent<Image>();
        if (neighborCardImage != null)
        {
            return neighborCardImage.color == playerColor || neighborCardImage.color == aiColor;
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
            Image cardImage = currentCard.GetComponent<Image>();
            if (cardImage != null)
            {
                Debug.Log($"Changing card color to {color}.");
                cardImage.color = color; // Изменяем цвет карты

                // Также обновляем isPlayer1Card свойство карты, чтобы отразить новую принадлежность
                if (color == playerColor)
                {
                    currentCard.isPlayer1Card = true; // Теперь это карта игрока
                }
                else if (color == aiColor)
                {
                    currentCard.isPlayer1Card = false; // Теперь это карта ИИ
                }
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