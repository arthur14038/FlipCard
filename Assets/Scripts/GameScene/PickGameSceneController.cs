using UnityEngine;
using System.Collections;
using UnityEngine.Advertisements;

public class PickGameSceneController : AbstractController
{
	public PickGameView pickGameView;
	public GameSettingView gameSettingView;
	public GameMainView gameMainView;
	PickModeJudgement judgement;
	PickGameRecord thisTimeRecord;

	protected override void Start()
	{
		if(GameMainLoop.Instance != null)
			GameMainLoop.Instance.RegisterController(this, LoadingComplete);		
	}

	public override IEnumerator Init()
	{
		judgement = new PickModeJudgement();
		judgement.exitGame = ExitGame;
		judgement.saveGameRecord = SaveTmpGameRecord;

		yield return StartCoroutine(gameSettingView.Init());
		yield return StartCoroutine(gameMainView.Init());
		yield return StartCoroutine(pickGameView.Init());
		yield return StartCoroutine(judgement.Init(gameMainView, gameSettingView, pickGameView));
		
		gameSettingView.HideUI(false);
		gameMainView.ShowUI(false);
		pickGameView.ShowUI(false);
	}
	
	void LoadingComplete()
	{
	}

	void SaveTmpGameRecord(PickGameRecord record)
	{
		thisTimeRecord = record;
	}

	void ExitGame()
	{
		AudioManager.Instance.StopMusic();
		int returnView = 4;

		bool showAd = false;
		
		if(thisTimeRecord != null)
		{
			ModelManager.Instance.SavePickGameRecord(thisTimeRecord);
		}

		if(showAd)
			StartCoroutine(WaitAdAndExit(returnView));
		else
			GameMainLoop.Instance.ChangeScene(SceneName.MainScene, returnView);
	}

	void Update()
	{
		if(judgement != null)
			judgement.JudgementUpdate();
	}

	IEnumerator WaitAdAndExit(int returnView)
	{
		while(!Advertisement.IsReady("video"))
			yield return null;

		Advertisement.Show("video");

		GameMainLoop.Instance.ChangeScene(SceneName.MainScene, returnView);
	}

	void OnDestroy()
	{
		GoogleAnalyticsManager.LogScreen(GoogleAnalyticsManager.ScreenName.LeaveGameScene);
	}
}
