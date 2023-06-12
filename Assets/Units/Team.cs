using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Team : MonoBehaviour
{
	[SerializeField] private int team = 0;

	public void SetTeam(int team)
	{
		this.team = team;
	}

	public bool IsFriend(Team other)
	{
		if (other.team == 0)
		{
			return false;
		}
		return team == other.team;
	}

	public bool IsEnemy(Team other)
	{
		return !IsFriend(other);
	}
}
