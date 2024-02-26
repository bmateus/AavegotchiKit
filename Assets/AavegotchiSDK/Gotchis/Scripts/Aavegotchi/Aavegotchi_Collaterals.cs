using System;
using System.Collections.Generic;
using UnityEngine;

namespace GotchiSDK
{
    public enum ECollateral
    {
        Eth,
        Aave,
        Dai,
        Link,
        USDT,
        USDC,
        TUSD,
        Uni,
        Yfi,
        Polygon,
        wEth,
        wBTC
    }

    [Serializable]
    public struct CollateralData
    {
        [SerializeField] public string CollateralName;
        [SerializeField] public GameObject CollateralMesh;
        [SerializeField] public Material PrimaryMaterial;
        [SerializeField] public Material CheeksMaterial;
        [SerializeField] public Material InnerMouthMaterial;
        [ColorUsage(true, true)]
        [SerializeField] public Color PrimaryColor;
        [ColorUsage(true, true)]
        [SerializeField] public Color SecondaryColor;
        [ColorUsage(true, true)]
        [SerializeField] public Color CheeksColor;
    }

    public class Aavegotchi_Collaterals : MonoBehaviour
    {
        [Header("Object References")]
        [SerializeField] private List<SkinnedMeshRenderer> CheekRenderers = new List<SkinnedMeshRenderer>();
        [SerializeField] private List<SkinnedMeshRenderer> EmotionEyeRenderers = new List<SkinnedMeshRenderer>();
        [SerializeField] private List<SkinnedMeshRenderer> MouthRenderers = new List<SkinnedMeshRenderer>();
        [SerializeField] private SkinnedMeshRenderer InnerMouthRenderer;

        [Header("CollateralData")]
        [SerializeField] public CollateralData EthCollateral;
        [SerializeField] public CollateralData AaveCollateral;
        [SerializeField] public CollateralData DaiCollateral;
        [SerializeField] public CollateralData LinkCollateral;
        [SerializeField] public CollateralData USDTCollateral;
        [SerializeField] public CollateralData USDCCollateral;
        [SerializeField] public CollateralData TUSDCollateral;
        [SerializeField] public CollateralData UniCollateral;
        [SerializeField] public CollateralData YfiCollateral;
        [SerializeField] public CollateralData PolygonCollateral;
        [SerializeField] public CollateralData wEthCollateral;
        [SerializeField] public CollateralData wBTCCollateral;

        private Dictionary<ECollateral, CollateralData> CollateralDataMap = new Dictionary<ECollateral, CollateralData>();

        //--------------------------------------------------------------------------------------------------
        void Awake()
        {
            CollateralDataMap.Add(ECollateral.Eth, EthCollateral);
            CollateralDataMap.Add(ECollateral.Aave, AaveCollateral);
            CollateralDataMap.Add(ECollateral.Dai, DaiCollateral);
            CollateralDataMap.Add(ECollateral.Link, LinkCollateral);
            CollateralDataMap.Add(ECollateral.USDT, USDTCollateral);
            CollateralDataMap.Add(ECollateral.USDC, USDCCollateral);
            CollateralDataMap.Add(ECollateral.TUSD, TUSDCollateral);
            CollateralDataMap.Add(ECollateral.Uni, UniCollateral);
            CollateralDataMap.Add(ECollateral.Yfi, YfiCollateral);
            CollateralDataMap.Add(ECollateral.Polygon, PolygonCollateral);
            CollateralDataMap.Add(ECollateral.wEth, wEthCollateral);
            CollateralDataMap.Add(ECollateral.wBTC, wBTCCollateral);
        }

        //--------------------------------------------------------------------------------------------------
        public void UpdateConfiguration(ECollateral collateral)
        {
            foreach (var (key, value) in CollateralDataMap)
            {
                value.CollateralMesh?.SetActive(key == collateral);
            }

            var collateralData = CollateralDataMap[collateral];

            foreach (var cheekRenderer in CheekRenderers)
            {
                cheekRenderer.material = collateralData.CheeksMaterial;
            }

            foreach (var emotionRenderer in EmotionEyeRenderers)
            {
                emotionRenderer.material = collateralData.PrimaryMaterial;
            }

            foreach (var mouthRenderer in MouthRenderers)
            {
                mouthRenderer.material = collateralData.PrimaryMaterial;
            }

            var materials = InnerMouthRenderer.materials;
            materials[1] = collateralData.InnerMouthMaterial;
            InnerMouthRenderer.materials = materials;
        }

        //--------------------------------------------------------------------------------------------------
        public CollateralData GetData(ECollateral targetCollateral)
        {
            return CollateralDataMap[targetCollateral];
        }
    }
}