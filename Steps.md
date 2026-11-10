Show relevant assets
- jump animation sprits
- isometric tiles image

Step 1 - Add animation
Create Folders scenes and scripts
Create Player scenes as a CharacterBody2D
Add a CollisionShape2D
Select the RectangleShape2D
Add AnimatedSprite2D
Add SpriteFrames
Drop jump sprites in the animation frames
Add AnimationPlayer node
Create new "Jump" animation
Add property track for the AnimatedSprite2D
Select Frame property
Add keys and set the time 
Play the animation
Animate the scale too 

Step 2 - Add input
Create the Game scene
Set as main scene
Instanciate the player scene
Add "Jump" action in the input map
Add an event for "space" key
Create and attach a Player.gd script
Add a "jump" function to call $AnimationPlayer.play
Add a Game script
Add the input.Is_action_just_pression to call $Player.jump()
Play the scene
Add camera to the player

Step 2.1 - Add Enemy
Create enemy scene of Area2D type
Add CollisionShape2D rectangle
Add AnimatedSprite2D 
Add SpriteFrames
Add frame from spritesheet (button on the sprite frame tool)
position += Vector2(speed * delta, 0)
connect to body_entered to print Game Over

Step 3 - Create the tile map
Add TileMap in game scene
Create the TileSet and select it 
Select isometric, Diamond down, vertical offset
Size 64 32
Drop the isometric image on the TileSet tool
Set texture region to 64 64 
You can add tiles now

Step 4 - Generate the tile map
Add scripts TilesGenerator, Math and Global from the other project
Make Global autoloadable (Project settings > autoload)
Preload TilesGenerator and Math in Game script
Instanciate (TileGenerator.new()) 
Add the tile generator to the scene (add_child)
In  _ready, start the generator (tileGen.start_generating_tiles)

!! Change the generator max pool size to 3

Create the player_step signal in Game script - needed by the generator
Double click on the signal in the UI to create the handler and connect it
Emit the player_set when jumping

connect to the "new_tile" signal
	tiles_gen.connect("new_tile", Callable(self, "on_new_tile"))

Add the tile
	$TileMap.call_deferred('set_cell', 0, position, 0, Vector2i(0, 0))
!! Make sure the tile id is correct



