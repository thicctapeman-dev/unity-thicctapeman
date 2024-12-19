using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace ThiccTapeman.Input
{
    // ------------------------------------ //
    // Input Item class                     //
    // ------------------------------------ //
    #region Input Item class

    public class InputItem
    {
        public InputAction inputAction;

        private bool triggered;
        public bool held;

        public InputItem(InputAction inputAction)
        {
            this.inputAction = inputAction;

            inputAction.performed += Performed;
            inputAction.canceled += Cancelled;
        }

        private void Performed(InputAction.CallbackContext obj)
        {
            held = true;
        }
        private void Cancelled(InputAction.CallbackContext obj)
        {
            held = false;
        }

        /// <summary>
        /// Reads the value
        /// </summary>
        /// <typeparam name="T">The type of variable that should be read and returned</typeparam>
        /// <returns>Returns the value in T</returns>
        public T ReadValue<T>() where T : struct
        {
            return inputAction.ReadValue<T>();
        }

        /// <summary>
        /// Makes buttons easier
        /// </summary>
        /// <returns>Returns true every other time it get's triggered</returns>
        public bool GetTriggered(bool everyTime = false)
        {
            if (everyTime) return inputAction.triggered;

            if (inputAction.triggered) triggered = !triggered;

            if (triggered)
            {
                return inputAction.triggered;
            }

            return false;
        }
    }

    #endregion
}
