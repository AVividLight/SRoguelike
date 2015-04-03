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
		
		return user;
	}
	
	
	private APIKey GetKey ( RestClient client, string username, string password )
	{
		
		APIKey newKey = null;
		
		client.Authenticator = new HttpBasicAuthenticator ( username, password );

		RestRequest apikeyRequest = new RestRequest ( "api/ApiKey", Method.POST );
		apikeyRequest.RequestFormat = DataFormat.Json;
		IRestResponse apikeyResponse = client.Execute ( apikeyRequest );
		
		newKey = JsonConvert.DeserializeObject<APIKey> ( apikeyResponse.Content );
		
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
		
		RestRequest meRequest = new RestRequest ( "api/Me", Method.GET );
		meRequest.RequestFormat = DataFormat.Json;
		IRestResponse meResponse = client.Execute ( meRequest );
		
		newMe = JsonConvert.DeserializeObject<Me> ( meResponse.Content );
		
		if ( String.IsNullOrEmpty ( newMe.userName ) == true )
		{
			
			UnityEngine.Debug.Log ( "Unable to GET for Me, this account could be corrupted!" );
			return null;
		}
		
		return newMe;
	}
}
