using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class meteorDestroyer : MonoBehaviour
{
    
    public static event System.Action<metoerDestructionModes> meteorDestroyed;

    public enum metoerDestructionModes{
        successful,
        stoppedButUnsuccessful,
        completeFail
    }

    public void destroyMeteor(metoerDestructionModes mode)
    {
        FindObjectOfType<PlayerMegaBlastCoordinator>().reset();
        if (meteorDestroyed != null) meteorDestroyed(mode);
        print("Meteor destroyed, " + mode.ToString());
        Destroy(gameObject);
    }
}
