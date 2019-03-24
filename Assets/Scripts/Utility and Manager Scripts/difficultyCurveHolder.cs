using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class difficultyCurveHolder : MonoBehaviour
{
    [CurveColor(CurveColorAttribute.availableCurveColors.green)]
    public AnimationCurve absorberStrengthMultiplier, targetCubePriority, specialCubeChance, cubeGenerationRate; //For the first room.

    [CurveColor(CurveColorAttribute.availableCurveColors.cyan)]
    public AnimationCurve shooterCubeLaunchSpeed; //For the escape corridor

    public static difficultyCurveHolder Instance;

    [SerializeField] float timeToFullDifficulty;
    float startTime;

    void Awake()
    {
        Instance = this;
        startTime = Time.time;
    }

    public static float currentDifficulty
    {
        get
        {
            return Mathf.Clamp01((Time.time - Instance.startTime) / Instance.timeToFullDifficulty);
        }
    }

    public static float getCurrentValue(AnimationCurve curve)
    {
        return curve.Evaluate(currentDifficulty);
    }
}
