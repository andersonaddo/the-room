using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class meteorPlayerDamager : MonoBehaviour
{

    [SerializeField] float shakeMag, shakeRough;
    [SerializeField] int damage;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine("damagePlayerUponHit");
    }

    IEnumerator damagePlayerUponHit (){
        yield return new WaitUntil(() => GetComponent<pathFollower>().currentLerp >= 1);
        FindObjectOfType<playerDamager>().inflictDamage(damage, new Vector2(shakeMag, shakeRough));
    }
}
