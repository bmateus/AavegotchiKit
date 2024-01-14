using Cysharp.Threading.Tasks;

namespace PortalDefender.AavegotchiKit
{
    /// <summary>
    /// Provides an interface for fetching SVG data to be used in the Gotchi's appearance.
    /// </summary>
    public interface ISvgProvider
    {
        UniTask<GotchiSvgData> GetSvg(GotchiData gotchiData);

        UniTask<string> GetSvg(GotchiData gotchiData, GotchiFacing facing);
    }

}