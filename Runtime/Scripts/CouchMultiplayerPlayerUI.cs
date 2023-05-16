using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.UI;

namespace SLIDDES.Multiplayer.Couch
{
    /// <summary>
    /// Component that lets the player control UI
    /// </summary>
    public class CouchMultiplayerPlayerUI : CouchMultiplayerPlayerBase
    {
        public GameObject CurrentSelectedGameObject => components.currentSelectedGameObject;

        public Components components;

        public override void Initialize(PlayerData playerData)
        {
            base.Initialize(playerData);

            components.manager = FindObjectOfType<CouchMultiplayerPlayerUIManager>();
            if(components.manager == null)
            {
                Debug.LogError($"{DebugPrefix()}");
                return;
            }

            components.manager.AddPlayerUI(this);
        }

        public override string DebugPrefix() => "[CouchMultiplayerPlayerUI] No CouchMultiplayerPlayerUIManager found! Make sure to add a CouchMultiplayerPlayerUIManager component in the scene";

        /// <summary>
        /// Set the UI current selected gameobject
        /// </summary>
        /// <param name="gameObject"></param>
        public void SetCurrentSelectedGameObject(GameObject gameObject)
        {
            components.currentSelectedGameObject = gameObject;
            components.multiplayerEventSystem.SetSelectedGameObject(gameObject);
        }

        [System.Serializable]
        public class Components
        {
            public MultiplayerEventSystem multiplayerEventSystem;

            [Header("Auto-Set")]
            [Tooltip("The manager of PlayerUI")]
            public CouchMultiplayerPlayerUIManager manager;
            [Tooltip("The current selected UI gameobject by the multiplayerEventSystem")]
            public GameObject currentSelectedGameObject;
        }
    }
}
