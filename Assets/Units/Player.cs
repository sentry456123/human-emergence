using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
	[SerializeField] private Team team;
	[SerializeField] private float speed = 1.0f;
	[SerializeField] private int maxHealth = 10;
	[SerializeField] private Weapon weapon;
	[SerializeField] private GameObject healthBarPrefab;

	private GameObject healthBar;
	private Image healthBarBackground;
	private Slider slider;

	private int health;

	public void SetWeapon(Weapon weapon)
	{
		this.weapon = weapon;
	}

	void Start()
	{
		health = maxHealth;

		healthBar = Instantiate(healthBarPrefab, FindObjectOfType<Canvas>().transform);
		healthBarBackground = healthBar.GetComponent<Image>();
		healthBarBackground.transform.position = Camera.main.WorldToScreenPoint(transform.position + new Vector3(0.0f, 0.5f, 0.0f));
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

		if (Input.GetMouseButtonDown(0))
		{
			weapon.SetTrigger(true);
		}
		if (Input.GetMouseButtonUp(0))
		{
			weapon.SetTrigger(false);
		}
		var mouseScreenPos = Input.mousePosition;
		var startingScreenPos = Camera.main.WorldToScreenPoint(transform.position);
		mouseScreenPos.x -= startingScreenPos.x;
		mouseScreenPos.y -= startingScreenPos.y;
		var angle = Mathf.Atan2(mouseScreenPos.y, mouseScreenPos.x) * Mathf.Rad2Deg;
		transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle - 90.0f));


		if (Input.GetKey(KeyCode.W))
		{
			transform.Translate(0.0f, speed * Time.deltaTime, 0.0f, Space.World);
		}
		if (Input.GetKey(KeyCode.S))
		{
			transform.Translate(0.0f, -speed * Time.deltaTime, 0.0f, Space.World);
		}
		if (Input.GetKey(KeyCode.A))
		{
			transform.Translate(-speed * Time.deltaTime, 0.0f, 0.0f, Space.World);
		}
		if (Input.GetKey(KeyCode.D))
		{
			transform.Translate(speed * Time.deltaTime, 0.0f, 0.0f, Space.World);
		}
	}

	void OnCollisionEnter2D(Collision2D collision)
	{
		var obj = collision.gameObject;
		if (!obj.CompareTag("Bullet"))
		{
			return;
		}
		if (obj.GetComponent<Team>().IsFriend(team))
		{
			return;
		}
		health -= obj.GetComponent<Bullet>().GetDamage();
	}

	void OnDestroy()
	{
		Destroy(healthBar);
	}

}
