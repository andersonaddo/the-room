using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IFreezable {


    void freeze();
    void unfreeze(bool destroyIfPossible, damageEffectors effector);
    bool isFrozen { get; set; }
}
