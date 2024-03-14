using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SLIDDES.Multiplayer.Couch
{
    public class CouchMultiplayerManagerHandler : MonoBehaviour
    {
        [Tooltip("Allow couch mutliplayer joining in this scene?")]
        public bool allowJoining = true;

        private void OnEnable()
        {
            CouchMultiplayerManager.PlayersCanJoin = allowJoining;
        }
    }
}
