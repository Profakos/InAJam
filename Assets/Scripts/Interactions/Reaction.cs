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

	public Sprite sprite;

	public List<string> options = new List<string>();
 
	public List<string> objectsToRemove = new List<string>();

	public List<Condition> setGameState = new List<Condition>(); 


}
