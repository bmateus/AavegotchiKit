using System.Collections.Generic;
using UnityEngine;

namespace GotchiSDK
{
    public class Aavegotchi_CollateralsManager : MonoBehaviour
    {
        [Header("Gameobject Links")]
        [SerializeField] public List<SkinnedMeshRenderer> CollateralColoredRenderers = new List<SkinnedMeshRenderer>();
        [SerializeField] public SkinnedMeshRenderer CheekRenderer;

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
        public void SetupForCollateral(ECollateral targetCollateral)
        {
            CheekRenderer.material = CollateralDataMap[targetCollateral].CheeksMaterial;

            foreach (var skinnedRenderer in CollateralColoredRenderers)
            {
                skinnedRenderer.material = CollateralDataMap[targetCollateral].PrimaryMaterial;
            }
        }

        //--------------------------------------------------------------------------------------------------
        public CollateralData GetData(ECollateral targetCollateral)
        {
            return CollateralDataMap[targetCollateral];
        }
    }
}

