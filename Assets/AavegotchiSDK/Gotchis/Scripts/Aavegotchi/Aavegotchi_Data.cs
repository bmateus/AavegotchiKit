namespace GotchiSDK
{
    [System.Serializable]
    public class Aavegotchi_Data
    {
        public int HauntID;
        public ECollateral CollateralType;
        public EEyeShape EyeShape;
        public EEyeColor EyeColor;

        public int Body_WearableID = 0;
        public int Face_WearableID = 0;
        public int Eyes_WearableID = 0;
        public int Head_WearableID = 0;
        public int Pet_WearableID = 0;
        public int HandLeft_WearableID = 0;
        public int HandRight_WearableID = 0;

        //--------------------------------------------------------------------------------------------------
        // Copy constructor
        //--------------------------------------------------------------------------------------------------
        public Aavegotchi_Data(Aavegotchi_Data other)
        {
            HauntID = other.HauntID;
            CollateralType = other.CollateralType;
            EyeShape = other.EyeShape;
            EyeColor = other.EyeColor;
            Body_WearableID = other.Body_WearableID;
            Face_WearableID = other.Face_WearableID;
            Eyes_WearableID = other.Eyes_WearableID;
            Head_WearableID = other.Head_WearableID;
            Pet_WearableID = other.Pet_WearableID;
            HandLeft_WearableID = other.HandLeft_WearableID;
            HandRight_WearableID = other.HandRight_WearableID;
        }

        public Aavegotchi_Data()
        {

        }
    }
}