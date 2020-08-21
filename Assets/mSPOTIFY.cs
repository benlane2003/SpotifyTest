using System;
using System.IO;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SpotifyAPI.Web;
using SpotifyAPI.Web.Auth;
using Newtonsoft.Json;
using static SpotifyAPI.Web.Scopes;

public class mSPOTIFY : MonoBehaviour
{
    public bool isConnecting = false;
    public bool isConnected = false;

    public string loadingStatus = "";   // If we are loading a track
    public bool isPlaying = false;
    public bool isStopped = false;

    public float timeLeft = 0f;
    public float length = 0f;

    private static string clientID = "32f3e20d4be24912a1cad1e919c12f42";
    private static string secretID = "856ddb9932db4e2ca069b460ea6c96e3";
    private const string credentialsPath = "credentials.json";
    private static readonly EmbedIOAuthServer server = new EmbedIOAuthServer(new Uri("http://localhost:5000/callback"), 5000);
    private static Uri authUri = null;
    private static SpotifyClient spotifyClient = null;
    private static AuthorizationCodeTokenResponse token = null;

    private void Start()
    {

    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Alpha2))
        {
            OpenLoginBrowser();
        }
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            SpotifyConnect();
        }
    }

    public bool Initialize()
    {
        return true;
    }

    public bool Reset()
    {
        return true;
    }

    public void Shutdown()
    {

    }

    public void FUpdate()
    {

    }

    public bool SUpdate()
    {
        return true;
    }

    public void ModeChange()
    {

    }

    public void SceneChange()
    {

    }

    public void TrackChange()
    {

    }

    public void AppChange()
    {

    }

    public void LayoutChange()
    {

    }

    // --- Authentication Process
    public void SpotifyConnect()
    {
        try
        {
            if (token != null)
            {
                Task.Run(() => Connect());
            }
            else
            {
                Task.Run(() => StartAuthentication());
            }
        }
        catch (APIException e)
        {
            Debug.Log(e.Message);
        }
    }

    public void SpotifyDisconnect()
    {

    }

    public void OpenLoginBrowser()
    {
        try
        {
            //BrowserUtil.Open(authUri);
            Application.OpenURL(authUri.AbsoluteUri);
            Debug.Log(authUri);
        }
        catch (APIException e)
        {
            Debug.Log(e.Message);
            Debug.Log("Unable to open browser, URL: " + authUri);
        }
    }

    public int CheckLoginSuccess()
    {
        return 0;
    }

    // --- Playback Controls
    public void Play()
    {

    }

    public void Pause()
    {

    }

    public void Stop()
    {

    }

    public void SetVolume(float value)
    {

    }

    public void SeekNormalized(float value)
    {

    }

    // --- SpotifyAPI calls
    public void SelectTrackAsync(string uri)
    {

    }

    public void GetRecommendationsAsync(string countryCode)
    {

    }

    public void GetTopSongsAsync(string genre)
    {

    }

    public void SearchAsync(string searchTerm)
    {

    }

    public void GetMoodsPlaylists()
    {

    }

    public void GetMoodPlaylistAsync(string mood)
    {

    }

    public void GetUserSavedContentAsync()
    {

    }

    public void GetMoreTracksResultsAsync()
    {

    }

    public void GetMoreArtistsResultsAsync()
    {

    }

    public void GetArtistTopTracksAsync(string id, string countryCode)
    {

    }

    public void GetAlbumTracksAsync(string id)
    {

    }

    public void GetMoreAlbumsResultsAsync()
    {

    }

    public void GetPlaylistTracksAsync(string id)
    {

    }

    public void GetMorePlaylistsResultsAsync()
    {

    }

    // --- Async Tasks
    private static async Task Connect()
    {
        try
        {
            Debug.Log("Connecting");
            //var json = File.ReadAllText(credentialsPath);
            //var token = JsonConvert.DeserializeObject<AuthorizationCodeTokenResponse>(json);

            var authenticator = new AuthorizationCodeAuthenticator(clientID, secretID, token);
            //authenticator.TokenRefreshed += (sender, payload) => File.WriteAllText(credentialsPath, JsonConvert.SerializeObject(payload));

            var config = SpotifyClientConfig.CreateDefault().WithAuthenticator(authenticator);

            spotifyClient = new SpotifyClient(config);

            var task = await spotifyClient.UserProfile.Current();
            Debug.Log(task.DisplayName);
            Debug.Log("Spotify logged in, Display Name: " + task.DisplayName + ", authenticated");

            server.Dispose();
            //Environment.Exit(0);
        }
        catch (APIException e)
        {
            Debug.Log(e.Message);
        }
    }

    private static async Task StartAuthentication()
    {
        try
        {
            await server.Start();
            Debug.Log("Authentication Started");
            server.AuthorizationCodeReceived += OnAuthorizationCodeReceived;

            var request = new LoginRequest(server.BaseUri, clientID, LoginRequest.ResponseType.Code)
            {
                Scope = new List<string> { UserFollowModify, UserFollowRead, UserLibraryModify,
                                       UserLibraryRead, UserModifyPlaybackState, UserReadCurrentlyPlaying,
                                       UserReadEmail, UserReadPlaybackPosition, UserReadPlaybackState,
                                       UserReadPrivate, UserReadRecentlyPlayed, UserTopRead }
            };
            Debug.Log("Request made");

            authUri = request.ToUri();
            Debug.Log(authUri.ToString());
        }
        catch (Exception e)
        {
            Debug.Log(e.ToString());
        }
    }

    private static async Task OnAuthorizationCodeReceived(object sender, AuthorizationCodeResponse response)
    {
        try
        {
            Debug.Log("Received, " + response.Code);
            await server.Stop();
            Debug.Log("Stopped");
            var config = SpotifyClientConfig.CreateDefault();
            Debug.Log("Created Config");
            Debug.Log(server.BaseUri);
            var requestToken = await new OAuthClient(config).RequestToken(new AuthorizationCodeTokenRequest(clientID, secretID, response.Code, server.BaseUri));
            token = requestToken;
            Debug.Log("Token received");

            await Task.Run(() => Connect());
        }
        catch (Exception e)
        {
            Debug.Log(e.ToString());
        }
    }
}
