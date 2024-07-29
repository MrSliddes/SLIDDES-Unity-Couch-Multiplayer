using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace SLIDDES.Multiplayer.Couch
{
    public class CouchMultiplayerSettings : ScriptableObject
    {
        public static CouchMultiplayerSettings Instance
        {
            get
            {
                if(instance == null)
                {
#if UNITY_EDITOR
                    instance = GetOrCreateSettings();
#else
                    instance = Resources.Load<CouchMultiplayerSettings>(ResourceFilePath);
#endif
                }
                return instance;
            }
        }
        public static readonly string FolderPath = "Assets/Settings/SLIDDES/Resources/";
        public static readonly string FilePath = "Assets/Settings/SLIDDES/Resources/CouchMultiplayerSettings.asset";
        public static readonly string ResourceFilePath = "CouchMultiplayerSettings";

        private static CouchMultiplayerSettings instance;

        #region Settings

        public static int MaxDevices => Instance.maxDevices;
        public static int MaxDisplays => Instance.maxDisplays;
        public static bool AllowSinglePlayerDeviceSwitch => Instance.allowSinglePlayerDeviceSwitch;
        public static string[] ExcludedDeviceNames
        {
            get
            {
                return Instance.excludedDeviceNames;
            }
        }
        public static InputActionProperty[] DeviceRegisterActions => Instance.deviceRegisterActions;
        public static bool ShowDebug => Instance.showDebug;
        public static bool ShowDebugDeviceNames => Instance.showDebugDeviceNames;

        [Range(1, 8)]
        [SerializeField] private int maxDevices = 8;
        [Range(1, 8)]
        [SerializeField] private int maxDisplays = 1;
        [SerializeField] private bool allowSinglePlayerDeviceSwitch = true;
        /// <summary>
        /// Device names that are excluded from being recognised as a player
        /// </summary>
        [SerializeField] private string[] excludedDeviceNames = new string[] { "Mouse" };
        [SerializeField] private InputActionProperty[] deviceRegisterActions;

        [Header("Debug")]
        [SerializeField] private bool showDebug;
        [SerializeField] private bool showDebugDeviceNames;

        #endregion Settings

#if UNITY_EDITOR
        internal static CouchMultiplayerSettings GetOrCreateSettings()
        {
            if(!AssetDatabase.IsValidFolder("Assets/Settings"))
            {
                AssetDatabase.CreateFolder("Assets", "Settings");
            }

            if(!AssetDatabase.IsValidFolder("Assets/Settings/SLIDDES"))
            {
                AssetDatabase.CreateFolder("Assets/Settings", "SLIDDES");
            }

            if(!AssetDatabase.IsValidFolder("Assets/Settings/SLIDDES/Resources"))
            {
                AssetDatabase.CreateFolder("Assets/Settings/SLIDDES", "Resources");
            }

            var settings = AssetDatabase.LoadAssetAtPath<CouchMultiplayerSettings>(FilePath);
            if(settings == null)
            {
                settings = ScriptableObject.CreateInstance<CouchMultiplayerSettings>();

                AssetDatabase.CreateAsset(settings, FilePath);
                AssetDatabase.SaveAssets();
            }
            instance = settings;
            return settings;
        }

        public static SerializedObject GetSerializedSettings()
        {
            return new SerializedObject(GetOrCreateSettings());
        }
    }
#endif
}
