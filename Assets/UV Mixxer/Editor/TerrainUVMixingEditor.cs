using System;
using System.Linq;
using UnityEditor;
using UnityEngine;

[Serializable]
public class TerrainUVMixingEditor : MaterialEditor
{
    #region Private Fields

    private Texture _Logo;

    #endregion

    #region Public Methods

    public override void Awake()
    {
        base.Awake();
        _Logo = Resources.Load("logo_icon") as Texture;
    }

    public override void OnInspectorGUI()
    {
        if (!isVisible)
            return;

        GUI.skin.label.richText = true;

        var targetMaterial = target as Material;
        var keywords = targetMaterial.shaderKeywords;

        EditorGUI.BeginChangeCheck();

        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        GUILayout.Label(_Logo, GUILayout.Width(186), GUILayout.Height(89));
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();

        GUILayout.Label("<b>BASE PARAMETERS</b>");

        var baseModifier = EditorGUILayout.Slider("Mixing coefficient", targetMaterial.GetFloat("_BaseModifier"), 0, 10);
        var baseUVCoordinate = EditorGUILayout.Slider("Base UV multiplier", targetMaterial.GetFloat("_BaseUVCoordinate"), -10, 10);

        GUILayout.Space(20);
        var mixingStartRange = EditorGUILayout.Slider("Mixing start distance", targetMaterial.GetFloat("_MixingStartRange"), 0, 1000);
        var mixingTransitionLength = EditorGUILayout.Slider("Mixing transition length", targetMaterial.GetFloat("_MixingTransitionLength"), 0, 500);

        GUILayout.Space(20);
        GUILayout.Label("<b>NOISE</b>");

        var octaves = targetMaterial.GetFloat("_Octaves");
        var frequency = targetMaterial.GetFloat("_Frequency");
        var amplitude = targetMaterial.GetFloat("_Amplitude");
        var lacunarity = targetMaterial.GetFloat("_Lacunarity");
        var persistence = targetMaterial.GetFloat("_Persistence");
        var offset = targetMaterial.GetVector("_Offset");
        var noiseValueOffset = targetMaterial.GetFloat("_NoiseValueOffset");
        var noiseCutoffDistance = targetMaterial.GetFloat("_NoiseCutoffDistance");

        var useNoise = keywords.Contains("MIXXER_USE_NOISE_ON");
        useNoise = GUILayout.Toggle(useNoise, "Use mixing noise");
        if (useNoise)
        {
            octaves = (int)EditorGUILayout.Slider("Octaves", (int)octaves, 0, 10);
            frequency = EditorGUILayout.Slider("Frequency", frequency, 0, 10);
            amplitude = EditorGUILayout.Slider("Amplitude", amplitude, 0, 10);
            lacunarity = EditorGUILayout.Slider("Lacunarity", lacunarity, 0, 10);
            persistence = EditorGUILayout.Slider("Persistence", persistence, 0, 10);
            offset = EditorGUILayout.Vector2Field("Offset", offset);
            GUILayout.Space(20);
            noiseValueOffset = EditorGUILayout.Slider("Noise value offset", noiseValueOffset, -1, 1);
            noiseCutoffDistance = EditorGUILayout.Slider("Noise cutoff distance", noiseCutoffDistance, 0, 100000);
        }

        GUILayout.Space(20);
        GUILayout.Label("<b>COLOR CORRECTION</b>");

        var brightness = targetMaterial.GetFloat("_Brightness");
        var contrast = targetMaterial.GetFloat("_Contrast");
        var saturation = targetMaterial.GetFloat("_Saturation");

        var useColorCorrection = keywords.Contains("MIXXER_USE_COLOR_CORRECTION_ON");
        useColorCorrection = GUILayout.Toggle(useColorCorrection, "Use color correction");
        if (useColorCorrection)
        {
            brightness = EditorGUILayout.Slider("Brightness", brightness, -1, 1);
            contrast = EditorGUILayout.Slider("Contrast", contrast, 0, 2);
            saturation = EditorGUILayout.Slider("Saturation", saturation, 0, 2);
        }

        GUILayout.Space(20);
        GUILayout.Label("<b>TRIPLANAR MAPPING</b>");

        var worldScale = targetMaterial.GetVector("_WorldScale");
        var worldTranslation = targetMaterial.GetVector("_WorldTranslation");

        var useTriplanarMapping = keywords.Contains("MIXXER_USE_TRIPLANAR_ON");
        useTriplanarMapping = GUILayout.Toggle(useTriplanarMapping, "Use triplanar mapping");
        if (useTriplanarMapping)
        {
            worldScale = EditorGUILayout.Vector3Field("World Scale", worldScale);
            worldTranslation = EditorGUILayout.Vector3Field("World Translation", worldTranslation);
        }

        if (EditorGUI.EndChangeCheck())
        {
            var newKeywords = new string[]
            {
                useNoise ? "MIXXER_USE_NOISE_ON" : "MIXXER_USE_NOISE_OFF",
                useColorCorrection ? "MIXXER_USE_COLOR_CORRECTION_ON" : "MIXXER_USE_COLOR_CORRECTION_OFF",
                useTriplanarMapping ? "MIXXER_USE_TRIPLANAR_ON" : "MIXXER_USE_TRIPLANAR_OFF"
            };
            targetMaterial.shaderKeywords = newKeywords;

            targetMaterial.SetFloat("_BaseModifier", baseModifier);

            targetMaterial.SetFloat("_MixingStartRange", mixingStartRange);
            targetMaterial.SetFloat("_MixingTransitionLength", mixingTransitionLength);

            targetMaterial.SetFloat("_Octaves", octaves);
            targetMaterial.SetFloat("_Frequency", frequency);
            targetMaterial.SetFloat("_Amplitude", amplitude);
            targetMaterial.SetFloat("_Lacunarity", lacunarity);
            targetMaterial.SetFloat("_Persistence", persistence);
            targetMaterial.SetVector("_Offset", offset);
            targetMaterial.SetFloat("_NoiseValueOffset", noiseValueOffset);
            targetMaterial.SetFloat("_NoiseCutoffDistance", noiseCutoffDistance);

            targetMaterial.SetFloat("_Brightness", brightness);
            targetMaterial.SetFloat("_Contrast", contrast);
            targetMaterial.SetFloat("_Saturation", saturation);

            targetMaterial.SetVector("_WorldScale", worldScale);
            targetMaterial.SetVector("_WorldTranslation", worldTranslation);

            targetMaterial.SetFloat("_BaseUVCoordinate", baseUVCoordinate);

            EditorUtility.SetDirty(targetMaterial);
        }
    }

    #endregion
}