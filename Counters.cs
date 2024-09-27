using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.UI;
using YG;
using static CardManager;

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
        GridCellForPlayerVsPlayer.OnCardTookByPlayer1 += GridCellForPlayerVsPlayer_OnCardTookByPlayer1;
        GridCellForPlayerVsPlayer.OnCardTookByPlayer2 += GridCellForPlayerVsPlayer_OnCardTookByPlayer2;
        ChangeTurn.OnDeckEmpty += OnDeckEmptyHandler;  // ������������� �� �������, ����� ������ �����
    }

    private void GridCellForPlayerVsPlayer_OnCardTookByPlayer2()
    {
        counter2++;
        counterPlayer2.text = counter2.ToString();
    }

    private void GridCellForPlayerVsPlayer_OnCardTookByPlayer1()
    {
        counter1++;
        counterPlayer1.text = counter1.ToString();
    }
    // ��������� �������, ����� ������ �����
    private void OnDeckEmptyHandler()
    {
        Debug.Log("��� ������ �����, ���������� ����������...");

        if (counter1 > counter2)
        {
            Debug.Log("����� 1 �������!");
            player1WinPanel.SetActive(true);  // ���������� ������ ������ ������ 1
            SaveScore();
        }
        else if (counter2 > counter1)
        {
            Debug.Log("����� 2 �������!");
            player2WinPanel.SetActive(true);  // ���������� ������ ������ ������ 2
        }
        else
        {
            Debug.Log("�����!");  // �������� ������ ��� �����, ���� ���������
            nobodyWinPanel.SetActive(true);
        }
    }
    private void OnDisable()
    {
        GridCellForPlayerVsPlayer.OnCardTookByPlayer1 -= GridCellForPlayerVsPlayer_OnCardTookByPlayer1;
        GridCellForPlayerVsPlayer.OnCardTookByPlayer2 -= GridCellForPlayerVsPlayer_OnCardTookByPlayer2;
        ChangeTurn.OnDeckEmpty -= OnDeckEmptyHandler;  // ������������ �� �������
    }
    private void SaveScore()
    {
        YandexGame.savesData.totalScore = score;
        YandexGame.SaveProgress();
    }

    private void AddScoreLeaderboard()
    {
        YandexGame.NewLeaderboardScores("Cards", score);
    }

}
