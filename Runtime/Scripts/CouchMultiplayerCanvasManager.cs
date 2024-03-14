using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;

namespace SLIDDES.Multiplayer.Couch
{
    [AddComponentMenu("SLIDDES/Multiplayer/Couch/Canvas Manager")]
    [RequireComponent(typeof(EventSystem))]
    [RequireComponent(typeof(InputSystemUIInputModule))]
    public class CouchMultiplayerCanvasManager : MonoBehaviour
    {
        public Components components;

        /// <summary>
        /// Action that gets triggerd when a player gets added
        /// </summary>
        private UnityAction<GameObject> actionOnAddPlayer;
        /// <summary>
        /// List containing all the player canvases
        /// </summary>
        private List<CouchMultiplayerPlayerCanvas> playerCanvases = new List<CouchMultiplayerPlayerCanvas>();

        private void OnEnable()
        {
            if(components.playerSpawner == null)
            {
                throw new NullReferenceException("PlayerSpawner not assigned!");
            }
            if(components.parentCanvases == null) components.parentCanvases = transform;

            actionOnAddPlayer = x => GeneratePlayerCanvases();
            components.playerSpawner.onPlayerInstantiate.AddListener(actionOnAddPlayer);

            GeneratePlayerCanvases();
        }

        private void OnDisable()
        {
            components.playerSpawner.onPlayerInstantiate.RemoveListener(actionOnAddPlayer);
        }

        /// <summary>
        /// Generate for each player the player canvas
        /// </summary>
        public void GeneratePlayerCanvases()
        {
            // Clear old canvases
            playerCanvases.Clear();
            foreach(Transform child in components.parentCanvases.transform)
            {
                Destroy(child.gameObject);
            }

            // Create a canvas for each player
            foreach(var item in components.playerSpawner.Players)
            {
                CouchMultiplayerPlayer player = (CouchMultiplayerPlayer)item;
                if(player == null) continue;

                GameObject a = Instantiate(components.prefabPlayerCanvas, components.parentCanvases);
                CouchMultiplayerPlayerCanvas canvas = a.GetComponent<CouchMultiplayerPlayerCanvas>();
                canvas.Initialize(player);
                playerCanvases.Add(canvas);
            }
        }

        [System.Serializable]
        public class Components
        {
            [Tooltip("The player canvas prefab")]
            public GameObject prefabPlayerCanvas;
            [Tooltip("The parent transform of all canvases")]
            public Transform parentCanvases;
            [Tooltip("Reference to the player spawner")]
            public CouchMultiplayerPlayerSpawner playerSpawner;
        }
    }
}
