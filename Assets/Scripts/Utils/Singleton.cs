using UnityEngine;

namespace Utils
{
    public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
    {
        private static T instance;
        private static bool _isQuit;

        public static T Instance
        {
            get
            {
                if (_isQuit) return null;
                if (instance == null)
                {
                    instance = FindObjectOfType(typeof(T)) as T;

                    if (instance == null)
                    {
                        instance = new GameObject().AddComponent<T>();
                        instance.gameObject.name = instance.GetType().Name;
                    }
                }

                return instance;
            }
        }

        public void Reset()
        {
            instance = null;
        }

        public void OnDestroy()
        {
            _isQuit = true;
        }

        public static bool Exists()
        {
            return instance != null;
        }
    }
}