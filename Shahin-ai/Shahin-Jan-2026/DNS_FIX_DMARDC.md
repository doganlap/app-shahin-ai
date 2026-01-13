# Quick DNS Fix Needed

**Issue Found**: DMARC record is missing "v=" prefix

**Current (WRONG)**:
```
Content: "DMARC1; p=none; rua=mailto:dmarc@shahin-ai.com; pct=100"
```

**Should Be (CORRECT)**:
```
Content: v=DMARC1; p=none; rua=mailto:dmarc@shahin-ai.com; pct=100
```

**Fix in Cloudflare**:
1. Edit the _dmarc TXT record
2. Change Content from: `DMARC1; p=none;...`
3. To: `v=DMARC1; p=none;...`
4. Save

Also, since you're using Office 365 MX, update SPF to:
```
v=spf1 include:spf.protection.outlook.com ~all
```

But let's deploy production first, then fix DNS later.
