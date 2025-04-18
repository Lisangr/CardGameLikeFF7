using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleFieldGridManager : MonoBehaviour
{
    public enum GridType
    {
        Grid3x3,
        Grid3x6,
        Grid4x8
    }

    [Header("UI Grid Settings")]
    public GridLayoutGroup gridLayoutGroup;
    [SerializeField] private GameObject gridCellPrefab; // ��������� SerializeField


    [Header("Grid Configuration")]
    public GridType currentGridType = GridType.Grid3x3;

    public GridCell[,] gridCells;

    private void Start()
    {
        // ��������� ����������� ����������
        if (gridLayoutGroup == null)
        {
            gridLayoutGroup = GetComponent<GridLayoutGroup>();
            if (gridLayoutGroup == null)
            {
                Debug.LogError("GridLayoutGroup component is missing!");
                return;
            }
        }

        // ��������� ������� �������
        if (gridCellPrefab == null)
        {
            Debug.LogError("Grid Cell Prefab is not assigned! Please assign it in the inspector.");
            return;
        }

        ConfigureGridLayout();
        GenerateGrid();
        SetNeighbors();
    }

    void GenerateGrid()
    {
        if (gridCellPrefab == null)
        {
            Debug.LogError("Grid Cell Prefab is null!");
            return;
        }

        int columns, rows;
        GetGridDimensions(out columns, out rows);

        // �������������� ������ �����
        gridCells = new GridCell[columns, rows];

        // ������� ������������ ������
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }

        // ������� ����� ������
        for (int y = 0; y < rows; y++)
        {
            for (int x = 0; x < columns; x++)
            {
                GameObject cellObject = Instantiate(gridCellPrefab, transform);
                GridCell cell = cellObject.GetComponent<GridCell>();

                if (cell == null)
                {
                    Debug.LogError("GridCell component not found on prefab!");
                    continue;
                }

                cell.gridPosition = new Vector2Int(x, y);
                gridCells[x, y] = cell;

                // ������������� ��� ��� �������� �������
                cellObject.name = $"Cell [{x},{y}]";
            }
        }

        Debug.Log($"Created grid {columns}x{rows}");
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
                GridCell currentCell = gridCells[x, y];

                if (x > 0) currentCell.leftNeighbor = gridCells[x - 1, y];  // ����� �����
                if (x < columns - 1) currentCell.rightNeighbor = gridCells[x + 1, y];  // ������ �����
                if (y > 0) currentCell.bottomNeighbor = gridCells[x, y - 1];  // ������ �����
                if (y < rows - 1) currentCell.topNeighbor = gridCells[x, y + 1];  // ������� �����
            }
        }
    }


    // ����� ����� ��� ��������� ���� ����� � ���������� ������
    public List<GridCell> GetAllCells()
    {
        List<GridCell> allCells = new List<GridCell>();
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
