using UnityEngine;
using UnityEngine.UI;

namespace ClassicBattleMode
{
    public class ClassicGameManager : MonoBehaviour
    {
        public bool isPlayer1Turn = true; // Lượt của người chơi 1
        public Text turnText; // Text để hiển thị lượt chơi
        public ClassicCell[] cells; // Mảng chứa tất cả các ô trong lưới

        private void Start()
        {
            // Lấy tất cả các CellScript của các ô trong lưới
            cells = FindObjectsOfType<ClassicCell>();
            UpdateTurnText();
        }

        // Hàm gọi khi một ô bị bắn
        public void PlayerShoot(int cellIndex)
        {
            if (isPlayer1Turn)
                Debug.Log("Người chơi 1 bắn vào ô: " + cellIndex);
            // Thực hiện logic khi Người chơi 1 bắn vào ô này
            else
                Debug.Log("Người chơi 2 bắn vào ô: " + cellIndex);
            // Thực hiện logic khi Người chơi 2 bắn vào ô này
            // Chuyển lượt chơi
            isPlayer1Turn = !isPlayer1Turn;
            UpdateTurnText();
        }

        // Cập nhật văn bản lượt chơi
        private void UpdateTurnText()
        {
            if (isPlayer1Turn)
                turnText.text = "Lượt của Người chơi 1";
            else
                turnText.text = "Lượt của Người chơi 2";
        }
    }
}