using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace PortalDefender.AavegotchiKit.WearableBrowser
{
    public class TabUI : MonoBehaviour
    {
        [SerializeField]
        Button button;

        [SerializeField]
        Image bg;

        [SerializeField]
        TMP_Text label;

        [SerializeField]
        Color selectedColor;

        [SerializeField]
        Color unselectedColor;

        public int TabId { get; private set; }

        // Start is called before the first frame update
        void Awake()
        {
            bg.color = unselectedColor;
        }

        public void Init(int tabId, string labelText, Action<int> select)
        {
            this.TabId = tabId;
            label.text = labelText;
            button.onClick.AddListener(() => select?.Invoke(tabId));
        }

        public void SetSelected(bool selected)
        {
            bg.color = selected ? selectedColor : unselectedColor;
        }
    }
}