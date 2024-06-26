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
        public Camera cameraOverlay;
        public LayerMask layerMaskCamera;
        public LayerMask layerMaskCameraOverlay;

        public GameObjectItem[] firstPersonGameObjects;
        public GameObjectItem[] thirdPersonGameObjects;

        public override string DebugPrefix() => "[CouchMultiplayerPlayerFP]";

        public override void RefreshCamera()
        {
            base.RefreshCamera();

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

        [System.Serializable]
        public class GameObjectItem
        { 
            public GameObject item;
            public bool setLayerRecursively;
        }
    }
}
