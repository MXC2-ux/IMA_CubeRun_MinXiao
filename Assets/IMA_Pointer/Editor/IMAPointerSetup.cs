// CMS26_IMA_Pointer · ships inside the package (Assets/IMA_Pointer/Editor).
// A unitypackage can't carry project settings, so this does two things when the project loads
// after the import (and a third, tags, as soon as an example needs one). Each of them does
// nothing once its work is done, so it may run on every load:
//  1. names User Layer 6 "Ground". Floor and table of every pointer scene are on that layer
//     (a scene stores the layer's number), and the mouse rays of the FSMs look for it;
//  2. adds the tags the examples use: none since 13.11.2026 (see Tags below);
//  3. appends the menu and the 29 examples to the end of File → Build Profiles → Scene List,
//     the menu first: the bar's Next and Back walk along that list. Your own scenes stay where
//     they are; a scene that is in the list but unticked counts as there.
using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

[InitializeOnLoad]
public static class IMAPointerSetup
{
    public const string GroundName = "Ground";
    /// The slot the scenes were built with. A scene stores this number, not the name.
    public const int GroundIndex = 6;
    /// The tags the examples use. None since 13.11.2026: the two examples that used tags are
    /// parked (Sort by colour: RedBin, BlueBin, YellowBin; Connect two points: RedSocket,
    /// BlueSocket, GreenSocket). An example that comes back brings its tags back into this list;
    /// the builder stops on a tag that is not in it.
    public static readonly string[] Tags = { };
    public const string Examples = "Assets/IMA_Pointer/Examples";

    static IMAPointerSetup()
    {
        // Right away (also in batch mode, where delayCall may never fire) and once more on the
        // next editor tick, in case the project settings weren't ready during the domain reload.
        Run();
        EditorApplication.delayCall += Run;
    }

    public static void Run()
    {
        try { AddLayer(); } catch (Exception e) { Debug.LogWarning("IMA_Pointer: layer later (" + e.Message + ")"); }
        try { AddTags(); } catch (Exception e) { Debug.LogWarning("IMA_Pointer: tags later (" + e.Message + ")"); }
        try { AddScenes(); } catch (Exception e) { Debug.LogWarning("IMA_Pointer: scene list later (" + e.Message + ")"); }
    }

    /// Makes User Layer 6 the layer Ground and returns 6. If that slot already has another
    /// name, or the project has a layer Ground somewhere else, nothing is changed: it says in
    /// the Console what to do and returns the number of the layer that is called Ground (or -1).
    public static int AddLayer()
    {
        var assets = AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset");
        if (assets == null || assets.Length == 0) throw new Exception("the tag manager is not loaded yet");
        var manager = new SerializedObject(assets[0]);
        var layers = manager.FindProperty("layers");
        if (layers == null || layers.arraySize <= GroundIndex) throw new Exception("the tag manager has no layers yet");

        var slot = layers.GetArrayElementAtIndex(GroundIndex);
        if (slot.stringValue == GroundName) return GroundIndex;
        var elsewhere = -1;
        for (int i = 0; i < layers.arraySize && elsewhere < 0; i++)
            if (layers.GetArrayElementAtIndex(i).stringValue == GroundName) elsewhere = i;

        if (string.IsNullOrEmpty(slot.stringValue) && elsewhere < 0)
        {
            slot.stringValue = GroundName;
            manager.ApplyModifiedPropertiesWithoutUndo();
            AssetDatabase.SaveAssets();
            Debug.Log("IMA_Pointer: added the layer Ground (User Layer " + GroundIndex + ")");
            return GroundIndex;
        }

        // Once per editor session: this is a thing to fix by hand, not to read on every compile.
        if (!SessionState.GetBool("IMAPointerSetup.layerWarned", false))
        {
            SessionState.SetBool("IMAPointerSetup.layerWarned", true);
            var use = string.IsNullOrEmpty(slot.stringValue) ? "has no name" : "is called \"" + slot.stringValue + "\"";
            Debug.LogWarning("IMA_Pointer: floor and table of the pointer scenes are on User Layer " + GroundIndex + ", which " + use + " in this project"
                + (elsewhere >= 0 ? ", and the layer Ground is User Layer " + elsewhere : "") + ". The examples still work (their mouse rays look for layer "
                + GroundIndex + " by number). For your own FSMs: Edit > Project Settings > Tags and Layers, call User Layer " + GroundIndex
                + " \"Ground\" (and give any other layer of that name a new one), or pick the layer of Floor and Table wherever a tutorial says Ground.");
        }
        return elsewhere;
    }

    public static void AddTags()
    {
        var existing = InternalEditorUtility.tags;
        var added = false;
        foreach (var tag in Tags)
            if (!existing.Contains(tag))
            {
                InternalEditorUtility.AddTag(tag);
                Debug.Log("IMA_Pointer: added tag " + tag);
                added = true;
            }
        if (added) AssetDatabase.SaveAssets();
    }

    /// Makes sure the menu and every example are in the Scene List: the missing ones are
    /// appended in the order of their numbers (Pointer_00_Menu first). Nothing that is in the
    /// list is moved, removed or ticked.
    public static void AddScenes()
    {
        // The files, not the AssetDatabase: it may not answer yet while the editor loads.
        if (!Directory.Exists(Examples)) return;
        var wanted = Directory.GetFiles(Examples, "Pointer_*.unity")
            .Select(p => p.Replace('\\', '/'))
            .OrderBy(p => p, StringComparer.Ordinal);
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
        Debug.Log("IMA_Pointer: added " + added + " scenes to the Scene List (the menu and the examples)");
    }
}
