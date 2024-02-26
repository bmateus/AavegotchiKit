using GotchiSDK;
using UnityEngine;

namespace PortalDefender.AavegotchiKit
{
    //Serves as an adapter for the official AavegotchiSDK
    public class GotchiSDKAppearance : MonoBehaviour, IGotchiAppearance
    {
        Aavegotchi_Base aavegotchiBase;
        Aavegotchi_Data aavegotchiData = new Aavegotchi_Data();

        private int[] eyeShapeTraitTraitRange = { 2, 5, 7, 10, 15, 20, 25, 42, 58, 75, 80, 85, 90, 93, 95, 98 };
        private EEyeShape[] eyeShapes =
        {
            EEyeShape.RareLow1,
            EEyeShape.RareLow2,
            EEyeShape.RareLow3,

            EEyeShape.UncommonLow1,
            EEyeShape.UncommonLow2,
            EEyeShape.UncommonLow3,

            EEyeShape.Common1,
            EEyeShape.Common2,
            EEyeShape.Common3,

            EEyeShape.UncommonHigh1,
            EEyeShape.UncommonHigh2,
            EEyeShape.UncommonHigh3,

            EEyeShape.RareHigh1,
            EEyeShape.RareHigh2,
            EEyeShape.RareHigh3
        };

        EEyeShape eyeShapeFromTraitValue(int eyeShapeTrait, EEyeShape collateralEyeShape, int haunt)
        {
            if (eyeShapeTrait <= 0)
            {
                return haunt == 1 ? EEyeShape.MythicalLow1_H1 : EEyeShape.MythicalLow1_H2;
            }
            else if (eyeShapeTrait == 1)
            {
                return haunt == 1 ? EEyeShape.MythicalLow2_H1 : EEyeShape.MythicalLow2_H2;
            }
            else if (eyeShapeTrait > 97)
            {
                return collateralEyeShape;
            }
            else
            {
                for (int i = 0; i < eyeShapeTraitTraitRange.Length - 1; i++)
                {
                    if (eyeShapeTrait >= eyeShapeTraitTraitRange[i]
                        && eyeShapeTrait < eyeShapeTraitTraitRange[i + 1])
                    {
                        return eyeShapes[i];
                    }
                }
            }

            return EEyeShape.ETH;
        } 

        public EEyeColor getEyeColor(int eyeColorTrait)
        {
            if (eyeColorTrait < 2) return EEyeColor.Mythical_Low;
            if (eyeColorTrait < 10) return EEyeColor.Rare_Low;
            if (eyeColorTrait < 25) return EEyeColor.Uncommon_Low;
            //if (eyeColorTrait < 75) return EEyeColor.None; //baked into the eye shape
            if (eyeColorTrait < 90) return EEyeColor.Uncommon_High;
            if (eyeColorTrait < 98) return EEyeColor.Rare_High;
            return EEyeColor.Mythical_High;
        }

        void Awake()
        {
            aavegotchiBase = GetComponent<Aavegotchi_Base>();
            if (aavegotchiBase == null)
            {
                Debug.LogError("Aavegotchi_Base not found on " + gameObject.name);
            }
        }

        public void Init(Gotchi gotchi)
        {
            aavegotchiData.HauntID = gotchi.Data.hauntId;

            var collateralData = GotchiDataProvider.Instance.GetCollateral(gotchi.Data.collateral);
            switch (collateralData.name)
            {
                case "maDAI":
                case "amDAI":
                    aavegotchiData.CollateralType = ECollateral.Dai;
                    aavegotchiData.EyeShape = EEyeShape.DAI;
                    break;

                case "maWETH":
                case "amWETH":
                    aavegotchiData.CollateralType = ECollateral.wEth;
                    aavegotchiData.EyeShape = EEyeShape.wETH;
                    break;

                case "maAAVE":
                case "amAAVE":
                    aavegotchiData.CollateralType = ECollateral.Aave;
                    aavegotchiData.EyeShape = EEyeShape.AAVE;
                    break;

                case "maLINK":
                case "amLINK":
                    aavegotchiData.CollateralType = ECollateral.Link;
                    aavegotchiData.EyeShape = EEyeShape.LINK;
                    break;

                case "maUSDT":
                case "amUSDT":
                    aavegotchiData.CollateralType = ECollateral.USDT;
                    aavegotchiData.EyeShape = EEyeShape.USDT;
                    break;

                case "maUSDC":
                case "amUSDC":
                    aavegotchiData.CollateralType = ECollateral.USDC;
                    aavegotchiData.EyeShape = EEyeShape.USDC;
                    break;

                case "maTUSD":
                case "amTUSD":
                    aavegotchiData.CollateralType = ECollateral.TUSD;
                    aavegotchiData.EyeShape = EEyeShape.TUSD;
                    break;

                case "maUNI":
                case "amUNI":
                    aavegotchiData.CollateralType = ECollateral.Uni;
                    aavegotchiData.EyeShape = EEyeShape.UNI;
                    break;

                case "maYFI":
                case "amYFI":
                    aavegotchiData.CollateralType = ECollateral.Yfi;
                    aavegotchiData.EyeShape = EEyeShape.YFI;
                    break;

                case "amWBTC":
                    aavegotchiData.CollateralType = ECollateral.wBTC;
                    aavegotchiData.EyeShape = EEyeShape.wBTC;
                    break;

                case "maPolygon":
                    aavegotchiData.CollateralType = ECollateral.Polygon;
                    aavegotchiData.EyeShape = EEyeShape.POLYGON;
                    break;
            }

            aavegotchiData.EyeColor = getEyeColor(gotchi.Data.GetTraitValue(GotchiTrait.EyeColor));
            aavegotchiData.EyeShape = eyeShapeFromTraitValue(gotchi.Data.GetTraitValue(GotchiTrait.EyeShape), aavegotchiData.EyeShape, aavegotchiData.HauntID);

            for (int i = 0; i < gotchi.Data.equippedWearables.Length; i++)
            {
                GotchiEquipmentSlot slot = (GotchiEquipmentSlot)i;
                switch(slot)
                {
                    case GotchiEquipmentSlot.BODY:
                        aavegotchiData.Body_WearableID = gotchi.Data.equippedWearables[i];
                        break;

                    case GotchiEquipmentSlot.FACE:
                        aavegotchiData.Face_WearableID = gotchi.Data.equippedWearables[i];
                        break;

                    case GotchiEquipmentSlot.EYES:
                        aavegotchiData.Eyes_WearableID = gotchi.Data.equippedWearables[i];
                        break;

                    case GotchiEquipmentSlot.HEAD:
                        aavegotchiData.Head_WearableID = gotchi.Data.equippedWearables[i];
                        break;

                    case GotchiEquipmentSlot.HAND_LEFT:
                        aavegotchiData.HandLeft_WearableID = gotchi.Data.equippedWearables[i];
                        break;

                    case GotchiEquipmentSlot.HAND_RIGHT:
                        aavegotchiData.HandRight_WearableID = gotchi.Data.equippedWearables[i];
                        break;

                    case GotchiEquipmentSlot.PET:
                        aavegotchiData.Pet_WearableID = gotchi.Data.equippedWearables[i];
                        break;

                    default:
                        break;
                }
            }

            aavegotchiBase.UpdateForData(aavegotchiData); 
        }
    }
}
