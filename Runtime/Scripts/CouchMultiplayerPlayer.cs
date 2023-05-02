using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Users;

namespace SLIDDES.Multiplayer.Couch
{
    /// <summary>
    /// Contains information about a player for couch mutliplayer
    /// </summary>
    [AddComponentMenu("SLIDDES/Multiplayer/Couch/Couch Multiplayer Player")]
    public class CouchMultiplayerPlayer : MonoBehaviour
    {
        public PlayerData playerData;
                
        [Tooltip("Reference to the player input script")]
        public PlayerInput playerInput;
        public new Camera camera;
        [Tooltip("The camera that renders the UI")]
        public Camera cameraUI;

        private readonly string debugPrefix = "[CouchMultiplayerPlayer]";

        /// <summary>
        /// Initialize the player with the playerData
        /// </summary>
        /// <param name="playerData"></param>
        /// <param name="spawner"></param>
        public virtual void Initialize(PlayerData playerData)
        {
            this.playerData = playerData;

            if(playerInput == null) { GetComponentInChildren<PlayerInput>(); }
            if(playerInput == null)
            {
                Debug.LogError($"{debugPrefix} playerInput is not assigned. Please assign the component reference");
            }

            // Set auto switch
            if(CouchMultiplayerManager.Instance.maxPlayers == 1)
            {
                playerInput.neverAutoSwitchControlSchemes = !CouchMultiplayerManager.Instance.singlePlayerInputSwitch;
            }
            else playerInput.neverAutoSwitchControlSchemes = true;        
        }

        /// <summary>
        /// Refreshes the camera with playerData camera values
        /// </summary>
        public virtual void RefreshCamera()
        {
                      
        }
    }
}
