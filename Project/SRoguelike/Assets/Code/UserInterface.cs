using System;
using UnityEngine;
using System.Collections;
using System.Text.RegularExpressions;
//Written by Michael Bethke
public class UserInterface : MonoBehaviour
{
	
	private GameManager gameManager;
	
	public GUISkin defaultSkin;
	
	public Font classicFont;
	public Font modernFont;
	public Font[] playerWrittingFont;
	
	public Texture2D[] blueButtons = new Texture2D[6];
	public Texture2D[] greyButtons = new Texture2D[3];
	public Texture2D[] purpleButtons = new Texture2D[3];
	public Texture2D[] redButtons = new Texture2D[3];
	
	private GUIStyle inactiveButton;
	private GUIStyle normalButton;
	private GUIStyle violentButton;
	private GUIStyle sensualButton;
	
	private GUIStyle normalTextfield;
	
	private GUIStyle labelCenterLargeStyle;
	private GUIStyle labelCenterMediumStyle;
	
	private GUIStyle labelLeftLargeStyle;
	private GUIStyle labelLeftMediumStyle;
	
	
	private GUIStyle classicButton;
	private GUIStyle modernButton;
	
	private GUIStyle playerWritingStyle;
	
	
	private Vector2 scrollPosition;
	
	internal int currentGUI = 0;
	private int switchGUI = 0;
	
	
	internal Scene currentScene;

	private int characterCreationStep = -1;
	private string characterBio = "";
	
