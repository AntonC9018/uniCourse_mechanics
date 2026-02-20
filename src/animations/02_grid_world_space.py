# code written by claude

from manim import *
from helper import set_default_output

# ── Constants ────────────────────────────────────────────────────────────────
N    = 5      # grid is NxN
CELL = 0.7    # 1 unit per cell
GAP  = 1.0    # horizontal gap between the two grids

# Colors
C_GRID     = BLUE_E
C_GRID_W   = TEAL_E
C_LABEL_G  = WHITE
C_LABEL_W  = YELLOW
C_ORIGIN   = ORANGE
C_ARROW_X  = GREEN
C_ARROW_Y  = RED
C_MAP_LINE = PURPLE_B
C_TITLE    = GRAY_A

# ── Scene anchors ─────────────────────────────────────────────────────────────
# Grid-space grid (LEFT): top-left corner at (G_OX, G_OY) in scene coords.
# Grid corner gy  → scene y = G_OY - gy * CELL
#
# World-space grid (RIGHT): world y grows UP.
# (0,0) is the TOP-LEFT corner of the BOTTOM-LEFT cell.
# So world y=0 is one CELL above the bottom edge (bottom edge = world y=-1).
#
# For projection lines to be horizontal:
#   G_OY - gy*CELL == W_OY + (N-1-gy)*CELL
#   → W_OY = G_OY - (N-1)*CELL

TOTAL = N * CELL
G_OX  = -(GAP / 2) - TOTAL
W_OX  =   GAP / 2
G_OY  =  TOTAL / 2
W_OY  =  G_OY - (N - 1) * CELL      # scene y of world corner y=0


# ── Corner helpers ────────────────────────────────────────────────────────────
def g_corner(gx, gy):
    """Scene position of grid-space corner (gx, gy). Grid y grows DOWN."""
    return np.array([G_OX + gx * CELL, G_OY - gy * CELL, 0])


def w_corner(wx, wy):
    """Scene position of world-space corner (wx, wy). World y grows UP."""
    return np.array([W_OX + wx * CELL, W_OY + wy * CELL, 0])


def g_cell_center(col, row):
    return g_corner(col, row) + np.array([CELL / 2, -CELL / 2, 0])


# ── Grid line builder ─────────────────────────────────────────────────────────
def make_grid_lines(ox, oy, color, y_sign=-1):
    lines = VGroup()
    for c in range(N + 1):
        x = ox + c * CELL
        lines.add(Line(np.array([x, oy, 0]),
                       np.array([x, oy + y_sign * TOTAL, 0]),
                       color=color, stroke_width=2))
    for r in range(N + 1):
        y = oy + y_sign * r * CELL
        lines.add(Line(np.array([ox, y, 0]),
                       np.array([ox + TOTAL, y, 0]),
                       color=color, stroke_width=2))
    return lines


def axis_arrow(start, end, color):
    return Arrow(start, end, buff=0, color=color, stroke_width=3,
                 max_tip_length_to_length_ratio=0.08)


