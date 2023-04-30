using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SLIDDES.Multiplayer.Couch
{
    public static class LayerMaskExtensions
    {
        public static bool HasLayer(this LayerMask layerMask, int layer)
        {
            if(layerMask == (layerMask | (1 << layer)))
            {
                return true;
            }

            return false;
        }

        public static bool[] HasLayers(this LayerMask layerMask)
        {
            var hasLayers = new bool[32];

            for(int i = 0; i < 32; i++)
            {
                if(layerMask == (layerMask | (1 << i)))
                {
                    hasLayers[i] = true;
                }
            }

            return hasLayers;
        }

        public static int[] IncludedLayers(this LayerMask layerMask)
        {
            List<int> layers = new List<int>();
            for(int i = 0; i < 32; i++)
            {
                if(layerMask == (layerMask | (1 << i)))
                {
                    layers.Add(i);
                }
            }
            return layers.ToArray();
        }

        /// <summary>
        /// Set the layer & the children layers
        /// </summary>
        /// <param name="transform">The transform to set the layer</param>
        /// <param name="layer">The new layer of the transform & its children</param>
        public static void SetLayerRecursively(this Transform transform, int layer)
        {
            transform.gameObject.layer = layer;
            foreach(Transform child in transform)
            {
                child.SetLayerRecursively(layer);
            }
        }
    }
}
