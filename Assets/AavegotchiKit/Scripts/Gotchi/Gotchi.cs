using UnityEngine;

namespace PortalDefender.AavegotchiKit
{
    // This class represents a gotchi in the game;
    // It is the main interface for the game to interact with the gotchi
    // A gotchi needs to be initialized with a GotchiData object before it can be used
    public class Gotchi : MonoBehaviour
    {
        GotchiData data;
        public GotchiData Data => data;

        Collateral collateral;

        public Collateral Collateral => collateral;

        GotchiState state;

        public GotchiState State => state;
        
        IGotchiAppearance appearance;

        IGotchiAppearance Appearance => appearance;


        private void Awake()
        {
            state = new GotchiState();
            appearance = GetComponentInChildren<IGotchiAppearance>();
        }

        public void Init(GotchiData data)
        {
            this.data = data;

            //resolve collateral
            collateral = GotchiDataProvider.Instance.GetCollateral(data.collateral);

            //InitHandPose();

            appearance.Init(this);
        }

        void InitHandPose()
        {
            //should it have open hands or close hands?
            state.HandPose = GotchiHandPose.DOWN_CLOSED;
            if (data.equippedWearables[(int)GotchiEquipmentSlot.BODY] != 0
                || data.equippedWearables[(int)GotchiEquipmentSlot.HAND_LEFT] != 0
                || data.equippedWearables[(int)GotchiEquipmentSlot.HAND_RIGHT] != 0)
            {
                state.HandPose = GotchiHandPose.DOWN_OPEN;
            }
        }

        public void Equip(Wearable selectedWearable, GotchiEquipmentSlot slot)
        {
            data.equippedWearables[(int)slot] = selectedWearable.id;
            appearance.Init(this);
        }
    }
}
