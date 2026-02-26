#!/bin/bash
echo "Creating STING Tools CompiledPlugin..."
mkdir -p CompiledPlugin
cd CompiledPlugin
base64 -d << 'B64EOF' | tar xz
