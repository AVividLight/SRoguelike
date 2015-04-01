using System;
using System.IO;
using System.Linq;
using UnityEngine;
using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
//Written by Michael Bethke
public class Generator : MonoBehaviour
{
	
	private WorldManager worldManager;
	
	static internal string loadingInformation;
	private sbyte loadingStage = -1;
	
	private Shader selfIllumDiffuse;
	private Environment defaultEnvironment = new Environment ();

	private Queue<Region> loadingRegionQueue = new Queue<Region> ();
	
	
	private void Start ()
	{
		
		worldManager = GameObject.FindGameObjectWithTag ( "WorldManager" ).GetComponent<WorldManager> ();
		selfIllumDiffuse = Shader.Find ( "Self-Illumin/Diffuse" ); 
	}
	
	
	private void Update ()
	{
		
		if ( loadingStage != -1 )
		{
			
			switch ( loadingStage )
			{
				
				case 0:
				{
					
					if ( loadingRegionQueue.Count () == 0 )
					{
						
						RepopulateRegionQueue ();
					}
					
					int regionsThisFrame = 0;
					while ( regionsThisFrame < 1 )
					{
                	
						int totalToLoad = loadingRegionQueue.Count ();
						if ( totalToLoad > 0 )
						{
							
							
							string singularOrPlural = "s";
							if ( totalToLoad - 1 == 1 )
							{
								
								singularOrPlural = "";
							}
							
							loadingInformation = "Creating " + ( totalToLoad - 1 ) + " region" + singularOrPlural + ".";
						
							Region currentRegion = loadingRegionQueue.Dequeue ();
							currentRegion.regionObject = CreateRegionObject ( currentRegion );
						}
						
						if ( loadingRegionQueue.Count () == 0 )
						{
                	
							loadingStage = 1;
							break;
						}
						
						regionsThisFrame += 1;
					}
				}
				break;
				
				case 1:
				{
					
					if ( loadingRegionQueue.Count () == 0 )
					{
						
						RepopulateRegionQueue ();
					}
					
					int regionsThisFrame = 0;
					while ( regionsThisFrame < 1 )
					{
                	
						int totalToLoad = loadingRegionQueue.Count ();
						if ( totalToLoad > 0 )
						{
							
							
							string singularOrPlural = "s";
							if ( totalToLoad - 1 == 1 )
							{
								
								singularOrPlural = "";
							}
							
							loadingInformation = "Analysing " + ( totalToLoad - 1 ) + " region" + singularOrPlural + " for water.";
						
							Region currentRegion = loadingRegionQueue.Dequeue ();
							currentRegion.map = CreateWater ( currentRegion );
							UpdateTexture ( currentRegion.map );
						}
						
						if ( loadingRegionQueue.Count () == 0 )
						{
                	
							loadingStage = 2;
							break;
						}
						
						regionsThisFrame += 1;
					}
				}
				break;
				
				/*
				case 2:
				int tileGrassPerFrame = 0;
				while ( tileGrassPerFrame < worldManager.world.regionDimensions.x * worldManager.world.regionDimensions.z * 1 )
				{
					
					if ( tileQueue.Count > 0 )
					{
						
						loadingInformation = "Analysing " + ( tileQueue.Count () - 1 ) + " tiles for grass";
						
						if ( CreateGrass ( tileQueue.Dequeue ()) != 0 )
						{
							
							UnityEngine.Debug.Log ( "Unable to create grass, " + tileQueue.Peek ().name );
						}
					} else {
							
						RepopulateTileQueue ();
						loadingStage = 3;
						break;
					}
					
					tileGrassPerFrame += 1;
				}
				break;
				
				case 3:
				int tileShoresPerFrame = 0;
				while ( tileShoresPerFrame < worldManager.world.regionDimensions.x * worldManager.world.regionDimensions.z * 1 )
				{
			
					if ( tileQueue.Count () > 0 )
					{
						
						loadingInformation = "Analysing " + ( tileQueue.Count () - 1 ) + " tiles for shore.";
						
						if ( CreateShore ( tileQueue.Dequeue ()) != 0 )
						{
							
							UnityEngine.Debug.Log ( "Unable to create shore, " + tileQueue.Peek ().name );
						}
					} else {
							
						RepopulateTileQueue ();
						loadingStage = 4;
						break;
					}
					
					tileShoresPerFrame += 1;
				}
				break;
				
				default:
				break;*/
			}
		}
	}
	
	
	private void RepopulateRegionQueue ()
	{
		
		loadingRegionQueue = new Queue<Region> ();
		
		Int2D regionIndex = new Int2D ( 0, 0 );
		while ( regionIndex.z < worldManager.world.worldDimensions.z )
		{
			
			regionIndex.x = 0;
			while ( regionIndex.x < worldManager.world.worldDimensions.x )
			{
				
				loadingRegionQueue.Enqueue ( worldManager.world.regions[regionIndex.x, regionIndex.z] );
				regionIndex.x += 1;
			}
			
			regionIndex.z += 1;
		}
	}
	
	
	/*private void RepopulateTileQueue ()
	{
		
		//tileQueue = new Queue<Tile> ();
		
		int regionYIndex = 0;
		while ( regionYIndex < worldManager.world.worldDimensions.z )
		{
	
			int regionXIndex = 0;
			while ( regionXIndex < worldManager.world.worldDimensions.x )
			{
		
	   			int tileYIndex = 0;
	   			while ( tileYIndex < worldManager.world.regionDimensions.z )
	   			{
				
	   				int tileXIndex = 0;
	   				while ( tileXIndex < worldManager.world.regionDimensions.x )
	   				{
					
	   					//tileQueue.Enqueue ( worldManager.world.regions[regionXIndex, regionYIndex].tiles[tileXIndex, tileYIndex] );
	   					tileXIndex += 1;
	   				}
	   				tileYIndex += 1;
	   			}
				regionXIndex += 1;
			}
			regionYIndex += 1;
		}
	}*/
	
	
	private GameObject CreateRegionObject ( Region region )
	{
		
		GameObject newRegionObject = new GameObject ();
		newRegionObject.name = "Region" + region.position.x + "," + region.position.z;
		
		MeshFilter meshFilter = ( MeshFilter ) newRegionObject.AddComponent ( typeof ( MeshFilter ));
		meshFilter.mesh = NewPlaneMesh ( new Vector2 ( region.world.regionDimensions.x / 2, region.world.regionDimensions.z / 2 ));
		
		MeshRenderer renderer = newRegionObject.AddComponent ( typeof ( MeshRenderer )) as MeshRenderer;
		renderer.material.mainTexture = region.map;
		renderer.material.shader = selfIllumDiffuse;

		newRegionObject.transform.position = new Vector3 (( region.position.x * region.world.regionDimensions.x ), 0, ( region.world.regionDimensions.z * region.position.z ) );
		newRegionObject.transform.parent = region.world.worldObject.transform;
		newRegionObject.transform.Rotate ( 270, 0, 0 );
		
		return newRegionObject;
	}
	
	
	private Mesh NewPlaneMesh ( Vector2 size )
	{
		
		Mesh newMesh = new Mesh();
		newMesh.name = "Plane";
		newMesh.vertices = new Vector3[]
		{
			
			new Vector3 ( -size.x, -size.y, 0.01f ),
			new Vector3 ( size.x, -size.y, 0.01f ),
			new Vector3 ( size.x, size.y, 0.01f ),
			new Vector3 ( -size.x, size.y, 0.01f )
		};
		
		newMesh.uv = new Vector2[]
		{
			
			new Vector2 ( 0, 0 ),
			new Vector2 ( 0, 1 ),
			new Vector2 ( 1, 1 ),
			new Vector2 ( 1, 0 )
		};
		
		newMesh.triangles = new int[] { 0, 1, 2, 0, 2, 3};
		newMesh.RecalculateNormals();
   		
		return newMesh;
	}
	
	
	/*public static T[] FlattenArray<T> ( T[,] twoDArray ) where T : class
	{
		
		if ( twoDArray != null )
		{
			
			T[] oneDArray = new T[twoDArray.GetLength ( 0 )  * twoDArray.GetLength ( 1 )];
		
			Int2D arrayIndex = new Int2D ( 0, 0 );
			while ( arrayIndex.z < twoDArray.GetLength ( 1 ))
			{
			
				arrayIndex.x = 0;
				while ( arrayIndex.x < twoDArray.GetLength ( 0 ))
				{
				
					oneDArray[(arrayIndex.z * twoDArray.GetLength ( 0 )) + arrayIndex.x] = ( twoDArray [arrayIndex.x, arrayIndex.z] );
					arrayIndex.x += 1;
				}
			
				arrayIndex.z += 1;
			}
			
			return oneDArray;
		}
	
		UnityEngine.Debug.LogError ( "The Original Array is Null!" );
		return null;
	}*/
	
	
	private Color[] FlattenColourArray ( Color[,] twoDArray )
	{
		
		if ( twoDArray != null )
		{
			
			Color[] oneDArray = new Color[twoDArray.GetLength ( 0 )  * twoDArray.GetLength ( 1 )];
		
			Int2D arrayIndex = new Int2D ( 0, 0 );
			while ( arrayIndex.z < twoDArray.GetLength ( 1 ))
			{
			
				arrayIndex.x = 0;
				while ( arrayIndex.x < twoDArray.GetLength ( 0 ))
				{
				
					oneDArray[(arrayIndex.z * twoDArray.GetLength ( 0 )) + arrayIndex.x] = ( twoDArray [arrayIndex.x, arrayIndex.z] );
					arrayIndex.x += 1;
				}
			
				arrayIndex.z += 1;
			}
			
			return oneDArray;
		}
	
		UnityEngine.Debug.LogError ( "The Original Array is Null!" );
		return null;
	}
	
	
	private Texture2D CreateWater ( Region localRegion )
	{
		
		Environment water;
		if ( worldManager.world.environments.environmentList.TryGetValue ( "Water", out water ))
		{
		
			Color[,] newTextureColours = new Color[ localRegion.world.tileSize * localRegion.world.regionDimensions.x, localRegion.world.tileSize * localRegion.world.regionDimensions.z ];
			
   			Int2D localTileIndex = new Int2D ( 0, 0 );
   			while ( localTileIndex.z < worldManager.world.regionDimensions.z )
   			{
			
				localTileIndex.x = 0;
   				while ( localTileIndex.x < worldManager.world.regionDimensions.x )
   				{
					
					if ( GetTilePerlin ( localRegion.tiles[localTileIndex.x, localTileIndex.z] ) < worldManager.world.environments.meta.seaLevel )
					{
				
						SetTileEnvironment ( localRegion.tiles[localTileIndex.x, localTileIndex.z], water );
					}
					
					Int2D offset = new Int2D ( localRegion.tiles[localTileIndex.x, localTileIndex.z].position.x * localRegion.world.tileSize, localRegion.tiles[localTileIndex.x, localTileIndex.z].position.z * localRegion.world.tileSize );
					foreach ( Color localColour in localRegion.tiles[localTileIndex.x, localTileIndex.z].pixels )
					{
						
						//UnityEngine.Debug.Log ( "L x " + newTextureColours.GetLength ( 0 ) + " z " + newTextureColours.GetLength ( 1 ));
						//UnityEngine.Debug.Log ( "I x " + localTileIndex.x + " z " + localTileIndex.z );
						//UnityEngine.Debug.Log ( "O x " + offset.x + " z " + offset.z );
						//UnityEngine.Debug.Log ( "C x " + ( localTileIndex.x + offset.x ) + " z " + ( localTileIndex.z + offset.z ));
						newTextureColours[offset.x, offset.z] = Color.red; //localColour;
					}
					
   					localTileIndex.x += 1;
   				}
				
   				localTileIndex.z += 1;
   			}
			
			Texture2D newTexture = new Texture2D ( localRegion.world.tileSize * localRegion.world.regionDimensions.x, localRegion.world.tileSize * localRegion.world.regionDimensions.z );
			newTexture.SetPixels ( FlattenColourArray ( newTextureColours ), 0 );
			
			return newTexture;
		}
		
		return null;
	}
	
	
	private void SetTileEnvironment ( Tile tile, Environment environment )
	{
		
		tile.environment = environment;
		
		Int2D pixelIndex = new Int2D ( 0, 0 );
		while ( pixelIndex.z < worldManager.world.tileSize )
		{
		
			pixelIndex.x = 0;
			while ( pixelIndex.x < worldManager.world.tileSize )
			{
				
				tile.pixels[pixelIndex.x, pixelIndex.z] = GetTileColour ( tile );
				
				pixelIndex.x += 1;
			}
			
			pixelIndex.z += 1;
		}
	}
	
	
	private Color GetTileColour ( Tile tile )
	{
		
		float minorVariation = UnityEngine.Random.Range ( 0.000f, 0.020f ) - 0.010f;
		Color baseColour = new Color ( tile.environment.baseColourRed + minorVariation, tile.environment.baseColourGreen + minorVariation, tile.environment.baseColourBlue + minorVariation, 1 );
		
		return baseColour;
	}
	
	
	private Texture2D UpdateTexture ( Texture2D texture )
	{
		
		texture.Apply ( false, false );
		
		return texture;
	}
	
	
	/*private int CreateWater ( Tile tile )
	{
		
		Environment water;
		if ( worldManager.world.environments.environmentList.TryGetValue ( "Water", out water ))
		{
		
			if ( GetTilePerlin ( tile ) < worldManager.world.environments.meta.seaLevel )
			{
				
				SetTileEnvironment ( tile, water );
			}

			return 0;
		}
		
		UnityEngine.Debug.Log ( "Unable to find environment 'Water'!" );
		return 1;
	}
	
	
	private int CreateGrass ( Tile tile )
	{
		
		Environment grassland;
		if ( !worldManager.world.environments.environmentList.TryGetValue ( "Grassland", out grassland ))
		{

			return 1;
		}
		
		
		if ( tile.environment.name == "Default" )
		{

			if ( FindNearest ( tile, 5, "Water" ) != null )
			{
			
				SetTileEnvironment ( tile, grassland );
			}
		}
		
		return 0;
	}
	
	
	private int CreateShore ( Tile tile )
	{
		
		Environment shore;
		if ( !worldManager.world.environments.environmentList.TryGetValue ( "Shore", out shore ))
		{

			return 1;
		}
		
		
		if ( tile.environment.name != "Water" )
		{

			if ( FindNearest ( tile, 1, "Water" ) != null )
			{
			
				SetTileEnvironment ( tile, shore );
			}
		}
		
		return 0;
	}*/
	
	
	private Tile FindNearest ( Tile tile, int range, String environmentName )
	{
		
		List<Int2D> visitedTiles = new List<Int2D> ();
		
		Queue<Tile> queue = new Queue<Tile> ();
		queue.Enqueue ( tile );
		
		int tilesToVisit = Mathf.CeilToInt ( Mathf.Pow (( range*2 )+1, 2 )/2 );
		while ( queue.Count > 0 )
		{
			
			Tile current = queue.Dequeue ();
			if ( current == null )
			{
				
				tilesToVisit -= 1;
				continue;
			}
			
			if ( visitedTiles.Contains ( current.position ))
			{
				
				continue;
			}
			
			if ( current.environment.name == environmentName )
			{
				
				return current;
			}
			
			visitedTiles.Add ( current.position );
			if ( visitedTiles.Count () >= tilesToVisit )
			{

				break;
			}

			queue.Enqueue ( current.Top );
			queue.Enqueue ( current.Left );
			queue.Enqueue ( current.Right );
			queue.Enqueue ( current.Bottom );
		}
		
		return null;
	}
	
	
	public World GenerateWorld ( Vector2 seed, Int2D worldSize, Int2D regionSize, int tileSize )
	{
			
		World newWorld = new World ( worldSize.x, worldSize.z, regionSize.x, regionSize.z );
		
		GameObject newWorldObject = new GameObject ();
		newWorldObject.transform.parent = gameObject.transform;
		newWorldObject.name = "NewWorld";
		newWorld.worldObject = newWorldObject;
		
		newWorld.worldPerlin = GeneratePerlinNoise ( seed, new Int2D (( tileSize * regionSize.x ) * worldSize.x, ( tileSize * regionSize.z ) * worldSize.z ));
		newWorld.environments = GetEnvironments ();
		
		newWorld.tileSize = tileSize;
		newWorld.regions = CreateAllRegions ( newWorld );
		
		loadingStage = 0;
		return newWorld;
	}
	
	
	private Environments GetEnvironments ()
	{
		
		//Catch errors
		Environments environments = JsonConvert.DeserializeObject<Environments> ( File.ReadAllText ( System.Environment.GetFolderPath ( System.Environment.SpecialFolder.Desktop ) + Path.DirectorySeparatorChar + "Default" + Path.DirectorySeparatorChar + "Environments.json" ));
		
		return environments;
	}
	
	
	private Region[,] CreateAllRegions ( World newWorld )
	{
		
		Region[,] newRegions = new Region[newWorld.worldDimensions.x, newWorld.worldDimensions.z];
		
		Int2D regionIndex = new Int2D ( 0, 0 );
		while ( regionIndex.z < newWorld.worldDimensions.z )
		{
			
			regionIndex.x = 0;
			while ( regionIndex.x < newWorld.worldDimensions.x )
			{
				
				newRegions [ regionIndex.x, regionIndex.z ] = CreateRegion ( newWorld, regionIndex );
				regionIndex.x += 1;
			}
			
			regionIndex.z += 1;
		}
		
		return newRegions;
	}
	
	
	private Region CreateRegion ( World newWorld, Int2D regionIndex )
	{
		
		Region newRegion = new Region ();
		newRegion.world = newWorld;
		newRegion.position = new Int2D ( regionIndex.x, regionIndex.z );
		newRegion.tiles = new Tile[newWorld.regionDimensions.x, newWorld.regionDimensions.z];
		newRegion.map = new Texture2D ( newRegion.world.tileSize * newRegion.world.regionDimensions.x, newRegion.world.tileSize * newRegion.world.regionDimensions.z );
		
		newRegion.tiles = CreateAllTiles ( newRegion );
		
		return newRegion;
	}
	
	
	private Tile[,] CreateAllTiles ( Region newRegion )
	{
		
		Tile[,] newTiles = new Tile[newRegion.world.regionDimensions.x, newRegion.world.regionDimensions.z];
		
		Int2D tileIndex = new Int2D ( 0, 0 );
		while ( tileIndex.z < newRegion.world.regionDimensions.z )
		{
			
			tileIndex.x = 0;
			while ( tileIndex.x < newRegion.world.regionDimensions.x )
			{
				
				newTiles[tileIndex.x, tileIndex.z] = CreateTile ( newRegion, tileIndex );
				tileIndex.x += 1;
			}
			
			tileIndex.z += 1;
		}
		
		return newTiles;
	}
	
	
	private Tile CreateTile ( Region newRegion, Int2D tileIndex )
	{
		
		Tile tile = new Tile ();
		tile.name = "Tile " + tileIndex.x + ", " + tileIndex.z;
		
		tile.region = newRegion;
		tile.position = new Int2D ( tileIndex.x, tileIndex.z );
		
		Color[,] tempColourArray = new Color[ newRegion.world.tileSize, newRegion.world.tileSize ];
		
		Int2D colourIndex = new Int2D ( 0, 0 );
		while ( colourIndex.z < tempColourArray.GetLength ( 1 ))
		{
	
			colourIndex.x = 0;
			while ( colourIndex.x < tempColourArray.GetLength ( 0 ))
			{
				
				tempColourArray[colourIndex.x, colourIndex.z] = Color.red;
				colourIndex.x += 1;
			}
			
			colourIndex.z += 1;
		}
		
		tile.pixels = tempColourArray;
		tile.environment = defaultEnvironment;
		return tile;
	}
	
	
	private float GetTilePerlin ( Tile newTile )
	{
		
		int xOffset = ( newTile.region.position.x * newTile.region.world.regionDimensions.x );
		int yOffset = ( newTile.region.position.z * newTile.region.world.regionDimensions.z );
		
		float newPerlinValue = newTile.region.world.worldPerlin[xOffset + newTile.position.x, yOffset + newTile.position.z];
		
		return newPerlinValue;
	}
	
	
	private float[,] GeneratePerlinNoise ( Vector2 seed, Int2D perlinSize )
	{
		
		float[,] pixels = new float [ perlinSize.x, perlinSize.z ];
		
		float scale = 2.2f;

		Vector2 pos = new Vector2 ( 0, 0 );
		while ( pos.y < perlinSize.z )
		{
			
			pos.x = 0;
			while ( pos.x < perlinSize.x )
			{
				
		    	float xCoordinate = seed.x + pos.x / perlinSize.x * scale;
		    	float yCoordinate = seed.y + pos.y / perlinSize.z * scale;
		    	float perlin = Mathf.PerlinNoise ( xCoordinate, yCoordinate );
				
				pixels [Convert.ToInt32 ( pos.x ), Convert.ToInt32 ( pos.y )] = perlin;
				
				pos.x += 1;
			}
			
			pos.y += 1;
		}
		
		return pixels;
	}
}