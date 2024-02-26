using UnityEngine;

namespace GotchiSDK
{
    public class Aavegotchi_EyeColor : MonoBehaviour
    {
        [SerializeField] 
        private int StakeColorMaterialIndex = 0;

        [SerializeField] 
        private int EyeColorMaterialIndex = -1;

        //--------------------------------------------------------------------------------------------------
        public void UpdateEyeColors(Material stackMaterial, Material eyeColorMaterial)
        {
            if (gameObject.TryGetComponent<SkinnedMeshRenderer>(out var skinnedMeshRenderer))
            {
                var materials = skinnedMeshRenderer.materials;

                if (StakeColorMaterialIndex >= 0 && StakeColorMaterialIndex < materials.Length)
                {
                    materials[StakeColorMaterialIndex] = stackMaterial;
                }

                if (EyeColorMaterialIndex >= 0 && EyeColorMaterialIndex < materials.Length)
                {
                    materials[EyeColorMaterialIndex] = eyeColorMaterial;
                }

                skinnedMeshRenderer.materials = materials;
            }
        }
    }
}
