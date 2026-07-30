

using System.Collections.Generic;
using UnityEngine;



[HelpURL("https://github.com/RugbugRedfern/Redmatch-2-SDK/wiki/MapInfo")]
public class MapInfo : BoundedBehaviour
{
	public override Color BoundsColor => Color.clear;
	public override Color BoundsOutlineColor => Color.green;
	[SerializeField] float maxHeight = 1000f;
	public Light sun;
	public Camera worldCamera;
	public bool killIfOutOfBounds;
	[SerializeField] ForceSetting flashlightSetting;

	public enum ForceSetting { AvailableIfNight, AlwaysAvailable, NeverAvailable };

	
// Some code here has been excluded from the SDK.

}