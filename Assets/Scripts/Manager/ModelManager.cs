using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;

public class ModelManager : SingletonMonoBehavior<ModelManager> {
	Dictionary<CardArrayLevel, GameRecord> timeModeRecordDict;
	Dictionary<CardArrayLevel, GameRecord> classicModeRecordDict;
	string gameRecordFileName = "GameRecord.json";
	string timeModeRecordFileName = "TimeModeRecord.json";
	string classicModeRecordFileName = "ClassicModeRecord.json";
	string encodeKey = "FlipCard";
	string encodeIV = "VeryGood";
	const int version = 1;

	public void Init()
	{
		string filePath = GetSaveFilePath(gameRecordFileName);
		WriteFileTool.DeleteFile(filePath);
		
		string timeModeRecordPath = GetSaveFilePath(timeModeRecordFileName);
		string encodedString = WriteFileTool.LoadFile(timeModeRecordPath);

		if(!string.IsNullOrEmpty(encodedString))
		{
			string jsonString = EncodeTool.GetDecodedBase64(encodedString, encodeKey, encodeIV);
			timeModeRecordDict = JsonConvert.DeserializeObject<Dictionary<CardArrayLevel, GameRecord>>(jsonString);			
		}else
		{
			timeModeRecordDict = new Dictionary<CardArrayLevel, GameRecord>();
        }

		string classicModeRecordPath = GetSaveFilePath(classicModeRecordFileName);
		encodedString = WriteFileTool.LoadFile(classicModeRecordPath);

		if(!string.IsNullOrEmpty(encodedString))
		{
			string jsonString = EncodeTool.GetDecodedBase64(encodedString, encodeKey, encodeIV);
			classicModeRecordDict = JsonConvert.DeserializeObject<Dictionary<CardArrayLevel, GameRecord>>(jsonString);
		} else
		{
			classicModeRecordDict = new Dictionary<CardArrayLevel, GameRecord>();
		}
	}

	public void SaveGameRecord(GameRecord record)
	{
		string jsonString = "";
		string encodedString = "";
		string filePath = "";
        switch(record.mode)
		{
			case GameMode.Classic:
				if(classicModeRecordDict.ContainsKey(record.level))
					classicModeRecordDict[record.level] = record;
				else
					classicModeRecordDict.Add(record.level, record);

				jsonString = JsonConvert.SerializeObject(classicModeRecordDict);
				encodedString = EncodeTool.GetEncodedBase64(jsonString, encodeKey, encodeIV);
				filePath = GetSaveFilePath(classicModeRecordFileName);
				WriteFileTool.WriteFile(filePath, encodedString);
				break;
			case GameMode.LimitTime:
				if(timeModeRecordDict.ContainsKey(record.level))
					timeModeRecordDict[record.level] = record;
				else
					timeModeRecordDict.Add(record.level, record);

				jsonString = JsonConvert.SerializeObject(timeModeRecordDict);
				encodedString = EncodeTool.GetEncodedBase64(jsonString, encodeKey, encodeIV);
				filePath = GetSaveFilePath(timeModeRecordFileName);
				WriteFileTool.WriteFile(filePath, encodedString);
				break;
		}
	}

	public GameRecord GetGameRecord(CardArrayLevel level, GameMode mode)
	{
		GameRecord record = null;
        switch(mode)
		{
			case GameMode.Classic:
				if(classicModeRecordDict.ContainsKey(level))
					record = classicModeRecordDict[level];
				break;
			case GameMode.LimitTime:
				if(timeModeRecordDict.ContainsKey(level))
					record = timeModeRecordDict[level];
				break;
		}

		if(record == null)
		{
			record = new GameRecord();
            record.level = level;
			record.mode = mode;
			record.highScore = 0;
			record.playTimes = 0;
			record.grade = 0;
		}
		return record;
	}
	
	public Dictionary<string, object> GetGameRecordForSendEvent(GameMode mode)
	{
		Dictionary<string, object> eventData = new Dictionary<string, object>();

		switch(mode)
		{
			case GameMode.Classic:
				foreach(KeyValuePair<CardArrayLevel, GameRecord> kvp in classicModeRecordDict)
				{
					eventData.Add(kvp.Key.ToString(), JsonConvert.SerializeObject(kvp.Value));
                }
				break;
			case GameMode.LimitTime:
				foreach(KeyValuePair<CardArrayLevel, GameRecord> kvp in timeModeRecordDict)
				{
					eventData.Add(kvp.Key.ToString(), JsonConvert.SerializeObject(kvp.Value));
				}
				break;
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