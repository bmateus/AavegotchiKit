using UnityEngine;

namespace PortalDefender.AavegotchiKit
{
    public class Gotchi : MonoBehaviour
    {
        GotchiAppearance appearance;

        GotchiFacing facing = GotchiFacing.FRONT;

        private void Awake()
        {
            appearance = GetComponentInChildren<GotchiAppearance>();
        }

        public void Init(GotchiData data)
        {
            appearance.Init(data);
        }

        public void SetFacing(GotchiFacing facing)
        {
            this.facing = facing;
            appearance.SetFacing(facing);
        }
    }
}
