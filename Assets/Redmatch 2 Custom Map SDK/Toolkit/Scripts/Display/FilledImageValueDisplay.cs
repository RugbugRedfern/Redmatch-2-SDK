
using UnityEngine;
using UnityEngine.UI;

public class FilledImageValueDisplay : ValueDisplay
{
	[SerializeField] Image fillImage;

#if REDMATCH
	public override void DisplayValue(params float[] values)
	{
		if(values.Length == 0)
			return;

		if(values.Length == 1)
		{
			fillImage.fillAmount = values[0] / 1f;
		}
		else
		{
			fillImage.fillAmount = values[0] / values[1];
		}
	}
#endif
}