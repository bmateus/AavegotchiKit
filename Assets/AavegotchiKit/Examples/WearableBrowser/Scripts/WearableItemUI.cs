using System;
using TMPro;
using Unity.VectorGraphics;
using UnityEngine;
using UnityEngine.UI;

namespace PortalDefender.AavegotchiKit.WearableBrowser
{
    public class WearableItemUI : MonoBehaviour
    {
        [SerializeField]
        WearableItemIconUI icon;

        [SerializeField]
        Button button;

        [SerializeField]
        TMP_Text label;
        
        public Wearable Data { get; private set; }

        internal void Init(Wearable wearable, Action<WearableItemUI> onItemSelected)
        {
            Data = wearable;

            icon.Init(wearable);
            
            label.text = $"{wearable.id}";
            button.onClick.AddListener(() => {
                onItemSelected?.Invoke(this);
            });
        }
    }
}