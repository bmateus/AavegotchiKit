using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace PortalDefender.AavegotchiKit.WearableBrowser
{
    public class WearableItemUI : MonoBehaviour
    {
        [SerializeField]
        Image bgImage_;

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

            if (wearable.HasSleeves)
            {
                bgImage_.color = Color.yellow;
            }
            
            label.text = $"{wearable.id}";
            button.onClick.AddListener(() => {
                onItemSelected?.Invoke(this);
            });
        }
    }
}