using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public enum SceneName{FirstScene = 0, MainScene, GameScene, TestMain, TestGame}
public class GameMainLoop : SingletonMonoBehavior<GameMainLoop> {
	IController controller;
	public LoadingPageManager loadingPage;
	public int showView;
	public int lastUnlockMode;
    VoidNoneParameter onSceneLoadComplete;

	public void RegisterController(IController controller, VoidNoneParameter onSceneLoadComplete = null)
    {
        this.onSceneLoadComplete = onSceneLoadComplete;
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
		
		AsyncOperation op = SceneManager.LoadSceneAsync(sceneName);
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

        onSceneLoadComplete?.Invoke();
    }
	
	// Use this for initialization
	IEnumerator Start () {
		DontDestroyOnLoad(this.gameObject);
		loadingPage.Init();

		GameSettingManager.LoadData();
		ModelManager.Instance.Init();
        ScreenEffectManager.Instance.Init();
		
		lastUnlockMode = PlayerPrefsManager.UnlockMode;

		//yield return StartCoroutine(InventoryManager.Instance.Init());
		yield return new WaitForSeconds(0.5f);

		Debug.LogFormat("GameMainLoop before controller Init, controller == null: {0}.", controller == null);
		if (controller != null)
			StartCoroutine(controller.Init());
	}
}
