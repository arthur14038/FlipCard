using UnityEngine;
using System.Collections;

public class MainSceneController : AbstractController {
	enum MainSceneView{MainPage = 0, SinglePlayer, TwoPlayer, Shop}
	MainSceneView currentView;
	public MainPageView mainPageView;
	public SinglePlayerView singlePlayerView;
	public TwoPlayerView twoPlayerView;
	string notifyMessage;
		
	public override IEnumerator Init ()
	{
		yield return StartCoroutine(mainPageView.Init());
		yield return StartCoroutine(singlePlayerView.Init());
		yield return StartCoroutine(twoPlayerView.Init());

		currentView = (MainSceneView)GameMainLoop.Instance.showView;

		mainPageView.onClick1P = ShowSinglePlayer;
		mainPageView.onClick2P = ShowTwoPlayers;
		mainPageView.onClickShop = ShowShop;
		mainPageView.onClickMail = SendMailToUs;
		mainPageView.onClickRate = ShowRatePage;
		mainPageView.onClickLeaveGame = LeaveGame;
		mainPageView.onClickNotify = SendNotifyMail;
        singlePlayerView.onClickBack = ShowMainPage;
		singlePlayerView.onClickPlay = GoToGameScene;
		twoPlayerView.onClickBack = ShowMainPage;

		mainPageView.HideUI(false);
		singlePlayerView.HideUI(false);
		twoPlayerView.HideUI(false);

		switch(currentView)
		{
			case MainSceneView.MainPage:
				mainPageView.ShowUI(false);
				break;
			case MainSceneView.SinglePlayer:
				singlePlayerView.ShowUI(false);
				break;
			case MainSceneView.TwoPlayer:
				twoPlayerView.ShowUI(false);
				break;
		}

		singlePlayerView.SetProgress(PlayerPrefsManager.OnePlayerProgress);
		AudioManager.Instance.PlayMusic("FlipCardBGM3", true); 
	}

	void GoToGameScene(CardArrayLevel level, GameMode mode)
	{
		AudioManager.Instance.StopMusic();
        CardArrayManager.currentLevel = level;
		CardArrayManager.currentMode = mode;
		GameMainLoop.Instance.ChangeScene(SceneName.GameScene);
	}

	void ShowMainPage()
	{
		switch(currentView)
		{
			case MainSceneView.SinglePlayer:
				singlePlayerView.HideUI(true);
				break;
			case MainSceneView.TwoPlayer:
				twoPlayerView.HideUI(true);
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
		twoPlayerView.ShowUI(true);
		currentView = MainSceneView.TwoPlayer;
		//UnityAnalyticsManager.Instance.SendCustomEvent(UnityAnalyticsManager.EventType.OnClick2P);
		//mainPageView.ShowUnderConstruction();
		//notifyMessage = "Please notify me when \"Play with Friend\" feature is launches.";
	}

	void ShowShop()
	{
		UnityAnalyticsManager.Instance.SendCustomEvent(UnityAnalyticsManager.EventType.OnClickShop);
		mainPageView.ShowUnderConstruction();
		notifyMessage = "Please notify me when \"Shop\" feature is launches.";
		//mainPageView.HideUI(true);
		//currentView = MainSceneView.Shop;
	}

	void SendNotifyMail()
	{
		string email = "playclaystudio@gmail.com";
		string subject = MyEscapeURL("Please notify me about...");
		string body = MyEscapeURL(notifyMessage);
		Application.OpenURL("mailto:" + email + "?subject=" + subject + "&body=" + body);
	}

	void ShowRatePage()
	{

	}

	void SendMailToUs()
	{
		string email = "playclaystudio@gmail.com";
		string subject = MyEscapeURL("User Feedback");
		Application.OpenURL("mailto:" + email + "?subject=" + subject);
	}

	void LeaveGame()
	{
		Application.Quit();
	}

	string MyEscapeURL(string url)
	{
		return WWW.EscapeURL(url).Replace("+", "%20");
	}
}
