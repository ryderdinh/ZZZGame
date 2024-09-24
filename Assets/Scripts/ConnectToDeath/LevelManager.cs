using System.Collections.Generic;
using UnityEngine;

namespace ConnectToDeath
{
    public class LevelManager : MonoBehaviour
    {
        public GameObject emptyTilePrefab; // Prefab cho ô trống
        public GameObject obstacleTilePrefab; // Prefab cho chướng ngại vật
        public GameObject startTilePrefab; // Prefab cho điểm bắt đầu
        public GameObject endTilePrefab; // Prefab cho điểm kết thúc

        public int width = 10; // Độ rộng của grid
        public int height = 10; // Chiều cao của grid
        private readonly List<GameObject> obstacles = new(); // Lưu danh sách các chướng ngại vật
        private readonly List<GameObject> validPath = new(); // Lưu trữ các ô hợp lệ trên đường đi
        private int emptyTileCount; // Số ô trống hợp lệ cần đi qua
        private GameObject endTile; // Ô kết thúc

        private int[,] grid; // Mảng 2D lưu trữ map

        private GameObject startTile; // Ô bắt đầu

        private void Start()
        {
            GenerateBaseMap(); // Tạo map cơ bản không có chướng ngại vật
            GenerateObstacles(); // Thêm chướng ngại vật và đảm bảo có đường hợp lệ
            FindAndNumberPath(); // Tìm và đánh số đường đi
        }

        private void GenerateBaseMap()
        {
            grid = new int[width, height];
            emptyTileCount = 0; // Đếm số lượng ô trống hợp lệ

            // Tạo lưới ban đầu với ô trống, điểm bắt đầu và điểm kết thúc
            for (var x = 0; x < width; x++)
            for (var y = 0; y < height; y++)
            {
                var tile = Instantiate(emptyTilePrefab, new Vector3(x, y, 0), Quaternion.identity);
                grid[x, y] = 0; // 0 đại diện cho ô trống
                emptyTileCount++; // Tăng số lượng ô trống

                tile.transform.parent = transform; // Gán tile vào Grid làm con
                tile.name = $"Tile ({x}, {y})";
            }

            // Chọn điểm bắt đầu và điểm kết thúc ngẫu nhiên
            var startPos = GetRandomEmptyPosition();
            Vector2Int endPos;

            // Đảm bảo điểm kết thúc khác với điểm bắt đầu
            do
            {
                endPos = GetRandomEmptyPosition();
            } while (endPos == startPos);

            // Đặt điểm bắt đầu và điểm kết thúc
            startTile = Instantiate(startTilePrefab, new Vector3(startPos.x, startPos.y, 0), Quaternion.identity);
            endTile = Instantiate(endTilePrefab, new Vector3(endPos.x, endPos.y, 0), Quaternion.identity);

            grid[startPos.x, startPos.y] = 2; // 2 là điểm bắt đầu
            grid[endPos.x, endPos.y] = 3; // 3 là điểm kết thúc

            // Gán Start và End làm con của đối tượng Grid
            startTile.transform.parent = transform;
            endTile.transform.parent = transform;

            emptyTileCount -= 2; // Trừ 2 ô đặc biệt là điểm bắt đầu và điểm kết thúc
        }


        // Hàm trả về vị trí trống ngẫu nhiên trong grid
        private Vector2Int GetRandomEmptyPosition()
        {
            Vector2Int randomPos;
            do
            {
                randomPos = new Vector2Int(Random.Range(0, width), Random.Range(0, height));
            } while (grid[randomPos.x, randomPos.y] != 0); // Tìm đến khi gặp ô trống

            return randomPos;
        }

        // Phương thức đặt chướng ngại vật ngẫu nhiên cho đến khi có đường hợp lệ
        private void GenerateObstacles()
        {
            var foundValidPath = false;

            while (!foundValidPath)
            {
                // Xóa tất cả các chướng ngại vật cũ trước khi tạo lại
                ClearObstacles();

                // Tạo các chướng ngại vật ngẫu nhiên
                var obstacleCount = Random.Range(1, width * height / 4); // Số lượng chướng ngại vật ngẫu nhiên

                for (var i = 0; i < obstacleCount; i++)
                {
                    var randomPos = new Vector2Int(Random.Range(0, width), Random.Range(0, height));

                    // Kiểm tra nếu vị trí này là ô trống thì đặt chướng ngại vật
                    if (grid[randomPos.x, randomPos.y] == 0)
                    {
                        grid[randomPos.x, randomPos.y] = 1; // 1 là chướng ngại vật
                        var obstacle = Instantiate(obstacleTilePrefab, new Vector3(randomPos.x, randomPos.y, 0),
                            Quaternion.identity);
                        obstacle.transform.parent = transform;

                        // Lưu chướng ngại vật vào danh sách để dễ xóa sau này
                        obstacles.Add(obstacle);
                    }
                }

                // Kiểm tra xem có đường hợp lệ không
                if (FindPathWithBFS())
                    foundValidPath = true; // Nếu tìm thấy đường hợp lệ, dừng quá trình tạo chướng ngại vật
            }
        }

