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
	protected VoidTwoInt gameOver;
	protected CardDealer dealer;
	protected CardArraySetting currentSetting;
	protected GameMenuView gameMenuView;

	public virtual IEnumerator Init(CardDealer dealer, VoidTwoInt gameOver, CardArraySetting currentSetting, GameMenuView gameMenuView)
	{
		currentState = GameState.Waiting;
		gameMenuView.onClickPause = PauseGame;
		gameMenuView.onClickResume = ResumeGame;
		this.dealer = dealer;
		this.gameOver = gameOver;
		this.currentSetting = currentSetting;
		this.gameMenuView = gameMenuView;
		yield return null;
	}

	public abstract IEnumerator StartGame();

	public virtual void PauseGame()
	{
		currentState = GameState.Pausing;
	}

	public virtual void ResumeGame()
	{
		currentState = GameState.Playing;
	}
	
	public virtual void JudgementUpdate()
	{

	}
}
