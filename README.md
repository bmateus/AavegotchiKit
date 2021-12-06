# Aavegotchi Kit
A Unity Package for getting started using Aavegotchi in a Unity game.

![Preview](https://github.com/bmateus/AavegotchiKit/blob/main/screenshot.PNG)

I made this kit while working on game and figured it could be helpful to others.
An Aavegotchi's graphics are stored on-chain as SVGs, but it is difficult to use this
in a Unity game. 

Initially, I tried converting the SVGs in a web build by sending a request from unity 
through the unity <-> js bridge, grabbing the svg from on-chain or theGraph, compositing 
it and then using URL.createObjectURL and then sending the URL back to unity, where you 
could do a web request to get a useable image. This works, but besides being 
overly-complicated, it has several disadvantages:

    - It takes a relatively long time to load up a gotchi
    - It uses up a lot of texture memory
    - You can't batch draw calls
    - Can only be used in a WebGL game

This approach might be ok for a game that uses a single Aavegotchi, but would perform 
poorly with many different gotchi simultaneously. It would also be nice to build 
Aavegotchi games for any platform, not just webGL

This new approach uses a javascript / python toolchain to pull all the Aavegotchi data 
from the blockchain and render all the SVGs into PNGs which can then all be packed together 
into a sprite atlas.

As new assets get added, the sprites will need to be updated; 
I'll add the tool I created to import the assets after I clean it up a bit.

There is a custom shader that can take a base aavegotchi and set it's colors based on its 
collateral type (This currently breaks draw call batching as well and am working on 
improving that).

The kit also includes a way to query a users gotchi from theGraph and also get an 
Aavegotchi's stats for a given gotchi id. This doesn't preclude using the kit to make 
custom gotchi (like setting up NPC gotchi for example)

The GotchiBrowser scene contains an example of how to set up a scene to be able to load 
up an Aavegotchi from theGraph.

There are probably a bunch of weirds cases where things might look wrong; 
Please feel free to report any issues or fix them and submit a pull request!



## Dependencies:

- Install these dependencies using the Unity Package Manager:

- UniTask v2.2.5: Aavegotchi Kit uses UniTask for handling asyncronous operations
https://github.com/Cysharp/UniTask.git?path=src/UniTask/Assets/Plugins/UniTask

- SimpleGraphQL v1.3.2: A graphQL client for reading the gotchi data from theGraph
https://github.com/LastAbyss/SimpleGraphQL-For-Unity.git



## Release Notes

- v0.1 - Dec 5 2021 - Initial Release









