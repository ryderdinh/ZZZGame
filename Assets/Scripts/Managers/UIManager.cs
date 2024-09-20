using ConnectToDeath;
using UnityEngine;
using Utils;

namespace Managers
{
    public class UIManager : Singleton<UIManager>
    {
        [SerializeField] private Transform popupParent;

        [HideInInspector] public PopupCTDLevel popupCtdLevel { get; private set; }

        private void Awake()
        {
            LoadUIResource();
        }

        private void LoadUIResource()
        {
            popupCtdLevel = Instantiate(Resources.Load<PopupCTDLevel>("Popups/CTDLevel"));
        }
    }
}