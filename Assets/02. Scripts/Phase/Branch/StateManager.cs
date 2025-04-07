using System.Collections.Generic;
using UnityEngine;

public class StateManager : Singleton<StateManager>
{
	private HashSet<string> _branches = new();

	public void SetBranch(string key)
	{
		if (!_branches.Contains(key))
		{
			_branches.Add(key);
		}
	}

	public bool GetBranch(string key)
	{
		return _branches.Contains(key);
	}
}
