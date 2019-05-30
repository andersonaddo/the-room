using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBulletLauncher : MonoBehaviour
{
    public GameObject bullet;
    [SerializeField] bool _canShoot = true; //To allow for it to stay encapsulated but still changeable in the inspector
    public bool canShoot{ get{return _canShoot;} }

    // Start is called before the first frame update
    void Awake()
    {
        PlayerCardboardPointer.pointerClickDown += shootBullet;
    }

    // Update is called once per frame
    void shootBullet()
    {
        if (!_canShoot) return;
        GameObject newBullet = objectPooler.Instance.requestObject("bullet");
        newBullet.transform.position = PlayerCardboardPointer.position;
        newBullet.GetComponent<playerBullet>().launch(PlayerCardboardPointer.forwardDirection);
    }

    public void enableShooting(){
        _canShoot = true;
    }

    public void disableShooting(){
        _canShoot = false;
    }
}
