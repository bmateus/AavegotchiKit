namespace PortalDefender.AavegotchiKit
{
    [System.Serializable]
    public class GotchiData 
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

        public enum Trait: int
        {
            Energy = 0,
            Aggressiveness,
            Spookiness,
            Brain,
            EyeShape,
            EyeColor
        }

        public int GetTraitValue(Trait trait)
        {
            return  numericTraits[(int)trait];
        }

        public enum Slot : int
        {
            BODY = 0,
            FACE,
            EYES,
            HEAD,
            HAND_LEFT,
            HAND_RIGHT,
            PET,
            BG
        }


    }
}
