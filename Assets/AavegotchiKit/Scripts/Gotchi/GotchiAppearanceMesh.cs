using System.ComponentModel;
using UnityEngine;

namespace PortalDefender.AavegotchiKit
{
    public class GotchiAppearanceMesh : MonoBehaviour, IGotchiAppearance
    {
        Gotchi gotchi;

        [SerializeField]
        GotchiMesh[] meshes; // there should be one for each facing direction

        private void Awake()
        {
            // hide all bodies until it's done loading
            for (int i=0; i < meshes.Length; i++)
            {
                meshes[i].gameObject.SetActive(false);
            }
        }

        public void Init(Gotchi gotchi)
        {
            this.gotchi = gotchi;
            this.gotchi.State.PropertyChanged -= State_PropertyChanged;
            this.gotchi.State.PropertyChanged += State_PropertyChanged;
            SetFacing(GotchiFacing.FRONT);
        }

        private void State_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            SetFacing(gotchi.State.Facing);
        }

        public void UpdateGotchiMesh(string svgData, GotchiFacing facing, string meshName = "GotchiMesh")
        {
            var result = SvgLoader.CreateSvgMesh(svgData, Vector2.zero);
            meshes[(int)facing].UpdateMesh(svgData, meshName);
        }   

        void SetFacing(GotchiFacing facing)
        {
            for (int i = 0; i < meshes.Length; i++)
            {
                meshes[i].gameObject.SetActive(i == (int)facing);
            }
        }

    }
}