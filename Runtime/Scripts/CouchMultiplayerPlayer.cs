using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;
using UnityEngine.InputSystem.Users;

namespace SLIDDES.Multiplayer.Couch
{
    /// <summary>
    /// Contains information about a player for couch mutliplayer
    /// </summary>
    [AddComponentMenu("SLIDDES/Multiplayer/Couch/Couch Multiplayer Player")]
    [RequireComponent(typeof(PlayerInput))]
    public class CouchMultiplayerPlayer : MonoBehaviour
    {
        public PlayerData PlayerData { get; private set; }
        public Camera Camera { get; private set; }
        public Camera CameraUI { get; private set; }
        public PlayerInput PlayerInput { get; private set; }
        public MultiplayerEventSystem MultiplayerEventSystem { get; private set; }

        public UnityEvent onInitialized;

        /// <summary>
        /// Initialize the player with the playerData
        /// </summary>
        /// <param name="playerData"></param>
        /// <param name="spawner"></param>
        public virtual void Initialize(PlayerData playerData)
        {
            PlayerData = playerData;

            if(PlayerInput == null)
            {
                PlayerInput = GetComponent<PlayerInput>();
                if(PlayerInput == null)
                {
                    GetComponentInChildren<PlayerInput>();
                    if(PlayerInput == null)
                    {
                        Debug.LogError($"{DebugPrefix()} playerInput is not assigned. Please assign the component reference");
                    }
                }
            }

            if(MultiplayerEventSystem == null)
            {
                MultiplayerEventSystem = GetComponent<MultiplayerEventSystem>();
                if(MultiplayerEventSystem == null)
                {
                    MultiplayerEventSystem = GetComponentInChildren<MultiplayerEventSystem>();                    
                }
            }

            // Set auto switch
            if(CouchMultiplayerManager.MaxPlayers == 1)
            {
                PlayerInput.neverAutoSwitchControlSchemes = !CouchMultiplayerManager.SinglePlayerInputSwitch;
            }
            else PlayerInput.neverAutoSwitchControlSchemes = true;

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
