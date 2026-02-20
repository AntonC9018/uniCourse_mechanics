# Generated using Claude (claude.ai)
from manim import *
from helper import set_default_output
import numpy as np

CELL_SIZE = 1.0

def make_tick(center, size=0.25):
    p1 = center + np.array([-size, 0, 0])
    p2 = center + np.array([-size * 0.2, -size * 0.6, 0])
    p3 = center + np.array([size, size * 0.6, 0])
    l1 = Line(p1, p2, color=GREEN, stroke_width=5)
    l2 = Line(p2, p3, color=GREEN, stroke_width=5)
    return VGroup(l1, l2)

class RaycastDistance(Scene):
    def __init__(self, **kwargs):
        set_default_output("02_raycast_distance")
        config.frame_height = 4
        config.pixel_height = 200
        super().__init__(**kwargs)

    def construct(self):
        # 5 cells total: player at col 0, object at col 4 (distance 4)
        # Grid is 1 row x 5 cols
        num_cols = 5
        num_rows = 1
        origin = np.array([
            -num_cols * CELL_SIZE / 2,
            -num_rows * CELL_SIZE / 2,
            0
        ])

        def col_center(col):
            return origin + np.array([(col + 0.5) * CELL_SIZE, 0.5 * CELL_SIZE, 0])

        # Draw grid
        grid_lines = VGroup()
        for r in range(num_rows + 1):
            line = Line(
                origin + np.array([0, r * CELL_SIZE, 0]),
                origin + np.array([num_cols * CELL_SIZE, r * CELL_SIZE, 0]),
                stroke_color=TEAL, stroke_width=2, stroke_opacity=0.5
            )
            grid_lines.add(line)
        for c in range(num_cols + 1):
            line = Line(
                origin + np.array([c * CELL_SIZE, 0, 0]),
                origin + np.array([c * CELL_SIZE, num_rows * CELL_SIZE, 0]),
                stroke_color=TEAL, stroke_width=2, stroke_opacity=0.5
            )
            grid_lines.add(line)
        self.add(grid_lines)

        # Player at col 0
        player_col = 0
        player_pos = col_center(player_col)
        player = Square(side_length=0.6, color=YELLOW, fill_color=YELLOW, fill_opacity=0.9)
        player.move_to(player_pos)
        player_label = MathTex("P", color=BLACK).scale(0.8).move_to(player_pos)
        self.add(player, player_label)

        # Object at col 4 (distance 4 from player)
        obj_col = 4
        obj_pos = col_center(obj_col)
        obj_sq = Square(side_length=0.6, color=RED, fill_color=RED, fill_opacity=0.8)
        obj_sq.move_to(obj_pos)
        self.add(obj_sq)

        self.wait(0.5)

        def do_raycast(max_dist):
            dist_label = MathTex(r"\text{Max distance} = " + str(max_dist), color=WHITE).scale(0.8)
            dist_label.to_edge(UP).shift(DOWN * 0.3)
            self.play(FadeIn(dist_label))

            highlights = []
            ticks = []
            found = False
            for i in range(1, num_cols):
                col = player_col + i
                if col >= num_cols or i > max_dist:
                    break

                pos = col_center(col)
                h = Square(side_length=CELL_SIZE, color=GREEN, fill_color=GREEN, fill_opacity=0.3)
                h.move_to(pos)
                highlights.append(h)
                self.play(FadeIn(h), run_time=0.3)

                if col == obj_col:
                    # Hit the object — draw tick and stop
                    tick = make_tick(pos)
                    ticks.append(tick)
                    found_label = MathTex(r"\text{Found!}", color=GREEN).scale(0.8)
                    found_label.to_edge(DOWN).shift(UP * 0.3)
                    self.play(Create(tick), Write(found_label), run_time=0.35)
                    self.wait(1.5)
                    self.play(FadeOut(found_label))
                    found = True
                    break

            always_play = [*[FadeOut(h) for h in highlights],
                *[FadeOut(t) for t in ticks],
                FadeOut(dist_label)]
            if not found:
                # Reached max distance without hitting object
                cross = Cross(obj_sq.copy().scale(1.5), color=RED, stroke_width=8)
                cross.move_to(obj_pos)
                not_found = MathTex(r"\text{Not found!}", color=RED).scale(0.8)
                not_found.to_edge(DOWN).shift(UP * 0.3)
                self.play(Create(cross), Write(not_found))
                self.wait(2)
                self.play(FadeOut(cross), FadeOut(not_found), *always_play)

            else:
                self.wait(1)
                self.play(*always_play)

        # First run: max distance 3, object at col 4 (distance 4) => not detected
        do_raycast(max_dist=3)

        # Second run: max distance 4 => finds the object
        do_raycast(max_dist=4)

        self.wait(1)
