using System.Collections.Generic;
using UnityEngine;

public class StateManager : Singleton<StateManager>
{
	private Dictionary<string, bool> _branches = new();

	public void SetBranch(string key, bool value)
	{
		_branches[key] = value;
	}

	public bool GetBranch(string key)
	{
		return _branches.TryGetValue(key, out bool value) && value;
	}

	public bool HasBranch(string key) => _branches.ContainsKey(key);
}
