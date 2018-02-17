using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactable : MonoBehaviour 
{
	public List<ReactionCollection> conditionalReactions = new List<ReactionCollection> ();

	public ReactionCollection defaultReaction = new ReactionCollection ();
 

	public ReactionCollection Interact(Dictionary<string, string> gameState)
	{
		foreach (var cR in conditionalReactions) 
		{
			if (cR.ConditionSatisfied (gameState)) 
			{
				return cR;
			}
		}
		
		return defaultReaction;
	}
}
