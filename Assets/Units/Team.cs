using System;
using UnityEngine;

[Serializable]
public class Team
{
	[SerializeField] private int id;

	public Team(int id)
	{
		this.id = id;
	}

	public void SetID(int id)
	{
		this.id = id;
	}

	public bool IsAlly(Team other)
	{
		if (other.id == 0)
		{
			return false;
		}
		return id == other.id;
	}

	public bool IsEnemy(Team other)
	{
		return !IsAlly(other);
	}
}
