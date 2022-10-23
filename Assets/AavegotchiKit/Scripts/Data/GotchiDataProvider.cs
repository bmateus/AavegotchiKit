using System.Linq;
using UnityEngine;

namespace PortalDefender.AavegotchiKit
{
    // Provides a singleton to access all Aavegotchi Data

    public partial class GotchiDataProvider : MonoBehaviour
    {
        static GotchiDataProvider instance_;
        public static GotchiDataProvider Instance => instance_;

        #region Base Parts

        public BasePartsDB basePartsDB;

        public Sprite GetBodySprite(Collateral collateral, GotchiFacing facing)
        {
            return basePartsDB.GetBodySprite(collateral, facing);
        }

        public Sprite GetHandsSprite(Collateral collateral, GotchiHandPose pose, GotchiFacing facing)
        {
            return basePartsDB.GetHandsSprite(collateral, pose, facing);
        }

        //mouths are only visible from the front
        public Sprite GetMouthSprite(Collateral collateral, GotchiMouthExpression expression = GotchiMouthExpression.HAPPY)
        {
            return basePartsDB.GetMouthSprite(collateral, expression);
        }

        public Sprite GetEyeSprite(int eyeShapeTrait, int eyeColorTrait, Collateral collateral, GotchiFacing facing)
        {
            if (facing == GotchiFacing.BACK)
                return null;

            var eyeColor = GotchiEyes.GetEyeColor(eyeColorTrait, collateral);

            Sprite eyeSprite = null;

            if (eyeShapeTrait < 0)
            {
                eyeSprite = eyeShapesDB.GetEyeShapeSprite(0, eyeColor, collateral, facing);
            }
            else if (eyeShapeTrait > 97)
            {
                eyeSprite = collateral.GetCollateralEyeSprite(eyeColor, facing);
            }
            else
            {
                int eyeShapeId = GotchiEyes.GetEyeShapeId(eyeShapeTrait);
                eyeSprite = eyeShapesDB.GetEyeShapeSprite(eyeShapeId, eyeColor, collateral, facing);
            }
            return eyeSprite;
        }

        public Sprite GetSpecialEyesSprite(int eyeShapeTrait, int eyeColorTrait, Collateral collateral, GotchiFacing facing, GotchiEyeExpression expression = GotchiEyeExpression.NONE)
        {
            if (expression == GotchiEyeExpression.NONE || facing != GotchiFacing.FRONT)
                return GetEyeSprite(eyeShapeTrait, eyeColorTrait, collateral, facing);

            return basePartsDB.GetSpecialEyes(collateral, expression);
        }

        public Sprite GetShadowSprite(GotchiFacing facing)
        {
            return basePartsDB.GetShadowSprite(facing);
        }

        public EyeShapesDB eyeShapesDB;

        #endregion

        #region Collaterals

        public CollateralDB collateralDB;

        public Collateral GetCollateral(string collateral)
        {
            return collateralDB.collaterals.Where(x => x.collateralType.Equals(collateral,
                System.StringComparison.CurrentCultureIgnoreCase)).FirstOrDefault();
        }

        #endregion

        #region Wearables

        public WearableDB wearableDB;

        public Wearable GetWearable(int wearableID)
        {
            return wearableDB.wearables.Where(x => x.id == wearableID).FirstOrDefault();
        }

        public WearableSetDB wearableSetDB;

        #endregion

        private void Awake()
        {
            instance_ = this;
        }
    }
}
