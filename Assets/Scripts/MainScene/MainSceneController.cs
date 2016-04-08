using UnityEngine;
using System.Collections;

public class MainSceneController : AbstractController {
	enum MainSceneView{MainPage = 0, FlipCard, TwoPlayer, Shop}
	MainSceneView currentView;
	public MainPageView mainPageView;
	public FlipCardView flipCardView;
	public TwoPlayerView twoPlayerView;
	public ShopView shopView;
	string notifyMessage;
	ThemeInfo wantedTheme;
		
	public override IEnumerator Init ()
	{
		yield return StartCoroutine(mainPageView.Init());
		yield return StartCoroutine(flipCardView.Init());
		yield return StartCoroutine(twoPlayerView.Init());
		yield return StartCoroutine(shopView.Init());

		currentView = (MainSceneView)GameMainLoop.Instance.showView;

		mainPageView.onClickFlipCard = ShowFlipCardPage;
		mainPageView.onClick2P = ShowTwoPlayers;
		mainPageView.onClickShop = ShowShop;
		mainPageView.onClickMail = SendMailToUs;
		mainPageView.onClickRate = ShowRatePage;
		mainPageView.onClickLeaveGame = LeaveGame;
		mainPageView.onClickNotify = SendNotifyMail;
		mainPageView.onClickComingSoon = StillInProgress;
		mainPageView.onClickGoToFacebook = OpenFacebookPage;
        flipCardView.onClickBack = ShowMainPage;
		flipCardView.onClickPlay = StartFlipCardGame;
		twoPlayerView.onClickBack = ShowMainPage;
		twoPlayerView.onClickPlay = StartTwoPlayerGame;
		shopView.onClickBack = ShowMainPage;
		shopView.onClickThemePrice = CheckCanAfford;
		shopView.onClickEquipCard = EquipCard;
		shopView.onClickEquipTheme = EquipTheme;
		shopView.onClickConfirmBuyTheme = BuyThemePack;
		shopView.onClickThemeInfo = ShowThemeInfo;

		shopView.HideUI(false);
        mainPageView.HideUI(false);
		flipCardView.HideUI(false);
		twoPlayerView.HideUI(false);

		switch(currentView)
		{
			case MainSceneView.MainPage:
				GoogleAnalyticsManager.LogScreen(GoogleAnalyticsManager.ScreenName.MainSceneMainPage);
				mainPageView.ShowUI(false);
				break;
			case MainSceneView.FlipCard:
				GoogleAnalyticsManager.LogScreen(GoogleAnalyticsManager.ScreenName.MainSceneInfiniteMode);
				flipCardView.ShowUI(false);
				break;
			case MainSceneView.TwoPlayer:
				GoogleAnalyticsManager.LogScreen(GoogleAnalyticsManager.ScreenName.MainSceneTwoPlayer);
				twoPlayerView.ShowUI(false);
				break;
			case MainSceneView.Shop:
				GoogleAnalyticsManager.LogScreen(GoogleAnalyticsManager.ScreenName.MainSceneShop);
				shopView.ShowUI(false);
				break;
		}
		
		AudioManager.Instance.PlayMusic("FlipCardBGM3", true); 
	}

	void StartTwoPlayerGame(int cardCount)
	{
		GoogleAnalyticsManager.LogEvent(GoogleAnalyticsManager.EventCategory.UserClickEvent,
			GoogleAnalyticsManager.EventAction.ClickTwoPlayer, cardCount.ToString());
        AudioManager.Instance.StopMusic();
		GameSettingManager.currentMode = GameMode.Competition;
		GameSettingManager.currentCardCount = cardCount;
        GameMainLoop.Instance.ChangeScene(SceneName.GameScene);
	}
	
	void StartFlipCardGame()
	{
		AudioManager.Instance.StopMusic();
		GameSettingManager.currentMode = GameMode.FlipCard;
		GameMainLoop.Instance.ChangeScene(SceneName.GameScene);
	}

	void ShowMainPage()
	{
		GoogleAnalyticsManager.LogScreen(GoogleAnalyticsManager.ScreenName.MainSceneMainPage);
		switch(currentView)
		{
			case MainSceneView.FlipCard:
				flipCardView.HideUI(true);
				break;
			case MainSceneView.TwoPlayer:
				twoPlayerView.HideUI(true);
				break;
			case MainSceneView.Shop:
				shopView.HideUI(true);
				break;
		}
		mainPageView.ShowUI(true);
		currentView = MainSceneView.MainPage;
	}

	void ShowFlipCardPage()
	{
		GoogleAnalyticsManager.LogScreen(GoogleAnalyticsManager.ScreenName.MainSceneInfiniteMode);
		mainPageView.HideUI(true);
		flipCardView.ShowUI(true);
		currentView = MainSceneView.FlipCard;
	}
	
