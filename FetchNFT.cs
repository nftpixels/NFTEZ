using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using UnityEngine.Networking;
using UnityEngine.UI;
using TMPro;
using System.Numerics;


public class FetchNFT : MonoBehaviour
{
    // Our chain variables
    [Header("Chain Settings (Required)")]

    [Tooltip("This field is required. Use chains like Binance, Ethereum, Cronos etc.")]
    public string _chain;
    [Tooltip("This field is required. Use testnet, mainnet etc.")]
    public string _network;
    [Tooltip("This is an optional field to specify which contract to use for the function.")]
    public string _contract;
    [Tooltip("This field is required. Enter the wallet address you want to fetch NFTs from.")]
    public string _account;


    // Our NFT Variables
    [Header("NFT Variables (Required)")]

    [Tooltip("The token ID we're using to fetch data.")]
    public int tokenToCheck;
    [Tooltip("The selected Token's URI link.")]
    public string tokenURI;
    [Tooltip("The selected Token's IPFS link.")]
    public string _ImageURL;
    private string OwnedNFTs;


    // Our NFT Data
    public static string NFT_Name;
    public static string NFT_URI;
    public static string NFT_IMG;

    // Our Text Elements
    [Header("UI Elements (Optional)")]

    [Tooltip("This is an optional TextMeshProUGUI Component.")]
    public TextMeshProUGUI NFTNameText;
    [Tooltip("This is an optional TextMeshProUGUI Component.")]
    public TextMeshProUGUI NFTUriText;
    [Tooltip("This is an optional TextMeshProUGUI Component.")]
    public TextMeshProUGUI NFTTokenIDText;
    [Tooltip("This is an optional TextMeshProUGUI Component.")]
    public TextMeshProUGUI NFTImageURL;


    // Our NFT Image
    [Header("Raw Image (Required)")]
    [Tooltip("Add a Raw Image to your scene. It will be used to stream the Image / GIF.")]

    public UniGifImage NFTImage;


    // Our Input Field
    [Header("Input Field (Optional)")]
    [Tooltip("This is an optional TMP_InputField Component")]
    public TMP_InputField TokenInput;



    // Json class for NFT Data
    public class NFTOBJECT
    {
        // You can add all matching Key:Value pairs below that are found in the OwnedNFTs Object.
        // We use the NFT object to get the URI, which we deserialize as well. See the public class below.

        public string uri { get; set; }

        public string tokenid { get; set; }
    }

    // Json class for NFT Data
    public class NFTURIDATA
    {
        // You can add all matching Key:Value pairs below that are found in the NFT URI.
        // Example URI: https://ipfs.io/ipfs/bafkreigdjxhyiuncq24prmyiavmww6mkbpx24be243fd5pabevodgco7li. 

        public string image { get; set; }

        public string name { get; set; }

        // public string description { get; set; } - This is an example of another Key:Value pair we can use.
    }



    public void ChangeTokenID()
    {
        // Change our active token to user input
        string SubmittedToken = TokenInput.text;
        tokenToCheck = int.Parse(SubmittedToken);

        // Clear our input field
        TokenInput.text = "";

        // Check our NFTs
        CheckNFTs();
    }


    // Check our wallet NFTs. You can change the the function being called to EVM.AllErc721 if you prefer to look for ERC721 Tokens.
    public async void CheckNFTs()
    {
        string chain = _chain;
        string network = _network;
        string account = _account;
        string contract = _contract;
        int first = 500;
        int skip = 0;
        string OwnedNFTs = await EVM.AllErc1155(chain, network, account, contract, first, skip);
        print(OwnedNFTs);

        NFTOBJECT[] _nftUri = JsonConvert.DeserializeObject<NFTOBJECT[]>(OwnedNFTs);

        foreach (NFTOBJECT _nft in _nftUri)
        {
            int NFTID = int.Parse(_nft.tokenid); // Parse our JSON string to an integer for better accuracy

            if (NFTID == tokenToCheck) // If our provided token matches a token in our OwnedNFTs object
            {
                // Set our NFT Data
                tokenURI = _nft.uri;
                NFT_URI = _nft.uri;

                // Set our text Elements in the scene
                NFTUriText.text = "NFT URI: " + NFT_URI;
                NFTTokenIDText.text = "NFT TokenID: " + _nft.tokenid;

                // Sanity checks
                Debug.Log("TokenID: " + _nft.tokenid);
                Debug.Log("URI: " + _nft.uri);

                // Get the Data from the URI we fetched
                Invoke("GetDataForNFT", 1f);
            }
        }
    }

    // Get the data from the URI we fetched in the previous function
    public async void GetDataForNFT()
    {
        // Sanitize our string to ensure the URL is correct
        if (tokenURI.StartsWith("ipfs://"))
        {
            tokenURI = tokenURI.Replace("ipfs://", "https://ipfs.io/ipfs/");
            Debug.Log("Response URI: " + tokenURI);
        }

        // Send our WebRequest to get the NFTURIDATA object
        UnityWebRequest webRequest = UnityWebRequest.Get(tokenURI);
        await webRequest.SendWebRequest();
        NFTURIDATA data =
            JsonConvert.DeserializeObject<NFTURIDATA>(
                System.Text.Encoding.UTF8.GetString(webRequest.downloadHandler.data)); // Deserialize it

        // Sanity check
        Debug.Log("NFT NAME: " + data.name);

        // Set our NFT data
        NFT_Name = data.name;
        _ImageURL = data.image;
        NFT_IMG = _ImageURL;

        // Set our text elements in the scene
        NFTNameText.text = data.name;
        NFTImageURL.text = "NFT IMAGE: " + data.image;

        // Sanitize our string to ensure the URL is correct
        if (_ImageURL.StartsWith("ipfs://"))
        {
            _ImageURL = _ImageURL.Replace("ipfs://", "https://ipfs.io/ipfs/");
            Debug.Log("NFT IMAGE URL: " + _ImageURL);
        }

        StartCoroutine(GetTexture()); // Download our image from IPFS
    }

    // Download and apply the NFT Texture
    public IEnumerator GetTexture()
    {
        UnityWebRequest uwr = UnityWebRequestTexture.GetTexture(_ImageURL);
        {
            yield return uwr.SendWebRequest();

            if (uwr.result != UnityWebRequest.Result.Success) // If the download fails..
            {
                StartCoroutine(GetGIF()); // Fallback on our GIF Function
                Debug.Log("Download Failed - Attempting to run GIF Function");
            }
            else
            {
                // Otherwise we download and apply our Image to the RawImage
                NFTImage.GetComponent<RawImage>().texture = DownloadHandlerTexture.GetContent(uwr);
                Debug.Log("NFT Processed Successfully!");
            }
        }
    }

    // Helper function to process the downloaded bytes
    public IEnumerator GetGIF()
    {
        yield return StartCoroutine(NFTImage.SetGifFromUrlCoroutine(_ImageURL));
        Debug.Log("The GIF has been retrieved");
    }

}
