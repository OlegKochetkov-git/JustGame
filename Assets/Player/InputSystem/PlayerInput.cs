using UnityEngine;
using UnityEngine.EventSystems;
#if ENABLE_INPUT_SYSTEM && STARTER_ASSETS_PACKAGES_CHECKED
using UnityEngine.InputSystem;
#endif

namespace StarterAssets
{
	public class PlayerInput : MonoBehaviour
	{
		[Header("Character Input Values")]
		public Vector2 move;
		public Vector2 look;
		public bool jump;
		public bool sprint;
		public bool leftMouseButtonClick;
		public bool rightMouseButtonClick;

		[Header("Movement Settings")]
		public bool analogMovement;

		[Header("Mouse Cursor Settings")]
		public bool cursorLocked = true;
		public bool cursorInputForLook = true;

#if ENABLE_INPUT_SYSTEM && STARTER_ASSETS_PACKAGES_CHECKED
		public void OnMove(InputAction.CallbackContext context)
		{
			MoveInput(context.ReadValue<Vector2>());
		}

		public void OnLook(InputAction.CallbackContext context)
		{
			if(cursorInputForLook)
			{
				LookInput(context.ReadValue<Vector2>());
			}
		}

		public void OnJump(InputAction.CallbackContext context)
		{
			JumpInput(context.performed);
		}

		public void OnSprint(InputAction.CallbackContext context)
		{
			SprintInput(context.performed);
		}

		public void OnLeftMouseClick(InputAction.CallbackContext context)
        {
			LeftMouseClick(context.performed);
			
		}

		public void OnRigthMouseClick(InputAction.CallbackContext context)
		{ 
			RightMouseClick(context.performed);
		}
#endif

		private void MoveInput(Vector2 newMoveDirection)
		{
			move = newMoveDirection;
		}

		private void LookInput(Vector2 newLookDirection)
		{
			look = newLookDirection;
		}

		private void JumpInput(bool newJumpState)
		{
			jump = newJumpState;
		}

		private void SprintInput(bool newSprintState)
		{
			sprint = newSprintState;
		}

		private void LeftMouseClick(bool newLeftMouseButtonState)
        {
			leftMouseButtonClick = newLeftMouseButtonState;
        }

		private void RightMouseClick(bool newrightMouseButtonClick)
		{
			rightMouseButtonClick = newrightMouseButtonClick;
		}
					
		private void OnApplicationFocus(bool hasFocus)
		{
			SetCursorState(cursorLocked);
		}

		private void SetCursorState(bool newState)
		{
			Cursor.lockState = newState ? CursorLockMode.Locked : CursorLockMode.None;
		}
    }
}