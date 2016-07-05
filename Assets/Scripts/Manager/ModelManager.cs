using UnityEngine;
using System.IO;
using Newtonsoft.Json;

public class ModelManager : SingletonMonoBehavior<ModelManager> {
	string gameRecordFileName = "GameRecord.json";
	string timeModeRecordFileName = "TimeModeRecord.json";
	string classicModeRecordFileName = "ClassicModeRecord.json";
	string flipCardRecordFileName = "FlipCardRecord.json";
	string pickGameRecordFileName = "PickGameRecord.json";
	string encodeKey = "FlipCard";
	string encodeIV = "VeryGood";
	NormalGameRecord flipCardGameRecord;
	PickGameRecord pickGameRecord;

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
			flipCardGameRecord = JsonConvert.DeserializeObject<NormalGameRecord>(jsonString);
			if(flipCardGameRecord.achievement == null)
				flipCardGameRecord.achievement = new bool[] { false, false, false, false, false, false };
		}

		string pickGameRecordPath = GetSaveFilePath(pickGameRecordFileName);
		encodedString = WriteFileTool.LoadFile(pickGameRecordPath);

		if(!string.IsNullOrEmpty(encodedString))
		{
			string jsonString = EncodeTool.GetDecodedBase64(encodedString, encodeKey, encodeIV);
			pickGameRecord = JsonConvert.DeserializeObject<PickGameRecord>(jsonString);
		}
	}

	public void SaveFlipCardGameRecord(NormalGameRecord record)
	{
		flipCardGameRecord = record;
		if(!PlayerPrefsManager.FirstInfiniteAchievement)
		{
			if(record.achievement[0] && record.achievement[1] && record.achievement[2])
			{
				PlayerPrefsManager.FirstInfiniteAchievement = true;
				InventoryManager.Instance.AddMoni(220);
            }
        }

		if(!PlayerPrefsManager.SecondInfiniteAchievement)
		{
			if(record.achievement[3] && record.achievement[4] && record.achievement[5])
				PlayerPrefsManager.SecondInfiniteAchievement = true;
		}
		string jsonString = JsonConvert.SerializeObject(flipCardGameRecord);
		string encodedString = EncodeTool.GetEncodedBase64(jsonString, encodeKey, encodeIV);
		string filePath = GetSaveFilePath(flipCardRecordFileName);
		WriteFileTool.WriteFile(filePath, encodedString);
	}

	public NormalGameRecord GetFlipCardGameRecord()
	{
		if(flipCardGameRecord == null)
		{
			flipCardGameRecord = new NormalGameRecord();
			flipCardGameRecord.lastLevel = new int[] { 0, 0, 0 };
			flipCardGameRecord.lastScore = new int[] { 0, 0, 0 };
			flipCardGameRecord.achievement = new bool[] { false, false, false, false, false, false };
            flipCardGameRecord.playTimes = 0;
			flipCardGameRecord.highLevel = 0;
			flipCardGameRecord.highScore = 0;
        }
		return flipCardGameRecord;
	}

	public void SavePickGameRecord(PickGameRecord record)
	{
		pickGameRecord = record;
		string jsonString = JsonConvert.SerializeObject(pickGameRecord);
		string encodedString = EncodeTool.GetEncodedBase64(jsonString, encodeKey, encodeIV);
		string filePath = GetSaveFilePath(pickGameRecordFileName);
		WriteFileTool.WriteFile(filePath, encodedString);
	}

	public PickGameRecord GetPickGameRecord()
	{
		if(pickGameRecord == null)
		{
			pickGameRecord = new PickGameRecord();
			pickGameRecord.lastLevels = new int[] { 0, 0, 0 };
			pickGameRecord.lastScores = new int[] { 0, 0, 0 };
			pickGameRecord.highLevel = 0;
			pickGameRecord.highScore = 0;
        }
		return pickGameRecord;
    }
	
	public int GetInfiniteScore()
	{
		return PlayerPrefsManager.InfiniteScore;
    }

	public void AddInfiniteScore(int addAmount)
	{
		PlayerPrefsManager.InfiniteScore += addAmount;
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

public class NormalGameRecord
{
	public int highLevel;
	public int[] lastLevel;
	public int highScore;
	public int[] lastScore;
	public bool[] achievement;
	public int playTimes;
}

public class PickGameRecord
{
	public int highLevel;
	public int[] lastLevels;
	public int highScore;
	public int[] lastScores;
	public int playTimes;
}