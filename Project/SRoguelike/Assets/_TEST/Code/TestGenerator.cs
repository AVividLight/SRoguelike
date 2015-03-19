using System;
using System.IO;
using System.Linq;
using UnityEngine;
using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
//Written by Michael Bethke
public class TestGenerator : MonoBehaviour
{
	
	World tempWorld;
	
	private int loadingStage = -1;
	
	private Material selfIllumDiffuse;
	private Queue<Tile> tileQueue = new Queue<Tile> ();
	
	
	private void Start ()
	{
		
		selfIllumDiffuse = new Material ( Shader.Find ( "Self-Illumin/Diffuse" )); 
		
		tempWorld = GenerateWorld ( 0, 0, 8, 6, 8, 8 );
		
		
		Tile t = FindNearest ( tempWorld.regions[2, 2].tiles[4, 3], 2, "Water" );
		InvokeRepeating ( "SlowUpdate", 1, 1 );
	}
	
	
	private void SlowUpdate ()
	{
		
		if ( loadingStage != -1 )
		{
			
			if ( tileQueue.Count > 0 )
			{
				
				tileQueue.Dequeue ().tileObject.renderer.material.color = Color.red;
			}
		}
	}
	
	
	private void RepopulateTileQueue ()
	{
		
		int regionYIndex = 0;
		while ( regionYIndex < tempWorld.worldDimensions.z )
		{
	
			int regionXIndex = 0;
			while ( regionXIndex < tempWorld.worldDimensions.x )
			{
		
	   			int tileYIndex = 0;
	   			while ( tileYIndex < tempWorld.regionDimensions.z )
	   			{
				
	   				int tileXIndex = 0;
	   				while ( tileXIndex < tempWorld.regionDimensions.x )
	   				{
					
	   					tileQueue.Enqueue ( tempWorld.regions[regionXIndex, regionYIndex].tiles[tileXIndex, tileYIndex] );
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
		newTile.transform.localPosition = new Vector3 (( tile.region.position.x * tile.region.world.regionDimensions.x ) + tile.position.x, 1, ( tile.region.position.z * tile.region.world.regionDimensions.z ) + tile.position.z );
		newTile.transform.parent = tile.region.regionObject.transform;
	
		newTile.renderer.material = selfIllumDiffuse;
		newTile.renderer.material.color = Color.white;
		
		tile.tileObject = newTile;
		
		return 0;
	}
	
	
	private Tile FindNearest ( Tile tile, int range, String environmentName )
	{
		
		Tile nearestTile = null;
		List<Tile> relevantTiles = new List<Tile> ();
		List<Int2D> visitedTiles = new List<Int2D> ();
		
		Queue<Tile> queue = new Queue<Tile> ();
		queue.Enqueue ( tile );
		
		while ( queue.Count > 0 )
		{
			
			Tile current = queue.Dequeue ();
			if ( current == null || visitedTiles.Contains ( current.position ))
			{
				
				continue;
			}
			
			if ( current.position.z > tile.position.z + range )
			{

				break;
			}
			
			visitedTiles.Add ( current.position );

			queue.Enqueue ( current.Top );
			queue.Enqueue ( current.Left );
			queue.Enqueue ( current.Right );
			queue.Enqueue ( current.Bottom );
			
			tileQueue.Enqueue ( current );
		}
		
		return nearestTile;
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
		
		tile.region.tiles [tileXIndex, tileYIndex] = tile;
		
		CreateTileObject ( tile );
		//tileQueue.Enqueue ( tile );
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
		
		float scale = 3.0f;

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
		
		return pixels;
	}
}