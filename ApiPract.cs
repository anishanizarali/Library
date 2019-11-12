using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class ApiPract : MonoBehaviour
{
    /// <summary>
    /// This class posts a token to a url for authentication and gets books in JSON
    /// </summary>

    //Token renewal timer
    private float timer = 0;

    //Token
    private string accessToken;

    //Urls
    private string getUrl = "https://library3.samford.edu/iii/sierra-api/v5/bibs/";
    private string postUrl = "https://library3.samford.edu/iii/sierra-api/v5/token";

    void Start()
    {
        //I think coroutines execute at the same time and wait for yield breakpoints of both methods to be met before continuing
        StartCoroutine(PostToken());
        StartCoroutine(GetBooks(getUrl));
    }

    //Reauthenticates every 5 minutes
    private void Update()
    {
        timer += Time.deltaTime;

        if (timer >= 300)
        {
            PostToken();
            timer = 0;
        }
    }

    IEnumerator GetBooks(string url)
    {
        using (UnityWebRequest webRequest = UnityWebRequest.Get(url))
        {
            yield return new WaitForSeconds(2);
            webRequest.SetRequestHeader("Authorization", "Bearer " + accessToken);
            webRequest.SetRequestHeader("fields", "title");

            yield return webRequest.SendWebRequest();

            string[] books = url.Split('/');

            if (webRequest.isNetworkError || webRequest.isHttpError)
            {
                Debug.Log(webRequest.error);
            }
            else
            {
                Debug.Log("Response: ");
                Debug.Log(webRequest.downloadHandler.text);
                Debug.Log(webRequest.responseCode);
            }
        }
    }

    IEnumerator PostToken()
    {
        Debug.Log("Post Request");

        WWWForm form = new WWWForm();
        form.AddField("grant_type", "client_credentials");

        using (UnityWebRequest www = UnityWebRequest.Post(postUrl, form))
        {
            www.SetRequestHeader("Authorization", "Basic b1pxYnQvM2lvTWxSeE5UZ3pmdzdSMit4bUt3UDpsaWJyYXJ5M1ZS");
            www.SetRequestHeader("Content-Type", "application/x-www-form-urlencoded");

            yield return www.SendWebRequest();

            if (www.isNetworkError || www.isHttpError)
            {
                Debug.Log(www.error);
            }
            else
            {
                string response = www.downloadHandler.text;
                string[] parameters = response.Split('"');
                accessToken = parameters[3];

                Debug.Log("Token: ");
                Debug.Log(accessToken);
            }
        }
    }
}
