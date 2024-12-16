using System.Collections.Generic;
using UnityEngine;

public class CellHighlighter : MonoBehaviour
{
    [SerializeField] private MapGame _mapGame;

    public List<SpriteRenderer> cells = new();
    public Color highlightColor = Color.green;
    public Color invalidHighlightColor = Color.red;
    public Color defaultColor = Color.white;
    public Vector2 cellSize = new(1, 1);
    private readonly List<int> _loggedIndices = new();

    private Item _dragItem;
    private List<SpriteRenderer> _highlightedCells = new();
    private Vector3 _initialItemPosition;
    private bool _isDragging;
    private Vector2Int _itemSize = Vector2Int.one;
    private Vector3? _validPlacementPosition;

    private void Start()
    {
        if (_mapGame == null)
            Debug.LogError("MapGame not found!");
    }

    private void FixedUpdate()
    {
        if (_isDragging) UpdateHighlights();
    }

    public void HandleDragging(bool isDragging, Item dragItem = null, Vector2Int itemSize = default)
    {
        if (isDragging)
        {
            if (dragItem != null)
            {
                _dragItem = dragItem;
                _initialItemPosition = dragItem.initialPosition;
            }

            _itemSize = itemSize;
            _isDragging = true;
        }
        else
        {
            if (_validPlacementPosition.HasValue)
            {
                _dragItem.transform.position = _validPlacementPosition.Value;

                var listGridCell = new List<GridCell>();
                foreach (var _ in _loggedIndices) listGridCell.Add(cells[_].GetComponent<GridCell>());

                _mapGame.PlaceItem(listGridCell, dragItem);
            }
            else
            {
                _dragItem.transform.position = _initialItemPosition;

                Debug.LogWarning("Invalid placement. Returning item to its original position.");
            }

            ClearHighlights();
            _dragItem = null;
            _isDragging = false;
        }
    }

    private void UpdateHighlights()
    {
        if (_dragItem == null || _mapGame == null) return;

        var overlappingCells = FindOverlappingCells();

        var validPlacement = _mapGame.CanPlaceItem(overlappingCells, _dragItem);

        HighlightCells(overlappingCells, validPlacement);

        if (validPlacement)
            _validPlacementPosition = CalculatePlacementPosition(overlappingCells);
        else
            _validPlacementPosition = null;
    }

    private List<SpriteRenderer> FindOverlappingCells()
    {
        var overlappingCells = new List<SpriteRenderer>();

        var itemPosition = _dragItem.transform.position;

        var itemMin = new Vector2(
            itemPosition.x - _itemSize.x / 2f * cellSize.x,
            itemPosition.y - _itemSize.y / 2f * cellSize.y
        );

        var itemMax = new Vector2(
            itemPosition.x + _itemSize.x / 2f * cellSize.x,
            itemPosition.y + _itemSize.y / 2f * cellSize.y
        );

        foreach (var cell in cells)
        {
            var cellPosition = cell.transform.position;

            if (cellPosition.x >= itemMin.x && cellPosition.x < itemMax.x &&
                cellPosition.y >= itemMin.y && cellPosition.y < itemMax.y)
                overlappingCells.Add(cell);
        }

        return overlappingCells;
    }

    private void HighlightCells(List<SpriteRenderer> cellsToHighlight, bool validPlacement)
    {
        foreach (var cell in _highlightedCells)
            if (!cellsToHighlight.Contains(cell))
                cell.color = defaultColor;

        foreach (var cell in cellsToHighlight)
        {
            cell.color = validPlacement ? highlightColor : invalidHighlightColor;

            if (validPlacement)
            {
                var index = cells.IndexOf(cell);
                if (!_loggedIndices.Contains(index)) _loggedIndices.Add(index);
            }
        }

        var idx = 0;
        while (idx < _loggedIndices.Count)
        {
            var check = false;
            foreach (var cell in cellsToHighlight)
            {
                var index = cells.IndexOf(cell);
                if (index == _loggedIndices[idx]) check = true;
            }

            if (!check)
                _loggedIndices.Remove(_loggedIndices[idx]);
            else
                idx++;
        }


        _highlightedCells = cellsToHighlight;
    }

    private Vector3 CalculatePlacementPosition(List<SpriteRenderer> overlappingCells)
    {
        var center = Vector3.zero;
        foreach (var cell in overlappingCells) center += cell.transform.position;

        return center / overlappingCells.Count;
    }

    private void ClearHighlights()
    {
        foreach (var cell in _highlightedCells) cell.color = defaultColor;

        _highlightedCells.Clear();
        _validPlacementPosition = null;
    }
}