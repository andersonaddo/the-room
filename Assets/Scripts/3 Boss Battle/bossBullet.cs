using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bossBullet : MonoBehaviour
{
    [SerializeField] GameObject destructionPS;
    [SerializeField] string playerLayer, shieldTag;
    [SerializeField] Vector2 shakeVector;
    [SerializeField] int damage;

    void OnCollisionEnter(Collision col)
    {
        if (col.gameObject.layer != LayerMask.NameToLayer(playerLayer)) return;

        if (col.gameObject.tag != shieldTag)
        {
            col.gameObject.GetComponentInParent<playerDamager>().inflictDamage(damage, shakeVector);
            Destroy(gameObject);
        }
        else
        {
            Instantiate(destructionPS, transform.position, Quaternion.LookRotation(FindObjectOfType<Camera>().transform.position - transform.position));
            bulletStreakCounter.signalBulletDestruction(gameObject);
            Destroy(gameObject);
        }
    }
}
