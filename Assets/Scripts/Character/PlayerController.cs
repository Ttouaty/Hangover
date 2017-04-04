using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine.Events;

[RequireComponent(typeof(Rigidbody), typeof(Animator))]
public class PlayerController : MonoBehaviour
{
	#region References
	private Rigidbody _rigidB;
	private Animator _animator;
	#endregion

	#region Input
	[Header("Inputs")]
	[SerializeField]
	[Range(0.3f, 1)]
	private float _axisActivationThreshold = 0.8f;
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

	private Dictionary<string, int> _activeInputs = new Dictionary<string, int>();
	#endregion

	#region Direction
	private Vector3 _activeSpeed = Vector3.zero;
	private Vector3 _drunkSpeed = Vector3.zero;
	private Vector3 _activeDirection = Vector3.zero;
	private float _maxSpeed = 3; //Max speed (units / s)
	#endregion

	#region States
	private bool _internalAllowInput = true;
	private bool _isFrozen = false;
	private bool _isStumbling = false;
	#endregion

	#region GamePlay
	[Header("Drunk Movement")]
	[SerializeField]
	private float _minDrunkInterval = 5;
	[SerializeField]
	private float _maxDrunkInterval = 20;
	[SerializeField]
	private float _stumbleForce = 2;
	[SerializeField]
	private float _stumbleTime = 1.5f;
	[SerializeField]
	private AnimationCurve _stumbleSpeedCurve;
	#endregion

	#region Getters
	public bool AllowInput
	{
		get { return _internalAllowInput && !_isFrozen; }
		set { _internalAllowInput = value; }
	}

	#endregion

	void Start()
	{
		_rigidB = GetComponent<Rigidbody>();
		_animator = GetComponent<Animator>();
		GenerateRandomInputs();
		StumbleInterval();
		CameraManager.Instance.AddTargetToTrack(transform);
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
		_rigidB.velocity = _activeSpeed + _drunkSpeed;


		transform.LookAt(transform.position + _activeDirection);
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

	void StumbleInterval()
	{
		Debug.Log("Starting stumbling coroutine");
		StartCoroutine("StumbleCoroutine"); // appel en string pour pouvoir la stopper en string
	}

	IEnumerator StumbleCoroutine()
	{
		float randomTime;
		while(true)
		{
			randomTime = UnityEngine.Random.Range(_minDrunkInterval, _maxDrunkInterval);

			yield return new WaitForSeconds(randomTime);
			if(AllowInput)
				StartCoroutine(ForceStumble());
		}
	}

	IEnumerator ForceStumble()
	{
		Debug.Log("Forcing stumble");

		Vector3 tempDirection = (Quaternion.AngleAxis(UnityEngine.Random.Range(0, 360), Vector3.up) * (Vector3.right * _stumbleForce));

		_isStumbling = true;

		float eT = 0;
		while(eT < _stumbleTime)
		{
			eT += Time.deltaTime;
			_drunkSpeed = _stumbleSpeedCurve.Evaluate(eT / _stumbleTime) * tempDirection;
			_activeSpeed *= 0.5f; // reduce character movement possibility
			yield return null;
		}

		_drunkSpeed = Vector3.zero;
		_isStumbling = false;
	}


	public void SpecialAction_Scream() { StartCoroutine(SpecialAction_Scream_Coroutine()); }
	public void SpecialAction_Dance() { StartCoroutine(SpecialAction_Dance_Coroutine()); }
	public void SpecialAction_Drink() { StartCoroutine(SpecialAction_Drink_Coroutine()); }

	IEnumerator SpecialAction_Scream_Coroutine()
	{
		AllowInput = false;
		Debug.Log("I AM SCREAMING !!!");
		yield return new WaitForSeconds(0.5f);
		AllowInput = true;
	}

	IEnumerator SpecialAction_Dance_Coroutine()
	{
		AllowInput = false;
		Debug.Log("I am Dancing");
		yield return new WaitForSeconds(0.5f);
		AllowInput = true;
	}

	IEnumerator SpecialAction_Drink_Coroutine()
	{
		AllowInput = false;
		Debug.Log("I am drinking");
		_animator.SetTrigger("Drink");
		yield return new WaitForSeconds(1f);
		AllowInput = true;
	}
}
