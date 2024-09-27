using UnityEngine;
using UnityEngine.UI;

public class ChangeTurn : MonoBehaviour
{
    public static ChangeTurn Instance { get; private set; }

    public Text turnPlayer1;
    public Text turnPlayer2;
    public Image player1BG;
    public Image player2BG;

    public CanvasGroup[] player1Cards;
    public CanvasGroup[] player2Cards;

    public static bool IsPlayer1Turn { get; private set; }

    private Vector2Int lastPlayerPosition;  // Переменная для хранения последней позиции хода игрока

    // Переменная для подсчета количества шагов (ходов)
    private int moveCount = 0;

    // Максимальное количество ходов
    private const int maxMoves = 18;

    // Событие для пустой колоды
    public delegate void DeckEmptyHandler();
    public static event DeckEmptyHandler OnDeckEmpty;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        // Если EnemyAI не найден, выбор хода определяется случайно
        int t = Random.Range(0, 100);
        SetTurn(t <= 50);
    }

    public void SetTurn(bool isPlayer1)
    {
        IsPlayer1Turn = isPlayer1;

        if (isPlayer1)
        {
            turnPlayer1.gameObject.SetActive(true);
            turnPlayer2.gameObject.SetActive(false);
            player1BG.color = Color.green;
            player2BG.color = Color.red;
            SetCardInteractability(player1Cards, true);
            SetCardInteractability(player2Cards, false);
        }
        else
        {
            turnPlayer1.gameObject.SetActive(false);
            turnPlayer2.gameObject.SetActive(true);
            player1BG.color = Color.red;
            player2BG.color = Color.green;
            SetCardInteractability(player1Cards, false);
            SetCardInteractability(player2Cards, true);
        }
    }

    // Метод для переключения хода
    public void SwitchTurn(Vector2Int playerPosition)
    {
        // Сохраняем позицию, где игрок совершил последний ход
        lastPlayerPosition = playerPosition;

        // Переключаем ход
        SetTurn(!IsPlayer1Turn);

        // Увеличиваем счетчик ходов
        moveCount++;

        // Проверяем, если достигли максимального количества ходов
        if (moveCount >= maxMoves)
        {
            Debug.Log("Достигнуто максимальное количество ходов. Вызов события OnDeckEmpty.");

            // Вызываем событие, если подписчики существуют

                OnDeckEmpty.Invoke();
            
        }
    }

    private void SetCardInteractability(CanvasGroup[] cardGroups, bool isInteractable)
    {
        foreach (CanvasGroup cardGroup in cardGroups)
        {
            cardGroup.blocksRaycasts = isInteractable;
        }
    }
}












/*
using UnityEngine;
using UnityEngine.UI;

public class ChangeTurn : MonoBehaviour
{
    public static ChangeTurn Instance { get; private set; }

    public Text turnPlayer1;
    public Text turnPlayer2;
    public Image player1BG;
    public Image player2BG;

    public CanvasGroup[] player1Cards;
    public CanvasGroup[] player2Cards;

    public static bool IsPlayer1Turn { get; private set; }

    private Vector2Int lastPlayerPosition;  // Переменная для хранения последней позиции хода игрока
    public delegate void DeckEmptyHandler();
    public static event DeckEmptyHandler OnDeckEmpty;


    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {        
            // Если EnemyAI не найден, выбор хода определяется случайно
            int t = Random.Range(0, 100);
            SetTurn(t <= 50);        
    }

    public void SetTurn(bool isPlayer1)
    {
        IsPlayer1Turn = isPlayer1;

        if (isPlayer1)
        {
            turnPlayer1.gameObject.SetActive(true);
            turnPlayer2.gameObject.SetActive(false);
            player1BG.color = Color.green;
            player2BG.color = Color.red;
            SetCardInteractability(player1Cards, true);
            SetCardInteractability(player2Cards, false);
        }
        else
        {
            turnPlayer1.gameObject.SetActive(false);
            turnPlayer2.gameObject.SetActive(true);
            player1BG.color = Color.red;
            player2BG.color = Color.green;
            SetCardInteractability(player1Cards, false);
            SetCardInteractability(player2Cards, true);
        }
    }

    // Метод для переключения хода
    public void SwitchTurn(Vector2Int playerPosition)
    {
        // Сохраняем позицию, где игрок совершил последний ход
        lastPlayerPosition = playerPosition;

        // Переключаем ход
        SetTurn(!IsPlayer1Turn);        
    }

    private void SetCardInteractability(CanvasGroup[] cardGroups, bool isInteractable)
    {
        foreach (CanvasGroup cardGroup in cardGroups)
        {
            cardGroup.blocksRaycasts = isInteractable;
        }
    }
}
*/