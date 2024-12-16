using System.Collections.Generic;
using UnityEngine;

public class MapGame : MonoBehaviour
{
    [Tooltip("The size of the grid.")] public Vector2Int gridSize = new(5, 5);

    [Tooltip("The size of each cell in the grid.")]
    public float cellSize = 1f;

    [Tooltip("The background sprite for each cell.")]
    public Sprite cellBackground;

    [SerializeField] private CellHighlighter cellHighlighter;

    private Item[,] grid;

    private void Start()
    {
        CreateMap();
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;

        var offsetX = gridSize.x * cellSize / 2;
        var offsetY = gridSize.y * cellSize / 2;

        for (var x = 0; x <= gridSize.x; x++)
            Gizmos.DrawLine(
                transform.position + new Vector3(x * cellSize - offsetX, -offsetY, 0),
                transform.position + new Vector3(x * cellSize - offsetX, gridSize.y * cellSize - offsetY, 0)
            );

        for (var y = 0; y <= gridSize.y; y++)
            Gizmos.DrawLine(
                transform.position + new Vector3(-offsetX, y * cellSize - offsetY, 0),
                transform.position + new Vector3(gridSize.x * cellSize - offsetX, y * cellSize - offsetY, 0)
            );

        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(
            transform.position,
            new Vector3(gridSize.x * cellSize, gridSize.y * cellSize, 0)
        );
    }

    private void CreateMap()
    {
        GenerateGrid();
    }

    private void GenerateGrid()
    {
        foreach (Transform child in transform) DestroyImmediate(child.gameObject);

        grid = new Item[gridSize.x, gridSize.y];

        var offsetX = gridSize.x * cellSize / 2;
        var offsetY = gridSize.y * cellSize / 2;

        for (var x = 0; x < gridSize.x; x++)
        for (var y = 0; y < gridSize.y; y++)
        {
            var cell = new GameObject($"Cell_{x}_{y}");
            cell.transform.SetParent(transform);

            cell.transform.localPosition = new Vector3(
                x * cellSize - offsetX + cellSize / 2,
                y * cellSize - offsetY + cellSize / 2,
                0
            );

            var spriteRenderer = cell.AddComponent<SpriteRenderer>();
            spriteRenderer.sprite = cellBackground;
            spriteRenderer.transform.localScale = new Vector3(cellSize, cellSize, 1);

            cellHighlighter.cells.Add(spriteRenderer);

            var collider = cell.AddComponent<BoxCollider2D>();
            collider.isTrigger = true;
            collider.size = new Vector3(cellSize - 0.2f, cellSize - 0.2f, 1);

            var gridCell = cell.AddComponent<GridCell>();

            gridCell.defaultColor = Color.white;
            gridCell.hoverColor = Color.green;

            gridCell.position = new Vector2Int(x, y);

            gridCell.x = x;
            gridCell.y = y;

            grid[x, y] = null;
        }
    }

    public void PlaceItem(List<GridCell> listCellIndex, Item itemData)
    {
        ClearItemFromGrid(itemData);

        foreach (var cell in listCellIndex)
            if (grid[cell.x, cell.y] != null && grid[cell.x, cell.y] != itemData)
            {
                Debug.LogWarning($"Cannot place item {itemData.name} at occupied cell ({cell.x}, {cell.y})!");
                return;
            }

        foreach (var cell in listCellIndex)
        {
            cell.SetOccupied(true);
            grid[cell.x, cell.y] = itemData;
        }
    }


    public bool CanPlaceItem(List<SpriteRenderer> overlappingCells, Item currentItem = null)
    {
        foreach (var cell in overlappingCells)
        {
            var gridCell = cell.GetComponent<GridCell>();
            if (gridCell == null) return false;

            // Nếu ô đã bị chiếm bởi item khác thì không thể đặt
            if (gridCell.IsOccupied() && grid[gridCell.x, gridCell.y] != currentItem)
                return false;
        }

        return true;
    }

    public void HandleOnDragItem(bool isDragging, Item item, Vector2Int itemSize)
    {
        if (isDragging)
            cellHighlighter.HandleDragging(true, item, itemSize);
        else
            cellHighlighter.HandleDragging(false);
    }

    private GridCell GetGridCell(int x, int y)
    {
        var cellObj = transform.Find($"Cell_{x}_{y}");
        return cellObj != null ? cellObj.GetComponent<GridCell>() : null;
    }

    public void ClearItemFromGrid(Item itemData)
    {
        for (var x = 0; x < gridSize.x; x++)
        for (var y = 0; y < gridSize.y; y++)
            if (grid[x, y] == itemData)
            {
                grid[x, y] = null; // Xóa item khỏi ô
                var cell = GetGridCell(x, y);
                if (cell != null) cell.SetOccupied(false); // Đặt trạng thái ô thành chưa chiếm
            }
    }
}