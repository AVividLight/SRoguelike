using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
//Written by Michael Bethke
public class Position
{
	
	private int xPosition = 0;
	private int zPosition = 0;
	
	public int x
	{
		
		get
		{
			
			return xPosition;
		}
		
		set
		{
			
			xPosition = value;
		}
	}
	
	public int z
	{
		
		get
		{
			
			return zPosition;
		}
		
		set
		{
			
			zPosition = value;
		}
	}
	
	
	public Position ( int pX = -1, int pZ = -1 )
	{
		
		if ( pX > -1 )
		{
			
			x = pX;
		}
		
		if ( pZ > -1 )
		{
			
			z = pZ;
		}
		
	}
}


public class Player
{
	
	public static Player player = new Player ();
	
	public Position position = new Position ( 0, 0 );
	
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