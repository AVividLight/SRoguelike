using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
//Written by Michael Bethke
public class GUIManager : MonoBehaviour
{
	
	public GUISkin guiskin;
	
	
	private APIManager apiManager;

	string username = "Username";
	string password = "Password";
	
	bool connected = false;
	bool credentialsError = false;
	private Rect loginWindowRect;
		
	internal List<String> information = new List<String> ();

	private Vector2 scrollPosition = new Vector2 ( 0, 0 );
	
	
	private void Start ()
	{
		
		apiManager = GameObject.FindGameObjectWithTag ( "APIManager" ).GetComponent<APIManager> ();
		loginWindowRect = new Rect ( Screen.width/2 - 150, Screen.height/2 - 50, 300, 100 );
	}
	

	private void OnGUI ()
	{
		
		GUI.skin = guiskin;
		
		if ( connected == false )
		{
			
			GUI.Window ( 0, loginWindowRect, LoginWindow, "Log In to Steamy API" );
		} else {
			
			scrollPosition = GUILayout.BeginScrollView ( scrollPosition, GUILayout.Width ( 300 ), GUILayout.Height ( Screen.height ));
			foreach ( string info in information )
			{
			
				GUILayout.Label ( info );
			}
			GUILayout.EndScrollView();
		}
	}
	
	
	private void LoginWindow ( int windowID )
	{
		
		GUILayout.BeginVertical ();
		GUILayout.FlexibleSpace ();
		GUILayout.BeginHorizontal ();
		
		GUILayout.BeginVertical ();
		GUILayout.BeginHorizontal ();
		
		username = GUILayout.TextField ( username, GUILayout.MinWidth ( 100 ), GUILayout.MaxWidth ( 100 ));
		password = GUILayout.TextField ( password, GUILayout.MinWidth ( 100 ), GUILayout.MaxWidth ( 100 ));
		
		GUILayout.EndHorizontal ();
		
		if ( GUILayout.Button ( "Connect" ))
		{
			
			User currentUser = apiManager.UserLogin ( username, password );
			if ( currentUser == null )
			{
				
				credentialsError = true;
				loginWindowRect.height = 158;
				
			} else {
				
				connected = true;
				
				information.Add ( "Successfully Connected to Steamy API" );
				if ( PopulateInformation ( currentUser ) != 0 )
				{
					
					information.Add ( "Unable to Populate Information!" );
				}
			}
		}
		
		if ( credentialsError == true )
		{
			
			GUILayout.Label ( "Unable to log in, check Username and Password!" );
		}
		
		
		GUILayout.EndVertical ();
		GUILayout.EndHorizontal ();
		GUILayout.FlexibleSpace ();
		GUILayout.EndVertical ();
	}
	
	
	private int PopulateInformation ( User user )
	{
		
		information.Add ( "APIKey" );
		information.Add ( "\t" + user.apikey.id );
		information.Add ( "\t" + user.apikey.secret );
		information.Add ( "Me" );
		information.Add ( "\t" + user.me.userName );
		
		return 0;
	}
}
