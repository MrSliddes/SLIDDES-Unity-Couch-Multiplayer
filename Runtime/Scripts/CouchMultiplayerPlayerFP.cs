using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SLIDDES.Multiplayer.Couch
{
    /// <summary>
    /// Extention class for a first person player
    /// </summary>
    public class CouchMultiplayerPlayerFP : CouchMultiplayerPlayer
    {
        public new Camera camera;
        public Camera cameraOverlay;
        public GameObject cameraCineMachine;
        public LayerMask layerMaskCamera;
        public LayerMask layerMaskCameraOverlay;

        public GameObjectItem[] firstPersonGameObjects;
        public GameObjectItem[] thirdPersonGameObjects;

        public override void RefreshCamera()
        {
            base.RefreshCamera();
        }

        [System.Serializable]
        public class GameObjectItem
        { 
            public GameObject item;
            public bool setLayerRecursively;
        }
    }
}
