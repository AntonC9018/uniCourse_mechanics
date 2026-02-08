from manim import *
import numpy as np
from typing import cast

from helper import set_default_output, setup_grid_and_camera, get_perpendicular_direction

class VelocityDirectionSpeed(MovingCameraScene):
    def __init__(self, **kwargs):
        set_default_output("01_normalized_times_length")
        super().__init__(**kwargs)

    def construct(self):
        velocities = [
            np.array([ 2, 0, 0]),
            np.array([ 0, 3, 0]),
            np.array([ 3, 4, 0]),
        ]
        for v in velocities:
            self.play_iter(v)

    def play_iter(self, velocity):
        speed = np.linalg.norm(velocity)
        direction = velocity / speed if speed > 0 else np.array([1, 0, 0])
        start_pos = np.array([0, 0, 0])

        perp_direction = get_perpendicular_direction(direction, prefer_up=True)
        
        # Setup grid and camera
        grid = setup_grid_and_camera(self, start_pos, start_pos + velocity, padding=2.0)
        self.add(grid)
        
        # Starting point
        start_point = grid.coords_to_point(*start_pos)
        direction_end_point = grid.coords_to_point(*(start_pos + direction))
        velocity_end_point = grid.coords_to_point(*(start_pos + velocity))
        
        # Create the velocity vector (final result)
        velocity_vector = Arrow(
            start=start_point,
            end=velocity_end_point,
            buff=0,
            color=RED,
            stroke_width=6
        )
        velocity_label = MathTex(r"\vec{v}", color=RED).scale(0.8)
        velocity_label.next_to(velocity_vector.get_center(), perp_direction, buff=0.2)
        
        # Show initial velocity vector
        self.play(GrowArrow(velocity_vector), Write(velocity_label))
        self.wait(1)
        
        # Fade out velocity to show decomposition
        self.play(
            velocity_vector.animate.set_opacity(0.3),
            velocity_label.animate.set_opacity(0.3))
        self.wait(0.5)
        
        # Create direction vector (unit vector, length = 1)
        direction_vector = Arrow(
            start=start_point,
            end=direction_end_point,
            buff=0,
            color=BLUE,
            stroke_width=6
        )
        direction_label = MathTex(r"\hat{d}", color=BLUE).scale(0.8)
        direction_label.next_to(direction_vector.get_center(), -perp_direction, buff=0.2)
        
        # Show direction vector
        self.play(GrowArrow(direction_vector), Write(direction_label))
        self.wait(0.5)
        
        # Create brace along the direction vector
        length_brace = BraceBetweenPoints(
            start_point, 
            direction_end_point,
            direction=perp_direction)
        length_text = MathTex(r"|\hat{d}| = 1", color=BLUE).scale(0.8)
        length_brace.put_at_tip(length_text)
        
        self.play(GrowFromCenter(length_brace), Write(length_text))
        self.wait(1)
        
        # Show speed scalar - position relative to camera frame
        speed_text = MathTex(r"s = " + f"{speed:.1f}", color=YELLOW).scale(0.9)
        camera = cast(MovingCamera, self.camera)
        f = camera.frame
        speed_text.next_to(f.get_corner(UL), DR, buff=0.2)
        
        self.play(Write(speed_text))
        self.wait(0.5)
        
        # Show multiplication formula
        formula = MathTex(
            r"\vec{v}", r"=", r"s", r"\cdot", r"\hat{d}",
            color=WHITE).scale(0.9)
        formula[0].set_color(RED)
        formula[2].set_color(YELLOW)
        formula[4].set_color(BLUE)
        formula.next_to(speed_text, DOWN, aligned_edge=LEFT, buff=0.3)
        
        self.play(Write(formula))
        self.wait(1)
        
        # Animate scaling: show direction vector scaling to velocity
        # Create a copy of direction vector that will scale
        scaling_vector = direction_vector.copy()
        scaling_vector.set_color(GREEN)
        
        # Remove old annotations
        self.play(
            FadeOut(length_brace),
            FadeOut(length_text),
            FadeOut(direction_label),
            direction_vector.animate.set_opacity(0.3))
        
        self.add(scaling_vector)
        self.wait(0.5)
        
        scaling_label = MathTex(r"s \cdot \hat{d}", color=GREEN).scale(0.8)
        scaling_label.add_updater(lambda x: x.next_to(scaling_vector.get_center(), -perp_direction))
        
        self.add(scaling_label)
        
        # Scale the direction vector by the speed
        self.play(
            scaling_vector.animate.put_start_and_end_on(
                start_point,
                velocity_end_point
            ),
            run_time=2
        )
        self.wait(0.5)
        
        # Show that scaled vector equals velocity
        equals_sign = MathTex(r"=", color=WHITE).scale(0.8)
        equals_sign.move_to(scaling_vector.get_end() + RIGHT * 0.3)
        
        self.play(
            scaling_vector.animate.set_color(RED),
            velocity_vector.animate.set_opacity(1),
            velocity_label.animate.set_opacity(1),
            scaling_label.animate.set_color(RED),
        )
        self.wait(1)
        
        # Highlight the relationship
        highlight_box = SurroundingRectangle(formula, color=YELLOW, buff=0.15)
        self.play(Create(highlight_box))
        self.wait(2)
        
        # Clear for next iteration
        self.clear()
