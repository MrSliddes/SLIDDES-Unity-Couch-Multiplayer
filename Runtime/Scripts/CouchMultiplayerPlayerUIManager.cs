using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.UI;

namespace SLIDDES.Multiplayer.Couch
{
    /// <summary>
    /// Manages all playerUI input
    /// </summary>
    public class CouchMultiplayerPlayerUIManager : MonoBehaviour
    {
        public Values values;
        public Components components;

        /// <summary>
        /// Collection of all Player UIs connected
        /// </summary>
        private List<CouchMultiplayerPlayerUI> playerUIs = new List<CouchMultiplayerPlayerUI>();

        // Start is called before the first frame update
        void Start()
        {
        
        }

        // Update is called once per frame
        void Update()
        {
            CheckSelected();
        }

        /// <summary>
        /// Add a player UI
        /// </summary>
        /// <param name="playerUI"></param>
        public void AddPlayerUI(CouchMultiplayerPlayerUI playerUI)
        {
            // Set
            playerUI.SetCurrentSelectedGameObject(components.firstSelected);

            if(values.sharedPlayerUI)
            {
                playerUI.components.multiplayerEventSystem.playerRoot = components.playerRoot;

                // If a playerUI is already active & single UI
                if(values.singleSelectedUIGameObject && playerUIs.Count != 0)
                {
                    playerUI.SetCurrentSelectedGameObject(playerUIs[0].components.currentSelectedGameObject);
                }
            }
            else
            {
                // No playerRoot as it is shared
                playerUI.components.multiplayerEventSystem.playerRoot = null;
            }

            playerUIs.Add(playerUI);
        }

        /// <summary>
        /// Set all playerUIs selected gameobject if singleSelectedUIGameObjects is active
        /// </summary>
        /// <param name="gameObject"></param>
        public void SetCurrentSelectedGameObject(GameObject gameObject)
        {
            if(!values.singleSelectedUIGameObject) return;

            foreach(var item in playerUIs)
            {
                item.SetCurrentSelectedGameObject(gameObject);
            }
        }


        /// <summary>
        /// Check the selected gameobject of the playerUIs
        /// </summary>
        private void CheckSelected()
        {
            if(!values.sharedPlayerUI) return;
            if(!values.singleSelectedUIGameObject) return;

            // Check if a playerUI has selected a new gameobject
            int index = -1;
            for(int i = 0; i < playerUIs.Count; i++) // No Unity callback available when something new is selected so we have to use this non optimized method
            {
                CouchMultiplayerPlayerUI playerUI = playerUIs[i];
                if(playerUI.CurrentSelectedGameObject != playerUI.components.multiplayerEventSystem.currentSelectedGameObject)
                {
                    // New gameobject was selected, tell other playerUI's
                    playerUI.components.currentSelectedGameObject = playerUI.components.multiplayerEventSystem.currentSelectedGameObject;
                    index = i;
                    break;
                }
            }

            // If index isnt -1, then set all playerUIs selected gameobject of playerUIs[index]
            if(index != -1)
            {
                for(int i = 0; i < playerUIs.Count; i++)
                {
                    if(i == index) continue; // skip cause already set

                    playerUIs[i].SetCurrentSelectedGameObject(playerUIs[index].CurrentSelectedGameObject);
                }
            }
        }


        [System.Serializable]
        public class Components
        {
            [Tooltip("The first selected UI gameobject by a CouchMultiplayerPlayerUI")]
            public GameObject firstSelected;
            [Tooltip("The playerRoot of the CouchMultiplayerPlayerUI with a canvas group attached")]
            public GameObject playerRoot;
        }

        [System.Serializable]
        public class Values
        {
            [Tooltip("Does each player have its own UI instance, or do all players share the same UI?")]
            public bool sharedPlayerUI = true; //TODO couple with canvas manager to set firstselected / player root
            [Tooltip("Only relevant if sharedPlayerUI is true! If only 1 UI gameobject can be selected at all times. So if 1 player selects an object all other players also select that gameobject. If set to false all players can select their individual UI gameobject")]
            public bool singleSelectedUIGameObject;
        }
    }
}
