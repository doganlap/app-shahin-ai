#!/bin/bash
# DNS Records Verification Script for shahin-ai.com
# Tests MX, SPF, DKIM, and DMARC records

echo "╔════════════════════════════════════════════════════════════════╗"
echo "║     DNS Records Verification for shahin-ai.com                ║"
echo "╚════════════════════════════════════════════════════════════════╝"
echo ""

DOMAIN="shahin-ai.com"

echo "⏳ Waiting for DNS propagation (15 seconds)..."
sleep 15
echo ""

# Check MX Records
echo "📧 Checking MX Records..."
echo "────────────────────────────────────────────────────────────────"
dig +short ${DOMAIN} MX
if [ $? -eq 0 ]; then
    echo "✅ MX records found"
else
    echo "❌ No MX records found"
fi
echo ""

# Check SPF Record
echo "🔒 Checking SPF Record..."
echo "────────────────────────────────────────────────────────────────"
SPF=$(dig +short ${DOMAIN} TXT | grep -i "v=spf1")
if [ -n "$SPF" ]; then
    echo "✅ SPF record found:"
    echo "$SPF"
else
    echo "❌ No SPF record found"
fi
echo ""

# Check DMARC Record
echo "🛡️  Checking DMARC Record..."
echo "────────────────────────────────────────────────────────────────"
DMARC=$(dig +short _dmarc.${DOMAIN} TXT | grep -i "v=DMARC1")
if [ -n "$DMARC" ]; then
    echo "✅ DMARC record found:"
    echo "$DMARC"
else
    echo "❌ No DMARC record found"
fi
echo ""

# Check DKIM Record (if exists)
echo "🔐 Checking DKIM Record (if configured)..."
echo "────────────────────────────────────────────────────────────────"
DKIM=$(dig +short google._domainkey.${DOMAIN} TXT 2>/dev/null | grep -i "v=DKIM1")
if [ -n "$DKIM" ]; then
    echo "✅ DKIM record found (google._domainkey)"
else
    echo "ℹ️  No DKIM record found (this is optional)"
fi
echo ""

# Overall Status
echo "╔════════════════════════════════════════════════════════════════╗"
echo "║                    Verification Complete                       ║"
echo "╚════════════════════════════════════════════════════════════════╝"
echo ""
echo "📝 Next Steps:"
echo "   1. If records are not showing, wait 5-15 more minutes"
echo "   2. Verify in Cloudflare dashboard that all records show Proxy = OFF"
echo "   3. Test email deliverability at: https://www.mail-tester.com"
echo "   4. Check DMARC reports at: dmarc@shahin-ai.com (after emails are sent)"
echo ""
