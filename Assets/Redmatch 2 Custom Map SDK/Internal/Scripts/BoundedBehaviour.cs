using UnityEngine;

public class BoundedBehaviour : MonoBehaviour
{
	public virtual Color BoundsColor { get; private set; }
	public virtual Color BoundsOutlineColor { get; private set; }
	public Bounds bounds = new Bounds(Vector3.zero, Vector3.one * 2);

	void OnDrawGizmos()
	{
		Gizmos.color = BoundsColor;
		Gizmos.DrawCube(transform.position + bounds.center, bounds.size);
	}

#if REDMATCH
	public Vector3 GetRandomPosition()
	{
		return new Vector3(
			Random.Range(bounds.min.x, bounds.max.x),
			Random.Range(bounds.min.y, bounds.max.y),
			Random.Range(bounds.min.z, bounds.max.z))
			+ transform.position;
	}

	public float GetBottom()
	{
		return bounds.min.y;
	}

	public float GetTop()
	{
		return bounds.max.y;
	}
#endif
}