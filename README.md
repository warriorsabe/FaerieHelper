# FaerieHelper

A small little code mod adding various (for now one) odd mechanics. This is my first *real* time using C#, so expect janky code (sorry about that).

Currently, adds only a single entity and a few associated triggers:

## Coriolis Controller
Add this to a room to add coriolis forces, which act at a right angle to the player's movement at a magnitude proportional to their speed. You can think of this as rotating the
player's movement direction, by default this rotation is clockwise but this can be changed by the mapper. The forces can also be restricted to a single axis if desired, disabled
during a dash, or toggled on and off with a flag. Also affects holdables.

### Coriolis Strength Trigger
Trigger to change coriolis strength mid-room; can also be used to change direction by changing to or from a negative value. Can be enabled/disabled with a flag.

### Coriolis Dash Mode Trigger
Trigger to change how the coriolis controller interacts with dashes mid-room. Can be enabled/disabled with a flag.

### Coriolis Direction Mode Trigger
Trigger to change which axes the coriolis controller applies forces to mid-room. Can be enabled/disabled with a flag.

### Coriolis Boolean Trigger
Trigger to modify other boolean values in the coriolis controller mid-room. Can be enabled/disabled with a flag.

#

Big thanks to Snip and Rain (and several other in #code-modding) for helping me through the learning process here, and for providing me some example code to work off of
