using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleFieldGridManagerForPlayerVsPlayer : MonoBehaviour
{
    [Header("UI Grid Settings")]
    public GridLayoutGroup gridLayoutGroup;  // ��������� Grid Layout Group
    public GameObject gridCellPrefab;        // ������ ������ ����� � �������

    public enum GridType
    {
        Grid3x3,
        Grid3x6,
        Grid4x8
    }

    [Header("Grid Configuration")]
    public GridType currentGridType;  // ��� ����� ��� BattleFieldBG

    public GridCellForPlayerVsPlayer[,] gridCells;    // ��������� ������ ��� �������� �����

    private void Start()
    {
        gridLayoutGroup = GetComponent<GridLayoutGroup>();

        if (gridLayoutGroup == null)
        {
            Debug.LogError("GridLayoutGroup component is missing on BattleFieldBG!");
            return;
        }

        ConfigureGridLayout();
        GenerateGrid();
        SetNeighbors(); // ������������� ������� ��� ������ ������
    }

    void ConfigureGridLayout()
    {
        switch (currentGridType)
        {
            case GridType.Grid3x3:
                gridLayoutGroup.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
                gridLayoutGroup.constraintCount = 3;
                break;
            case GridType.Grid3x6:
                gridLayoutGroup.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
                gridLayoutGroup.constraintCount = 4;
                break;
            case GridType.Grid4x8:
                gridLayoutGroup.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
                gridLayoutGroup.constraintCount = 8;
                break;
            default:
                Debug.LogWarning("Grid type not set!");
                break;
        }
    }
    
    void GenerateGrid()
    {
        int numberOfRows, numberOfColumns;
        GetGridDimensions(out numberOfColumns, out numberOfRows);

        // �������������� ������ �����
        gridCells = new GridCellForPlayerVsPlayer[numberOfColumns, numberOfRows];

        // ������� ������ ������, ���� ��� ����
        foreach (Transform child in gridLayoutGroup.transform)
        {
            Destroy(child.gameObject);
        }

        // ������� ����� ������ � ��������� �� � �������
        for (int y = 0; y < numberOfRows; y++)  // ������� �� ������� (y)
        {
            for (int x = 0; x < numberOfColumns; x++)  // ������� �� �������� (x)
            {
                GameObject cell = Instantiate(gridCellPrefab, gridLayoutGroup.transform);
                gridCells[x, y] = cell.GetComponent<GridCellForPlayerVsPlayer>(); // ����������� ������ � ������
                gridCells[x, y].gridPosition = new Vector2Int(x, y); // ������ ������� ������
            }
        }
    }

    // �������� ������� ����� �� ������ ���������� ����
    public void GetGridDimensions(out int columns, out int rows)
    {
        switch (currentGridType)
        {
            case GridType.Grid3x3:
                columns = 3;
                rows = 3;
                break;
            case GridType.Grid3x6:
                columns = 6;
                rows = 3;
                break;
            case GridType.Grid4x8:
                columns = 8;
                rows = 4;
                break;
            default:
                columns = 0;
                rows = 0;
                break;
        }
    }

    void SetNeighbors()
    {
        int columns = gridCells.GetLength(0);
        int rows = gridCells.GetLength(1);

        for (int x = 0; x < columns; x++)
        {
            for (int y = 0; y < rows; y++)
            {
                GridCellForPlayerVsPlayer currentCell = gridCells[x, y];

                if (x > 0) currentCell.leftNeighbor = gridCells[x - 1, y];  // ����� �����
                if (x < columns - 1) currentCell.rightNeighbor = gridCells[x + 1, y];  // ������ �����
                if (y > 0) currentCell.bottomNeighbor = gridCells[x, y - 1];  // ������ �����
                if (y < rows - 1) currentCell.topNeighbor = gridCells[x, y + 1];  // ������� �����
            }
        }
    }


    // ����� ����� ��� ��������� ���� ����� � ���������� ������
    public List<GridCellForPlayerVsPlayer> GetAllCells()
    {
        List<GridCellForPlayerVsPlayer> allCells = new List<GridCellForPlayerVsPlayer>();
        int columns = gridCells.GetLength(0);
        int rows = gridCells.GetLength(1);

        for (int y = 0; y < rows; y++)
        {
            for (int x = 0; x < columns; x++)
            {
                allCells.Add(gridCells[x, y]);
            }
        }

        return allCells;
    }
}
