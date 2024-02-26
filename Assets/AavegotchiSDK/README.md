The First Release of the AavegotchiSDK was made available which allows using 3D Aavegotchis in Unity.

There are a few dependencies that need to be set up before you can get started
Packages Required:
- Addressable Assets
    Required to load wearables
- Universal Render Pipeline
    All the shaders and materials are set up for URP
    Need to create a URP asset and assign it to the scriptable render pipeline settings in Project Settings>Graphics
- TextMeshPro
    Was included in the SDK, but needs some additional setup to get working (just install it beforehand)
- DOTween
    Also included in the SDK, but needs some additional setup to get working (just install it beforehand)
    When setting it up, create the ASMDEF in the DOTween utility folder and add it to the AavegotchiSDK assembly definition references

This is the first release of the AavegotchiSDK so it is a little bit rough around the edges still. 
I'm sure this will get cleaned up in future releases.
I've made some additional modifications to the AavegotchiSDK to make it a bit cleaner and to get it to work with AavegotchiKit:
- Removed unnecessary files (Gotchi Guardians related assets)  
- Put scripts into a namespace (GotchiSDK)
- Refactored the audio to just use a single audio source
- Rearranged and renamed some folders
- Resized some of the textures so they aren't so large (added a convert.bat script that uses image-magick)
