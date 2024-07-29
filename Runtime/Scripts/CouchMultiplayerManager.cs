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
        public static CouchMultiplayerManager Instance
        {
            get
            {
                if(instance == null && !ApplicationIsQuitting)
                {
                    GameObject g = new GameObject("Couch Multiplayer Manager");
                    instance = g.AddComponent<CouchMultiplayerManager>();
                }
                return instance;
            }
        }
        public static bool ApplicationIsQuitting { get; private set; }
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
                    foreach(InputActionProperty joinAction in CouchMultiplayerSettings.DeviceRegisterActions)
                    {
                        joinAction.action.Enable();
                    }
                }
                else
                {
                    Instance.inputActionAddPlayer.Disable();
                    foreach(InputActionProperty joinAction in CouchMultiplayerSettings.DeviceRegisterActions)
                    {
                        joinAction.action.Disable();
                    }
                }
            }
        }
        /// <summary>
        /// Amount of players currently active
        /// </summary>
        public static int PlayerAmount => Instance.players.Count;
        public static PlayerData[] PlayerDatas => Instance.players.Values.ToArray();

        public UnityAction<PlayerData> onAddPlayer;
        public UnityAction<PlayerData> onRemovePlayer;

        /// <summary>
        /// The private reference to the CCM
        /// </summary>
        private static CouchMultiplayerManager instance;

        private bool playersCanJoin = true;
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

            ApplicationIsQuitting = false;

            // Dont destroy on scene load
            DontDestroyOnLoad(gameObject);
        }

        private void OnEnable()
        {
            Application.quitting += () => { ApplicationIsQuitting = true; };
        }

        private void OnDisable()
        {
            Application.quitting -= () => { ApplicationIsQuitting = true; };
        }

        // Start is called before the first frame update
        void Start()
        {
            // Setup join input action

            // if the join action is a reference, clone it so we don't run into problems with the action being disabled by
            // PlayerInput when devices are assigned to individual players
            for(int i = 0; i < CouchMultiplayerSettings.DeviceRegisterActions.Length; i++)
            {
                InputActionProperty joinAction = CouchMultiplayerSettings.DeviceRegisterActions[i];
                if(joinAction.reference != null && joinAction.action?.actionMap?.asset != null)
                {
                    var inputActionAsset = Instantiate(joinAction.action.actionMap.asset);
                    var inputActionReference = InputActionReference.Create(inputActionAsset.FindAction(joinAction.action.name));
                    joinAction = new InputActionProperty(inputActionReference);
                    joinAction.action.performed += x =>
                    {
                        if(playersCanJoin) AddPlayer(x.control.device);
                    };
                    joinAction.action.Enable();
                }
            }
        }


        /// <summary>
        /// Add a player to the couch
        /// </summary>
        /// <param name="inputDevice"></param>
        public void AddPlayer(InputDevice inputDevice)
        {
            // Debug log device name if required
            if(CouchMultiplayerSettings.ShowDebugDeviceNames) Debug.Log($"{debugPrefix} AddPlayer() with device {inputDevice.name}");

            // Check if device name is excluded
            if(CouchMultiplayerSettings.ExcludedDeviceNames.Any(inputDevice.name.Contains))
            {
                if(CouchMultiplayerSettings.ShowDebug) Debug.Log($"{debugPrefix} Can't add excluded device: {inputDevice.name}");
                return;
            }

            // Check if max player amount has been reached
            if(PlayerAmount >= CouchMultiplayerSettings.MaxDevices)
            {
                if(CouchMultiplayerSettings.ShowDebug) Debug.Log($"{debugPrefix} Max device amount of {CouchMultiplayerSettings.MaxDevices} has been reached, can't add new player");
                return;
            }

            // Check if inputDevice isn't already added
            if(players.ContainsKey(inputDevice))
            {
                if(CouchMultiplayerSettings.ShowDebug && Application.isEditor) Debug.Log($"{debugPrefix} Player of device {inputDevice.name} already added, ignoring input");
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

#if USING_SLIDDES_UI
            if(SLIDDES.UI.InputManager.Instance != null)
            {
                SLIDDES.UI.InputManager.Instance.UpdatePlayers();
            }
#endif
        }

        /// <summary>
        /// Add a player to the couch
        /// </summary>
        /// <param name="context"></param>
        public void AddPlayer(InputAction.CallbackContext context)
        {
            AddPlayer(context.control.device);
        }

        public void RemovePlayer(InputDevice inputDevice)
        {
            if(!players.ContainsKey(inputDevice))
            {
                return;
            }

            PlayerData playerData = players[inputDevice];
            players.Remove(inputDevice);
            onRemovePlayer?.Invoke(playerData);
        }

        /// <summary>
        /// Clear all players connected
        /// </summary>
        public void ClearPlayers()
        {
            players.Clear();
            playerIndexCounter = 0;
        }

        public PlayerData GetPlayerData(PlayerInput playerInput)
        {
            return players.Values.FirstOrDefault(x => x.inputDevice == playerInput.devices[0]);
        }
    }
}
