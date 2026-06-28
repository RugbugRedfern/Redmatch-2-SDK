
using UnityEngine;

public class CameraFacingBillboard : MonoBehaviour
{
	[SerializeField] bool maintainSize;

#if REDMATCH
	Camera cam;
	Vector3 originalScale;

	void Awake()
	{
		originalScale = transform.localScale;
	}

	void LateUpdate()
	{
		if(cam == null || !cam.gameObject.activeSelf)
			cam = Camera.main;

		if(cam == null || !cam.gameObject.activeSelf)
			cam = FindObjectOfType<Camera>();

		if(cam == null)
			return;

		transform.LookAt(cam.transform);
		transform.Rotate(Vector3.up * 180);

		if(maintainSize)
		{
			transform.localScale = originalScale * Vector3.Distance(transform.position, cam.transform.position);
		}
	}
#endif
}