# FaerieHelper

A small little code mod adding various (for now one) odd mechanics. This is my first *real* time using C#, so expect janky code (sorry about that).

Currently, adds only a couple entities and a few associated triggers:

## Coriolis Controller
Add this to a room to add coriolis forces, which act at a right angle to the player's movement at a magnitude proportional to their speed. You can think of this as rotating the player's movement direction, akin to superdash steering. By default this rotation is counterclockwise, but this can be changed by the mapper. The forces can also be restricted to only affect specific player states, and by default will not act if it would prevent the player from performing basic techs. Also can affect holdables, and if desired can be configured to only affect them and not the player.

### Coriolis Strength Trigger
Trigger to change coriolis strength mid-room; can also be used to change direction by changing to or from a negative value. Can be enabled/disabled with a flag.

### Coriolis State Control Trigger
Trigger to modify the list of states affected by the local coriolis controller. Can be enabled/disabled with a flag.

### Coriolis Boolean Trigger
Trigger to modify other boolean values in the coriolis controller mid-room. Can be enabled/disabled with a flag.


## Momentum Dash Block
Dash block that, when broken, cancels the player's dash and dash cooldown, preserving momentum. Will also add speed and maintain player momentum for several frames after being broken, akin to retained speed. By default does not cause the player to exit a red bubble, but will still modify speed. Can be set to multiply speed instead of adding it, and accepts negative values for the speed modification. Can be made to set a temporary flag when broken, which will be reset if the player dies or leaves the screen.

#

Big thanks to Snip and μ (and several other in #code_modding) for helping me through the learning process here, and for providing me some example code to work off of
