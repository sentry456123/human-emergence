using UnityEngine;

public class Weapon : MonoBehaviour
{
	[HideInInspector] public Unit shooter;

	[SerializeField] private float cooldown = 1.0f;
	[SerializeField] private float recoilPerShot = 0.0f;
	[SerializeField] private float maxRecoil = 0.0f;
	[SerializeField] private Bullet bullet;
	[SerializeField] private GameObject muzzle;
	[SerializeField] private AudioSource shotSound;

	private bool trigger = false;
	private bool ready = true;
	private float recoil = 0.0f;

	public void SetTrigger(bool trigger)
	{
		this.trigger = trigger;
	}

	void Start()
	{
		shooter = GetComponentInParent<Unit>();
	}

	void Update()
	{
		if (trigger && ready)
		{
			Shoot();
			recoil += recoilPerShot;
			shotSound.PlayOneShot(shotSound.clip);
			ready = false;
			Invoke(nameof(ResetCooldown), cooldown);
		}

		if (!trigger)
		{
			recoil *= 0.5f;
		}
		if (recoil > maxRecoil)
		{
			recoil = maxRecoil;
		}
	}

	private void Shoot()
	{
		Vector3 rotation = transform.rotation.eulerAngles;
		rotation.z += Random.Range(-recoil / 0.5f, recoil / 0.5f);
		var instance = Instantiate(bullet, muzzle.transform.position, Quaternion.Euler(rotation));
		instance.team = shooter.team;
	}

	private void ResetCooldown()
	{
		ready = true;
	}
}
