using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Analytics;

public class UnityAnalyticsManager : SingletonMonoBehavior<UnityAnalyticsManager> {
	public enum EventType {OnClickShop, OnClickComingSoon, OnClickThemeInfo, OnClickRate, 
		InfiniteGameRecord}

	public void SendCustomEvent(EventType eventType, Dictionary<string, object> eventData = null)
	{
		#if UNITY_EDITOR
		//if(eventData != null)
			Debug.LogFormat("Send Event {0}: {1}", eventType, Newtonsoft.Json.JsonConvert.SerializeObject(eventData));
		#endif
		Analytics.CustomEvent(eventType.ToString(), eventData);
    }
}
