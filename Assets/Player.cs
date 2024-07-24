using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

#nullable enable

public class Player : MonoBehaviour
{
	[SerializeField] private MainCamera? camera;

	private Unit? selectedUnit;

    void Update()
    {
		if (selectedUnit.IsDestroyed())
		{
			selectedUnit = null;
		}

		if (selectedUnit == null)
		{
			Unit? unit = CheckSelectedUnit();
			if (unit != null)
			{
				selectedUnit = unit;
				unit.PlayerBegin();
			}
		}
		else
		{
            if (camera == null)
            {
				Debug.LogError("Main camera not set in Player.cs");
			}

			camera.toFollow = selectedUnit.transform;

			Vector2 direction = Vector2.zero;

			if (Input.GetKey(KeyCode.W))
				direction.y += 1.0f;
			if (Input.GetKey(KeyCode.S))
				direction.y += -1.0f;
			if (Input.GetKey(KeyCode.A))
				direction.x += -1.0f;
			if (Input.GetKey(KeyCode.D))
				direction.x += 1.0f;

			selectedUnit.PlayerMove(direction);

			Vector2 mouseWorldPosition = camera.GetComponent<Camera>().ScreenToWorldPoint(Input.mousePosition);
			Vector2 rotation = (mouseWorldPosition - (Vector2)selectedUnit.transform.position).normalized;
			selectedUnit.transform.up = rotation;

			selectedUnit.PlayerSetFire(Input.GetKey(KeyCode.Mouse0));

			bool quit = false;

			quit = quit || Input.GetKeyDown(KeyCode.Q);
			quit = quit || CheckSelectedUnit() == selectedUnit;

			if (quit)
			{
				selectedUnit.PlayerEnd();
				selectedUnit = null;
				camera.toFollow = null;
			}
		}
	}

	private Unit? CheckSelectedUnit()
	{
		if (EventSystem.current.IsPointerOverGameObject())
			return null;
		if (!Input.GetKeyDown(KeyCode.Mouse0))
			return null;

		Vector3 worldMousePosition = camera.GetComponent<Camera>().ScreenToWorldPoint(Input.mousePosition);
		RaycastHit2D raycastHit = Physics2D.Raycast(new Vector2(worldMousePosition.x, worldMousePosition.y), Vector2.zero, 0.0f);

		if (raycastHit. collider == null)
			return null;
		if (raycastHit.collider.TryGetComponent<Unit>(out var unit))
			return unit;

		return null;
	}
}
