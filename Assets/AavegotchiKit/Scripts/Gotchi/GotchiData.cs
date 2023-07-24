namespace PortalDefender.AavegotchiKit
{
    [System.Serializable]
    public class GotchiData 
    {
        public int id;
        public int hauntId;
        public string name;
        public string collateral;
        public short[] numericTraits;
        public ushort[] equippedWearables;
        public int level;
        public int kinship;
        public int status; // 0 == portal, 1 == VRF_PENDING, 2 == open portal, 3 == Aavegotchi
        public string lastInteracted;

        public GotchiData()
        {
            id = 0;
            hauntId = 1;
            name = "Default";
            collateral = "0xE0b22E0037B130A9F56bBb537684E6fA18192341";
            numericTraits = new short[6];
            equippedWearables = new ushort[8];
            level = 1;
            kinship = 0;
            status = 0;
        }

        public int GetTraitValue(GotchiTrait trait)
        {
            return  numericTraits[(int)trait];
        }
    }
}
