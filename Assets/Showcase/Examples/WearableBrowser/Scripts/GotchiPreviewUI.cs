using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace PortalDefender.AavegotchiKit.WearableBrowser
{
    // Display's a Gotchi using uGUI

    // Allows changing the:
    // - hand pose
    // - eye expression
    // - mouth expression
    // - facing direction

    public class GotchiPreviewUI : MonoBehaviour
    {
        [SerializeField]
        Gotchi gotchi;

        public Gotchi Gotchi => gotchi;

        [SerializeField]
        Button turnButton;

        [SerializeField]
        TMP_Dropdown eyeExpressionDropdown;

        [SerializeField]
        TMP_Dropdown mouthExpressionDropdown;

        [SerializeField]
        TMP_Dropdown handposeDropdown;

        [SerializeField]
        TMP_Dropdown collateralDropdown;

        [SerializeField]
        TMP_InputField eyeShapeInput;

        [SerializeField]
        TMP_InputField eyeColorInput;

        int facing = 0;

        GotchiData gotchiData = new GotchiData();

        private void Start()
        {
            loadGotchi();

            turnButton.onClick.AddListener(turn);

            handposeDropdown.AddOptions(new List<string> { "Down Closed", "Down Open", "Up" });
            handposeDropdown.value = (int)gotchi.State.HandPose;
            handposeDropdown.onValueChanged.AddListener(v =>
            {
                gotchi.State.HandPose = (GotchiHandPose)v;
            });

            eyeExpressionDropdown.AddOptions(new List<string> { "None", "Happy", "Mad", "Sleeping" });
            eyeExpressionDropdown.value = (int)gotchi.State.EyeExpression;
            eyeExpressionDropdown.onValueChanged.AddListener(v =>
            {
                gotchi.State.EyeExpression = (GotchiEyeExpression)v;
            });

            mouthExpressionDropdown.AddOptions(new List<string> { "Happy", "Neutral" });
            mouthExpressionDropdown.value = (int)gotchi.State.MouthExpression;
            mouthExpressionDropdown.onValueChanged.AddListener(v =>
            {
                gotchi.State.MouthExpression = (GotchiMouthExpression)v;
            });

            //get collateral options
            collateralDropdown.AddOptions(GotchiDataProvider.Instance.collateralDB.collaterals.Select(x => x.name).ToList());
            collateralDropdown.onValueChanged.AddListener(v =>
            {
                var collateral = GotchiDataProvider.Instance.collateralDB.collaterals[v];
                Debug.Log($"Collateral changed to: {collateral.name}");
                gotchi.Data.collateral = collateral.collateralType;
                gotchi.Init(gotchi.Data); //re-init with current data
            });

            eyeShapeInput.text = $"{gotchi.Data.numericTraits[(int)GotchiTrait.EyeShape]}";
            eyeShapeInput.onEndEdit.AddListener(v =>
            {
                if (short.TryParse(v, out short shape))
                {
                    gotchi.Data.numericTraits[(int)GotchiTrait.EyeShape] = shape;
                    gotchi.Init(gotchi.Data); //re-init with current data
                }
            });

            eyeColorInput.text = $"{gotchi.Data.numericTraits[(int)GotchiTrait.EyeColor]}";
            eyeColorInput.onEndEdit.AddListener(v =>
            {
                if (short.TryParse(v, out short shape))
                {
                    gotchi.Data.numericTraits[(int)GotchiTrait.EyeColor] = shape;
                    gotchi.Init(gotchi.Data); //re-init with current data
                }
            });

        }


        void turn()
        {
            facing = (facing + 1) % 4;
            switch (facing)
            {
                case 0: gotchi.State.Facing = GotchiFacing.FRONT; break;
                case 1: gotchi.State.Facing = GotchiFacing.LEFT; break;
                case 2: gotchi.State.Facing = GotchiFacing.BACK; break;
                case 3: gotchi.State.Facing = GotchiFacing.RIGHT; break;
            }
        }

        void loadGotchi()
        {
            gotchi.gameObject.SetActive(true);
            gotchi.Init(gotchiData);

            gotchi.State.PropertyChanged += State_PropertyChanged;

            handposeDropdown.value = (int)gotchi.State.HandPose;
            eyeExpressionDropdown.value = (int)gotchi.State.EyeExpression;
            mouthExpressionDropdown.value = (int)gotchi.State.MouthExpression;
        }

        private void State_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            handposeDropdown.SetValueWithoutNotify((int)gotchi.State.HandPose);
            eyeExpressionDropdown.SetValueWithoutNotify((int)gotchi.State.EyeExpression);
            mouthExpressionDropdown.SetValueWithoutNotify((int)gotchi.State.MouthExpression);
        }
    }
}

