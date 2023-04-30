using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace SLIDDES.Multiplayer.Couch
{
    /// <summary>
    /// Contains information about a player for couch mutliplayer
    /// </summary>
    [AddComponentMenu("SLIDDES/Multiplayer/Couch/Couch Multiplayer Player")]
    public class CouchMultiplayerPlayer : MonoBehaviour
    {
        public PlayerData playerData;
        [Tooltip("The corresponding cameras of the player. Can be left to null if the player doesnt have a camera. First index is used for Canvas overlay")]
        public CameraConfiguration[] cameraConfigurations;

        [Header("Components")]
        [Tooltip("Reference to the player input script")]
        public PlayerInput playerInput;

        private readonly string debugPrefix = "[CouchMultiplayerPlayer]";

        /// <summary>
        /// Initialize the player with the playerData
        /// </summary>
        /// <param name="playerData"></param>
        /// <param name="spawner"></param>
        public void Initialize(PlayerData playerData)
        {
            this.playerData = playerData;

            if(playerInput == null) { GetComponentInChildren<PlayerInput>(); }
            if(playerInput == null)
            {
                Debug.LogError($"{debugPrefix} playerInput is not assigned. Please assign the component reference");
            }

            playerInput.SwitchCurrentControlScheme(playerData.inputDevice);
        }

        /// <summary>
        /// Refreshes the camera with playerData camera values
        /// </summary>
        public void RefreshCamera()
        {
            foreach(var config in cameraConfigurations)
            {
                config.camera.targetDisplay = playerData.cameraTargetDisplay;
                if(config.setViewPortRect) config.camera.rect = playerData.cameraViewPortRect;

                if(!config.setLayers) continue;

                // Set camera overlay layer based on playerIndex
                int overlayLayer = -1;
                if(config.cameraOverlayLayers.value != 0) // check if a layer has been selected
                {
                    int[] availableLayers = config.cameraOverlayLayers.IncludedLayers();
                    // Check if not out of bounds
                    if(playerData.playerIndex > availableLayers.Length - 1)
                    {
                        Debug.LogError($"{debugPrefix} Too few layers selected! Make sure that max amount of players is equal to selected layers");
                    }
                    overlayLayer = availableLayers[playerData.playerIndex];
                    config.camera.gameObject.layer = overlayLayer;
                }

                // Set camera culling mask
                int layer = -1;
                if(config.cameraLayers.value != 0) // check if a layer has been selected
                {
                    int[] availableLayers = config.cameraLayers.IncludedLayers();
                    layer = availableLayers[playerData.playerIndex];
                    config.camera.cullingMask &= ~(1 << layer); // turn off bit
                                                                // Set overlay layer on
                    if(config.cameraOverlayLayers.value != 0) config.camera.cullingMask |= 1 << overlayLayer; // turn on bit
                }

                if(!config.setGameObjectLayers) continue;
                // Set gameobject overlay layers
                if(config.cameraOverlayLayers.value != 0)
                {
                    foreach(var item in config.gameObjectsFirstPersonLayer)
                    {
                        if(config.setGameObjectLayersRecursively) item.transform.SetLayerRecursively(overlayLayer); else item.layer = overlayLayer;
                    }
                }
                // Set gameobject layers
                if(config.cameraLayers.value != 0)
                {
                    foreach(var item in config.gameObjectsThirdPersonLayer)
                    {
                        if(config.setGameObjectLayersRecursively) item.transform.SetLayerRecursively(layer); else item.layer = layer;
                    }
                }
            }
        }

        [System.Serializable]
        public class CameraConfiguration
        {
            public Camera camera;
            [Tooltip("When camera is refreshed set view port rect")]
            public bool setViewPortRect = true;
            [Tooltip("Set the camera layer to that of the player index")]
            public bool setLayers;
            [Tooltip("The layers used for overlay")]
            public LayerMask cameraOverlayLayers;
            [Tooltip("The layers used for the culling mask of the camera")]
            public LayerMask cameraLayers;
            [Tooltip("Set correct layers for gameobjects")]
            public bool setGameObjectLayers;
            [Tooltip("Set the layers of the gameobjects recursively")]
            public bool setGameObjectLayersRecursively = true;
            public GameObject[] gameObjectsFirstPersonLayer;
            public GameObject[] gameObjectsThirdPersonLayer;
        }
    }
}
