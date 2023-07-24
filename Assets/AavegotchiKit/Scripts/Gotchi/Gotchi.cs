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


        GotchiState state;

        public GotchiState State => state;
        

        IGotchiAppearance appearance;

        public IGotchiAppearance Appearance => appearance;


        private void Awake()
        {
            state = new GotchiState();
            appearance = GetComponentInChildren<IGotchiAppearance>();
        }

        public void Init(GotchiData data)
        {
            this.data = data;
            appearance.Init(this);
        }

        public void Equip(Wearable selectedWearable, GotchiEquipmentSlot slot)
        {
            data.equippedWearables[(int)slot] = (ushort)selectedWearable.id;
            appearance.Init(this);
        }
    }
}
