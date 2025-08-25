using System;
using UnityEditor;
using UnityEngine;
using Random = System.Random;

[CustomEditor(typeof(MapGenerator))]
public class MapGeneratorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUI.BeginChangeCheck();

        EditorGUILayout.LabelField("Size", EditorStyles.boldLabel);
        SerializedProperty mapSizeProp = serializedObject.FindProperty("mapSize");
        mapSizeProp.intValue = EditorGUILayout.IntSlider("Map size:", mapSizeProp.intValue, 100, 1000);
        EditorGUILayout.Space();

        EditorGUILayout.LabelField("General settings", EditorStyles.boldLabel);
        SerializedProperty autoUpdateProp = serializedObject.FindProperty("autoUpdate");
        autoUpdateProp.boolValue = GUILayout.Toggle(autoUpdateProp.boolValue, "Auto update");
        SerializedProperty generateWaterProp = serializedObject.FindProperty("generateWater");
        generateWaterProp.boolValue = GUILayout.Toggle(generateWaterProp.boolValue, "Generate water");

        if (generateWaterProp.boolValue)
        {
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Water settings", EditorStyles.boldLabel);
            SerializedProperty waterGeneratorProp = serializedObject.FindProperty("waterGenerator");
            EditorGUILayout.PropertyField(waterGeneratorProp, new GUIContent("Water generator"));
            SerializedProperty waterLevelProp = serializedObject.FindProperty("waterLevel");
            waterLevelProp.floatValue = EditorGUILayout.Slider("Water level:", waterLevelProp.floatValue, 0.01f, 5f);
            EditorGUILayout.Space();
        }

        SerializedProperty islandProp = serializedObject.FindProperty("island");
        islandProp.boolValue = GUILayout.Toggle(islandProp.boolValue, "Generate island:");

        if (islandProp.boolValue)
        {
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Island settings", EditorStyles.boldLabel);
            SerializedProperty islandSizeProp = serializedObject.FindProperty("islandSizeMultiplier");
            islandSizeProp.floatValue = EditorGUILayout.Slider("Island size:", islandSizeProp.floatValue, .5f, 2f);
            SerializedProperty islandMinProp = serializedObject.FindProperty("islandMin");
            islandMinProp.floatValue = EditorGUILayout.Slider("Island fallout:", islandMinProp.floatValue, 0f, 1f);
        }

        EditorGUILayout.Space();

        EditorGUILayout.LabelField("Point sampling", EditorStyles.boldLabel);
        SerializedProperty distributionDataProp = serializedObject.FindProperty("distributionData");
        SerializedProperty distTypeProp = distributionDataProp.FindPropertyRelative("distributionType");
        distTypeProp.enumValueIndex = (int)(DistributionType)EditorGUILayout.EnumPopup("Vertex distribution type:",
            (DistributionType)distTypeProp.enumValueIndex);

        switch ((DistributionType)distTypeProp.enumValueIndex)
        {
            case DistributionType.Random:
                SerializedProperty pointDensityProp = distributionDataProp.FindPropertyRelative("pointDensity");
                pointDensityProp.intValue = EditorGUILayout.IntSlider("Point density:", pointDensityProp.intValue, 500, 1000);
                break;
            case DistributionType.Poisson:
                SerializedProperty radiusProp = distributionDataProp.FindPropertyRelative("radius");
                radiusProp.floatValue = EditorGUILayout.Slider("Radius between vertex:", radiusProp.floatValue, 7f, 30f);
                SerializedProperty rejectionProp = distributionDataProp.FindPropertyRelative("rejectionSamples");
                rejectionProp.intValue = EditorGUILayout.IntSlider("Rejection samples:", rejectionProp.intValue, 5, 50);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        EditorGUILayout.Space();

        EditorGUILayout.LabelField("Noise settings", EditorStyles.boldLabel);
        SerializedProperty noiseDataProp = serializedObject.FindProperty("noiseData");
        SerializedProperty meshHeightProp = noiseDataProp.FindPropertyRelative("meshHeightMultiplier");
        meshHeightProp.floatValue = EditorGUILayout.Slider("Mesh height:", meshHeightProp.floatValue, 0.0001f, 300f);
        SerializedProperty noiseScaleProp = noiseDataProp.FindPropertyRelative("noiseScale");
        noiseScaleProp.floatValue = EditorGUILayout.Slider("Noise scale:", noiseScaleProp.floatValue, 0.0001f, 10000f);
        SerializedProperty heightCurveProp = noiseDataProp.FindPropertyRelative("meshHeightCurve");
        EditorGUILayout.PropertyField(heightCurveProp, new GUIContent("Height curve"));
        SerializedProperty octavesProp = noiseDataProp.FindPropertyRelative("octaves");
        octavesProp.intValue = EditorGUILayout.IntSlider("Octaves:", octavesProp.intValue, 1, 5);
        SerializedProperty persistenceProp = noiseDataProp.FindPropertyRelative("persistenceType");
        persistenceProp.enumValueIndex = (int)(PersistenceType)EditorGUILayout.EnumPopup("Persistence type:",
            (PersistenceType)persistenceProp.enumValueIndex);
        SerializedProperty lacunarityProp = noiseDataProp.FindPropertyRelative("lacunarity");
        lacunarityProp.floatValue = EditorGUILayout.Slider("Lacunarity:", lacunarityProp.floatValue, 1f, 5f);
        SerializedProperty seedProp = noiseDataProp.FindPropertyRelative("seed");
        seedProp.intValue = EditorGUILayout.IntField("Seed:", seedProp.intValue);
        SerializedProperty offsetProp = noiseDataProp.FindPropertyRelative("offset");
        offsetProp.vector2Value = EditorGUILayout.Vector2Field("Offset:", offsetProp.vector2Value);
        EditorGUILayout.Space();

        bool changed = EditorGUI.EndChangeCheck();
        serializedObject.ApplyModifiedProperties();

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Generate random seed"))
        {
            foreach (MapGenerator generator in targets)
            {
                generator.GenerateRandomSeed();
            }
        }
        bool manualGenerate = GUILayout.Button("Generate");
        EditorGUILayout.EndHorizontal();

        if (changed)
        {
            foreach (MapGenerator generator in targets)
            {
                if (generator.autoUpdate)
                {
                    generator.GenerateMap();
                }
            }
        }

        if (manualGenerate)
        {
            foreach (MapGenerator generator in targets)
            {
                generator.GenerateMap();
            }
        }
    }
}
