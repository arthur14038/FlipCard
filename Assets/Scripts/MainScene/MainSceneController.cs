using UnityEngine;
using System.Collections;

public class MainSceneController : AbstractController {
	public MainPageView mainPageView;
		
	public override IEnumerator Init ()
	{
		yield return StartCoroutine(mainPageView.Init(GameMainLoop.Instance.showView));

		mainPageView.onClickPlay = GoToGameScene;
		mainPageView.ShowUI(false);

		mainPageView.SetOnePlayerProgress(PlayerPrefsManager.OnePlayerProgress);

		AdBanner.Instance.ShowBanner();
	}

	void GoToGameScene(CardArrayLevel level)
	{
		CardArrayManager.currentLevel = level;
		AdBanner.Instance.HideBanner();
		GameMainLoop.Instance.ChangeScene(SceneName.GameScene);
	}
}