	private string textfieldTempValue = "Player";
	
	
	void Start ()
	{
		
		gameManager = GameObject.FindGameObjectWithTag ( "GameManager" ).GetComponent<GameManager> ();
		
		GenerateStandardGUIStyles ( classicFont );
	}
	
	
	private void GenerateStandardGUIStyles ( Font mainFont )
	{
		
		inactiveButton = new GUIStyle ();
		inactiveButton.font = mainFont;
		inactiveButton.fontSize = 16;
		inactiveButton.normal.background = greyButtons[0];
		inactiveButton.alignment = TextAnchor.MiddleCenter;
		inactiveButton.border = new RectOffset ( 6, 6, 6, 6 );
		inactiveButton.padding = new RectOffset ( 4, 4, 4, 2 );
		inactiveButton.margin = new RectOffset ( 4, 4, 4, 4 );
		
		normalButton = new GUIStyle ();
		normalButton.font = mainFont;
		normalButton.fontSize = 16;
		normalButton.normal.background = blueButtons[2];
		normalButton.hover.background = blueButtons[1];
		normalButton.active.background = blueButtons[0];
		normalButton.alignment = TextAnchor.MiddleCenter;
		normalButton.border = new RectOffset ( 6, 6, 6, 6 );
		normalButton.padding = new RectOffset ( 4, 4, 4, 2 );
		normalButton.margin = new RectOffset ( 4, 4, 4, 4 );
		
		violentButton = new GUIStyle ();
		violentButton.font = mainFont;
		violentButton.fontSize = 16;
		violentButton.normal.background = redButtons[2];
		violentButton.hover.background = redButtons[1];
		violentButton.active.background = redButtons[0];
		violentButton.alignment = TextAnchor.MiddleCenter;
		violentButton.border = new RectOffset ( 6, 6, 6, 6 );
		violentButton.padding = new RectOffset ( 4, 4, 4, 2 );
		violentButton.margin = new RectOffset ( 4, 4, 4, 4 );
		
		sensualButton = new GUIStyle ();
		sensualButton.font = mainFont;
		sensualButton.fontSize = 16;
		sensualButton.normal.background = purpleButtons[2];
		sensualButton.hover.background = purpleButtons[1];
		sensualButton.active.background = purpleButtons[0];
		sensualButton.alignment = TextAnchor.MiddleCenter;
		sensualButton.border = new RectOffset ( 6, 6, 6, 6 );
		sensualButton.padding = new RectOffset ( 4, 4, 4, 2 );
		sensualButton.margin = new RectOffset ( 4, 4, 4, 4 );
		
		
		normalTextfield = new GUIStyle ();
		normalTextfield.font = mainFont;
		normalTextfield.fontSize = 16;
		normalTextfield.normal.background = blueButtons[2];
		normalTextfield.hover.background = blueButtons[1];
		normalTextfield.focused.background = blueButtons[0];
		normalTextfield.alignment = TextAnchor.MiddleLeft;
		normalTextfield.border = new RectOffset ( 4, 4, 4, 4 );
		normalTextfield.padding = new RectOffset ( 4, 4, 4, 2 );
		normalTextfield.margin = new RectOffset ( 4, 4, 4, 4 );
		
		
		labelCenterLargeStyle = new GUIStyle ();
		labelCenterLargeStyle.font = mainFont;
		labelCenterLargeStyle.fontSize = 24;
		labelCenterLargeStyle.wordWrap = true;
		labelCenterLargeStyle.alignment = TextAnchor.MiddleCenter;
		labelCenterLargeStyle.padding = new RectOffset ( 1, 1, 3, 3 );
		labelCenterLargeStyle.margin = new RectOffset ( 4, 4, 4, 4 );
		
		labelCenterMediumStyle = new GUIStyle ();
		labelCenterMediumStyle.font = mainFont;
		labelCenterMediumStyle.fontSize = 16;
		labelCenterMediumStyle.wordWrap = true;
		labelCenterMediumStyle.alignment = TextAnchor.MiddleCenter;
		labelCenterMediumStyle.padding = new RectOffset ( 1, 1, 3, 3 );
		labelCenterMediumStyle.margin = new RectOffset ( 4, 4, 4, 4 );
		
		
		labelLeftLargeStyle = new GUIStyle ();
		labelLeftLargeStyle.font = mainFont;
		labelLeftLargeStyle.fontSize = 24;
		labelLeftLargeStyle.wordWrap = true;
		labelLeftLargeStyle.alignment = TextAnchor.MiddleLeft;
		labelLeftLargeStyle.padding = new RectOffset ( 1, 1, 3, 3 );
		labelLeftLargeStyle.margin = new RectOffset ( 4, 4, 4, 4 );
		
		labelLeftMediumStyle = new GUIStyle ();
		labelLeftMediumStyle.font = mainFont;
		labelLeftMediumStyle.fontSize = 16;
		labelLeftMediumStyle.wordWrap = true;
		labelLeftMediumStyle.alignment = TextAnchor.MiddleLeft;
		labelLeftMediumStyle.padding = new RectOffset ( 1, 1, 3, 3 );
		labelLeftMediumStyle.margin = new RectOffset ( 4, 4, 4, 4 );
		
		
		classicButton = new GUIStyle ();
		classicButton.font = classicFont;
		classicButton.fontSize = 16;
		classicButton.normal.background = blueButtons[2];
		classicButton.hover.background = blueButtons[1];
		classicButton.active.background = blueButtons[0];
		classicButton.alignment = TextAnchor.MiddleCenter;
		classicButton.border = new RectOffset ( 6, 6, 6, 6 );
		classicButton.padding = new RectOffset ( 4, 4, 4, 2 );
		classicButton.margin = new RectOffset ( 4, 4, 4, 4 );
		
		modernButton = new GUIStyle ();
		modernButton.font = modernFont;
		modernButton.fontSize = 16;
		modernButton.normal.background = blueButtons[2];
		modernButton.hover.background = blueButtons[1];
		modernButton.active.background = blueButtons[0];
		modernButton.alignment = TextAnchor.MiddleCenter;
		modernButton.border = new RectOffset ( 6, 6, 6, 6 );
		modernButton.padding = new RectOffset ( 4, 4, 4, 2 );
		modernButton.margin = new RectOffset ( 4, 4, 4, 4 );
	}
	
	
	private void GenerateVariableGUIStyles ()
	{
		
		playerWritingStyle = new GUIStyle ();
		playerWritingStyle.font = Player.player.HandWriting;
		playerWritingStyle.fontSize = 48;
		playerWritingStyle.wordWrap = true;
		playerWritingStyle.alignment = TextAnchor.MiddleLeft;
		playerWritingStyle.padding = new RectOffset ( 3, 3, 6, 6 );
		playerWritingStyle.margin = new RectOffset ( 4, 4, 4, 4 );
	}
	
	
	public string VerifyCharacters ( string originalString )
	{

		string newString = RemoveWhiteSpace ( originalString );
		if ( string.IsNullOrEmpty ( newString ))
		{
			
			return "";
		}
		
		newString.Replace ( System.Environment.NewLine, "" );
		newString = char.ToUpper ( newString[0] ) + newString.Substring ( 1 );
		
		return newString;
	}
	
	private string RemoveWhiteSpace ( string originalString )
	{
		
		return originalString.Trim ().Replace ( System.Environment.NewLine, "" );
	}
	
	
	/*private IEnumerator AutoText ( string originalMessage )
	{
		
		char[] charArray = originalMessage.ToCharArray ();
		
		int charIndex = 0;
		while ( charIndex < charArray.Length )
		{
			
			currentMessage = currentMessage + charArray[charIndex];
			yield return new WaitForSeconds ( 0.2f );
		}
	}*/


