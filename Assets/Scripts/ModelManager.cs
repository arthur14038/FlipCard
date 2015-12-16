using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;

public class ModelManager : SingletonMonoBehavior<ModelManager> {
	Dictionary<CardArrayLevel, GameRecord> gameRecordDictionary;
	Dictionary<string, int> playTimesDictionary;
	string gameRecordFileName = "GameRecord.json";
	string playTimesFileName = "PlayTimes.json";
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

	public int AddPlayTimes(CardArrayLevel level)
	{
		int addUpPlayTimes = 0;
		if(playTimesDictionary.ContainsKey(level.ToString()))
		{
			playTimesDictionary[level.ToString()] += 1;
			addUpPlayTimes = playTimesDictionary[level.ToString()];
        } else
		{
			addUpPlayTimes = 1;
            playTimesDictionary.Add(level.ToString(), 1);
        }
		string jsonString = JsonConvert.SerializeObject(playTimesDictionary);
		string encodedString = EncodeTool.GetEncodedBase64(jsonString, encodeKey, encodeIV);
		string filePath = GetSaveFilePath(playTimesFileName);
		WriteFileTool.WriteFile(filePath, encodedString);
		return addUpPlayTimes;
    }

	public Dictionary<string, int> GetPlayTimes()
	{
		return playTimesDictionary;
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
