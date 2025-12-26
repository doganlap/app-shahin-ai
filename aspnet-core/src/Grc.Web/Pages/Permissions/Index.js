(function () {
    $(document).ready(function () {
        $('#permissionsForm').on('submit', function (e) {
            e.preventDefault();
            
            var formData = new FormData(this);
            var grantedPermissions = [];
            
            $('input[name="grantedPermissions"]:checked').each(function () {
                grantedPermissions.push($(this).val());
            });
            
            var roleId = $('input[name="roleId"]').val();
            
            abp.ajax({
                url: abp.appPath + 'Permissions?handler=UpdatePermissions',
                type: 'POST',
                data: JSON.stringify({
                    roleId: roleId,
                    grantedPermissions: grantedPermissions
                }),
                contentType: 'application/json'
            }).done(function (result) {
                if (result.success) {
                    abp.notify.success('Permissions updated successfully');
                    location.reload();
                } else {
                    abp.notify.error(result.message || 'Failed to update permissions');
                }
            }).fail(function () {
                abp.notify.error('Failed to update permissions');
            });
        });
    });
})();

