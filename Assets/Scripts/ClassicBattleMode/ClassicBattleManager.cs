using ClassicBattleMode;
using UnityEngine;

public class ClassicBattleManager : MonoBehaviour
{
    public GameObject gridPanelPlayer1; // Lưới cho người chơi 1
    public GameObject gridPanelPlayer2; // Lưới cho người chơi 2
    public ClassicGameManager gameManager; // Quản lý trò chơi
    public ClassicPlayer player1; // Người chơi 1
    public ClassicPlayer player2; // Người chơi 2

    private void Start()
    {
        // Kết nối với GameManager
        gameManager = FindObjectOfType<ClassicGameManager>();

        // Khởi tạo Player 1 và Player 2
        player1 = new ClassicPlayer(gridPanelPlayer1);
        player2 = new ClassicPlayer(gridPanelPlayer2);

        // Kiểm tra nếu GridPanel đã được gán
        if (gridPanelPlayer1 == null || gridPanelPlayer2 == null)
        {
            Debug.LogError("GridPanel cho người chơi 1 hoặc người chơi 2 chưa được gán!");
            return;
        }

        InitializeBattle();
    }

    // Hàm khởi tạo trận đấu
    private void InitializeBattle()
    {
        Debug.Log("Trận đấu đã được khởi tạo giữa Người chơi 1 và Người chơi 2");
        // Ở đây bạn có thể bắt đầu các logic cần thiết cho mỗi người chơi
    }
}