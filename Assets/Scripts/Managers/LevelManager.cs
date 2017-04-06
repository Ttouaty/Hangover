using UnityEngine;
using System.Collections;

public class LevelManager : GenericSingleton<LevelManager>
{
	public PlayerController PlayerRef;
	public Animator PauseAnimator;

	private bool _introIsDone = false;
	IEnumerator Start()
	{
		PlayerRef.AllowInput = false;
		yield return new WaitForSeconds(0.5f);
		PlayerRef.SetActionMat("phone");
		PlayerRef.GetComponentInChildren<Animator>().SetBool("PhoneIsOut", true);
		yield return new WaitForSeconds(1.5f);
		PauseAnimator.SetTrigger("Enter");

		yield return new WaitUntil(() => InputManager.GetButtonDown(InputEnum.Start));

		PauseAnimator.SetTrigger("Return");
		yield return new WaitForSeconds(0.5f);

		PlayerRef.GetComponentInChildren<Animator>().SetBool("PhoneIsOut", false);

		yield return new WaitForSeconds(1f);
		PlayerRef.SetActionMat("idle");
		PlayerRef.AllowInput = true;
		_introIsDone = true;
	}

	void Update()
	{
		if(_introIsDone)
		{
			if (InputManager.GetButtonDown(InputEnum.Start))
			{
				if (GameManager.Instance.PauseIsActive)
				{
					PlayerRef.SetActionMat("phone");
					PlayerRef.GetComponentInChildren<Animator>().SetBool("PhoneIsOut", false);
					PauseAnimator.SetTrigger("Return");
				}
				else
				{
					PlayerRef.SetActionMat("phone");
					PlayerRef.GetComponentInChildren<Animator>().SetBool("PhoneIsOut", true);
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

	public void WinGame()
	{
		_introIsDone = false;
		GameManager.Instance.WinGame();
	}
}
