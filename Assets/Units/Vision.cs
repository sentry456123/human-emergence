using System;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]

public class Vision : MonoBehaviour
{
	[HideInInspector] public Action<GameObject> onTriggerStay;
	[HideInInspector] public Action<GameObject> onTriggerExit;

	void OnTriggerStay2D(Collider2D collider)
	{
		onTriggerStay(collider.gameObject);
	}

	void OnTriggerExit2D(Collider2D collider)
	{
		onTriggerExit(collider.gameObject);
	}
}
