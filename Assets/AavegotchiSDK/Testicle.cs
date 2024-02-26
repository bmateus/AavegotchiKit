using GotchiSDK;
using UnityEngine;

public class Testicle : MonoBehaviour
{

    Aavegotchi_Base gotchiBase;
    Animator animator;

    void Start()
    {
        //grab the aavegotchi base
        gotchiBase = GetComponent<Aavegotchi_Base>();
        Aavegotchi_Data data = new Aavegotchi_Data();
        data.CollateralType = ECollateral.USDT;
        data.EyeShape = EEyeShape.USDT;
        data.EyeColor = EEyeColor.Mythical_High;

        data.Head_WearableID = 309;
        data.Body_WearableID = 310;
        data.HandLeft_WearableID = 312;
        data.HandRight_WearableID = 311;

        gotchiBase.UpdateForData(data);
        
        animator = GetComponent<Animator>();        
    }


    [ContextMenu("Test")]
    public void Test()
    {
        animator.SetTrigger("Victory");
    }   
}
