using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class RestApiManager : MonoBehaviour
{
    [SerializeField] RawImage[] YourRawImage;
    [SerializeField] int userId = 1;
    [SerializeField] string myApiPath = "https://my-json-server.typicode.com/HeroFromThePast/FakeApi2023";
    [SerializeField] string rickYMortyApi = "https://rickandmortyapi.com/api";
    int[] downloadedDeck = new int[5];
    int imageIndex = 0;
    public void GetCharactersButton()
    {
        StartCoroutine(GetPlayerInfo());
    }

    IEnumerator GetPlayerInfo()
    {

        UnityWebRequest www = UnityWebRequest.Get(myApiPath + "/users/" + userId);
        yield return www.Send();

        if (www.isNetworkError)
        {
            Debug.Log("NETWORK ERROR: " + www.error);
        }
        else
        {
            // Show results as text
            Debug.Log(www.downloadHandler.text);

            string json = www.downloadHandler.text;

            if (www.responseCode == 200) //funciona
            {
                UserJsonData user = JsonUtility.FromJson<UserJsonData>(json);
                Debug.Log(user.name);
                Debug.Log(user.deck);


                for (int i = 0; i < user.deck.Length; i++)
                {
                    StartCoroutine(GetCharacters(user.deck[i], i));
                }




            }
            else
            {
                string message = "status: " + www.responseCode;
                message += "\ncontent-type: " + www.GetResponseHeader("content-type");
                message += "\nError: " + www.error;
                Debug.Log(message);
            }

        }
    }


    IEnumerator GetCharacters(int charId, int imageIndex)
    {
        //UnityWebRequest www = UnityWebRequest.Get(rickYMortyApi + "/character/" + downloadedDeck[0] + ","  + downloadedDeck[1] + "," + downloadedDeck[2] + "," + downloadedDeck[3] + "," + downloadedDeck[4]);
        UnityWebRequest www = UnityWebRequest.Get(rickYMortyApi + "/character/" + charId);
        yield return www.Send();

        if (www.isNetworkError)
        {
            Debug.Log("NETWORK ERROR: " + www.error);
        }
        else
        {
            // Show results as text
            // Debug.Log(www.downloadHandler.text);

            string json = www.downloadHandler.text;

            if (www.responseCode == 200) //funciona
            {
                //CharactersList characters = JsonUtility.FromJson<CharactersList>(www.downloadHandler.text);
                Character character = JsonUtility.FromJson<Character>(json);
                Debug.Log(character.name);

                StartCoroutine(DownloadImage(character.image, imageIndex));


            }
            else
            {
                string message = "status: " + www.responseCode;
                message += "\ncontent-type: " + www.GetResponseHeader("content-type");
                message += "\nError: " + www.error;
                Debug.Log(message);
            }

        }
    }

    IEnumerator DownloadImage(string MediaUrl, int imageIndex)
    {
        UnityWebRequest request = UnityWebRequestTexture.GetTexture(MediaUrl);
        yield return request.SendWebRequest();
        if (request.isNetworkError || request.isHttpError)
            Debug.Log(request.error);
        else YourRawImage[imageIndex].texture = ((DownloadHandlerTexture)request.downloadHandler).texture;
    }

}

public class UserJsonData
{
    public int id;
    public string name;
    public int[] deck;
}


[System.Serializable]
public class CharactersList
{
    public CharacterListInfo info;
    public List<Character> results;

}

[System.Serializable]
public class CharacterListInfo
{
    public int count;
    public int pages;
    public string next;
    public string prev;
}

[System.Serializable]
public class Character
{
    public int id;
    public string name;
    public string species;
    public string image;
}
