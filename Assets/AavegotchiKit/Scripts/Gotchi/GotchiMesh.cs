using UnityEngine;

namespace PortalDefender.AavegotchiKit
{    
    public class GotchiMesh : MonoBehaviour
    {
        [SerializeField]
        GotchiFacing facing_;

        public GotchiFacing Facing => facing_;
        
        [SerializeField]
        //[HideInInspector]
        Mesh mesh_;

        [SerializeField]
        //[HideInInspector]
        Texture2D texture_;

        [SerializeField]
        MeshRenderer renderer_;

        [SerializeField]
        MeshFilter filter_;


        [ContextMenu("Cleanup")]
        public void Cleanup()
        {
#if UNITY_EDITOR
            if (texture_ != null)
            {
                DestroyImmediate(texture_);
            }

            if (mesh_ != null)
            {
                DestroyImmediate(mesh_);
            }
#endif
        }

        public void UpdateMesh(string svgData, string meshName = "GotchiMesh")
        {
            var result = SvgLoader.CreateSvgMesh(svgData, Vector2.zero);

            Cleanup();

            mesh_ = result.Item1;
            mesh_.name = meshName;
            texture_ = result.Item2;

            filter_.mesh = result.Item1;
            renderer_.sharedMaterial.mainTexture = result.Item2;
           
        }
    }
}