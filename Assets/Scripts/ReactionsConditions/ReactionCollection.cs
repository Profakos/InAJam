using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ReactionCollection 
{
	public Condition condition = new Condition ();

	public List<Reaction> reactions = new List<Reaction>();

	public bool ConditionSatisfied(Dictionary<string, string> gameState)
	{
		if (gameState.ContainsKey (condition.name) && gameState [condition.name] == condition.value) 
		{
			return true;
		}

		return false;
	}

}
