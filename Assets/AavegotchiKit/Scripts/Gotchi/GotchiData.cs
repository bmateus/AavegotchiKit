namespace PortalDefender.AavegotchiKit
{
    [System.Serializable]
    public partial class GotchiData 
    {
        public int id;
        public int hauntId;
        public string name;
        public string collateral;
        public int[] numericTraits;
        public int[] equippedWearables;
        public int level;
        public int kinship;
        public int status;

        public int GetTraitValue(GotchiTrait trait)
        {
            return  numericTraits[(int)trait];
        }
    }
}
