using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace SLIDDES.Multiplayer.Couch
{
    /// <summary>
    /// The singular instance manager for couch multiplayer.
    /// </summary>
    /// <remarks>
    /// CCM stands for Couch Multiplayer Manager
    /// </remarks>
    [AddComponentMenu("SLIDDES/Multiplayer/Couch/Couch Multiplayer Manager")]
    [DefaultExecutionOrder(-1)] // Execute the script before default time executed scripts
    public class CouchMultiplayerManager : MonoBehaviour
    {
        /// <summary>
        /// The singular instance of the couch multiplayer manager
        /// </summary>
        public static CouchMultiplayerManager Instance => instance;

        // Shortcuts
        /// <summary>
        /// Get an array of all players playerData
        /// </summary>
        public static PlayerData[] PlayerDatas => Instance.players.Values.ToArray();
        /// <summary>
        /// Amount of players currently active
        /// </summary>
        public static int PlayerAmount => Instance.players.Count;

        public static bool PlayersCanJoin
        {
            get => Instance.playersCanJoin;
            set
            {
                Instance.playersCanJoin = value;
                if(Instance.inputActionAddPlayer == null) return; // dont need to set anything
                if(value)
                {
                    Instance.inputActionAddPlayer.Enable();
                    Instance.joinAction.action.Enable();
                }
                else
                {
                    Instance.inputActionAddPlayer.Disable();
                    Instance.joinAction.action.Disable();
                }
            }
        }

        // Values
        [Tooltip("Dont destroy this gameObject when loading a new scene. Recommended to be set to true")]
        public bool dontDestroyOnLoad = true;
        [Tooltip("Device names that are excluded from being recognised as a player")]
        public string[] excludedDeviceNames = new string[] { "Mouse" };

        [Header("Player Settings")]
        [Tooltip("Allow for players to join the game?")]
        [SerializeField] private bool playersCanJoin = true;
        [Tooltip("The button the player needs to press for joining")]
        public InputActionProperty joinAction;
        [Range(1, 8)]
        [Tooltip("The maximum amount of players that can play")]
        public int maxPlayers = 8;
        [Tooltip("The max amount of displays used. Players are devided equally over the displays")]
        [Range(1, 8)]
        public int maxDisplays = 1;
        [Tooltip("Allow for input switch when only 1 player is playing")]
        public bool singlePlayerInputSwitch = true;
                
        [Header("Debug Settings")]
        [Tooltip("Show debug messages from this script")]
        public bool showDebug;
        [Tooltip("Debug log the device name when trying to add a new player")]
        public bool debugDeviceNameOnAdd;

        // Events
        [Header("Events")]
        [Tooltip("When a new player gets added")]
        public UnityEvent<PlayerData> onAddPlayer;

        /// <summary>
        /// The private reference to the CCM
        /// </summary>
        private static CouchMultiplayerManager instance;

        /// <summary>
        /// Keeps track of how many players have joined
        /// </summary>
        private int playerIndexCounter;
        /// <summary>
        /// Readonly string that serves as prefix for this script debug messages
        /// </summary>
        private readonly string debugPrefix = "[CouchMultiplayerManager]";
        /// <summary>
        /// Dictionary containing all active players
        /// </summary>
        private Dictionary<InputDevice, PlayerData> players = new Dictionary<InputDevice, PlayerData>();
        /// <summary>
        /// Input action reference for adding player
        /// </summary>
        private InputAction inputActionAddPlayer;

        private void Awake()
        {
            // Check if only 1 instance is active
            if(instance != null && instance != this)
            {
                Debug.LogWarning($"{debugPrefix} Detected more than 1 instance of a couchMutliplayerManager script. Only 1 instance is allowed in a scene! Destroying the newly created one...");
                Destroy(gameObject);
                return;
            }

            // No instance active, set instance
            instance = this;

            // Dont destroy on scene load
            if(dontDestroyOnLoad) DontDestroyOnLoad(gameObject);
        }

        // Start is called before the first frame update
        void Start()
        {
            // Setup join input action

            // if the join action is a reference, clone it so we don't run into problems with the action being disabled by
            // PlayerInput when devices are assigned to individual players
            if(joinAction.reference != null && joinAction.action?.actionMap?.asset != null)
            {
                var inputActionAsset = Instantiate(joinAction.action.actionMap.asset);
                var inputActionReference = InputActionReference.Create(inputActionAsset.FindAction(joinAction.action.name));
                joinAction = new InputActionProperty(inputActionReference);
                joinAction.action.performed += x => AddPlayer(x.control.device);
                joinAction.action.Enable();
            }
        }
               

        /// <summary>
        /// Add a player to the couch
        /// </summary>
        /// <param name="inputDevice"></param>
        public void AddPlayer(InputDevice inputDevice)
        {
            // Debug log device name if required
            if(debugDeviceNameOnAdd) Debug.Log($"{debugPrefix} AddPlayer() with device {inputDevice.name}");

            // Check if device name is excluded
            if(excludedDeviceNames.Any(inputDevice.name.Contains))
            {
                if(showDebug) Debug.Log($"{debugPrefix} Can't add excluded device: {inputDevice.name}");
                return;
            }

            // Check if max player amount has been reached
            if(PlayerAmount >= maxPlayers)
            {
                if(showDebug) Debug.Log($"{debugPrefix} Max player amount of {maxPlayers} has been reached, can't add new player");
                return;
            }

            // Check if inputDevice isn't already added
            if(players.ContainsKey(inputDevice))
            {
                if(showDebug && Application.isEditor) Debug.Log($"{debugPrefix} Player of device {inputDevice.name} already added, ignoring input");
                return;
            }

            // Add inputDevice with playerData
            PlayerData playerData = new PlayerData() 
            { 
                inputDevice = inputDevice,
                playerIndex = playerIndexCounter
            };
            players.Add(inputDevice, playerData);

            playerIndexCounter++;

            onAddPlayer.Invoke(playerData);
        }
        
        /// <summary>
        /// Clear all players connected
        /// </summary>
        public void ClearPlayers()
        {
            players.Clear();
            playerIndexCounter = 0;
        }
    }
}
