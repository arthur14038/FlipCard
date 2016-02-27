using UnityEngine;
using System.Collections;

public class CompetitionModeJudgement : GameModeJudgement
{
	public enum WhosTurn {WaitingReady, Player1Playing, Player2Playing, WaitingPlayer1, WaitingPlayer2}
	WhosTurn currentTurn;
	int player1Score;
	int player2Score;
	int comboCount;
	bool player1Ready = false;
	bool player2Ready = false;
	CompetitionModeView competitionModeView;
	CompetitionModeSetting currentModeSetting;

	public override IEnumerator Init(GameMainView gameMainView, GameSettingView gameSettingView, AbstractView modeView)
	{
		yield return gameMainView.StartCoroutine(base.Init(gameMainView, gameSettingView, modeView));
		gameMainView.LoadCard(currentCardArraySetting.row * currentCardArraySetting.column, 0);
		gameMainView.SetUsingCard(currentCardArraySetting.row * currentCardArraySetting.column, 0);
		currentModeSetting = GameSettingManager.GetCurrentCompetitionModeSetting();
		gameMainView.completeOneRound = RoundComplete;
		gameMainView.cardMatch = CardMatch;
		player1Score = 0;
		player2Score = 0;
		comboCount = 0;
		competitionModeView = (CompetitionModeView)modeView;
		competitionModeView.button_Player1.onLightUp = OnPlayerButtonLightUp;
		competitionModeView.button_Player2.onLightUp = OnPlayerButtonLightUp;
		competitionModeView.SetTwoPlayerScore(player1Score, player2Score);
		competitionModeView.onClickPause = PauseGame;
		competitionModeView.onTwoPlayerReady = StartGame;
		currentTurn = WhosTurn.WaitingReady;
		competitionModeView.SetPlayerButton(currentTurn);
	}

	protected override IEnumerator StartGame()
	{
		gameMainView.SetGoldCard(currentModeSetting.goldCardCount, false);
		yield return gameMainView.StartCoroutine(gameMainView.DealCard(currentCardArraySetting.edgeLength, GetCardPos()));
		yield return new WaitForSeconds(0.2f);
		yield return competitionModeView.StartCoroutine(competitionModeView.FadeOutInstruction());
		gameMainView.ToggleMask(false);
		currentState = GameState.Playing;
		AudioManager.Instance.PlayMusic("GamePlayBGM", true);
	}

	void OnPlayerButtonLightUp(PressButtonTool pressButton)
	{
		AudioManager.Instance.PlayOneShot("GameResultScoreCount");
		switch(currentTurn)
		{
			case WhosTurn.WaitingReady:
				if(!player1Ready && pressButton == competitionModeView.button_Player1)
				{
					player1Ready = true;
					competitionModeView.button_Player1.SetChargeTime(0f);
                }
				if(!player2Ready && pressButton == competitionModeView.button_Player2)
				{
					player2Ready = true;
					competitionModeView.button_Player2.SetChargeTime(0f);
				}
				if(player1Ready && player2Ready)
				{
					currentTurn = (WhosTurn)Random.Range((int)WhosTurn.Player1Playing, (int)WhosTurn.Player2Playing + 1);
                    competitionModeView.StartCoroutine(competitionModeView.PlayerReadyEffect(currentTurn));
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
		AddScore(1, (int)currentTurn, null);
		GameOver(player1Score, player2Score);
	}

	protected override void GameOver(params int[] values)
	{
		base.GameOver(values);
		gameSettingView.ShowTwoPlayersGameOver(values[0], values[1]);
	}

	void AddScore(int addAmount, int playerNumber, CardBase[] cards)
	{
		int saveScore = 0;
		int score = 0;
		switch(playerNumber)
		{
			case 1:
				score = player1Score;
				break;
			case 2:
				score = player2Score;
				break;
		}
		saveScore = score;
		score += addAmount;
		if(score < 0)
			score = 0;

		if(saveScore != score)
		{
			switch(playerNumber)
			{
				case 1:
					player1Score = score;
					break;
				case 2:
					player2Score = score;
					break;
			}
			competitionModeView.SetTwoPlayerScore(player1Score, player2Score);

			if(cards != null)
			{
				foreach(CardBase matchCard in cards)
				{
					Vector2 pos = matchCard.GetAnchorPosition();
					pos.x += currentCardArraySetting.edgeLength / 2 - 20f;
					gameMainView.ShowScoreText((score - saveScore) / cards.Length, pos);
				}
			}
		}
	}

	void CardMatch(bool match, params CardBase[] cards)
	{
		if(currentState != GameState.GameOver)
		{
			bool takeTurn = false;
			int scoreChangeAmount = 0;
			if(match)
			{
				if(comboCount == 0)
					gameMainView.ToggleCardGlow(true);

				scoreChangeAmount = currentModeSetting.matchAddScore * cards.Length;

				if(comboCount > 0)
					scoreChangeAmount *= 2;
				
				++comboCount;

				if(cards[0].IsGoldCard())
					scoreChangeAmount *= 2;

				if(cards[1].IsGoldCard())
					scoreChangeAmount *= 2;
			} else
			{
				scoreChangeAmount = currentModeSetting.mismatchReduceScore * cards.Length;
				if(comboCount > 0)
				{
					comboCount = 0;
					gameMainView.ToggleCardGlow(false);
				}
				takeTurn = true;
			}

			if(scoreChangeAmount != 0)
				AddScore(scoreChangeAmount, (int)currentTurn, cards);

			if(takeTurn)
			{
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
		}
	}
}
