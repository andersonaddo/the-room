using UnityEditor;
using UnityEngine;
using System.Linq;

[CustomEditor(typeof(roomVisualsHelper))]
public class roomVisualsHelperEditor : Editor
{

    SerializedProperty materialChangersList, lightChangersList;

    void OnEnable()
    {
        materialChangersList = serializedObject.FindProperty("materialChangers");
        lightChangersList = serializedObject.FindProperty("changers");

    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update(); //set serialized object based on what is in the actual class (in case I've changed things from the last frame)
        roomVisualsHelper helper = (roomVisualsHelper)target;

        //Showing original script opener
        GUI.enabled = false;
        EditorGUILayout.ObjectField("Script:", MonoScript.FromMonoBehaviour((roomVisualsHelper)target), typeof(roomVisualsHelper), false);
        GUI.enabled = true;

        //This section is for editing main materials...
        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
        EditorGUILayout.LabelField("Emissive Material Editing", EditorStyles.boldLabel);
        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
        EditorGUILayout.PropertyField(materialChangersList, true);


        if (GUILayout.Button("Reset All Material Defualts"))
        {
            foreach (roomVisualsMaterialChanger matChanger in helper.materialChangers)
                helper.resetToEmissionDefault(matChanger);
        }


        //This section is for editing realtime lights...
        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
        EditorGUILayout.LabelField("Realtime lights Editing", EditorStyles.boldLabel);
        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
        EditorGUILayout.PropertyField(lightChangersList, true);

        if (GUILayout.Button("Reset All Light Defualts"))
        {
            foreach (roomVisualsLightsChanger lightChanger in helper.changers)
                helper.resetRealtimeLights(lightChanger);
        }


        //This section is for editing realtime lights...
        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
        EditorGUILayout.LabelField("Theme Saving and Loading", EditorStyles.boldLabel);
        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

        if (GUILayout.Button("Save Theme"))
        {
            EditorUtility.DisplayDialog("Saving New Room Visual",
                                        "Go through all your changers. Only the ones that are expanded will be saved as configurations in this saved holder",
                                        "Go Ahead",
                                        "Cancel");

            //Credit to http://wiki.unity3d.com/index.php/CreateScriptableObjectAsset for the below code
            roomVisualsHolder newHolder =  ScriptableObject.CreateInstance<roomVisualsHolder>();

            foreach (roomVisualsLightsChanger lightChanger in helper.changers.Where ((lightChanger) => lightChanger.isExpanded))
            {
                roomLightConfiguration lightConfig = new roomLightConfiguration();
                lightConfig.targetClassification = lightChanger.targetClassification;
                lightConfig.realTimeLightColor = lightChanger.testRealTimeLightColor;
                lightConfig.range = lightChanger.testRange;
                lightConfig.intensity = lightChanger.testIntensity;
                newHolder.lightConfigurations.Add(lightConfig);
            }

            
            foreach (roomVisualsMaterialChanger matChanger in helper.materialChangers.Where ((mChanger) => mChanger.isExpanded))
            {
                roomMaterialConfiguration matConfig = new roomMaterialConfiguration();
                matConfig.mat = matChanger.mat;
                matConfig.emissMatColor = matChanger.testMainEmissMatColor;
                newHolder.materialConfigurations.Add(matConfig);
            }


            string path = "Assets";
            string assetPathAndName = AssetDatabase.GenerateUniqueAssetPath(path + "/New " + typeof(roomVisualsHolder).ToString() + ".asset");
            AssetDatabase.CreateAsset(newHolder, assetPathAndName);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            EditorUtility.FocusProjectWindow();
            Selection.activeObject = newHolder;
        }


        if (GUILayout.Button("Load Theme"))
        {
            EditorUtility.OpenFilePanel("Select a theme", "Assets", "asset");
        }

        serializedObject.ApplyModifiedProperties(); //take what is in the object stream and put it in the class (so the the class itself is updated with the changes made here
        Repaint();
    }
}