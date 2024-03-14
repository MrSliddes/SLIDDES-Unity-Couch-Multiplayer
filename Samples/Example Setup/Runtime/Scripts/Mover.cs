using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace SLIDDES.Multiplayer.Couch.Samples
{
    public class Mover : MonoBehaviour
    {
        private Vector2 velocity;

        // Start is called before the first frame update
        void Start()
        {
        
        }

        // Update is called once per frame
        void Update()
        {
            transform.Translate(velocity * Time.deltaTime);
        }

        public void OnMovement(InputAction.CallbackContext context)
        {
            velocity = context.ReadValue<Vector2>();
        }
    }
}
