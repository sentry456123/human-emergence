using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitFactory : MonoBehaviour
{
	[SerializeField] private List<Unit> units;
	[SerializeField] private List<Weapon> weapons;
	[SerializeField] private float cooldown = 2.0f;

	private float lastSpawnTime = 0.0f;

	void Update()
	{
		if (lastSpawnTime + cooldown < Time.time)
		{
			Spawn();
			lastSpawnTime = Time.time;
		}
	}

	private void Spawn()
	{
		var position = new Vector3(Random.Range(-50.0f, 50.0f), Random.Range(-50.0f, 50.0f), 0.0f);
		var quaternion = Quaternion.Euler(0.0f, 0.0f, Random.Range(0.0f, 360.0f));
		var unit = Instantiate(units[Random.Range(0, units.Capacity)], position, quaternion);
		var weapon = Instantiate(weapons[Random.Range(0, weapons.Capacity)], unit.transform);
		unit.SetWeapon(weapon);
		unit.GetComponent<Team>().SetTeam(Random.Range(1, 3));
	}
}
