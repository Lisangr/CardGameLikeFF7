using UnityEngine;
using UnityEngine.UI;

public class ChangeTurn : MonoBehaviour
{
    public static ChangeTurn Instance { get; private set; }

    public Text turnPlayer1;
    public Text turnPlayer2;
/*    public Image player1BG;
    public Image player2BG;*/

    public CanvasGroup[] player1Cards;
    public CanvasGroup[] player2Cards;

    public static bool IsPlayer1Turn { get; private set; }

    private Vector2Int lastPlayerPosition;  // Координаты для хранения последней позиции хода игрока

    // Переменная для подсчета количества ходов (очков)
    private int moveCount = 0;

    // Максимальное количество ходов
    private const int maxMoves = 18;

    // Событие для конца партии
    public delegate void DeckEmptyHandler();
    public static event DeckEmptyHandler OnDeckEmpty;

    // Событие смены хода
    public delegate void TurnChangedHandler();
    public event TurnChangedHandler OnTurnChanged;

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

        // Устанавливаем цвета с 10% прозрачности
        Color player1Color = new Color(0, 1, 0, 0.05f); // Зеленый с 10% прозрачности
        Color player2Color = new Color(1, 0, 0, 0.05f); // Красный с 10% прозрачности

        if (isPlayer1)
        {
            turnPlayer1.gameObject.SetActive(true);
            turnPlayer2.gameObject.SetActive(false);
          /*  player1BG.color = player1Color; // Установка зеленого с 10% прозрачности
            player2BG.color = player2Color; // Установка красного с 10% прозрачности*/
            SetCardInteractability(player1Cards, true);
            SetCardInteractability(player2Cards, false);
        }
        else
        {
            turnPlayer1.gameObject.SetActive(false);
            turnPlayer2.gameObject.SetActive(true);
           /* player1BG.color = player2Color; // Установка красного с 10% прозрачности
            player2BG.color = player1Color; // Установка зеленого с 10% прозрачности*/
            SetCardInteractability(player1Cards, false);
            SetCardInteractability(player2Cards, true);
        }
    }

    public void SwitchTurn(Vector2Int playerPosition)
    {
        lastPlayerPosition = playerPosition;
        SetTurn(!IsPlayer1Turn);

        // Вызываем событие смены хода
        if (OnTurnChanged != null)
        {
            OnTurnChanged.Invoke();
            Debug.Log($"Ход сменился. Сейчас ход игрока {(IsPlayer1Turn ? "1" : "2 (ИИ)")}");
        }

        moveCount++;
        if (moveCount >= maxMoves)
        {
            OnDeckEmpty?.Invoke();
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