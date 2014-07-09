Public Access Wars
----

An entry for SA Gamedev IX.

A multiplayer game for 3-10 people where each player runs a show for a competing Public Access TV network. Use your limited budget and substandard props to create a TV show that people want to watch. You have one week to rescue the network in the public access ratings war.


Coding conventions
----
constant variable names are uppercase
public variables are prefixed "u" (e.g. uHostData, uPlayerPrefab)
method parameters are prefixed "p" (e.g. pRoomName, pEvent)
other variables are lower camel case (e.g. userName, password)

Each scene has a SceneManager class which gets called on certain actions (such as players connecting and disconnecting).

Each player has a Player GameObject with a Player script attached. Custom variables for a particular scene should be put in a seperate script and referenced in the SceneManager - so that they are attached and removed when needed.

We use Daikon Forge UI and where possible, Data binding should be used to hook up the GUI to the scripts.

The Awake() method should be used when setting references to other objects (like to get a reference to the scene manager or network manager). This is because it runs before the Start() method.