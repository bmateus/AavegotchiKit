using Aavegotchi.AavegotchiDiamond.ContractDefinition;
using Aavegotchi.AavegotchiDiamond.Service;
using Cysharp.Threading.Tasks;
using Nethereum.Web3;
using System.Text;

namespace PortalDefender.AavegotchiKit
{
    public class BasePartsFetcher
    {
        Web3 web3_;

        Web3 Web3 => web3_ ??= new Web3(Constants.DefaultPolygonRPC);

        AavegotchiDiamondService diamondSvc_;
        AavegotchiDiamondService GotchiDiamond => diamondSvc_ ??= new AavegotchiDiamondService(Web3, Constants.AavegotchiDiamondAddress);

        public async UniTask<string> GetPortalClosed(int hauntId)
        {
            var svg = await GotchiDiamond.GetSvgQueryAsync(new GetSvgFunction() { SvgType = Encoding.UTF8.GetBytes("portal-closed"), ItemId = hauntId });

            return "<svg xmlns=\"http://www.w3.org/2000/svg\" viewBox=\"0 0 64 64\">" + svg + "</svg>";
        }

        public async UniTask<string> GetPortalOpen(int hauntId)
        {
            var svg = await GotchiDiamond.GetSvgQueryAsync(new GetSvgFunction() { SvgType = Encoding.UTF8.GetBytes("portal-open"), ItemId = hauntId });

            return "<svg xmlns=\"http://www.w3.org/2000/svg\" viewBox=\"0 0 64 64\">" + svg + "</svg>";
        }

        enum BasePartsId : int
        {
            AAVEGOTCHI_BODY_SVG_ID = 2,
            HANDS_SVG_ID = 3,
            BACKGROUND_SVG_ID = 4,
        }

        private async UniTask<string[]> GetBaseParts(BasePartsId basePartsId)
        {
            byte[][] types = new byte[][]
            {
                Encoding.UTF8.GetBytes("aavegotchi"),
                Encoding.UTF8.GetBytes("aavegotchi-left"),
                Encoding.UTF8.GetBytes("aavegotchi-right"),
                Encoding.UTF8.GetBytes("aavegotchi-back")
            };

            string[] svgs = new string[types.Length];
            for (int i = 0; i < types.Length; ++i)
            {
                svgs[i] = await GotchiDiamond.GetSvgQueryAsync(new GetSvgFunction() { SvgType = types[i], ItemId = (int)basePartsId });
            }

            return svgs;
        }

        public async UniTask<string[]> GetBaseBody()
        {
            return await GetBaseParts(BasePartsId.AAVEGOTCHI_BODY_SVG_ID);
        }

        public async UniTask<string[]> GetBaseHands()
        {
            return await GetBaseParts(BasePartsId.HANDS_SVG_ID);
        }

        public async UniTask<string[]> GetBaseBackground()
        {
            return await GetBaseParts(BasePartsId.BACKGROUND_SVG_ID);
        }

    }
}
