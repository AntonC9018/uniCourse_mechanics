---
marp: true
headingDivider: 2
---

# Introduction to Game Mechanics and Collision Detection

## Linear Algebra and Geometry basics

1. Vectors
2. Operations with vectors
    - addition
    - subtraction
    - length of vector
    - dot product (projection)
3. Unit vectors, normalization
4. 2D Rotations (special case of linear transformations)
5. Line representation
6. Linear interpolation

Resources:
- [3Blue1Brown course](https://www.3blue1brown.com/?v=vectors)
- [3Blue1Brown на русском](https://www.youtube.com/playlist?list=PLVjLpKXnAGLXPaS7FRBjd5yZeXwJxZil2)

## Discrete (cell/grid) 2D physics simulations

1. Integer movement, turn-based mechanics
2. Detecting collisions with walls, other objects
3. Using vectors to reduce code duplication
4. Enabling smooth movement (movement animation)
5. 2D raycasts
6. Optimization techniques:
    - Uniform grid / spatial hash
    - ...
    - Geometry separation (static, dynamic)

## 2D Collisions

1. Defining a collision
2. [Axis-aligned collision detection](https://www.jeffreythompson.org/collision-detection/index.php):
    - point
    - line
    - rectangle
    - circle
3. Non-axis-aligned collision detection
    - AABB (Axis-Aligned Bounding Box)
    - [Separating Axis Theorem](https://www.youtube.com/watch?v=Nm1Cgmbg5SQ&list=PLtrSb4XxIVbpZpV65kk73OoUcIrBzoSiO&index=11)

Additional Resources: 
- [Physics engine playlist](https://www.youtube.com/watch?v=vcgtwY39FT0&list=PLtrSb4XxIVbpZpV65kk73OoUcIrBzoSiO&index=1)
- [Axis-aligned collision demo in JS](https://developer.mozilla.org/en-US/docs/Games/Techniques/2D_collision_detection)
- [Intersection formulae](https://paulbourke.net/geometry/pointlineplane/)


## 2D Movement

1. Position
2. Simulating movement with velocity
3. Newtonian mechanics (forces and acceleration)
4. Momentum, momentul transfer on collision

Resources:
- [Video course](https://www.youtube.com/playlist?list=PLbuK0gG93AsENAa67XysaOr5K0cczxye_)
- [Collisions + Movement in p5.js](https://youtu.be/dJNFPv9Mj-Y)