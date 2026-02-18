from manim import *
from helper import set_default_output

GRID_COLS = 5
GRID_ROWS = 5
CELL_SIZE = 1.2  # visual size of each cell in Manim units

# Colors
GRID_COLOR      = BLUE_E
LABEL_COLOR     = WHITE
HIGHLIGHT_COLOR = YELLOW
AXIS_X_COLOR    = GREEN
AXIS_Y_COLOR    = RED
SIZE_COLOR      = ORANGE


def cell_center(col, row):
    """Return the Manim scene position of cell (col, row).
    Top-left cell is (0,0); x grows right, y grows DOWN.
    We flip y so the grid renders top-to-bottom correctly."""
    x = col * CELL_SIZE - (GRID_COLS - 1) * CELL_SIZE / 2
    y = -row * CELL_SIZE + (GRID_ROWS - 1) * CELL_SIZE / 2
    return np.array([x, y, 0])


# claude wrote most of this
class GridCoords(MovingCameraScene):
    def __init__(self, **kwargs):
        set_default_output("02_grid_coords")
        super().__init__(**kwargs)

    def construct(self):

        # ── 1. BUILD THE GRID ─────────────────────────────────────────────
        grid_lines = VGroup()
        total_w = GRID_COLS * CELL_SIZE
        total_h = GRID_ROWS * CELL_SIZE
        ox = -(GRID_COLS * CELL_SIZE) / 2
        oy =  (GRID_ROWS * CELL_SIZE) / 2

        # vertical lines
        for c in range(GRID_COLS + 1):
            x = ox + c * CELL_SIZE
            grid_lines.add(
                Line(np.array([x, oy, 0]),
                     np.array([x, oy - total_h, 0]),
                     color=GRID_COLOR, stroke_width=2))
        # horizontal lines
        for r in range(GRID_ROWS + 1):
            y = oy - r * CELL_SIZE
            grid_lines.add(
                Line(np.array([ox, y, 0]),
                     np.array([ox + total_w, y, 0]),
                     color=GRID_COLOR, stroke_width=2))

        # ── 2. COORDINATE LABELS (all cells) ─────────────────────────────
        coord_labels = VGroup()
        for row in range(GRID_ROWS):
            for col in range(GRID_COLS):
                lbl = Text(f"({col},{row})", font_size=18, color=LABEL_COLOR)
                lbl.move_to(cell_center(col, row))
                coord_labels.add(lbl)

        # ── 3. AXIS ARROWS & LABELS ───────────────────────────────────────
        arrow_gap = 0

        # x arrow runs along the TOP edge, pointing right
        x_arrow_start = np.array([ox,          oy + arrow_gap, 0])
        x_arrow_end   = np.array([ox + total_w, oy + arrow_gap, 0])
        arrow_x = Arrow(x_arrow_start, x_arrow_end,
                        buff=0, color=AXIS_X_COLOR, stroke_width=4,
                        max_tip_length_to_length_ratio=0.08)
        label_x = Text("x", font_size=26, color=AXIS_X_COLOR)
        label_x.next_to(arrow_x, RIGHT, buff=0.15)

        # y arrow runs along the LEFT edge, pointing down
        y_arrow_start = np.array([ox - arrow_gap, oy,           0])
        y_arrow_end   = np.array([ox - arrow_gap, oy - total_h, 0])
        arrow_y = Arrow(y_arrow_start, y_arrow_end,
                        buff=0, color=AXIS_Y_COLOR, stroke_width=4,
                        max_tip_length_to_length_ratio=0.08)
        label_y = Text("y", font_size=26, color=AXIS_Y_COLOR)
        label_y.next_to(arrow_y, LEFT, buff=0.15)

        # ── 4. "SIZE = 1" BRACE ───────────────────────────────────────────
        # Show on the top edge of the grid (first column)
        p_left  = cell_center(0, 0) + LEFT  * CELL_SIZE / 2 + UP * CELL_SIZE / 2
        p_right = cell_center(0, 0) + RIGHT * CELL_SIZE / 2 + UP * CELL_SIZE / 2
        brace = BraceBetweenPoints(p_left, p_right, direction=UP, color=SIZE_COLOR)
        brace_lbl = brace.get_text("1", buff=0.1)
        brace_lbl.set_color(SIZE_COLOR).scale(0.8)

        # ── ANIMATION SEQUENCE ────────────────────────────────────────────

        # Step 1 – draw the grid
        self.add(grid_lines)
        self.wait(0.4)

        # Step 2 – show origin label (0,0) first
        origin_lbl = coord_labels[0]   # row=0, col=0 → index 0
        self.play(FadeIn(origin_lbl, scale=1.3), run_time=0.6)
        self.wait(0.3)

        # Step 3 – show axis arrows so viewer understands direction
        self.play(
            GrowArrow(arrow_x), Write(label_x),
            GrowArrow(arrow_y), Write(label_y),
            run_time=1.2)
        self.wait(0.5)

        # Step 4 – reveal all other coordinate labels
        rest_labels = VGroup(*[lbl for lbl in coord_labels if lbl is not origin_lbl])
        self.play(LaggedStart(
            *[FadeIn(lbl, scale=0.8) for lbl in rest_labels],
            lag_ratio=0.04), run_time=2)
        self.wait(0.5)

        # Step 5 – fade out arrows, then show the cell-size brace on top
        # self.play(FadeOut(arrow_x, label_x, arrow_y, label_y), run_time=0.6)
        self.play(Create(brace), Write(brace_lbl), run_time=1)
        self.wait(4)