class GridMapping(MovingCameraScene):
    def __init__(self, **kwargs):
        set_default_output("02_grid_world_space")
        config.frame_height = 8
        config.frame_width = 12
        config.pixel_height = 500
        config.pixel_width = 1000
        super().__init__(**kwargs)

    def construct(self):
        # world grid top edge in scene coords
        w_top_y = W_OY + (N - 1) * CELL   # == G_OY  (same as grid top)

        # ═══════════════════════════════════════════════════════════════════
        # GRID-SPACE (LEFT)
        # ═══════════════════════════════════════════════════════════════════
        g_lines = make_grid_lines(G_OX, G_OY, C_GRID, y_sign=-1)

        g_labels = VGroup(*[
            MathTex(f"({c},{r})", font_size=25, color=C_LABEL_G)
            .move_to(g_cell_center(c, r))
            for r in range(N) for c in range(N)
        ])

        g_title = Text("Grid space", font_size=22, color=C_TITLE)
        g_title.next_to(g_lines, UP, buff=0.45)

        # Arrows start AT the coordinate origin: g_corner(0,0)
        g_origin_pt = g_corner(0, 0)
        g_arrow_x = axis_arrow(g_origin_pt, g_origin_pt + RIGHT * TOTAL, C_ARROW_X)
        g_label_x = Text("x", font_size=20, color=C_ARROW_X)
        g_label_x.next_to(g_arrow_x, RIGHT, buff=0.1)

        g_arrow_y = axis_arrow(g_origin_pt, g_origin_pt + DOWN * TOTAL, C_ARROW_Y)
        g_label_y = Text("y", font_size=20, color=C_ARROW_Y)
        g_label_y.next_to(g_arrow_y, DOWN, buff=0.1)

        # Grid origin dot + label
        g_origin_dot = Dot(g_origin_pt, radius=0.10, color=C_ORIGIN)
        g_origin_label = MathTex("(0,0)", font_size=25, color=C_ORIGIN)
        g_origin_label.next_to(g_origin_dot, UR, buff=0.06)

        # ═══════════════════════════════════════════════════════════════════
        # WORLD-SPACE (RIGHT)
        # The world grid spans world y=-1 (bottom edge) to y=4 (top edge).
        # We draw 5 rows anchored from the top (w_top_y).
        # ═══════════════════════════════════════════════════════════════════
        w_lines = make_grid_lines(W_OX, w_top_y, C_GRID_W, y_sign=-1)

        # Screen row r=0 → top → world top-left corner y = N-1 = 4
        # Screen row r=4 → bottom → world top-left corner y = 0
        w_labels = VGroup(*[
            MathTex(f"({c},{N-1-r})", font_size=25, color=C_LABEL_W)
            .move_to(np.array([
                W_OX + c * CELL + CELL / 2,
                w_top_y - r * CELL - CELL / 2,
                0]))
            for r in range(N) for c in range(N)
        ])

        w_title = Text("World space", font_size=22, color=C_TITLE)
        w_title.next_to(w_lines, UP, buff=0.45)

        # World x arrow: starts at world corner (0, N-1) = top-left of top-left cell
        w_x_origin = w_corner(0, N - 1)   # scene: (W_OX, w_top_y)
        w_arrow_x = axis_arrow(w_x_origin, w_x_origin + RIGHT * TOTAL, C_ARROW_X)
        w_label_x = Text("x", font_size=20, color=C_ARROW_X)
        w_label_x.next_to(w_arrow_x, RIGHT, buff=0.1)

        # World y arrow: starts at world corner (0, -1) = bottom edge, goes UP
        w_y_origin = w_corner(0, -1)      # scene: (W_OX, W_OY - CELL)
        w_arrow_y = axis_arrow(w_y_origin, w_y_origin + UP * TOTAL, C_ARROW_Y)
        w_label_y = Text("y", font_size=20, color=C_ARROW_Y)
        w_label_y.next_to(w_arrow_y, UP, buff=0.1)

        # World origin dot + label: shown later (after removing pre-existing one if any)
        w_origin_pt = w_corner(0, 0)      # top-left corner of bottom-left cell
        w_origin_dot = Dot(w_origin_pt, radius=0.10, color=C_ORIGIN)
        w_origin_label = MathTex("(0,0)", font_size=25, color=C_ORIGIN)
        w_origin_label.next_to(w_origin_dot, LEFT, buff=0.06)

        # ═══════════════════════════════════════════════════════════════════
        # PROJECTION LINES  (perfectly horizontal)
        # gy=0→wy=4, gy=1→wy=3, gy=2→wy=2, gy=3→wy=1, gy=4→wy=0, gy=5→wy=-1
        # ═══════════════════════════════════════════════════════════════════
        proj_dots_g = VGroup()
        proj_dots_w = VGroup()
        proj_lines  = VGroup()
        proj_label_g  = VGroup()
        proj_label_w  = VGroup()

        for gy in range(N + 1):
            wy = (N - 1) - gy
            scene_y = G_OY - gy * CELL

            gpt = np.array([G_OX,         scene_y, 0])
            wpt = np.array([W_OX + TOTAL, scene_y, 0])

            proj_dots_g.add(Dot(gpt, radius=0.07, color=C_MAP_LINE))
            proj_dots_w.add(Dot(wpt, radius=0.07, color=C_MAP_LINE))
            proj_lines.add(DashedLine(gpt, wpt, color=C_MAP_LINE,
                                      stroke_width=1.8, dash_length=0.10))

            gl = MathTex(f"gy={gy}", font_size=24, color=C_MAP_LINE)
            gl.next_to(gpt, LEFT, buff=0.40)
            proj_label_g.add(gl)

            wl = MathTex(f"wy={wy}", font_size=24, color=C_MAP_LINE)
            wl.next_to(wpt, RIGHT, buff=0.40)
            proj_label_w.add(wl)

        proj_labels = VGroup(proj_label_g, proj_label_w)

        # ── Specific corner callouts ──────────────────────────────────────
        h_g00 = Dot(g_corner(0, 0), radius=0.08, color=C_ORIGIN)
        h_w04 = Dot(w_corner(0, N - 1), radius=0.08, color=C_ORIGIN)
        label_g00 = MathTex(r"\text{grid }(0,0)", font_size=25, color=C_ORIGIN)
        label_g00.next_to(h_g00, UL, buff=0.07)
        label_w04 = MathTex(r"\text{world }(0,4)", font_size=25, color=C_ORIGIN)
        label_w04.next_to(h_w04, UR, buff=0.07)

        h_g04 = Dot(g_corner(0, 4), radius=0.08, color=C_ARROW_X)
        h_w00 = Dot(w_corner(0, 0), radius=0.08, color=C_ARROW_X)
        label_g04 = MathTex(r"\text{grid }(0,4)", font_size=25, color=C_ARROW_X)
        label_g04.next_to(h_g04, DL, buff=0.07)
        label_w00_callout = MathTex(r"\text{world }(0,0)", font_size=25, color=C_ARROW_X)
        label_w00_callout.next_to(h_w00, DR, buff=0.07)

        h_gfrac = Dot(g_corner(0, 0.2), radius=0.08, color=C_ARROW_X)
        h_wfrac = Dot(w_corner(0, 3.8), radius=0.08, color=C_ARROW_X)
        label_gfrac = MathTex(r"\text{grid }(0,0.2)", font_size=25, color=C_ARROW_X)
        label_gfrac.next_to(h_gfrac, DL, buff=0.07)
        label_wfrac_callout = MathTex(r"\text{world }(0,3.8)", font_size=25, color=C_ARROW_X)
        label_wfrac_callout.next_to(h_wfrac, DR, buff=0.07)

        # ── Column highlight rects ─────────────────────────────────────────
        g_col_rect = Rectangle(width=CELL, height=TOTAL,
                                stroke_color=C_ARROW_Y, stroke_width=2,
                                fill_color=C_ARROW_Y, fill_opacity=0.15)
        g_col_rect.move_to(np.array([G_OX + CELL/2, G_OY - TOTAL/2, 0]))

        w_col_rect = Rectangle(width=CELL, height=TOTAL,
                                stroke_color=C_ARROW_Y, stroke_width=2,
                                fill_color=C_ARROW_Y, fill_opacity=0.15)
        w_col_rect.move_to(np.array([W_OX + CELL/2, w_top_y - TOTAL/2, 0]))

        # ── Formula ───────────────────────────────────────────────────────
        formula = MathTex(
            r"w_x = g_x \qquad w_y = (N-1) - g_y",
            font_size=32, color=WHITE)
        formula.move_to(np.array([0, G_OY - TOTAL - 0.8, 0]))

        # ═══════════════════════════════════════════════════════════════════
        # ANIMATION  (all run_times 2× slower than previous version)
        # ═══════════════════════════════════════════════════════════════════

        # Phase 1 – both grids
        self.play(Create(g_lines), Create(w_lines), run_time=2.8)
        self.play(Write(g_title), Write(w_title), run_time=1.4)
        self.wait(0.6)

        # Phase 2 – grid-space labels + axes (arrows from origin)
        self.play(
            LaggedStart(*[FadeIn(l) for l in g_labels], lag_ratio=0.03),
            GrowArrow(g_arrow_x), Write(g_label_x),
            GrowArrow(g_arrow_y), Write(g_label_y),
            run_time=3.2)
        self.wait(0.6)

        # Phase 3 – world-space labels + axes (arrows from their origins)
        self.play(
            LaggedStart(*[FadeIn(l) for l in w_labels], lag_ratio=0.03),
            GrowArrow(w_arrow_x), Write(w_label_x),
            GrowArrow(w_arrow_y), Write(w_label_y),
            run_time=3.2)
        self.wait(0.8)

        # Phase 4 – show grid origin (0,0)
        self.play(FadeIn(g_origin_dot), Write(g_origin_label), run_time=1.8)
        self.wait(1.0)

        # Phase 4b – show world origin (0,0) at top-left of bottom-left cell
        self.play(FadeIn(w_origin_dot), Write(w_origin_label), run_time=1.8)
        self.wait(1.0)

        # Phase 5 – x stays the same
        g_row0 = Rectangle(width=TOTAL, height=CELL,
                            stroke_color=C_ARROW_X, stroke_width=2,
                            fill_color=C_ARROW_X, fill_opacity=0.18)
        g_row0.move_to(np.array([G_OX + TOTAL/2, G_OY - CELL/2, 0]))
        w_row0 = Rectangle(width=TOTAL, height=CELL,
                            stroke_color=C_ARROW_X, stroke_width=2,
                            fill_color=C_ARROW_X, fill_opacity=0.18)
        w_row0.move_to(np.array([W_OX + TOTAL/2, w_top_y - CELL/2, 0]))
        x_note = Text("x stays the same", font_size=20, color=C_ARROW_X)
        x_note.move_to(np.array([0, G_OY + 0.9, 0]))

        self.play(FadeOut(g_arrow_y, g_arrow_x, g_label_x, g_label_y,
                          w_arrow_x, w_arrow_y, w_label_x, w_label_y))
        self.play(FadeIn(g_row0), FadeIn(w_row0), Write(x_note), run_time=1.8)
        self.wait(1.8)
        self.play(FadeOut(g_row0), FadeOut(w_row0), FadeOut(x_note), run_time=1.0)

        # Phase 6 – focus left column, dim labels
        y_note = Text("y flips", font_size=19, color=C_ARROW_Y)
        y_note.move_to(np.array([0, G_OY + 0.9, 0]))
        self.play(FadeIn(g_col_rect), FadeIn(w_col_rect),
                  Write(y_note), FadeOut(
                      w_origin_dot,
                      w_origin_label,
                      g_origin_dot,
                      g_origin_label), run_time=1.8)
        self.play(g_labels.animate.set_opacity(0.12),
                  w_labels.animate.set_opacity(0.12), run_time=0.8)
        self.wait(2)

        # Phase 7 – dots at each row boundary + world left-edge coords
        self.play(
            LaggedStart(*[FadeIn(d) for d in proj_dots_g], lag_ratio=0.15),
            LaggedStart(*[FadeIn(d) for d in proj_dots_w], lag_ratio=0.15),
            run_time=2.0)
        self.wait(0.4)

        # Phase 8 – show gy/wy labels first, trace lines, then hide all labels
        self.play(
            LaggedStart(*[FadeIn(l) for l in proj_label_g], lag_ratio=0.10),
            LaggedStart(*[FadeIn(l) for l in proj_label_w], lag_ratio=0.10),
            run_time=2.0)
        self.wait(0.6)
        self.play(
            LaggedStart(*[Create(l) for l in proj_lines], lag_ratio=0.15),
            run_time=3.6)
        self.wait(1.0)
        self.play(FadeOut(proj_label_g, proj_label_w), run_time=0.8)
        self.wait(0.4)

        self.play(Write(formula), run_time=2.2)
        self.wait(0.8)

        # Phase 9 – grid(0,0) ↔ world(0,4)
        # Remove BOTH origin dots first so highlights don't overlap them
        self.play(FadeIn(h_g00), Write(label_g00),
                  FadeIn(h_w04), Write(label_w04), run_time=1.8)
        self.wait(2.4)

        # Phase 10 – grid(0,4) ↔ world(0,0)
        self.play(FadeIn(h_g04), Write(label_g04),
                  FadeIn(h_w00), Write(label_w00_callout), run_time=1.8)
        self.wait(2.8)

        # Phase 10 – 
        self.play(FadeIn(h_gfrac), Write(label_gfrac),
                  FadeIn(h_wfrac), Write(label_wfrac_callout), run_time=1.8)
        self.wait(8)
