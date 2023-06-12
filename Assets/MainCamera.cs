using UnityEngine;

#nullable enable

public class MainCamera : MonoBehaviour
{

	[SerializeField] private float speed = 5.0f;
	[SerializeField] private Transform? follow;

	void Update()
	{
		if (follow == null)
		{
			if (Input.GetKey(KeyCode.W))
			{
				transform.position += GetComponent<Camera>().orthographicSize * speed * Time.deltaTime * transform.up;
			}
			if (Input.GetKey(KeyCode.S))
			{
				transform.position -= GetComponent<Camera>().orthographicSize * speed * Time.deltaTime * transform.up;
			}
			if (Input.GetKey(KeyCode.A))
			{
				transform.position -= GetComponent<Camera>().orthographicSize * speed * Time.deltaTime * transform.right;
			}
			if (Input.GetKey(KeyCode.D))
			{
				transform.position += GetComponent<Camera>().orthographicSize * speed * Time.deltaTime * transform.right;
			}
		}
		else
		{
			this.transform.position = follow.position;
			this.transform.Translate(0.0f, 0.0f, -1.0f);
		}
		if (Input.GetAxis("Mouse ScrollWheel") > 0)
		{
			GetComponent<Camera>().orthographicSize /= 1.5f;
		}
		if (Input.GetAxis("Mouse ScrollWheel") < 0)
		{
			GetComponent<Camera>().orthographicSize *= 1.5f;
		}
	}
}
