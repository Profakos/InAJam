using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactable : MonoBehaviour 
{
	public ReactionCollection reaction = new ReactionCollection ();

	public ReactionCollection Interact()
	{
		return reaction;
	}
}
