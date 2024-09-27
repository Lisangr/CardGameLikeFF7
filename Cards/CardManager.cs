using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardManager : MonoBehaviour
{
    [Header("UI Grid Settings")]
    public GridLayoutGroup gridLayoutGroup;
    public GameObject cardUIPrefab;
    public List<Card> cardList;  // Все карты


    private void Start()
    {
        gridLayoutGroup = GetComponent<GridLayoutGroup>();

        if (cardList == null || cardList.Count == 0)
        {
            cardList = new List<Card>();  // Инициализируем пустой список карт
        }

        SpawnCardsInGrid();
    }

    // Метод для спауна карт на UI
    void SpawnCardsInGrid()
    {
        // Спавним ровно 9 карт
        for (int i = 0; i < 9; i++)
        {
            SpawnCard(i);
        }

        Debug.Log($"Создано и добавлено {cardList.Count} карт в лист.");
    }

    void SpawnCard(int index)
    {
        // Создаем объект карты и размещаем на UI
        GameObject cardObject = Instantiate(cardUIPrefab, gridLayoutGroup.transform);

        // Получаем компонент Card и присваиваем случайные значения
        Card cardDisplay = cardObject.GetComponent<Card>();

        if (cardDisplay != null)
        {
            // Задаем случайные значения для карты
            cardDisplay.leftValue = Random.Range(1, 10);
            cardDisplay.rightValue = Random.Range(1, 10);
            cardDisplay.topValue = Random.Range(1, 10);
            cardDisplay.bottomValue = Random.Range(1, 10);

            // Добавляем карту в список после ее спауна на UI
            cardList.Add(cardDisplay);

            // Отображаем карту на UI
            cardDisplay.SetCard();
        }
        else
        {
            Debug.LogWarning("Компонент Card не найден на префабе карты!");
        }
    }
    /*
    // Метод для удаления карты из колоды и UI
    public void RemoveCardFromDeck(Card card)
    {
        if (cardList.Contains(card))
        {
            // Удаляем карту из списка
            cardList.Remove(card);
            Debug.Log($"Карта удалена. Осталось карт: {cardList.Count}");

            // Уничтожаем объект карты из UI
            Destroy(card.gameObject);

            // Проверяем, если карт больше не осталось, вызываем событие
            if (cardList.Count == 0)
            {
                    Debug.Log("Колода пуста! Событие OnDeckEmpty вызвано.");
                    OnDeckEmpty.Invoke();
                
            }
        }
        else
        {
            Debug.LogWarning("Карта не найдена в колоде!");
        }
    }*/
}