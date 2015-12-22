using UnityEngine;
using System.Collections;

public abstract class GameModeJudgement{
	public enum GameState { Waiting, Playing, Pausing, GameOver}
	public static GameState CurrentGameState
	{
		get
		{
			return currentState;
		}
	}
	protected static GameState currentState;
	protected GameMainView gameMainView;
	protected GameSettingView gameSettingView;
	protected CardArraySetting currentSetting;
	public VoidNoneParameter exitGame;

	public virtual IEnumerator Init(GameMainView gameMainView, GameSettingView gameSettingView, AbstractView modeView)
	{
		currentState = GameState.Waiting;
		currentSetting = CardArrayManager.GetCurrentLevelSetting();
        this.gameSettingView = gameSettingView;
		this.gameMainView = gameMainView;
		gameSettingView.onClickResume = ResumeGame;
		gameSettingView.onClickExit = ExitGame;
		yield return null;
	}

	protected abstract IEnumerator StartGame();

	protected virtual void GameOver(params int[] values)
	{
		currentState = GameState.GameOver;
    }

	protected virtual void PauseGame()
	{
		currentState = GameState.Pausing;
		gameSettingView.ShowUI(true);
	}

	protected virtual void ResumeGame()
	{
		currentState = GameState.Playing;
		gameSettingView.HideUI(true);
    }
	
	protected virtual void ExitGame()
	{
		if(exitGame != null)
			exitGame();
	}

	public virtual void JudgementUpdate()
	{

	}
}
