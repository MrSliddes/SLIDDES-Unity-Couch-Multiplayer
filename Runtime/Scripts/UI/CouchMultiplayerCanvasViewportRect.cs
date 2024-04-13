using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SLIDDES.Multiplayer.Couch.UI;

namespace SLIDDES.Multiplayer.Couch
{
    /// <summary>
    /// This component can be used to scale a canvas to its camera viewport rect without using sepperate cameras
    /// </summary>
    [AddComponentMenu("SLIDDES/Multiplayer/Couch/Couch Multiplayer Canvas Viewport Rect")]
    [RequireComponent(typeof(RectTransform))]
    public class CouchMultiplayerCanvasViewportRect : MonoBehaviour
    {
        [SerializeField] private RectTransform scalerTransformRect;

        private PlayerData PlayerData;
        private CouchMultiplayerPlayerCanvas playerCanvas;
        private Rect viewportRect;
        private RectTransform rectTransform;

        public void Initialize(CouchMultiplayerPlayerCanvas playerCanvas, PlayerData playerData)
        {
            this.playerCanvas = playerCanvas;
            this.PlayerData = playerData;
            this.viewportRect = playerData.cameraViewPortRect;

            rectTransform = GetComponent<RectTransform>();

            rectTransform.anchorMin = this.viewportRect.min;
            rectTransform.anchorMax = this.viewportRect.max;
            rectTransform.SetRect(0, 0, 0, 0);

            if(scalerTransformRect != null )
            {
                float scale = getScale(CouchMultiplayerManager.PlayerDatas.Length);
                scalerTransformRect.localScale = Vector3.one * scale;

                float width = rectTransform.rect.width * scale;
                float height = rectTransform.rect.height * scale;

                scalerTransformRect.SetRect(-height, -height, -width, -width);
            }
        }

        private float getScale(int viewportRectsAmount)
        {
            if(viewportRectsAmount == 1) return 1;
            if(viewportRectsAmount >= 2 && viewportRectsAmount <= 4)
            {
                return 0.5f;
            }
            if(viewportRectsAmount >= 5 && viewportRectsAmount <= 8)
            {
                return 0.25f;
            }
            return 0.125f;
        }
    }
}
