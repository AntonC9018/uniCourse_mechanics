from manim import *
from helper import set_default_output, setup_grid_and_camera

class Grid(MovingCameraScene):
    def __init__(self, **kwargs):
        set_default_output("02_grid")
        super().__init__(**kwargs)

    def construct(self):
        grid = setup_grid_and_camera(
                scene=self,
                start_pos=np.array([0, 0, 0]),
                end_pos=np.array([4, 4, 0]),
                padding=0,
                grid_padding=0.5)

        self.play(Create(grid), run_time=2)
        self.wait(2)
