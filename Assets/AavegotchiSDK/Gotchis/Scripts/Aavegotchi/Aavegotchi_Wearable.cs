using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace GotchiSDK
{
    public enum EWearableSlot
    {
        Body,
        Face,
        Eyes,
        Head,
        Hand_left,
        Hand_right,
        Pet,
    }

    public enum EWearableTypeHint
    {
        Body,
        Face,
        Eyes,
        Head,
        Hand_Melee,
        Hand_Shield,
        Hand_Grenade,
        Hand_Ranged,
        Hand_Other,
        Pet,
        Pet_Back,
        Face_Mouth,
    }

    public class Aavegotchi_Wearable : MonoBehaviour
    {
        [Header("Must Match!")]
        [SerializeField]
        public int WearableID;

        [Header("[OPTIONAL] Load a material addressable dynmically to apply to mesh renderers")]
        [SerializeField]
        private string MaterialName;

        [Header("[OPTIONAL] used by wearables like pets to control their animations")]
        [SerializeField]
        public Animator CharacterAnimator = null;

        [Header("Required for fixing bones")]
        [SerializeField]
        private List<SkinnedMeshRenderer> SkinnedMeshRenderers;

        [Header("[OPTIONAL] used for dynamic materials")]
        [SerializeField]
        private List<MeshRenderer> MeshRenderers;

        [Header("Wearable types")]
        [SerializeField]
        public EWearableTypeHint WearableTypeHint;

        [SerializeField]
        public EWearableSlot EquippedSlot;

        [Header("[OPTIONAL] Used primarily for body wearables")]
        [SerializeField]
        public GameObject LeftShoulder;

        [SerializeField]
        public GameObject RightShoulder;

        [Header("[OPTIONAL] Hand Transform overrides")]
        [SerializeField]
        public Transform LeftHandTransform;

        [SerializeField]
        public Transform RightHandTransform;

        [Header("Base Gotchi modifications")]
        [SerializeField]
        public bool DisableCollateral = false;

        [SerializeField]
        public bool DisableEyes = false;

        [SerializeField]
        public bool IsSkinnedWeapon = false;

        [SerializeField]
        public bool DisableShoulder = false;

        //--------------------------------------------------------------------------------------------------
        public void Initialize(Transform rootBone, EWearableSlot targetSlot, SkinnedMeshRenderer mainBodyRenderer, Action onLoadedCompletedCB)
        {
            EquippedSlot = targetSlot;

            foreach (var skinnedMeshRenderer in SkinnedMeshRenderers)
            {
                skinnedMeshRenderer.bones = mainBodyRenderer.bones;
                skinnedMeshRenderer.rootBone = rootBone;
            }

            if (IsSkinnedWeapon)
            {
                // Since theres only 1 right now, it seems reasonable to assume skinned mesh renderer on indexes means which arm its for.
                if (SkinnedMeshRenderers.Count == 2)
                {
                    SkinnedMeshRenderers[0].gameObject.SetActive(targetSlot == EWearableSlot.Hand_left);
                    SkinnedMeshRenderers[1].gameObject.SetActive(targetSlot == EWearableSlot.Hand_right);
                }
            }

            if (EquippedSlot == EWearableSlot.Hand_left && LeftHandTransform != null)
            {
                foreach (var meshRenderer in MeshRenderers)
                {
                    meshRenderer.transform.SetParent(LeftHandTransform, false);
                }
            }
            else if (EquippedSlot == EWearableSlot.Hand_right && RightHandTransform != null)
            {
                foreach (var meshRenderer in MeshRenderers)
                {
                    meshRenderer.transform.SetParent(RightHandTransform, false);
                }
            }

            if (string.IsNullOrEmpty(MaterialName))
            {
                onLoadedCompletedCB.Invoke();
                return;
            }

            gameObject.SetActive(false);

            // Loading material
            Addressables.LoadAssetAsync<Material>($"Wearable_Mat_{MaterialName}").Completed += (task) =>
            {
                if (task.Result != null)
                {
                    foreach (var skinnedMeshRenderer in SkinnedMeshRenderers)
                    {
                        if (skinnedMeshRenderer.materials.Length > 1)
                        {
                            var materialsCopy = skinnedMeshRenderer.materials;

                            for (int i = 0; i < materialsCopy.Length; ++i)
                            {
                                materialsCopy[i] = task.Result;
                            }

                            skinnedMeshRenderer.materials = materialsCopy;
                        }
                        else
                        {
                            skinnedMeshRenderer.material = task.Result;
                        }
                    }

                    foreach (var meshRenderer in MeshRenderers)
                    {
                        if (meshRenderer.materials.Length > 1)
                        {
                            var materialsCopy = meshRenderer.materials;

                            for (int i = 0; i < materialsCopy.Length; ++i)
                            {
                                materialsCopy[i] = task.Result;
                            }

                            meshRenderer.materials = materialsCopy;
                        }
                        else
                        {
                            meshRenderer.material = task.Result;
                        }
                    }
                }
                else
                {
                    Debug.LogError($"Failed to load material: {MaterialName} - for wearable with ID: {WearableID}");
                }

                gameObject.SetActive(true);

                onLoadedCompletedCB.Invoke();
            };
        }

        //--------------------------------------------------------------------------------------------------
        internal void UpdateSleeves(bool leftSleeveDisabled, bool rightSleeveDisabled)
        {
            if (LeftShoulder != null)
            {
                LeftShoulder.gameObject.SetActive(!leftSleeveDisabled);
            }

            if (RightShoulder != null)
            {
                RightShoulder.gameObject.SetActive(!rightSleeveDisabled);
            }
        }
    }
}
