using UnityEngine;

public class Weapon : MonoBehaviour
{
	[SerializeField] private float cooldown = 1.0f;
	[SerializeField] private float recoilPerShot = 0.0f;
	[SerializeField] private float maxRecoil = 0.0f;
	[SerializeField] private GameObject bullet;
	[SerializeField] private GameObject muzzle;
	[SerializeField] private AudioSource shotSound;

	private bool trigger = false;
	private bool canShoot = true;
	private float recoil = 0.0f;

	public void SetTrigger(bool trigger)
	{
		this.trigger = trigger;
	}

	void Update()
	{
		if (trigger && canShoot)
		{
			Shoot();
			recoil += recoilPerShot;
			shotSound.PlayOneShot(shotSound.clip);
			canShoot = false;
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
		Instantiate(bullet, muzzle.transform.position, Quaternion.Euler(rotation));
	}

	private void ResetCooldown()
	{
		canShoot = true;
	}
}
