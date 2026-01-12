(function () {
    var l = abp.localization.getResource('Grc');

    var dataTable = $('#VendorsTable').DataTable(
        abp.libs.datatables.normalizeConfiguration({
            serverSide: false,
            paging: true,
            order: [[0, "asc"]],
            searching: true,
            scrollX: true,
            ajax: function (requestData, callback, settings) {
                abp.ajax({
                    url: '/api/admin/vendors',
                    type: 'GET'
                }).done(function (result) {
                    callback({
                        recordsTotal: result.totalCount || 0,
                        recordsFiltered: result.totalCount || 0,
                        data: result.items || []
                    });
                }).fail(function(xhr, status, error) {
                    console.error('API call failed:', status, error);
                    abp.notify.error('Failed to load vendors. Endpoint: /api/admin/vendors');
                    callback({
                        recordsTotal: 0,
                        recordsFiltered: 0,
                        data: []
                    });
                });
            },
            columns: [
                {
                    title: l('VendorName'),
                    data: "vendorName",
                    render: function (data, type, row) {
                        var currentLang = abp.localization.currentCulture.cultureName;
                        var name = currentLang === 'ar' && row.nameAr ? row.nameAr : (row.nameEn || data);
                        return name;
                    }
                },
                {
                    title: l('Category'),
                    data: "category",
                    render: function (data) {
                        if (!data) return '-';
                        return '<span class="badge bg-info">' + data + '</span>';
                    }
                },
                {
                    title: l('RiskScore'),
                    data: "riskScore",
                    render: function (data) {
                        var badgeClass = 'bg-success';
                        if (data >= 4) badgeClass = 'bg-danger';
                        else if (data >= 3) badgeClass = 'bg-warning';
                        return '<span class="badge ' + badgeClass + '">' + (data || 0) + '</span>';
                    }
                },
                {
                    title: l('Status'),
                    data: "status",
                    render: function (data) {
                        var badgeClass = data === 'Active' ? 'bg-success' : 'bg-secondary';
                        return '<span class="badge ' + badgeClass + '">' + (data || 'Active') + '</span>';
                    }
                },
                {
                    title: l('Actions'),
                    data: null,
                    orderable: false,
                    render: function (data, type, row) {
                        var html = '<div class="btn-group">';
                        if (row.website) {
                            html += '<a href="' + row.website + '" target="_blank" class="btn btn-sm btn-outline-primary" title="Visit Website"><i class="fas fa-external-link-alt"></i></a>';
                        }
                        if (row.contactEmail) {
                            html += '<a href="mailto:' + row.contactEmail + '" class="btn btn-sm btn-outline-info" title="Send Email"><i class="fas fa-envelope"></i></a>';
                        }
                        html += '</div>';
                        return html;
                    }
                }
            ]
        })
    );
})();
