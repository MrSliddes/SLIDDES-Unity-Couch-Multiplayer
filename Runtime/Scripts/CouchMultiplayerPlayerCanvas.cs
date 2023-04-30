using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace SLIDDES.Multiplayer.Couch
{
    [AddComponentMenu("SLIDDES/Multiplayer/Couch/Player Canvas")]
    public class CouchMultiplayerPlayerCanvas : MonoBehaviour
    {
        public Components components;
        public Events events; 

        public void Initialize(CouchMultiplayerPlayer player)
        {
            components.player = player;
            components.canvas.worldCamera = player.camera;

            events.onInitialized.Invoke();
        }

        [System.Serializable]
        public class Components
        {
            [Tooltip("The corresponding player component of this canvas. Assigned by CouchMultiplayerCanvasManager")]
            public CouchMultiplayerPlayer player;
            [Tooltip("The canvas belonging to this player")]
            public Canvas canvas;
        }

        [System.Serializable]
        public class Events
        {
            [Tooltip("Triggerd when the canvas is initialized")]
            public UnityEvent onInitialized;
        }
    }
}
