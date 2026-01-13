#!/bin/bash
# Load NVM and set Node.js 24 as default
export NVM_DIR="$HOME/.nvm"
[ -s "$NVM_DIR/nvm.sh" ] && \. "$NVM_DIR/nvm.sh"
[ -s "$NVM_DIR/bash_completion" ] && \. "$NVM_DIR/bash_completion"

# Set Node.js 24 as active version
nvm use 24 2>/dev/null || true

echo "Node.js environment loaded:"
echo "Node.js: $(node --version)"
echo "NPM: $(npm --version)"
echo "Yarn: $(yarn --version)"