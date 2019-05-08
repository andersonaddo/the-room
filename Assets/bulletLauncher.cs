using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bulletLauncher : MonoBehaviour
{

    [SerializeField] GameObject bullet, releasePS;
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
            fireBullet();
            yield return new WaitForSeconds(firingRate);
        }
    }

    private void fireBullet()
    {
        pathGenerator gen = pathGens[Random.Range(0, pathGens.Count)];
        Instantiate(releasePS, gen.transform.position, Quaternion.identity);
        GameObject madeBullet = Instantiate(bullet, gen.transform.position, Quaternion.identity);
        madeBullet.GetComponent<pathFollower>().setPath(gen.bezierPath);
    }
}
