using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class gameManager : MonoBehaviour {

    public static gameManager Instance;

    public cubeTypes currentTargetType { get; private set; }
    int score;
    public event Action<int> scoreChanged;

    public float firstRoomStarterTime;
    float firstRoomMaxTime; //This could potentially be changed by power ups for something later on
    float firstRoomTimeLeft;

    public float firstRoomThemeChangeTime;
    float nextThemeChangeTime;
    bool canChangeTheme = true;

    public float getTimeLeftPercentage
    {
        get
        {
            return firstRoomTimeLeft / firstRoomMaxTime;
        }
    }


    void Awake () {
        Instance = this;
        firstRoomMaxTime = firstRoomStarterTime;
        firstRoomTimeLeft = firstRoomStarterTime;
        nextThemeChangeTime = Time.time + firstRoomThemeChangeTime;
	}
	
	void Update () {
        if (firstRoomTimeLeft != 0) firstRoomTimeLeft -= Time.deltaTime;
        if (firstRoomTimeLeft < 0) firstRoomTimeLeft = 0;

        if (Time.time > nextThemeChangeTime)
        {
            FindObjectOfType<roomVisualsChanger>().chooseNextTheme();
            nextThemeChangeTime = Time.time + firstRoomThemeChangeTime;
        }
	}

    public void incrementScore(int increment)
    {
        score += increment;
        if (scoreChanged != null) scoreChanged(score);
    }

    public void updateTargetType (cubeTypes type)
    {
        currentTargetType = type;
    }
}
