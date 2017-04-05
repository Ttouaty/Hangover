using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.Events;

[RequireComponent(typeof(Animator))]
public class QTE : MonoBehaviour
{
	public Sprite[] InputSprites;
	public Color[] AvailableColors;
	public Image TargetImage;

	public int TotalRequiredInputs = 3;

	private int[] AvailableInputs = new int[] { 0, 1, 2, 3 };
	private List<int> _requiredInputsList = new List<int>();

	private float _maxTimeBeforeResetInput = 2;
	private float _timeLeftBeforeResetInput;

	private bool _processingInputs = true;

	private int _listenedinputNumber;
	private int _listenedinputIndex = 0;

	[SerializeField]
	private UnityEvent OnSuccess;
	[SerializeField]
	private UnityEvent OnFail;

	private PlayerController TargetPlayer;

	private void GenerateRandomRequiredInput()
	{
		_processingInputs = false;
		_requiredInputsList.Clear();
		for (int i = 0; i < TotalRequiredInputs; i++)
		{
			_requiredInputsList.Add(AvailableInputs.RandomElement());
		}
		_timeLeftBeforeResetInput = _maxTimeBeforeResetInput;

		_listenedinputIndex = 0;
		_listenedinputNumber = _requiredInputsList[_listenedinputIndex];
		StartCoroutine(DisplayInputs(_requiredInputsList[_listenedinputIndex]));
	}

	void Awake()
	{
		enabled = false;
	}

	void Start()
	{
		GenerateRandomRequiredInput();
		GetComponent<Animator>().ResetTrigger("Return");
	}

	void Update()
	{
		_timeLeftBeforeResetInput -= Time.deltaTime;

		if(_timeLeftBeforeResetInput <= 0)
			GenerateRandomRequiredInput();

		if (_processingInputs)
			ProcessInput();
	}


	void ProcessInput()
	{
		int inputPressed = InputManager.AnyButtonDown();
		if (inputPressed != -1)
		{
			if(inputPressed == _listenedinputNumber)
			{
				Debug.Log("Right inputPressed !");
				_listenedinputIndex++;
				if (_listenedinputIndex == TotalRequiredInputs)
				{
					Debug.Log("success !");
					GetComponent<Animator>().SetTrigger("Return");

					OnSuccess.Invoke();
					Destroy(this);
					return;
				}
				_listenedinputNumber = _requiredInputsList[_listenedinputIndex];

				StartCoroutine(DisplayInputs(_requiredInputsList[_listenedinputIndex]));
				_timeLeftBeforeResetInput = _maxTimeBeforeResetInput;
			}
			else
			{
				Debug.LogError("Wrong BITCH !");
				OnFail.Invoke();
				GenerateRandomRequiredInput();
			}
		}
	}

	IEnumerator DisplayInputs(int inputNumber)
	{
		GetComponent<Animator>().SetTrigger("Return");
		yield return new WaitForSeconds(0.2f);
		TargetImage.transform.GetChild(0).GetComponent<Image>().sprite = InputSprites[inputNumber];
		TargetImage.color = AvailableColors[Random.Range(0, AvailableColors.Length -1)];
		GetComponent<Animator>().SetTrigger("Activate");
		yield return new WaitForSeconds(0.2f);
		_processingInputs = true;
	}

	void OnTriggerEnter()
	{
		enabled = true;
		TargetPlayer = FindObjectOfType<PlayerController>();
		TargetPlayer.AllowInput = false;
	}

	void OnDestroy()
	{
		TargetPlayer.AllowInput = true;
	}
}
