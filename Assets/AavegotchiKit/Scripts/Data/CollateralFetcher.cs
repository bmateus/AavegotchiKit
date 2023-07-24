using Aavegotchi.AavegotchiDiamond.ContractDefinition;
using Aavegotchi.AavegotchiDiamond.Service;
using Cysharp.Threading.Tasks;
using Nethereum.Hex.HexConvertors.Extensions;
using Nethereum.Web3;
using System.Numerics;
using System.Text;

namespace PortalDefender.AavegotchiKit
{
    public class CollateralFetcher
    {
        Web3 web3_;

        Web3 Web3 => web3_ ??= new Web3(Constants.DefaultPolygonRPC);

        AavegotchiDiamondService diamondSvc_;
        AavegotchiDiamondService GotchiDiamond => diamondSvc_ ??= new AavegotchiDiamondService(Web3, Constants.AavegotchiDiamondAddress);

        public async UniTask<Collateral> GetCollateral(string collateralType, int hauntId)
        {
            var hauntCollaterals = await GotchiDiamond.GetCollateralInfoQueryAsync(new BigInteger(hauntId));
            foreach (var collateral in hauntCollaterals.Collateralinfo)
            {
                var address = collateral.CollateralType;
                if (address == collateralType)
                {
                    var typeInfo = collateral.CollateralTypeInfo;

                    Collateral c = new Collateral();
                    c.collateralType = collateral.CollateralType;
                    c.haunt = hauntId;
                    c.modifiers = typeInfo.Modifiers.ToArray();
                    c.primaryColor = typeInfo.PrimaryColor.ToHex(true);
                    c.secondaryColor = typeInfo.SecondaryColor.ToHex(true);
                    c.cheekColor = typeInfo.CheekColor.ToHex(true);
                    c.svgId = typeInfo.SvgId;
                    c.eyeShapeSvgId = typeInfo.EyeShapeSvgId;
                    c.svgs = await GetCollateralSvgs(typeInfo.SvgId);
                    c.eyeShapeSvgs = await GetCollateralEyeShapeSvgs(typeInfo.EyeShapeSvgId, hauntId);
                    return c;
                }
            }

            return null;
        }

        private async UniTask<string[]> GetCollateralSvgs(int svgId)
        {
            byte[][] types = new byte[][]
            {
                Encoding.UTF8.GetBytes("collaterals"),
                Encoding.UTF8.GetBytes("collaterals-left"),
                Encoding.UTF8.GetBytes("collaterals-right"),
                Encoding.UTF8.GetBytes("collaterals-back")
            };

            string[] svgs = new string[types.Length];
            for (int i = 0; i < types.Length; ++i)
            {
                svgs[i] = await GotchiDiamond.GetSvgQueryAsync(new GetSvgFunction() { SvgType = types[i], ItemId = svgId });
            }

            return svgs;
        }

        private async UniTask<string[]> GetCollateralEyeShapeSvgs(int eyeShapeSvgId, int hauntId)
        {
            string hauntIdStr = hauntId == 1 ? "" : "H" + hauntId;

            byte[][] types = new byte[][]
            {
                Encoding.UTF8.GetBytes($"eyeShapes{hauntIdStr}"),
                Encoding.UTF8.GetBytes($"eyeShapes{hauntIdStr}-left"),
                Encoding.UTF8.GetBytes($"eyeShapes{hauntIdStr}-right")
                //no eyes when facing back
            };

            string[] svgs = new string[types.Length];
            for (int i = 0; i < types.Length; ++i)
            {
                svgs[i] = await GotchiDiamond.GetSvgQueryAsync(new GetSvgFunction() { SvgType = types[i], ItemId = eyeShapeSvgId });
            }

            return svgs;
        }

    }
}