        // Phương thức xóa tất cả chướng ngại vật cũ
        private void ClearObstacles()
        {
            foreach (var obstacle in obstacles) Destroy(obstacle); // Xóa các đối tượng chướng ngại vật trong danh sách
            obstacles.Clear(); // Xóa danh sách chướng ngại vật

            // Xóa thông tin chướng ngại vật trong lưới
            for (var x = 0; x < width; x++)
            for (var y = 0; y < height; y++)
                if (grid[x, y] == 1) // 1 đại diện cho chướng ngại vật
                    grid[x, y] = 0; // Đặt lại thành ô trống
        }

        // Phương thức tìm và đánh số đường đi hợp lệ
        private void FindAndNumberPath()
        {
            if (FindPathWithBFS())
            {
                Debug.Log("Tìm thấy đường đi hợp lệ!");
                NumberPath(); // Đánh số thứ tự đường đi
            }
        }

        private bool FindPathWithBFS()
        {
            validPath.Clear(); // Xóa đường đi trước đó
            var startPos = new Vector2Int((int)startTile.transform.position.x, (int)startTile.transform.position.y);
            var endPos = new Vector2Int((int)endTile.transform.position.x, (int)endTile.transform.position.y);

            var queue = new Queue<Vector2Int>(); // Hàng đợi cho BFS
            var visited = new HashSet<Vector2Int>(); // Lưu các ô đã thăm
            var cameFrom = new Dictionary<Vector2Int, Vector2Int>(); // Lưu dấu đường đi

            var visitedEmptyCount = 0; // Số lượng ô trống đã thăm

            queue.Enqueue(startPos);
            visited.Add(startPos);

            while (queue.Count > 0)
            {
                var currentPos = queue.Dequeue();

                // Nếu đã đến điểm kết thúc và đi qua tất cả các ô trống hợp lệ
                if (currentPos == endPos && visitedEmptyCount == emptyTileCount)
                {
                    ReconstructPath(cameFrom, currentPos); // Tái tạo lại đường đi từ điểm kết thúc về điểm bắt đầu
                    return true;
                }

                // Kiểm tra tất cả 4 hướng (lên, xuống, trái, phải)
                Vector2Int[] directions = { Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right };
                foreach (var direction in directions)
                {
                    var nextPos = currentPos + direction;

                    // Kiểm tra nếu vị trí hợp lệ và chưa thăm
                    if (nextPos.x >= 0 && nextPos.x < width && nextPos.y >= 0 && nextPos.y < height &&
                        grid[nextPos.x, nextPos.y] != 1 && !visited.Contains(nextPos)) // 1 là chướng ngại vật
                    {
                        queue.Enqueue(nextPos);
                        visited.Add(nextPos);
                        cameFrom[nextPos] = currentPos; // Ghi nhớ từ đâu đến vị trí này

                        // Nếu là ô trống, tăng số lượng ô đã đi qua
                        if (grid[nextPos.x, nextPos.y] == 0) visitedEmptyCount++;
                    }
                }
            }

            return false; // Không tìm thấy đường hợp lệ
        }

        // Hàm tái tạo lại đường đi từ điểm kết thúc về điểm bắt đầu
        private void ReconstructPath(Dictionary<Vector2Int, Vector2Int> cameFrom, Vector2Int currentPos)
        {
            while (cameFrom.ContainsKey(currentPos))
            {
                validPath.Add(GameObject.Find($"Tile ({currentPos.x}, {currentPos.y})"));
                currentPos = cameFrom[currentPos]; // Quay lại vị trí trước đó
            }

            validPath.Add(GameObject.Find($"Tile ({currentPos.x}, {currentPos.y})")); // Thêm vị trí bắt đầu
        }


        // Đánh số thứ tự cho các ô trong đường đi
        private void NumberPath()
        {
            // Đường đi hợp lệ được lưu trong danh sách validPath
            validPath.Reverse(); // Vì DFS thêm từ cuối đến đầu, cần đảo ngược lại để có thứ tự đúng

            for (var i = 0; i < validPath.Count; i++)
            {
                var tile = validPath[i];
                // Đặt số thứ tự trên các ô
                tile.name += $" - Step {i + 1}";
                Debug.Log($"Step {i + 1}: {tile.name}");
            }
        }
    }
}