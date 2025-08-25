using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(WaterGenerator))]
public class WaterGeneratorEditor : Editor
{
    public override void OnInspectorGUI() 
    {
        serializedObject.Update();

        if (DrawDefaultInspector())
        {
            foreach (WaterGenerator gen in targets)
            {
                gen.GenerateWater();
            }
        }

        serializedObject.ApplyModifiedProperties();

        if (GUILayout.Button("Update"))
        {
            foreach (WaterGenerator gen in targets)
            {
                gen.GenerateWater();
            }
        }
    }
}
