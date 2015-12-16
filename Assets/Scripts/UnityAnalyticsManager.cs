using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Analytics;

public class UnityAnalyticsManager : SingletonMonoBehavior<UnityAnalyticsManager> {
	public enum EventType { OnClick2P, OnClickShop, OnClickRate, OnClickMail, PlayGame }

	public void SendCustomEvent(EventType eventType, Dictionary<string, object> eventData = null)
	{
		Analytics.CustomEvent(eventType.ToString(), eventData);
    }

	public void SendCustomEvent(EventType eventType, Dictionary<string, int> eventData)
	{
		Dictionary<string, object> data = new Dictionary<string, object>();
		foreach(KeyValuePair<string, int> kvp in eventData)
		{
			data.Add(kvp.Key, kvp.Value);
		}
        Analytics.CustomEvent(eventType.ToString(), data);
	}
}
