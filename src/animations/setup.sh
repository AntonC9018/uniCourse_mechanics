#!/bin/bash
set -e

# 1. Check for system dependencies (FFmpeg is strictly required)
if ! command -v ffmpeg &> /dev/null
then
    echo "❌ Error: FFmpeg could not be found."
    echo "   Please install it first:"
    echo "   - macOS: brew install ffmpeg"
    echo "   - Linux: sudo apt install ffmpeg"
    exit 1
fi
echo "✅ FFmpeg found."

# 2. Install system dependencies (Linux only)
if [[ "$OSTYPE" == "linux-gnu"* ]]; then
    echo "📦 Installing system dependencies..."
    echo "   (You may be prompted for your password)"
    sudo apt install -y libcairo2-dev libpango1.0-dev ffmpeg pkg-config python3-dev
    echo "✅ System dependencies installed."
fi

sudo apt install python3-venv

# 3. Create and Activate Virtual Environment
VENV_NAME="manim_env"
if [ ! -d "$VENV_NAME" ]; then
    echo "📦 Creating virtual environment '$VENV_NAME'..."
    python3 -m venv $VENV_NAME
else
    echo "📦 Virtual environment '$VENV_NAME' already exists."
fi

echo "🔌 Activating virtual environment..."
source $VENV_NAME/bin/activate

# 4. Install Manim
echo "⬇️  Installing Manim (this may take a minute)..."
pip install --upgrade pip
pip install manim

# 5. Verify installation
echo ""
echo "✅ Installation complete!"
echo "📍 Manim version: $(manim --version)"
echo ""
echo "To use Manim:"
echo "  1. Activate the environment: source $VENV_NAME/bin/activate"
echo "  2. Run your animations: manim -pql scene.py SceneName"
