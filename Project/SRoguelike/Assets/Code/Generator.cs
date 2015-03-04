using System;
using System.IO;
using System.Linq;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
//Written by Michael Bethke
/*public class TileObject
{
	
	public string name;
	public bool colliderEnabled;
	
	public Vector3 scale;
	public Vector3 position;
	
	public GameObject parent;
	
	public Material material;
	public Color colour;
}*/


public class Generator : MonoBehaviour
{
	
	private WorldManager worldManager;
	
	static internal string loadingInformation;
	//private int currentLoadingPercentage = 0;
	//private int totalLoadingPercentage = 0;
	private sbyte loadingStage = -1;
	
	private Material selfIllumDiffuse;

	//private List<TileObject> tileQueue = new List<TileObject> ();
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
				while ( newTilesPerFrame < worldManager.world.worldDimensions.x * worldManager.world.worldDimensions.z )
				{
			
					if ( tileQueue.Any () == true )
					{
						
						//loadingInformation = "Loading tile " + currentLoadingPercentage + " of " + totalLoadingPercentage + ".";
						loadingInformation = "Creating " + ( tileQueue.Count () - 1 ) + " tile objects.";
							
						GameObject newTile = GameObject.CreatePrimitive ( PrimitiveType.Plane );
						
						newTile.name = tileQueue[0].name;
						newTile.collider.enabled = false; //tileQueue[0].colliderEnabled;
					
						newTile.transform.localScale = new Vector3 ( 0.1f, 1, 0.1f ); //tileQueue[0].scale;
						newTile.transform.localPosition = new Vector3 ( tileQueue[0].position.x, 1, tileQueue[0].position.z ); //tileQueue[0].position;
						newTile.transform.parent = tileQueue[0].region.regionObject.transform;
					
						newTile.renderer.material = selfIllumDiffuse; //tileQueue[0].material;
						newTile.renderer.material.color = Color.white; //tileQueue[0].colour;
						
						tileQueue[0].tileObject = newTile;
						
						tileQueue.RemoveAt ( 0 );
						//currentLoadingPercentage += 1;
					} else {
							
						RebuildTileQueue ();
						loadingStage = 1;
						
						//currentLoadingPercentage = 0;
						//totalLoadingPercentage = 0;
						break;
					}
					
					newTilesPerFrame += 1;
				}
				break;
				
				case 1:
				int tileEnvironmentsPerFrame = 0;
				while ( tileEnvironmentsPerFrame < worldManager.world.worldDimensions.x * worldManager.world.worldDimensions.z )
				{
			
					if ( tileQueue.Any () == true )
					{
						
						loadingInformation = "Creating Environments for " + ( tileQueue.Count () - 1 ) + " tiles.";
						
						tileQueue[0].environment = GetTileEnvironment ( tileQueue[0] );
						tileQueue[0].tileObject.renderer.material.color = GetTileColour ( tileQueue[0] );
						
						tileQueue.RemoveAt ( 0 );
					}
					
					tileEnvironmentsPerFrame += 1;
				}
				
				/*int regionEnvironmentsPerFrame = 0;
				int tileEnvironmentsPerFrame = 0;
				
				int regionYIndex = 0;
				while ( regionYIndex < worldManager.world.worldDimensions.z )
				{
			
					int regionXIndex = 0;
					while ( regionXIndex < worldManager.world.worldDimensions.x )
					{
				
						loadingInformation = "Finding environment for " + ( worldManager.world.worldDimensions.x * worldManager.world.worldDimensions.z ) + " tiles.";
						regionXIndex += 1;
					}
			
					regionYIndex += 1;
				}
				
				regionEnvironmentsPerFrame += 1;
				
				if ( regionEnvironmentsPerFrame < worldManager.world.worldDimensions.x * worldManager.world.worldDimensions.z )
				{
					
					loadingStage = -1;
					//loadingInformation = "";
				}*/
				break;
				
