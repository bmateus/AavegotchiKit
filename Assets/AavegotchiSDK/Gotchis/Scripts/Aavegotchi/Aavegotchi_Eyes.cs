using System;
using System.Collections.Generic;
using UnityEngine;

namespace GotchiSDK
{
    public enum EEyeShape
    {
        ETH,
        AAVE,
        DAI,
        LINK,
        USDT,
        USDC,
        TUSD,
        UNI,
        YFI,
        POLYGON,
        wETH,
        wBTC,

        Common1,
        Common2,
        Common3,

        UncommonLow1,
        UncommonLow2,
        UncommonLow3,

        UncommonHigh1,
        UncommonHigh2,
        UncommonHigh3,

        RareLow1,
        RareLow2,
        RareLow3,

        RareHigh1,
        RareHigh2,
        RareHigh3,

        MythicalLow1_H1,
        MythicalLow2_H1,

        MythicalLow1_H2,
        MythicalLow2_H2,
    }

    public enum EEyeColor
    {
        Common,

        Uncommon_Low,
        Uncommon_High,

        Rare_Low,
        Rare_High,

        Mythical_High,
        Mythical_Low,
    }

    [Serializable]
    public struct EyePair
    {
        [SerializeField] public GameObject LeftEye;
        [SerializeField] public GameObject RightEye;
    }

    public class Aavegotchi_Eyes : MonoBehaviour
    {
        [Header("Eye Color Materials")]
        [SerializeField] public Material EyeColor_UncommonLow_Mat;
        [SerializeField] public Material EyeColor_UncommonHigh_Mat;

        [SerializeField] public Material EyeColor_RareLow_Mat;
        [SerializeField] public Material EyeColor_RareHigh_Mat;

        [SerializeField] public Material EyeColor_MythicalLow_Mat;
        [SerializeField] public Material EyeColor_MythicalHigh_Mat;

        [Header("Collateral Eyes")]
        [SerializeField] public EyePair ETH_Eyes;
        [SerializeField] public EyePair AAVE_Eyes;
        [SerializeField] public EyePair DAI_Eyes;
        [SerializeField] public EyePair LINK_Eyes;
        [SerializeField] public EyePair USDT_Eyes;
        [SerializeField] public EyePair USDC_Eyes;
        [SerializeField] public EyePair TUSD_Eyes;
        [SerializeField] public EyePair UNI_Eyes;
        [SerializeField] public EyePair YFI_Eyes;
        [SerializeField] public EyePair POLYGON_Eyes;
        [SerializeField] public EyePair wETH_Eyes;
        [SerializeField] public EyePair wBTC_Eyes;

        [Header("Common Eyes")]
        [SerializeField] public EyePair Common1_Eyes;
        [SerializeField] public EyePair Common2_Eyes;
        [SerializeField] public EyePair Common3_Eyes;

        [Header("Uncommon Eyes")]
        [SerializeField] public EyePair UncommonLow1_Eyes;
        [SerializeField] public EyePair UncommonLow2_Eyes;
        [SerializeField] public EyePair UncommonLow3_Eyes;

        [SerializeField] public EyePair UncommonHigh1_Eyes;
        [SerializeField] public EyePair UncommonHigh2_Eyes;
        [SerializeField] public EyePair UncommonHigh3_Eyes;

        [Header("Rare Eyes")]
        [SerializeField] public EyePair RareLow1_Eyes;
        [SerializeField] public EyePair RareLow2_Eyes;
        [SerializeField] public EyePair RareLow3_Eyes;

        [SerializeField] public EyePair RareHigh1_Eyes;
        [SerializeField] public EyePair RareHigh2_Eyes;
        [SerializeField] public EyePair RareHigh3_Eyes;

        [Header("Mythical Low Eyes")]
        [SerializeField] public EyePair MythicalLow1_H1_Eyes;
        [SerializeField] public EyePair MythicalLow2_H1_Eyes;

        [SerializeField] public EyePair MythicalLow1_H2_Eyes;
        [SerializeField] public EyePair MythicalLow2_H2_Eyes;

        private Dictionary<EEyeShape, EyePair> EyeMap = new Dictionary<EEyeShape, EyePair>();

