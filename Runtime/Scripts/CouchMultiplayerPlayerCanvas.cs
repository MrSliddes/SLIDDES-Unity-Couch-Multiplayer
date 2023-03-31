using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace SLIDDES.Multiplayer.Couch
{
    public class CouchMultiplayerPlayerCanvas : MonoBehaviour
    {
        public Components components;
        public Events events; 

        // Start is called before the first frame update
        void Start()
        {
        
        }

        // Update is called once per frame
        void Update()
        {
        
        }

        [System.Serializable]
        public class Components
        {
            [Tooltip("The corresponding player gameobject of this canvas. Assigned by CouchMultiplayerCanvasManager")]
            public GameObject player;
        }

        [System.Serializable]
        public class Events
        {
            public UnityEvent onInitialize;
        }
    }
}
