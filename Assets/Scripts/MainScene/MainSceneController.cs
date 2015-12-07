using UnityEngine;
using System.Collections;

public class MainSceneController : AbstractController {
	enum MainSceneView{MainPage = 0, SinglePlayer, TwoPlayer, Shop}
	MainSceneView currentView;
	public MainPageView mainPageView;
	public SinglePlayerView singlePlayerView;
		
	public override IEnumerator Init ()
	{
		yield return StartCoroutine(mainPageView.Init());
		yield return StartCoroutine(singlePlayerView.Init());

		currentView = (MainSceneView)GameMainLoop.Instance.showView;

		mainPageView.onClick1P = ShowSinglePlayer;
		//mainPageView.onClick2P = ShowTwoPlayers;
		mainPageView.onClickLeaveGame = LeaveGame;
		//mainPageView.onClickShop = ShowShop;
		singlePlayerView.onClickBack = ShowMainPage;
		singlePlayerView.onClickPlay = GoToGameScene;

		mainPageView.HideUI(false);
		singlePlayerView.HideUI(false);

		switch(currentView)
		{
		case MainSceneView.MainPage:
			mainPageView.ShowUI(false);
			break;
		case MainSceneView.SinglePlayer:
			singlePlayerView.ShowUI(false);
			break;
		}

		singlePlayerView.SetProgress(PlayerPrefsManager.OnePlayerProgress);

		AdBanner.Instance.ShowBanner();
	}

	void GoToGameScene(CardArrayLevel level)
	{
		CardArrayManager.currentLevel = level;
		AdBanner.Instance.HideBanner();
		GameMainLoop.Instance.ChangeScene(SceneName.GameScene);
	}

	void ShowMainPage()
	{
		switch(currentView)
		{
		case MainSceneView.SinglePlayer:
			singlePlayerView.HideUI(true);
			break;
		}
		mainPageView.ShowUI(true);
		currentView = MainSceneView.MainPage;
	}

	void ShowSinglePlayer()
	{
		mainPageView.HideUI(true);
		singlePlayerView.ShowUI(true);
		currentView = MainSceneView.SinglePlayer;
	}

	void ShowTwoPlayers()
	{
		mainPageView.HideUI(true);
		currentView = MainSceneView.TwoPlayer;
	}

	void ShowShop()
	{
		mainPageView.HideUI(true);
		currentView = MainSceneView.Shop;
	}

	void LeaveGame()
	{
		Application.Quit();
	}
}