				default:
				break;
			}
		}
	}
	
	
	private void RebuildTileQueue ()
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
		
		//currentLoadingPercentage = 0;
		//totalLoadingPercentage = 0;
	}
	
	
	public World GenerateWorld ( float xSeed, float ySeed, int worldWidth, int worldHeight, int regionWidth, int regionHeight )
	{
		
		//currentLoadingPercentage = 0;
		//totalLoadingPercentage = ( worldWidth * regionWidth ) * ( worldHeight * regionHeight );
			
		World newWorld = new World ( worldWidth, worldHeight, regionWidth, regionHeight );
		
		GameObject newWorldObject = new GameObject ();
		newWorldObject.transform.parent = gameObject.transform;
		newWorldObject.name = "NewWorld";
		newWorld.worldObject = newWorldObject;
		
		newWorld.worldPerlin = GeneratePerlinNoise ( xSeed, ySeed, worldWidth * regionWidth, worldHeight * regionHeight );
		newWorld.environments = GetEnvironments ();
		
		newWorld.regions = CreateRegions ( newWorld );
		
		loadingStage = 0;
		
		return newWorld;
	}
	
	
	private Environments GetEnvironments ()
	{
		
		Environments environments = new Environments ();
		
		string environmentsXML = Read.XMLFile ( "/Users/michaelbethke/Desktop/Default/Environments.xml" ); //Environment.GetFolderPath ( Environment.SpecialFolder.Desktop ) + Path.DirectorySeparatorChar + "Default" + Path.DirectorySeparatorChar + "Environments.xml"
		
		environments = environmentsXML.DeserializeXml<Environments> ();
		environments.minimumEnvironmentConditions = SetEnvironmentConditions ( environments );
		
		int environmentIndex = 0;
		while ( environmentIndex < environments.allEnvironments.Count ())
		{
			
			//Catch errors in parsing
			environments.allEnvironments[environmentIndex].baseColour = new Color ( Convert.ToSingle ( environments.allEnvironments[environmentIndex].baseColourRed ), Convert.ToSingle ( environments.allEnvironments[environmentIndex].baseColourGreen ), Convert.ToSingle ( environments.allEnvironments[environmentIndex].baseColourBlue ), 1 );
			environmentIndex += 1;
		}
		
		return environments;
	}
	
	
	private SortedDictionary <float, Environment> SetEnvironmentConditions ( Environments allEnvironments )
	{
		
		SortedDictionary <float, Environment> minimumEnvironmentConditions = new SortedDictionary <float, Environment> ();
		
		foreach ( Environment environment in allEnvironments.allEnvironments )
		{
			
			float minimumElevation = 0;
			try
			{
				
				minimumElevation = Convert.ToSingle ( environment.minElevation );
			} catch ( Exception error )
			{
				
				UnityEngine.Debug.Log ( error );
			}
			
			minimumEnvironmentConditions.Add ( minimumElevation, environment );
		}
		
		return minimumEnvironmentConditions;
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
		
		newRegionObject.name = "Region" + regionYIndex + "," + regionXIndex;
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
				
				//currentLoadingPercentage += 1;
			}
			
			tileYIndex += 1;
		}
		
		return newTiles;
	}
	
	
	private Tile GenerateTile ( Region newRegion, int tileXIndex, int tileYIndex )
	{
		
		Tile tile = new Tile ();
		
		tile.region = newRegion;
		tile.position = new Int2D ( tileXIndex, tileYIndex );
		//tile.environment = GetTileEnvironment ( newRegion, tileXIndex, tileYIndex, regionWidth, regionHeight, regionXIndex, regionYIndex );
		
		//Tile newTileObject = new Tile ();
		tile.name = "Tile " + tileYIndex + ", " + tileXIndex;
		//newTileObject.colliderEnabled = false;
		
		//newTileObject.scale = new Vector3 ( 0.1f, 1, 0.1f );
		tile.position = new Int2D (( newRegion.position.x * newRegion.world.regionDimensions.x ) + tileXIndex, ( newRegion.position.z * newRegion.world.regionDimensions.z ) + tileYIndex );
		//newTileObject.parent = newRegion.regionObject;
		
		//newTileObject.material = selfIllumDiffuse;
		//newTileObject.colour = Color.white; //GetTileColour ( tile );
		
		tileQueue.Add ( tile );
		//totalLoadingPercentage += 1;
		
		return tile;
	}
	
	
	private Environment GetTileEnvironment ( Tile newTile )
	{

		float tilePerlin = GetTilePerlin ( newTile ).grayscale;
		
		float higherElevation = 0;
		float lowerElevation = 0;
		
		Environment higherEnvironment = null;
		Environment lowerEnvironment = null;
		
		foreach ( KeyValuePair<float, Environment> keyValuePair in newTile.region.world.environments.minimumEnvironmentConditions )
		{
			
			float currentElevation = keyValuePair.Key;
			
			if ( currentElevation > tilePerlin )
			{

				higherElevation = currentElevation;
				higherEnvironment = keyValuePair.Value;
				break;
			} else
			{
				
				lowerElevation = currentElevation;
				lowerEnvironment = keyValuePair.Value;
			}
		}
		
		if ( Mathf.Abs ( higherElevation - tilePerlin ) > Mathf.Abs ( tilePerlin - lowerElevation ))
		{
			
			return lowerEnvironment;
		} else
		{
			
			return higherEnvironment;
		}
	}
	
	
	private Color GetTilePerlin ( Tile newTile )
	{
		
		int xOffset = ( newTile.region.position.x * newTile.region.world.regionDimensions.x );
		int yOffset = ( newTile.region.position.z * newTile.region.world.regionDimensions.z );
		
		UnityEngine.Debug.Log (( xOffset + newTile.position.x ) + " " + ( yOffset + newTile.position.z ));
		
		Color newPerlinValue = newTile.region.world.worldPerlin[xOffset + newTile.position.x, yOffset + newTile.position.z];
		Color newColour = new Color ( newPerlinValue.grayscale, newPerlinValue.grayscale, newPerlinValue.grayscale, 1 );
		
		return newColour;
	}
	
	
	private Color GetTileColour ( Tile tile )
	{
		
		float minorVariation = UnityEngine.Random.Range ( 0.000f, 0.020f ) - 0.010f;
		
		Color baseColour = new Color ( tile.environment.baseColour.r + minorVariation, tile.environment.baseColour.g + minorVariation, tile.environment.baseColour.b + minorVariation, 1 );
		
		return baseColour;
	}
	
	
	private Color[,] GeneratePerlinNoise ( float xSeed, float ySeed, int perlinWidth, int perlinHeight )
	{
		
		Color[,] pixels = new Color [ perlinWidth, perlinHeight ];
		
		float scale = 3.0f;

		float y = 0;
		while ( y < perlinHeight )
		{
			
			float x = 0;
			while ( x < perlinWidth )
			{
				
		    	float xCoordinate = xSeed + x / perlinWidth * scale;
		    	float yCoordinate = ySeed + y / perlinHeight * scale;
		    	float sample = Mathf.PerlinNoise ( xCoordinate, yCoordinate );
				
				pixels [Convert.ToInt32 ( x ), Convert.ToInt32 ( y )] = new Color ( sample, sample, sample, 1 );
				
				x += 1;
			}
			
			y += 1;
		}
		
		//Currently only works for squares
		bool createTextureObject = false;
		if ( createTextureObject == true )
			CreatePerlinObject ( pixels );
		
		return pixels;
	}
	
	
	private void CreatePerlinObject ( Color[,] twoDPixels )
	{
		
		Color[] texturePixelArray = new Color[twoDPixels.GetLength ( 0 ) * twoDPixels.GetLength ( 1 )];
		
		int pixelY = 0;
		while ( pixelY < twoDPixels.GetLength ( 1 ))
		{
			
			int pixelX = 0;
			while ( pixelX < twoDPixels.GetLength ( 0 ))
			{
				
				texturePixelArray [pixelY * twoDPixels.GetLength ( 1 ) + pixelX] = twoDPixels[pixelX, pixelY];
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