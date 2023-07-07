using UnityEngine;

namespace PortalDefender.AavegotchiKit
{
    // This class can be used to create a local gotchi that is not connected to the blockchain.
    // It is useful for:
    // - creating a gotchi for the player to play with before they have a gotchi on the blockchain
    // - creating NPCs
    // - testing and debugging.
    class GotchiLocalData : MonoBehaviour
    {
        [SerializeField]
        GotchiData data_;

        [SerializeField]
        GotchiHandPose initialHandPose_;

        void Start()
        {
            Gotchi gotchi_ = GetComponent<Gotchi>();
            if (gotchi_ != null)
            {
                gotchi_.Init(data_);
                gotchi_.State.HandPose = initialHandPose_;
            }
        }

    }
}