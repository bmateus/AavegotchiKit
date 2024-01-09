using UnityEngine;

namespace PortalDefender.AavegotchiKit
{
    /// <summary>
    /// This class represents a gotchi in the game; 
    /// It provides access to the GotchiData, GotchiState and GotchiAppearance
    /// </summary>
    ///
    /// <remarks>
    /// Init needs to be called to initialize the appearance data with the data and state
    /// the data may be optional depending on the implementation of the appearance
    /// </remarks>
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

        /// <summary>
        /// Equips the selected wearable to the specified slot and re-initializes the appearance
        /// </summary>
        /// <param name="selectedWearable"></param>
        /// <param name="slot"></param>
        public void Equip(Wearable selectedWearable, GotchiEquipmentSlot slot)
        {
            data.equippedWearables[(int)slot] = (ushort)selectedWearable.id;
            appearance.Init(this);
        }
    }
}
