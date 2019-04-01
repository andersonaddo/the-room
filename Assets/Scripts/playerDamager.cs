using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class playerDamager : MonoBehaviour
{

    [SerializeField] float maxHealth;
    [SerializeField] PostProcessVolume damagePP;
    [SerializeField] float restorationSpeed;
    float currentCurveX;
    [SerializeField] AnimationCurve restorationCurve;

    float currentHealth;

    void Start()
    {
        currentHealth = maxHealth;
    }

    void Update()
    {
        currentCurveX = Mathf.MoveTowards(currentCurveX, 0, restorationSpeed * Time.deltaTime);
        damagePP.weight = restorationCurve.Evaluate(currentCurveX);
    }

    public void inflictDamage(int damage)
    {
        currentHealth = Mathf.Clamp(currentHealth - damage, 0, maxHealth);
        damagePP.weight = 1;
        currentCurveX = 1;
    }
}
