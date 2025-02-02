using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

namespace StarterAssets
{
	public class StarterAssetsInputs : MonoBehaviour
	{
		[SerializeField] GameObject mainMenu;
		[Header("Character Input Values")]
		public Vector2 move;
		public Vector2 look;
		public bool jump;
		public bool sprint;

		[Header("Movement Settings")]
		public bool analogMovement;

		[Header("Mouse Cursor Settings")]
		public bool cursorLocked = true;
		public bool cursorInputForLook = true;
		public bool action;
		public bool grow;
		public bool taking;

		public bool mainMenuOpen;

#if ENABLE_INPUT_SYSTEM
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
		public void OnAction(InputValue value)
		{
			Action(value.isPressed);
		}
		public void OnGrow(InputValue value)
		{
			Grow(value.isPressed);
		}
		public void OnTaking(InputValue value)
		{
			Taking(value.isPressed);
		}
		public void OnMainMenu(InputValue value)
		{
			mainMenuOpen = !mainMenuOpen;
			if (mainMenuOpen)
			{
				Time.timeScale = 0;
				AudioListener.pause = true;
				if (mainMenu) mainMenu.SetActive(true);
			}
			else
			{
				Time.timeScale = 1;
				AudioListener.pause = false;
				if (mainMenu) mainMenu.SetActive(false);
			}
		}
#endif


		public void MoveInput(Vector2 newMoveDirection)
		{
			move = newMoveDirection;
		} 

		public void LookInput(Vector2 newLookDirection)
		{
			look = newLookDirection;
		}

		public void JumpInput(bool newJumpState)
		{
			jump = newJumpState;
		}

		public void SprintInput(bool newSprintState)
		{
			sprint = newSprintState;
		}

		private void OnApplicationFocus(bool hasFocus)
		{
			SetCursorState(cursorLocked);
		}
		private void Action(bool newAction)
		{
			action = newAction;
		}	private void Grow(bool newGrow)
		{
			grow = newGrow;
		}	private void Taking(bool newTaking)
		{
			taking = newTaking;
		}

		private void SetCursorState(bool newState)
		{
			Cursor.lockState = newState ? CursorLockMode.Locked : CursorLockMode.None;
		}
	}
	
}