using UnityEngine;
using System.Collections;
using UnityEngine.Events;

public class LDTrigger : MonoBehaviour
{
	public UnityEvent OnEnter;
	public UnityEvent OnExit;
	public UnityEvent OnStay;

	void OnTriggerEnter() { OnEnter.Invoke(); }
	void OnTriggerExit() { OnExit.Invoke(); }
	void OnTriggerStay() { OnStay.Invoke(); }
}
