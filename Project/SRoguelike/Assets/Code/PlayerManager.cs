using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
//Written by Michael Bethke
public class Player
{
	
	public static Player player = new Player ();
	
	private Font handWriting;
	public Font HandWriting
	{
		
		get
		{
			
			return handWriting;
		}
		
		set
		{
			
			handWriting = value;
		}
	}
	
	private string name;
	public string Name 
	{
		
		get 
		{
			
			return name; 
		}
		
		set 
		{
			
			name = value; 
		}
	}
	
	internal List<Property> properties = new List<Property> ();
}


public class PlayerManager : MonoBehaviour
{

	
}