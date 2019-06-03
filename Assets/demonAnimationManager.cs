using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    public void meteorBeginningAnim(){
        animator.SetTrigger("callMeteors");
    }

    public void meteorEndAnim(){
        animator.SetTrigger("dismissMeteors");
    }

    void Update(){
        if (playerBlastScript.canShoot 
            && playerBlastScript.raycastOnMeteor 
            && playerBlastScript.isShooting
            && playerBlastScript.isOnDelayMode){
                animator.SetBool("isSuffering", true);
            }else{
                animator.SetBool("isSuffering", false);
            }
    }
}
