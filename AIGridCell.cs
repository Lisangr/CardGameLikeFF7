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

    // ����� ��� ������ ������������
    private Color playerColor;     // ������� ��� ���� ������
    private Color aiColor;         // ������� ��� ���� ��
    private Color neutralColor;    // ����� ��� ����������� ����

    // ��������� ����������� ������� ��� �������� ����������� ����
    public delegate void CounterForPlayer();
    public static event CounterForPlayer OnCardTookByPlayer1;
    public static event CounterForPlayer OnCardTookByPlayer2;

    void Start()
    {
        // ������������� �����, ����� ��������� �����
        if (!ColorUtility.TryParseHtmlString("#4FC3F7", out playerColor))  // ������� ��� ������
        {
            Debug.LogError("Invalid color string for #4FC3F7");
            playerColor = Color.blue;
        }

        if (!ColorUtility.TryParseHtmlString("#81C784", out aiColor))  // ������� ��� ��
        {
            Debug.LogError("Invalid color string for #81C784");
            aiColor = Color.green;
        }

        neutralColor = Color.white;  // ����� ��� ����������� ����
    }

    public void OnDrop(PointerEventData eventData)
    {
        // ���������, ��� � ������ ��� ��� �����
        if (currentCard != null)
        {
            Debug.LogWarning("������ ��� ������ ������.");
            return;
        }

        GameObject droppedObject = eventData.pointerDrag;

        if (droppedObject != null && droppedObject.GetComponent<DraggableCard>())
        {
            DraggableCard draggableCard = droppedObject.GetComponent<DraggableCard>();

            // ���������, ����������� �� ����� �������� ������
            if ((draggableCard.card.isPlayer1Card && !ChangeTurn.IsPlayer1Turn) ||
                (!draggableCard.card.isPlayer1Card && ChangeTurn.IsPlayer1Turn))
            {
                Debug.LogWarning("������ �������� ����� ������� ������.");
                return;
            }

            // ��������� ����� � ��� ������
            droppedObject.transform.SetParent(transform);
            droppedObject.transform.localPosition = Vector3.zero;

            // ��������� ������ �� �����
            currentCard = draggableCard.card;

            // ������������� ��������� ���� ����� (�����������)
            SetInitialCardColor(draggableCard.card);

            // ��������� ����� � ������ ���
            draggableCard.LockCard();
            ChangeTurn.Instance.SwitchTurn(gridPosition);

            // ���������� ����� � ��������
            CompareWithNeighbors(draggableCard.card);
        }
    }

    // ����� ����� ��� ��������� ���������� ����� �����
    private void SetInitialCardColor(Card card)
    {
        Image cardImage = card.GetComponent<Image>();
        if (cardImage != null)
        {
            cardImage.color = neutralColor; // ���������� ����� ������������ �����
        }
    }

    public void CompareWithNeighbors(Card newCard)
    {
        // � ����� �������
        if (leftNeighbor != null && leftNeighbor.HasCard())
        {
            Card leftCard = leftNeighbor.GetCard();
            // ��� ������ ������ �� ���������� ����� �������� ����� ����� � ������ ��������� ������
            if (CanBeatCard(newCard, leftCard, newCard.leftValue, leftCard.rightValue))
            {
                if (!newCard.isPlayer1Card)
                {
                    leftNeighbor.ChangeCardColor(aiColor); // ������� ��� ���� ��
                    OnCardTookByPlayer2?.Invoke();
                    Debug.Log($"�� ����� ����� ����� ����� ��������� {newCard.leftValue} > {leftCard.rightValue}");
                }
                else
                {
                    leftNeighbor.ChangeCardColor(playerColor); // ������� ��� ���� ������
                    OnCardTookByPlayer1?.Invoke();
                }
            }
        }

        // � ������ �������
        if (rightNeighbor != null && rightNeighbor.HasCard())
        {
            Card rightCard = rightNeighbor.GetCard();
            // ��� ������� ������ �� ���������� ������ �������� ����� ����� � ����� ��������� ������
            if (CanBeatCard(newCard, rightCard, newCard.rightValue, rightCard.leftValue))
            {
                if (!newCard.isPlayer1Card)
                {
                    rightNeighbor.ChangeCardColor(aiColor); // ������� ��� ���� ��
                    OnCardTookByPlayer2?.Invoke();
                    Debug.Log($"�� ����� ����� ������ ����� ��������� {newCard.rightValue} > {rightCard.leftValue}");
                }
                else
                {
                    rightNeighbor.ChangeCardColor(playerColor); // ������� ��� ���� ������
                    OnCardTookByPlayer1?.Invoke();
                }
            }
        }

        // � ������� �������
        if (topNeighbor != null && topNeighbor.HasCard())
        {
            Card topCard = topNeighbor.GetCard();
            // �����: ��� �������� ������ �� ���������� ������� �������� ����� ����� � ������ ��������� ������
            if (CanBeatCard(newCard, topCard, newCard.bottomValue, topCard.topValue))
            {
                if (!newCard.isPlayer1Card)
                {
                    topNeighbor.ChangeCardColor(aiColor); // ������� ��� ���� ��
                    OnCardTookByPlayer2?.Invoke();
                    Debug.Log($"�� ����� ����� ������ ����� ��������� {newCard.bottomValue} > {topCard.topValue}");
                }
                else
                {
                    topNeighbor.ChangeCardColor(playerColor); // ������� ��� ���� ������
                    OnCardTookByPlayer1?.Invoke();
                }
            }
        }

        // � ������ �������
        if (bottomNeighbor != null && bottomNeighbor.HasCard())
        {
            Card bottomCard = bottomNeighbor.GetCard();
            // �����: ��� ������� ������ �� ���������� ������ �������� ����� ����� � ������� ��������� ������
            if (CanBeatCard(newCard, bottomCard, newCard.topValue, bottomCard.bottomValue))
            {
                if (!newCard.isPlayer1Card)
                {
                    bottomNeighbor.ChangeCardColor(aiColor); // ������� ��� ���� ��
                    OnCardTookByPlayer2?.Invoke();
                    Debug.Log($"�� ����� ����� ����� ����� ��������� {newCard.topValue} > {bottomCard.bottomValue}");
                }
                else
                {
                    bottomNeighbor.ChangeCardColor(playerColor); // ������� ��� ���� ������
                    OnCardTookByPlayer1?.Invoke();
                }
            }
        }
    }

    // ����� ��� ��������, ����� �� ����� ��������� ������
    private bool CanBeatCard(Card newCard, Card neighborCard, int newCardValue, int neighborCardValue)
    {
        // ���� ����� ��� ��������� (������� ����), ���������� ��������, ����� ��������� ����� ����� �������
        bool wasBeaten = IsCardColored(neighborCard);

        // ���� ����� ��� �� ������ ������ � �� ���� �������
        if (newCard.isPlayer1Card == neighborCard.isPlayer1Card && !wasBeaten)
        {
            return false;
        }

        // ���������� �������� ����
        return newCardValue > neighborCardValue;
    }

    // ���������, ���� �� ����� ��� ���������
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
                cardImage.color = color; // �������� ���� �����

                // ����� ��������� isPlayer1Card �������� �����, ����� �������� ����� ��������������
                if (color == playerColor)
                {
                    currentCard.isPlayer1Card = true; // ������ ��� ����� ������
                }
                else if (color == aiColor)
                {
                    currentCard.isPlayer1Card = false; // ������ ��� ����� ��
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

    public bool SetCard(Card card)
    {
        // Если ячейка уже занята, не позволяем установить другую карту
        if (currentCard != null)
        {
            Debug.LogWarning($"Ячейка {gridPosition} уже содержит карту! Попытка установить: L:{card.leftValue} R:{card.rightValue} T:{card.topValue} B:{card.bottomValue}");
            return false;
        }
        
        currentCard = card;
        return true;
    }
}