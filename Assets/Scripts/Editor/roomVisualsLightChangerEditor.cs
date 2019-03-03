using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(roomVisualsLightsChanger))]
public class roomVisualsLightChangerEditor : PropertyDrawer
{
    float numberOfLines = 12;

    SerializedProperty defaultRange, testRange,
        defaultColor, testColor,
        defaultIntensity, testIntensity, classification, isExpanded, autoUpdate;


    public override void OnGUI (Rect position, SerializedProperty property, GUIContent label)
    {
        //Initializing the serialized properties...
        defaultColor = property.FindPropertyRelative("defaultRealTimeLightColor");
        testColor = property.FindPropertyRelative("testRealTimeLightColor");

        defaultRange = property.FindPropertyRelative("defaultRange");
        testRange = property.FindPropertyRelative("testRange");

        defaultIntensity = property.FindPropertyRelative("defaultIntensity");
        testIntensity = property.FindPropertyRelative("testIntensity");

        classification = property.FindPropertyRelative("targetClassification");
        isExpanded = property.FindPropertyRelative("isExpanded");
        autoUpdate = property.FindPropertyRelative("autoUpdate");


        //Drawing the main label...
        label = EditorGUI.BeginProperty(position, label, property); //Allows us to have functionality like duplication within the array
        label.text = ((LightClass.lightClassifications)classification.enumValueIndex).ToString("G");
        EditorGUI.PrefixLabel(position, label);

        Rect currentPosition = position;
        currentPosition = EditorGUI.IndentedRect(currentPosition); //Applying the new indent to the position;
        currentPosition.height = 16; //Limiting the height for each property to 16 pixels
        currentPosition.y += 18; //Going to the next Line...

        isExpanded.boolValue = EditorGUI.Foldout(currentPosition, isExpanded.boolValue, "Expand");
        currentPosition.y += 18;

        if (isExpanded.boolValue)
        {
            EditorGUI.PropertyField(currentPosition, classification);
            currentPosition.y += 18;
            EditorGUI.PropertyField(currentPosition, defaultColor);
            currentPosition.y += 18;
            EditorGUI.PropertyField(currentPosition, defaultIntensity);
            currentPosition.y += 18;
            EditorGUI.PropertyField(currentPosition, defaultRange);
            currentPosition.y += 18;


            EditorGUI.BeginChangeCheck();
            EditorGUI.PropertyField(currentPosition, testColor);
            currentPosition.y += 18;
            EditorGUI.PropertyField(currentPosition, testIntensity);
            currentPosition.y += 18;
            EditorGUI.PropertyField(currentPosition, testRange);
            currentPosition.y += 18;
            EditorGUI.EndChangeCheck();

            EditorGUI.PropertyField(currentPosition, autoUpdate);
            currentPosition.y += 18;

            roomVisualsLightsChanger changer = repackage();
            if (GUI.changed && autoUpdate.boolValue) Object.FindObjectOfType<roomVisualsHelper>().testRealtimeLights(changer);

            //The next two buttons will be on the same line, so I have to split currentPosition in 2
            float originalX = currentPosition.x;
            currentPosition.width /= 2;

            if (GUI.Button(currentPosition, "Test onto Defualt"))
            {
                defaultColor.colorValue = testColor.colorValue;
                defaultIntensity.floatValue = testIntensity.floatValue;
                defaultRange.floatValue = testRange.floatValue;
            }

            currentPosition.x = currentPosition.xMax;

            if (GUI.Button(currentPosition, "Default onto Test"))
            {
                testColor.colorValue = defaultColor.colorValue;
                testIntensity.floatValue = defaultIntensity.floatValue;
                testRange.floatValue = defaultRange.floatValue;
            }

            currentPosition.x = originalX;
            currentPosition.y += 18;
            if (GUI.Button(currentPosition, "Apply Test")) Object.FindObjectOfType<roomVisualsHelper>().testRealtimeLights(changer);
            currentPosition.x = currentPosition.xMax;
            if (GUI.Button(currentPosition, "Apply Default")) Object.FindObjectOfType<roomVisualsHelper>().resetRealtimeLights(changer);
           
            //Resoring currentPosition to it's normal size and position (incase I wanna add anything here later)
            currentPosition.x = originalX;
            currentPosition.width *= 2;
        }
        EditorGUI.EndProperty();
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        isExpanded = property.FindPropertyRelative("isExpanded");

        if (!isExpanded.boolValue) {
            return 52f;
        }
        else
        {
            return 16f + (numberOfLines - 1) * 18f;
        }
    }

    public roomVisualsLightsChanger repackage()
    {
        roomVisualsLightsChanger changer = new roomVisualsLightsChanger();
        changer.defaultIntensity = defaultIntensity.floatValue;
        changer.defaultRange = defaultRange.floatValue;
        changer.defaultRealTimeLightColor = defaultColor.colorValue;
        changer.testIntensity = testIntensity.floatValue;
        changer.testRange = testRange.floatValue;
        changer.testRealTimeLightColor = testColor.colorValue;

        changer.targetClassification = (LightClass.lightClassifications) classification.enumValueIndex;
        return changer;


    }
}