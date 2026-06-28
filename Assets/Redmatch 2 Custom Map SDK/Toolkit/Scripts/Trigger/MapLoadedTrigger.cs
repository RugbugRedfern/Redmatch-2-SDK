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
