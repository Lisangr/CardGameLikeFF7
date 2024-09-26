using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleFieldGridManagerForPlayerVsPlayer : MonoBehaviour
{
    [Header("UI Grid Settings")]
    public GridLayoutGroup gridLayoutGroup;  // Компонент Grid Layout Group
    public GameObject gridCellPrefab;        // Префаб ячейки сетки с рамками

    public enum GridType
    {
        Grid3x3,
        Grid3x6,
        Grid4x8
    }

    [Header("Grid Configuration")]
    public GridType currentGridType;  // Тип сетки для BattleFieldBG

    public GridCellForPlayerVsPlayer[,] gridCells;    // Двумерный массив для хранения ячеек

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
        SetNeighbors(); // Устанавливаем соседей для каждой ячейки
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

        // Инициализируем массив ячеек
        gridCells = new GridCellForPlayerVsPlayer[numberOfColumns, numberOfRows];

        // Очищаем старые ячейки, если они есть
        foreach (Transform child in gridLayoutGroup.transform)
        {
            Destroy(child.gameObject);
        }

        // Спавним новые ячейки и сохраняем их в массиве
        for (int y = 0; y < numberOfRows; y++)  // Перебор по строкам (y)
        {
            for (int x = 0; x < numberOfColumns; x++)  // Перебор по столбцам (x)
            {
                GameObject cell = Instantiate(gridCellPrefab, gridLayoutGroup.transform);
                gridCells[x, y] = cell.GetComponent<GridCellForPlayerVsPlayer>(); // Присваиваем ячейку в массив
                gridCells[x, y].gridPosition = new Vector2Int(x, y); // Задаем позицию ячейки
            }
        }
    }

    // Получаем размеры сетки на основе выбранного типа
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

                if (x > 0) currentCell.leftNeighbor = gridCells[x - 1, y];  // Левый сосед
                if (x < columns - 1) currentCell.rightNeighbor = gridCells[x + 1, y];  // Правый сосед
                if (y > 0) currentCell.bottomNeighbor = gridCells[x, y - 1];  // Нижний сосед
                if (y < rows - 1) currentCell.topNeighbor = gridCells[x, y + 1];  // Верхний сосед
            }
        }
    }


    // Новый метод для получения всех ячеек в одномерном списке
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
