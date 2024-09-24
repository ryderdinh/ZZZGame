using UnityEngine;
using UnityEngine.UI;

namespace ClassicBattleMode
{
    public class ClassicCell : MonoBehaviour
    {
        public int cellIndex; // Chỉ số của ô trong lưới
        public Button cellButton; // Nút của ô để xử lý sự kiện bấm

        private ClassicGameManager gameManager;

        private void Start()
        {
            // Gán GameManager để gọi khi người chơi bấm vào ô
            gameManager = FindObjectOfType<ClassicGameManager>();

            // Nếu có Button, thêm sự kiện bấm
            if (cellButton != null) cellButton.onClick.AddListener(OnCellClick);
        }

        // Hàm xử lý khi ô bị bấm
        private void OnCellClick()
        {
            Debug.Log("Ô đã bị bấm: " + cellIndex);
            gameManager.PlayerShoot(cellIndex); // Gọi GameManager để xử lý bắn
        }
    }
}