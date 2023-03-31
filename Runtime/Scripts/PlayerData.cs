using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace SLIDDES.Multiplayer.Couch
{
    /// <summary>
    /// Contains data relevant for use in couch multiplayer
    /// </summary>
    [System.Serializable]
    public partial class PlayerData
    {
        [Header("Values")]
        [Tooltip("The index number of this player")]
        public int playerIndex = -1;
        [Tooltip("The index number of the display")]
        public int cameraTargetDisplay = -1;
        [Tooltip("The viewport rect of the camera")]
        public Rect cameraViewPortRect = new Rect(-1, -1, -1, -1);       

        /// <summary>
        /// The corresponding input device of this player
        /// </summary>
        public InputDevice inputDevice;
    }
}
