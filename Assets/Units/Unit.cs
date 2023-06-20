using UnityEngine;
using UnityEngine.UIElements;

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
	[SerializeField] private Vision vision;

	private HealthBar healthBar;
	private int health;
	private Quaternion direction;
	private bool moving = false;
	private GameObject target;

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

	public void SetColor(Color color)
	{
		var spriteRenderer = GetComponent<SpriteRenderer>();
		spriteRenderer.material.color = color;
	}

	void Start()
	{
		direction = transform.rotation;

		healthBar = GetComponent<HealthBar>();
		health = maxHealth;
		healthBar.maxHealth = maxHealth;
		healthBar.health = health;

		vision.onTriggerStay = OnVisionTriggerStay;
		vision.onTriggerExit = OnVisionTriggerExit;
	}

	void Update()
	{
		if (health <= 0)
		{
			Destroy(gameObject);
		}

		healthBar.maxHealth = maxHealth;
		healthBar.health = health;

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

		if (target != null)
		{
			moving = false;
			if (Target(target))
			{
				weapon.SetTrigger(true);
			}
		}
		else
		{
			LookForward();
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
		}
		else if (obj.TryGetComponent(out Base bas))
		{
			if (bas.team.IsAlly(team))
			{
				return;
			}
			health -= bas.Health;
		}
	}

	private float Distance(GameObject obj)
	{
		return Vector3.Distance(obj.transform.position, transform.position);
	}

	private bool Target(GameObject target)
	{
		Vector3 vectorToTarget = target.transform.position - transform.position;
		float angle = Mathf.Atan2(vectorToTarget.y, vectorToTarget.x) * Mathf.Rad2Deg - rotationModifier;
		Quaternion quaternion = Quaternion.AngleAxis(angle, Vector3.forward);
		Quaternion targetRotation = Quaternion.Slerp(transform.rotation, quaternion, Time.fixedDeltaTime * RotationSpeed);
		float rotationAmount = Quaternion.Angle(transform.rotation, quaternion);
		transform.rotation = targetRotation;
		return rotationAmount < 3.0f && rotationAmount > -3.0f;
	}

	private void LookForward()
	{
		transform.rotation = Quaternion.Slerp(transform.rotation, direction, Time.fixedDeltaTime * RotationSpeed);
	}

	private void Move()
	{
		transform.Translate(0, MovingSpeed * Time.deltaTime, 0);
	}

	private void OnVisionTriggerStay(GameObject obj)
	{
		if (obj.TryGetComponent(out Unit unit))
		{
			if (unit.team.IsAlly(team))
			{
				return;
			}
			if (target == null)
			{
				target = obj;
				return;
			}
			if (Distance(obj) < Distance(target))
			{
				target = obj;
				return;
			}
		}
	}

	private void OnVisionTriggerExit(GameObject obj)
	{
		if (obj == target)
		{
			target = null;
		}
	}
}
