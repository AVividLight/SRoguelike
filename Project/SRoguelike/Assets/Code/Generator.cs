using System;
using System.IO;
using System.Linq;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
//Written by Michael Bethke
public class Generator : MonoBehaviour
{
	
	internal int currentLoadingPercentage = 0;
	internal int totalLoadingPercentage = 0;

	//private ExternalInformation externalInformation;
	
	private Material selfIllumDiffuse;
	
	
	private void Start ()
	{
		
		//externalInformation = GameObject.FindGameObjectWithTag ( "InformationManager" ).GetComponent<ExternalInformation> ();
		
		selfIllumDiffuse = new Material ( Shader.Find ( "Self-Illumin/Diffuse" )); 
	}
	
	
	public World GenerateWorld ( float xSeed, float ySeed, int worldWidth, int worldHeight, int regionWidth, int regionHeight )
	{
		
		currentLoadingPercentage = 0;
		totalLoadingPercentage = ( worldWidth * regionWidth ) * ( worldHeight * regionHeight );
			
		World newWorld = new World ( worldWidth, worldHeight, regionWidth, regionHeight );
		
		GameObject newWorldObject = new GameObject ();
		newWorldObject.transform.parent = gameObject.transform;
		newWorldObject.name = "NewWorld";
		newWorld.worldObject = newWorldObject;
		
		newWorld.worldPerlin = GeneratePerlinNoise ( xSeed, ySeed, worldWidth * regionWidth, worldHeight * regionHeight );
		newWorld.environments = GetEnvironments ();
		
		newWorld.regions = CreateRegions ( newWorld, worldWidth, worldHeight, regionWidth, regionHeight );
		
		return newWorld;
	}
	
	
	private Environments GetEnvironments ()
	{
		
		Environments environments = new Environments ();
		
		string environmentsXML = /*externalInformation.ReadXMLFile*/ Read.XMLFile ( "/Users/michaelbethke/Desktop/Default/Environments.xml" );
		
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
	
	
	private Region[,] CreateRegions ( World newWorld, int worldWidth, int worldHeight, int regionWidth, int regionHeight )
	{
		
		Region[,] newRegions = new Region[worldWidth, worldHeight];
		
		int regionIndex = 0;
		int regionYIndex = 0;
		while ( regionYIndex < worldHeight )
		{
			
			int regionXIndex = 0;
			while ( regionXIndex < worldWidth )
			{
				
				newRegions [ regionXIndex, regionYIndex ] = GenerateRegion ( newWorld, regionWidth, regionHeight, regionXIndex, regionYIndex, regionIndex );
				regionXIndex += 1;
				regionIndex += 1;
			}
			
			regionYIndex += 1;
		}
		
		return newRegions;
	}
	
	
	private Region GenerateRegion ( World newWorld, int regionWidth, int regionHeight, int regionXIndex, int regionYIndex, int regionIndex )
	{
		
		Region newRegion = new Region ();
		newRegion.world = newWorld;
		
		GameObject newRegionObject = GameObject.CreatePrimitive ( PrimitiveType.Plane );
		
		newRegionObject.name = "Region" + regionIndex;
		newRegionObject.collider.enabled = false;
		
		newRegionObject.transform.localScale = new Vector3 ( regionWidth * 0.1f, 1, regionHeight * 0.1f );
		newRegionObject.transform.position = new Vector3 (( regionWidth * regionXIndex ) + regionWidth/2 - 0.5f /*Tile Width*/, 0, ( regionHeight * regionYIndex ) + regionHeight/2 - 0.5f /*Tile Height*/ );
		newRegionObject.transform.parent = newWorld.worldObject.transform;
		
		newRegion.regionObject = newRegionObject;
		
		//if ( regionXIndex == 4 && regionYIndex == 3 )
		//{
			
			newRegion.tiles = CreateTiles ( newRegion, regionWidth, regionHeight, regionXIndex, regionYIndex );
			newRegion.regionObject.GetComponent<MeshRenderer> ().enabled = false;
		//}
		
		return newRegion;
	}
	
	
	private Tile[,] CreateTiles ( Region newRegion, int regionWidth, int regionHeight, int regionXIndex, int regionYIndex )
	{
		
		 Tile[,] newTiles = new Tile[regionWidth, regionHeight];
		 
		 int tileYIndex = 0;
		 while ( tileYIndex < regionHeight )
		 {
			 
			 int tileXIndex = 0;
			 while ( tileXIndex < regionWidth )
			 {
				 
				 newTiles[tileXIndex, tileYIndex] = GenerateTile ( newRegion, tileXIndex, tileYIndex, regionWidth, regionHeight, regionXIndex, regionYIndex );
				 tileXIndex += 1;
				 
				 currentLoadingPercentage += 1;
			 }
		 	
			 tileYIndex += 1;
		 }
		 
		 return newTiles;
	}
	
	
	private Tile GenerateTile ( Region newRegion, int tileXIndex, int tileYIndex, int regionWidth, int regionHeight, int regionXIndex, int regionYIndex )
	{
		
		Tile tile = new Tile ();
		
		tile.region = newRegion;
		tile.environment = GetTileEnvironment ( newRegion, tileXIndex, tileYIndex, regionWidth, regionHeight, regionXIndex, regionYIndex );
		
		GameObject newTileObject = GameObject.CreatePrimitive ( PrimitiveType.Plane );
		
		newTileObject.name = "Tile " + tileYIndex + ", " + tileXIndex;
		newTileObject.collider.enabled = false;
		
		newTileObject.transform.localScale = new Vector3 ( 0.1f, 1, 0.1f );
		newTileObject.transform.localPosition = new Vector3 (( regionXIndex * regionWidth ) + tileXIndex, 0, ( regionYIndex * regionHeight ) + tileYIndex );
		newTileObject.transform.parent = newRegion.regionObject.transform;
		
		newTileObject.renderer.material = selfIllumDiffuse;
		newTileObject.renderer.material.color = GetTileColour ( tile );
		
		tile.tileObject = newTileObject;
		
		return tile;
	}
	
	
	private Environment GetTileEnvironment ( Region newRegion, int tileXIndex, int tileYIndex, int regionWidth, int regionHeight, int regionXIndex, int regionYIndex )
	{

		float tilePerlin = GetTilePerlin ( newRegion, tileXIndex, tileYIndex, regionWidth, regionHeight, regionXIndex, regionYIndex ).grayscale;
		
		float higherElevation = 0;
		float lowerElevation = 0;
		
		Environment higherEnvironment = null;
		Environment lowerEnvironment = null;
		
		foreach ( KeyValuePair<float, Environment> keyValuePair in newRegion.world.environments.minimumEnvironmentConditions )
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
	
	
	private Color GetTilePerlin ( Region newRegion, int tileXIndex, int tileYIndex, int regionWidth, int regionHeight, int regionXIndex, int regionYIndex )
	{
		
		int xOffset = ( regionXIndex * regionWidth );
		int yOffset = ( regionYIndex * regionHeight );
		
		Color newPerlinValue = newRegion.world.worldPerlin[xOffset + tileXIndex, yOffset + tileYIndex];
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
		
		float scale = 2.0f;

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