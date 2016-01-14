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
	ThemePack wantedThemePack;
		
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
		mainPageView.onClickComingSoon = StillInProgress;
        classicModeView.onClickBack = ShowMainPage;
		classicModeView.onClickPlay = GoToGameScene;
		twoPlayerView.onClickBack = ShowMainPage;
		twoPlayerView.onClickPlay = GoToGameScene;
		timeModeView.onClickBack = ShowMainPage;
		timeModeView.onClickPlay = GoToGameScene;
		shopView.onClickBack = ShowMainPage;
		shopView.onClickThemePrice = CheckCanAfford;
		shopView.onClickEquipCard = EquipCard;
		shopView.onClickEquipTheme = EquipTheme;
		shopView.onClickBuyMoniPack = BuyMoniPack;
		shopView.onClickConfirmBuyTheme = BuyThemePack;
		shopView.onClickThemeInfo = ShowThemeInfo;

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
		//GameMainLoop.Instance.ChangeScene(SceneName.TestGame);
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

	void StillInProgress()
	{
		UnityAnalyticsManager.Instance.SendCustomEvent(UnityAnalyticsManager.EventType.OnClick2P);
		mainPageView.ShowUnderConstruction();
		notifyMessage = "Please notify me when \"New Mode\" feature is launches.";
	}

	void SendMailToUs()
	{
		string email = "playclaystudio@gmail.com";
		string subject = MyEscapeURL("User Feedback");
		Application.OpenURL("mailto:" + email + "?subject=" + subject);
	}

	void EquipTheme(string themeItemId)
	{
		InventoryManager.Instance.EquipItem(themeItemId);
		mainPageView.UpdateTheme();
		shopView.UpdateThemePackList();
	}

	void EquipCard(string cardBackItemId, string cardFaceItemId)
	{
		InventoryManager.Instance.EquipItem(cardBackItemId);
		InventoryManager.Instance.EquipItem(cardFaceItemId);
		shopView.UpdateThemePackList();
	}

	void CheckCanAfford(ThemePack themePack)
	{
		wantedThemePack = themePack;
		if(wantedThemePack.theme.CanAfford())
			shopView.ShowConfirmBuy(wantedThemePack.theme);
		else
			shopView.ShowMoniNotEnough();
	}

	void BuyThemePack()
	{
		InventoryManager.Instance.BuyThemePack(wantedThemePack, BuyThemePackCallback);
		shopView.ShowLoadingWindow();
	}

	void BuyThemePackCallback(bool success)
	{
		string message = "";
		ThemeInfo info = InventoryManager.Instance.GetThemeInfo(wantedThemePack.theme.ItemId);
		if(success)
		{
			shopView.UpdateThemePackList();
			message = string.Format("Buying Success!\n<color=#007A80FF>{0}</color>\nnow in the theme list", info.themeName);
		} else
		{
			message = string.Format("<i>{0}</i>\nBuying Failed", info.themeName);
		}
		shopView.ShowBuyMsg(message);
	}
	
	void BuyMoniPack(int tier)
	{
		string moniPackItemId = "";
		switch(tier)
		{
			case 3:
				moniPackItemId = FlipCardStoreAsset.MONI_PACK_TIER3_ITEM_ID;
				break;
			case 4:
				moniPackItemId = FlipCardStoreAsset.MONI_PACK_TIER4_ITEM_ID;
				break;
			case 5:
				moniPackItemId = FlipCardStoreAsset.MONI_PACK_TIER5_ITEM_ID;
				break;
			case 11:
				moniPackItemId = FlipCardStoreAsset.MONI_PACK_TIER11_ITEM_ID;
				break;
			case 23:
				moniPackItemId = FlipCardStoreAsset.MONI_PACK_TIER23_ITEM_ID;
				break;
			case 45:
				moniPackItemId = FlipCardStoreAsset.MONI_PACK_TIER45_ITEM_ID;
				break;
		}
		InventoryManager.Instance.BuyCurrencyPack(moniPackItemId, shopView.ShowBuyMsg, BuyCurrencyPackCancel);
		shopView.ShowLoadingWindow();
	}
	
	void BuyCurrencyPackCancel()
	{
		shopView.ShowBuyMsg("Transaction Cancel");
	}

	void ShowThemeInfo(string themeItemId)
	{
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
}
