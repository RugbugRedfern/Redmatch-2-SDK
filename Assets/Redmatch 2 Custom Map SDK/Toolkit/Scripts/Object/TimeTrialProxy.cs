using System.Collections.Generic;

public class TimeTrialProxy : ErrorCollectionBehaviour
{
	public TimeTrialNodeProxy[] nodes;
	public float platinumTime;
	public float goldTime;
	public float silverTime;
	public float bronzeTime;

	public override void RunErrorChecks(ref List<string> errors)
	{
		foreach(var node in nodes)
		{
			if(node == null)
			{
				errors.Add("A node is null.");
			}
		}

		if(platinumTime <= 0 || platinumTime > goldTime)
		{
			errors.Add("Platinum time is invalid.");
		}

		if(goldTime <= 0 || goldTime > silverTime)
		{
			errors.Add("Gold time is invalid.");
		}

		if(silverTime <= 0 || silverTime > bronzeTime)
		{
			errors.Add("Silver time is invalid.");
		}

		if(bronzeTime <= 0)
		{
			errors.Add("Bronze time is invalid.");
		}

		if(nodes.Length < 2)
		{
			errors.Add("Not enough nodes.");
		}
	}
}
