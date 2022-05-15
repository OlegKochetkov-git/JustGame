using UnityEngine;
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

		[Header("Movement Settings")]
		public bool analogMovement;

		[Header("Mouse Cursor Settings")]
		public bool cursorLocked = true;
		public bool cursorInputForLook = true;

#if ENABLE_INPUT_SYSTEM && STARTER_ASSETS_PACKAGES_CHECKED
		public void OnMove(InputValue value)
		{
			MoveInput(value.Get<Vector2>());
		}

		public void OnLook(InputValue value)
		{
			if(cursorInputForLook)
			{
				LookInput(value.Get<Vector2>());
			}
		}

		public void OnJump(InputValue value)
		{
			JumpInput(value.isPressed);
		}

		public void OnSprint(InputValue value)
		{
			SprintInput(value.isPressed);
		}

		public void OnClickMouse(InputValue value)
        {
			LeftMouseClick(value.isPressed);
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