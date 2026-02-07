from manim import *
import helper

class Context:
    def __init__(self, angle_deg=40):
        self.enemy = Enemy(direction=UP)
        self.detection = FOV(angle_deg=angle_deg, radius=10)
        self.player = Player(position=LEFT * 3 + DOWN * 1)
        self.enemy.attach_fov(self.detection)
        self.player.set_detection_fov(self.detection, self)

    # DETECTION LOGIC using dot product
    def is_player_detected(self):
        # Get positions
        enemy_pos = self.enemy.obj.get_center()
        player_pos = self.player.obj.get_center()
        
        # Vector from enemy to player
        to_player = player_pos - enemy_pos
        if np.linalg.norm(to_player) < 0.01:  # Avoid division by zero
            return False
        to_player_normalized = to_player / np.linalg.norm(to_player)
        enemy_look_dir = self.enemy.direction()
        dot = np.dot(to_player_normalized, enemy_look_dir)
        
        # Threshold from detection angle (angle stored in radians)
        half_angle_rad = self.detection.angle.get_value() / 2
        threshold = np.cos(half_angle_rad)
        
        return dot >= threshold

    def add_to(self, scene):
        scene.add(self.detection.cone)
        scene.add(self.enemy.obj)
        scene.add(self.player.obj)


class FOV:
    def __init__(self, angle_deg, radius):
        # Convert input degrees to radians, store in radians
        self.angle = ValueTracker(np.radians(angle_deg))
        self.radius = radius
        # angle is already in radians
        self.cone = Sector(
            radius=self.radius,
            angle=self.angle.get_value(),
            start_angle=np.pi/2 - self.angle.get_value() / 2,
            color=YELLOW,
            fill_opacity=0.3,
            stroke_width=0)


class Player:
    def __init__(self, position):
        # Create a player character (GREEN)
        self.obj = Circle(radius=0.2, color=GREEN, fill_opacity=1)
        self.obj.move_to(position)

    def set_detection_fov(self, detection: FOV, context: 'Context'):
        # Player updater for detection
        def update_player_detection(p):
            if context.is_player_detected():
                p.set_color(ORANGE)
                detection.cone.set_fill(opacity=0.6)
            else:
                p.set_color(GREEN)
                detection.cone.set_fill(opacity=0.3)
        self.obj.add_updater(update_player_detection)


class Enemy:
    def __init__(self, direction: np.ndarray):
        # helper.get_vector_angle_2d returns radians, store in radians
        a: float = helper.get_vector_angle_2d(direction)
        self.angle = ValueTracker(a)
        self.obj = Circle(radius=0.3, color=RED, fill_opacity=1)
        self.obj.move_to(ORIGIN)

    def direction_2d(self):
        # helper.get_vector_from_angle_2d expects radians
        return helper.get_vector_from_angle_2d(self.angle.get_value())

    def direction(self):
        dir_2d = self.direction_2d()
        return np.array([dir_2d[0], dir_2d[1], 0])

    def attach_fov(self, fov: FOV):
        # Update detection cone based on enemy angle and detection angle
        def update_detection_cone(cone):
            angle_rad = fov.angle.get_value()
            enemy_dir_rad = self.angle.get_value()
            # Preserve current fill opacity
            current_opacity = cone.fill_opacity
            # Everything is in radians
            sect = Sector(
                radius=fov.radius,
                angle=angle_rad,
                start_angle=enemy_dir_rad - angle_rad/2,
                color=YELLOW,
                fill_opacity=current_opacity,
                stroke_width=0)
            cone.become(sect)
        fov.cone.add_updater(update_detection_cone)
