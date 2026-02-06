#!/bin/bash
set -e

source manim_env/bin/activate

echo "🎬 Rendering GIF..."
manim -qm -i $1

echo "✨ Done! Check the 'media' folder for your GIF."
