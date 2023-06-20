using UnityEngine;

public class Weapon : MonoBehaviour
{
	[HideInInspector] public Unit shooter;

	[SerializeField] private float cooldown = 1.0f;
	[SerializeField, Range(0.0f, 360.0f)] private float recoilPerShot = 0.0f;
	[SerializeField, Range(0.0f, 360.0f)] private float maxRecoil = 0.0f;
	[SerializeField, Range(0.0f, 360.0f)] private float scattering = 0.0f;
	[SerializeField] private Bullet bullet;
	[SerializeField] private GameObject muzzle;
	[SerializeField] private AudioSource shotSound;
	[SerializeField, Range(1, 100)] private int bulletPerShot = 1;

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
		recoil = scattering;
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
			recoil = 0.5f;
		}
		if (recoil > maxRecoil)
		{
			recoil = maxRecoil;
		}
	}

	private void Shoot()
	{
		float recoilResult = Random.Range(-recoil * 0.5f, recoil * 0.5f);
		for (int i = 0; i < bulletPerShot; i++)
		{
			Vector3 rotation = transform.rotation.eulerAngles;
			float scatteringResult = Random.Range(-scattering * 0.5f, scattering * 0.5f);
			rotation.z += recoilResult;
			rotation.z += scatteringResult;
			var bullet = Instantiate(this.bullet, muzzle.transform.position, Quaternion.Euler(rotation));
			bullet.team = shooter.team;
		}
	}

	private void ResetCooldown()
	{
		ready = true;
	}
}