	void ShowTwoPlayers()
	{
		GoogleAnalyticsManager.LogScreen(GoogleAnalyticsManager.ScreenName.MainSceneTwoPlayer);
		mainPageView.HideUI(true);
		twoPlayerView.ShowUI(true);
		currentView = MainSceneView.TwoPlayer;
	}
	
	void ShowShop()
	{
		GoogleAnalyticsManager.LogScreen(GoogleAnalyticsManager.ScreenName.MainSceneShop);
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
		GoogleAnalyticsManager.LogEvent(GoogleAnalyticsManager.EventCategory.UserClickEvent, GoogleAnalyticsManager.EventAction.ClickRate);
		string storeUrl = "";
#if UNITY_IPHONE
		storeUrl = "https://itunes.apple.com/us/app/flip-card-polilu/id1076638340?l=zh&ls=1&mt=8";
#elif UNITY_ANDROID
		storeUrl = "https://play.google.com/store/apps/details?id=com.PlayClay.FlipCard";
#endif
		Application.OpenURL(storeUrl);
	}

	void StillInProgress()
	{
		GoogleAnalyticsManager.LogEvent(GoogleAnalyticsManager.EventCategory.UserClickEvent, GoogleAnalyticsManager.EventAction.ClickComingSoon);
		mainPageView.ShowUnderConstruction();
		notifyMessage = "Please notify me when \"New Mode\" feature is launches.";
	}

	void SendMailToUs()
	{
		string email = "playclaystudio@gmail.com";
		string subject = MyEscapeURL("User Feedback");
		Application.OpenURL("mailto:" + email + "?subject=" + subject);
	}

	void OpenFacebookPage()
	{
		GoogleAnalyticsManager.LogEvent(GoogleAnalyticsManager.EventCategory.UserClickEvent, GoogleAnalyticsManager.EventAction.ClickFacebook);
		Application.OpenURL("https://www.facebook.com/PlayClayStudio");
	}

	void EquipTheme(string themeItemId)
	{
		InventoryManager.Instance.EquipTheme(themeItemId);
		mainPageView.UpdateTheme();
		shopView.UpdateThemePackList();
	}

	void EquipCard(string cardBackItemId, string cardFaceItemId)
	{
		InventoryManager.Instance.EquipCardBack(cardBackItemId);
		InventoryManager.Instance.EquipCardFace(cardFaceItemId);
		shopView.UpdateThemePackList();
	}

	void CheckCanAfford(ThemeInfo themeInfo)
	{
		wantedTheme = themeInfo;
		if(InventoryManager.Instance.CanAfford(wantedTheme.themeItemId))
			shopView.ShowConfirmBuy(wantedTheme.requireMoni);
		else
			shopView.ShowMoniNotEnough();
	}

	void BuyThemePack()
	{
		GoogleAnalyticsManager.LogEvent(GoogleAnalyticsManager.EventCategory.BuyTheme,
			GoogleAnalyticsManager.EventAction.ConfirmBuyTheme, wantedTheme.themeItemId);
		InventoryManager.Instance.BuyTheme(wantedTheme.themeItemId, BuyThemePackCallback);
		shopView.ShowLoadingWindow();
	}

	void BuyThemePackCallback(bool success)
	{
		string message = "";
		ThemeInfo info = InventoryManager.Instance.GetThemeInfo(wantedTheme.themeItemId);
		if(success)
		{
			shopView.UpdateThemePackList();
			message = string.Format("Buying Success!\n<color=#007A80FF>{0}</color>\nnow in the theme list.", info.themeName);
		} else
		{
			message = string.Format("<i>{0}</i>\nBuying Failed", info.themeName);
		}
		shopView.ShowBuyMsg(message);
	}
	
	void BuyCurrencyPackCancel()
	{
		shopView.ShowBuyMsg("Transaction Cancel");
	}

	void ShowThemeInfo(string themeItemId)
	{
		GoogleAnalyticsManager.LogEvent(GoogleAnalyticsManager.EventCategory.UserClickEvent, 
			GoogleAnalyticsManager.EventAction.ClickThemeInfo, themeItemId);
		ThemeInfo themeInfo = InventoryManager.Instance.GetThemeInfo(themeItemId);
		shopView.ShowThemeInfo(themeInfo.themeName, themeInfo.themeContent);
	}

	void LeaveGame()
	{
		Application.Quit();
	}

	string MyEscapeURL(string url)
	{
		return WWW.EscapeURL(url).Replace("+", "%20");
	}

	void OnDestroy()
	{
		GoogleAnalyticsManager.LogScreen(GoogleAnalyticsManager.ScreenName.LeaveMainScene);
	}
}
