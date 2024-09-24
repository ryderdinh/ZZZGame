using System.Collections.Generic;
using UnityEngine;

namespace ClassicBattleMode
{
    public class ClassicPlayer
    {
        public GameObject gridPanel; // Lưới của người chơi
        public List<ClassicShip> ships; // Danh sách các tàu của người chơi

        // Constructor để khởi tạo người chơi với lưới
        public ClassicPlayer(GameObject gridPanel)
        {
            this.gridPanel = gridPanel;
            ships = new List<ClassicShip>();

            // Khởi tạo tàu cho người chơi (có thể tạo thủ công hoặc động)
            InitializeShips();
        }

        // Khởi tạo các tàu chiến
        private void InitializeShips()
        {
            // Ví dụ: Khởi tạo 2 tàu cho người chơi
            var ship1 = new ClassicShip("Tàu nhỏ", 1);
            var ship2 = new ClassicShip("Tàu trung bình", 3);

            ships.Add(ship1);
            ships.Add(ship2);

            Debug.Log("Tàu đã được khởi tạo cho người chơi");
        }

        // Hàm để kiểm tra nếu người chơi đã thua (tất cả tàu bị phá hủy)
        public bool HasLost()
        {
            foreach (var ship in ships)
                if (!ship.isDestroyed)
                    return false;
            return true;
        }
    }
}