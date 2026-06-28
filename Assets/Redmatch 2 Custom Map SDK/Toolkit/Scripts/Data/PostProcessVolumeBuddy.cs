
using UnityEngine;

public class PostProcessVolumeBuddy : MonoBehaviour
{
#if !REDMATCH
	[Header("Uncheck this if you change any values on the Post Processing Profile.", order = 0)]
	[Space(-10, order = 1)]
	[Header("Otherwise, leave it checked so that the post-processing will stay", order = 2)]
	[Space(-10, order = 3)]
	[Header("consistent with any future updates.", order = 4)]
	[Space(10, order = 5)]
#else
	[Header("For custom maps")]
#endif
	public bool useDefaultPostProcessingProfile = true;
	
// Some code here has been excluded from the SDK.

}
