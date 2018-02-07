using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Reaction 
{
	public string displayedText;
 
	public List<GameObject> objectsToRemove = new List<GameObject>();

	public List<Condition> setGameState = new List<Condition>(); 

}
