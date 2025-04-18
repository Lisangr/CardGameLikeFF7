using UnityEngine;
using UnityEngine.UI;
using YG;

public class Counters : MonoBehaviour
{
    public Text counterPlayer1;
    public Text counterPlayer2;
    public GameObject player1WinPanel;
    public GameObject player2WinPanel;
    public GameObject nobodyWinPanel;
    private int counter1 = 0;
    private int counter2 = 0;

    private int score = 0;

    private void Awake()
    {
        player1WinPanel.SetActive(false);
        player2WinPanel.SetActive(false);
        nobodyWinPanel.SetActive(false);
    }

    void Start()
    {
        counterPlayer1.text = counter1.ToString();
        counterPlayer2.text = counter2.ToString();
    }

    private void OnEnable()
    {
        // ������������� �� ������� ����� ����� �����
        GridCellForPlayerVsPlayer.OnCardTookByPlayer1 += CardTookByPlayer1;
        GridCellForPlayerVsPlayer.OnCardTookByPlayer2 += CardTookByPlayer2;

        // ��������� �������� �� ������� �� AIGridCell
        AIGridCell.OnCardTookByPlayer1 += CardTookByPlayer1;
        AIGridCell.OnCardTookByPlayer2 += CardTookByPlayer2;

        ChangeTurn.OnDeckEmpty += OnDeckEmptyHandler;
    }

    private void CardTookByPlayer2()
    {
        counter2++;
        counterPlayer2.text = counter2.ToString();
        Debug.Log($"����� 2 (��) �������� �����. ����� ���������: {counter2}");
    }

    private void CardTookByPlayer1()
    {
        counter1++;
        counterPlayer1.text = counter1.ToString();
        Debug.Log($"����� 1 �������� �����. ����� ���������: {counter1}");
    }

    private void OnDeckEmptyHandler()
    {
        Debug.Log("��� ������ �����, ���������� ����������...");

        if (counter1 > counter2)
        {
            Debug.Log("����� 1 �������!");
            player1WinPanel.SetActive(true);
            SaveScore();
        }
        else if (counter2 > counter1)
        {
            Debug.Log("����� 2 (��) �������!");
            player2WinPanel.SetActive(true);
        }
        else
        {
            Debug.Log("�����!");
            nobodyWinPanel.SetActive(true);
        }
    }

    private void OnDisable()
    {
        // ������������ �� ������� ����� ����� �����
        GridCellForPlayerVsPlayer.OnCardTookByPlayer1 -= CardTookByPlayer1;
        GridCellForPlayerVsPlayer.OnCardTookByPlayer2 -= CardTookByPlayer2;

        // ������������ �� ������� AIGridCell
        AIGridCell.OnCardTookByPlayer1 -= CardTookByPlayer1;
        AIGridCell.OnCardTookByPlayer2 -= CardTookByPlayer2;

        ChangeTurn.OnDeckEmpty -= OnDeckEmptyHandler;
    }

    private void SaveScore()
    {
        score = counter1; // ������������� ���� ������ 1 ��� ��������
        YandexGame.savesData.totalScore = score;
        YandexGame.SaveProgress();
        AddScoreLeaderboard();
    }

    private void AddScoreLeaderboard()
    {
        YandexGame.NewLeaderboardScores("Cards", score);
    }
}