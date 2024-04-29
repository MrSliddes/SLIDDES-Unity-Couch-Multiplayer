using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace SLIDDES.Multiplayer.Couch
{
    /// <summary>
    /// Spawns players
    /// </summary>
    [AddComponentMenu("SLIDDES/Multiplayer/Couch/Couch Multiplayer Player Spawner")]
    public class CouchMultiplayerPlayerSpawner : MonoBehaviour
    {
        /// <summary>
        /// Contains all of the active players
        /// </summary>
        public CouchMultiplayerPlayer[] Players => players.Values.ToArray();

        [Header("Values")]
        [Tooltip("Remove players that are already present in the scene for cleanup before spawning players")]
        [SerializeField] private bool removePlayersOnAwake = true;
        [Tooltip("Clear the playerData stored in manager on start")]
        [SerializeField] private bool clearPlayerDataOnStart;
        [Tooltip("Spawn the connected players (from CouchMutliplayerManager) on start")]
        [SerializeField] private bool spawnOnStart;
        [SerializeField] private bool orderSpawningOnLobbyID;
        [Tooltip("The amount of players to spawn once this spawner has initalized")]
        [SerializeField] private int playersToSpawnOnInitialized;

        [Header("Split-Screen")]
        [Tooltip("Enable splitscreen for the players. Also set this to true if using CouchMultiplayerCanvasViewportRect")]
        [SerializeField] private bool enableSplitScreen;
        [SerializeField] private SplitScreenMode splitScreenMode = SplitScreenMode.Horizontal;

        [Header("Debug")]
        [Tooltip("Show debug messages from this script")]
        [SerializeField] private bool showDebug;

        [Header("Components")]
        [Tooltip("The prefab of the player to be spawned")]
        [SerializeField] private GameObject prefabPlayer;
        [Tooltip("When the prefab of the player spawns, look for an rigidbody component and set the correct position of that component")]
        [SerializeField] private bool onSpawnSetPrefabRBPosition = true;
        [Tooltip("The parent transform where all instantiated players will be parented under. If left null this gameObject transform will be assigned as parent")]
        [SerializeField] private Transform parentTransformPlayers;
        [Tooltip("The spawn points for each player. If the amount of players exceeds the amount of spawn points the players will spawn at the last spawn point index")]
        [SerializeField] private Transform[] playerSpawnPositions;

        [Tooltip("When a player gameobject gets instantiated")]
        public UnityEvent<GameObject> onPlayerInstantiate;
        [Tooltip("When the spawner is done spawning players")]
        public UnityEvent onFinishedSpawning;

        public UnityEvent<PlayerInput> onPlayerInputDeviceLost;
        public UnityEvent<PlayerInput> onPlayerInputDeviceRegained;
        public UnityEvent<PlayerInput> onPlayerInputControlsChanged;

        /// <summary>
        /// Keeps track of on what spawn point to spawn the next player
        /// </summary>
        private int playerSpawnPointIndex;
        /// <summary>
        /// Debug prefix for each debug message from this script
        /// </summary>
        private readonly string debugPrefix = "[CouchMultiplayerPlayerSpawner]";
        /// <summary>
        /// Unity action reference for spawning player
        /// </summary>
        private UnityAction<PlayerData> actionSpawnPlayer => SpawnPlayer;
        /// <summary>
        /// A list containing all of the active players
        /// </summary>
        private Dictionary<PlayerData, CouchMultiplayerPlayer> players = new Dictionary<PlayerData, CouchMultiplayerPlayer>();
        private Dictionary<PlayerInput, bool> playersDeviceStatus = new Dictionary<PlayerInput, bool>();
        private Coroutine coroutineSpawnAllPlayers;

        private void Awake()
        {
            // Check removing old players
            if(removePlayersOnAwake) ClearPlayersInScene();
            // Assign parent if null
            if(parentTransformPlayers == null) parentTransformPlayers = transform;
        }

        private void OnEnable()
        {
            if(CouchMultiplayerManager.Instance == null)
            {
                Debug.LogError($"{debugPrefix} No CouchMutliplayerManager script found! Make sure 1 instance of it is active in the scene");
                return;
            }

            // Attach to couchMultiplayerManager
            CouchMultiplayerManager.Instance.onAddPlayer.AddListener(actionSpawnPlayer);
        }

        private void OnDisable()
        {
            if(CouchMultiplayerManager.Instance != null)
            {
                // Detach to couchMultiplayerManager
                CouchMultiplayerManager.Instance.onAddPlayer.RemoveListener(actionSpawnPlayer);
            }
        }

        private void Start()
        {
            if(clearPlayerDataOnStart) CouchMultiplayerManager.Instance.ClearPlayers();
            if(playersToSpawnOnInitialized > 0)
            {
                InputDevice[] inputDevices = InputSystem.devices.ToArray();
                if(showDebug)
                {
                    foreach(var item in inputDevices)
                    {
                        Debug.Log($"{debugPrefix} Found inputDevice: {item.displayName}");
                    }
                }
                for(int i = 0; i < playersToSpawnOnInitialized; i++)
                {
                    // Check if input device available
                    if(i < inputDevices.Length)
                    {
                        if(inputDevices[i].displayName == "Mouse") continue;
                        CouchMultiplayerManager.Instance.AddPlayer(inputDevices[i]);
                    }
                    else
                    {
                        break;
                    }
                }
            }
            if(spawnOnStart) SpawnAllPlayers();
        }

        private void Update()
        {
            CheckPlayersDeviceStatus();
        }

        /// <summary>
        /// Clears all present player gameobjects in the scene
        /// </summary>
        public void ClearPlayersInScene()
        {
            CouchMultiplayerPlayer[] foundPlayers = FindObjectsOfType<CouchMultiplayerPlayer>();
            foreach(CouchMultiplayerPlayer player in foundPlayers)
            {
                if(player.gameObject != null) Destroy(player.gameObject);
            }

            players.Clear();
            playersDeviceStatus.Clear();
        }

        /// <summary>
        /// Clears the players in the manager and in scene
        /// </summary>
        public void ClearPlayersInSceneAndManager()
        {
            ClearPlayersInScene();
            CouchMultiplayerManager.Instance.ClearPlayers();
        }

        /// <summary>
        /// Spawn all players connected to couchMultiplayerManager
        /// </summary>
        public void SpawnAllPlayers()
        {
            if(coroutineSpawnAllPlayers != null) StopCoroutine(coroutineSpawnAllPlayers);
            coroutineSpawnAllPlayers = StartCoroutine(SpawnAllPlayersAsync());
        }

        /// <summary>
        /// Spawn a player gameobject in the scene
        /// </summary>
        /// <param name="playerData">Corresponding player data</param>
        public void SpawnPlayer(PlayerData playerData)
        {
            // Check if player is already added
            if(players.ContainsKey(playerData))
            {
                if(showDebug) Debug.Log($"{debugPrefix} Player with index {playerData.playerIndex} already added, ignoring...");
                return;
            }

            // Create player
            if(prefabPlayer == null)
            {
                Debug.LogError($"{debugPrefix} prefabPlayer has not been set in the inspector of couchMultiplayerPlayerSpawner! Make sure to set the prefabPlayer reference");
                return;
            }

            PlayerInput playerInput = PlayerInput.Instantiate(prefab: prefabPlayer, playerIndex: playerData.playerIndex, pairWithDevice: playerData.inputDevice);
            playerInput.onDeviceLost += x => onPlayerInputDeviceLost?.Invoke(x);
            playerInput.onDeviceRegained += x => onPlayerInputDeviceRegained?.Invoke(x);
            playerInput.onControlsChanged += x => onPlayerInputControlsChanged?.Invoke(x);

            playersDeviceStatus.Add(playerInput, playerData.inputDevice != null);

            GameObject a = playerInput.gameObject;
            Transform spawn = GetPlayerSpawnTransform();           
            a.transform.SetParent(spawn, false);

            if(onSpawnSetPrefabRBPosition)
            {
                Rigidbody rb = a.GetComponent<Rigidbody>();
                if(rb == null) rb = a.GetComponentInChildren<Rigidbody>();
                if(rb != null)
                {
                    rb.position = spawn.position;
                }
            }

            // Get / create cmp
            CouchMultiplayerPlayer cmp = a.GetComponent<CouchMultiplayerPlayer>();
            if(cmp == null) cmp = a.GetComponentInChildren<CouchMultiplayerPlayer>();
            if(cmp == null)
            {
                // No cmp component on player, add cmp component
                cmp = a.AddComponent<CouchMultiplayerPlayer>();
                if(showDebug) Debug.Log($"{debugPrefix} No CouchMultiplayerBase component found on player prefab! Adding component...");
            }
            
            // Initialize CouchMultiplayerPlayer
            cmp.Initialize(playerData);

            // Add to players
            players.Add(playerData, cmp);

            // Refresh cameras
            if(enableSplitScreen)
            {
                // Get display data for each player
                PlayerData[] playerDisplays = Display.GetPlayerDisplays(CouchMultiplayerManager.MaxDisplays, CouchMultiplayerManager.PlayerAmount, splitScreenMode);
                
                int pIndex = 0;
                foreach(var player in players) // ductape solution but works
                {
                    player.Value.PlayerData.cameraTargetDisplay = playerDisplays[pIndex].cameraTargetDisplay;
                    player.Value.PlayerData.cameraViewPortRect = playerDisplays[pIndex].cameraViewPortRect;
                    player.Value.PlayerData.splitScreenMode = splitScreenMode;
                    player.Value.RefreshCamera();
                    pIndex++;
                }
            }

            onPlayerInstantiate?.Invoke(a);
            if(showDebug) Debug.Log($"{debugPrefix} Spawned player with playerIndex {playerData.playerIndex}");
        }

        /// <summary>
        /// Returns the current player spawn position
        /// </summary>
        /// <returns>Vector3 position</returns>
        private Transform GetPlayerSpawnTransform()
        {
            // If spawn positions aren't set
            if(playerSpawnPositions == null || playerSpawnPositions.Length == 0)
            {
                if(showDebug) Debug.Log($"{debugPrefix} No spawn positions available for player! Spawning player on {gameObject.name}...");
                return transform;
            }

            // Get next iteraction of spawnPoint
            int i = playerSpawnPointIndex;
            // Check if we can increase it for next iteration
            if(playerSpawnPointIndex < playerSpawnPositions.Length - 1)
            {
                playerSpawnPointIndex++;
            }
            return playerSpawnPositions[i];
        }

        private IEnumerator SpawnAllPlayersAsync()
        {
            PlayerData[] playerDatas = CouchMultiplayerManager.PlayerDatas;
            if(orderSpawningOnLobbyID)
            {
                playerDatas = playerDatas.OrderBy(x => x.lobbyIndex).ToArray();
            }

            foreach(PlayerData playerData in playerDatas)
            {
                SpawnPlayer(playerData);
            }
            // Wait for gameobject to be created
            yield return null;
            onFinishedSpawning?.Invoke();
            yield break;
        }

        private void CheckPlayersDeviceStatus()
        {
            var playerDevices = playersDeviceStatus.ToArray();

            for(int i = 0; i < playerDevices.Length; i++)
            {
                var item = playerDevices[i];
                if(item.Key.devices.Count <= 0 && item.Value)
                {
                    if(showDebug) Debug.Log($"{debugPrefix} Device lost of player {item.Key.playerIndex}");

                    playersDeviceStatus[item.Key] = false;
                    onPlayerInputDeviceLost?.Invoke(item.Key);
                }
                else if(item.Key.devices.Count > 0 && !item.Value)
                {
                    if(showDebug) Debug.Log($"{debugPrefix} Device regained of player {item.Key.playerIndex}");

                    playersDeviceStatus[item.Key] = true;
                    onPlayerInputDeviceRegained?.Invoke(item.Key);
                }
            }
        }
    }
}

