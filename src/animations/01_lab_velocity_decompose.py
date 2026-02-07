from manim import *
from helper import set_default_output, create_component_animations_2d, setup_grid_and_camera

class VelocityDecomposition(MovingCameraScene):
    def __init__(self, **kwargs):
        set_default_output("01_vector_decomposition")
        super().__init__(**kwargs)

    def construct(self):
        velocities = [
            np.array([ 3,  5, 0]),
            np.array([-5,  2, 0]),
            np.array([ 3, -3, 0]),
            np.array([-1, -2, 0])]
        for v in velocities:
            self.play_iter(v)

    def play_iter(self, velocity):
        start_pos = np.array([0, 0, 0])
        end_pos = start_pos + velocity
        grid = setup_grid_and_camera(
                scene=self,
                start_pos=start_pos,
                end_pos=end_pos,
                padding=2,
                grid_padding=5)
        self.add(grid)

        apple = ImageMobject("assets/apple.png")
        apple.scale(0.3)  # Adjust size to fit the grid

        ghost_apple = apple.copy()
        ghost_apple.set_opacity(0.5)

        start_point = grid.coords_to_point(*start_pos)
        apple.move_to(start_point)
        
        target_point = grid.coords_to_point(*end_pos)

        velocity_vector = Arrow(
            start=start_point, 
            end=target_point, 
            buff=0, 
            color=RED, 
            stroke_width=6)
        ghost_apple.add_updater(lambda m: m.move_to(velocity_vector.get_end()))
        component_anims = create_component_animations_2d(start_point, target_point, velocity)

        self.add(apple)
        self.wait(0.5)
        
        self.add(ghost_apple)
        self.play(GrowArrow(velocity_vector))
        self.wait(0.5)

        component_anims.play(self)
        
        self.wait(2)
        self.clear()
