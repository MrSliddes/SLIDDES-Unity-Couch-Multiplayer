using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

namespace SLIDDES.Multiplayer.Couch.Editor
{
    // Register a SettingsProvider using IMGUI for the drawing framework:
    static class CouchMultiplayerSettingsIMGUIRegister
    {
        private static bool foldoutExludedDeviceNames;
        private static bool foldoutDeviceRegisterActions;
        private static GUIStyle foldoutGUIStyle = new GUIStyle(EditorStyles.foldout) { fontStyle = FontStyle.Bold };
        private static UnityEditor.Editor cachedEditor;

        [SettingsProvider]
        public static SettingsProvider CreateMyCustomSettingsProvider()
        {
            // First parameter is the path in the Settings window.
            // Second parameter is the scope of this setting: it only appears in the Project Settings window.
            var provider = new SettingsProvider("Project/CouchMultiplayerSettingsIMGUISettings", SettingsScope.Project)
            {
                // By default the last token of the path is used as display name if no label is provided.
                label = "SDS Couch Multiplayer",
                // Create the SettingsProvider and initialize its drawing (IMGUI) function in place:
                guiHandler = (searchContext) =>
                {
                    var settings = CouchMultiplayerSettings.GetSerializedSettings();

                    UnityEditor.Editor.CreateCachedEditor(settings.targetObject, null, ref cachedEditor);
                    cachedEditor.OnInspectorGUI();

                    settings.ApplyModifiedProperties();
                },

                // Populate the search keywords to enable smart search filtering and label highlighting:
                keywords = new HashSet<string>(new[] { "Couch Multiplayer", "SLIDDES" })
            };

            return provider;
        }
    }

}
