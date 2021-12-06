using System.Linq;
using UnityEngine;

namespace com.mycompany
{
    public class AavegotchiData : MonoBehaviour
    {
        static AavegotchiData instance_;
        public static AavegotchiData Instance => instance_;

        public enum Facing : int
        {
            FRONT,  //AKA SOUTH
            LEFT,   //AKA WEST
            RIGHT,  //AKA EAST
            BACK    //AKA NORTH
        }

        [SerializeField]
        Sprite[] body;

        public Sprite GetBodySprite(Facing facing)
        {
            return body[(int)facing];
        }

        [SerializeField]
        Sprite[] handsUp;
        [SerializeField]
        Sprite[] handsDownClosed;
        [SerializeField]
        Sprite[] handsDownOpen;

        public enum HandPose : int
        {
            DOWN_CLOSED,
            DOWN_OPEN,
            UP
        }

        public Sprite GetHandsSprite(HandPose pose, Facing facing)
        {
            switch (pose)
            {
                case HandPose.DOWN_CLOSED: return handsDownClosed[(int)facing];
                case HandPose.DOWN_OPEN: return handsDownOpen[(int)facing];
                case HandPose.UP: return handsUp[(int)facing];
                default: return null;
            }
        }


        public enum MouthExpression : int
        {
            HAPPY,
            NEUTRAL
        }

        [SerializeField]
        Sprite[] mouths;

        //mouths are only visible from the front
        public Sprite GetMouthSprite(MouthExpression expression = MouthExpression.HAPPY)
        {
            return mouths[(int)expression];
        }


        public static Color MythicalLow     = new Color32(0xFF, 0x00, 0xFF, 0xFF);
        public static Color RareLow         = new Color32(0x00, 0x64, 0xFF, 0xFF);
        public static Color UncommonLow     = new Color32(0x5D, 0x24, 0xBF, 0xFF);
        public static Color UncommonHigh    = new Color32(0x36, 0x81, 0x8E, 0xFF);
        public static Color RareHigh        = new Color32(0xEA, 0x8C, 0x27, 0xFF);
        public static Color MythicalHigh    = new Color32(0x51, 0xFF, 0xA8, 0xFF);

        public Color GetEyeColor(int eyeColorTrait, Collateral collateral)
        {
            if (eyeColorTrait < 2) return MythicalLow;
            if (eyeColorTrait < 10) return RareLow;
            if (eyeColorTrait < 25) return UncommonLow;
            if (eyeColorTrait < 75) return collateral.PrimaryColor;
            if (eyeColorTrait < 90) return UncommonHigh;
            if (eyeColorTrait < 98) return RareHigh;
            return MythicalHigh;
        }

        int[] eyeShapeTraitTraitRange = { 0, 1, 2, 5, 7, 10, 15, 20, 25, 42, 58, 75, 80, 85, 90, 93, 95, 98 };

        public Sprite GetEyeSprite(int eyeShapeTrait, Collateral collateral, Facing facing)
        {
            if (facing == Facing.BACK)
                return null;

            Sprite eyeSprite = null;

            if (eyeShapeTrait < 0)
            {
                eyeSprite = eyeShapesDB.eyeShapes[0].sprites[(int)facing];
            }
            else if (eyeShapeTrait > 97)
            {
                eyeSprite = collateral.eyeSprites[(int)facing];
            }
            else
            {
                for (int i=0; i < eyeShapeTraitTraitRange.Length - 1; i++)
                {
                    if (eyeShapeTrait >= eyeShapeTraitTraitRange[i] 
                        && eyeShapeTrait < eyeShapeTraitTraitRange[i + 1])
                    {
                        eyeSprite = eyeShapesDB.eyeShapes[i].sprites[(int)facing];
                        break;
                    }
                }
            }
            return eyeSprite;
        }

        public enum EyeExpression : int
        {
            NONE,
            HAPPY,
            MAD,
            SLEEPING
        }

        [SerializeField]
        Sprite[] specialEyes;

        public Sprite GetSpecialEyesSprite(int eyeShapeTrait, Collateral collateral, Facing facing, EyeExpression expression = EyeExpression.NONE)
        {
            if (expression == EyeExpression.NONE || facing != Facing.FRONT)
                return GetEyeSprite(eyeShapeTrait, collateral, facing);
            return specialEyes[(int)expression];
        }

        [SerializeField]
        Sprite[] shadows;

        public Sprite GetShadowSprite(Facing facing)
        {
            switch (facing)
            {
                case Facing.FRONT:
                case Facing.BACK:
                default:
                    return shadows[0];
                case Facing.LEFT:
                case Facing.RIGHT:
                    return shadows[1];
            }
        }


        public WearableDB wearableDB;

        public Wearable GetWearable(int wearableID)
        {
            return wearableDB.wearables.Where(x => x.id == wearableID).FirstOrDefault();
        }


        public CollateralDB collateralDB;

        public Collateral GetCollateral(string collateral)
        {
            return collateralDB.collaterals.Where(x => x.collateralType.Equals(collateral, 
                System.StringComparison.CurrentCultureIgnoreCase )).FirstOrDefault();
        }


        public EyeShapesDB eyeShapesDB;


        public WearableSetDB wearableSetDB;


        private void Awake()
        {
            instance_ = this;
        }
    }
}
