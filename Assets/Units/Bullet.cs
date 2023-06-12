using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.VFX;

public class Bullet : MonoBehaviour
{
	[SerializeField] private Team team;
	[SerializeField] private float speed = 20.0f;
	[SerializeField] private float range = 3.0f;
	[SerializeField] private int damage = 1;

	private Rigidbody2D rBody;
	private float spawnedTime;

	public int GetDamage()
	{
		return damage;
	}

	void Start()
	{
		rBody = GetComponent<Rigidbody2D>();
		spawnedTime = Time.time;
		
	}

	void Update()
	{
		if (spawnedTime + range / speed < Time.time)
		{
			Destroy(this.gameObject);
		}
	}

	void FixedUpdate()
	{
		rBody.MovePosition(rBody.position + new Vector2(transform.up.x, transform.up.y) * speed * Time.fixedDeltaTime);
	}

	private void OnCollisionEnter2D(Collision2D collision)
	{
		var obj = collision.gameObject;
		if (obj.CompareTag("Bullet"))
		{
			return;
		}
		
		Destroy(this.gameObject);
	}
}
