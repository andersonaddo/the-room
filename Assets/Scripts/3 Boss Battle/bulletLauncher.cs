using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class bulletLauncher : MonoBehaviour
{
     [SerializeField] bool _canShoot = false; //To allow for it to stay encapsulated but still changeable in the inspector
    public bool canShoot{ get{return _canShoot;} }

    [SerializeField] GameObject bullet, releasePS;
    [SerializeField] string bulletLayer;
    [SerializeField] float firingRate;
    [SerializeField] List<pathGenerator> pathGens = new List<pathGenerator>();
    
    void Start()
    {
        StartCoroutine("startFiring");
    }


    IEnumerator startFiring()
    {
        while (true)
        {
            if (_canShoot) fireBullet();
            yield return new WaitForSeconds(firingRate);
        }
    }

    private void fireBullet()
    {
        pathGenerator gen = pathGens[Random.Range(0, pathGens.Count)];
        Instantiate(releasePS, gen.transform.position, Quaternion.identity);
        GameObject madeBullet = Instantiate(bullet, gen.transform.position, Quaternion.identity);
        madeBullet.GetComponent<pathFollower>().setPath(gen.bezierPath);
        bulletStreakCounter.signalBulletLaunch(madeBullet);
    }

    public void enableShooting(){
        _canShoot = true;
    }

    public void disableShooting(){
        _canShoot = false;
    }

    public void destroyAllBullets(){
        var bullets  = FindObjectsOfType<pathFollower>()
            .Where(follower => follower.gameObject.layer == LayerMask.NameToLayer(bulletLayer));
        
        foreach(pathFollower follower in bullets)
            Destroy(follower.gameObject);
    }
}
