# 🔑 Fix Claude Code Credentials Error

**Error:** "Claude Code credentials not found. Please authenticate to see usage data."

---

## 🔍 Problem

The Claude API key is not configured, so Claude AI features cannot work.

---

## ✅ Solution

### Step 1: Get Your Claude API Key

1. **Visit Anthropic Console:**
   - Go to: https://console.anthropic.com/
   - Sign in or create an account

2. **Create API Key:**
   - Navigate to **API Keys** section
   - Click **"Create Key"**
   - Copy the key (starts with `sk-ant-api03-`)

---

### Step 2: Add to Environment Variables

**Option 1: Add to .env file (Recommended)**

```bash
# Edit .env file
cd /home/Shahin-ai/Shahin-Jan-2026
nano .env  # or use your preferred editor
```

Add or update:
```bash
CLAUDE_ENABLED=true
CLAUDE_API_KEY=sk-ant-api03-your-actual-key-here
CLAUDE_MODEL=claude-sonnet-4-5-20250514
CLAUDE_MAX_TOKENS=4096
```

**Option 2: Set Environment Variable (Temporary)**

```bash
export CLAUDE_API_KEY=sk-ant-api03-your-actual-key-here
export CLAUDE_ENABLED=true
```

**Option 3: Docker Compose Environment**

If using Docker, add to `docker-compose.yml`:
```yaml
environment:
  - CLAUDE_ENABLED=true
  - CLAUDE_API_KEY=sk-ant-api03-your-actual-key-here
  - CLAUDE_MODEL=claude-sonnet-4-5-20250514
```

---

### Step 3: Restart Application

**If running locally:**
```bash
# Stop application (Ctrl+C)
# Then restart
cd src/GrcMvc
dotnet run
```

**If using Docker:**
```bash
docker-compose restart grcmvc
# Or
docker-compose up -d grcmvc
```

---

## ✅ Verification

### Check if Key is Loaded:

```bash
# Check environment variable
env | grep CLAUDE_API_KEY

# Check in Docker container
docker exec shahin-jan-2026_grcmvc_1 env | grep CLAUDE_API_KEY
```

### Check Application Logs:

```bash
# Look for Claude configuration messages
docker logs shahin-jan-2026_grcmvc_1 | grep -i claude

# Should see:
# ✓ Claude API key configured
# OR
# ⚠ Claude API key is not configured
```

### Test Claude Features:

1. **Via API:**
   ```bash
   curl http://localhost:5137/api/diagnostics/claude-status
   ```

2. **Via UI:**
   - Navigate to AI-powered features
   - Try chat or agent features
   - Should work without "credentials not found" error

---

## 🔧 Quick Fix Script

Run this to add placeholder (then replace with real key):

```bash
cd /home/Shahin-ai/Shahin-Jan-2026

# Check if .env exists
if [ ! -f .env ]; then
    echo "Creating .env file..."
    touch .env
fi

# Add Claude configuration if not exists
if ! grep -q "CLAUDE_API_KEY" .env; then
    echo "" >> .env
    echo "# Claude AI Configuration" >> .env
    echo "CLAUDE_ENABLED=true" >> .env
    echo "CLAUDE_API_KEY=your-claude-api-key-here" >> .env
    echo "CLAUDE_MODEL=claude-sonnet-4-5-20250514" >> .env
    echo "CLAUDE_MAX_TOKENS=4096" >> .env
    echo "✅ Added Claude configuration to .env"
    echo "⚠️  IMPORTANT: Replace 'your-claude-api-key-here' with your actual API key!"
else
    echo "CLAUDE_API_KEY already exists in .env"
fi
```

---

## 📋 Configuration Locations

The application checks for Claude API key in this order:

1. **Environment Variable:** `CLAUDE_API_KEY` (highest priority)
2. **appsettings.json:** `ClaudeAgents:ApiKey`
3. **Docker Compose:** `CLAUDE_API_KEY` in environment section

---

## 🔒 Security Notes

⚠️ **Important:**
- ✅ Never commit `.env` file to git (already in .gitignore)
- ✅ Keep your API key secure
- ✅ Rotate keys regularly
- ✅ Use different keys for dev/staging/production
- ✅ Monitor API usage in Anthropic console

---

## 🐛 Troubleshooting

### Error: "ClaudeAgents:ApiKey is required"
- **Cause:** API key not set
- **Fix:** Add `CLAUDE_API_KEY` to `.env` or environment

### Error: "API key format appears incorrect"
- **Cause:** Key doesn't start with `sk-ant-`
- **Fix:** Verify you copied the full key from Anthropic console

### Error: "401 Unauthorized"
- **Cause:** Invalid or expired API key
- **Fix:** Generate new key from Anthropic console

### Error: "Rate limit exceeded"
- **Cause:** Too many API requests
- **Fix:** Wait or upgrade your Anthropic plan

---

## ✅ After Fixing

Once configured, you should see:
- ✅ No "credentials not found" errors
- ✅ Claude AI features working
- ✅ Usage data visible
- ✅ Chat/agent features functional

---

**Status:** ⏳ **Action Required** - Add your Claude API key to fix the error
