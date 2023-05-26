using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(ProceduralGenerationManager))]
public class LandscapeGeneratorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        ProceduralGenerationManager perlinGen = (ProceduralGenerationManager)target;

        if (DrawDefaultInspector())
            if (perlinGen.IsAutoGenerate())
                perlinGen.GenerateMap();

        if (GUILayout.Button("Generate"))
            perlinGen.GenerateMap();
    }
}
