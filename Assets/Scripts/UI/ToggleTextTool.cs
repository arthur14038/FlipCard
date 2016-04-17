using UnityEngine;
using UnityEngine.UI;

public class ToggleTextTool : MonoBehaviour {
	public Text text;
    public Image[] images;
	public string onString;
	public string offString;
	public Color onColor = Color.white;
	public Color offColor = Color.white;
	public bool changeTextContent = true;
	public bool changeColor = true;
	Toggle theToggle;

	void OnEnable()
	{
		if(theToggle == null)
			theToggle = this.GetComponent<Toggle>();

		if(theToggle != null)
		{
			theToggle.onValueChanged.AddListener(OnToggleValueChange);
			OnToggleValueChange(theToggle.isOn);
		}
	}

	public void OnToggleValueChange(bool value)
	{
		if(value)
		{
			if(text != null)
			{
				if(changeTextContent)
					text.text = Localization.Get(onString.Replace("\\n", "\n"));
				if(changeColor)
					text.color = onColor;
			}
			if(changeColor)
				foreach(Image image in images)
					image.color = onColor;
		}else
		{
			if(text != null)
			{
				if(changeTextContent)
					text.text = Localization.Get(offString.Replace("\\n", "\n"));
				if(changeColor)
					text.color = offColor;
			}
			if(changeColor)
				foreach (Image image in images)
					image.color = offColor;
        }
	}
}
