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
	
	internal List<Element> elements = new List<Element> ();
}


public class Element : IEquatable <Element>
{
	
	public string id = "";
	
	public override String ToString()
	{
		
		return id;
	}

	public bool Equals ( Element other )
	{
		
		return this.id == other.id;
	}
}


public class PlayerManager : MonoBehaviour
{

	
}