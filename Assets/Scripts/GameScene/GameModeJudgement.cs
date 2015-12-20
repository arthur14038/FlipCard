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
		this.dealer = dealer;
		this.gameOver = gameOver;
		this.currentSetting = currentSetting;
		this.gameMenuView = gameMenuView;
		yield return null;
	}

	public abstract IEnumerator StartGame();
	public abstract void PauseGame();
	public abstract void ResumeGame();

	public virtual void JudgementUpdate()
	{

	}
}
