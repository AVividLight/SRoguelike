using System;
using RestSharp;
using UnityEngine;
using Newtonsoft.Json;
using System.Collections;
//Written by Michael Bethke
public class User
{
	
	public APIKey apikey = new APIKey ();
	public Me me = new Me ();
}


public class APIKey
{
	
	public string id { get; set; }
	public string secret { get; set; }
}


public class Me
{
	
	public string userName { get; set; }
}


public class APIManager : MonoBehaviour
{	
	
	public User UserLogin ( string username, string password )
	{
		
		User user = new User ();
		
		RestClient client = new RestClient ( "https://steamy.io:443" );
		
		user.apikey = GetKey ( client, username, password );
		user.me = GetMe ( client, user );
		
		//UnityEngine.Debug.Log ( GetFrontPage ( client, user ));
		//UnityEngine.Debug.Log ( GetDownloadHealth ( client, user ));
		
		return user;
	}
	
	
	private APIKey GetKey ( RestClient client, string username, string password )
	{
		
		APIKey newKey = null;
		
		client.Authenticator = new HttpBasicAuthenticator ( username, password );

		RestRequest request = new RestRequest ( "api/ApiKey", Method.POST );
		request.RequestFormat = DataFormat.Json;
		IRestResponse response = client.Execute ( request );
		
		newKey = JsonConvert.DeserializeObject<APIKey> ( response.Content );
		
		if ( String.IsNullOrEmpty ( newKey.id ) == true || String.IsNullOrEmpty ( newKey.secret ) == true )
		{
			
			UnityEngine.Debug.Log ( "Unable to POST for APIKey, check Username and Password" );
			return null;
		}
		
		return newKey;
	}
	
	
	private Me GetMe ( RestClient client, User user )
	{
		
		Me newMe = null;
		
		client.Authenticator = new HttpBasicAuthenticator ( user.apikey.id, user.apikey.secret );
		
		RestRequest request = new RestRequest ( "api/Me", Method.GET );
		request.RequestFormat = DataFormat.Json;
		IRestResponse response = client.Execute ( request );
		
		newMe = JsonConvert.DeserializeObject<Me> ( response.Content );
		
		if ( String.IsNullOrEmpty ( newMe.userName ) == true )
		{
			
			UnityEngine.Debug.Log ( "Unable to GET for Me, this account could be corrupted!" );
			return null;
		}
		
		return newMe;
	}
	
	
	private string GetFrontPage ( RestClient client, User user )
	{
		
		string frontPage = null;
		
		client.Authenticator = new HttpBasicAuthenticator ( user.apikey.id, user.apikey.secret );
		
		RestRequest request = new RestRequest ( "api/Frontpage", Method.GET );
		request.RequestFormat = DataFormat.Json;
		IRestResponse response = client.Execute ( request );
		
		frontPage = response.Content;
		
		if ( String.IsNullOrEmpty ( frontPage ) == true )
		{
		
			UnityEngine.Debug.Log ( "Unable to GET for FrontPage, something went wrong!" );
			return null;
		}
		
		return frontPage;
	}
	
	
	private string GetDownloadHealth ( RestClient client, User user )
	{
			
		string downloadHealth = null;
		    
		client.Authenticator = new HttpBasicAuthenticator ( user.apikey.id, user.apikey.secret );
		    
		RestRequest request = new RestRequest ( "api/download/health", Method.GET );
		request.RequestFormat = DataFormat.Json;
		IRestResponse response = client.Execute ( request );
		    
		downloadHealth = response.Content;
		    
		if ( String.IsNullOrEmpty ( downloadHealth ) == true )
		{
		    
			UnityEngine.Debug.Log ( "Unable to GET for Download Health, something went wrong!" );
			return null;
		}
		    
		return downloadHealth;
	}
	
	
	private string GetAPIVersion ( RestClient client, User user )
	{
			
		string apiVersion = null;
		    
		client.Authenticator = new HttpBasicAuthenticator ( user.apikey.id, user.apikey.secret );
		    
		RestRequest request = new RestRequest ( "api/Version", Method.GET );
		request.RequestFormat = DataFormat.Json;
		IRestResponse response = client.Execute ( request );
		    
		apiVersion = response.Content;
		    
		if ( String.IsNullOrEmpty ( apiVersion ) == true )
		{
		    
			UnityEngine.Debug.Log ( "Unable to GET for Download Health, something went wrong!" );
			return null;
		}
		    
		return apiVersion;
	}
}
