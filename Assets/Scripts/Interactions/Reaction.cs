using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Reaction 
{
	public string displayName;

	public string text;

	public string portrait;

	public ImageLocation imageLocation;

	public bool flipImage;
 
	public List<GameObject> objectsToRemove = new List<GameObject>();

	public List<Condition> setGameState = new List<Condition>(); 

}
