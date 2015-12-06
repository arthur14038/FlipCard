using UnityEngine;
using System.Collections;

public class GameSceneController : AbstractController {
	public enum GameState{Waiting, Playing, GameOver, Pausing}
	public static GameState currentState;
	public GameMenuView gameMenuView;
	public CardDealer dealer;
	float gamePassTime;
	float gameTime;
	int score;
	CardArraySetting currentSetting;

	public override IEnumerator Init ()
	{
		yield return StartCoroutine(gameMenuView.Init());

		gameMenuView.onClickPause = PauseGame;
		gameMenuView.onClickReady = ShowCardFaceAndStart;
		gameMenuView.onClickExit = ExitGame;
		gameMenuView.onClickResume = ResumeGame;

		gameMenuView.ShowUI(false);

		currentSetting = CardArrayManager.GetCurrentLevelSetting();
		gameTime = currentSetting.gameTime;
		dealer.Init(currentSetting, NextRound, ScoreChange);

		gamePassTime = 0;
		currentState = GameState.Waiting;
		gameMenuView.SetTimeBar(1f);
		yield return StartCoroutine(dealer.DealCard());
	}

	void PauseGame()
	{
		currentState = GameState.Pausing;
	}

	void ResumeGame()
	{
		currentState = GameState.Playing;
	}

	void ExitGame()
	{
		if(currentState == GameState.GameOver)
			SaveGameRecord();
		GameMainLoop.Instance.ChangeScene(SceneName.MainScene, 1);
	}

	void ShowCardFaceAndStart()
	{
		StartCoroutine(DealCardRoutine());
	}

	void NextRound()
	{
		gameTime += currentSetting.awardTime;
		gameMenuView.AddTimeEffect(1f - gamePassTime/gameTime);
		StartCoroutine(NextRoundRoutine());
	}

	void GameOver()
	{
		if(PlayerPrefsManager.OnePlayerProgress == (int)currentSetting.level)
			PlayerPrefsManager.OnePlayerProgress += 1;

		currentState = GameState.GameOver;
		gameMenuView.SetTimeBar(0f);
		gameMenuView.ShowGameOverWindow(score);
	}

	void ScoreChange(int changeAmount)
	{
		score += changeAmount;
		if(score < 0)
			score = 0;
		gameMenuView.SetScore(score);
	}

	IEnumerator DealCardRoutine()
	{
		dealer.FlipAllCard();
		yield return new WaitForSeconds(0.35f + currentSetting.showCardTime);		
		dealer.FlipAllCard();
		yield return new WaitForSeconds(0.35f);
		gameMenuView.ToggleMask(false);
		currentState = GameState.Playing;
	}

	IEnumerator NextRoundRoutine()
	{
		gameMenuView.ToggleMask(true);
		currentState = GameState.Waiting;
		yield return new WaitForSeconds(0.3f);
		yield return StartCoroutine(dealer.DealCard());
		yield return new WaitForSeconds(0.3f);
		yield return StartCoroutine(DealCardRoutine());
	}

	void SaveGameRecord()
	{		
		GameRecord record = ModelManager.Instance.GetGameRecord(currentSetting.level);
		if(score > record.highScore)
			record.highScore = score;
		record.playTimes += 1;

		ModelManager.Instance.SaveGameRecord(record);
	}

	void Update()
	{
		if(currentState == GameState.Playing)
		{
			gameMenuView.SetTimeBar(1f - gamePassTime/gameTime);
			gamePassTime += Time.deltaTime;
			if(gamePassTime >= gameTime)
				GameOver();
		}
	}
}
