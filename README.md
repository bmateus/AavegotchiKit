# Aavegotchi Kit
A Unity Package for getting started using Aavegotchi in a Unity game.

![Preview](https://github.com/bmateus/AavegotchiKit/blob/main/Showcase.PNG)

I made this kit while working on a game and figured it could be helpful to others.
An Aavegotchi's graphics are stored on-chain as SVGs, and it can be a bit difficult 
to figure out how to use them in Unity, with the poor support for SVGs in Unity

The Aavegotchi Diamond contract has been included as a easy to use DLL,
so you can interact with the contract directly from Unity using Nethereum

While you can pull the SVG assets directly from the blockchain with the kit,
The SVGs have also been included in an offline database

The kit also includes a way to query a users gotchi from theGraph and also get an 
Aavegotchi's stats for a given gotchi id. This doesn't preclude using the kit to make 
custom gotchi (like setting up NPC gotchi for example)

The kit comes with several example scenes to demonstrate how to do various things:

-- The "GotchiBrowser" scene contains an example of how to set up a scene to be able to load 
up an Aavegotchi from theGraph. You can browse through all the available gotchis

-- The "WearableBrowser" scene contains an example of how to set up a game UI;
You can browse through all the available wearables and see how they look on a gotchi

-- The "NethereumTest" scene demonstrates how to use Nethereum to query the blockchain;
It demonstrates how to query the SVG Facet of the Aavegotchi Diamond Contract

-- The "GotchiController" scene demonstrates how to use the kit to set up a gotchi
with a controller & physics; The gotchi can move around and play with a ball and can 
also wander on its own

-- The "MetaMask" scene demonstrates connecting a wallet with metamask; 
after connecting and signing in, it loads up the connected users gotchis 
and lets you pet them. This uses the MetaMask plugin for Unity available 
on the Unity asset store: 
https://assetstore.unity.com/packages/decentralization/infrastructure/metamask-246786
For WebGL builds, the LoginManager in this example attempts to use the MetaMask plugin 
first to connect to a wallet, falling back to the QR code method if the plugin is not available

-- More examples coming soon!

Please feel free to report any issues or fix them and submit a pull request!

## Dependencies:

- Install these dependencies using the Unity Package Manager:

- UniTask v2.2.5: Aavegotchi Kit uses UniTask for handling asyncronous operations
https://github.com/Cysharp/UniTask.git?path=src/UniTask/Assets/Plugins/UniTask

- SimpleGraphQL v1.3.2: A graphQL client for reading the gotchi data from theGraph
https://github.com/LastAbyss/SimpleGraphQL-For-Unity.git
No longer supported but works fine

- Vector Graphics v2.0.0-preview.21 - my fork: Used for rendering the SVGs
This package doesn't seem to be supported by Unity and this for contains some fixes 
for the SVG renderer that allow it to work with the Aavegotchi SVGs
https://github.com/bmateus/com.unity.vectorgraphics.git

- WebGL Threading Patcher (if making WebGL builds):
https://github.com/VolodymyrBS/WebGLThreadingPatcher.git


## Release Notes

- v0.1 - Dec 5 2021 - Initial Release









