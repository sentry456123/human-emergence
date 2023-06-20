using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(HealthBar))]

public class Base : MonoBehaviour
{
	public Team team;

	[SerializeField] private List<Unit> units;
	[SerializeField] private Quaternion initialUnitRotation;
	[SerializeField] private float cooldown = 2.0f;
	[SerializeField] private int maxHealth = 10000;

	private float lastSpawnTime = 0.0f;
	private int health;
	private HealthBar healthBar;

	public int Health
	{
		get { return health; }
	}

	void Start()
	{
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

		if (lastSpawnTime + cooldown < Time.time)
		{
			Spawn();
			lastSpawnTime = Time.time;
		}
	}

	void OnCollisionEnter2D(Collision2D collision)
	{
		var collider = collision.collider;
		GameObject obj = collider.gameObject;
		if (obj.TryGetComponent(out Unit unit))
		{
			if (unit.team.IsAlly(team))
				return;
			health -= unit.Health;
		}
		else if (obj.TryGetComponent(out Bullet bullet))
		{
			if (bullet.team.IsAlly(team))
				return;
			health -= bullet.Damage;
		}
	}

	private void Spawn()
	{
		var xRange = Random.Range(transform.position.x - transform.lossyScale.x / 2, transform.position.x + transform.lossyScale.x / 2);
		var yRange = Random.Range(transform.position.y - transform.lossyScale.y / 2, transform.position.y + transform.lossyScale.y / 2);
		var position = new Vector3(xRange, yRange, 0.0f);
		var quaternion = initialUnitRotation;
		var unit = Instantiate(units[Random.Range(0, units.Capacity)], position, quaternion);
		unit.team = team;
		if (team.IsAlly(new Team(1)))
		{
			unit.SetColor(Color.red);
		}
		else if (team.IsAlly(new Team(2)))
		{
			unit.SetColor(Color.blue);
		}
	}
}
