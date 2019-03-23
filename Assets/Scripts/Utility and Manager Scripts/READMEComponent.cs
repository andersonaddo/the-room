using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class READMEComponent : MonoBehaviour {

    [Header("README Comments")]
    [TextArea(3, 6)]
    [Tooltip("Doesn't do anything. Just comments shown in inspector :3")]
    [SerializeField] string Notes = "Meh is a powerful word.";
}
