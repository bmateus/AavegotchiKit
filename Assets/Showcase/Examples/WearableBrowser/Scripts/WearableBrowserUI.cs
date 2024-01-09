using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace PortalDefender.AavegotchiKit.WearableBrowser
{
    public class WearableBrowserUI : MonoBehaviour
    {
        public GotchiEquipmentSlot CurrentSlot { get; private set; }

        [SerializeField]
        WearableItemUI itemPrefab;

        [SerializeField]
        Transform contentRoot;

        List<GameObject> items = new List<GameObject>();

        [SerializeField]
        TabGroupUI tabGroup;

        [SerializeField]
        WearableItemInfoUI itemInfo;

        private void Start()
        {
            InitTabGroup();
            Refresh();
        }

        void InitTabGroup()
        {
            foreach(var slot in Enum.GetValues(typeof(GotchiEquipmentSlot)) )
            {
                tabGroup.AddTab((int)slot, Enum.GetName(typeof(GotchiEquipmentSlot), slot));
            }
            tabGroup.Select(0);
            tabGroup.OnTabChanged += OnTabChanged;
        }

        void Clear()
        {
            foreach(var item in items)
            {
                Destroy(item);
            }
            items.Clear();
        }

        void Refresh()
        {
            Clear();

            var wearables = GotchiDataProvider.Instance.wearableDB.wearables.Where(x => x.data.slotPositions[(int)CurrentSlot]);
            foreach(var wearable in wearables)
            {
                var item = Instantiate(itemPrefab, contentRoot);
                item.Init(wearable.data, OnItemSelected);
                items.Add(item.gameObject);
            }
        }

        void OnItemSelected(WearableItemUI item)
        {
            Debug.Log("Item Selected");
            itemInfo.UpdateInfo(item.Data);

        }


        void OnTabChanged(TabUI previous, TabUI selected)
        {
            CurrentSlot = (GotchiEquipmentSlot)selected.TabId;
            Refresh();
        }




    }
}