from manim import *
import subprocess
from pathlib import Path
import math
from typing import cast

from numpy.typing import NDArray

def _get_git_root():
    """
    Returns the root directory of the git repository.
    """
    try:
        git_root = subprocess.check_output(
            ['git', 'rev-parse', '--show-toplevel'],
            stderr=subprocess.DEVNULL,
            text=True
        ).strip()
        return Path(git_root)
    except (subprocess.CalledProcessError, FileNotFoundError):
        # Fallback to current directory if not in a git repo
        return Path.cwd()

def set_default_output(name):
    config.output_file = get_output_path(name)
    print(config.output_file)
    config.format = "gif"
    config.frame_height = 8
    config.frame_width = 8
    config.pixel_height = 400
    config.pixel_width = 400

def get_output_path(name):
    git_root = _get_git_root()
    illustrations_dir = git_root / "illustrations"
    illustrations_dir.mkdir(parents=True, exist_ok=True)
    return str(illustrations_dir / name)

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

        padding = np.array([1, 1, 0]) * 2
        mins = np.minimum(start_pos, end_pos) - padding
        maxs = np.maximum(start_pos, end_pos) + padding
        size = maxs - mins

        grid_padding = np.array([1, 1, 0]) * 5
        grid_maxs = maxs + grid_padding
        grid_mins = mins - grid_padding

        grid = NumberPlane(
            x_range=[grid_mins[0], grid_maxs[0], 1],
            y_range=[grid_mins[1], grid_maxs[1], 1],
            axis_config={"stroke_width": 0},
            background_line_style={
                "stroke_color": TEAL,
                "stroke_width": 2,
                "stroke_opacity": 0.3
            }
        )
        self.add(grid)

        aspect_ratio = config.pixel_width / config.pixel_height
        camera = cast(MovingCamera, self.camera)
        f = camera.frame
        actual_width = max(size[0], size[1] * aspect_ratio)
        f.move_to(grid.get_center())
        f.scale_to_fit_width(actual_width)

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
            stroke_width=6
        )
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

class ComponentAnimation:
    def __init__(self, label: MathTex, line: DashedLine):
        self.label = label
        self.line = line

class ComponentAnimations:
    def __init__(self, anims: list[ComponentAnimation]):
        self.anims = anims

    def play(self, scene: Scene):
        for x in self.anims:
            scene.play(Create(x.line), Write(x.label))

def create_component_animations_2d(
        start_point: NDArray,
        end_point: NDArray,
        displacement_normalized: NDArray) -> ComponentAnimations:

    x_direction = DOWN
    x_label = MathTex(f"x = {displacement_normalized[0]}", color=BLUE)
    y_label = MathTex(f"y = {displacement_normalized[1]}", color=BLUE)
    if displacement_normalized[1] > 0:
        corner_point = np.array([end_point[0], start_point[1], start_point[2]])
    else:
        corner_point = np.array([start_point[0], end_point[1], start_point[2]])

    first_line = DashedLine(start_point, corner_point, color=BLUE)
    second_line = DashedLine(corner_point, end_point, color=BLUE)
    lines = [first_line, second_line]
    ret: list[ComponentAnimation] = []
    def add_label(label, direction):
        line = lines[len(ret)]
        label.next_to(line, direction)
        ret.append(ComponentAnimation(label, line))
    def add_x():
        add_label(x_label, x_direction)
    def add_y():
        add_label(y_label, y_direction)

    if math.copysign(1, displacement_normalized[0]) == math.copysign(1, displacement_normalized[1]):
        y_direction = RIGHT
    else:
        y_direction = LEFT

    if displacement_normalized[1] > 0:
        add_x()
        add_y()
    else:
        add_y()
        add_x()

    return ComponentAnimations(ret)
