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
	
	public int tileSize;
	
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
	
	public Region Top
	{
			
		get
		{
			
			if ( position.z + 1 > world.worldDimensions.z - 1 )
			{
					
				return null;
			}
			
			return world.regions[position.x, position.z + 1];
		}
	}
	public Region Left
	{
		
		get
		{
			
			if ( position.x - 1 < 0 )
			{
				
				return null;
			}
			
			return world.regions[position.x - 1, position.z];
		}
	}
	public Region Right
	{
		
		get
		{
			
			if ( position.x + 1 > world.worldDimensions.x - 1 )
			{
				
				return null;
			}
			
			return world.regions[position.x + 1, position.z];
		}
	}
	public Region Bottom
	{
		
		get
		{
			
			if ( position.z - 1 < 0 )
			{
				
				return null;
			}
			
			return world.regions[position.x, position.z - 1];
		}
	}
	
	public Tile[,] tiles;
	public Texture2D map;
	
	public GameObject regionObject;
}


public class Tile
{
	
	public string name;
	
	public Region region;
	public Int2D position;
	public Int2D globalPosition
	{
		
		get
		{
			
			return new Int2D (( region.position.x * region.world.regionDimensions.x ) + position.x, ( region.position.z * region.world.regionDimensions.z ) + position.z );
		}
		
		/*
		Convert Global to Local (if it's ever needed)
		
		int tLocalOrigin = tile.globalPosition.z - range;
		int tLocalRegion = tLocalOrigin / tile.region.world.regionDimensions.z;
		int tLocalTile = tLocalOrigin % tile.region.world.regionDimensions.z;
	    
		UnityEngine.Debug.Log ( tLocalOrigin + ", " + tLocalRegion + ", " + tLocalTile );
		*/
	}
	
	public Tile Top
	{
			
		get
		{
			
			if ( position.z + 1 > region.world.regionDimensions.z - 1 )
			{
				
				if ( region.position.z + 1 > region.world.worldDimensions.z - 1 )
				{
					
					return null;
				}
				
				return region.world.regions[region.position.x, region.position.z + 1].tiles[position.x, 0];
			}
			
			return region.tiles[position.x, position.z + 1];
		}
	}
	public Tile Left
	{
		
		get
		{
			
			if ( position.x - 1 < 0 )
			{
				
				if ( region.position.x - 1 < 0 )
				{
					
					return null;
				}
				
				return region.world.regions[region.position.x - 1, region.position.z].tiles[region.world.regionDimensions.x - 1, position.z];
			}
			
			return region.tiles[position.x - 1, position.z];
		}
	}
	public Tile Right
	{
		
		get
		{
			
			if ( position.x + 1 > region.world.regionDimensions.x - 1 )
			{
				
				if ( region.position.x + 1 > region.world.worldDimensions.x - 1  )
				{
					
					return null;
				}
				
				return region.world.regions[region.position.x + 1, region.position.z].tiles[0, position.z];
			}
			
			return region.tiles[position.x + 1, position.z];
		}
	}
	public Tile Bottom
	{
		
		get
		{
			
			if ( position.z - 1 < 0 )
			{
				
				if ( region.position.z - 1 < 0 )
				{
					
					return null;
				}
				
				return region.world.regions[region.position.x, region.position.z - 1].tiles[position.x, region.world.regionDimensions.z - 1];
			}
			
			return region.tiles[position.x, position.z - 1];
		}
	}
	
	public Color[,] pixels;
	public Environment environment;
	
	public bool walkable { get; set; }
}


public class Environments
{
	
	public EnvironmentsMeta meta = new EnvironmentsMeta ();
	
	public Dictionary <string, Environment> environmentList = new Dictionary <string, Environment> ();
}


public class EnvironmentsMeta
{
	
	public float seaLevel;
}


public class Environment : IEquatable <Environment>
{
	
	public String name = "Default";

	public float baseColourRed = 0.000f;
	public float baseColourGreen = 0.000f;
	public float baseColourBlue = 0.000f;
	
	public Walkable walkable = new Walkable ();
	
	public Climate climate = new Climate ();
	
	
	public bool Equals ( Environment other )
	{
		
		if ( other is Environment == false )
		{
			
			return false;
		}
		
		return this.name == other.name;
	}
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
	
	
	internal void CreateNewWorld ( Int2D worldSize, Int2D regionSize, int desiredTileSize )
	{
		
		int tileSize = desiredTileSize; //Determine if tileSize will fit in a region, fix if not ( Euclidean Algorithm ?)

		Vector2 seed = new Vector2 ( UnityEngine.Random.Range ( 0.00f, 1.00f ), UnityEngine.Random.Range ( 0.00f, 1.00f ));
		
		world = generator.GenerateWorld ( seed, worldSize, regionSize, tileSize );
	}
}