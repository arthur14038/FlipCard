using UnityEngine;
using System.Collections;

public class CompetitionModeJudgement : GameModeJudgement
{
	public enum WhosTurn {WaitingReady, Player1Playing, Player2Playing, WaitingPlayer1, WaitingPlayer2}
	WhosTurn currentTurn;
	int player1Score;
	int player2Scroe;
	bool player1Ready = false;
	bool player2Ready = false;

	public override IEnumerator Init(CardDealer dealer, VoidTwoInt gameOver, CardArraySetting currentSetting, GameMenuView gameMenuView)
	{
		yield return gameMenuView.StartCoroutine(base.Init(dealer, gameOver, currentSetting, gameMenuView));
		dealer.Init(currentSetting, GameOver, CardMatch);
		player1Score = 0;
		player2Scroe = 0;
		gameMenuView.button_Player1.onLightUp = OnPlayerButtonLightUp;
		gameMenuView.button_Player2.onLightUp = OnPlayerButtonLightUp;
		gameMenuView.SetTwoPlayerScore(player1Score, player2Scroe);
		currentTurn = WhosTurn.WaitingReady;
		gameMenuView.SetPlayerButton(currentTurn);
		yield return gameMenuView.StartCoroutine(dealer.DealCard());
	}

	public override IEnumerator StartGame()
	{
		dealer.FlipAllCard();
		yield return new WaitForSeconds(0.35f + currentSetting.showCardTime);
		dealer.FlipAllCard();
		yield return new WaitForSeconds(0.35f);
		gameMenuView.ToggleMask(false);
		currentState = GameState.Playing;
	}

	void OnPlayerButtonLightUp(PressButtonTool pressButton)
	{
		switch(currentTurn)
		{
			case WhosTurn.WaitingReady:
				if(pressButton == gameMenuView.button_Player1)
					player1Ready = true;
				if(pressButton == gameMenuView.button_Player2)
					player2Ready = true;
				if(player1Ready && player2Ready)
				{
					currentTurn = (WhosTurn)Random.Range((int)WhosTurn.Player1Playing, (int)WhosTurn.Player2Playing + 1);
					gameMenuView.StartCoroutine(gameMenuView.DisableChooseSide(currentTurn));
				}
				break;
			case WhosTurn.WaitingPlayer1:
				if(pressButton == gameMenuView.button_Player1)
				{
					currentTurn = WhosTurn.Player1Playing;
					gameMenuView.SetPlayerButton(currentTurn);
					gameMenuView.ToggleMask(false);
				}
				break;
			case WhosTurn.WaitingPlayer2:
				if(pressButton == gameMenuView.button_Player2)
				{
					currentTurn = WhosTurn.Player2Playing;
					gameMenuView.SetPlayerButton(currentTurn);
					gameMenuView.ToggleMask(false);
				}
				break;
		}
	}

	void GameOver()
	{
		if(gameOver != null)
			gameOver(player1Score, player2Scroe);
	}

	void CardMatch(bool match, params Card[] cards)
	{
		if(currentState == GameState.Playing)
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
					gameMenuView.SetPlayerButton(currentTurn);
					gameMenuView.ToggleMask(true);
				}
				if(currentTurn == WhosTurn.Player2Playing)
				{
					currentTurn = WhosTurn.WaitingPlayer1;
					gameMenuView.SetPlayerButton(currentTurn);
					gameMenuView.ToggleMask(true);
				}
			}
			if(scoreChangeAmount != 0)
			{
				//加分效果
			}
		}
	}
}
