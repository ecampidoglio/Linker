#!/usr/bin/env bash
########################################################################################
# This is a Cake bootstrapper script for Linux and macOS using .NET Core 2.1
########################################################################################

SCRIPT_DIR=$( cd "$( dirname "${BASH_SOURCE[0]}" )" && pwd )
CAKE_VERSION=0.33.0
TOOLS_DIR=$SCRIPT_DIR/tools
CAKE_TOOL=$TOOLS_DIR/dotnet-cake

# Make sure the tools folder exist
if [ ! -d "$TOOLS_DIR" ]; then
  mkdir "$TOOLS_DIR"
fi

# Install the Cake Tool locally
if [ ! -f "$CAKE_TOOL" ]; then
    echo -e "\033[92mInstalling 'cake.tool' $CAKE_VERSION in '$TOOLS_DIR'\033[0m"
    dotnet tool install Cake.Tool --version $CAKE_VERSION --tool-path $TOOLS_DIR
fi

# Run the build script
exec "$CAKE_TOOL" "$@"
