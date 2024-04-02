using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace SLIDDES.Multiplayer.Couch
{
    public class CouchMultiplayerPlayerLobby : MonoBehaviour
    {
        public static CouchMultiplayerPlayerLobby ActiveLobby;
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

        [SerializeField] private bool soloPlayerInput;
        [SerializeField] private bool canStartLobby = true;
        [SerializeField] private int minimumPlayersRequired = 1;
        [SerializeField] private bool showDebug;
        [SerializeField] private InputCallback[] inputCallbacks;

        public UnityEvent<PlayerInput[]> onJoinedPlayersChanged;
        public UnityEvent<PlayerInput> onPlayerJoined;
        public UnityEvent<PlayerInput> onPlayerLeave;
        public UnityEvent onLobbyStart;
        public UnityEvent onLobbyClear;

        private readonly string debugPrefix = "[CMPlayerLobby]";
        private PlayerInput activePlayerInput;
        private List<PlayerInput> joinedPlayers = new List<PlayerInput>();
        private Dictionary<string, InputCallback> inputCallbacksDictionary = new Dictionary<string, InputCallback>();

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

        public void JoinLobby(PlayerInput playerInput)
        {
            // Check if player isnt already in
            if(joinedPlayers.Contains(playerInput))
            {
                // Player already joined
                return;
            }

            if(soloPlayerInput && activePlayerInput == null)
            {
                ActivePlayerInput = playerInput;
            }

            if(showDebug) Debug.Log($"{debugPrefix} Player {playerInput.playerIndex} joined lobby");
            joinedPlayers.Add(playerInput);
            onPlayerJoined?.Invoke(playerInput);
            onJoinedPlayersChanged?.Invoke(joinedPlayers.ToArray());
        }

        public void LeaveLobby(PlayerInput playerInput) 
        { 
            if(!joinedPlayers.Contains(playerInput))
            {
                // Player never joined
                return;
            }

            if(soloPlayerInput && ActivePlayerInput == playerInput)
            {
                ActivePlayerInput = null;
            }

            if(showDebug) Debug.Log($"{debugPrefix} Player {playerInput.playerIndex} leaved lobby");
            joinedPlayers.Remove(playerInput);
            onPlayerLeave?.Invoke(playerInput);
            onJoinedPlayersChanged?.Invoke(joinedPlayers.ToArray());
        }

        public void ClearLobby()
        {
            foreach(PlayerInput playerInput in joinedPlayers)
            {
                onPlayerLeave?.Invoke(playerInput);
            }
            joinedPlayers.Clear();
            onJoinedPlayersChanged?.Invoke(joinedPlayers.ToArray());
            onLobbyClear?.Invoke();
        }

        public void StartLobby()
        {
            if(!CanStartLobby) return;

            onLobbyStart?.Invoke();
        }

        public void ReceiveInput(InputAction.CallbackContext context, PlayerInput playerInput)
        {
            if(!joinedPlayers.Contains(playerInput)) return;
            if(soloPlayerInput)
            {
                if(ActivePlayerInput != playerInput) return;
            }

            string key = context.action.name;
            if(showDebug) Debug.Log($"{debugPrefix} Receive Input: {key}");

            if(!inputCallbacksDictionary.ContainsKey(key)) return;
            inputCallbacksDictionary[key].onCallbackContext?.Invoke(context);
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

        [System.Serializable]
        public class InputCallback
        {
            public string inputActionName;
            public UnityEvent<InputAction.CallbackContext> onCallbackContext;
        }
    }
}
