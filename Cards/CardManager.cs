using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardManager : MonoBehaviour
{
    public enum GameMode
    {
        PlayerVsAI,
        PlayerVsPlayer
    }
    
    [Header("Game Settings")]
    [SerializeField] private GameMode currentGameMode = GameMode.PlayerVsAI;
    
    [Header("UI Grid Settings")]
    public GridLayoutGroup player1GridLayout;  // Сетка для карт игрока 1
    public GridLayoutGroup player2GridLayout;  // Сетка для карт игрока 2 (в режиме PvP)
    public GameObject cardUIPrefab;
    
    [Header("Card Lists")]
    public List<Card> player1Cards = new List<Card>();  // Карты первого игрока
    public List<Card> player2Cards = new List<Card>();  // Карты второго игрока или ИИ
    
    [Header("AI Settings")]
    [SerializeField] private Transform aiCardsParent;  // Родительский объект для карт ИИ (используется только в PlayerVsAI)
    
    private AIPlayer aiPlayer;
    private bool cardsSpawned = false;  // Флаг для предотвращения повторного создания карт

    private void Start()
    {
        // Проверка и инициализация сетки для первого игрока
        if (player1GridLayout == null)
        {
            player1GridLayout = GetComponent<GridLayoutGroup>();
            
            if (player1GridLayout == null)
            {
                Debug.LogError("Не найдена сетка для карт первого игрока!");
                return;
            }
            
            Debug.Log($"Найдена сетка для карт первого игрока: {player1GridLayout.gameObject.name}");
        }
        
        // Настройка в зависимости от режима игры
        if (currentGameMode == GameMode.PlayerVsAI)
        {
            aiPlayer = FindObjectOfType<AIPlayer>();
            if (aiPlayer == null)
            {
                Debug.LogError("AIPlayer не найден на сцене! Режим PlayerVsAI не может быть активирован.");
            }
        }
        else // PlayerVsPlayer
        {
            // Проверка сетки для второго игрока в режиме PvP
            if (player2GridLayout == null)
            {
                Debug.LogWarning("Не назначена сетка для карт второго игрока в режиме PvP!");
            }
        }
        
        // Инициализация списков карт
        if (player1Cards == null) player1Cards = new List<Card>();
        if (player2Cards == null) player2Cards = new List<Card>();
        
        // Создание карт, если ещё не созданы
        if (!cardsSpawned)
        {
            SpawnPlayer1Cards();
            
            if (currentGameMode == GameMode.PlayerVsAI)
            {
                SpawnAICards();
            }
            else
            {
                SpawnPlayer2Cards();
            }
            
            cardsSpawned = true;
            
            // Логирование созданных карт
            Invoke("LogCardsInfo", 0.5f);
        }
    }

    // Создание карт для первого игрока
    private void SpawnPlayer1Cards()
    {
        // Очистка списка карт первого игрока
        foreach (Card card in player1Cards)
        {
            if (card != null && card.gameObject != null)
            {
                Destroy(card.gameObject);
            }
        }
        player1Cards.Clear();
        
        // Создание 9 карт для первого игрока
        for (int i = 0; i < 9; i++)
        {
            GameObject cardObject = Instantiate(cardUIPrefab, player1GridLayout.transform);
            cardObject.name = $"Player1_Card_{i}";
            
            Card cardDisplay = cardObject.GetComponent<Card>();
            if (cardDisplay == null)
            {
                Debug.LogError("Компонент Card не найден на префабе карты игрока 1!");
                continue;
            }
            
            // Задаем случайные значения для карты
            cardDisplay.leftValue = Random.Range(1, 10);
            cardDisplay.rightValue = Random.Range(1, 10);
            cardDisplay.topValue = Random.Range(1, 10);
            cardDisplay.bottomValue = Random.Range(1, 10);
            cardDisplay.isPlayer1Card = true;  // Это карта первого игрока
            
            // Обновляем отображение значений на карте
            cardDisplay.SetCard();
            
            // Активируем компонент DraggableCard
            DraggableCard draggable = cardObject.GetComponent<DraggableCard>();
            if (draggable != null)
            {
                draggable.enabled = true;
            }
            
            // Добавляем карту в список
            player1Cards.Add(cardDisplay);
            
            Debug.Log($"Создана карта игрока 1 (#{i}): L:{cardDisplay.leftValue} R:{cardDisplay.rightValue} T:{cardDisplay.topValue} B:{cardDisplay.bottomValue}");
        }
        
        Debug.Log($"Создано {player1Cards.Count} карт для игрока 1");
    }

    // Создание карт для ИИ
    public void SpawnAICards()
    {
        // Проверка существования AIPlayer
        if (aiPlayer == null)
        {
            aiPlayer = FindObjectOfType<AIPlayer>();
            if (aiPlayer == null)
            {
                Debug.LogError("AIPlayer не найден на сцене!");
                return;
            }
        }
        
        // Очистка существующих карт у ИИ
        aiPlayer.ClearCards();
        
        // Проверка существования родительского объекта
        if (aiCardsParent == null)
        {
            Debug.LogError("Родительский объект для карт ИИ не назначен!");
            return;
        }
        
        // Создание 9 карт для ИИ
        for (int i = 0; i < 9; i++)
        {
            GameObject cardObject = Instantiate(cardUIPrefab, aiCardsParent);
            cardObject.name = $"AI_Card_{i}";
            
            Card cardDisplay = cardObject.GetComponent<Card>();
            if (cardDisplay == null)
            {
                Debug.LogError("Компонент Card не найден на префабе карты ИИ!");
                continue;
            }
            
            // Задаем случайные значения для карты
            cardDisplay.leftValue = Random.Range(1, 10);
            cardDisplay.rightValue = Random.Range(1, 10);
            cardDisplay.topValue = Random.Range(1, 10);
            cardDisplay.bottomValue = Random.Range(1, 10);
            cardDisplay.isPlayer1Card = false;  // Это карта ИИ
            
            // Обновляем отображение значений на карте
            cardDisplay.SetCard();
            
            // Добавляем карту ИИ
            aiPlayer.AddCard(cardDisplay);
            
            Debug.Log($"Создана карта ИИ (#{i}): L:{cardDisplay.leftValue} R:{cardDisplay.rightValue} T:{cardDisplay.topValue} B:{cardDisplay.bottomValue}");
        }
        
        Debug.Log($"Созданы карты для ИИ: 9 шт. Всего карт у ИИ: {aiPlayer.GetCardCount()}");
    }

    // Создание карт для второго игрока (в режиме PvP)
    private void SpawnPlayer2Cards()
    {
        // Проверка существования сетки для второго игрока
        if (player2GridLayout == null)
        {
            Debug.LogError("Не назначена сетка для карт второго игрока!");
            return;
        }
        
        // Очистка списка карт второго игрока
        foreach (Card card in player2Cards)
        {
            if (card != null && card.gameObject != null)
            {
                Destroy(card.gameObject);
            }
        }
        player2Cards.Clear();
        
        // Создание 9 карт для второго игрока
        for (int i = 0; i < 9; i++)
        {
            GameObject cardObject = Instantiate(cardUIPrefab, player2GridLayout.transform);
            cardObject.name = $"Player2_Card_{i}";
            
            Card cardDisplay = cardObject.GetComponent<Card>();
            if (cardDisplay == null)
            {
                Debug.LogError("Компонент Card не найден на префабе карты игрока 2!");
                continue;
            }
            
            // Задаем случайные значения для карты
            cardDisplay.leftValue = Random.Range(1, 10);
            cardDisplay.rightValue = Random.Range(1, 10);
            cardDisplay.topValue = Random.Range(1, 10);
            cardDisplay.bottomValue = Random.Range(1, 10);
            cardDisplay.isPlayer1Card = false;  // Это карта второго игрока
            
            // Обновляем отображение значений на карте
            cardDisplay.SetCard();
            
            // Активируем компонент DraggableCard
            DraggableCard draggable = cardObject.GetComponent<DraggableCard>();
            if (draggable != null)
            {
                draggable.enabled = true;
            }
            
            // Добавляем карту в список
            player2Cards.Add(cardDisplay);
            
            Debug.Log($"Создана карта игрока 2 (#{i}): L:{cardDisplay.leftValue} R:{cardDisplay.rightValue} T:{cardDisplay.topValue} B:{cardDisplay.bottomValue}");
        }
        
        Debug.Log($"Создано {player2Cards.Count} карт для игрока 2");
    }

    // Логирование информации о картах
    private void LogCardsInfo()
    {
        Debug.Log("======= ИНФОРМАЦИЯ О КАРТАХ =======");
        Debug.Log($"Режим игры: {currentGameMode}");
        Debug.Log($"Карты игрока 1: {player1Cards.Count}");
        
        if (currentGameMode == GameMode.PlayerVsAI)
        {
            if (aiPlayer != null)
            {
                Debug.Log($"Карты ИИ: {aiPlayer.GetCardCount()}");
            }
            else
            {
                Debug.Log("AIPlayer не инициализирован!");
            }
        }
        else
        {
            Debug.Log($"Карты игрока 2: {player2Cards.Count}");
        }
    }

    // Публичный метод для переключения режима игры
    public void SetGameMode(GameMode mode)
    {
        currentGameMode = mode;
        cardsSpawned = false;  // Сбрасываем флаг, чтобы карты были пересозданы при смене режима
        
        // Очищаем старые карты
        foreach (Card card in player1Cards)
        {
            if (card != null && card.gameObject != null)
            {
                Destroy(card.gameObject);
            }
        }
        player1Cards.Clear();
        
        foreach (Card card in player2Cards)
        {
            if (card != null && card.gameObject != null)
            {
                Destroy(card.gameObject);
            }
        }
        player2Cards.Clear();
        
        if (aiPlayer != null)
        {
            aiPlayer.ClearCards();
        }
        
        // Создаем новые карты в соответствии с выбранным режимом
        SpawnPlayer1Cards();
        
        if (mode == GameMode.PlayerVsAI)
        {
            SpawnAICards();
        }
        else
        {
            SpawnPlayer2Cards();
        }
        
        cardsSpawned = true;
        LogCardsInfo();
    }
}