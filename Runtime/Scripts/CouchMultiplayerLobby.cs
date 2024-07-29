using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace SLIDDES.Multiplayer.Couch
{
    public class CouchMultiplayerLobby : MonoBehaviour
    {
        public static CouchMultiplayerLobby ActiveLobby { get; private set; }

        public bool CanStartLobby
        {
            get
            {
                return canStartLobby && joinedPlayers.Count >= minimumPlayersRequired;
            }
            set
            {
                canStartLobby = value;
            }
        }
        public bool IsStartingLobby
        {
            get
            {
                return isStartingLobby;
            }
        }
        public PlayerInput ActivePlayerInput
        {
            get
            {
                return activePlayerInput;
            }
            set
            {
                activePlayerInput = value;
            }
        }

        public PlayerInput[] JoinedPlayers => joinedPlayers.ToArray();

        [SerializeField] private bool oneAtATimePlayerInput;
        [SerializeField] private bool canStartLobby = true;
        [SerializeField] private int minimumPlayersRequired = 1;
        [Space]
        [SerializeField] private StartingMode startingMode;
        [SerializeField] private float startingTime = 0;
        [SerializeField] private bool assignLobbyIDOnLobbyStart = true;
        [Space]
        [SerializeField] private bool showDebug;
        [SerializeField] private InputCallback[] inputCallbacks;

        public UnityEvent<PlayerInput[]> onJoinedPlayersChanged;
        public UnityEvent<PlayerInput> onPlayerJoined;
        public UnityEvent<PlayerInput> onPlayerLeave;
        public UnityEvent onLobbyStarting;
        public UnityEvent<float> onLobbyStartingTimer;
        public UnityEvent onLobbyCancelStarting;
        public UnityEvent onLobbyStart;
        public UnityEvent onLobbyClear;

        private readonly string debugPrefix = "[CMPlayerLobby]";
        private bool isStartingLobby;
        private PlayerInput activePlayerInput;
        private List<PlayerInput> joinedPlayers = new List<PlayerInput>();
        private Dictionary<string, InputCallback> inputCallbacksDictionary = new Dictionary<string, InputCallback>();
        private Coroutine coroutineStartingLobbyAsync;
        /// <summary>
        /// The input device that is starting the lobby
        /// </summary>
        private InputDevice startingLobbyInputDevice;

        private void Awake()
        {
            for(int i = 0; i < inputCallbacks.Length; i++)
            {
                inputCallbacksDictionary.Add(inputCallbacks[i].inputActionName, inputCallbacks[i]);
            }
        }

        private void OnEnable()
        {
            ActiveLobby = this;
        }

        private void OnDisable()
        {
            if(ActiveLobby == this)
            {
                ActiveLobby = null;
            }
        }

        public void AddPlayer(PlayerInput playerInput)
        {
            if(playerInput == null)
            {
                Debug.LogError($"{debugPrefix} AddPlayer playerInput is null");
                return;
            }

            if(isStartingLobby) return;

            // Check if player isnt already in
            if(joinedPlayers.Contains(playerInput))
            {
                // Player already joined
                return;
            }

            if(oneAtATimePlayerInput && activePlayerInput == null)
            {
                ActivePlayerInput = playerInput;
            }

            if(showDebug) Debug.Log($"{debugPrefix} Player {playerInput.playerIndex} joined lobby");

            joinedPlayers.Add(playerInput);
            onPlayerJoined?.Invoke(playerInput);
            onJoinedPlayersChanged?.Invoke(joinedPlayers.ToArray());
        }

        public void RemovePlayer(PlayerInput playerInput)
        {
            if(isStartingLobby) return;

            if(!joinedPlayers.Contains(playerInput))
            {
                // Player never joined
                return;
            }

            if(oneAtATimePlayerInput && ActivePlayerInput == playerInput)
            {
                ActivePlayerInput = null;
            }

            if(showDebug) Debug.Log($"{debugPrefix} Player {playerInput.playerIndex} leaved lobby");

            joinedPlayers.Remove(playerInput);
            onPlayerLeave?.Invoke(playerInput);
            onJoinedPlayersChanged?.Invoke(joinedPlayers.ToArray());
        }

        public void StartingLobby(InputAction.CallbackContext context)
        {
            if(!CanStartLobby) return;

            switch(startingMode)
            {
                case StartingMode.Instant:
                    isStartingLobby = true;
                    onLobbyStarting?.Invoke();
                    StartLobby();
                    break;
                case StartingMode.HoldToStart:
                    // Only allow input device that started to perform / cancel the starting of lobby
                    if(startingLobbyInputDevice == null)
                    {
                        startingLobbyInputDevice = context.control.device;
                    }
                    else if(startingLobbyInputDevice != context.control.device)
                    {
                        return;
                    }

                    if(context.performed)
                    {
                        if(coroutineStartingLobbyAsync != null) StopCoroutine(coroutineStartingLobbyAsync);
                        coroutineStartingLobbyAsync = StartCoroutine(StartingLobbyAsync());
                    }
                    else if(context.canceled)
                    {
                        CancelStartingLobby(context);
                    }
                    break;
                case StartingMode.CountDown:
                    if(coroutineStartingLobbyAsync == null)
                    {
                        coroutineStartingLobbyAsync = StartCoroutine(StartingLobbyAsync());
                    }
                    break;
                default:
                    break;
            }
        }

        public void CancelStartingLobby(InputAction.CallbackContext context)
        {
            if(coroutineStartingLobbyAsync != null)
            {
                StopCoroutine(coroutineStartingLobbyAsync);
                coroutineStartingLobbyAsync = null;
                isStartingLobby = false;
                startingLobbyInputDevice = null;
                onLobbyCancelStarting?.Invoke();
            }
        }

        public void StartLobby()
        {
            if(!CanStartLobby) return;

            if(assignLobbyIDOnLobbyStart) AssignLobbyIDsToPlayers();

            onLobbyStart?.Invoke();
        }

        public void ReceiveInputFromPlayer(PlayerInput playerInput, InputAction.CallbackContext context)
        {
            if(!joinedPlayers.Contains(playerInput)) return;
            if(oneAtATimePlayerInput)
            {
                if(ActivePlayerInput != playerInput) return;
            }

            string key = context.action.name;

            if(showDebug) Debug.Log($"{debugPrefix} Receive Input: {key}");

            if(!inputCallbacksDictionary.ContainsKey(key)) return;

            inputCallbacksDictionary[key].onCallbackContext?.Invoke(context);
        }

        public void ClearLobby()
        {
            foreach(PlayerInput playerInput in joinedPlayers)
            {
                onPlayerLeave?.Invoke(playerInput);
            }

            startingLobbyInputDevice = null;
            ActivePlayerInput = null;
            joinedPlayers.Clear();

            onJoinedPlayersChanged?.Invoke(joinedPlayers.ToArray());
            onLobbyClear?.Invoke();
        }

        public void AssignLobbyIDsToPlayers()
        {
            for(int i = 0; i < joinedPlayers.Count; i++)
            {
                PlayerData playerData = CouchMultiplayerManager.Instance.GetPlayerData(joinedPlayers[i]);
                if(playerData != null)
                {
                    playerData.lobbyIndex = i;
                }
            }
        }

        public void NextActivePlayerInput(bool loopAround = false)
        {
            if(ActivePlayerInput == null) return;

            int index = joinedPlayers.IndexOf(ActivePlayerInput);
            index++;
            if(index >= joinedPlayers.Count)
            {
                if(loopAround)
                {
                    index = 0;
                }
                else
                {
                    ActivePlayerInput = null;
                    return;
                }
            }
            ActivePlayerInput = joinedPlayers[index];
        }

        public void PreviousActivePlayerInput(bool loopAround = false)
        {
            if(ActivePlayerInput == null) return;

            int index = joinedPlayers.IndexOf(ActivePlayerInput);
            index--;
            if(index <= 0)
            {
                if(loopAround)
                {
                    index = joinedPlayers.Count - 1;
                }
                else
                {
                    ActivePlayerInput = null;
                    return;
                }
            }
            ActivePlayerInput = joinedPlayers[index];
        }

        private IEnumerator StartingLobbyAsync()
        {
            isStartingLobby = true;
            onLobbyStarting?.Invoke();

            float time = startingTime;
            while(time > 0)
            {
                yield return null;
                time -= Time.unscaledDeltaTime;
                onLobbyStartingTimer?.Invoke(time);
            }

            coroutineStartingLobbyAsync = null;
            StartLobby();
            yield break;
        }

        [System.Serializable]
        public class InputCallback
        {
            public string inputActionName;
            public UnityEvent<InputAction.CallbackContext> onCallbackContext;
        }

        [System.Serializable]
        public enum StartingMode
        {
            Instant,
            HoldToStart,
            CountDown
        }
    }
}