        //--------------------------------------------------------------------------------------------------
        void Awake()
        {
            EyeMap.Add(EEyeShape.ETH, ETH_Eyes);
            EyeMap.Add(EEyeShape.AAVE, AAVE_Eyes);
            EyeMap.Add(EEyeShape.DAI, DAI_Eyes);
            EyeMap.Add(EEyeShape.LINK, LINK_Eyes);
            EyeMap.Add(EEyeShape.USDT, USDT_Eyes);
            EyeMap.Add(EEyeShape.USDC, USDC_Eyes);
            EyeMap.Add(EEyeShape.TUSD, TUSD_Eyes);
            EyeMap.Add(EEyeShape.UNI, UNI_Eyes);
            EyeMap.Add(EEyeShape.YFI, YFI_Eyes);
            EyeMap.Add(EEyeShape.POLYGON, POLYGON_Eyes);
            EyeMap.Add(EEyeShape.wETH, wETH_Eyes);
            EyeMap.Add(EEyeShape.wBTC, wBTC_Eyes);

            EyeMap.Add(EEyeShape.Common1, Common1_Eyes);
            EyeMap.Add(EEyeShape.Common2, Common2_Eyes);
            EyeMap.Add(EEyeShape.Common3, Common3_Eyes);

            EyeMap.Add(EEyeShape.UncommonLow1, UncommonLow1_Eyes);
            EyeMap.Add(EEyeShape.UncommonLow2, UncommonLow2_Eyes);
            EyeMap.Add(EEyeShape.UncommonLow3, UncommonLow3_Eyes);

            EyeMap.Add(EEyeShape.UncommonHigh1, UncommonHigh1_Eyes);
            EyeMap.Add(EEyeShape.UncommonHigh2, UncommonHigh2_Eyes);
            EyeMap.Add(EEyeShape.UncommonHigh3, UncommonHigh3_Eyes);

            EyeMap.Add(EEyeShape.RareLow1, RareLow1_Eyes);
            EyeMap.Add(EEyeShape.RareLow2, RareLow2_Eyes);
            EyeMap.Add(EEyeShape.RareLow3, RareLow3_Eyes);

            EyeMap.Add(EEyeShape.RareHigh1, RareHigh1_Eyes);
            EyeMap.Add(EEyeShape.RareHigh2, RareHigh2_Eyes);
            EyeMap.Add(EEyeShape.RareHigh3, RareHigh3_Eyes);

            EyeMap.Add(EEyeShape.MythicalLow1_H1, MythicalLow1_H1_Eyes);
            EyeMap.Add(EEyeShape.MythicalLow2_H1, MythicalLow2_H1_Eyes);

            EyeMap.Add(EEyeShape.MythicalLow1_H2, MythicalLow1_H2_Eyes);
            EyeMap.Add(EEyeShape.MythicalLow2_H2, MythicalLow2_H2_Eyes);
        }

        //--------------------------------------------------------------------------------------------------
        public void UpdateConfiguration(EEyeShape eyeShape, ECollateral collateral, EEyeColor eyeColor, Aavegotchi_Collaterals collateralData)
        {
            foreach (var (key, value) in EyeMap)
            {
                if (value.LeftEye != null)
                {
                    value.LeftEye.SetActive(key == eyeShape);
                }
                if (value.RightEye != null)
                {
                    value.RightEye.SetActive(key == eyeShape);
                }

                if (key == eyeShape)
                {
                    var primaryMaterial = collateralData.GetData(collateral).PrimaryMaterial;
                    var secondaryMaterial = collateralData.GetData(collateral).PrimaryMaterial;

                    switch (eyeColor)
                    {
                        case EEyeColor.Uncommon_Low:
                            secondaryMaterial = EyeColor_UncommonLow_Mat;
                            break;
                        case EEyeColor.Uncommon_High:
                            secondaryMaterial = EyeColor_UncommonHigh_Mat;
                            break;
                        case EEyeColor.Rare_Low:
                            secondaryMaterial = EyeColor_RareLow_Mat;
                            break;
                        case EEyeColor.Rare_High:
                            secondaryMaterial = EyeColor_RareHigh_Mat;
                            break;
                        case EEyeColor.Mythical_Low:
                            secondaryMaterial = EyeColor_MythicalLow_Mat;
                            break;
                        case EEyeColor.Mythical_High:
                            secondaryMaterial = EyeColor_MythicalHigh_Mat;
                            break;

                    }

                    if (value.LeftEye.TryGetComponent<Aavegotchi_EyeColor>(out var leftColorer))
                    {
                        leftColorer.UpdateEyeColors(primaryMaterial, secondaryMaterial);
                    }

                    if (value.RightEye.TryGetComponent<Aavegotchi_EyeColor>(out var rightColorer))
                    {
                        rightColorer.UpdateEyeColors(primaryMaterial, secondaryMaterial);
                    }
                }
            }
        }
    }
}
