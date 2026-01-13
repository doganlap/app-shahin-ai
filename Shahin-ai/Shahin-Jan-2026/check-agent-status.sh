#!/bin/bash
# Check AI Agent Service Status

echo "=== AI Agent Service Status ==="
echo ""

# Check Claude Agent
echo "Claude AI Agent:"
if [ -z "$CLAUDE_API_KEY" ]; then
    echo "  Status: ❌ OFF (No API key configured)"
    echo "  Message: Set CLAUDE_API_KEY environment variable to enable"
else
    echo "  Status: ✅ ON (API key configured)"
    echo "  Model: ${CLAUDE_MODEL:-claude-sonnet-4-20250514}"
fi

echo ""
echo "Microsoft Copilot Agent:"
if [ -z "$COPILOT_CLIENT_ID" ]; then
    echo "  Status: ❌ OFF (No Client ID configured)"
    echo "  Message: Set COPILOT_CLIENT_ID environment variable to enable"
else
    echo "  Status: ✅ ON (Client ID configured)"
fi

echo ""
echo "To enable Claude Agent:"
echo "  export CLAUDE_API_KEY='your-api-key-here'"
echo ""
echo "To enable Copilot Agent:"
echo "  export COPILOT_CLIENT_ID='your-client-id-here'"
echo "  export COPILOT_CLIENT_SECRET='your-client-secret-here'"
echo "  export AZURE_TENANT_ID='your-tenant-id-here'"
