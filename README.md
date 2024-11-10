# FaerieHelper

A small little code mod adding various (for now one) odd mechanics. This is my first *real* time using C#, so expect janky code (sorry about that).
This is just a quick initial release to get it out the door for 3RJC, expect more stuff like triggers to change settings and other features to come later.

Currently, adds only a single entity:

# Coriolis Controller
Add this to a room to add coriolis forces, which act at a right angle to the player's movement at a magnitude proportional to their speed. You can think of this as rotating the
player's movement direction, by default this rotation is clockwise but this can be changed by the mapper. The forces can also be restricted to a single axis if desired, disabled
during a dash, or toggled on and off with a flag (untested). Compatibility with other mods has yet to be tested or worked on, so using this controller with other stuff affecting
movement (particularly gravity and dashing) may result in unintended behavior at the moment.