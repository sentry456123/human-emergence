using UnityEngine;

[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(HealthBar))]

public class Unit : MonoBehaviour
{
	public Team team;

	[SerializeField] private float speed = 1.0f;
	[SerializeField] private float rotationSpeed = 1.0f;
	[SerializeField, Range(0, 360)] private float rotationOffset;
	[SerializeField] private int maxHealth = 100;
	[SerializeField] private Weapon weapon;
	[SerializeField] private Vision vision;
	[SerializeField] private bool randomizeStats = true;

	private HealthBar healthBar;
	private int health;
	private Quaternion direction;
	private bool moving = false;
	private GameObject target;
	private float aimingSkill = 1.0f;
	private bool isControlledByPlayer = false;

	public int Health => health;
	public float AimingSkill => aimingSkill;
	private float MovingSpeed => speed;
	private float RotationSpeed => rotationSpeed;

	public void SetWeapon(Weapon weapon) 
	{
		this.weapon = weapon;
	}

	public void SetColor(Color color)
	{
		var spriteRenderer = GetComponent<SpriteRenderer>();
		spriteRenderer.material.color = color;
	}

	public void PlayerBegin()
	{
		isControlledByPlayer = true;
	}

	public void PlayerEnd()
	{
		isControlledByPlayer = false;
	}

	public void PlayerSetFire(bool fire)
	{
		weapon.SetTrigger(fire);
	}

	public void PlayerMove(Vector2 direction)
	{
		direction.Normalize();

		transform.position += new Vector3(direction.x * MovingSpeed * Time.deltaTime, direction.y * MovingSpeed * Time.deltaTime, 0.0f);
	}

	void Start()
	{
		RandomizeStats();

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

		if (!isControlledByPlayer)
		{
			if (moving)
			{
				Move();
			}
		}

	}

	void FixedUpdate()
	{
		if (!isControlledByPlayer)
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
	}

	void OnCollisionEnter2D(Collision2D collision)
	{
		var obj = collision.gameObject;
		if (obj.TryGetComponent(out Bullet bullet))
		{
			if (!bullet.Collidable)
				return;
			if (bullet.team.IsAlly(team))
				return;
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

	private void RandomizeStats()
	{
		float randomNumber = 1.0f;

		if (randomizeStats)
		{
			for (int i = 0; i < 200; i++)
			{
				randomNumber *= Random.Range(0.95f, 1.05f);
			}
			maxHealth = (int)(maxHealth * randomNumber);

			randomNumber = 1.0f;
			for (int i = 0; i < 200; i++)
			{
				randomNumber *= Random.Range(0.95f, 1.05f);
			}
			speed *= randomNumber;

			randomNumber = 1.0f;
			for (int i = 0; i < 200; i++)
			{
				randomNumber *= Random.Range(0.95f, 1.05f);
			}
			rotationSpeed *= randomNumber;

			randomNumber = 1.0f;
			for (int i = 0; i < 10; i++)
			{
				randomNumber *= Random.Range(0.95f, 1.05f);
			}
			vision.GetComponent<CircleCollider2D>().radius *= randomNumber;

			randomNumber = 1.0f;
			for (int i = 0; i < 200; i++)
			{
				randomNumber *= Random.Range(0.95f, 1.05f);
			}
			aimingSkill *= randomNumber;
		}
	}

	private float Distance(GameObject obj)
	{
		return Vector3.Distance(obj.transform.position, transform.position);
	}

	private bool Target(GameObject target)
	{
		Vector3 vectorToTarget = target.transform.position - transform.position;
		float angle = Mathf.Atan2(vectorToTarget.y, vectorToTarget.x) * Mathf.Rad2Deg - rotationOffset;
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
