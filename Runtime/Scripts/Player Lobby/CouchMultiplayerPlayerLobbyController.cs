using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

namespace SLIDDES.Multiplayer.Couch
{
    public class CouchMultiplayerPlayerLobbyController : MonoBehaviour
    {
        public bool AllowJoining
        { 
            get
            {
                return allowJoining;
            }
            set
            {
                allowJoining = value;
            }
        }
        public bool AllowLeaving
        {
            get
            {
                return allowLeaving;
            }
            set
            {
                allowLeaving = value;
            }
        }
        public bool AllowInputSending
        { 
            get
            {
                return allowInputSending;
            }
            set
            {
                allowInputSending = value;
            }
        }
        public bool AllowStarting
        {
            get
            {
                return allowStarting;
            }
            set
            {
                allowStarting = value;
            }
        }

        [SerializeField] private bool allowJoining = true;
        [SerializeField] private bool allowLeaving = true;
        [SerializeField] private bool allowInputSending = true;
        [SerializeField] private bool allowStarting = true;
        [SerializeField] private PlayerInput playerInput;

        private CouchMultiplayerPlayerLobby ActiveLobby => CouchMultiplayerPlayerLobby.ActiveLobby;

        public void JoinLobby(InputAction.CallbackContext context)
        {
            if(ActiveLobby == null) return;
            if(!AllowJoining) return;

            if(context.canceled)
            {
                ActiveLobby.JoinLobby(playerInput);
            }
        }

        public void LeaveLobby(InputAction.CallbackContext context)
        {
            if(ActiveLobby == null) return;
            if(!AllowLeaving) return;

            if(context.canceled)
            {
                ActiveLobby.LeaveLobby(playerInput);
            }
        }

        public void StartingLobby(InputAction.CallbackContext context)
        {
            if(ActiveLobby == null) return;
            if(!AllowStarting) return;

            ActiveLobby.StartingLobby(context);
        }

        public void StartLobby(InputAction.CallbackContext context)
        {
            if(ActiveLobby == null) return;
            if(!AllowStarting) return;

            if(context.canceled)
            {
                ActiveLobby.StartLobby();
            }
        }

        public void SendInput(InputAction.CallbackContext context)
        {
            if(ActiveLobby == null) return;
            if(!AllowInputSending) return;

            ActiveLobby.ReceiveInput(context, playerInput);
        }
    }
}
