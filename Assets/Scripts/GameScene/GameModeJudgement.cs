using UnityEngine;
using System.Collections;

public abstract class GameModeJudgement{
	public enum GameState { Waiting, Playing, Pausing, GameOver}
	protected GameState currentState;
	protected GameMainView gameMainView;
	protected GameSettingView gameSettingView;
	protected CardArraySetting currentCardArraySetting;
	public VoidNoneParameter exitGame;
	public VoidGameRecord saveGameRecord;

	public virtual IEnumerator Init(GameMainView gameMainView, GameSettingView gameSettingView, AbstractView modeView)
	{
		currentState = GameState.Waiting;
        this.gameSettingView = gameSettingView;
		this.gameMainView = gameMainView;
		currentCardArraySetting = GameSettingManager.GetCurrentCardArraySetting();
		gameSettingView.onClickResume = ResumeGame;
		gameSettingView.onClickExit = ExitGame;
		yield return null;
	}

	protected abstract IEnumerator StartGame();

	protected virtual void GameOver(params int[] values)
	{
		currentState = GameState.GameOver;
		gameMainView.ToggleMask(true);
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

	protected virtual Vector2[] GetCardPos()
	{
		Vector2[] cardPos = new Vector2[currentCardArraySetting.row * currentCardArraySetting.column];

		for(int i = 0 ; i < cardPos.Length ; ++i)
		{
			float x = currentCardArraySetting.realFirstPosition.x + (i % currentCardArraySetting.column) * (currentCardArraySetting.edgeLength + currentCardArraySetting.cardGap);
			float y = currentCardArraySetting.realFirstPosition.y - (i / currentCardArraySetting.column) * (currentCardArraySetting.edgeLength + currentCardArraySetting.cardGap);
			cardPos[i] = new Vector2(x, y);
		}

		return cardPos;
	}
}
