using System;
using UnityEngine;
using Sirenix.OdinInspector;
using Aavegotchi.AavegotchiDiamond.ContractDefinition;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace PortalDefender.AavegotchiKit
{
    [Serializable]
    public class BaseParts
    {
        public string[] body;
        public string[] hands;
        public string[] mouth_neutral;
        public string[] mouth_happy;
        public string[] eyes_mad;
        public string[] eyes_happy;
        public string[] eyes_sleepy;
        public string[] shadow;

        public string portalOpenH1;
        public string portalClosedH1;

        public string portalOpenH2;
        public string portalClosedH2;

        [Button]
        private async void RefreshData()
        {
            BasePartsFetcher fetcher = new BasePartsFetcher();

            portalOpenH1 = await fetcher.GetPortalOpen(1);
            portalClosedH1 = await fetcher.GetPortalClosed(1);

            portalOpenH2 = await fetcher.GetPortalOpen(2);
            portalClosedH2 = await fetcher.GetPortalClosed(2);

            var hands = await fetcher.GetBaseBody();
            foreach (var hand in hands)
            {
                Debug.Log("hand: " + hand);
            }               

        }

    }
}
