using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace SLIDDES.Multiplayer.Couch
{
    [AddComponentMenu("SLIDDES/Multiplayer/Couch/Couch Multiplayer Player Canvas")]
    [RequireComponent(typeof(Canvas))]
    public class CouchMultiplayerPlayerCanvas : MonoBehaviour
    {
        public Canvas Canvas => canvas;
        public CanvasScaler CanvasScaler => canvasScaler;

        [Tooltip("The corresponding player component of this canvas. Assigned by CouchMultiplayerCanvasManager")]
        private CouchMultiplayerPlayer player;
        [Tooltip("The canvas belonging to this player")]
        private Canvas canvas;
        private CanvasScaler canvasScaler;

        public UnityEvent<GameObject> onInitialized;

        private void Awake()
        {
            canvas = GetComponent<Canvas>();
            canvasScaler = GetComponent<CanvasScaler>();
        }

        public void Initialize(CouchMultiplayerPlayer player)
        {
            this.player = player;
            this.canvas.worldCamera = player.CameraUI;

            onInitialized?.Invoke(player.gameObject);
        }
    }
}
