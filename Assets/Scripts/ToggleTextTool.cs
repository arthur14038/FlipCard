using UnityEngine;
using UnityEngine.UI;

public class ToggleTextTool : MonoBehaviour {
	public Text text;
    public Image[] images;
	public string onString;
	public string offString;
	public Color onColor = Color.white;
	public Color offColor = Color.white;
	Toggle theToggle;
	bool saveToggleValue;

	void OnEnable()
	{
		if(theToggle == null)
			theToggle = this.GetComponent<Toggle>();

		if(theToggle != null)
		{
			saveToggleValue = theToggle.isOn;
			OnToggleValueChange(saveToggleValue);
		}
	}

	public void OnToggleValueChange(bool value)
	{
		saveToggleValue = value;
		if(value)
		{
			text.text = onString.Replace("\\n", "\n");
			text.color = onColor;
            foreach(Image image in images)
                image.color = onColor;
		}else
		{
			text.text = offString.Replace("\\n", "\n");
			text.color = offColor;
            foreach (Image image in images)
                image.color = offColor;
        }
	}
}
