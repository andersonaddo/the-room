// CurveDrawer.cs
// Created by Alexander Ameye
// Version 1.1.0
//https://forum.unity.com/threads/changing-how-animation-curve-window-looks.488841/

using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(CurveColorAttribute))]
public class CurveDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        CurveColorAttribute cca = attribute as CurveColorAttribute;
        if (property.propertyType == SerializedPropertyType.AnimationCurve)
        {
            EditorGUI.CurveField(position, property, calculateColor(cca.curveColor), new Rect());
        }
    }

    Color calculateColor(CurveColorAttribute.availableCurveColors color)
    {
        switch (color)
        {
            case CurveColorAttribute.availableCurveColors.blue:
                return Color.blue;
            case CurveColorAttribute.availableCurveColors.green:
                return Color.green;
            case CurveColorAttribute.availableCurveColors.red:
                return Color.red;
            case CurveColorAttribute.availableCurveColors.cyan:
                return Color.cyan;
            case CurveColorAttribute.availableCurveColors.majenta:
                return Color.magenta;
            default:
                return Color.yellow;
        }
    }
}