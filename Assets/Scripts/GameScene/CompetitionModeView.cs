using UnityEngine;
using System.Collections;
using DG.Tweening;
using UnityEngine.UI;

public class CompetitionModeView : AbstractView
{
	public IEnumeratorNoneParameter onTwoPlayerReady;
	public VoidNoneParameter onClickPause;
	[SerializeField]
	Text text_ScorePlayer1Title;
	[SerializeField]
	Text text_ScorePlayer2Title;
	[SerializeField]
	Text text_Player1Score;
	[SerializeField]
	Text text_Player2Score;
	[SerializeField]
	Text text_Instruction;
	[SerializeField]
	CanvasGroup group_Instruction;
	[SerializeField]
	RectTransform image_Player1Arrow;
	[SerializeField]
	RectTransform image_Player2Arrow;
	[SerializeField]
	public PressButtonTool button_Player1;
	[SerializeField]
	public PressButtonTool button_Player2;

	public override IEnumerator Init()
	{
		group_Instruction.gameObject.SetActive(true);
		group_Instruction.alpha = 1f;
		text_Instruction.gameObject.SetActive(true);
		TogglePlayerArrow(3);
		StartCoroutine(ShowInstruction(Localization.Get("CompetitionGameView/TapToReady")));
		yield return null;
		UpdateText();
		Localization.Event_ChangeLocaliztion += UpdateText;
	}

	void OnDestroy()
	{
		Localization.Event_ChangeLocaliztion -= UpdateText;
	}

	void UpdateText()
	{
		text_ScorePlayer1Title.text = Localization.Get("CompetitionGameView/Player1Title");
		text_ScorePlayer2Title.text = Localization.Get("CompetitionGameView/Player2Title");
	}

	public void OnClickPause()
	{
		AudioManager.Instance.PlayOneShot("Button_Click");
		if(onClickPause != null)
			onClickPause();
	}
	
	public void SetTwoPlayerScore(int player1Score, int player2Score)
	{
		text_Player1Score.text = player1Score.ToString();
		text_Player2Score.text = player2Score.ToString();
	}

	public IEnumerator PlayerReadyEffect(CompetitionModeJudgement.WhosTurn currentTurn)
	{
		TogglePlayerArrow(0);
		string msg = "";
		switch(currentTurn)
		{
			case CompetitionModeJudgement.WhosTurn.Player1Playing:
				button_Player1.UpdateButton(PressButtonTool.PressButtonState.LightUp);
				button_Player2.UpdateButton(PressButtonTool.PressButtonState.Disable);
				msg = Localization.Get("CompetitionGameView/Player1First");
				break;
			case CompetitionModeJudgement.WhosTurn.Player2Playing:
				button_Player1.UpdateButton(PressButtonTool.PressButtonState.Disable);
				button_Player2.UpdateButton(PressButtonTool.PressButtonState.LightUp);
				msg = Localization.Get("CompetitionGameView/Player2First");
				break;
		}
		
		yield return StartCoroutine(ShowInstruction(msg));
		yield return new WaitForSeconds(0.5f);
		
		StartCoroutine(onTwoPlayerReady());
	}

	public void SetPlayerButton(CompetitionModeJudgement.WhosTurn currentTurn)
	{
		switch(currentTurn)
		{
		case CompetitionModeJudgement.WhosTurn.WaitingReady:
			button_Player1.UpdateButton(PressButtonTool.PressButtonState.Clickable);
			button_Player2.UpdateButton(PressButtonTool.PressButtonState.Clickable);
			break;
		case CompetitionModeJudgement.WhosTurn.WaitingPlayer1:
			ShowTakeTurn(currentTurn);
			button_Player1.UpdateButton(PressButtonTool.PressButtonState.Clickable);
			button_Player2.UpdateButton(PressButtonTool.PressButtonState.Disable);
			break;
		case CompetitionModeJudgement.WhosTurn.WaitingPlayer2:
			ShowTakeTurn(currentTurn);
			button_Player1.UpdateButton(PressButtonTool.PressButtonState.Disable);
			button_Player2.UpdateButton(PressButtonTool.PressButtonState.Clickable);
			break;
		case CompetitionModeJudgement.WhosTurn.Player1Playing:
			StartCoroutine(FadeOutInstruction());
			button_Player1.UpdateButton(PressButtonTool.PressButtonState.LightUp);
			button_Player2.UpdateButton(PressButtonTool.PressButtonState.Disable);
			break;
		case CompetitionModeJudgement.WhosTurn.Player2Playing:
			StartCoroutine(FadeOutInstruction());
			button_Player1.UpdateButton(PressButtonTool.PressButtonState.Disable);
			button_Player2.UpdateButton(PressButtonTool.PressButtonState.LightUp);
			break;
		}
	}

	/// <summary>
	/// 切換箭頭的狀態
	/// </summary>
	/// <param name="index">0: Close Both, 1: Player1, 2: Player2, 3: Open Both</param>
	void TogglePlayerArrow(int index)
	{
		image_Player1Arrow.gameObject.SetActive(false);
		image_Player2Arrow.gameObject.SetActive(false);
		switch(index)
		{
			case 1:
				image_Player1Arrow.gameObject.SetActive(true);
				break;
			case 2:
				image_Player2Arrow.gameObject.SetActive(true);
				break;
			case 3:
				image_Player2Arrow.gameObject.SetActive(true);
				image_Player1Arrow.gameObject.SetActive(true);
				break;
		}
	}

	void ShowTakeTurn(CompetitionModeJudgement.WhosTurn currentTurn)
	{
		group_Instruction.gameObject.SetActive(true);
		group_Instruction.alpha = 0f;
		group_Instruction.DOFade(1f, 0.3f);

		string msg = "";

		if(currentTurn == CompetitionModeJudgement.WhosTurn.WaitingPlayer1)
		{
			msg = Localization.Get("CompetitionGameView/Player1Turn");
			TogglePlayerArrow(1);
		}
		else if(currentTurn == CompetitionModeJudgement.WhosTurn.WaitingPlayer2)
		{
			msg = Localization.Get("CompetitionGameView/Player2Turn");
			TogglePlayerArrow(2);
		}

		StartCoroutine(ShowInstruction(msg));
	}
	
	IEnumerator ShowInstruction(string content)
	{		
		text_Instruction.text = content;
		text_Instruction.gameObject.SetActive(true);
		text_Instruction.rectTransform.localScale = Vector2.zero;
		yield return text_Instruction.rectTransform.DOScale(1f, 0.3f).SetEase(Ease.OutBack).WaitForCompletion();
	}

	public IEnumerator FadeOutInstruction()
	{
		group_Instruction.DOFade(0f, 0.3f).WaitForCompletion();
		yield return text_Instruction.rectTransform.DOScale(0f, 0.3f).SetEase(Ease.InBack).WaitForCompletion();
		group_Instruction.gameObject.SetActive(false);
	}

	protected override IEnumerator HideUIAnimation()
	{
		yield return null;
		hideCoroutine = null;
	}

	protected override IEnumerator ShowUIAnimation()
	{
		yield return null;
		showCoroutine = null;
	}
}
