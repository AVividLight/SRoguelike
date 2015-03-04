using System;
using System.Xml;
using UnityEngine;
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
	
	[XmlElement ( "Environment" )]
	public List<Environment> allEnvironments = new List<Environment> ();
	
	[XmlIgnore]
	public SortedDictionary <float, Environment> minimumEnvironmentConditions;
}


public class Environment
{
	
	[XmlAttribute]
	public String name;
	
	[XmlElement]
	public String baseColourRed;
	public String baseColourGreen;
	public String baseColourBlue;
	
	public String minElevation;
	public String maxElevation;
	
	public Climate climate;
	
	[XmlIgnore]
	public Color baseColour;
}


public class Climate
{
	
	public String averageTemperature;
	public String averageHumidity;
	public String averagePrecipitation;
	public String averageAtmosphericPressure;
	public String averageWind;
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
}


public class WorldManager : MonoBehaviour
{
	
	public World world;
	
	private Generator generator;
	
	//public Color testColour;


	private void Start ()
	{
	
		generator = gameObject.GetComponent<Generator> ();
		
		//UnityEngine.Debug.Log ( testColour );
	}
	
	
	internal void CreateNewWorld ( int worldWidth, int worldHeight, int regionWidth, int regionHeight )
	{

		float xSeed = UnityEngine.Random.Range ( 0.00f, 1.00f );
		float ySeed = UnityEngine.Random.Range ( 0.00f, 1.00f );
		
		world = generator.GenerateWorld ( xSeed, ySeed, worldWidth, worldHeight, regionWidth, regionHeight );
	}
}
