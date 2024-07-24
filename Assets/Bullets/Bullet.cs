using UnityEngine;

[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(Rigidbody2D))]

public class Bullet : MonoBehaviour
{
	[HideInInspector] public Team team;

	[SerializeField] private float speed = 20.0f;
	[SerializeField] private float range = 3.0f;
	[SerializeField] private int damage = 1;
	[SerializeField] private bool collidable = true;

	private Rigidbody2D rBody;
	private float spawnedTime;

	public int Damage { get => damage; }
	public bool Collidable {  get => collidable; }

	void Start()
	{
		rBody = GetComponent<Rigidbody2D>();
		spawnedTime = Time.time;
	}

	void Update()
	{
		if (spawnedTime + range / speed < Time.time)
		{
			Destroy(gameObject);
		}
	}

	void FixedUpdate()
	{
		rBody.MovePosition(rBody.position + speed * Time.fixedDeltaTime * new Vector2(transform.up.x, transform.up.y));
	}

	private void OnCollisionEnter2D(Collision2D collision)
	{
		if (!collidable)
			return;

		var obj = collision.gameObject;
		if (obj.CompareTag("Bullet"))
			return;
		if (obj.TryGetComponent(out Unit unit))
		{
			if (unit.team.IsAlly(team))
				return;
			Destroy(gameObject);
		} else if (obj.TryGetComponent(out Base bas))
		{
			if (bas.team.IsAlly(team))
				return;
			Destroy(gameObject);
		}
	}
}
