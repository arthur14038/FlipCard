using UnityEngine;
using Newtonsoft.Json;

public class ModelManager : SingletonMonoBehavior<ModelManager> {
	string gameRecordFileName = "GameRecord.json";
	string timeModeRecordFileName = "TimeModeRecord.json";
	string classicModeRecordFileName = "ClassicModeRecord.json";
	string flipCardRecordFileName = "FlipCardRecord.json";
	string encodeKey = "FlipCard";
	string encodeIV = "VeryGood";
	GameRecord flipCardGameRecord;

	public void Init()
	{
		string filePath = GetSaveFilePath(gameRecordFileName);
		WriteFileTool.DeleteFile(filePath);
		
		string timeModeRecordPath = GetSaveFilePath(timeModeRecordFileName);
		WriteFileTool.DeleteFile(timeModeRecordPath);

		string classicModeRecordPath = GetSaveFilePath(classicModeRecordFileName);
		WriteFileTool.DeleteFile(classicModeRecordPath);

		string flipCardRecordPath = GetSaveFilePath(flipCardRecordFileName);
		string encodedString = WriteFileTool.LoadFile(flipCardRecordPath);

		if(!string.IsNullOrEmpty(encodedString))
		{
			string jsonString = EncodeTool.GetDecodedBase64(encodedString, encodeKey, encodeIV);
			flipCardGameRecord = JsonConvert.DeserializeObject<GameRecord>(jsonString);
		}
    }

	public void SaveFlipCardGameRecord(GameRecord record)
	{
		flipCardGameRecord = record;
		string jsonString = JsonConvert.SerializeObject(flipCardGameRecord);
		string encodedString = EncodeTool.GetEncodedBase64(jsonString, encodeKey, encodeIV);
		string filePath = GetSaveFilePath(flipCardRecordFileName);
		WriteFileTool.WriteFile(filePath, encodedString);
	}

	public GameRecord GetFlipCardGameRecord()
	{
		return flipCardGameRecord;
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

public class GameRecord
{
	public int highLevel;
	public int[] lastLevel;
	public int highScore;
	public int[] lastScore;
	public int playTimes;
}