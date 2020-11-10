using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.Networking;
using SimpleJSON;
using UnityEngine.UI;
using TMPro;

public class PokeAPIController_Client : MonoBehaviour
{
    public GameObject player1, player2;
    private bool player1_update;
    public RawImage player1Role;

    public RawImage pokeRawImage;
    public TextMeshProUGUI pokeNameText, pokeNumText;
    public TextMeshProUGUI[] pokeTypeTextArray;

    private readonly string basePokeURL = "https://pokeapi.co/api/v2/";


    // Start is called before the first frame update
    void Start()
    {
        pokeRawImage.texture = Texture2D.blackTexture;

        pokeNameText.text = "";
        pokeNumText.text = "";

        foreach ( TextMeshProUGUI pokeTypeText in pokeTypeTextArray )
        {
            pokeTypeText.text = "";
        }

        // Start Play
        player1_update = false;
        StartCoroutine(GetPokemonAtIndex(2, PlayerParam.Client.Player_Index));
        // OnButtonRandomPokemon();
    }

    // Update is called once per frame
    void Update()
    {
        if ( player1_update == false && PlayerParam.Status_Update == true )
        {
            player1_update = true;
            StartCoroutine(GetPokemonAtIndex(1, PlayerParam.Server.Player_Index));
        }
    }

    
    public void OnButtonRandomPokemon()
    {
        int randomPokeIndex = Random.Range(1, 888); // Min: inclusive, Max: exclusive.

        pokeRawImage.texture = Texture2D.blackTexture;

        pokeNameText.text = "Loading...";
        pokeNumText.text = "#" + randomPokeIndex;

        foreach ( TextMeshProUGUI pokeTypeText in pokeTypeTextArray )
        {
            pokeTypeText.text = "";
        }

        StartCoroutine(GetPokemonAtIndex(1, randomPokeIndex));
    }

    IEnumerator GetPokemonAtIndex(int player, int pokemonIndex)
    {
        // Get Pokemon Info.

        string pokemonURL = basePokeURL + "pokemon/" + pokemonIndex.ToString();
        // Example URL: https://pokeapi.co/api/v2/pokemon/151.

        UnityWebRequest pokeInfoRequest = UnityWebRequest.Get(pokemonURL);

        yield return pokeInfoRequest.SendWebRequest();

        if ( pokeInfoRequest.isNetworkError || pokeInfoRequest.isHttpError )
        {
            Debug.LogError(pokeInfoRequest.error);
            yield break;
        }

        JSONNode pokeInfo = JSON.Parse(pokeInfoRequest.downloadHandler.text);

        string pokeName = pokeInfo["name"];
        string pokeSpriteURL = pokeInfo["sprites"]["front_default"];

        JSONNode pokeTypes = pokeInfo["types"];
        string[] pokeTypeNames = new string [pokeTypes.Count];

        for ( int i = 0, j = pokeTypes.Count - 1; i < pokeTypes.Count; i++, j-- )
        {
            pokeTypeNames[j] = pokeTypes[i]["type"]["name"];
        }

        // Get Pokemon Sprite.

        UnityWebRequest pokeSpriteRequest = UnityWebRequestTexture.GetTexture(pokeSpriteURL);

        yield return pokeSpriteRequest.SendWebRequest();

        if ( pokeSpriteRequest.isNetworkError || pokeSpriteRequest.isHttpError )
        {
            Debug.LogError(pokeSpriteRequest.error);
            yield break;
        }

        // Set UI Objects.

        pokeRawImage.texture = DownloadHandlerTexture.GetContent(pokeSpriteRequest);
        pokeRawImage.texture.filterMode = FilterMode.Point;


        if ( player == 1 )
        {
            player1.GetComponent<SpriteRenderer>().sprite = Sprite.Create(DownloadHandlerTexture.GetContent(pokeSpriteRequest), new Rect(0, 0, pokeRawImage.texture.width, pokeRawImage.texture.height), new Vector2(0.5F, 0.5F));
        }
        else if ( player == 2 )
        {
            // player2.GetComponent<SpriteRenderer>().sprite = Sprite.Create(DownloadHandlerTexture.GetContent(pokeSpriteRequest), new Rect(0, 0, pokeRawImage.texture.width, pokeRawImage.texture.height), Vector2.zero);
            player2.GetComponent<SpriteRenderer>().sprite = Sprite.Create(DownloadHandlerTexture.GetContent(pokeSpriteRequest), new Rect(0, 0, pokeRawImage.texture.width, pokeRawImage.texture.height), new Vector2(0.5F, 0.5F));
        }
        

        pokeNameText.text = CapitalizeFirstLetter(pokeName);

        for ( int i = 0; i < pokeTypeNames.Length; i++ )
        {
            pokeTypeTextArray[i].text = CapitalizeFirstLetter(pokeTypeNames[i]);
        }

    }

    private string CapitalizeFirstLetter(string str)
    {
        return char.ToUpper(str[0]) + str.Substring(1);
    }
}
