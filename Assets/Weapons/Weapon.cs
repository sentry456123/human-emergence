using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UIElements;


#nullable enable

public class Weapon : MonoBehaviour
{
	[SerializeField] private float cooldown = 1.0f;
	[SerializeField, Range(0.0f, 360.0f)] private float recoilPerShot = 0.0f;
	[SerializeField, Range(0.0f, 360.0f)] private float maxRecoil = 0.0f;
	[SerializeField, Range(0.0f, 360.0f)] private float scattering = 0.0f;
	[SerializeField] private Bullet bullet;
	[SerializeField] private GameObject muzzle;
	[SerializeField] private AudioClip? shotSound;
	[SerializeField, Range(1, 100)] private int bulletPerShot = 1;

	private Unit shooter;
	private bool trigger = false;
	private bool ready = true;
	private float recoil = 0.0f;

	public Unit Shooter => shooter;

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
			if (shotSound != null)
			{
				AudioSource.PlayClipAtPoint(shotSound, transform.position);
				GameObject gameObject = new GameObject("One shot audio");
				gameObject.transform.position = transform.position;
				AudioSource audioSource = (AudioSource)gameObject.AddComponent(typeof(AudioSource));
				audioSource.clip = shotSound;
				audioSource.spatialBlend = 1.0f;
				audioSource.volume = 1.0f;
				audioSource.Play();
				Destroy(gameObject, shotSound.length * ((Time.timeScale < 0.01f) ? 0.01f : Time.timeScale));
			}
			ready = false;
			Invoke(nameof(ResetCooldown), cooldown);
		}

		if (!trigger)
		{
			recoil -= Time.deltaTime * 10.0f;
		}

		recoil = Mathf.Clamp(recoil, 0.0f, maxRecoil);
	}

	private void Shoot()
	{
		float recoilResult = Random.Range(-recoil * 0.5f, recoil * 0.5f);
		for (int i = 0; i < bulletPerShot; i++)
		{
			Vector3 rotation = transform.rotation.eulerAngles;
			float scatteringResult = Random.Range(-scattering * 0.5f, scattering * 0.5f);

			float result = 0.0f;
			result += recoilResult;
			result /= shooter.AimingSkill;
			result += scatteringResult;
			rotation.z += result;

			Vector3 position = muzzle == null ? transform.position : muzzle.transform.position;
			
			var bullet = Instantiate(this.bullet, position, Quaternion.Euler(rotation));
			bullet.team = shooter.team;
		}
	}

	private void ResetCooldown()
	{
		ready = true;
	}
}
