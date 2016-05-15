using UnityEngine;
using System.Collections;

public enum SceneName{FirstScene = 0, MainScene, GameScene, PickGameScene}
public class GameMainLoop : SingletonMonoBehavior<GameMainLoop> {
	IController controller;
	public LoadingPageManager loadingPage;
	public int showView;
	public int lastUnlockMode;
    VoidNoneParameter onSceneLoadComplete;
	bool syncFinished;

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

#if UNITY_5_3_OR_NEWER
		AsyncOperation op = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(sceneName);
#else
		AsyncOperation op = Application.LoadLevelAsync(sceneName);
#endif
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

        if (onSceneLoadComplete != null)
            onSceneLoadComplete();
	}
	
	// Use this for initialization
	IEnumerator Start () {
		DontDestroyOnLoad(this.gameObject);
		Localization.Init();
		loadingPage.Init();
		
		GameSettingManager.LoadData();
		ModelManager.Instance.Init();
        ScreenEffectManager.Instance.Init();

		if(PlayerPrefsManager.UnlockMode > 3)
			PlayerPrefsManager.UnlockMode = 3;
		lastUnlockMode = PlayerPrefsManager.UnlockMode;

		yield return StartCoroutine(InventoryManager.Instance.Init());
		yield return new WaitForSeconds(0.5f);

		if(controller != null)
			StartCoroutine(controller.Init());
	}
}
