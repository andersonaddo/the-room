using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class cartDamageSlider : MonoBehaviour
{
    [SerializeField] Material goodMaterial, mediumMaterial, badMaterial;
    bool hasSwitchedToMedium, hasSwitchedToBad;

    [SerializeField] Image sliderFill;
    [SerializeField] Slider slider;

    void Start()
    {
        sliderFill.material = goodMaterial;
        playerDamager.playerDamaged += changeSliderValue;
    }

    void changeSliderValue(float newValue)
    {
        slider.value = newValue; //This will call updateMaterials() automatically
    }

    public void updateMaterials()
    {
        if (slider.value <= 0.6f && slider.value > 0.3f && !hasSwitchedToMedium)
        {
            sliderFill.material = mediumMaterial;
            hasSwitchedToMedium = true;
        }

        if (slider.value <= 0.3f && !hasSwitchedToBad)
        {
            sliderFill.material = badMaterial;
            hasSwitchedToMedium = true;
        }
    }
}
