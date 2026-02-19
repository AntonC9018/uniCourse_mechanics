# Generated using Claude (claude.ai)
from manim import *
from helper import set_default_output
import numpy as np

CELL_SIZE = 1.0
GRID_ROWS = 5
GRID_COLS = 5

def cell_center(grid_origin, row, col):
    """
    grid_origin is the TOP-LEFT corner of the grid.
    Row 0 is at the top, increasing downward (standard grid convention).
    Y is inverted relative to manim's default (which goes up).
    """
    x = grid_origin[0] + (col + 0.5) * CELL_SIZE
    y = grid_origin[1] - (row + 0.5) * CELL_SIZE
    return np.array([x, y, 0])

def make_tick(center, size=0.25):
    p1 = center + np.array([-size, 0, 0])
    p2 = center + np.array([-size * 0.2, -size * 0.6, 0])
    p3 = center + np.array([size, size * 0.6, 0])
    l1 = Line(p1, p2, color=GREEN, stroke_width=5)
    l2 = Line(p2, p3, color=GREEN, stroke_width=5)
    return VGroup(l1, l2)

class RaycastOrthogonal(Scene):
    def __init__(self, **kwargs):
        set_default_output("02_raycast_orthogonal")
        super().__init__(**kwargs)

    def construct(self):
        # TOP-LEFT corner of the grid, centered on screen
        grid_origin = np.array([
            -GRID_COLS * CELL_SIZE / 2,
             GRID_ROWS * CELL_SIZE / 2,
            0
        ])
        player_rc = (2, 2)  # center of 5x5 grid

        # Grid convention: row increases downward, col increases rightward.
        # Player at (row=2, col=2).
        # Objects:
        #   (1, 2) = one step above player (row-1)   -> UP hits at distance 1
        #   (2, 4) = two steps right                  -> RIGHT hits at distance 2
        #   LEFT and DOWN are clear
        object_cells = {(1, 2), (2, 4), (0, 0)}

        # Draw grid lines
        grid_lines = VGroup()
        for r in range(GRID_ROWS + 1):
            y = grid_origin[1] - r * CELL_SIZE
            line = Line(
                np.array([grid_origin[0], y, 0]),
                np.array([grid_origin[0] + GRID_COLS * CELL_SIZE, y, 0]),
                stroke_color=TEAL, stroke_width=2, stroke_opacity=0.5
            )
            grid_lines.add(line)
        for c in range(GRID_COLS + 1):
            x = grid_origin[0] + c * CELL_SIZE
            line = Line(
                np.array([x, grid_origin[1], 0]),
                np.array([x, grid_origin[1] - GRID_ROWS * CELL_SIZE, 0]),
                stroke_color=TEAL, stroke_width=2, stroke_opacity=0.5
            )
            grid_lines.add(line)
        self.add(grid_lines)

        # Player
        player_center = cell_center(grid_origin, player_rc[0], player_rc[1])
        player = Square(side_length=0.6, color=YELLOW, fill_color=YELLOW, fill_opacity=0.9)
        player.move_to(player_center)
        player_label = MathTex("P", color=BLACK).scale(0.8).move_to(player_center)
        self.add(player, player_label)

        # Objects
        for (r, c) in object_cells:
            pos = cell_center(grid_origin, r, c)
            sq = Square(side_length=0.6, color=RED, fill_color=RED, fill_opacity=0.8)
            sq.move_to(pos)
            self.add(sq)

        self.wait(0.5)

        # Directions in grid space (dr, dc).
        # UP   = row decreases (-1) -> visually UP on screen   -> grid label (0, -1)
        # DOWN = row increases (+1) -> visually DOWN on screen  -> grid label (0,  1)
        # RIGHT/LEFT are unaffected by the Y inversion.
        directions = [
            ((0,  1), RIGHT, r"(1,\ 0)"),
            ((-1, 0), UP,    r"(0,\ -1)"),
            ((0, -1), LEFT,  r"(-1,\ 0)"),
            ((1,  0), DOWN,  r"(0,\ 1)"),
        ]

        for (dr, dc), arrow_dir, label_str in directions:
            dir_label = MathTex(label_str, color=WHITE).scale(0.9)
            dir_label.to_corner(UL).shift(RIGHT * 0.3 + DOWN * 0.3)
            self.play(FadeIn(dir_label))

            arrow = Arrow(
                start=player_center,
                end=player_center + arrow_dir * 0.8,
                buff=0, color=GREEN, stroke_width=6
            )
            self.play(GrowArrow(arrow))

            highlights = []
            ticks = []
            r, c = player_rc[0] + dr, player_rc[1] + dc
            while 0 <= r < GRID_ROWS and 0 <= c < GRID_COLS:
                pos = cell_center(grid_origin, r, c)
                h = Square(side_length=CELL_SIZE, color=GREEN, fill_color=GREEN, fill_opacity=0.3)
                h.move_to(pos)
                highlights.append(h)
                self.play(FadeIn(h), run_time=0.3)

                if (r, c) in object_cells:
                    tick = make_tick(pos)
                    ticks.append(tick)
                    self.play(Create(tick), run_time=0.35)
                    break

                r += dr
                c += dc

            self.wait(0.6)
            self.play(
                *[FadeOut(h) for h in highlights],
                *[FadeOut(t) for t in ticks],
                FadeOut(arrow),
                FadeOut(dir_label),
            )

        self.wait(1)
