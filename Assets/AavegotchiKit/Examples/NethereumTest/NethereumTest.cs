using System;
using UnityEngine;
using Aavegotchi.AavegotchiDiamond.Service;
using Aavegotchi.AavegotchiDiamond.ContractDefinition;
using Nethereum.Web3;
using Cysharp.Threading.Tasks;
using Nethereum.Unity.Rpc;
using Unity.VectorGraphics;
using TMPro;

public class TestNethereum : MonoBehaviour
{
    [SerializeField]
    SVGImage img;

    [SerializeField]
    TMP_InputField gotchiId;

    private void Start()
    {
        gotchiId.text = "23202"; //Portal Defender
    }

    public void GetAavegotchiSvg()
    {
        Debug.Log($"GetAavegotchiSvg({gotchiId.text})");
        GetAavegotchiSvgAsync().Forget();        
    }

    async UniTaskVoid GetAavegotchiSvgAsync()
    {
        var rpc = "https://rpc-mainnet.matic.quiknode.pro";
        
        var web3 = new Web3(new UnityWebRequestRpcTaskClient(new Uri(rpc)));

        var tokenId = int.Parse(gotchiId.text);

        var getAavegotchiSvg = new GetAavegotchiSvgFunction { TokenId = tokenId };

        var AAVEGOTCHI_DIAMOND_ADDRESS = "0x86935F11C86623deC8a25696E1C19a8659CbF95d";

        var svc = new AavegotchiDiamondService(web3, AAVEGOTCHI_DIAMOND_ADDRESS);

        try
        {            
            if (img)
            {
                img.sprite = null;
                var svg = await svc.GetAavegotchiSvgQueryAsync(getAavegotchiSvg);
                //Debug.Log("Got SVG: " + svg);
                var sprite = SvgLoader.CreateSvgSprite(svg, Vector2.zero);
                img.sprite = sprite;
            }
        }
        catch (Exception e)
        {
            Debug.Log("Error: " + e.Message);
        }
    }
}
