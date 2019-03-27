// CurveAttribute.cs
// Created by Alexander Ameye
// Version 1.1.0
//https://forum.unity.com/threads/changing-how-animation-curve-window-looks.488841/

using UnityEngine;

public class CurveColorAttribute : PropertyAttribute
{
    public availableCurveColors curveColor;

    public CurveColorAttribute(availableCurveColors color)
    {
        this.curveColor = color;
    }

    public enum availableCurveColors
    {
        red,
        blue,
        green,
        yellow,
        cyan,
        majenta,
    }
}