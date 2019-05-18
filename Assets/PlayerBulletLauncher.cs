using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBulletLauncher : MonoBehaviour
{
    public GameObject bullet;
    public bool canShoot = true;

    // Start is called before the first frame update
    void Awake()
    {
        PlayerCardboardPointer.pointerClickDown += shootBullet;
    }

    // Update is called once per frame
    void shootBullet()
    {
        if (!canShoot) return;
        GameObject newBullet = objectPooler.Instance.requestObject("bullet");
        newBullet.transform.position = PlayerCardboardPointer.position;
        newBullet.GetComponent<playerBullet>().launch(PlayerCardboardPointer.forwardDirection);
    }
}
