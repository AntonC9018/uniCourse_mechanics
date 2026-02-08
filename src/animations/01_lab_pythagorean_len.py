from manim import *
import helper
import enemy_sight

class DotProductDetection(Scene):
    def __init__(self, **kwargs):
        helper.set_default_output("01_lab_dot_product_explanation.gif")
        super().__init__(**kwargs)

    def construct(self):
        # Create context with all objects
        ctx = enemy_sight.Context()
        
        # Add labels for enemy and player
        enemy_label = MathTex("E", color=RED).next_to(ctx.enemy.obj)
        player_label = MathTex("P", color=GREEN)
        
        def update_player_label(label):
            label.next_to(ctx.player.obj)
        player_label.add_updater(update_player_label)
        
        # Enemy look direction arrow (normalized) - using LabeledArrow
        look_arrow = LabeledArrow(
            start=ORIGIN,
            end=UP * 2,
            label_tex=r"\vec{d}",
            color=BLUE)
        
        def update_look_direction(arrow):
            enemy_dir = ctx.enemy.direction()
            new_end = ORIGIN + 2 * enemy_dir
            arrow.put_start_and_end_on(ORIGIN, new_end)
        look_arrow.add_updater(update_look_direction)
        
        # Vector from enemy to player - using LabeledArrow
        to_player_arrow = LabeledArrow(
            start=ORIGIN,
            end=ctx.player.obj.get_center(),
            label_tex=r"\vec{v}",
            color=YELLOW)
        
        def update_to_player_vec(arrow):
            arrow.put_start_and_end_on(ORIGIN, ctx.player.obj.get_center())
            arrow.set_color(YELLOW)
        to_player_arrow.add_updater(update_to_player_vec)
        
        # Angle arc
        angle_arc = Arc(
            radius=0.8,
            start_angle=np.pi/2,
            angle=0,
            color=WHITE,
            stroke_width=3)
        angle_label = MathTex(r"\alpha", color=WHITE).scale(0.8)
        
        def update_angle_arc(arc):
            enemy_dir = ctx.enemy.direction()
            player_pos = ctx.player.obj.get_center()
            
            if np.linalg.norm(player_pos) > 0.01:
                player_dir = player_pos / np.linalg.norm(player_pos)
                
                # Calculate angles
                enemy_angle = helper.get_vector_angle_2d(enemy_dir)
                player_angle = helper.get_vector_angle_2d(player_dir)
                # Fix: angle should go from player to enemy (clockwise)
                angle_between = enemy_angle - player_angle
                
                arc.become(Arc(
                    radius=0.8,
                    start_angle=player_angle,
                    angle=angle_between,
                    color=WHITE,
                    stroke_width=3))
        angle_arc.add_updater(update_angle_arc)
        
        def update_angle_label(label):
            enemy_dir = ctx.enemy.direction()
            player_pos = ctx.player.obj.get_center()
            
            if np.linalg.norm(player_pos) > 0.01:
                player_dir = player_pos / np.linalg.norm(player_pos)
                mid_dir = (enemy_dir + player_dir) / 2
                if np.linalg.norm(mid_dir) > 0.01:
                    mid_dir = mid_dir / np.linalg.norm(mid_dir)
                    label.move_to(0.5 * mid_dir)
        angle_label.add_updater(update_angle_label)
        
        # Formula display
        def create_formula(tex):
            ret = MathTex(tex)
            ret.to_edge(DOWN).shift(UP * 0.5)
            return ret

        formula_1 = create_formula(r"\vec{v} = P - E")
        formula_2 = create_formula(r"\vec{d} \cdot \vec{v} = |\vec{d}| |\vec{v}| \cos(\alpha)")
        formula_3 = create_formula(r"\cos(\alpha) = \frac{\vec{d} \cdot \vec{v}}{|\vec{v}|}")
        formula_4 = create_formula(r"\text{Detected: } \cos(\alpha) \geq \cos(\beta/2)")
        
        # Animation sequence
        self.play(
            FadeIn(ctx.enemy.obj),
            FadeIn(enemy_label))
        self.wait(0.8)
        
        # Show look direction
        self.play(
            *[GrowArrow(mob) if isinstance(mob, Arrow) else FadeIn(mob) 
              for mob in look_arrow.get_mobjects()])
        self.wait(0.8)
        
        # Show detection cone
        self.play(FadeIn(ctx.detection.cone))
        self.wait(0.8)
        
        # Show player
        self.play(
            FadeIn(ctx.player.obj),
            FadeIn(player_label))
        self.wait(0.8)
        
        # Show vector from E to P
        self.play(Write(formula_1), run_time=1.5)
        self.wait(0.5)
        self.play(
            *[GrowArrow(mob) if isinstance(mob, Arrow) else FadeIn(mob) 
              for mob in to_player_arrow.get_mobjects()],
            run_time=1.2)
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
