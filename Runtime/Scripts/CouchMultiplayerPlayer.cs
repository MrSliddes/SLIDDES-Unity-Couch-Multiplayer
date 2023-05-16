using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Users;

namespace SLIDDES.Multiplayer.Couch
{
    /// <summary>
    /// Contains information about a player for couch mutliplayer
    /// </summary>
    [AddComponentMenu("SLIDDES/Multiplayer/Couch/Couch Multiplayer Player")]
    public class CouchMultiplayerPlayer : CouchMultiplayerPlayerBase
    {        
        [Tooltip("The camera of the player")]
        public new Camera camera;
        [Tooltip("The camera that renders the UI")]
        public Camera cameraUI;
                        
        public override string DebugPrefix() => "[CouchMultiplayerPlayer]";
               
    }
}
