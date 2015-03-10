using System;
using System.Xml;
using UnityEngine;
using Newtonsoft.Json;
using System.Collections;
using System.Xml.Serialization;
using System.Collections.Generic;
//Written by Michael Bethke
public class World
{
	
	public GameObject worldObject;
	public Int2D worldDimensions = new Int2D ();
	
	public Environments environments;
	public float[,] worldPerlin;
	
	public Region[,] regions;
	public Int2D regionDimensions = new Int2D ();
	
	public World ( int argWorldWidth, int argWorldHeight, int argRegionWidth, int argRegionHeight )
	{
		
		this.worldDimensions.x = argWorldWidth;
		this.worldDimensions.z = argWorldHeight;
		
		this.regionDimensions.x = argRegionWidth;
		this.regionDimensions.z = argRegionHeight;
	}
}


public class Region
{
	
	public World world;
	public Int2D position;
	
	public GameObject regionObject;
	
	public Tile[,] tiles;
}


public class Tile
{
	
	public string name;
	
	public Region region;
	public Int2D position;
	
	public GameObject tileObject;
	public bool walkable { get; set; }
	
	public Environment environment;
}


[XmlRoot ( "Environments" )]
public class Environments
{
	
	//[XmlElement ( "Meta" )]
	public EnvironmentsMeta meta = new EnvironmentsMeta ();
	
	//[XmlElement ( "Environment" )]
	//public List<Environment> allEnvironments = new List<Environment> ();
	
	//[XmlIgnore]
	//public SortedDictionary <float, Environment> minimumEnvironmentConditions;
	
	public Dictionary <string, Environment> environmentList = new Dictionary <string, Environment> ();
}


public class EnvironmentsMeta
{
	
	[XmlElement]
	public float seaLevel;
}


public class Environment
{
	
	public String name;

	public float baseColourRed;
	public float baseColourGreen;
	public float baseColourBlue;
	
	public Walkable walkable = new Walkable ();
	
	public Climate climate = new Climate ();
}


public class Walkable
{
	
	public bool foot = false;
	public bool boat = false;
}


public class Climate
{
	
	public float averageTemperature = 0;
	public float averageHumidity = 0;
	public float averagePrecipitation = 0;
	public float averageAtmosphericPressure = 0;
	public float averageWind = 0;
}


public class Int2D
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
	
	
	public Int2D ( int pX = -1, int pZ = -1 )
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
	
	
	public string AsString ()
	{
		
		return "X: " + x + " Z: " + z;
	}
}


public class WorldManager : MonoBehaviour
{
	
	public World world;
	
	private Generator generator;


	private void Start ()
	{
	
		generator = gameObject.GetComponent<Generator> ();
	}
	
	
	internal void CreateNewWorld ( int worldWidth, int worldHeight, int regionWidth, int regionHeight )
	{

		float xSeed = UnityEngine.Random.Range ( 0.00f, 1.00f );
		float ySeed = UnityEngine.Random.Range ( 0.00f, 1.00f );
		
		world = generator.GenerateWorld ( xSeed, ySeed, worldWidth, worldHeight, regionWidth, regionHeight );
	}
}