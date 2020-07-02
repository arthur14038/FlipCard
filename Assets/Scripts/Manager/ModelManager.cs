using UnityEngine;
using System.Collections.Generic;
using Newtonsoft.Json;

public class ModelManager : SingletonMonoBehavior<ModelManager> {
	Dictionary<LevelDifficulty, GameRecord> timeModeRecordDict;
	Dictionary<LevelDifficulty, GameRecord> classicModeRecordDict;
	string gameRecordFileName = "GameRecord.json";
	string timeModeRecordFileName = "TimeModeRecord.json";
	string classicModeRecordFileName = "ClassicModeRecord.json";
	string encodeKey = "FlipCard";
	string encodeIV = "VeryGood";
	const int ClassicModeRecordVersion = 0;
	const int TimeModeRecordVersion = 1;

	public void Init()
	{
		Debug.Log("ModelManager Init");

		string filePath = GetSaveFilePath(gameRecordFileName);
		WriteFileTool.DeleteFile(filePath);
		
		string timeModeRecordPath = GetSaveFilePath(timeModeRecordFileName);
		string encodedString = WriteFileTool.LoadFile(timeModeRecordPath);

		if(!string.IsNullOrEmpty(encodedString))
		{
			string jsonString = EncodeTool.GetDecodedBase64(encodedString, encodeKey, encodeIV);
			timeModeRecordDict = JsonConvert.DeserializeObject<Dictionary<LevelDifficulty, GameRecord>>(jsonString);

			if(!timeModeRecordDict.ContainsKey(LevelDifficulty.Lock))
			{				
				GameRecord timeModeVersionControl = new GameRecord();
				timeModeVersionControl.highScore = 0;
				timeModeRecordDict.Add(LevelDifficulty.Lock, timeModeVersionControl);
			}
			
			if(timeModeRecordDict[LevelDifficulty.Lock].highScore != TimeModeRecordVersion)
			{
				timeModeRecordDict.Clear();
				WriteFileTool.DeleteFile(timeModeRecordPath);
			}
		}else
		{
			timeModeRecordDict = new Dictionary<LevelDifficulty, GameRecord>();
        }

		string classicModeRecordPath = GetSaveFilePath(classicModeRecordFileName);
		encodedString = WriteFileTool.LoadFile(classicModeRecordPath);

		if(!string.IsNullOrEmpty(encodedString))
		{
			string jsonString = EncodeTool.GetDecodedBase64(encodedString, encodeKey, encodeIV);
			classicModeRecordDict = JsonConvert.DeserializeObject<Dictionary<LevelDifficulty, GameRecord>>(jsonString);

			if(!classicModeRecordDict.ContainsKey(LevelDifficulty.Lock))
			{
				GameRecord classicModeVersionControl = new GameRecord();
				classicModeVersionControl.highScore = 0;
				classicModeRecordDict.Add(LevelDifficulty.Lock, classicModeVersionControl);
			}

			if(classicModeRecordDict[LevelDifficulty.Lock].highScore != ClassicModeRecordVersion)
			{
				classicModeRecordDict.Clear();
				WriteFileTool.DeleteFile(classicModeRecordPath);
			}
		} else
		{
			classicModeRecordDict = new Dictionary<LevelDifficulty, GameRecord>();
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

			GameRecord classicModeVersionControl = new GameRecord();
			classicModeVersionControl.highScore = ClassicModeRecordVersion;
						
			if(classicModeRecordDict.ContainsKey(LevelDifficulty.Lock))
				classicModeRecordDict[LevelDifficulty.Lock] = classicModeVersionControl;
			else
				classicModeRecordDict.Add(LevelDifficulty.Lock, classicModeVersionControl);

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

			GameRecord timeModeVersionControl = new GameRecord();
			timeModeVersionControl.highScore = TimeModeRecordVersion;

			if(timeModeRecordDict.ContainsKey(LevelDifficulty.Lock))
				timeModeRecordDict[LevelDifficulty.Lock] = timeModeVersionControl;
			else
				timeModeRecordDict.Add(LevelDifficulty.Lock, timeModeVersionControl);

				jsonString = JsonConvert.SerializeObject(timeModeRecordDict);
				encodedString = EncodeTool.GetEncodedBase64(jsonString, encodeKey, encodeIV);
				filePath = GetSaveFilePath(timeModeRecordFileName);
				WriteFileTool.WriteFile(filePath, encodedString);
				break;
		}
	}

	public GameRecord GetGameRecord(LevelDifficulty level, GameMode mode)
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
				foreach(KeyValuePair<LevelDifficulty, GameRecord> kvp in classicModeRecordDict)
				{
					eventData.Add(kvp.Key.ToString(), JsonConvert.SerializeObject(kvp.Value));
                }
				break;
			case GameMode.LimitTime:
				foreach(KeyValuePair<LevelDifficulty, GameRecord> kvp in timeModeRecordDict)
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
		string path = Application.persistentDataPath.Substring( 0, Application.persistentDataPath.Length - 5 );
		path = path.Substring( 0, path.LastIndexOf( '/' ) );
		saveFilePath = Path.Combine( Path.Combine( path, "Documents" ), fileName );
		#elif UNITY_ANDROID
		saveFilePath = Application.persistentDataPath + "/" + fileName ;
		#else
		saveFilePath = Application.dataPath + "/" + fileName;
		#endif
		return saveFilePath;
	}
}