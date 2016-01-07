using UnityEngine;
using UnityEditor;
using System.Collections;

public class FlipCardEditor : EditorWindow
{
	bool showPlayerMessage = true;
	int unlockMode;
	int classicModeProgress;
	int timeModeProgress;

	[MenuItem("Window/FlipCardEditor")]
	static void Init()
	{
		FlipCardEditor window = (FlipCardEditor)EditorWindow.GetWindow(typeof(FlipCardEditor));
		window.autoRepaintOnSceneChange = true;
	}

	void OnGUI()
	{
		if(GUILayout.Button("PlayerMessage"))
		{
			showPlayerMessage = !showPlayerMessage;
		}
		if(showPlayerMessage)
		{
			GUILayout.Label("MusicSetting: " + PlayerPrefsManager.MusicSetting);
			GUILayout.Label("SoundSetting: " + PlayerPrefsManager.SoundSetting);
			GUILayout.Label("UnlockMode: " + PlayerPrefsManager.UnlockMode);
			GUILayout.Label("ClassicModeProgress: " + PlayerPrefsManager.ClassicModeProgress);
			GUILayout.Label("TimeModeProgress: " + PlayerPrefsManager.TimeModeProgress);
		}

		GUILayout.BeginHorizontal();
		if(GUILayout.Button("設定UnlockMode", GUILayout.Width(250)))
		{
			PlayerPrefsManager.UnlockMode = unlockMode;
		}
		unlockMode = EditorGUILayout.IntField(unlockMode);
		GUILayout.EndHorizontal();

		GUILayout.BeginHorizontal();
		if(GUILayout.Button("設定ClassicModeProgress", GUILayout.Width(250)))
		{
			PlayerPrefsManager.ClassicModeProgress = classicModeProgress;
		}
		classicModeProgress = EditorGUILayout.IntField(classicModeProgress);
		GUILayout.EndHorizontal();

		GUILayout.BeginHorizontal();
		if(GUILayout.Button("設定TimeModeProgress", GUILayout.Width(250)))
		{
			PlayerPrefsManager.TimeModeProgress = timeModeProgress;
		}
		timeModeProgress = EditorGUILayout.IntField(timeModeProgress);
		GUILayout.EndHorizontal();

		GUILayout.BeginHorizontal();
		if(GUILayout.Button("清除PlayerPrefs"))
		{
			PlayerPrefs.DeleteAll();
			DeleteGameRecord();
        }
		GUILayout.EndHorizontal();
	}

	void DeleteGameRecord()
	{
		string filePath = GetSaveFilePath("GameRecord.json");
		string timeModeRecordPath = GetSaveFilePath("TimeModeRecord.json");
		string classicModeRecordPath = GetSaveFilePath("ClassicModeRecord.json");
		WriteFileTool.DeleteFile(filePath);
		WriteFileTool.DeleteFile(timeModeRecordPath);
		WriteFileTool.DeleteFile(classicModeRecordPath);
	}

	string GetSaveFilePath(string fileName)
	{
		return Application.persistentDataPath + "/" + fileName;
	}
}
