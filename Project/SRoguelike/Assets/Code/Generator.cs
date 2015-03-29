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
	
	private Material selfIllumDiffuse;
	private Environment defaultEnvironment = new Environment ();

	private Queue<Tile> tileQueue = new Queue<Tile> ();
	
	
	private void Start ()
	{
		
		worldManager = GameObject.FindGameObjectWithTag ( "WorldManager" ).GetComponent<WorldManager> ();
		selfIllumDiffuse = new Material ( Shader.Find ( "Self-Illumin/Diffuse" )); 
	}
	
	
	private void Update ()
	{
		
		if ( loadingStage != -1 )
		{
			
			switch ( loadingStage )
			{
				
				case 0:
				int newTilesPerFrame = 0;
				while ( newTilesPerFrame < ( worldManager.world.regionDimensions.x * worldManager.world.regionDimensions.z ) * 1 )
				{
			
					if ( tileQueue.Count > 0 )
					{

						loadingInformation = "Creating " + ( tileQueue.Count () - 1 ) + " tiles.";
							
						/*if ( CreateTileObject ( tileQueue.Peek ()) != 0 )
						{
							
							UnityEngine.Debug.Log ( "Unable to create tile object, " + tileQueue.Peek ().name );
						}*/
						
						tileQueue.Dequeue ();
					} else {
							
						RepopulateTileQueue ();
						loadingStage = 1;
						break;
					}
					
					newTilesPerFrame += 1;
				}
				break;
				
				case 1:
				int tileWaterPerFrame = 0;
				while ( tileWaterPerFrame < worldManager.world.regionDimensions.x * worldManager.world.regionDimensions.z )
				{
			
					if ( tileQueue.Count () > 0 )
					{
						
						loadingInformation = "Analysing " + ( tileQueue.Count () - 1 ) + " tiles for water.";
						
						if ( CreateWater ( tileQueue.Peek ()) != 0 )
						{
							
							UnityEngine.Debug.Log ( "Unable to create environment, " + tileQueue.Peek ().name );
						}
						
						tileQueue.Dequeue ();
					} else {
							
						RepopulateTileQueue ();
						loadingStage = 2;
						break;
					}
					
					tileWaterPerFrame += 1;
				}
				break;
				
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
				break;
			}
		}
	}
	
	
	private void RepopulateTileQueue ()
	{
		
		tileQueue = new Queue<Tile> ();
		
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
					
	   					tileQueue.Enqueue ( worldManager.world.regions[regionXIndex, regionYIndex].tiles[tileXIndex, tileYIndex] );
	   					tileXIndex += 1;
	   				}
	   				tileYIndex += 1;
	   			}
				regionXIndex += 1;
			}
			regionYIndex += 1;
		}
	}
	
	
	/*private int CreateTileObject ( Tile tile )
	{
		
		GameObject newTile = GameObject.CreatePrimitive ( PrimitiveType.Plane );
		
		newTile.name = tile.name;
		newTile.collider.enabled = false;
	
		newTile.transform.localScale = new Vector3 ( 0.1f, 1, 0.1f );
		newTile.transform.localPosition = new Vector3 (( tile.region.position.x * tile.region.world.regionDimensions.x ) + tile.position.x, 1, ( tile.region.position.z * tile.region.world.regionDimensions.z ) + tile.position.z );
		newTile.transform.parent = tile.region.regionObject.transform;
	
		newTile.renderer.material = selfIllumDiffuse;
		newTile.renderer.material.color = Color.white;
		
		tile.tileObject = newTile;
		
		return 0;
	}*/
	
	
	private int CreateWater ( Tile tile )
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
	}
	
	
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
	
	
	private void SetTileEnvironment ( Tile tile, Environment environment )
	{
		
		tile.environment = environment;
		//tile.tileObject.renderer.material.color = GetTileColour ( tile );
	}
	
	
	public World GenerateWorld ( float xSeed, float ySeed, int worldWidth, int worldHeight, int regionWidth, int regionHeight )
	{
			
		World newWorld = new World ( worldWidth, worldHeight, regionWidth, regionHeight );
		
		GameObject newWorldObject = new GameObject ();
		newWorldObject.transform.parent = gameObject.transform;
		newWorldObject.name = "NewWorld";
		newWorld.worldObject = newWorldObject;
		
		newWorld.worldPerlin = GeneratePerlinNoise ( xSeed, ySeed, regionWidth * worldWidth, regionHeight * worldHeight );
		newWorld.environments = GetEnvironments ();
		
		newWorld.regions = CreateRegions ( newWorld );
		
		loadingStage = 0;
		return newWorld;
	}
	
	
	private Environments GetEnvironments ()
	{
		
		//Catch errors
		Environments environments = JsonConvert.DeserializeObject<Environments>( File.ReadAllText ( System.Environment.GetFolderPath ( System.Environment.SpecialFolder.Desktop ) + Path.DirectorySeparatorChar + "Default" + Path.DirectorySeparatorChar + "Environments.json" ));
		
		return environments;
	}
	
	
	private Region[,] CreateRegions ( World newWorld )
	{
		
		Region[,] newRegions = new Region[newWorld.worldDimensions.x, newWorld.worldDimensions.z];
		
		int regionYIndex = 0;
		while ( regionYIndex < newWorld.worldDimensions.z )
		{
			
			int regionXIndex = 0;
			while ( regionXIndex < newWorld.worldDimensions.x )
			{
				
				newRegions [ regionXIndex, regionYIndex ] = GenerateRegion ( newWorld, regionXIndex, regionYIndex );
				regionXIndex += 1;
			}
			
			regionYIndex += 1;
		}
		
		return newRegions;
	}
	
	
	private Region GenerateRegion ( World newWorld, int regionXIndex, int regionYIndex )
	{
		
		Region newRegion = new Region ();
		newRegion.world = newWorld;
		newRegion.position = new Int2D ( regionXIndex, regionYIndex );
		
		GameObject newRegionObject = GameObject.CreatePrimitive ( PrimitiveType.Plane );
		
		newRegionObject.name = "Region" + regionXIndex + "," + regionYIndex;
		newRegionObject.collider.enabled = false;
		
		newRegionObject.transform.localScale = new Vector3 ( newWorld.regionDimensions.x * 0.1f, 1, newWorld.regionDimensions.z * 0.1f );
		newRegionObject.transform.position = new Vector3 (( newWorld.regionDimensions.x * regionXIndex ) + newWorld.regionDimensions.x/2 - 0.5f /* 0.5f = Tile Width/2 */, 0, ( newWorld.regionDimensions.z * regionYIndex ) + newWorld.regionDimensions.z/2 - 0.5f /* 0.5f = Tile Height/2 */ );
		newRegionObject.transform.parent = newWorld.worldObject.transform;
		
		newRegion.regionObject = newRegionObject;
		newRegion.tiles = new Tile[newWorld.regionDimensions.x, newWorld.regionDimensions.z];
		
		//if ( regionXIndex == Player.player.position.x && regionYIndex == Player.player.position.z )
		//{
			
			newRegion.tiles = CreateTiles ( newRegion );
			newRegion.regionObject.GetComponent<MeshRenderer> ().enabled = false;
		//}
		
		return newRegion;
	}
	
	
	private Tile[,] CreateTiles ( Region newRegion )
	{
		
		Tile[,] newTiles = new Tile[newRegion.world.regionDimensions.x, newRegion.world.regionDimensions.z];
		
		int tileYIndex = 0;
		while ( tileYIndex < newRegion.world.regionDimensions.z )
		{
			
			int tileXIndex = 0;
			while ( tileXIndex < newRegion.world.regionDimensions.x )
			{
				
				newTiles[tileXIndex, tileYIndex] = GenerateTile ( newRegion, tileXIndex, tileYIndex );
				tileXIndex += 1;
			}
			
			tileYIndex += 1;
		}
		
		return newTiles;
	}
	
	
	private Tile GenerateTile ( Region newRegion, int tileXIndex, int tileYIndex )
	{
		
		Tile tile = new Tile ();
		tile.name = "Tile " + tileYIndex + ", " + tileXIndex;
		
		tile.region = newRegion;
		tile.position = new Int2D ( tileXIndex, tileYIndex );
		
		tile.environment = defaultEnvironment;
		
		tile.region.tiles [tileXIndex, tileYIndex] = tile;
		tileQueue.Enqueue ( tile );
		return tile;
	}
	
	
	private float GetTilePerlin ( Tile newTile )
	{
		
		int xOffset = ( newTile.region.position.x * newTile.region.world.regionDimensions.x );
		int yOffset = ( newTile.region.position.z * newTile.region.world.regionDimensions.z );
		
		float newPerlinValue = newTile.region.world.worldPerlin[xOffset + newTile.position.x, yOffset + newTile.position.z];
		
		return newPerlinValue;
	}
	
	
	private Color GetTileColour ( Tile tile )
	{
		
		float minorVariation = UnityEngine.Random.Range ( 0.000f, 0.020f ) - 0.010f;
		Color baseColour = new Color ( tile.environment.baseColourRed + minorVariation, tile.environment.baseColourGreen + minorVariation, tile.environment.baseColourBlue + minorVariation, 1 );
		
		return baseColour;
	}
	
	
	private float[,] GeneratePerlinNoise ( float xSeed, float ySeed, int perlinWidth, int perlinHeight )
	{
		
		float[,] pixels = new float [ perlinWidth, perlinHeight ];
		
		float scale = 2.2f;

		float y = 0;
		while ( y < perlinHeight )
		{
			
			float x = 0;
			while ( x < perlinWidth )
			{
				
		    	float xCoordinate = xSeed + x / perlinWidth * scale;
		    	float yCoordinate = ySeed + y / perlinHeight * scale;
		    	float perlin = Mathf.PerlinNoise ( xCoordinate, yCoordinate );
				
				pixels [Convert.ToInt32 ( x ), Convert.ToInt32 ( y )] = perlin;
				
				x += 1;
			}
			
			y += 1;
		}
		
		//Only works for squares
		bool createTextureObject = false;
		if ( createTextureObject == true )
			CreatePerlinObject ( pixels );
		
		return pixels;
	}
	
	
	private void CreatePerlinObject ( float[,] twoDPixels )
	{
		
		Color[] texturePixelArray = new Color[twoDPixels.GetLength ( 0 ) * twoDPixels.GetLength ( 1 )];
		
		int pixelY = 0;
		while ( pixelY < twoDPixels.GetLength ( 1 ))
		{
			
			int pixelX = 0;
			while ( pixelX < twoDPixels.GetLength ( 0 ))
			{
				
				texturePixelArray [pixelY * twoDPixels.GetLength ( 1 ) + pixelX] = new Color ( twoDPixels[pixelX, pixelY], twoDPixels[pixelX, pixelY], twoDPixels[pixelX, pixelY], 1 );
				pixelX += 1;
			}
				
			pixelY += 1;
		}	
		
		Texture2D noiseTexture = new Texture2D ( twoDPixels.GetLength ( 0 ), twoDPixels.GetLength ( 1 ) );
		noiseTexture.SetPixels ( texturePixelArray );
		noiseTexture.Apply ();
		
		GameObject perlinPlane = GameObject.CreatePrimitive ( PrimitiveType.Plane );
		perlinPlane.transform.position = new Vector3 ( 50, 0, 12.5f );
		perlinPlane.transform.localScale = new Vector3 ( noiseTexture.width * 0.1f, 1, noiseTexture.height * 0.1f );
		perlinPlane.transform.eulerAngles = new Vector3 ( 0, 180, 0 );
		perlinPlane.renderer.material = new Material ( Shader.Find ( "Self-Illumin/Diffuse" ));
		perlinPlane.renderer.material.mainTexture = noiseTexture;
		perlinPlane.name = "PerlinPlane";
	}
}