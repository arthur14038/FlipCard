using UnityEngine;
using System.Collections;

public enum SceneName{FirstScene = 0, MainScene, GameScene}
public class GameMainLoop : SingletonMonoBehavior<GameMainLoop> {
	IController controller;
	public LoadingPageManager loadingPage;
	public int showView;

	public void RegisterController(IController controller)
	{
		this.controller = controller;
	}

	public void ChangeScene(SceneName scene, int view = 0)
	{
		showView = view;
		controller = null;
		StartCoroutine(LoadNextLevel(scene.ToString()));
	}

	IEnumerator LoadNextLevel(string sceneName)
	{
		yield return StartCoroutine(loadingPage.TurnOn());

		AsyncOperation op = Application.LoadLevelAsync(sceneName);
		op.allowSceneActivation = false;
		while (op.progress < 0.9f) {
			yield return new WaitForEndOfFrame();
		}

		// Allow the activation of the scene again.
		op.allowSceneActivation = true;
		
		while(!op.isDone) {
			yield return new WaitForEndOfFrame();
		}

		if(controller != null)
			yield return StartCoroutine(controller.Init());

		yield return new WaitForSeconds(0.5f);

		yield return StartCoroutine(loadingPage.FadeOutLoadingPage());
	}

	// Use this for initialization
	IEnumerator Start () {
		DontDestroyOnLoad(this.gameObject);
		loadingPage.Init();
		ModelManager.Instance.Init();
        ScreenEffectManager.Instance.Init();
		CardArrayManager.LoadData();
		yield return new WaitForSeconds(0.5f);
		if(controller != null)
			StartCoroutine(controller.Init());
	}
}
