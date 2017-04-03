using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;
using System;

public enum InputEnum
{
	A = 0,
	B = 1,
	X = 2,
	Y = 3,
	LB = 4,
	RB = 5,
	Select = 6,
	Start = 7
}

public class InputManager : GenericSingleton<InputManager>
{
	private static float _inputLockTime = 0;

	public static bool InputLocked { get { return _inputLockTime != 0; } }

	void LateUpdate()
	{
		_inputLockTime = _inputLockTime.Reduce(Time.deltaTime);
	}

	public static string[] GetJoystickNames()
	{
		string[] joystickNames = Input.GetJoystickNames();

		string[] returnArray = new string[joystickNames.Length + 1];
		for (int i = 1; i < joystickNames.Length + 1; i++)
		{
			returnArray[i] = joystickNames[i - 1];
		}
		returnArray[0] = "Keyboard";

		return returnArray;
	}

	public static string AnyKeyDown()
	{
		return Input.inputString;
	}


	public static int AnyButtonDown()
	{
		for (int j = 0; j < 12; j++)
		{
			if (GetButtonDown(j))
				return j;
		}

		return -1;
	}

	#region GetButtonDown
	public static bool GetButtonDown(InputEnum buttonName)
	{
		return GetButtonDown((int)buttonName);
	}

	public static bool GetButtonDown(int buttonNumber)
	{
		if (InputLocked)
			return false;

		return Input.GetKeyDown("joystick button " + buttonNumber);
	}

	#endregion

	#region GetButtonUp
	public static bool GetButtonUp(InputEnum buttonName )
	{
		return GetButtonUp((int)buttonName);
	}

	public static bool GetButtonUp(int buttonNumber)
	{
		if (InputLocked)
			return false;
		return Input.GetKeyUp("joystick button " + buttonNumber);
	}

	#endregion

	#region GetButtonHeld
	public static bool GetButtonHeld(InputEnum buttonName)
	{
		return GetButtonHeld((int)buttonName);
	}

	public static bool GetButtonHeld(int buttonNumber)
	{
		if (InputLocked)
			return false;

		return Input.GetKey("joystick button " + buttonNumber);
	}
	#endregion

	#region GetAxis
	public static float GetAxis(string axisName)
	{
		if (InputLocked)
			return 0;
		return Input.GetAxis(axisName);
	}

	public static float GetAxisRaw(string axisName)
	{
		if(InputLocked)
			return 0;
		return Input.GetAxisRaw(axisName);
	}

	public static Vector2 GetStickDirection()
	{
		return GetStickDirection(true);
	}

	public static Vector2 GetStickDirection(bool raw)
	{
		if (StickIsNeutral() || InputLocked)
			return Vector2.zero;
		if(raw)
			return new Vector2(GetAxisRaw("Horizontal"), GetAxisRaw("Vertical"));
		return new Vector2(GetAxis("Horizontal"), GetAxis("Vertical"));
	}

	public static bool StickIsNeutral(float deadZone = 0f)
	{
		if(deadZone == 0)
			return Input.GetAxisRaw("Horizontal") == 0 && Input.GetAxisRaw("Vertical") == 0;
		else
			return Mathf.Abs(Input.GetAxis("Horizontal")) < deadZone && Mathf.Abs(Input.GetAxis("Vertical")) < deadZone;
	}
	#endregion

	public static void AddInputLockTime(float additionnalTime)
	{
		_inputLockTime += additionnalTime;
	}

	public static void SetInputLockTime(float newTime)
	{
		_inputLockTime = newTime;
	}
	

}
