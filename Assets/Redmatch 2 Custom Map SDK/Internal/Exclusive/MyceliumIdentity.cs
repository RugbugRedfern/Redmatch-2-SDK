using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if !REDMATCH
public class MyceliumIdentity : MonoBehaviour
{
	[HideInInspector] public int NetID = -1;
}
#endif