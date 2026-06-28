
using UnityEngine;
using UnityEngine.UI;

public class TextValueDisplay : ValueDisplay
{
	[SerializeField] Text[] text;
	[SerializeField] string format = "0";

#if REDMATCH
	public override void DisplayValue(params float[] values)
	{
		for(int i = 0; i < values.Length && i < text.Length; i++)
		{
			text[i].text = values[i].ToString(format);
		}
	}
#endif
}