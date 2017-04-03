using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine.Events;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
	#region References
	private Rigidbody _rigidB;
	#endregion

	#region Input
	[SerializeField]
	[Tooltip("ATTENTION! 3rd input axis not defined in inputManager yet.")]
	private string[] _possibleInputs = new string[] {
		"axis/1/1", "axis/1/-1",
		"axis/2/1", "axis/2/-1",
		"axis/4/1", "axis/4/-1",
		"axis/5/1", "axis/5/-1",
		"axis/6/1", "axis/6/-1",
		"axis/7/1", "axis/7/-1",
		"button/0",
		"button/1",
		"button/2",
		"button/3",
		"button/4",
		"button/5",
		"button/6",
		"button/7"
	};
	[SerializeField]
	private UnityEvent _availableActions;

	[SerializeField]
	[Range(0.3f, 1)]
	private float _axisActivationThreshold = 0.8f;
	private Dictionary<string, int> _activeInputs = new Dictionary<string, int>();
	#endregion

	#region Direction
	private Vector3 _activeSpeed = Vector3.zero;
	private Vector3 _activeDirection = Vector3.zero;
	private float _maxSpeed = 3; //Max speed (units / s)
	#endregion

	#region States
	private bool _internalAllowInput = true;
	private bool _isFrozen = false;
	#endregion

	#region Getters
	public bool AllowInput
	{
		get { return _internalAllowInput && !_isFrozen; }
		private set { _internalAllowInput = value; }
	}

	#endregion

	void Start()
	{
		_rigidB = GetComponent<Rigidbody>();
		GenerateRandomInputs();
	}

	void Update()
	{
		if (AllowInput)
			ProcessInput();

		ProcessActiveSpeed();
	}

	void GenerateRandomInputs()
	{
		Debug.Log("Generating new random inputs");
		List<string> tempArray = new List<string>();

		for (int i = 0; i < _possibleInputs.Length; i++)
		{
			tempArray.Add(_possibleInputs[i]);
		}

		for (int i = 0; i < _availableActions.GetPersistentEventCount(); i++)
		{
			_activeInputs[tempArray.ShiftRandomElement()] = i;
		}
	}

	void ProcessInput()
	{
		string[] fragString;
		for (int i = 0; i < _possibleInputs.Length; i++)
		{
			if (!_activeInputs.ContainsKey(_possibleInputs[i]))
				continue;

			fragString = _possibleInputs[i].Split('/');

			if (fragString[0] == "axis")
			{
				if (Mathf.Abs(InputManager.GetAxis(fragString[1])) > _axisActivationThreshold && Mathf.Sign(InputManager.GetAxis(fragString[1])) == Convert.ToInt32(fragString[2]))
				{ 
					((MonoBehaviour)_availableActions.GetPersistentTarget(_activeInputs[_possibleInputs[i]])).Invoke(_availableActions.GetPersistentMethodName(_activeInputs[_possibleInputs[i]]), 0);
				}
			}
			else
			{
				if (InputManager.GetButtonHeld(Convert.ToInt32(fragString[1])))
				{
					((MonoBehaviour) _availableActions.GetPersistentTarget(_activeInputs[_possibleInputs[i]])).Invoke(_availableActions.GetPersistentMethodName(_activeInputs[_possibleInputs[i]]), 0);
				}
			}
		}
	}

	void ProcessActiveSpeed()
	{
		_activeSpeed = _activeDirection * _maxSpeed;
		_activeSpeed.y = _rigidB.velocity.y;

		transform.LookAt(transform.position + _activeDirection);
		_rigidB.velocity = _activeSpeed;

		_activeDirection = Vector3.zero;
	}

	public void GoRight()
	{
		_activeDirection += Camera.main.transform.right;
	}

	public void GoLeft()
	{
		_activeDirection -= Camera.main.transform.right;
	}

	public void GoUp()
	{
		_activeDirection += Camera.main.transform.forward.ZeroY().normalized;
	}

	public void GoDown()
	{
		_activeDirection -= Camera.main.transform.forward.ZeroY().normalized;
	}


}
