using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class ScoreText : MonoBehaviour {
    public Color positiveColor = Color.green;
    public Color negativeColor = Color.red;
	public Color positiveOutlineColor = Color.green;
	public Color negativeOutlineColor = Color.red;
	Text thisText;
	Outline thisOutline;
    VoidScoreText recycle;

    public void Init(VoidScoreText recycle)
    {
        this.gameObject.SetActive(false);
        this.recycle = recycle;
        thisText = this.GetComponent<Text>();
		thisOutline = this.GetComponent<Outline>();
    }

    public void ShowScoreText(int score, Vector2 pos)
    {
        if (!this.gameObject.activeSelf)
            this.gameObject.SetActive(true);
        thisText.rectTransform.anchoredPosition = pos;
        if (score < 0)
        {
            thisText.text = score.ToString();
            thisText.color = negativeColor;
			thisOutline.effectColor = negativeOutlineColor;
        }
        else
        {
            thisText.text = "+" + score;
            thisText.color = positiveColor;
			thisOutline.effectColor = positiveOutlineColor;
		}
        thisText.rectTransform.DOAnchorPosY(pos.y + 50f, 0.8f).SetEase(Ease.OutQuad);
        thisText.DOFade(0f, 0.2f).SetDelay(0.6f).OnComplete(
            delegate () {
                this.gameObject.SetActive(false);
                if (recycle != null)
                    recycle(this);
            }
        );
    }
}
