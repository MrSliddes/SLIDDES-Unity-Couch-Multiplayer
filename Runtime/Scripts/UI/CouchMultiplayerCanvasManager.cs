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
    [AddComponentMenu("SLIDDES/Multiplayer/Couch/Couch Multiplayer Canvas Manager")]
    public class CouchMultiplayerCanvasManager : MonoBehaviour
    {
        [Tooltip("The player canvas prefab")]
        [SerializeField] private GameObject prefabPlayerCanvas;
        [Tooltip("The parent transform of all canvases")]
        [SerializeField] private Transform parentCanvases;
        [Tooltip("Reference to the player spawner")]
        [SerializeField] private CouchMultiplayerPlayerSpawner playerSpawner;

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
            if(playerSpawner == null)
            {
                throw new NullReferenceException("PlayerSpawner not assigned!");
            }
            if(parentCanvases == null) parentCanvases = transform;

            actionOnAddPlayer = x => GeneratePlayerCanvases();
            playerSpawner.onPlayerInstantiate.AddListener(actionOnAddPlayer);

            GeneratePlayerCanvases();
        }

        private void OnDisable()
        {
            playerSpawner.onPlayerInstantiate.RemoveListener(actionOnAddPlayer);
        }

        /// <summary>
        /// Generate for each player the player canvas
        /// </summary>
        public void GeneratePlayerCanvases()
        {
            // Clear old canvases
            playerCanvases.Clear();
            foreach(Transform child in parentCanvases.transform)
            {
                Destroy(child.gameObject);
            }

            // Create a canvas for each player
            CouchMultiplayerPlayer[] players = playerSpawner.Players;
            for(int i = 0; i < players.Length; i++)
            {
                CouchMultiplayerPlayer player = players[i];
                if(player == null) continue;

                GameObject a = Instantiate(prefabPlayerCanvas, parentCanvases);
                CouchMultiplayerPlayerCanvas canvas = a.GetComponent<CouchMultiplayerPlayerCanvas>();
                canvas.Initialize(player);
                playerCanvases.Add(canvas);
                player.MultiplayerEventSystem.playerRoot = a;
            }
        }
    }
}
