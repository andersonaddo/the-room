using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class timedSelfDestruct : MonoBehaviour {

    [SerializeField] float aliveTime;
    [SerializeField] bool startOnAwake = true;


    public void cancel()
    {
        CancelInvoke();
    }

   public void startTimer()
    {
        Invoke("selfDestruct", aliveTime);
    }

	void Start () {
        if (startOnAwake) startTimer();
	}
	
	void selfDestruct () {
        ISelfDestructInstructions instructions = GetComponent<ISelfDestructInstructions>();
        if (instructions != null) instructions.selfDestruct();
        else Destroy(gameObject);
	}
}
