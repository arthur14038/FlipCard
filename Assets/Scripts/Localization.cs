using UnityEngine;
using System;
using System.Collections.Generic;
using Newtonsoft.Json;

public class Localization{
	static Dictionary<string, StringTable> dic_Localization = new Dictionary<string, StringTable>();
	public enum LocalizationType { zh_tw, en_us }
	static LocalizationType currentType = LocalizationType.zh_tw;
	//static LocalizationType currentType = LocalizationType.en_us;
	public static Action Event_ChangeLocaliztion = delegate { };

	public static void Init()
	{
		string jsonString = ((TextAsset)Resources.Load("Localization")).text;

		List<StringTable> tmpList = JsonConvert.DeserializeObject<List<StringTable>>(jsonString);

		foreach(StringTable table in tmpList)
		{
			if(dic_Localization.ContainsKey(table.key))
			{
				dic_Localization[table.key] = table;
            } else
			{
				dic_Localization.Add(table.key, table);
            }
		}
		switch(Application.systemLanguage)
		{
			case SystemLanguage.Chinese:
			case SystemLanguage.ChineseSimplified:
			case SystemLanguage.ChineseTraditional:
				currentType = LocalizationType.zh_tw;
                break;
			default:
				currentType = LocalizationType.en_us;
				break;
		}
	}

	public static string Get(string key)
	{
		if(dic_Localization.ContainsKey(key))
		{
			switch(currentType)
			{
				case LocalizationType.en_us:
					return dic_Localization[key].en_us;
				case LocalizationType.zh_tw:
					return dic_Localization[key].zh_tw;
                default:
					return key;
			}
		} else
		{
			Debug.LogWarningFormat("Can not get {0} value", key);
			return key;
		}
	}

	public static void SetType(LocalizationType type)
	{
		if(currentType != type)
		{
			currentType = type;
			Event_ChangeLocaliztion();
		}
	}
}

class StringTable
{
	public string key;
	public string zh_tw;
	public string en_us;
}