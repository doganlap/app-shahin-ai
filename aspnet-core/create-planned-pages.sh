set -e

BASE="src/Grc.Web/Pages"

mkdir -p "$BASE/FrameworkLibrary" "$BASE/Assessments" "$BASE/Risks" "$BASE/Evidence"

# ---- FrameworkLibrary CreateModal ----
cat > "$BASE/FrameworkLibrary/CreateModal.cshtml" <<'EOF'
@page
@model DoganConsult.Grc.Web.Pages.FrameworkLibrary.CreateModalModel
@using Volo.Abp.AspNetCore.Mvc.UI.Bootstrap.TagHelpers.Modal
@{
    Layout = null;
}
<abp-modal>
    <abp-modal-header title="Create Framework (Planned)"></abp-modal-header>
    <abp-modal-body>
        <p>Placeholder modal. Implement DTO + AppService call later.</p>
    </abp-modal-body>
    <abp-modal-footer buttons="@(AbpModalButtons.Close)"></abp-modal-footer>
</abp-modal>
EOF

cat > "$BASE/FrameworkLibrary/CreateModal.cshtml.cs" <<'EOF'
using Volo.Abp.AspNetCore.Mvc.UI.RazorPages;

namespace DoganConsult.Grc.Web.Pages.FrameworkLibrary;

public class CreateModalModel : AbpPageModel
{
    public void OnGet() { }
}
EOF

# ---- FrameworkLibrary EditModal ----
cat > "$BASE/FrameworkLibrary/EditModal.cshtml" <<'EOF'
@page "{id?}"
@model DoganConsult.Grc.Web.Pages.FrameworkLibrary.EditModalModel
@using Volo.Abp.AspNetCore.Mvc.UI.Bootstrap.TagHelpers.Modal
@{
    Layout = null;
}
<abp-modal>
    <abp-modal-header title="Edit Framework (Planned)"></abp-modal-header>
    <abp-modal-body>
        <p>Framework Id: <strong>@Model.Id</strong></p>
        <p>Placeholder modal. Implement DTO + AppService call later.</p>
    </abp-modal-body>
    <abp-modal-footer buttons="@(AbpModalButtons.Close)"></abp-modal-footer>
</abp-modal>
EOF

cat > "$BASE/FrameworkLibrary/EditModal.cshtml.cs" <<'EOF'
using Microsoft.AspNetCore.Mvc;
using Volo.Abp.AspNetCore.Mvc.UI.RazorPages;

namespace DoganConsult.Grc.Web.Pages.FrameworkLibrary;

public class EditModalModel : AbpPageModel
{
    [BindProperty(SupportsGet = true)]
    public string? Id { get; set; }

    public void OnGet() { }
}
EOF

# ---- Assessments Create/Edit/Details ----
cat > "$BASE/Assessments/Create.cshtml" <<'EOF'
@page
@model DoganConsult.Grc.Web.Pages.Assessments.CreateModel

<h3>Create Assessment (Planned)</h3>
<p>Placeholder page. Wire to POST /api/app/assessment later.</p>
EOF

cat > "$BASE/Assessments/Create.cshtml.cs" <<'EOF'
using Volo.Abp.AspNetCore.Mvc.UI.RazorPages;

namespace DoganConsult.Grc.Web.Pages.Assessments;

public class CreateModel : AbpPageModel
{
    public void OnGet() { }
}
EOF

cat > "$BASE/Assessments/Edit.cshtml" <<'EOF'
@page "{id?}"
@model DoganConsult.Grc.Web.Pages.Assessments.EditModel

<h3>Edit Assessment (Planned)</h3>
<p>Assessment Id: <strong>@Model.Id</strong></p>
<p>Placeholder page. Wire to PUT /api/app/assessment/{id} later.</p>
EOF

cat > "$BASE/Assessments/Edit.cshtml.cs" <<'EOF'
using Microsoft.AspNetCore.Mvc;
using Volo.Abp.AspNetCore.Mvc.UI.RazorPages;

namespace DoganConsult.Grc.Web.Pages.Assessments;

public class EditModel : AbpPageModel
{
    [BindProperty(SupportsGet = true)]
    public string? Id { get; set; }

    public void OnGet() { }
}
EOF

cat > "$BASE/Assessments/Details.cshtml" <<'EOF'
@page "{id?}"
@model DoganConsult.Grc.Web.Pages.Assessments.DetailsModel

<h3>Assessment Details (Planned)</h3>
<p>Assessment Id: <strong>@Model.Id</strong></p>
<p>Placeholder page. Wire to GET /api/app/assessment/{id} later.</p>
EOF

cat > "$BASE/Assessments/Details.cshtml.cs" <<'EOF'
using Microsoft.AspNetCore.Mvc;
using Volo.Abp.AspNetCore.Mvc.UI.RazorPages;

namespace DoganConsult.Grc.Web.Pages.Assessments;

public class DetailsModel : AbpPageModel
{
    [BindProperty(SupportsGet = true)]
    public string? Id { get; set; }

    public void OnGet() { }
}
EOF

# ---- Risks Create/Edit ----
cat > "$BASE/Risks/Create.cshtml" <<'EOF'
@page
@model DoganConsult.Grc.Web.Pages.Risks.CreateModel

<h3>Create Risk (Planned)</h3>
<p>Placeholder page. Wire to POST /api/app/risk later.</p>
EOF

cat > "$BASE/Risks/Create.cshtml.cs" <<'EOF'
using Volo.Abp.AspNetCore.Mvc.UI.RazorPages;

namespace DoganConsult.Grc.Web.Pages.Risks;

public class CreateModel : AbpPageModel
{
    public void OnGet() { }
}
EOF

cat > "$BASE/Risks/Edit.cshtml" <<'EOF'
@page "{id?}"
@model DoganConsult.Grc.Web.Pages.Risks.EditModel

<h3>Edit Risk (Planned)</h3>
<p>Risk Id: <strong>@Model.Id</strong></p>
<p>Placeholder page. Wire to PUT /api/app/risk/{id} later.</p>
EOF

cat > "$BASE/Risks/Edit.cshtml.cs" <<'EOF'
using Microsoft.AspNetCore.Mvc;
using Volo.Abp.AspNetCore.Mvc.UI.RazorPages;

namespace DoganConsult.Grc.Web.Pages.Risks;

public class EditModel : AbpPageModel
{
    [BindProperty(SupportsGet = true)]
    public string? Id { get; set; }

    public void OnGet() { }
}
EOF

# ---- Evidence Upload ----
cat > "$BASE/Evidence/Upload.cshtml" <<'EOF'
@page
@model DoganConsult.Grc.Web.Pages.Evidence.UploadModel

<h3>Upload Evidence (Planned)</h3>
<p>Placeholder page. Wire to POST /api/app/evidence/upload later.</p>
EOF

cat > "$BASE/Evidence/Upload.cshtml.cs" <<'EOF'
using Volo.Abp.AspNetCore.Mvc.UI.RazorPages;

namespace DoganConsult.Grc.Web.Pages.Evidence;

public class UploadModel : AbpPageModel
{
    public void OnGet() { }
}
EOF

echo "✅ All 8 planned pages created successfully."
