
------------------------------------
NFT-EZ FOR CHAINSAFE - By Nftpixels
------------------------------------

- This package makes it easy for new users to the Chainsafe SDK to fetch any NFT using the GUI Editor. It features tooltips, so hover over a variable in the editor to find out what it does. 

- There are plenty of comments and debugging in the scripts as well. 

- The script will download and apply both Image based files and GIFs!


PACKAGE REQUIREMENTS:
--------------------

- Install the ChainSafe SDK.
- Connect your wallet before running the scene as it checks for the connected wallet.


BASIC INSTALLATION: 
--------------------

1. Add this package to your unity project. It will add: 

- A demo scene that works out of the box.
- The required "FetchNFT.cs" script.
- The required Helper classes for fetching GIFs from IPFS and other URLs.
- A crisp high-five. 


2. Open the demo scene and run it. Enter your tokenID and away you go! 


----------------------------------------------------------------------------


ADVANCED INSTALLATION:
----------------------

- You can integrate the GIF functionality fairly easily by doing the following

1. Add a RAW IMAGE component to your scene. 

2. Add the "UniGifImage.cs" script to it found under the UniGif folder. 

3. Add the UniGifAspectController.cs script to it found under the UniGif folder. 

4. Add the "FetchNFT.cs" script or any other custom script to the same scene as we'll be passing that IPFS Data to the helper class.

5. Call the following function from anywhere in your scene to download and stream your GIF to the RAW IMAGE. Remember to pass it your Image URL & Raw Image Component.

-     public IEnumerator {FunctionName} ()
    {
        yield return StartCoroutine({YourRawImageComponentHere}.SetGifFromUrlCoroutine({YourIPFSURLHere}));
        Debug.Log("The GIF has been retrieved");
    }

----------------------------------------------------------------------------

Happy Coding! :) 

~ NFTPixels