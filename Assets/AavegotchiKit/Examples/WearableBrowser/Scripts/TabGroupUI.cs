using System;
using System.Collections.Generic;
using UnityEngine;

namespace PortalDefender.AavegotchiKit.WearableBrowser
{
    public class TabGroupUI : MonoBehaviour
    {
        public event Action<TabUI, TabUI> OnTabChanged;

        int selectedTabId = -1;

        [SerializeField]
        TabUI tabPrefab;

        List<TabUI> tabs = new List<TabUI>();

        internal void AddTab(int slot, string labelText)
        {
            var tab = Instantiate(tabPrefab, transform);
            tab.Init(slot, labelText, Select);
            tabs.Add(tab);
        }

        internal void Select(int tabId)
        {
            if (tabId == selectedTabId)
                return;

            TabUI lastTab = null;

            if (selectedTabId >= 0 && selectedTabId < tabs.Count)
            {
                lastTab = tabs[selectedTabId];
                lastTab.SetSelected(false);
            }

            TabUI currentTab = null;

            if (tabId >= 0 && tabId < tabs.Count)
            {
                currentTab = tabs[tabId];
                currentTab.SetSelected(true);
            }

            selectedTabId = tabId;

            OnTabChanged?.Invoke(lastTab, currentTab);
        }
    }
}