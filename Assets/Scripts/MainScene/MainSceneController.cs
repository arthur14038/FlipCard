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
		mainPageView.onClick2P = ShowTwoPlayers;
		mainPageView.onClickShop = ShowShop;
		mainPageView.onClickMail = SendMailToUs;
		mainPageView.onClickRate = ShowRatePage;
		mainPageView.onClickLeaveGame = LeaveGame;
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
	}

	void GoToGameScene(CardArrayLevel level)
	{
		CardArrayManager.currentLevel = level;

		int thisLevelPlayTimes = ModelManager.Instance.AddPlayTimes(level);
		if(thisLevelPlayTimes % 3 == 0)
		{
			UnityAnalyticsManager.Instance.SendCustomEvent(UnityAnalyticsManager.EventType.PlayGame, ModelManager.Instance.GetPlayTimes());
		}

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
		UnityAnalyticsManager.Instance.SendCustomEvent(UnityAnalyticsManager.EventType.OnClick2P);
		//mainPageView.HideUI(true);
		//currentView = MainSceneView.TwoPlayer;
	}

	void ShowShop()
	{
		UnityAnalyticsManager.Instance.SendCustomEvent(UnityAnalyticsManager.EventType.OnClickShop);
		//mainPageView.HideUI(true);
		//currentView = MainSceneView.Shop;
	}

	void ShowRatePage()
	{
		UnityAnalyticsManager.Instance.SendCustomEvent(UnityAnalyticsManager.EventType.OnClickRate);
	}

	void SendMailToUs()
	{
		UnityAnalyticsManager.Instance.SendCustomEvent(UnityAnalyticsManager.EventType.OnClickMail);
	}

	void LeaveGame()
	{
		Application.Quit();
	}
}
