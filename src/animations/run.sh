#!/bin/bash
set -e

source manim_env/bin/activate

echo "🎬 Rendering GIF..."
manim -qm -i 01_lab_velocity_decompose.py

echo "✨ Done! Check the 'media' folder for your GIF."
