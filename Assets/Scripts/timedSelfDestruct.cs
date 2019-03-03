using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class timedSelfDestruct : MonoBehaviour {

    [SerializeField] float aliveTime;

    public void cancel()
    {
        CancelInvoke();
    }

	void Start () {
        Invoke("selfDestruct", aliveTime);
	}
	
	void selfDestruct () {
        ISelfDestructInstructions instructions = GetComponent<ISelfDestructInstructions>();
        if (instructions != null) instructions.selfDestruct();
        else Destroy(gameObject);
	}
}
