using UnityEngine;

public class Tile : MonoBehaviour
{
    public bool isStartTile; // Ô bắt đầu
    public bool isConnected; // Ô này đã được nối hay chưa
    public bool isObstacle; // Ô này là chướng ngại vật
    public bool isPartOfCorrectPath; // Ô này là một phần của đường đi đúng
    public int gridX, gridY; // Vị trí của ô trong lưới
    private readonly Color connectedColor = Color.yellow;

    private readonly Color defaultColor = Color.white;
    private readonly Color hintColor = Color.cyan; // Màu cho ô gợi ý
    private readonly Color obstacleColor = Color.black; // Màu cho ô chướng ngại vật
    private Color correctPathColor = Color.blue; // Màu cho đường đi đúng (nếu cần hiển thị)

    private SpriteRenderer spriteRenderer; // Để thay đổi màu sắc của ô

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        // Nếu là chướng ngại vật, tô màu đen
        if (isObstacle)
        {
            spriteRenderer.color = obstacleColor;
        }
        else
        {
            // Ban đầu, tô màu trắng cho tất cả các ô
            spriteRenderer.color = defaultColor;

            // Nếu là ô bắt đầu, tô màu xanh lá
            if (isStartTile)
            {
                spriteRenderer.color = Color.green;
                isConnected = true; // Điểm bắt đầu luôn được nối
            }
        }
    }

    // Đánh dấu ô này đã được nối
    public void ConnectTile()
    {
        if (!isObstacle) // Không thể nối nếu là chướng ngại vật
        {
            isConnected = true;
            spriteRenderer.color = connectedColor; // Thay đổi màu để chỉ ra rằng ô đã được nối
        }
    }

    // Đánh dấu ô này là ô gợi ý (highlight bằng màu khác)
    public void ShowHint()
    {
        if (!isConnected && isPartOfCorrectPath) spriteRenderer.color = hintColor; // Đổi màu ô để hiển thị gợi ý
    }

    // Xóa gợi ý (đặt lại màu ban đầu)
    public void ClearHint()
    {
        if (!isConnected && isPartOfCorrectPath)
            spriteRenderer.color = defaultColor; // Đặt lại màu trắng nếu ô chưa được nối
    }

    public bool IsAdjacentToCorrectTile(Tile otherTile)
    {
        // Nếu ô này hoặc ô kia là chướng ngại vật, hoặc không nằm trên đường đi đúng, trả về false
        if (isObstacle || otherTile.isObstacle || !otherTile.isPartOfCorrectPath) return false;

        // Kiểm tra khoảng cách giữa hai ô (liền kề theo trục X hoặc Y)
        var dx = Mathf.Abs(gridX - otherTile.gridX);
        var dy = Mathf.Abs(gridY - otherTile.gridY);

        return (dx == 1 && dy == 0) || (dx == 0 && dy == 1);
    }
}