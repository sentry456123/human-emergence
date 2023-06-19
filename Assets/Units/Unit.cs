using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(HealthBar))]

public class Unit : MonoBehaviour
{
	public Team team;

	[SerializeField] private float speed = 1.0f;
	[SerializeField] private float rotationSpeed = 1.0f;
	[SerializeField, Range(0, 360)] private float rotationModifier;
	[SerializeField] private int maxHealth = 100;
	[SerializeField] private Weapon weapon;
	[SerializeField] private Collider2D vision;

	private HealthBar healthBar;
	private int health;
	private Quaternion initialRotation;
	private bool lookingAround = false;
	private bool moving = false;

	public int Health
	{
		get { return health; }
	}

	private float MovingSpeed
	{
		get { return speed * ((float)health / (float)maxHealth); }
	}

	private float RotationSpeed
	{
		get { return rotationSpeed * ((float)health / (float)maxHealth); }
	}

	public void SetWeapon(Weapon weapon)
	{
		this.weapon = weapon;
	}

	void Start()
	{
		initialRotation = transform.rotation;

		healthBar = GetComponent<HealthBar>();
		health = maxHealth;
		healthBar.maxHealth = maxHealth;
		healthBar.health = health;
	}

	void Update()
	{
		if (health <= 0)
		{
			Destroy(gameObject);
		}

		healthBar.maxHealth = maxHealth;
		healthBar.health = health;

		float x = transform.position.x;
		float y = transform.position.y;
		float z = transform.position.z;
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
			transform.rotation = initialRotation;
			moving = true;
			weapon.SetTrigger(false);
		}
	}

	void OnCollisionEnter2D(Collision2D collision)
	{
		var obj = collision.gameObject;
		if (obj.TryGetComponent(out Bullet bullet))
		{
			if (bullet.team.IsAlly(team))
			{
				return;
			}
			health -= bullet.Damage;
			lookingAround = true;
			Invoke(nameof(StopLookingAround), 3.0f);
		}
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
			if (obj == gameObject)
				continue;
			if (obj.TryGetComponent(out Unit unit))
			{
				if (unit.team.IsAlly(team))
					continue;
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
			else if (obj.TryGetComponent(out Base bas))
			{
				if (bas.team.IsAlly(team))
					continue;
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
		}
		return closest;
	}

	private float Distance(GameObject obj)
	{
		return Vector3.Distance(obj.transform.position, transform.position);
	}

	private bool Target(GameObject target)
	{
		Vector3 vectorToTarget = target.transform.position - transform.position;
		float angle = Mathf.Atan2(vectorToTarget.y, vectorToTarget.x) * Mathf.Rad2Deg - rotationModifier;
		Quaternion q = Quaternion.AngleAxis(angle, Vector3.forward);
		Quaternion targetRotation = Quaternion.Slerp(transform.rotation, q, Time.deltaTime * RotationSpeed);
		float rotationAmount = Quaternion.Angle(transform.rotation, targetRotation);
		transform.rotation = targetRotation;
		return rotationAmount < 3.0f;
	}

	private void Move()
	{
		transform.Translate(0, MovingSpeed * Time.deltaTime, 0);
	}

	private void LookAround()
	{
		transform.Rotate(new Vector3(0.0f, 0.0f, 1.0f), RotationSpeed);
	}

	private void StopLookingAround()
	{
		lookingAround = false;
	}
}
