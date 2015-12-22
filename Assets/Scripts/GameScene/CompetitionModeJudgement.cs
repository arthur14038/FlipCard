using UnityEngine;
using System.Collections;

public class CompetitionModeJudgement : GameModeJudgement
{
	public enum WhosTurn {WaitingReady, Player1Playing, Player2Playing, WaitingPlayer1, WaitingPlayer2}
	WhosTurn currentTurn;
	int player1Score;
	int player2Score;
	bool player1Ready = false;
	bool player2Ready = false;
	CompetitionModeView competitionModeView;

	public override IEnumerator Init(GameMainView gameMainView, GameSettingView gameSettingView, AbstractView modeView)
	{
		yield return gameMainView.StartCoroutine(base.Init(gameMainView, gameSettingView, modeView));
		gameMainView.completeOneRound = RoundComplete;
		gameMainView.cardMatch = CardMatch;
		player1Score = 0;
		player2Score = 0;
		competitionModeView = (CompetitionModeView)modeView;
		competitionModeView.button_Player1.onLightUp = OnPlayerButtonLightUp;
		competitionModeView.button_Player2.onLightUp = OnPlayerButtonLightUp;
		competitionModeView.SetTwoPlayerScore(player1Score, player2Score);
		currentTurn = WhosTurn.WaitingReady;
		competitionModeView.SetPlayerButton(currentTurn);
		yield return gameMainView.StartCoroutine(gameMainView.DealCard());
	}

	protected override IEnumerator StartGame()
	{
		gameMainView.FlipAllCard();
		yield return new WaitForSeconds(0.35f + currentSetting.showCardTime);
		gameMainView.FlipAllCard();
		yield return new WaitForSeconds(0.35f);
		gameMainView.ToggleMask(false);
		currentState = GameState.Playing;
	}

	void OnPlayerButtonLightUp(PressButtonTool pressButton)
	{
		switch(currentTurn)
		{
		case WhosTurn.WaitingReady:
			if(pressButton == competitionModeView.button_Player1)
				player1Ready = true;
			if(pressButton == competitionModeView.button_Player2)
				player2Ready = true;
			if(player1Ready && player2Ready)
			{
				currentTurn = (WhosTurn)Random.Range((int)WhosTurn.Player1Playing, (int)WhosTurn.Player2Playing + 1);
				gameMainView.StartCoroutine(competitionModeView.DisableChooseSide(currentTurn));
			}
			break;
		case WhosTurn.WaitingPlayer1:
			if(pressButton == competitionModeView.button_Player1)
			{
				currentTurn = WhosTurn.Player1Playing;
				competitionModeView.SetPlayerButton(currentTurn);
				gameMainView.ToggleMask(false);
			}
			break;
		case WhosTurn.WaitingPlayer2:
			if(pressButton == competitionModeView.button_Player2)
			{
				currentTurn = WhosTurn.Player2Playing;
				competitionModeView.SetPlayerButton(currentTurn);
				gameMainView.ToggleMask(false);
			}
			break;
		}
	}

	void RoundComplete()
	{
		GameOver(player1Score, player2Score);
	}

	protected override void GameOver(params int[] values)
	{
		base.GameOver(values);
		competitionModeView.ShowGameOver(values[0], values[1]);
	}

	void CardMatch(bool match, params Card[] cards)
	{
		if(currentState != GameState.GameOver)
		{
			int scoreChangeAmount = 0;
			if(match)
			{
				scoreChangeAmount = 8;
			}else
			{
				scoreChangeAmount = -2;
				if(currentTurn == WhosTurn.Player1Playing)
				{
					currentTurn = WhosTurn.WaitingPlayer2;
					competitionModeView.SetPlayerButton(currentTurn);
					gameMainView.ToggleMask(true);
				}
				if(currentTurn == WhosTurn.Player2Playing)
				{
					currentTurn = WhosTurn.WaitingPlayer1;
					competitionModeView.SetPlayerButton(currentTurn);
					gameMainView.ToggleMask(true);
				}
			}
			if(scoreChangeAmount != 0)
			{
				//加分效果
			}
		}
	}
}
