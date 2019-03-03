using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(roomVisualsMaterialChanger))]
public class roomVisualsMaterialChangerEditor : PropertyDrawer
{
    float numberOfLines = 9;

    SerializedProperty material, deafultEmissColour, testEmissColor, isExpanded, autoUpdate;


    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        //Initializing the serialized properties...
        material = property.FindPropertyRelative("mat");

        deafultEmissColour = property.FindPropertyRelative("defaultMainEmissMatColor");
        testEmissColor = property.FindPropertyRelative("testMainEmissMatColor");

        isExpanded = property.FindPropertyRelative("isExpanded");
        autoUpdate = property.FindPropertyRelative("autoUpdate");


        //Drawing the main label...
        label = EditorGUI.BeginProperty(position, label, property); //Allows us to have functionality like duplication within the array
        label.text = (material.objectReferenceValue == null) ? "Empty Changer" : ((Material)material.objectReferenceValue).name + " Changer";
        EditorGUI.PrefixLabel(position, label);

        Rect currentPosition = position;
        currentPosition = EditorGUI.IndentedRect(currentPosition); //Applying the new indent to the position;
        currentPosition.height = 16; //Limiting the height for each property to 16 pixels
        currentPosition.y += 18; //Going to the next Line...

        isExpanded.boolValue = EditorGUI.Foldout(currentPosition, isExpanded.boolValue, "Expand");
        currentPosition.y += 18;

        if (isExpanded.boolValue)
        {
            EditorGUI.PropertyField(currentPosition, material);
            currentPosition.y += 18;
            EditorGUI.PropertyField(currentPosition, deafultEmissColour);
            currentPosition.y += 18;


            EditorGUI.BeginChangeCheck();
            EditorGUI.PropertyField(currentPosition, testEmissColor);
            currentPosition.y += 18;
            EditorGUI.EndChangeCheck();

            EditorGUI.PropertyField(currentPosition, autoUpdate);
            currentPosition.y += 18;

            roomVisualsMaterialChanger changer = repackage();
            if (GUI.changed && autoUpdate.boolValue) Object.FindObjectOfType<roomVisualsHelper>().testMaterialEmission(changer);

            //The next two buttons will be on the same line, so I have to split currentPosition in 2
            float originalX = currentPosition.x;
            currentPosition.width /= 2;

            if (GUI.Button(currentPosition, "Test onto Defualt"))
            {
                deafultEmissColour.colorValue = testEmissColor.colorValue;
            }

            currentPosition.x = currentPosition.xMax;

            if (GUI.Button(currentPosition, "Default onto Test"))
            {
                testEmissColor.colorValue = deafultEmissColour.colorValue;
            }

            currentPosition.x = originalX;
            currentPosition.y += 18;
  
            //Don't worry if material in changer is null, that is checked in roomVisualsHelper
            if (GUI.Button(currentPosition, "Apply Test")) Object.FindObjectOfType<roomVisualsHelper>().testMaterialEmission(changer);
            currentPosition.x = currentPosition.xMax;
            if (GUI.Button(currentPosition, "Apply Default")) Object.FindObjectOfType<roomVisualsHelper>().resetToEmissionDefault(changer);

            //Resoring currentPosition to it's normal size and position (incase I wanna add anything here later)
            currentPosition.x = originalX;
            currentPosition.width *= 2;
        }
        EditorGUI.EndProperty();
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        isExpanded = property.FindPropertyRelative("isExpanded");

        if (!isExpanded.boolValue)
        {
            return 52f;
        }
        else
        {
            return 16f + (numberOfLines - 1) * 18f;
        }
    }

    public roomVisualsMaterialChanger repackage()
    {
        roomVisualsMaterialChanger changer = new roomVisualsMaterialChanger();
        changer.mat = (Material)material.objectReferenceValue ?? null;
        changer.testMainEmissMatColor = testEmissColor.colorValue;
        changer.defaultMainEmissMatColor = deafultEmissColour.colorValue;
        return changer;
    }
}
