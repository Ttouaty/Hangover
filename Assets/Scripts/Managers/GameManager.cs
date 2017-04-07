using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class GameManager : GenericSingleton<GameManager>
{
	public GameObject CreditPanel;
	//public bool IsInGame = false;

	public bool PauseIsActive = false;
	public bool CanPause = false;

	protected override void Awake()
	{
		DontDestroyOnLoad(gameObject);
	}

	public void BringUpCredits()
	{
		CreditPanel.GetComponent<Animator>().SetTrigger("Enter");
		EventSystem.current.SetSelectedGameObject(CreditPanel.GetComponentInChildren<Button>(true).gameObject);
	}

	public void SendOutCredits()
	{
		CreditPanel.GetComponent<Animator>().SetTrigger("Return");
		EventSystem.current.SetSelectedGameObject(EventSystem.current.firstSelectedGameObject);
	}

	public void StartGame()
	{
		CanPause = true;
		LoadScene("SceneProto");
		//IsInGame = true;
	}

	public void LoadScene(string sceneName)
	{
		StartCoroutine(AutoFade.StartFade(0.5f, WaitForLoadScene(sceneName), 0.5f, Color.white));
		EventSystem.current.SetSelectedGameObject(null);
		EventSystem.current.gameObject.SetActive(false);
	}

	IEnumerator WaitForLoadScene(string sceneName)
	{
		AsyncOperation tempOp = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Single);
		yield return new WaitUntil(() => tempOp.isDone);
	}

	public void ExitGame()
	{
		Application.Quit();
	}

	public void WinGame()
	{
		CameraManager.Instance.GetComponent<Animator>().SetTrigger("Win");
		PlayerController variableDebile = FindObjectOfType<PlayerController>();
		variableDebile.AllowInput = false;
		variableDebile.StopAllCoroutines();
		variableDebile.SetActionMat("keys");
		variableDebile.GetComponentInChildren<Animator>().SetTrigger("Win");
		StartCoroutine(WaitForStart());
	}

	IEnumerator WaitForStart()
	{
		yield return new WaitUntil(() => InputManager.GetButtonDown(InputEnum.Start));
		PauseIsActive = false;
		CanPause = false;
		LoadScene("SceneMenu");
	}
}
