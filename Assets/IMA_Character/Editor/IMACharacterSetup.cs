// CMS26_IMA_Character · ships inside the package (Assets/IMA_Character/Editor).
// A unitypackage can't carry project settings, so this does three things when the project loads
// after the import:
//  1. adds the two tags the scenes use (Hazard, Checkpoint);
//  2. appends Playground, Title and the station scenes to the end of File → Build Profiles →
//     Scene List (the title screen loads the Playground by name). Your own scenes stay where they are;
//     untick a scene to leave it out of your build. Character_Starter is a template and stays
//     out of the list;
//  3. switches the sample materials of Synty's ANIMATION - Base Locomotion from the built-in
//     Standard shader to the Universal Render Pipeline's Lit shader. They come that way from
//     the store, and in this project's render pipeline they would show pink.
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

[InitializeOnLoad]
public static class IMACharacterSetup
{
    static readonly string[] Tags = { "Hazard", "Checkpoint" };
    const string Root = "Assets/IMA_Character";
    const string SyntySamples = "Assets/Synty/AnimationBaseLocomotion/Samples/Materials";

    static IMACharacterSetup()
    {
        // Right away (also in batch mode, where delayCall may never fire) and once more on the
        // next editor tick, in case the editor wasn't ready during the domain reload.
        try { AddTags(); } catch (System.Exception e) { Debug.LogWarning("IMA_Character: tags later (" + e.Message + ")"); }
        try { AddScenes(); } catch (System.Exception e) { Debug.LogWarning("IMA_Character: scene list later (" + e.Message + ")"); }
        try { ConvertSyntyMaterials(); } catch (System.Exception e) { Debug.LogWarning("IMA_Character: Synty materials later (" + e.Message + ")"); }
        EditorApplication.delayCall += AddTags;
        EditorApplication.delayCall += AddScenes;
        EditorApplication.delayCall += ConvertSyntyMaterials;
    }

    public static void AddTags()
    {
        var existing = InternalEditorUtility.tags;
        var added = false;
        foreach (var tag in Tags)
            if (!existing.Contains(tag))
            {
                InternalEditorUtility.AddTag(tag);
                Debug.Log("IMA_Character: added tag " + tag);
                added = true;
            }
        if (added) AssetDatabase.SaveAssets();
    }

    /// Makes sure the package's scenes are in the Scene List (appended at the end). A scene that
    /// is in the list but unticked counts as there.
    public static void AddScenes()
    {
        // File checks, not the AssetDatabase: it may not answer yet while the editor loads.
        var wanted = new[] { Root + "/Playground.unity", Root + "/Title.unity" }.Where(File.Exists).ToList();
        if (Directory.Exists(Root + "/Stations"))
            wanted.AddRange(Directory.GetFiles(Root + "/Stations", "*.unity")
                .Select(p => p.Replace('\\', '/'))
                .OrderBy(p => p, System.StringComparer.Ordinal));
        var list = EditorBuildSettings.scenes.ToList();
        var added = 0;
        foreach (var path in wanted)
        {
            if (list.Any(s => s.path == path)) continue;
            list.Add(new EditorBuildSettingsScene(path, true));
            added++;
        }
        if (added == 0) return;
        EditorBuildSettings.scenes = list.ToArray();
        AssetDatabase.SaveAssets();
        Debug.Log("IMA_Character: added " + added + " scene(s) to the Scene List");
    }

    /// Synty's sample materials with the Standard shader get the URP Lit shader, with the same
    /// texture and colour. Does nothing when they are converted already or the pack is missing.
    public static void ConvertSyntyMaterials()
    {
        if (!Directory.Exists(SyntySamples)) return;
        var lit = Shader.Find("Universal Render Pipeline/Lit");
        if (lit == null) return;
        var changed = 0;
        foreach (var file in Directory.GetFiles(SyntySamples, "*.mat"))
        {
            var m = AssetDatabase.LoadAssetAtPath<Material>(file.Replace('\\', '/'));
            if (m == null || m.shader == null || m.shader.name != "Standard") continue;
            var texture = m.HasProperty("_MainTex") ? m.GetTexture("_MainTex") : null;
            var colour = m.HasProperty("_Color") ? m.GetColor("_Color") : Color.white;
            var smoothness = m.HasProperty("_Glossiness") ? m.GetFloat("_Glossiness") : 0.5f;
            var metallic = m.HasProperty("_Metallic") ? m.GetFloat("_Metallic") : 0f;
            var seeThrough = m.HasProperty("_Mode") && m.GetFloat("_Mode") >= 2f;
            m.shader = lit;
            m.SetTexture("_BaseMap", texture);
            m.SetColor("_BaseColor", colour);
            m.SetFloat("_Smoothness", smoothness);
            m.SetFloat("_Metallic", metallic);
            if (seeThrough)
            {
                m.SetFloat("_Surface", 1f);
                m.SetFloat("_SrcBlend", (float)UnityEngine.Rendering.BlendMode.SrcAlpha);
                m.SetFloat("_DstBlend", (float)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                m.SetFloat("_ZWrite", 0f);
                m.EnableKeyword("_SURFACE_TYPE_TRANSPARENT");
                m.SetOverrideTag("RenderType", "Transparent");
                m.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Transparent;
            }
            EditorUtility.SetDirty(m);
            changed++;
        }
        if (changed == 0) return;
        AssetDatabase.SaveAssets();
        Debug.Log("IMA_Character: switched " + changed + " Synty sample material(s) to the URP Lit shader");
    }
}
