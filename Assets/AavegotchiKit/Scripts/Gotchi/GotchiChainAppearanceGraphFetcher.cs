using PortalDefender.AavegotchiKit.GraphQL;
using UnityEngine;

namespace PortalDefender.AavegotchiKit
{
    public class GotchiChainAppearanceGraphFetcher : MonoBehaviour
    {
        [SerializeField]
        int gotchiId_;

        void Start()
        {
            Refresh();            
        }

        public async void Refresh()
        {
            if (!GraphManager.IsInitialized)
            {
                Debug.LogError("Can't Use Graph! Requires a GraphManager in the scene.");
                return;
            }

            var gotchiData = await GraphManager.Instance.GetGotchiData(gotchiId_.ToString());

            if (gotchiData != null
                && gotchiData.status == 3
                && gotchiData.name != "Default")
            {
                var gotchi = GetComponent<Gotchi>();
                gotchi.Init(gotchiData);
                //gotchi.State.Facing = GotchiFacing.FRONT;
            }
        }
    }

}
