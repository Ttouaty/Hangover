using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Collider))]
public class FocusZone : MonoBehaviour {

	private Collider _colliRef;
	public Transform TargetTransform;
	void Awake()
	{
		_colliRef = GetComponentInChildren<Collider>();
		_colliRef.isTrigger = true;
		if (TargetTransform == null)
			TargetTransform = transform;
	}

	void OnTriggerEnter()
	{
		CameraManager.Instance.AddTargetToTrack(TargetTransform);
	}

	void OnTriggerExit()
	{
		CameraManager.Instance.RemoveTargetToTrack(TargetTransform);
	}

	void OnDisable()
	{
		CameraManager.Instance.RemoveTargetToTrack(TargetTransform);
	}
}
