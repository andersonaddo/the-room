using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class gameManager : MonoBehaviour {

    public static gameManager Instance;
    transitioner gameStageTransitioner;

    public cubeTypes currentTargetType { get; private set; }
    int score;
    public event Action<int> scoreChanged;

    [SerializeField]
    float maxSummoningProgression;
    float currentProgression;

    public float firstRoomThemeChangeTime;
    float nextThemeChangeTime;
    bool canChangeTheme = true;

    [SerializeField] float progAdditionUponAbsorption = 5;

    public List<Transform> absorbers = new List<Transform>();

    public float progressionPercentage
    {
        get
        {
            return currentProgression / maxSummoningProgression;
        }
    }


    void Awake () {
        Instance = this;
        nextThemeChangeTime = Time.time + firstRoomThemeChangeTime;
        gameStageTransitioner = FindObjectOfType<transitioner>();
        roomVisualsChanger.themeChanged += updateTargetType;
    }
	
	void Update () {

        if (gameStageTransitioner.currentStage == transitioner.gameStage.firstRoom
            && Time.time > nextThemeChangeTime)
        {
            FindObjectOfType<roomVisualsChanger>().goToNextTheme();
            nextThemeChangeTime = Time.time + firstRoomThemeChangeTime;
        }
	}

    public void incrementScore(int increment)
    {
        score += increment;
        if (scoreChanged != null) scoreChanged(score);
    }

    public void updateTargetType (roomVisualsHolder holder)
    {
        currentTargetType = holder.targetType;
    }

    public void signalCubeAbsorption()
    {
        currentProgression += progAdditionUponAbsorption;
        if (currentProgression > maxSummoningProgression) currentProgression = maxSummoningProgression;
    }
}