	private void OnGUI ()
	{
		
		switchGUI = currentGUI;
		
		GUI.skin = defaultSkin;
		
		switch ( switchGUI )
		{
			
			case 0:
			GUILayout.Window ( 0, new Rect ( 0, 0, Screen.width, Screen.height ), MainMenuWindow, "" );
			break;
			
			case 1:
			GUILayout.Window ( 1, new Rect ( 0, 0, Screen.width, Screen.height ), OptionsWindow, "" );
			break;
			
			case 2:
			GUILayout.Window ( 2, new Rect ( 0, 0, Screen.width, Screen.height ), IntroWindow, "" );
			break;
			
			case 3:
			GUILayout.Window ( 3, new Rect ( 0, 0, Screen.width, Screen.height ), CharacterCreationWindow, "" );
			break;
			
			case 4:
			break;
			
			default:
			break;
		}
	}
	
	
	private void MainMenuWindow ( int windowID )
	{
		
		GUILayout.BeginHorizontal ();
		GUILayout.FlexibleSpace ();
		GUILayout.BeginVertical ();
		GUILayout.Space ( 20 );
		GUILayout.Label ( "SRoguelike", labelCenterLargeStyle );
		GUILayout.FlexibleSpace ();
		if ( GUILayout.Button ( "Start New Game", normalButton ))
		{
		
			gameManager.NewGame ();
		}
		
		GUILayout.Button ( "Load Saved Game", inactiveButton );
		
		if ( GUILayout.Button ( "Options", normalButton ))
		{
			
			currentGUI = 1;
		}
		
		GUILayout.Button ( "Quit Game", violentButton );
		
		
		GUILayout.FlexibleSpace ();
		GUILayout.EndVertical ();
		GUILayout.FlexibleSpace ();
		GUILayout.EndHorizontal ();
	}
	
	
	private void OptionsWindow ( int windowID )
	{
		
		GUILayout.BeginVertical ();
		GUILayout.FlexibleSpace ();
		GUILayout.BeginHorizontal ();
		GUILayout.FlexibleSpace ();
		GUILayout.BeginVertical ();
		scrollPosition = GUILayout.BeginScrollView ( scrollPosition, false, true, GUILayout.Width ( 800 ), GUILayout.Height ( 400 ));
		
		GUILayout.Label ( "OPTIONS WINDOW", labelLeftLargeStyle );
		if ( GUILayout.Button ( "Back", normalButton ))
		{
			
			currentGUI = 0;
		}
		GUILayout.BeginHorizontal ();
		if ( GUILayout.Button ( "Modern Style", modernButton ))
		{
			
			GenerateStandardGUIStyles ( modernFont );
		}
		
		if ( GUILayout.Button ( "Classic Style", classicButton ))
		{

			GenerateStandardGUIStyles ( classicFont );
		}
		GUILayout.FlexibleSpace ();
		GUILayout.EndHorizontal ();
		
		GUILayout.Button ( "Force One Save", normalButton );
		
		GUILayout.Button ( "Developer Mode", sensualButton );
		
		GUILayout.EndScrollView ();
		GUILayout.EndVertical ();
		GUILayout.FlexibleSpace ();
		GUILayout.EndHorizontal ();
		GUILayout.FlexibleSpace ();
		GUILayout.EndVertical ();
	}
	
	
	internal void StartIntro ()
	{
		
		Player.player.HandWriting = playerWrittingFont[UnityEngine.Random.Range ( 0, playerWrittingFont.Length )];
		GenerateVariableGUIStyles ();
		
		currentScene = gameManager.story.introduction.scenes[0];
		currentGUI = 2;
	}
	
	
	private void IntroWindow ( int windowID )
	{
		
		GUI.FocusWindow ( windowID );
		
		GUILayout.BeginHorizontal ();
		GUILayout.BeginVertical ();
		scrollPosition = GUILayout.BeginScrollView ( scrollPosition );

		GUILayout.Label ( currentScene.rawText, playerWritingStyle );
		
		GUILayout.FlexibleSpace ();
		
		int interactionIndex = 0;
		while ( interactionIndex < currentScene.interactions.Count )
		{
			
			switch ( currentScene.interactions[interactionIndex].interactionType )
			{
				
				case "button":
				InsertButton ( interactionIndex );
				break;
			}
			
			interactionIndex += 1;
		}
	
		GUILayout.EndScrollView ();
		GUILayout.EndVertical ();
		GUILayout.EndHorizontal ();
	}
	
	
	private void InsertLabel ( string text )
	{
		
		GUILayout.Space ( 10 );
		GUILayout.Label ( text, labelCenterMediumStyle );
	}
	
	
	private void InsertButton ( int interactionIndex )
	{
		
		if ( currentScene.interactions[interactionIndex].interactionAffect.affectValue > -1 )
		{
			
			if ( GUILayout.Button ( currentScene.interactions[interactionIndex].interactionMessage, normalButton ))
			{
			
				SetVariable.ToPlayer ( currentScene.interactions[interactionIndex].interactionAffect.affectProperty, currentScene.interactions[interactionIndex].interactionAffect.affectValue.ToString ());
				MoveCharacterCreationScene ( characterCreationStep += 1 );
			}
		} else
		{
			
			int destinationInt;
			if ( int.TryParse ( currentScene.interactions[interactionIndex].interactionDestination, out destinationInt ))
			{
				
				if ( GUILayout.Button ( currentScene.interactions[interactionIndex].interactionMessage, normalButton ))
				{
			
				
					currentScene = gameManager.story.introduction.scenes[destinationInt];
					scrollPosition.y = Mathf.NegativeInfinity;
				}
			} else
			{
				
				if ( currentScene.interactions[interactionIndex].interactionDestination == "end" )
				{
				
					if ( GUILayout.Button ( currentScene.interactions[interactionIndex].interactionMessage, normalButton ))
					{
					
						currentGUI = currentScene.interactions[interactionIndex].interactionEndNextGUI;
					}
				}
			} 
		}
	}
	
	
	private void InsertPlayerNameTextfield ()
	{
		
		GUILayout.BeginHorizontal ();
		textfieldTempValue = GUILayout.TextField ( VerifyCharacters ( textfieldTempValue ), 34, normalTextfield );
		if ( GUILayout.Button ( "Confirm", normalButton, GUILayout.Width ( 200 )))
		{
		
			if ( string.IsNullOrEmpty ( RemoveWhiteSpace ( textfieldTempValue )))
			{
			
				textfieldTempValue = "Player";
			}
			
			SetVariable.ToPlayer ( "Name", textfieldTempValue );
			
			MoveCharacterCreationScene ( characterCreationStep += 1 );
			GUIUtility.keyboardControl = 0;
		}
		
		GUILayout.EndHorizontal ();
	}

	
	private void MoveCharacterCreationScene ( int nextStep )
	{
		
		characterCreationStep = nextStep;
		currentScene = gameManager.story.characterCreation.scenes[characterCreationStep];
		characterBio = GetVariable.FromPlayer ( currentScene.rawText );
	}
	
	
	private void CharacterCreationWindow ( int windowID )
	{
		
		GUILayout.BeginVertical ();
		
		GUILayout.Label ( characterBio, playerWritingStyle );
		
		GUILayout.FlexibleSpace ();
		GUILayout.BeginHorizontal ();
		GUILayout.FlexibleSpace ();
		GUILayout.BeginVertical ();
		GUILayout.FlexibleSpace ();
		
		GUILayout.Label ( "Character Creation", labelCenterLargeStyle );
		scrollPosition = GUILayout.BeginScrollView ( scrollPosition, false, false, GUILayout.Width ( 600 ), GUILayout.Height ( 202 ));
		
		if ( characterCreationStep == -1 )
		{
			
			if ( GUILayout.Button ( "Create Character", normalButton ))
			{
		
				MoveCharacterCreationScene ( 0 );
			}
		} else
		{
		
			int interactionIndex = 0;
			while ( interactionIndex < currentScene.interactions.Count )
			{
				
				switch ( currentScene.interactions[interactionIndex].interactionType )
				{
					
					case "label":
					InsertLabel ( currentScene.interactions[interactionIndex].interactionMessage );
					GUILayout.Space ( 10 );
					break;
					
					case "button":
					InsertButton ( interactionIndex );
					break;
					
					case "textfield":
					InsertPlayerNameTextfield ();
					break;
					
					default:
					UnityEngine.Debug.Log ( "Default" );
					break;
				}
				
				interactionIndex += 1;
			}
			
			if ( characterCreationStep > 0 )
			{
			
				GUILayout.Space ( 20 );
				if ( GUILayout.Button ( "Previous Step", violentButton ))
				{
		
					MoveCharacterCreationScene ( characterCreationStep - 1 );
				}
			}
		}
		
		GUILayout.EndScrollView ();
		GUILayout.EndVertical ();
		GUILayout.FlexibleSpace ();
		GUILayout.EndHorizontal ();
		GUILayout.EndVertical ();
	}
}
