using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace SLIDDES.Multiplayer.Couch
{
    public class CouchMultiplayerCanvasManager : MonoBehaviour
    {
        public Components components;

        private UnityAction<GameObject> actionOnAddPlayer;
        private List<CouchMultiplayerPlayerCanvas> playerCanvases = new List<CouchMultiplayerPlayerCanvas>();

        private void OnEnable()
        {
            if(components.playerSpawner == null)
            {
                throw new NullReferenceException("PlayerSpawner not assigned!");
            }
            if(components.parentCanvases == null) components.parentCanvases = transform;

            actionOnAddPlayer = x => RefreshPlayerCanvases();
            components.playerSpawner.events.onPlayerInstantiate.AddListener(actionOnAddPlayer);

            RefreshPlayerCanvases();
        }

        private void OnDisable()
        {
            components.playerSpawner.events.onPlayerInstantiate.RemoveListener(actionOnAddPlayer);
        }


        public void AddPlayerCanvas()
        {

        }

        public void RefreshPlayerCanvases()
        {
            // Clear old canvases
            foreach(Transform child in components.parentCanvases.transform)
            {
                Destroy(child.gameObject);
            }

            // Create a canvas for each player
            foreach(var item in components.playerSpawner.Players)
            {

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
