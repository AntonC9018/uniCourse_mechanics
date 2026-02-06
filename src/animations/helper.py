from manim import *
import subprocess
from pathlib import Path
import math
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
    animations_dir = git_root / "animations"
    animations_dir.mkdir(parents=True, exist_ok=True)
    return str(animations_dir / name)

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

import numpy as np
from typing import cast

def setup_grid_and_camera(
    scene: MovingCameraScene,
    start_pos: np.ndarray,
    end_pos: np.ndarray,
    padding: float = 2.0,
    grid_padding: float = 5.0
) -> NumberPlane:
    """
    Setup a number plane grid and adjust camera to fit the scene.
    
    Args:
        scene: The MovingCameraScene to adjust
        start_pos: Starting position (3D array)
        end_pos: Ending position (3D array)
        padding: Padding around the main area
        grid_padding: Additional padding for grid extension
    
    Returns:
        NumberPlane: The created grid
    """
    padding_vec = np.array([1, 1, 0]) * padding
    mins = np.minimum(start_pos, end_pos) - padding_vec
    maxs = np.maximum(start_pos, end_pos) + padding_vec
    size = maxs - mins
    
    grid_padding_vec = np.array([1, 1, 0]) * grid_padding
    grid_maxs = maxs + grid_padding_vec
    grid_mins = mins - grid_padding_vec
    
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
    
    # Adjust camera
    aspect_ratio = config.pixel_width / config.pixel_height
    camera = cast(MovingCamera, scene.camera)
    f = camera.frame
    actual_width = max(size[0], size[1] * aspect_ratio)
    f.move_to(grid.get_center())
    f.scale_to_fit_width(actual_width)
    
    return grid


def get_perpendicular_direction(vector: np.ndarray, prefer_up: bool = True) -> np.ndarray:
    """
    Get a perpendicular direction to a 2D vector.
    
    Args:
        vector: 2D or 3D vector (z component ignored)
        prefer_up: If True, prefer upward direction; otherwise downward
    
    Returns:
        Normalized perpendicular vector
    """
    perp = np.array([-vector[1], vector[0], 0])

    norm = np.linalg.norm(perp)
    if norm > 0:
        perp = perp / norm
    
    if prefer_up and perp[1] < 0:
        perp = -perp
    elif not prefer_up and perp[1] > 0:
        perp = -perp
    
    return perp


def get_vector_angle(vector: np.ndarray) -> float:
    """
    Get the angle of a vector in radians.
    
    Args:
        vector: 2D or 3D vector (z component ignored)
    
    Returns:
        Angle in radians
    """
    return np.arctan2(vector[1], vector[0])
