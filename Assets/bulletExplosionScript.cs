using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bulletExplosionScript : MonoBehaviour, ISelfDestructInstructions
{
    public void selfDestruct()
    {
        resetForPooling();
    }

    public void explode()
    {
        GetComponent<ParticleSystem>().time = 0;
        GetComponent<ParticleSystem>().Play();
        GetComponent<timedSelfDestruct>().startTimer();
    }

    void resetForPooling()
    {
        //Timed self destruct doesn't have to be cancelled, it is the only thing that can destory the explosion...
        objectPooler.Instance.returnObject("bulletExplosion", gameObject);
    }
}
