using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ReactionCollection 
{
	public string reactionCollectionName;

	public List<Condition> conditionList = new List<Condition> ();

	public List<Reaction> reactions = new List<Reaction>();

	public bool ConditionSatisfied(Dictionary<string, string> gameState)
	{
		foreach (var condition in conditionList) 
		{
			if (!gameState.ContainsKey (condition.name) || gameState [condition.name] != condition.value)
			{
				return false;
			}
		}

		return true;
	}

}
