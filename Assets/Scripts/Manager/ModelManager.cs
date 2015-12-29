using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;

public class ModelManager : SingletonMonoBehavior<ModelManager> {
	Dictionary<CardArrayLevel, GameRecord> gameRecordDictionary;
	string gameRecordFileName = "GameRecord.json";
	string encodeKey = "FlipCard";
	string encodeIV = "VeryGood";

	public void Init()
	{
		string filePath = GetSaveFilePath(gameRecordFileName);
		string encodedString = WriteFileTool.LoadFile(filePath);
		if(!string.IsNullOrEmpty(encodedString))
		{
			string jsonString = EncodeTool.GetDecodedBase64(encodedString, encodeKey, encodeIV);
			gameRecordDictionary = JsonConvert.DeserializeObject<Dictionary<CardArrayLevel, GameRecord>>(jsonString);
		}else
		{
			gameRecordDictionary = new Dictionary<CardArrayLevel, GameRecord>();
		}
	}

	public void SaveGameRecord(GameRecord record)
	{
		if(gameRecordDictionary.ContainsKey(record.level))
		{
			gameRecordDictionary[record.level] = record;
		}else
		{
			gameRecordDictionary.Add(record.level, record);
		}
		string jsonString = JsonConvert.SerializeObject(gameRecordDictionary);
		string encodedString = EncodeTool.GetEncodedBase64(jsonString, encodeKey, encodeIV);
		string filePath = GetSaveFilePath(gameRecordFileName);
		WriteFileTool.WriteFile(filePath, encodedString);
	}

	public GameRecord GetGameRecord(CardArrayLevel level)
	{
		if(gameRecordDictionary.ContainsKey(level))
		{
			return gameRecordDictionary[level];
		}else
		{
			GameRecord record = new GameRecord();
			record.level = level;
			record.highScore = 0;
			record.playTimes = 0;
			return record;
		}
	}

	public Dictionary<string, object> GetAllGameRecord()
	{
		Dictionary<string, object> eventData = new Dictionary<string, object>();
		foreach(KeyValuePair<CardArrayLevel, GameRecord> kvp in gameRecordDictionary)
		{
			eventData.Add(kvp.Key.ToString(), JsonConvert.SerializeObject(kvp.Value));
		}
		return eventData;
	}
		
	string GetSaveFilePath(string fileName)
	{
		string saveFilePath = "";
		#if UNITY_IPHONE			
		string fileNameBase = Application.dataPath.Substring(0, Application.dataPath.LastIndexOf('/'));
		saveFilePath = fileNameBase.Substring(0, fileNameBase.LastIndexOf('/')) + "/Documents/" + fileName;
		#elif UNITY_ANDROID
		saveFilePath = Application.persistentDataPath + "/" + fileName ;
		#else
		saveFilePath = Application.dataPath + "/" + fileName;
		#endif
		return saveFilePath;
	}
}
