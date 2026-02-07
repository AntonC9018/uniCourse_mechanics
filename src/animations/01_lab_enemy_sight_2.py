from manim import *
import helper
import enemy_sight

class LArrow:
    """Arrow with a label that stays perpendicular to it at the center"""
    def __init__(self, start, end, label, color, stroke_width=6):
        self.arrow = Arrow(
            start=start,
            end=end,
            buff=0,
            color=color,
            stroke_width=stroke_width,
            max_tip_length_to_length_ratio=0.15)
        
        self.label = MathTex(label, color=color).scale(0.8)
        
        # Calculate initial perpendicular direction
        self._update_label_position()
    
    def _update_label_position(self):
        """Update label to be perpendicular to arrow at its center"""
        arrow_vec = self.arrow.get_end() - self.arrow.get_start()
        
        if np.linalg.norm(arrow_vec[:2]) > 0.01:
            # Get 2D perpendicular (rotate 90 degrees counterclockwise)
            perp = np.array([-arrow_vec[1], arrow_vec[0], 0])
            perp = perp / np.linalg.norm(perp) * 0.3  # Distance from arrow
            
            center = self.arrow.get_center()
            self.label.move_to(center + perp)
    
    def add_updater(self, update_func):
        """Add updater to the arrow, label follows automatically"""
        def combined_updater(mob):
            update_func(mob)
            self._update_label_position()
        
        self.arrow.add_updater(combined_updater)
    
    def play(self, scene):
        scene.play(GrowArrow(self.arrow), Write(self.label))


