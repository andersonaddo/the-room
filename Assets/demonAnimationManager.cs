using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This simple class should be the  only class that interacts directly with the skeleton's animator component
/// </summary>
[RequireComponent(typeof(Animator))]
public class demonAnimationManager : MonoBehaviour
{

    Animator animator;
    PlayerMegaBlastCoordinator playerBlastScript;

    void Start()
    {
        animator = GetComponent<Animator>();
        animator.SetTrigger("yell");
        playerBlastScript = FindObjectOfType<PlayerMegaBlastCoordinator>();
    }

    void Update(){
        //Cause the skeleton to show pain if the player is starting to damage a metoer
        if (playerBlastScript.canShoot 
            && playerBlastScript.raycastOnMeteor 
            && playerBlastScript.isShooting
            && playerBlastScript.isOnDelayMode){
                animator.SetBool("isSuffering", true);
            }else{
                animator.SetBool("isSuffering", false);
            }
    }

    public void meteorBeginningAnim(){
        animator.SetTrigger("callMeteors");
    }

    public void meteorEndAnim(){
        animator.SetTrigger("dismissMeteors");
    }

    public void startDizziness(){
        animator.SetBool("isDizzy", true);
    }

    public void endDizziness(){
        animator.SetBool("isDizzy", false);
    }

    public void triggerHitAnim(){
        animator.SetTrigger("triggerDamage");
    }

    public void triggerDeathAnim(){
        animator.SetTrigger("die");
        animator.SetBool("isDead", true);
    }
}
