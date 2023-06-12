using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Unit : MonoBehaviour
{
	[SerializeField] private int team;
	[SerializeField] private float speed = 1.0f;
	[SerializeField] private float rotationSpeed = 1.0f;
	[SerializeField] private float rotationModifier;
	[SerializeField] private int maxHealth = 10;
	[SerializeField] private Weapon weapon;
	[SerializeField] private Collider2D vision;
	[SerializeField] private GameObject healthBarPrefab;

	private GameObject healthBar;
	private Image healthBarBackground;
	private Image healthBarCurrent;
	private Slider slider;

	private int health;
	private bool lookingAround = false;
	private bool moving = false;

	public void SetWeapon(Weapon weapon)
	{
		this.weapon = weapon;
	}

	public void SetTeam(int team)
	{
		this.team = team;
	}

	void Start()
	{
		health = maxHealth;

		healthBar = Instantiate(healthBarPrefab, FindObjectOfType<Canvas>().transform);
		healthBarBackground = healthBar.GetComponent<Image>();
		healthBarBackground.transform.position = Camera.main.WorldToScreenPoint(transform.position + new Vector3(0.0f, 0.5f, 0.0f));
		healthBarCurrent = new List<Image>(healthBarBackground.GetComponentsInChildren<Image>()).Find(img => img != healthBarBackground);
		slider = healthBarBackground.GetComponent<Slider>();
		slider.maxValue = maxHealth;
		slider.value = health;
	}

	void Update()
	{
		if (health <= 0)
		{
			Destroy(this.gameObject);
		}

		healthBarBackground.transform.position = Camera.main.WorldToScreenPoint(transform.position + new Vector3(0.0f, 0.5f, 0.0f));
		slider.value = health;

		float x = transform.position.x;
		float y = transform.position.y;
		float z = transform.position.z;
		if (x < -50.0f)
		{
			x = 50.0f;
		}
		if (x > 50.0f)
		{
			x = -50.0f;
		}
		if (y < -50.0f)
		{
			y = 50.0f;
		}
		if (y > 50.0f)
		{
			y = -50.0f;
		}
		transform.position = new Vector3(x, y, z);
		if (moving)
		{
			Move();
		}
	}

	void FixedUpdate()
	{
		if (!weapon)
		{
			return;
		}

		var target = Seek();
		if (target)
		{
			moving = false;
			if (Target(target))
			{
				weapon.SetTrigger(true);
			}
		}
		else if (lookingAround)
		{
			moving = false;
			LookAround();
		}
		else
		{
			moving = true;
			weapon.SetTrigger(false);
		}
	}

	void OnCollisionEnter2D(Collision2D collision)
	{
		var obj = collision.gameObject;
		if (!obj.CompareTag("Bullet"))
		{
			return;
		}
		health -= obj.GetComponent<Bullet>().GetDamage();
		lookingAround = true;
		Invoke(nameof(StopLookingAround), 3.0f);
	}

	void OnDestroy()
	{
		Destroy(healthBar);
	}

	private GameObject Seek()
	{
		var filter = new ContactFilter2D();
		filter.NoFilter();
		filter.useTriggers = true;
		var colliders = new List<Collider2D>(20);
		vision.OverlapCollider(filter, colliders);
		GameObject closest = null;
		foreach (var collider in colliders)
		{
			var obj = collider.gameObject;
			if (!obj.TryGetComponent<Unit>(out Unit unit))
			{
				continue;
			}
			if (IsFriend(unit, this))
			{
				continue;
			}
			if (obj == this.gameObject)
			{
				continue;
			}
			if (closest == null)
			{
				closest = obj;
				continue;
			}
			if (Distance(obj) < Distance(closest))
			{
				closest = obj;
			}
		}
		return closest;
	}

	private float Distance(GameObject obj)
	{
		return Vector3.Distance(obj.transform.position, this.transform.position);
	}

	private bool Target(GameObject target)
	{
		Vector3 vectorToTarget = target.transform.position - transform.position;
		float angle = Mathf.Atan2(vectorToTarget.y, vectorToTarget.x) * Mathf.Rad2Deg - rotationModifier;
		Quaternion q = Quaternion.AngleAxis(angle, Vector3.forward);
		Quaternion targetRotation = Quaternion.Slerp(transform.rotation, q, Time.deltaTime * rotationSpeed);
		float rotationAmount = Quaternion.Angle(transform.rotation, targetRotation);
		transform.rotation = targetRotation;
		return rotationAmount < 3.0f;
	}

	private void Move()
	{
		transform.Translate(0, speed * Time.deltaTime, 0);
	}

	private void LookAround()
	{
		transform.Rotate(new Vector3(0.0f, 0.0f, 1.0f), rotationSpeed);
	}

	private void StopLookingAround()
	{
		lookingAround = false;
	}

	public static bool IsFriend(Unit unit1, Unit unit2)
	{
		return unit1.team != 0 && unit1.team == unit2.team;
	}
}
