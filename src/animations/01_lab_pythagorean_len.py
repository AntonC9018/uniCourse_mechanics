from manim import *
from typing import cast
from helper import get_perpendicular_direction, set_default_output, create_component_animations_2d, setup_grid_and_camera, has_one_decimal_or_less
import numbers

class VelocityDecomposition(MovingCameraScene):
    def __init__(self, **kwargs):
        set_default_output("01_pythagorean_len")
        super().__init__(**kwargs)

    def construct(self):
        velocities = [
            np.array([3, 4, 0]),
            np.array([-2, 3, 0]),
            np.array([-3, -2, 0]),
        ]
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

        start_point = grid.coords_to_point(*start_pos)
        target_point = grid.coords_to_point(*end_pos)
        velocity_vector = Arrow(
            start=start_point, 
            end=target_point, 
            buff=0, 
            color=RED, 
            stroke_width=6
        )
        velocity_text = MathTex(r"\vec{v}", color=RED).scale(0.8)
        perp_direction = get_perpendicular_direction(velocity)
        velocity_text.add_updater(lambda x: x.next_to(velocity_vector.get_center(), perp_direction))

        component_anims = create_component_animations_2d(start_point, target_point, end_pos)

        vec_v_str = r"\left|\vec{v}\right|"
        def get_formula_text(x, y):
            def to_str(n):
                s = n;
                if isinstance(n, float):
                    s = f"{n:.1}"
                elif isinstance(n, int):
                    s = f"{n}"
                if isinstance(n, numbers.Number) and n < 0: # type: ignore
                    s = f"({s})"
                return s

            x = to_str(x)
            y = to_str(y)

            ret = MathTex(vec_v_str, "=", r"\sqrt{", x, "^2", "+", y, "^2", "}", color=YELLOW)
            # not searching, because it might be the same
            ret[0].set_color(RED)
            ret[3].set_color(BLUE) # x
            ret[6].set_color(BLUE) # y
            return ret

        camera = cast(MovingCamera, self.camera)
        f = camera.frame

        formula_text_1 = get_formula_text('x', 'y')
        formula_text_1.next_to(f.get_top(), DOWN, buff=0.2)

        formulas = FormulasHelper(formula_text_1)
        formula_text_2 = get_formula_text(end_pos[0], 'y') 
        formulas.add(formula_text_2)
        formulas.add(get_formula_text(end_pos[0], end_pos[1]))

        vector_size = np.linalg.norm(end_pos)
        equality = r'\approx'
        if has_one_decimal_or_less(vector_size):
            equality = '='

        def xy():
            ret = MathTex(vec_v_str, "=", r"\sqrt{", f"{end_pos[0] ** 2:.1f}", "+", f"{end_pos[1] ** 2:.1f}", "}", color=YELLOW)
            ret[0].set_color(RED)
            ret[3].set_color(BLUE)
            ret[5].set_color(BLUE)
            return ret;
        formulas.add(xy())
        def f1():
            ret = MathTex(vec_v_str, "=", r"\sqrt{", f"{vector_size * vector_size:.1f}", "}", color=YELLOW)
            ret[0].set_color(RED)
            ret[3].set_color(BLUE)
            return ret
        formulas.add(f1())
        def f2():
            ret = MathTex(vec_v_str, equality, f"{vector_size:.1f}", color=YELLOW)
            ret[0].set_color(RED)
            ret[2].set_color(BLUE)
            return ret
        formulas.add(f2())

        frame_x = SurroundingRectangle(component_anims.x().label[0], color=RED)
        frame_y = SurroundingRectangle(component_anims.y().label[0], color=RED)
        frame_x_formula = SurroundingRectangle(formula_text_1.get_part_by_tex('x'), color=RED) # type: ignore
        frame_y_formula = SurroundingRectangle(formula_text_2.get_part_by_tex('y'), color=RED) # type: ignore

        self.add(velocity_text)
        self.play(GrowArrow(velocity_vector))
        self.wait(0.5)

        self.play(formulas.next())

        component_anims.play(self)

        self.play(Create(frame_x))
        self.wait(0.3)
        self.play(ReplacementTransform(frame_x, frame_x_formula))
        self.play(FadeOut(frame_x_formula))
        self.play(formulas.next())

        self.wait(0.2)

        self.play(Create(frame_y))
        self.wait(0.3)
        self.play(ReplacementTransform(frame_y, frame_y_formula))
        self.play(FadeOut(frame_y_formula))
        self.play(formulas.next())
        self.wait(0.3)
        self.play(formulas.next())
        self.wait(0.3)
        self.play(formulas.next())
        self.wait(0.3)
        self.play(formulas.next())

        self.wait(2)
        self.clear()

class FormulasHelper:
    def __init__(self, first_formula):
        self.formulas = [first_formula]
        self.index = 0

    def add(self, f):
        f.move_to(self.formulas[0])
        self.formulas.append(f)

    def next(self):
        f = self.formulas[self.index]
        if self.index == 0:
            ret = Write(f)
        else:
            ret = TransformMatchingTex(self.formulas[self.index - 1], f)
        self.index += 1
        return ret
