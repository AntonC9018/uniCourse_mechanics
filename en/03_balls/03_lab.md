# Lab 3

- [Main video with the theory](https://www.youtube.com/watch?v=eED4bSkYCB8)
- [Good info](https://www.cs.cmu.edu/~jbruce/thesis/chapters/thesis-ch03.pdf)

## 1. The simple setup

### 1.1. Setup

- Initialize a new project in the game framework of choice.
- Write code that displays a ball on the screen.
- Store the position and the radius of the ball in variables.
- Make an abstraction for position and radius of the ball (a `Ball` class).
- Try changing the position / radius of the ball back and forth using `sin`.

### 1.2. Movement

- Add a velocity vector field (or multiple fields) to the `Ball` class.
- Update the ball's position each frame based on its velocity.
  Make sure the displacement is dependent on the time passed since last frame.
  (Note: don't use `FixedUpdate`, this has to do with the physics system, 
  we're doing the physics manually)
- Set the velocity to a non-zero value and watch the ball fly off the screen.
- Try applying random changes to the velocity.

### 1.3. Collision with walls

- Make a function that checks if the ball went off the screen.
- Add simple collision simulation by inverting the ball's velocity if it hits a wall.
  Invert the x component when it hits the left or the right wall,
  and the y component when it hits the top or the bottom wall.
- Make sure you take the ball's radius into account.
  For this, test it visually with small velocity values.
- Try reducing the velocity by a half when the ball hits a wall.
  Fix the issue of the ball being stuck inside a wall,
  which arises when the ball fully passes outside the wall
  and has its speed reduced.

  <details>
  <summary>How?</summary>

  The simplest fix is to remember if you've passed the wall this frame,
  and check that again before changing sign.

  Another OK fix is to only invert the velocity if it's already
  going in the opposite direction.
  </details>

### 1.4. Add another ball

- Make the program work for any number of balls.
  At this point, don't simulate collisions between balls.

## 2. Collisions between multiple balls

### 2.1. No mass

- Make a function that checks if two balls collide.
- Make a function that finds the point of collision of two objects,
  and the line tangent both balls at that point.
- Implement collision between balls.
  Reference the formulas for finding the point of collision
  between two balls, calculate the line of reflection by
  reflecting the velocity around the tangent to that point.

### 2.2. Momentum

- Add mass to objects.
- Use the laws of conservation of momentum and conservation of energy
  to derive the momentum transfer formulas for elastic collisions.
- Implement proper momentum transfer between objects,
  without energy loss.

Advanced:
- Implement energy loss during a collision event (non-elastic collisions).
  
### 2.3. Broad phase optimizations

- Implement the Uniform Grid or the Spatial Hash broad phase optimization technique.
- Implement the [sweep and prune algorithm](https://leanrada.com/notes/sweep-and-prune/).

Optionally:
- Implement partitioning by constructing a KD-tree.
- Implement partitioning by constructing a Quadtree.


### 2.4. (Advanced, Optional) Continuous Collision Detection

- Try setting the objects' velocity to a high value 
  and observe them pass through each other.
  Set up a test for this.
  This is called tunneling.
- Solve tunneling by doing continuous collision detection.

## 3. Newtonian physics
