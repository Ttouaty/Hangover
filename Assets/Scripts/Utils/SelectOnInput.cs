using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

public class SelectOnInput : MonoBehaviour
{

	public EventSystem eventSystem;
	public GameObject selectedObject;

	private bool _usingGamepad;

	// Use this for initialization
	void Start()
	{

	}

	// Update is called once per frame
	void Update()
	{
		if (Input.GetAxisRaw("2") != 0 && !_usingGamepad)
		{
			eventSystem.SetSelectedGameObject(selectedObject);
			_usingGamepad = true;
		}

		if(Input.GetMouseButtonDown(0))
			_usingGamepad = false;
	}

	private void OnDisable()
	{
		_usingGamepad = false;
	}
}