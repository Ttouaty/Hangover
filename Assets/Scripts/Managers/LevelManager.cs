using UnityEngine;
using System.Collections;

public class LevelManager : GenericSingleton<LevelManager>
{
	public PlayerController PlayerRef;
	public Animator PauseAnimator;
	IEnumerator Start()
	{
		PlayerRef.AllowInput = false;
		yield return new WaitForSeconds(1);
		PlayerRef.GetComponentInChildren<Animator>().SetBool("PhoneIsOut", true);
		yield return new WaitForSeconds(0.5f);
				
	}

	void Update()
	{
		if (InputManager.GetButtonDown(InputEnum.Start))
		{
			if (GameManager.Instance.PauseIsActive)
			{
				PauseAnimator.SetTrigger("Return");
			}
			else
			{
				PauseAnimator.SetTrigger("Enter");
			}
			GameManager.Instance.PauseIsActive = !GameManager.Instance.PauseIsActive;
		}

		if (GameManager.Instance.PauseIsActive)
		{
			if (InputManager.GetButtonDown(InputEnum.Select))
			{
				GameManager.Instance.PauseIsActive = !GameManager.Instance.PauseIsActive;
				GameManager.Instance.CanPause = false;
				GameManager.Instance.LoadScene("SceneMenu");
			}
		}
	}
}
