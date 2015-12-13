using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class ScoreText : MonoBehaviour {
    public Color positiveColor = Color.green;
    public Color negativeColor = Color.red;
    Text thisText;
    Shadow thisShadow;
    VoidScoreText recycle;

    public void Init(VoidScoreText recycle)
    {
        this.gameObject.SetActive(false);
        this.recycle = recycle;
        thisText = this.GetComponent<Text>();
        thisShadow = this.GetComponent<Shadow>();
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
            thisShadow.effectColor = negativeColor / 4f + Color.black;
        }
        else
        {
            thisText.text = "+" + score;
            thisText.color = positiveColor;
            thisShadow.effectColor = positiveColor / 4f + Color.black;
        }
        thisText.rectTransform.DOAnchorPosY(pos.y + 50f, 1f).SetEase(Ease.OutQuad);
        thisText.DOFade(0f, 0.2f).SetDelay(0.8f).OnComplete(
            delegate () {
                this.gameObject.SetActive(false);
                if (recycle != null)
                    recycle(this);
            }
        );
    }
}
