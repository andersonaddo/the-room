using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using EZCameraShake;

public class playerDamager : MonoBehaviour
{

    [SerializeField] float maxHealth;
    float currentHealth;

    public static event Action<float> playerDamaged;

    [SerializeField] PostProcessVolume damagePP;
    [SerializeField] float restorationSpeed;
    [SerializeField] AnimationCurve restorationCurve;
    float currentCurveX;

    [SerializeField] float shakeFadeIn, shakeFadeOut;

    void Start()
    {
        currentHealth = maxHealth;
    }

    void Update()
    {
        currentCurveX = Mathf.MoveTowards(currentCurveX, 0, restorationSpeed * Time.deltaTime);
        damagePP.weight = restorationCurve.Evaluate(currentCurveX);
    }

    /// <summary>
    /// Inflict damage onto the player
    /// </summary>
    /// <param name="damage">The amount of damage to inflict</param>
    /// <param name="shakeInformation">x represents shake magnitude, y shake roughness</param>
    public void inflictDamage(int damage, Vector2 shakeInformation)
    {
        currentHealth = Mathf.Clamp(currentHealth - damage, 0, maxHealth);
        damagePP.weight = 1;
        currentCurveX = 1;
        CameraShaker.Instance.ShakeOnce(shakeInformation.x, shakeInformation.y, shakeFadeIn, shakeFadeOut);
        if (playerDamaged != null) playerDamaged(currentHealth / maxHealth);
    }
}
