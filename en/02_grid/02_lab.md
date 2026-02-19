# Lab 2

> You may choose to do this assignment in pairs,
> one person implementing the mechanics and another 
> making it more visually appealing.

## 1. Implement a grid

- Make a project in the game library of choice.
- Write code that procedurally creates and draws a grid of a given size.
- Make a 16 by 8 grid and have it display fully on the screen.
- Use one or more textures for the tiles.
- Make it so that when you hover over a cell, it gets highlighted.

Optionally:
- Add particle effects over highlighted cells.
- Add particle effects when clicking cells with mouse.

## 2. Player

- Add a player.
- The player should be able to move orthogonally, controlled by mouse.
- Highlight the cells that the player can move onto (allowed moves).
- Make sure the player can't move off the grid.

Optionally:
- Add an actual sprite for the player.
- Add sprite idle animation to the player.


## 3. Additional movement

- Implement diagonal movement for the player, controlled by mouse.
- Make a button that switches between orthogonal and diagonal movement.
- Make sure the allowed moves highlighting works for either mode.
- Limit the maximum distance that the player can travel in a move.

Optionally:
- Add a sound effect when trying to select a cell that's not movable to.

## 4. Animation

- Use linear interpolation between the start and the end point of the player's
  path to animate its movement.
- Don't accept user input while animating the movement.

Optionally:
- Queue the user input and apply it one-by-one after the animation finishes.
- Animate the player's path with particles.
- Play a sprite animation on player movement.

## 5. Obstacles

- Procedurally add obstacles.
- Store them or just their positions in a list.
- Make obstacles block movement for the player.
- Make the player be able to destroy the closest obstacle on their path.

Optionally:
- Make different kinds of obstacles that do or do not get destroyed by the player.
- Add sound effects and/or particle effects on destruction.

## 6. Optimization

- Implement the uniform grid and the spatial hash techniques,
  recreating the hash table before computing the allowed moves.

Optionally:
- Separate the static from the dynamic objects,
  create the static data structure once, 
  and recreate the dynamic one as usual.
- Make obstacles move randomly after each of the player's moves.

Advanced:
- Add the ability for obstacles to move to the same place 
  that another obstacle moved off of in the same period.
