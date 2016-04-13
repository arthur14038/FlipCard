using UnityEngine;
using System.Collections;

public class GoogleAnalyticsManager
{	
	static bool CanSendGA
	{
		get
		{
			if(GoogleAnalyticsV3.instance == null)
				return false;

			return true;
		}
	}

#if UNITY_EDITOR
	static bool TestGA = true;
#else
	static bool TestGA = false;
#endif

	public static void LogScreen(string screenName)
	{
		if(TestGA)
			Debug.LogFormat("LogScreen: {0}", screenName);
		else
		{
			if(!CanSendGA)
				return;

			if(string.IsNullOrEmpty(screenName))
				return;

			GoogleAnalyticsV3.instance.LogScreen(screenName);
		}
	}

	public static void LogEvent(string category, string action, string label = null)
	{
		if(TestGA)
			Debug.LogFormat("LogEvent: {0} {1} {2}", category, action, label);
		else
		{
			if(!CanSendGA)
				return;

			if(string.IsNullOrEmpty(category) || string.IsNullOrEmpty(action))
				return;

			EventHitBuilder eventHitBuilder = new EventHitBuilder();
			eventHitBuilder.SetEventCategory(category);
			eventHitBuilder.SetEventAction(action);
			eventHitBuilder.SetEventLabel(label);

			GoogleAnalyticsV3.instance.LogEvent(eventHitBuilder);
		}
	}

	public static void LogEvent(string category, string action, long value, string label = null)
	{
		if(TestGA)
			Debug.LogFormat("LogEvent: {0} {1} {2} {3}", category, action, value, label);
		else
		{
			if(!CanSendGA)
				return;

			if(string.IsNullOrEmpty(category) || string.IsNullOrEmpty(action))
				return;

			EventHitBuilder eventHitBuilder = new EventHitBuilder();
			eventHitBuilder.SetEventCategory(category);
			eventHitBuilder.SetEventAction(action);
			eventHitBuilder.SetEventLabel(label);
			eventHitBuilder.SetEventValue(value);

			GoogleAnalyticsV3.instance.LogEvent(eventHitBuilder);
		}
	}

	public static void LogTiming(string category, long value, string name = null, string label = null)
	{
		if(TestGA)
			Debug.LogFormat("LogTiming: {0} {1} {2} {3}", category, value, name, label);
		else
		{
			if(!CanSendGA)
				return;

			if(string.IsNullOrEmpty(category))
				return;

			TimingHitBuilder timingHitBuilder = new TimingHitBuilder();
			timingHitBuilder.SetTimingCategory(category);
			timingHitBuilder.SetTimingInterval(value);
			timingHitBuilder.SetTimingName(name);
			timingHitBuilder.SetTimingLabel(label);

			GoogleAnalyticsV3.instance.LogTiming(timingHitBuilder);
		}
	}

	public struct ScreenName
	{
		public const string FirstScene = "入口場景";
		public const string LeaveFirstScene = "離開入口場景";
		public const string MainSceneMainPage = "主場景-主頁面";
		public const string MainSceneShop = "主場景-商店";
		public const string MainSceneTwoPlayer = "主場景-雙人";
		public const string MainSceneInfiniteMode = "主場景-無限";
		public const string LeaveMainScene = "離開主場景";
		public const string GameSceneTwoPlayer = "遊戲場景-雙人";
		public const string GameSceneInfiniteMode = "遊戲場景-無限";
		public const string LeaveGameScene = "離開遊戲場景";
	}

	public struct EventCategory
	{
		public const string UserClickEvent = "使用者點擊事件";
		public const string BuyTheme = "購買主題";
		public const string GameRecord = "過關紀錄";
		public const string QuitGame = "放棄遊戲";
	}

	public struct EventAction
	{
		public const string ClickThemeInfo = "點擊主題內容";
		public const string ClickComingSoon = "點擊ComingSoon";
		public const string ClickRate = "點擊評價";
		public const string ClickFacebook = "點擊Facebook";
		public const string ClickTwoPlayer = "點擊雙人遊戲關卡";
		public const string ClickMoniBoard = "點擊Moni板";
		public const string ConfirmBuyTheme = "確認購買主題";
		public const string CancelBuyTheme = "取消購買主題";
	}
}
