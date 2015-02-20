using System;
using System.IO;
using System.Xml;
using UnityEngine;
using System.Linq;
using System.Reflection;
using System.Collections;
using System.Xml.Serialization;
using System.Collections.Generic;
//using System.Collections.Specialized;
using System.Text.RegularExpressions;
//Written by Michael Bethke
[XmlRoot ( "Story" )]
public class Story
{
	
	[XmlElement ( "Introduction" )]
	public Introduction introduction;
	
	[XmlElement ( "CharacterCreate" )]
	public CharacterCreation characterCreation;
}


public class Introduction
{
	
	[XmlElement ( "Scene" )]
	public List<Scene> scenes = new List<Scene> ();
}


public class CharacterCreation
{
	
	[XmlElement ( "Scene" )]
	public List<Scene> scenes = new List<Scene> ();
}


public class Scene
{
	
	[XmlAttribute ( "id" )]
	public int id;
	
	[XmlElement ( "text" )]
	public string rawText;
	
	[XmlIgnore]
	public string richText;
	
	[XmlElement ( "Interaction" )]
	public List<Interaction> interactions = new List<Interaction> ();
}


public class Interaction
{
	
	public string interactionType;
	public string interactionMessage;
	
	public string interactionDestination;
	public int interactionEndNextGUI;
	
	[XmlElement ( "InteractionAffect" )]
	public InteractionAffect interactionAffect = new InteractionAffect ();
}


public class InteractionAffect
{
	
	[XmlElement ( "affectProperty" )]
	public string affectProperty;
	
	[XmlElement ( "affectValue" )]
	public int affectValue = -1;
}


[XmlRoot ( "Replacers" )]
public class Replacers
{
	
	[XmlElement ( "Property" )]
	public List<Property> properties = new List<Property> ();
	
	[XmlIgnore]
	public static Replacers replacers;
}


public class Property : IEquatable <Property>
{
	
	[XmlAttribute ( "id" )]
	public string id;
	
	[XmlElement ( "Value" )]
	public List<Value> propertyValue = new List<Value> ();
	
	
	public override String ToString()
	{
		
		return id;
	}

	public bool Equals ( Property other )
	{
		
		if ( other is Property == false )
		{
			
			return false;
		}
		
		return this.id == other.id && this.propertyValue == other.propertyValue;
	}
}


public class Value
{
	
	[XmlAttribute ( "id" )]
	public int id;
	
	[XmlElement ( "Adjective" )]
	public List<String> adjectives = new List<String> ();
}


public static class GetVariable
{
	
	public static string FromPlayer ( string originalString )
	{
		
		try
		{
			
			/*MatchCollection allOccurrences = Regex.Matches ( originalString, "{(.*?)}" );
			if ( allOccurrences.Count > 0 )
			{
				
				foreach ( Match match in allOccurrences )
				{
				
					UnityEngine.Debug.Log ( match.ToString ().Substring ( 1, ( match.Length - 1 ) - 1 ));
				}
			}*/
		
			//return Regex.Replace ( originalString, "{(.*?)}", m => Player.player.GetType ().GetProperty ( m.Groups[1].Value ).GetValue ( Player.player, null ).ToString ());
			
			//foos.Where(f => f.Contains("b")).Select(f => Regex.Replace(f, "b", "d"));
			//return Replacers.replacers.Where ( d => d.Contains ( m => "{(.*?)}" ))
			
			return Regex.Replace ( originalString, "{(.*?)}", d => Replacers.replacers.properties.Where ( p => p.id == d.Groups[1].Value ).FirstOrDefault ());
			//return Regex.Replace ( originalString, "{(.*?)}", m => Replacers.replacers.properties.FindIndex ( m.Groups[1].Value ) );
		} catch ( Exception e )
		{
			
			UnityEngine.Debug.LogError ( e );
			return "ERROR: Unable to find '" + Regex.Match ( originalString, "{(.*?)}" ) + "'";
		}
	}
}


public static class SetVariable
{
	
	public static bool IntToPlayer ( string propertyName, int newValue )
	{
		
		PropertyInfo propertyInfo = Player.player.GetType ().GetProperty ( propertyName );
		if ( propertyInfo != null )
		{
			
			propertyInfo.SetValue ( Player.player, newValue, null );
		}
		
		return true;
	}
	
	public static bool StringToPlayer ( string propertyName, string newValue )
	{
		
		PropertyInfo propertyInfo = Player.player.GetType ().GetProperty ( propertyName );
		if ( propertyInfo != null )
		{
			
			propertyInfo.SetValue ( Player.player, newValue, null );
		}
		
		return true;
	}
}


public class GameManager : MonoBehaviour
{
	
	internal Story story = new Story ();
	private Replacers tempReplacers = new Replacers ();
	//internal Replacers replacers = new Replacers ();

	private WorldManager worldManager;
	private UserInterface userInterface;
	
	
	private void Start ()
	{
		
		worldManager = GameObject.FindGameObjectWithTag ( "WorldManager" ).GetComponent<WorldManager> ();
		userInterface = GameObject.FindGameObjectWithTag ( "UserInterface" ).GetComponent<UserInterface> ();
	}
	
	
	internal void NewGame ( bool skipIntro = false )
	{
		
		if ( skipIntro == true )
		{
			
			userInterface.currentGUI = 4;
			StartGame ( 10, 10, 8, 8 );
			return;
		}
		
		string storyXML = Read.XMLFile ( "/Users/michaelbethke/Desktop/Default/Story.xml" );
		story = storyXML.DeserializeXml<Story> ();
		
		string replacersXML = Read.XMLFile ( System.Environment.GetFolderPath ( System.Environment.SpecialFolder.Desktop ) + Path.DirectorySeparatorChar + "Default" + Path.DirectorySeparatorChar + "Replacers.xml" );
		tempReplacers = replacersXML.DeserializeXml<Replacers> ();
		Replacers.replacers = tempReplacers;
		
		/*foreach ( Property newProperty in replacers.properties )
		{
			
			UnityEngine.Debug.Log ( newProperty.id );
			
			//Property newProperty = replacers.properties.Find ( x => ( x.id == "PhysicalGender" ));
			if ( newProperty != null )
			{
				
				foreach ( Value newValue in newProperty.propertyValue )
				{
					
					UnityEngine.Debug.Log ( "\t" + newValue.id );
					foreach ( String newAdjective in newValue.adjectives )
					{
						
						UnityEngine.Debug.Log ( "\t\t" + newAdjective );
					}
				}
			}
		}*/
		
		
		userInterface.StartIntro ();
	}


	internal void StartGame ( int worldWidth, int worldHeight, int regionWidth, int regionHeight )
	{
	
		worldManager.CreateNewWorld ( worldWidth, worldHeight, regionWidth, regionHeight );
	}
}
