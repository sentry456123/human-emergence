using UnityEngine;

#nullable enable

public class MainCamera : MonoBehaviour
{
	public Transform? toFollow;
	[SerializeField] private float speed = 5.0f;

	private Camera target;
	private float desiredZoom;

	void Start()
	{
		target = GetComponent<Camera>();
		desiredZoom = target.orthographicSize;
	}

	void Update()
	{
		float dash = Input.GetKey(KeyCode.LeftShift) ? 3.0f : 1.0f;

		if (toFollow == null)
		{
			if (Input.GetKey(KeyCode.W))
			{
				transform.position += target.orthographicSize * speed * Time.deltaTime * transform.up * dash;
			}
			if (Input.GetKey(KeyCode.S))
			{
				transform.position -= target.orthographicSize * speed * Time.deltaTime * transform.up * dash;
			}
			if (Input.GetKey(KeyCode.A))
			{
				transform.position -= target.orthographicSize * speed * Time.deltaTime * transform.right * dash;
			}
			if (Input.GetKey(KeyCode.D))
			{
				transform.position += target.orthographicSize * speed * Time.deltaTime * transform.right * dash;
			}
		}
		else
		{
			transform.position = toFollow.position;
		}
		if (Input.GetAxis("Mouse ScrollWheel") > 0)
		{
			desiredZoom /= 1.5f;
		}
		if (Input.GetAxis("Mouse ScrollWheel") < 0)
		{
			desiredZoom *= 1.5f;
		}

		target.orthographicSize = Mathf.Lerp(target.orthographicSize, desiredZoom, Time.deltaTime * 10.0f);
	}
}
