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

	private List<Tile> tileQueue = new List<Tile> ();
	
	
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
			
					if ( tileQueue.Any () == true )
					{

						loadingInformation = "Creating " + ( tileQueue.Count () - 1 ) + " tile objects.";
							
						if ( CreateTileObject ( tileQueue[0] ) != 0 )
						{
							
							UnityEngine.Debug.Log ( "Unable to create tile object, " + tileQueue[0].name );
						}
						
						tileQueue.RemoveAt ( 0 );
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
			
					if ( tileQueue.Any () == true )
					{
						
						loadingInformation = "Looking for water on " + ( tileQueue.Count () - 1 ) + " tiles.";
						
						if ( CreateWater ( tileQueue[0] ) != 0 )
						{
							
							UnityEngine.Debug.Log ( "Unable to create environment, " + tileQueue[0].name );
						}
						
						tileQueue.RemoveAt ( 0 );
					} else {
							
						RepopulateTileQueue ();
						loadingStage = 2;
						break;
					}
					
					tileWaterPerFrame += 1;
				}
				break;
				
				case 2:
				int tileShoresPerFrame = 0;
				while ( tileShoresPerFrame < worldManager.world.regionDimensions.x * worldManager.world.regionDimensions.z )
				{
			
					if ( tileQueue.Any () == true )
					{
						
						loadingInformation = "Looking for shore on " + ( tileQueue.Count () - 1 ) + " tiles.";
						
						if ( CreateShore ( tileQueue[0] ) != 0 )
						{
							
							UnityEngine.Debug.Log ( "Unable to create shore, " + tileQueue[0].name );
						}
						
						tileQueue.RemoveAt ( 0 );
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
					
	   					tileQueue.Add ( worldManager.world.regions[regionXIndex, regionYIndex].tiles[tileXIndex, tileYIndex] );
	   					tileXIndex += 1;
	   				}
	   				tileYIndex += 1;
	   			}
				regionXIndex += 1;
			}
			regionYIndex += 1;
		}
	}
	
	
	private int CreateTileObject ( Tile tile )
	{
		
		GameObject newTile = GameObject.CreatePrimitive ( PrimitiveType.Plane );
		
		newTile.name = tile.name;
		newTile.collider.enabled = false;
	
		newTile.transform.localScale = new Vector3 ( 0.1f, 1, 0.1f );
		newTile.transform.localPosition = new Vector3 ( tile.position.x, 1, tile.position.z );
		newTile.transform.parent = tile.region.regionObject.transform;
	
		newTile.renderer.material = selfIllumDiffuse;
		newTile.renderer.material.color = Color.white;
		
		tile.tileObject = newTile;
		
		return 0;
	}
	
	
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
	
	
	private int CreateShore ( Tile tile )
	{

		Environment shore;
		if ( tile.region.world.environments.environmentList.TryGetValue ( "Shore", out shore ))
		{
		
			//UnityEngine.Debug.Log ( tile.position.AsString ());
			if ( tile.position.x > 0 )
			{
				
				UnityEngine.Debug.Log ( worldManager.world.tiles [tile.position.x, tile.position.z].name );
				/*if ( tile.region.world.tiles [tile.position.x - 1, tile.position.z].name == "Water" )
				{
					
					//tile.region.world.tiles [tile.position.x - 1, tile.position.z].tileObject.renderer.material.color = Color.yellow;
				}*/
			}
				
			if ( tile.position.z > 0 )
			{}
			
			if ( tile.position.x < tile.region.world.regionDimensions.x )
			{}
				
			if ( tile.position.z < tile.region.world.regionDimensions.z )
			{}

			
			return 0;
		}
		
		UnityEngine.Debug.Log ( "Unable to find environment 'Water'!" );
		return 1;
	}
	
	
	private void SetTileEnvironment ( Tile tile, Environment environment )
	{
		
		tile.environment = environment;
		tile.tileObject.renderer.material.color = GetTileColour ( tile );
		//tile.tileObject.renderer.material.color = Color.green;
	}
	
	
	public World GenerateWorld ( float xSeed, float ySeed, int worldWidth, int worldHeight, int regionWidth, int regionHeight )
	{
			
		World newWorld = new World ( worldWidth, worldHeight, regionWidth, regionHeight );
		newWorld.tiles = new Tile[worldWidth * regionWidth, worldHeight * regionHeight];
		
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
		tile.position = new Int2D (( newRegion.position.x * newRegion.world.regionDimensions.x ) + tileXIndex, ( newRegion.position.z * newRegion.world.regionDimensions.z ) + tileYIndex );
		
		newRegion.world.tiles[tileXIndex, tileYIndex] = tile;
		tileQueue.Add ( tile );
		return tile;
	}
	
	
	private float GetTilePerlin ( Tile newTile )
	{
		
		//int xOffset = ( newTile.region.position.x * newTile.region.world.regionDimensions.x );
		//int yOffset = ( newTile.region.position.z * newTile.region.world.regionDimensions.z );
		
		float newPerlinValue = newTile.region.world.worldPerlin[/*xOffset +*/ newTile.position.x, /*yOffset +*/ newTile.position.z];
		
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
		
		float scale = 1.0f;

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