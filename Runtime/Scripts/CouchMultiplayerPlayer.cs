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

        [Header("Components")]
        [Tooltip("The corresponding camera of the player. Can be left to null if the player doesnt have a camera")]
        public new Camera camera;
        [Tooltip("Reference to the player input script")]
        public PlayerInput playerInput;

        private readonly string debugPrefix = "[CouchMultiplayerPlayer]";

        // Start is called before the first frame update
        void Start()
        {
            
        }

        // Update is called once per frame
        void Update()
        {
        
        }

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
            camera.targetDisplay = playerData.cameraTargetDisplay;
            camera.rect = playerData.cameraViewPortRect;
        }
    }
}
