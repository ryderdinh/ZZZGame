using UnityEngine;

public class GridCell : MonoBehaviour
{
    [Tooltip("Color when an item is hovering over the cell.")]
    public Color hoverColor = Color.green;

    [Tooltip("Default color of the cell.")]
    public Color defaultColor = Color.white;

    [Tooltip("Grid position of this cell.")]
    public Vector2Int position;

    public int x;
    public int y;

    private bool isOccupied;

    private SpriteRenderer spriteRenderer;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.color = defaultColor;

        var boxCollider2D = GetComponent<BoxCollider2D>();
        boxCollider2D.isTrigger = true;
    }

    public bool IsOccupied()
    {
        return isOccupied;
    }

    public void SetOccupied(bool occupied)
    {
        isOccupied = occupied;
    }
}