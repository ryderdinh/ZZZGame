using System.Collections.Generic;
using UnityEngine;

namespace ClassicBattleMode
{
    public class ClassicShip : MonoBehaviour
    {
        public string shipName;
        public int size;
        public bool isDestroyed;
        public List<Vector2Int> positions; // Vị trí trên lưới của tàu

        // Constructor để khởi tạo tàu
        public ClassicShip(string name, int size)
        {
            shipName = name;
            this.size = size;
            positions = new List<Vector2Int>();
        }

        // Gọi khi tàu bị bắn trúng
        public void Hit()
        {
            Debug.Log($"{shipName} bị bắn trúng!");
        }
    }
}