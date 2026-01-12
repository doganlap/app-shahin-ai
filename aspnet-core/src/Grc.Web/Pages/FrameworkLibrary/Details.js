$(function () {
    var l = abp.localization.getResource('Grc');
    
    var dataTable = $('#ControlsTable').DataTable(
        abp.libs.datatables.normalizeConfiguration({
            serverSide: true,
            paging: true,
            order: [[0, 'asc']],
            searching: false,
            scrollX: true,
            ajax: abp.libs.datatables.createAjax(
                function (input) {
                    return grc.frameworkLibrary.application.contracts.frameworks.framework.getControls(
                        frameworkId,
                        {
                            maxResultCount: input.length,
                            skipCount: input.start,
                            sorting: input.sorting
                        }
                    );
                }
            ),
            columnDefs: [
                {
                    title: l('ControlNumber'),
                    data: 'controlNumber',
                    render: function (data) {
                        return '<strong>' + data + '</strong>';
                    }
                },
                {
                    title: l('Title'),
                    data: 'title.en',
                    render: function (data, type, row) {
                        return '<div class="control-title">' + data + '</div>';
                    }
                },
                {
                    title: l('Domain'),
                    data: 'domainCode',
                    render: function (data) {
                        return '<span class="badge bg-secondary">' + data + '</span>';
                    }
                },
                {
                    title: l('Type'),
                    data: 'controlType',
                    render: function (data) {
                        var badgeClass = {
                            'Preventive': 'bg-success',
                            'Detective': 'bg-info',
                            'Corrective': 'bg-warning'
                        }[data] || 'bg-secondary';
                        return '<span class="badge ' + badgeClass + '">' + l('ControlType:' + data) + '</span>';
                    }
                },
                {
                    title: l('Priority'),
                    data: 'priority',
                    render: function (data) {
                        var badgeClass = {
                            'Critical': 'bg-danger',
                            'High': 'bg-warning',
                            'Medium': 'bg-info',
                            'Low': 'bg-secondary'
                        }[data] || 'bg-secondary';
                        return '<span class="badge ' + badgeClass + '">' + l('Priority:' + data) + '</span>';
                    }
                },
                {
                    title: l('MaturityLevel'),
                    data: 'maturityLevel',
                    render: function (data) {
                        var stars = '';
                        for (var i = 0; i < data; i++) {
                            stars += '<i class="fas fa-star text-warning"></i>';
                        }
                        return stars + ' <small class="text-muted">(Level ' + data + ')</small>';
                    }
                },
                {
                    title: l('Actions'),
                    data: null,
                    orderable: false,
                    render: function (data, type, row) {
                        return '<button class="btn btn-sm btn-primary view-control-btn" data-id="' + row.id + '">' +
                               '<i class="fas fa-eye"></i> ' + l('View') +
                               '</button>';
                    }
                }
            ]
        })
    );

    // View control details
    $(document).on('click', '.view-control-btn', function () {
        var controlId = $(this).data('id');
        // TODO: Show control details in modal or navigate to details page
        abp.message.info('Control details for ID: ' + controlId);
    });

    // Search controls button
    $('#SearchControlsButton').on('click', function () {
        abp.message.info('Full-text search coming soon!');
    });
});

