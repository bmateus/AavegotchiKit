using UnityEngine;

namespace com.mycompany
{
    public class Gotchi : MonoBehaviour
    {
        GotchiAppearance appearance;

        AavegotchiData.Facing facing = AavegotchiData.Facing.FRONT;

        private void Awake()
        {
            appearance = GetComponentInChildren<GotchiAppearance>();
        }

        public void Init(GotchiData data)
        {
            appearance.Init(data);
        }

        public void SetFacing(AavegotchiData.Facing facing)
        {
            this.facing = facing;
            appearance.SetFacing(facing);
        }
    }
}
