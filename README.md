# FluidSim
 
Fluid simulation based on color differential. Implemented in Unity with GLSL and C#.
Neutral fluid is .5 blue, .5 green, 0 red.
Blue differential disperses to neighboring pixels and becomes green.
Green differential is subtracted from blue.
Red represents solids, which don't propagate internally. Solids leave behind their green when moved/destroyed, creating ripples.
Blue can propagate to the edge of solids. Blue differential can be read in script to apply force/effects.

Algorithms sourced from: https://web.archive.org/web/20160418004149/http://freespace.virgin.net/hugo.elias/graphics/x_water.htm
