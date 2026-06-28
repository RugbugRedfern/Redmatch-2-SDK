using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ValueDisplay : MonoBehaviour
{
#if REDMATCH
	public abstract void DisplayValue(params float[] values);
#endif
}
