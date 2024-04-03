using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SLIDDES.Multiplayer.Couch
{
    public class CouchMultiplayerManagerHandler : MonoBehaviour
    {
        public bool OnEnableSetAllowJoining
        { 
            get
            {
                return onEnableSetAllowJoining;
            }
            set
            {
                onEnableSetAllowJoining = value;
            }
        }
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

        [SerializeReference] private bool onEnableSetAllowJoining;
        [Tooltip("Allow couch mutliplayer joining in this scene?")]
        [SerializeReference] bool allowJoining = true;

        private void OnEnable()
        {
            if(onEnableSetAllowJoining)
            {
                CouchMultiplayerManager.PlayersCanJoin = allowJoining;
            }
        }

        public void ClearPlayers()
        {
            if(CouchMultiplayerManager.Instance == null)
            {
                Debug.LogWarning("CouchMultiplayerManager Instance is null");
                return;
            }

            CouchMultiplayerManager.Instance.ClearPlayers();
        }
    }
}
