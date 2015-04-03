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
	
	public int interactionDestination;
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
	public List<Value> propertyValues = new List<Value> ();
	
	[XmlIgnore]
	public sbyte propertyIndex;
	
	
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
		
		return this.id == other.id && this.propertyValues == other.propertyValues;
	}
}


public class Value
{
	
	[XmlAttribute ( "id" )]
	public string id;
	
	[XmlElement ( "Adjective" )]
	public List<String> adjectives = new List<String> ();
	
	
	public int ToInt ()
	{
		
		sbyte valInt;
		if ( sbyte.TryParse ( id, out valInt ))
		{
			
			return valInt;
		}
		
		return -128;
	}
}


public static class GetVariable
{
	
	public static string FromPlayer ( string originalString )
	{
		
		try
		{
			
			return Regex.Replace ( originalString, "{(.*?)}", match =>
			{
				
				string formattedMatch = match.ToString ().Substring ( 1, match.ToString ().Length - 2 );
				var property = Player.player.properties.First ( p => p.id == formattedMatch );
				
				return property.propertyValues[(sbyte) property.propertyIndex].adjectives[UnityEngine.Random.Range ( 0, property.propertyValues[property.propertyIndex].adjectives.Count () - 0 )];
			});	
		} catch ( Exception e )
		{
			
			string errorString = "Unable to find all instances, ";
			
			foreach ( Match m in Regex.Matches ( originalString, "{(.*?)}" ))
			{
				
				errorString += m.Value + " (" + m.Index + ")";
			} 
			
			UnityEngine.Debug.LogError ( errorString + "\n" + e );
			
			return errorString;
		}
	}
}


public static class SetVariable
{
	
	public static bool ToPlayer ( string property, string val )
	{

		if ( String.IsNullOrEmpty ( property.Trim ()) == false && String.IsNullOrEmpty ( val.Trim ()) == false )
		{

			Property replacerProperty = Replacers.replacers.properties.SingleOrDefault ( prop => prop.id == property );
			if ( replacerProperty != null )
			{

				Property playerProperty = Player.player.properties.SingleOrDefault ( prop => prop.id == property );
				if ( playerProperty != null )
				{

					sbyte valInt;
					if ( sbyte.TryParse ( val, out valInt ))
					{

						playerProperty.propertyIndex = valInt;
					} else
					{

						playerProperty.propertyValues[0].adjectives[0] = val;
					}
				} else
				{

					sbyte valInt;
					if ( sbyte.TryParse ( val, out valInt ))
					{

						replacerProperty.propertyIndex = valInt;
					} else
					{

						replacerProperty.propertyIndex = 0;
						replacerProperty.propertyValues[0].adjectives[0] = val;
					}

					Player.player.properties.Add ( replacerProperty );
				}

				return true;
			}
		}
		
		return false;
	}
}


public class GameManager : MonoBehaviour
{
	
	internal Story story = new Story ();
	private Replacers tempReplacers = new Replacers ();

	private WorldManager worldManager;
	private UserInterface userInterface;
	
	
	private void Start ()
	{
		
		worldManager = GameObject.FindGameObjectWithTag ( "WorldManager" ).GetComponent<WorldManager> ();
		userInterface = GameObject.FindGameObjectWithTag ( "UserInterface" ).GetComponent<UserInterface> ();
	}
	
	
	internal void NewGame ( bool skipIntro = true )
	{
		
		if ( skipIntro == true )
		{
			
			userInterface.currentGUI = 4;
			StartGame ( new Int2D ( 8, 6 ), new Int2D ( 8, 8 ), 1 );
			return;
		}
		
		string tempFolderLocation = System.Environment.GetFolderPath ( System.Environment.SpecialFolder.Desktop ) + Path.DirectorySeparatorChar + "Default" + Path.DirectorySeparatorChar;
		
		string storyXML = Read.XMLFile ( tempFolderLocation + "Story.xml" );
		story = storyXML.DeserializeXml<Story> ();
		
		string replacersXML = Read.XMLFile ( tempFolderLocation + "Replacers.xml" );
		tempReplacers = replacersXML.DeserializeXml<Replacers> ();
		Replacers.replacers = tempReplacers;
		
		userInterface.StartIntro ();
	}


	internal void StartGame ( Int2D worldSize, Int2D regionSize, int desiredTileSize )
	{
	
		worldManager.CreateNewWorld ( worldSize, regionSize, desiredTileSize );
	}
}
