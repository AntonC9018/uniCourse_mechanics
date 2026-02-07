from manim import *
import helper
import enemy_sight

class EnemyDetection(Scene):
    def __init__(self, **kwargs):
        helper.set_default_output("01_lab_enemy_sight.gif")
        super().__init__(**kwargs)

    def construct(self):
        # Create context with all objects
        ctx = enemy_sight.Context()
        
        # Add all objects to scene
        ctx.add_to(self)
        self.wait(0.8)
        
        # 2. Player in zone
        self.play(ctx.player.obj.animate.move_to(UP * 2), run_time=2)
        self.wait(0.8)
        
        # 3. Player not in zone behind
        self.play(ctx.player.obj.animate.move_to(DOWN * 2.5), run_time=2.5)
        self.wait(0.8)
        
        # 4. Enemy rotates to face the player
        self.play(ctx.enemy.angle.animate.set_value(270 * DEGREES), run_time=3, rate_func=smooth)
        self.wait(0.8)
        
        # 5. Player moves away (to the right)
        self.play(ctx.player.obj.animate.move_to(RIGHT * 3 + DOWN * 1), run_time=2)
        self.wait(0.8)
        
        # 6. Enemy faces the other way that the player went and misses it
        self.play(ctx.enemy.angle.animate.set_value(180 * DEGREES), run_time=3, rate_func=smooth)
        self.wait(1)
        
        self.wait(1)


