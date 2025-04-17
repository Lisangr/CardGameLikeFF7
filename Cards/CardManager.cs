using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardManager : MonoBehaviour
{
    [Header("UI Grid Settings")]
    public GridLayoutGroup gridLayoutGroup;
    public GameObject cardUIPrefab;
    public List<Card> cardList;

    private AIPlayer aiPlayer;
    private bool cardsSpawned = false; // Флаг для предотвращения повторного создания карт

    private void Start()
    {
        // Проверяем инициализацию GridLayoutGroup
        if (gridLayoutGroup == null)
        {
            gridLayoutGroup = GetComponent<GridLayoutGroup>();
            
            // Если не нашли на текущем объекте, пробуем найти в дочерних
            if (gridLayoutGroup == null)
            {
                gridLayoutGroup = GetComponentInChildren<GridLayoutGroup>();
                
                // Если всё еще не нашли, ищем вообще в сцене
                if (gridLayoutGroup == null)
                {
                    Debug.LogError("GridLayoutGroup не найден ни на объекте, ни в его дочерних элементах!");
                    return;
                }
            }
            
            Debug.Log($"Найден GridLayoutGroup на объекте: {gridLayoutGroup.gameObject.name}");
        }
        
        aiPlayer = FindObjectOfType<AIPlayer>();

        if (aiPlayer == null)
        {
            Debug.LogError("AIPlayer не найден на сцене!");
        }

        if (cardList == null || cardList.Count == 0)
        {
            cardList = new List<Card>();
        }

        // Проверяем, не были ли уже созданы карты
        if (!cardsSpawned)
        {
            SpawnCardsInGrid();
            SpawnAICards();
            cardsSpawned = true;
            
            // Логируем иерархию карт после спавна
            Invoke("LogCardsHierarchy", 0.5f);
        }
    }

    void SpawnCardsInGrid()
    {
        // Создаем всего 9 карт
        for (int i = 0; i < 9; i++)
        {
            SpawnCard(i);
        }

        Debug.Log($"Создано и добавлено {cardList.Count} карт в сетку.");
    }

    void SpawnCard(int index)
    {
        // Создаем объект карты и добавляем на UI
        GameObject cardObject = Instantiate(cardUIPrefab, gridLayoutGroup.transform);

        // Получаем компонент Card и устанавливаем случайные значения
        Card cardDisplay = cardObject.GetComponent<Card>();

        if (cardDisplay != null)
        {
            // Задаем случайные значения для карты
            cardDisplay.leftValue = Random.Range(1, 10);
            cardDisplay.rightValue = Random.Range(1, 10);
            cardDisplay.topValue = Random.Range(1, 10);
            cardDisplay.bottomValue = Random.Range(1, 10);
            cardDisplay.isPlayer1Card = true; // Это карта игрока

            // Добавляем карту в список карт на основе их UI
            cardList.Add(cardDisplay);

            // Обновляем карту на UI
            cardDisplay.SetCard();
        }
        else
        {
            Debug.LogWarning("Компонент Card не найден на объекте карты!");
        }
    }

    [SerializeField]
    private Transform aiCardsParent; // Добавьте это поле и назначьте его в инспекторе на подходящий объект-контейнер

    public void SpawnAICards()
    {
        // Очистка существующих карт у ИИ
        AIPlayer aiPlayer = FindObjectOfType<AIPlayer>();
        if (aiPlayer != null)
        {
            aiPlayer.ClearCards();
        }
        else
        {
            Debug.LogError("AIPlayer не найден на сцене!");
            return;
        }

        // Проверка существования родительского объекта
        if (aiCardsParent == null)
        {
            Debug.LogError("Родительский объект для карт ИИ не назначен!");
            return;
        }

        // Создание карт для ИИ
        for (int i = 0; i < 9; i++)
        {
            // Создаем карту под правильным родителем
            GameObject cardObject = Instantiate(cardUIPrefab, aiCardsParent);
            cardObject.name = $"AI_Card_{i}";
            
            Card cardDisplay = cardObject.GetComponent<Card>();
            if (cardDisplay == null)
            {
                Debug.LogError("Компонент Card не найден на префабе карты ИИ!");
                continue;
            }
            
            // Устанавливаем значения для карты
            cardDisplay.leftValue = Random.Range(1, 10);
            cardDisplay.rightValue = Random.Range(1, 10);
            cardDisplay.topValue = Random.Range(1, 10);
            cardDisplay.bottomValue = Random.Range(1, 10);
            cardDisplay.isPlayer1Card = false; // Это карта ИИ
            
            // Обновляем отображение значений на карте
            cardDisplay.SetCard();
            
            // Логируем информацию о карте
            Debug.Log($"Создана карта ИИ {i} с родителем: {cardObject.transform.parent.name}");
            
            // Добавляем карту ИИ
            aiPlayer.AddCard(cardDisplay);
        }

        Debug.Log($"Созданы карты для AI: 9 шт. Всего карт у ИИ: {aiPlayer.GetCardCount()}");
    }
}