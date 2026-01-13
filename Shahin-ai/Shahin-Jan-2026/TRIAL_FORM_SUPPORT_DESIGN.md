# Trial Registration Form - Support & Design Changes

**Route:** `/trial`  
**File:** `src/GrcMvc/Views/Trial/Index.cshtml`  
**Date:** 2026-01-22

---

## 🔍 Current Features

### 1. AI Assistant Chat (Support Feature)

**Location:** Bottom-left corner of the page  
**Trigger:** Appears automatically after 10 seconds  
**Purpose:** Help users with registration questions

**Features:**
- AI-powered chat assistant
- Appears as floating bubble with robot icon
- Opens chat window when clicked
- Sends messages to `/api/agent/chat/public`
- Context: `trial_registration`

**Code Location:** Lines 687-864 in `Index.cshtml`

---

### 2. "Powered by Dogan Consult" Footer

**Location:** Bottom of registration form  
**Text:** "Powered by Dogan Consult" with lightning icon  
**Link:** https://www.doganconsult.com  
**Style:** Small text, subtle design

**Code Location:** Lines 589-598 in `Index.cshtml`

---

## 🎨 Design Elements

### Current Design:
- **Dark theme** with purple/blue gradient
- **Two-column layout** (info panel + form)
- **Modern glassmorphism** effects
- **Arabic RTL** support
- **Responsive** design (mobile-friendly)

### Key Visual Elements:
- Trial badge: "7 أيام مجاناً" (7 days free)
- Feature list with checkmarks
- Gradient buttons
- Glass-effect containers

---

## 🔧 Support Integration

### Available Support Features:

1. **AI Chat Assistant**
   - Route: `/api/agent/chat/public`
   - Context: `trial_registration`
   - Anonymous access (no auth required)

2. **Contact Form API**
   - Route: `/api/support/contact`
   - Method: POST
   - Fields: Name, Email, Subject, Message
   - Sends email to: `support@grc-system.sa`

3. **Diagnostics Tracking**
   - Route: `/api/diagnostics/visitor`
   - Tracks: Page views, form interactions, errors

---

## 📋 What Might Have Changed

### Possible Changes:

1. **AI Assistant Integration**
   - May have been added/modified
   - Chat functionality might be new
   - Support endpoint might have changed

2. **Design Updates**
   - "Powered by Dogan Consult" footer
   - Color scheme or layout changes
   - Responsive design improvements

3. **Support Contact**
   - Contact form endpoint
   - Email notification settings
   - Support email address

---

## ✅ Verification Checklist

To verify current state:

- [ ] Check if AI assistant appears on `/trial` page
- [ ] Verify "Powered by Dogan Consult" footer is visible
- [ ] Test AI chat functionality
- [ ] Check contact form submission
- [ ] Verify support email: `support@grc-system.sa`
- [ ] Test responsive design on mobile

---

## 🔍 Quick Test Commands

```bash
# Check trial page loads
curl -s http://localhost:8888/trial | grep -i "dogan\|support\|assistant"

# Test AI chat endpoint
curl -X GET "http://localhost:8888/api/agent/chat/public?message=help&context=trial_registration"

# Test contact form
curl -X POST http://localhost:8888/api/support/contact \
  -H "Content-Type: application/json" \
  -d '{"name":"Test","email":"test@test.com","subject":"Test","message":"Test message"}'
```

---

## 🎯 Next Steps

If you want to:
1. **Remove "Powered by Dogan Consult"** - Edit lines 589-598
2. **Modify AI assistant** - Edit lines 687-864
3. **Change support email** - Edit `SupportController.cs` line 296
4. **Update design** - Edit CSS in `<style>` section (lines 30-430)

---

**Current Status:** Form includes AI assistant and Dogan Consult branding
