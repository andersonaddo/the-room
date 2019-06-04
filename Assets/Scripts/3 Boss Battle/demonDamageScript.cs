using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;

/// <summary>
/// Manages skeleton damage taking and death
/// </summary>
public class demonDamageScript : MonoBehaviour
{
    demonAnimationManager animationManager;
    
    [SerializeField] GameObject forceField;
    Material fieldMaterial;  
    [SerializeField] float fieldFadeTime;
    float originalFieldFresnalWidth;

    [SerializeField] Collider hitBox;
    [SerializeField] int maxHealth;
    int currentHealth;
    [SerializeField] int damagePerHit;

    [SerializeField] string playerBulletLayerName;
    public bool isDead {get; private set;}



    void Start(){
        fieldMaterial = forceField.GetComponent<Renderer>().material;
        originalFieldFresnalWidth = fieldMaterial.GetFloat("_FresnelWidth");
        currentHealth = maxHealth;
        animationManager = GetComponent<demonAnimationManager>();

        //Disabling damage upon instantiation
        hitBox.enabled = false;
    }

    public void enableDamage(){
        if (isDead) return;
        hitBox.enabled = true;
        forceField.GetComponent<Collider>().enabled = false;

        DOTween.To(() => fieldMaterial.GetFloat("_FresnelWidth"),
             x => fieldMaterial.SetFloat("_FresnelWidth", x), 
             0, 
             fieldFadeTime);

        animationManager.startDizziness();
    }

    public void disableDamage(){
        hitBox.enabled = false;
        forceField.GetComponent<Collider>().enabled = true;

        DOTween.To(() => fieldMaterial.GetFloat("_FresnelWidth"),
             x => fieldMaterial.SetFloat("_FresnelWidth", x), 
             originalFieldFresnalWidth, 
             fieldFadeTime);

        animationManager.endDizziness();
    }


    //Called when hit by a bullet
    void OnCollisionEnter(Collision col){
        if (col.gameObject.layer == LayerMask.NameToLayer(playerBulletLayerName)){
            if (!isDead) getHit();
        }
    }

    void getHit()
    {
        currentHealth = Mathf.Max(currentHealth - damagePerHit, 0);
        isDead = currentHealth == 0;
        if (isDead){
            animationManager.triggerDeathAnim();
            animationManager.endDizziness();
        } 
        else animationManager.triggerHitAnim();
    }
}
