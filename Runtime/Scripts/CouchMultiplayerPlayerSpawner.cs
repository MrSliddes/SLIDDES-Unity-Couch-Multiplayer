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
        public CouchMultiplayerPlayerBase[] Players => players.Values.ToArray();

        [Header("Values")]
        [Tooltip("Remove players that are already present in the scene for cleanup before spawning players")]
        [SerializeField] private bool removePlayersOnAwake = true;
        [Tooltip("Clear the playerData stored in manager on start")]
        [SerializeField] private bool clearPlayerDataOnStart;
        [Tooltip("Spawn the connected players (from CouchMutliplayerManager) on start")]
        [SerializeField] private bool spawnOnStart;
        [Tooltip("The amount of players to spawn once this spawner has initalized")]
        [SerializeField] private int playersToSpawnOnInitialized;

        [Header("Split-Screen")]
        [SerializeField] private bool enableSplitScreen;

        [Header("Debug")]
        [Tooltip("Show debug messages from this script")]
        [SerializeField] private bool showDebug;

        [Header("Components")]
        [Tooltip("The prefab of the player to be spawned")]
        [SerializeField] private GameObject prefabPlayer;
        [Tooltip("The parent transform where all instantiated players will be parented under. If left null this gameObject transform will be assigned as parent")]
        [SerializeField] private Transform parentTransformPlayers;
        [Tooltip("The spawn points for each player. If the amount of players exceeds the amount of spawn points the players will spawn at the last spawn point index")]
        [SerializeField] private Transform[] playerSpawnPositions;

        [Tooltip("When a player gameobject gets instantiated")]
        public UnityEvent<GameObject> onPlayerInstantiate;
        [Tooltip("When the spawner is done spawning players")]
        public UnityEvent onFinishedSpawning;

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
        private Dictionary<PlayerData, CouchMultiplayerPlayerBase> players = new Dictionary<PlayerData, CouchMultiplayerPlayerBase>();
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


        /// <summary>
        /// Clears all present player gameobjects in the scene
        /// </summary>
        public void ClearPlayersInScene()
        {
            CouchMultiplayerPlayerBase[] foundPlayers = FindObjectsOfType<CouchMultiplayerPlayerBase>();
            foreach(CouchMultiplayerPlayerBase player in foundPlayers)
            {
                if(player.gameObject != null) Destroy(player.gameObject);
            }

            players.Clear();
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
            Transform spawn = GetPlayerSpawnTransform();
            //GameObject a = Instantiate(prefabPlayer, spawn.position, spawn.rotation);            
            PlayerInput playerInput = PlayerInput.Instantiate(prefabPlayer, playerData.playerIndex, "", -1, pairWithDevice: playerData.inputDevice);
            GameObject a = playerInput.gameObject;
            a.transform.SetParent(spawn, false);

            // Get / create cmp
            CouchMultiplayerPlayerBase cmpb = a.GetComponent<CouchMultiplayerPlayerBase>();
            if(cmpb == null) cmpb = a.GetComponentInChildren<CouchMultiplayerPlayerBase>();
            if(cmpb == null)
            {
                // No cmp component on player, add cmp component
                cmpb = a.AddComponent<CouchMultiplayerPlayerBase>();
                if(showDebug) Debug.Log($"{debugPrefix} No CouchMultiplayerBase component found on player prefab! Adding component...");
            }
            
            // Initialize CouchMultiplayerPlayer
            cmpb.Initialize(playerData);

            // Add to players
            players.Add(playerData, cmpb);

            // Refresh cameras
            if(enableSplitScreen)
            {
                // Get display data for each player
                PlayerData[] playerDisplays = Display.GetPlayerDisplays(CouchMultiplayerManager.MaxDisplays, CouchMultiplayerManager.PlayerAmount);
                
                int pIndex = 0;
                foreach(var player in players) // ductape solution but works
                {
                    player.Value.playerData.cameraTargetDisplay = playerDisplays[pIndex].cameraTargetDisplay;
                    player.Value.playerData.cameraViewPortRect = playerDisplays[pIndex].cameraViewPortRect;
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
            foreach(PlayerData playerData in CouchMultiplayerManager.PlayerDatas)
            {
                SpawnPlayer(playerData);
            }
            // Wait for gameobject to be created
            yield return null;
            onFinishedSpawning?.Invoke();
            yield break;
        }
    }
}

