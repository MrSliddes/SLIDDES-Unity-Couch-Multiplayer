using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;

namespace SLIDDES.Multiplayer.Couch
{
    /// <summary>
    /// Contains information about a player for couch mutliplayer
    /// </summary>
    [AddComponentMenu("SLIDDES/Multiplayer/Couch/Couch Multiplayer Player")]
    [RequireComponent(typeof(PlayerInput))]
    [RequireComponent(typeof(MultiplayerEventSystem))]
    [RequireComponent(typeof(InputSystemUIInputModule))]
    public class CouchMultiplayerPlayer : MonoBehaviour
    {
        public PlayerData PlayerData { get; private set; }
        public Camera Camera { get; private set; }
        public Camera CameraUI
        {
            get
            {
                return cameraUI;
            }
            set
            {
                cameraUI = value;
            }
        }
        public PlayerInput PlayerInput { get; private set; }
        public MultiplayerEventSystem MultiplayerEventSystem { get; private set; }

        public UnityAction onInitialized;

        [Tooltip("Reference to the player camera that holds the UI. (Optional)")]
        [SerializeField] private Camera cameraUI;

        // First person
        [Tooltip("Overlay camera of the player (think of a camera that renders the player weapon for example")]
        [SerializeField] private Camera cameraOverlay;
        [SerializeField] private LayerMask layerMaskCamera;
        [SerializeField] private LayerMask layerMaskCameraOverlay;
        [SerializeField] private GameObjectItem[] firstPersonGameObjects;
        [SerializeField] private GameObjectItem[] thirdPersonGameObjects;

        // Lobby
        [SerializeField] private bool canJoinLobby = true;
        [SerializeField] private bool canLeaveLobby = true;
        [SerializeField] private bool canStartLobby = true;
        [SerializeField] private bool canSendInputToLobby = true;

        private void Awake()
        {
            PlayerInput = GetComponentInChildren<PlayerInput>();
            if(PlayerInput == null)
            {
                Debug.LogError($"{DebugPrefix()} playerInput is not assigned. Please assign the component reference");
            }

            MultiplayerEventSystem = GetComponentInChildren<MultiplayerEventSystem>();
        }

        /// <summary>
        /// Initialize the player with the playerData
        /// </summary>
        /// <param name="playerData"></param>
        /// <param name="spawner"></param>
        public virtual void Initialize(PlayerData playerData)
        {
            PlayerData = playerData;

            // Set auto switch
            if(CouchMultiplayerSettings.MaxDevices == 1)
            {
                PlayerInput.neverAutoSwitchControlSchemes = !CouchMultiplayerSettings.AllowSinglePlayerDeviceSwitch;
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

        public virtual string DebugPrefix() => "[CouchMultiplayerPlayer]";

        /// <summary>
        /// Refreshes the camera with playerData camera values
        /// </summary>
        public virtual void RefreshCamera()
        {
            // First person
            if(cameraOverlay != null)
            {
                GetComponent<Camera>().rect = PlayerData.cameraViewPortRect;
                GetComponent<Camera>().targetDisplay = PlayerData.cameraTargetDisplay;
                cameraOverlay.rect = GetComponent<Camera>().rect;
                cameraOverlay.targetDisplay = GetComponent<Camera>().targetDisplay;

                // Set camera cullingMask, by turning bit off
                int[] includedLayers = layerMaskCamera.IncludedLayers();
                int cameraLayer = includedLayers[PlayerData.playerIndex];
                GetComponent<Camera>().cullingMask &= ~(1 << cameraLayer); // turn off bit

                // Set camera overlay layer, by turning bit on
                int[] includedOverlayLayers = layerMaskCameraOverlay.IncludedLayers();
                int cameraOverlayLayer = includedOverlayLayers[PlayerData.playerIndex];
                cameraOverlay.cullingMask |= 1 << cameraOverlayLayer; // turn on bit

                // First person gameobjects
                foreach(var item in firstPersonGameObjects)
                {
                    if(item.setLayerRecursively) item.item.transform.SetLayerRecursively(cameraOverlayLayer); else item.item.layer = cameraOverlayLayer;
                }

                // Third person gameobjects
                foreach(var item in thirdPersonGameObjects)
                {
                    if(item.setLayerRecursively) item.item.transform.SetLayerRecursively(cameraLayer); else item.item.layer = cameraLayer;
                }
            }
        }

        #region Lobby

        public void JoinLobby(InputAction.CallbackContext context)
        {
            if(!canJoinLobby) return;

            if(context.canceled && CouchMultiplayerLobby.ActiveLobby != null)
            {
                CouchMultiplayerLobby.ActiveLobby.AddPlayer(PlayerInput);
            }
        }

        public void LeaveLobby(InputAction.CallbackContext context)
        {
            if(!canLeaveLobby) return;

            if(context.canceled && CouchMultiplayerLobby.ActiveLobby != null)
            {
                CouchMultiplayerLobby.ActiveLobby.RemovePlayer(PlayerInput);
            }
        }

        public void StartingLobby(InputAction.CallbackContext context)
        {
            if(!canStartLobby) return;

            if(context.canceled && CouchMultiplayerLobby.ActiveLobby != null)
            {
                CouchMultiplayerLobby.ActiveLobby.StartingLobby(context);
            }
        }

        public void StartLobby(InputAction.CallbackContext context)
        {
            if(!canStartLobby) return;

            if(context.canceled && CouchMultiplayerLobby.ActiveLobby != null)
            {
                CouchMultiplayerLobby.ActiveLobby.StartLobby();
            }
        }

        public void SendInputToLobby(InputAction.CallbackContext context)
        {
            if(canSendInputToLobby) return;

            if(context.canceled && CouchMultiplayerLobby.ActiveLobby != null)
            {
                CouchMultiplayerLobby.ActiveLobby.ReceiveInputFromPlayer(PlayerInput, context);
            }
        }

        #endregion

        [System.Serializable]
        public class GameObjectItem
        {
            public GameObject item;
            public bool setLayerRecursively;
        }
    }
}
