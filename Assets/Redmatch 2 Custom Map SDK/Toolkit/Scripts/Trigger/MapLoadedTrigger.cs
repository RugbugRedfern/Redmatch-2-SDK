using UnityEngine;



[HelpURL("https://github.com/RugbugRedfern/Redmatch-2-SDK/wiki/Triggers#maploadedtrigger-c-source")]
public class MapLoadedTrigger : Trigger
{
	public override string GetTarget()
	{
		return "None.";
	}

#if REDMATCH
	void Start()
	{
		Activate(null, null);
	}
#endif
}
