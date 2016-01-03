using UnityEngine;
using System.Collections;

public class MainSceneController : AbstractController {
	enum MainSceneView{MainPage = 0, ClassicMode, TwoPlayer, TimeMode, Shop}
	MainSceneView currentView;
	public MainPageView mainPageView;
	public ClassicModeView classicModeView;
	public TwoPlayerView twoPlayerView;
	public TimeModeView timeModeView;
	public ShopView shopView;
	string notifyMessage;
		
	public override IEnumerator Init ()
	{
		yield return StartCoroutine(mainPageView.Init());
		yield return StartCoroutine(classicModeView.Init());
		yield return StartCoroutine(twoPlayerView.Init());
		yield return StartCoroutine(timeModeView.Init());
		yield return StartCoroutine(shopView.Init());

		currentView = (MainSceneView)GameMainLoop.Instance.showView;

		mainPageView.onClickClassicMode = ShowClassicMode;
		mainPageView.onClick2P = ShowTwoPlayers;
		mainPageView.onClickTimeMode = ShowTimeMode;
		mainPageView.onClickShop = ShowShop;
		mainPageView.onClickMail = SendMailToUs;
		mainPageView.onClickRate = ShowRatePage;
		mainPageView.onClickLeaveGame = LeaveGame;
		mainPageView.onClickNotify = SendNotifyMail;
		classicModeView.onClickBack = ShowMainPage;
		classicModeView.onClickPlay = GoToGameScene;
		twoPlayerView.onClickBack = ShowMainPage;
		twoPlayerView.onClickPlay = GoToGameScene;
		timeModeView.onClickBack = ShowMainPage;
		timeModeView.onClickPlay = GoToGameScene;
		shopView.onClickBack = ShowMainPage;

		shopView.HideUI(false);
        mainPageView.HideUI(false);
		classicModeView.HideUI(false);
		twoPlayerView.HideUI(false);
		timeModeView.HideUI(false);

		switch(currentView)
		{
			case MainSceneView.MainPage:
				mainPageView.ShowUI(false);
				break;
			case MainSceneView.ClassicMode:
				classicModeView.ShowUI(false);
				break;
			case MainSceneView.TwoPlayer:
				twoPlayerView.ShowUI(false);
				break;
			case MainSceneView.Shop:
				shopView.ShowUI(false);
				break;
			case MainSceneView.TimeMode:
				timeModeView.ShowUI(false);
				break;
		}

		classicModeView.SetProgress(PlayerPrefsManager.ClassicModeProgress);
		timeModeView.SetProgress(PlayerPrefsManager.TimeModeProgress);
		AudioManager.Instance.PlayMusic("FlipCardBGM3", true); 
	}

	void GoToGameScene(CardArrayLevel level, GameMode mode)
	{
		AudioManager.Instance.StopMusic();
        GameSettingManager.currentLevel = level;
		GameSettingManager.currentMode = mode;
		GameMainLoop.Instance.ChangeScene(SceneName.GameScene);
	}

	void ShowMainPage()
	{
		switch(currentView)
		{
			case MainSceneView.ClassicMode:
				classicModeView.HideUI(true);
				break;
			case MainSceneView.TwoPlayer:
				twoPlayerView.HideUI(true);
				break;
			case MainSceneView.Shop:
				shopView.HideUI(true);
				break;
			case MainSceneView.TimeMode:
				timeModeView.HideUI(true);
				break;
		}
		mainPageView.ShowUI(true);
		currentView = MainSceneView.MainPage;
	}

	void ShowClassicMode()
	{
		mainPageView.HideUI(true);
		classicModeView.ShowUI(true);
		currentView = MainSceneView.ClassicMode;
	}

	void ShowTwoPlayers()
	{
		mainPageView.HideUI(true);
		twoPlayerView.ShowUI(true);
		currentView = MainSceneView.TwoPlayer;
	}

	void ShowTimeMode()
	{
		mainPageView.HideUI(true);
		timeModeView.ShowUI(true);
		currentView = MainSceneView.TimeMode;
	}

	void ShowShop()
	{
		mainPageView.HideUI(true);
		shopView.ShowUI(true);
		currentView = MainSceneView.Shop;
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
