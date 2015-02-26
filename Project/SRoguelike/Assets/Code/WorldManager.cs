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
	public Environments environments;
	public Color[,] worldPerlin;
	
	public int worldWidth { get; set; }
	public int worldHeight { get; set; }
	
	
	public Region[,] regions;
	
	public int regionWidth { get; set; }
	public int regionHeight { get; set; }
	
	public World ( int argWorldWidth, int argWorldHeight, int argRegionWidth, int argRegionHeight )
	{
		
		this.worldWidth = argWorldWidth;
		this.worldHeight = argWorldHeight;
		
		this.regionWidth = argRegionWidth;
		this.regionHeight = argRegionHeight;
	}
}


public class Region
{
	
	public World world;
	public GameObject regionObject;
	
	public Tile[,] tiles;
}


public class Tile
{
	
	public Region region;
	
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
