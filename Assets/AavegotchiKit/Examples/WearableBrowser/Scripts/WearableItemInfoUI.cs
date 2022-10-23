using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VectorGraphics;
using UnityEngine;
using UnityEngine.UI;

namespace PortalDefender.AavegotchiKit.WearableBrowser
{
    public class WearableItemInfoUI : MonoBehaviour
    {
        [SerializeField]
        WearableItemIconUI icon;

        [SerializeField]
        TMP_Text id;

        [SerializeField]
        TMP_Text name;

        [SerializeField]
        Button actionButton;

        Wearable selectedWearable;

        public Button ActionButton => actionButton;

        private void Start()
        {
            actionButton.onClick.AddListener(EquipWearable);
        }

        public void UpdateInfo(Wearable wearableData)
        {
            selectedWearable = wearableData;
            icon.Init(wearableData);
            id.text = $"{wearableData.id}";
            name.text = wearableData.name;

            //is the gotchi wearning this?


        }

        void EquipWearable()
        {
            var gotchiBrowser = FindObjectOfType<GotchiPreviewUI>();
            var wearableBrowser = FindObjectOfType<WearableBrowserUI>();
            if (gotchiBrowser && wearableBrowser)
            {
                gotchiBrowser.Gotchi.Equip(selectedWearable, wearableBrowser.CurrentSlot);
            }
        }

    }
}