﻿using RiskOfOptions.Components.Options;
using RiskOfOptions.Interfaces;
using RiskOfOptions.Options;
using RoR2.UI;
using UnityEngine;
using UnityEngine.UI;

namespace RiskOfOptions.Components.OptionComponents
{
    public class OverrideController : MonoBehaviour
    {
        public ModOptionPanelController modOptionPanelController;

        public string overridingName;
        public string overridingCategoryName;

        public string modGuid;

        private bool previouslyOverridden = false;

        private void Awake()
        {
            
        }

        private void OnEnable()
        {
            CheckForOverride();
        }

        public void CheckForOverride(bool onEnable = false)
        {
            modOptionPanelController = GetComponentInParent<ModOptionPanelController>();

            var tempOption = ModSettingsManager.GetOption(GetComponent<BaseSettingsControl>().settingName);

            if (tempOption.OptionOverride == null)
                return;

            HGButton[] buttons = GetComponentsInChildren<HGButton>();
            Slider[] sliders = GetComponentsInChildren<Slider>();

            var overridingOption = ModSettingsManager.GetOption(overridingName, overridingCategoryName, modGuid);

            if (!(overridingOption is IBoolProvider overrideBoolProvider))
                return;

            var currentlyOverridden = (overrideBoolProvider.Value && tempOption.OptionOverride.OverrideOnTrue) || (!overrideBoolProvider.Value && !tempOption.OptionOverride.OverrideOnTrue);

            if (currentlyOverridden)
            {
                if (previouslyOverridden)
                {
                    return;
                }

                foreach (var button in buttons)
                {
                    button.interactable = false;
                }


                foreach (var slider in sliders)
                {
                    slider.interactable = false;

                    slider.transform.Find("Fill Area").Find("Fill").GetComponent<UnityEngine.UI.Image>().color = slider.colors.disabledColor;
                    slider.transform.parent.Find("TextArea").GetComponent<UnityEngine.UI.Image>().color = slider.colors.disabledColor;
                }

                //if (tempOption is SliderOption overridenSettingsSlider)
                //{
                //    GetComponentInChildren<SettingsSlider>()?.MoveSlider(((SliderOverride)overridenSettingsSlider.OptionOverride).ValueToReturnWhenOverriden);
                //    GetComponentInChildren<SettingsStepSlider>()?.MoveSlider(((SliderOverride)overridenSettingsSlider.OptionOverride).ValueToReturnWhenOverriden);
                //}
                if (tempOption is CheckBoxOption overridenCheckBoxOption)
                {
                    if (!onEnable)
                    {
                        overridenCheckBoxOption.Invoke();
                    }
                }
            }
            else
            {
                if (!previouslyOverridden)
                {
                    return;
                }

                foreach (var button in buttons)
                {
                    button.interactable = true;
                }

                foreach (var slider in sliders)
                {
                    slider.interactable = true;

                    slider.transform.Find("Fill Area").Find("Fill").GetComponent<UnityEngine.UI.Image>().color = slider.colors.normalColor;
                    slider.transform.parent.Find("TextArea").GetComponent<UnityEngine.UI.Image>().color = GetComponent<HGButton>().colors.normalColor;
                }

                //if (tempOption is SliderOption overridenSettingsSlider)
                //{ 
                //    var slider = GetComponentInChildren<SettingsSlider>();

                //    if (slider != null)
                //    {
                //        float newValue = overridenSettingsSlider.Value.Remap(0, 100, slider.minValue, slider.maxValue);
                //        slider.slider.value = newValue;
                //    }

                //    GetComponentInChildren<SettingsSlider>()?.MoveSlider(tempOption.GetValue<float>());
                //    GetComponentInChildren<SettingsStepSlider>()?.MoveSlider(tempOption.GetValue<float>());
                //}
                if (tempOption is CheckBoxOption overridenCheckBoxOption)
                {
                    if (!onEnable)
                    {
                        overridenCheckBoxOption.Invoke();
                    }
                }
            }

            previouslyOverridden = currentlyOverridden;
        }
    }
}