class DotProductDetection(Scene):
    def __init__(self, **kwargs):
        helper.set_default_output("01_lab_dot_product_explanation.gif")
        super().__init__(**kwargs)

    def construct(self):
        # Create context with all objects
        ctx = enemy_sight.Context(angle_deg=70)
        
        beta_label = MathTex(r"\beta")
        def update_beta(beta):
            beta.move_to()
        beta_label.add_updater(update_beta)
        
        # Add labels for enemy and player
        enemy_label = MathTex("E", color=RED).next_to(ctx.enemy.obj)
        player_label = MathTex("P", color=GREEN)
        
        def update_player_label(label):
            label.next_to(ctx.player.obj)
        player_label.add_updater(update_player_label)
        
        # Enemy look direction arrow (normalized) - using LArrow
        look_arrow = LArrow(
            label=r"\hat{d}",
            start=ORIGIN,
            end=UP * 2,
            color=BLUE)
        
        def update_look_direction(arrow):
            enemy_dir = ctx.enemy.direction()
            new_end = ORIGIN + 2 * enemy_dir
            arrow.put_start_and_end_on(ORIGIN, new_end)
        look_arrow.add_updater(update_look_direction)
        
        # Vector from enemy to player - using LArrow
        to_player_arrow = LArrow(
            label=r"\vec{v}",
            start=ORIGIN,
            end=ctx.player.obj.get_center(),
            color=YELLOW)
        
        def update_to_player_vec(arrow):
            arrow.put_start_and_end_on(ORIGIN, ctx.player.obj.get_center())
            arrow.set_color(YELLOW)
        to_player_arrow.add_updater(update_to_player_vec)
        

        def create_alpha_arc():
            enemy_dir = ctx.enemy.direction()
            player_pos = ctx.player.obj.get_center()
            
            if np.linalg.norm(player_pos) <= 0.01:
                return None

            player_dir = player_pos / np.linalg.norm(player_pos)
            
            enemy_angle = helper.get_vector_angle_2d(enemy_dir)
            player_angle = helper.get_vector_angle_2d(player_dir)
            angle_between = enemy_angle - player_angle
            if angle_between > np.pi:
                angle_between -= np.pi * 2
            elif angle_between < -np.pi:
                angle_between += np.pi * 2
            
            return Arc(
                radius=0.8,
                start_angle=player_angle,
                angle=angle_between,
                color=WHITE,
                stroke_width=3,
                arc_center=ctx.enemy.obj.get_center())
        # Angle arc
        angle_arc = create_alpha_arc()
        assert angle_arc is not None
        angle_label = MathTex(r"\alpha", color=WHITE).scale(0.8)
        
        def update_angle_arc(arc):
            new_arc = create_alpha_arc()
            if new_arc == None:
                return
            arc.become(new_arc)
        angle_arc.add_updater(update_angle_arc)
        
        def update_angle_label(label):
            enemy_dir = ctx.enemy.direction()
            player_dir = ctx.player.obj.get_center() - ctx.enemy.obj.get_center()
            
            if np.linalg.norm(player_dir) <= 0.01:
                return

            player_dir = player_dir / np.linalg.norm(player_dir)
            mid_dir = (enemy_dir + player_dir) / 2

            if np.linalg.norm(mid_dir) <= 0.01:
                return

            mid_dir = mid_dir / np.linalg.norm(mid_dir)
            label.move_to(0.5 * mid_dir + ctx.enemy.obj.get_center())

        angle_label.add_updater(update_angle_label)

        def create_beta_arc():
            radius = 2
            def create_arc():
                angle = ctx.enemy.angle.get_value()
                beta = ctx.detection.angle.get_value()
                arc = Arc(
                    radius=radius,
                    start_angle=angle-beta/2,
                    angle=beta,
                    color=WHITE,
                    stroke_width=3,
                    arc_center=ctx.enemy.obj.get_center())
                # arc.move_to(ctx.enemy.obj.get_center())
                return arc
            def enemy(beta_arc):
                beta_arc.become(create_arc())
            beta_arc = create_arc()
            beta_arc.add_updater(enemy)

            beta_text = MathTex(r"\beta")
            def pos(beta_text):
                beta_text.move_to(ctx.enemy.obj.get_center() + (radius + 0.5) * ctx.enemy.direction())
            beta_text.add_updater(pos)
            pos(beta_text)

            return beta_arc, beta_text

        beta_arc, beta_text = create_beta_arc()
        
        # Formula display
        def create_formula(*tex):
            ret = MathTex(*tex)
            ret.to_edge(DOWN).shift(UP * 0.5)
            return ret

        f_dot = r"\hat{d} \cdot \vec{v}"
        vec_d = r"|\hat{d}|"
        vec_v = r"|\vec{v}|"
        cos_alpha = r"\cos(\alpha)"
        formula_1 = create_formula(r"\vec{v} = P - E")
        formula_2 = create_formula(f_dot, "=", vec_d, vec_v, cos_alpha)
        formula_3 = create_formula(cos_alpha, "=", r"\frac{"+f_dot+"}"+"{"+vec_v+"}")
        formula_4 = create_formula(cos_alpha, r"\geq", r"\cos(\beta / 2)")
        
        # Animation sequence
        self.play(
            FadeIn(ctx.enemy.obj),
            FadeIn(enemy_label))
        self.wait(0.8)
        
        # Show look direction
        look_arrow.play(self)
        self.wait(0.8)
        
        # Show detection cone
        self.play(FadeIn(ctx.detection.cone), Create(beta_arc), FadeIn(beta_text))
        self.wait(0.8)
        
        # Show player
        self.play(
            FadeIn(ctx.player.obj),
            FadeIn(player_label))
        self.wait(0.8)
        
        # Show vector from E to P
        self.play(Write(formula_1), run_time=1.5)
        self.wait(0.5)
        to_player_arrow.play(self)
        self.wait(1.5)
        
        # Show angle
        self.play(
            Create(angle_arc),
            FadeIn(angle_label),
            run_time=1.2)
        self.wait(1.5)
        
        # Show dot product formula - slower
        self.play(Transform(formula_1, formula_2), run_time=2.5)
        self.wait(2)
        
        # Show cos(alpha) isolation - slower
        self.play(Transform(formula_1, formula_3), run_time=2.5)
        self.wait(2)
        
        # Show detection condition - slower
        self.play(Transform(formula_1, formula_4), run_time=2.5)
        self.wait(1.5)
        
        # Demonstrate: move player inside detection zone
        new_pos_inside = UP * 1.8 + RIGHT * 0.3
        self.play(
            ctx.player.obj.animate.move_to(new_pos_inside),
            run_time=2.5)
        self.wait(2)
        
        # Move player outside detection zone
        new_pos_outside = LEFT * 2 + UP * 1
        self.play(
            ctx.player.obj.animate.move_to(new_pos_outside),
            run_time=2.5)
        self.wait(2)
        
        # Rotate enemy to show dynamic detection
        self.play(
            ctx.enemy.angle.animate.set_value(np.radians(180)),
            run_time=4,
            rate_func=smooth)
        self.wait(2)
        
        self.wait(1)
