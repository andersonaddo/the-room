using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class difficultyCurveHolder : MonoBehaviour
{
    //For the first room.
    [CurveColor(CurveColorAttribute.availableCurveColors.green)]
    public AnimationCurve absorberStrengthMultiplier, targetCubePriority, specialCubeChance, cubeGenerationRate;

    //For the escape corridor
    [CurveColor(CurveColorAttribute.availableCurveColors.cyan)]
    public AnimationCurve shooterCubeLaunchSpeed, corruptionDeltaZ, richochetCubeSpeed;

    public static difficultyCurveHolder Instance;

    [SerializeField] float timeToFullDifficulty;
    float startTime;

    enum difficultyModes
    {
        firstRoom,
        escapeCorridor,
        bossRoom
    }

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

    public void resetDifficulty()
    {
        startTime = Time.deltaTime;
    }
}
