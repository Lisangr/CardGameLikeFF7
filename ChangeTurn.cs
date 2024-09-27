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

    private Vector2Int lastPlayerPosition;  // ���������� ��� �������� ��������� ������� ���� ������

    // ���������� ��� �������� ���������� ����� (�����)
    private int moveCount = 0;

    // ������������ ���������� �����
    private const int maxMoves = 18;

    // ������� ��� ������ ������
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
        // ���� EnemyAI �� ������, ����� ���� ������������ ��������
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

    // ����� ��� ������������ ����
    public void SwitchTurn(Vector2Int playerPosition)
    {
        // ��������� �������, ��� ����� �������� ��������� ���
        lastPlayerPosition = playerPosition;

        // ����������� ���
        SetTurn(!IsPlayer1Turn);

        // ����������� ������� �����
        moveCount++;

        // ���������, ���� �������� ������������� ���������� �����
        if (moveCount >= maxMoves)
        {
            Debug.Log("���������� ������������ ���������� �����. ����� ������� OnDeckEmpty.");

            // �������� �������, ���� ���������� ����������

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

    private Vector2Int lastPlayerPosition;  // ���������� ��� �������� ��������� ������� ���� ������
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
            // ���� EnemyAI �� ������, ����� ���� ������������ ��������
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

    // ����� ��� ������������ ����
    public void SwitchTurn(Vector2Int playerPosition)
    {
        // ��������� �������, ��� ����� �������� ��������� ���
        lastPlayerPosition = playerPosition;

        // ����������� ���
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