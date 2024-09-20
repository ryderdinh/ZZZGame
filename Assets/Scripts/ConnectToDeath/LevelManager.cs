using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public GameObject tilePrefab; // Prefab của các ô
    public int gridWidth = 5; // Chiều rộng lưới
    public int gridHeight = 5; // Chiều cao lưới
    public int numberOfObstacles = 3; // Số lượng chướng ngại vật trong lưới
    private readonly List<Tile> correctPath = new(); // Danh sách các ô theo đúng đường đi
    private Tile endTile;
    private Tile[,] grid; // Lưới chứa các ô
    private Tile hintTile; // Ô được gợi ý
    private bool isDragging; // Đánh dấu khi người chơi đang kéo
    private Tile lastConnectedTile; // Ô cuối cùng đã được nối

    private Tile startTile; // Ô bắt đầu

    private void Start()
    {
        GenerateGrid();
        SetStartTile();
        SetEndTile();
        PlaceObstacles();
        if (PathIsValid())
        {
            CreateCorrectPath();
            ShowHintForNextTile();
        }
        else
        {
            Debug.LogError("Không tìm được đường đi hợp lệ!");
        }
    }

    private void Update()
    {
        HandleMouseInput();
    }

    // Tạo lưới các ô vuông (Tile)
    private void GenerateGrid()
    {
        grid = new Tile[gridWidth, gridHeight];

        for (var x = 0; x < gridWidth; x++)
        for (var y = 0; y < gridHeight; y++)
        {
            var tilePosition = new Vector3(x, y, 0);
            var tileObject = Instantiate(tilePrefab, tilePosition, Quaternion.identity);
            var tile = tileObject.GetComponent<Tile>();
            tile.gridX = x;
            tile.gridY = y;
            grid[x, y] = tile;
        }
    }

    // Chọn ngẫu nhiên một ô làm ô bắt đầu
    private void SetStartTile()
    {
        var startX = Random.Range(0, gridWidth);
        var startY = Random.Range(0, gridHeight);

        startTile = grid[startX, startY];
        startTile.isStartTile = true;
        startTile.ConnectTile();
        lastConnectedTile = startTile;
    }

    private void SetEndTile()
    {
        var endX = Random.Range(0, gridWidth);
        var endY = Random.Range(0, gridHeight);

        endTile = grid[endX, endY];
        endTile.isPartOfCorrectPath = true; // Đánh dấu ô này là điểm cuối
        endTile.GetComponent<SpriteRenderer>().color = Color.red; // Tô màu đỏ cho ô kết thúc
    }

    // Đặt chướng ngại vật ngẫu nhiên trong lưới
    private void PlaceObstacles()
    {
        var placedObstacles = 0;

        while (placedObstacles < numberOfObstacles)
        {
            var obstacleX = Random.Range(0, gridWidth);
            var obstacleY = Random.Range(0, gridHeight);

            var obstacleTile = grid[obstacleX, obstacleY];

            // Không đặt chướng ngại vật tại ô bắt đầu hoặc ô kết thúc
            if (!obstacleTile.isStartTile && obstacleTile != endTile && !obstacleTile.isObstacle)
            {
                obstacleTile.isObstacle = true;
                placedObstacles++;
            }
        }
    }

    private bool PathIsValid()
    {
        return BFS(startTile, endTile);
    }

    // Sử dụng Breadth-First Search (BFS) để tìm đường
    private bool BFS(Tile start, Tile goal)
    {
        var frontier = new Queue<Tile>();
        var visited = new HashSet<Tile>();

        frontier.Enqueue(start);
        visited.Add(start);

        while (frontier.Count > 0)
        {
            var current = frontier.Dequeue();

            if (current == goal) return true; // Tìm thấy đường đến điểm cuối

            // Lấy các ô liền kề
            foreach (var neighbor in GetNeighbors(current))
                if (!visited.Contains(neighbor) && !neighbor.isObstacle)
                {
                    frontier.Enqueue(neighbor);
                    visited.Add(neighbor);
                }
        }

        return false; // Không tìm thấy đường đi
    }

    // Lấy các ô liền kề (không phải chướng ngại vật)
    private List<Tile> GetNeighbors(Tile tile)
    {
        var neighbors = new List<Tile>();

        // Kiểm tra các hướng lên, xuống, trái, phải
        if (tile.gridX > 0) neighbors.Add(grid[tile.gridX - 1, tile.gridY]);
        if (tile.gridX < gridWidth - 1) neighbors.Add(grid[tile.gridX + 1, tile.gridY]);
        if (tile.gridY > 0) neighbors.Add(grid[tile.gridX, tile.gridY - 1]);
        if (tile.gridY < gridHeight - 1) neighbors.Add(grid[tile.gridX, tile.gridY + 1]);

        return neighbors;
    }

    // Tạo đường đi đúng trong lưới
    private void CreateCorrectPath()
    {
        // Khởi tạo từ ô bắt đầu
        var currentTile = startTile;
        correctPath.Add(currentTile);
        currentTile.isPartOfCorrectPath = true;

        // Giả sử bạn muốn một đường đi cố định
        // Ở đây bạn sẽ xây dựng đường đi theo quy tắc của bạn
        // Ví dụ: di chuyển sang phải, xuống dưới, ...
        for (var i = 0; i < 4; i++)
        {
            var nextX = currentTile.gridX + 1; // Đi sang phải
            if (nextX < gridWidth)
            {
                var nextTile = grid[nextX, currentTile.gridY];
                if (!nextTile.isObstacle)
                {
                    correctPath.Add(nextTile);
                    nextTile.isPartOfCorrectPath = true;
                    currentTile = nextTile; // Cập nhật ô hiện tại
                }
            }
        }

        for (var i = 0; i < 4; i++)
        {
            var nextY = currentTile.gridY + 1; // Đi xuống dưới
            if (nextY < gridHeight)
            {
                var nextTile = grid[currentTile.gridX, nextY];
                if (!nextTile.isObstacle)
                {
                    correctPath.Add(nextTile);
                    nextTile.isPartOfCorrectPath = true;
                    currentTile = nextTile;
                }
            }
        }
    }

    // Xử lý sự kiện chuột
    private void HandleMouseInput()
    {
        // Khi người chơi bắt đầu chạm vào màn hình
        if (Input.GetMouseButtonDown(0))
        {
            var mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            var hit = Physics2D.Raycast(mousePos, Vector2.zero);

            if (hit.collider != null)
            {
                var clickedTile = hit.collider.GetComponent<Tile>();

                // Nếu người chơi bắt đầu kéo từ ô đã nối (ô bắt đầu hoặc ô đã được nối)
                if (clickedTile != null && clickedTile.isConnected)
                {
                    isDragging = true;
                    lastConnectedTile = clickedTile;
                }
            }
        }

        // Khi người chơi kéo chuột và thả chuột
        if (isDragging)
        {
            var mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            var hit = Physics2D.Raycast(mousePos, Vector2.zero);

            if (hit.collider != null)
            {
                var hoveredTile = hit.collider.GetComponent<Tile>();

                // Nếu ô liền kề và nằm trên đường đi đúng, thực hiện kết nối
                if (hoveredTile != null && !hoveredTile.isConnected &&
                    lastConnectedTile.IsAdjacentToCorrectTile(hoveredTile))
                {
                    hoveredTile.ConnectTile();
                    lastConnectedTile = hoveredTile;

                    ClearPreviousHint(); // Xóa gợi ý cũ
                    ShowHintForNextTile(); // Hiển thị gợi ý mới

                    // Kiểm tra nếu tất cả các ô trên đường đi đúng đã được nối
                    if (CheckWinCondition()) Debug.Log("Level Complete!");
                }
            }

            // Khi người chơi thả chuột
            if (Input.GetMouseButtonUp(0)) isDragging = false;
        }
    }


    // Hiển thị gợi ý cho ô tiếp theo trên đường đi đúng
    private void ShowHintForNextTile()
    {
        foreach (var tile in correctPath)
            // Nếu ô nằm trên đường đi đúng, chưa được nối và liền kề với ô cuối cùng đã nối
            if (!tile.isConnected && lastConnectedTile.IsAdjacentToCorrectTile(tile))
            {
                hintTile = tile;
                hintTile.ShowHint();
                break; // Chỉ gợi ý một ô
            }
    }


    // Xóa gợi ý trước đó
    private void ClearPreviousHint()
    {
        if (hintTile != null) hintTile.ClearHint();
    }

    // Kiểm tra xem tất cả các ô trên đường đi đúng đã được nối chưa
    private bool CheckWinCondition()
    {
        // Duyệt qua tất cả các ô trong danh sách đường đi đúng
        foreach (var tile in correctPath)
            // Nếu có bất kỳ ô nào chưa được nối, điều kiện thắng chưa đạt
            if (!tile.isConnected)
                return false;
        // Chỉ khi tất cả các ô đã được nối, trả về true
        return true;
    }
}