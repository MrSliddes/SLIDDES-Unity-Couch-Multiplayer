using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace SLIDDES.Multiplayer.Couch
{
    /// <summary>
    /// Base script for a multiplayer player
    /// </summary>
    public class CouchMultiplayerPlayerBase : MonoBehaviour
    {
        [Tooltip("The playerdata of this player")]
        public PlayerData playerData;
        [Tooltip("Reference to the player input script")]
        public PlayerInput playerInput;

        public UnityEvent onInitialized;

        /// <summary>
        /// Initialize the player with the playerData
        /// </summary>
        /// <param name="playerData"></param>
        /// <param name="spawner"></param>
        public virtual void Initialize(PlayerData playerData)
        {
            this.playerData = playerData;

            if(playerInput == null)
            {
                playerInput = GetComponent<PlayerInput>();
                if(playerInput == null) GetComponentInChildren<PlayerInput>();
                if(playerInput == null)
                {
                    Debug.LogError($"{DebugPrefix()} playerInput is not assigned. Please assign the component reference");
                }
            }

            // Set auto switch
            if(CouchMultiplayerManager.MaxPlayers == 1)
            {
                playerInput.neverAutoSwitchControlSchemes = !CouchMultiplayerManager.SinglePlayerInputSwitch;
            }
            else playerInput.neverAutoSwitchControlSchemes = true;

            onInitialized?.Invoke();
        }

        // Start is called before the first frame update
        public virtual void Start()
        {
        
        }

        // Update is called once per frame
        public virtual void Update()
        {
        
        }

        public virtual string DebugPrefix() => "[CouchMultiplayerBase]";

        /// <summary>
        /// Refreshes the camera with playerData camera values
        /// </summary>
        public virtual void RefreshCamera()
        {

        }
    }
}
