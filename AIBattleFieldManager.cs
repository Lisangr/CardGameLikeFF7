using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class AIBattleFieldManager : MonoBehaviour
{
    [Header("UI Grid Settings")]
    public GridLayoutGroup gridLayoutGroup;
    [SerializeField] private GameObject gridCellPrefab; // Префаб с AIGridCell

    public enum GridType
    {
        Grid3x3,
        Grid3x6,
        Grid4x8
    }

    [Header("Grid Configuration")]
    public GridType currentGridType = GridType.Grid3x3;

    public AIGridCell[,] gridCells;

    private void Start()
    {
        // Проверяем необходимые компоненты
        if (gridLayoutGroup == null)
        {
            gridLayoutGroup = GetComponent<GridLayoutGroup>();
            if (gridLayoutGroup == null)
            {
                Debug.LogError("GridLayoutGroup component is missing!");
                return;
            }
        }

        // Проверяем наличие префаба
        if (gridCellPrefab == null)
        {
            Debug.LogError("Grid Cell Prefab is not assigned! Please assign it in the inspector.");
            return;
        }

        ConfigureGridLayout();
        GenerateGrid();
        SetNeighbors();
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
                gridLayoutGroup.constraintCount = 6;
                break;
            case GridType.Grid4x8:
                gridLayoutGroup.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
                gridLayoutGroup.constraintCount = 8;
                break;
        }
    }

    void GenerateGrid()
    {
        int columns, rows;
        GetGridDimensions(out columns, out rows);

        // Инициализируем массив ячеек
        gridCells = new AIGridCell[columns, rows];

        // Удаляем существующие ячейки
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }

        // Создаем новые ячейки
        for (int y = 0; y < rows; y++)
        {
            for (int x = 0; x < columns; x++)
            {
                GameObject cellObject = Instantiate(gridCellPrefab, transform);
                AIGridCell cell = cellObject.GetComponent<AIGridCell>();

                if (cell == null)
                {
                    Debug.LogError("AIGridCell component not found on prefab!");
                    continue;
                }

                cell.gridPosition = new Vector2Int(x, y);
                gridCells[x, y] = cell;

                // Устанавливаем имя для удобства отладки
                cellObject.name = $"Cell [{x},{y}]";
            }
        }

        Debug.Log($"Created grid {columns}x{rows}");
    }

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
                columns = 3;
                rows = 3;
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
                AIGridCell currentCell = gridCells[x, y];

                if (currentCell != null)
                {
                    if (x > 0) currentCell.leftNeighbor = gridCells[x - 1, y];
                    if (x < columns - 1) currentCell.rightNeighbor = gridCells[x + 1, y];
                    if (y > 0) currentCell.bottomNeighbor = gridCells[x, y - 1];
                    if (y < rows - 1) currentCell.topNeighbor = gridCells[x, y + 1];
                }
            }
        }
    }

    public List<AIGridCell> GetEmptyCells()
    {
        List<AIGridCell> emptyCells = new List<AIGridCell>();
        int columns = gridCells.GetLength(0);
        int rows = gridCells.GetLength(1);

        for (int x = 0; x < columns; x++)
        {
            for (int y = 0; y < rows; y++)
            {
                if (gridCells[x, y] != null && !gridCells[x, y].HasCard())
                {
                    emptyCells.Add(gridCells[x, y]);
                }
            }
        }

        return emptyCells;
    }

    public List<AIGridCell> GetAllCells()
    {
        List<AIGridCell> allCells = new List<AIGridCell>();
        int columns = gridCells.GetLength(0);
        int rows = gridCells.GetLength(1);

        for (int x = 0; x < columns; x++)
        {
            for (int y = 0; y < rows; y++)
            {
                if (gridCells[x, y] != null)
                {
                    allCells.Add(gridCells[x, y]);
                }
            }
        }

        return allCells;
    }
}