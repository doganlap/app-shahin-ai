$(function () {
    var l = abp.localization.getResource('Grc');
    
    var dataTable = $('#FrameworksTable').DataTable(
        abp.libs.datatables.normalizeConfiguration({
            serverSide: true,
            paging: true,
            order: [[0, 'asc']],
            searching: false,
            scrollX: true,
            ajax: abp.libs.datatables.createAjax(
                function (input) {
                    return grc.frameworkLibrary.application.contracts.frameworks.framework.getList({
                        maxResultCount: input.length,
                        skipCount: input.start,
                        sorting: input.sorting,
                        regulatorId: $('#RegulatorFilter').val() || null,
                        category: $('#CategoryFilter').val() || null,
                        status: $('#StatusFilter').val() || null,
                        isMandatory: $('#MandatoryFilter').val() !== '' ? $('#MandatoryFilter').val() === 'true' : null
                    });
                }
            ),
            columnDefs: [
                {
                    title: l('Code'),
                    data: 'code',
                    render: function (data, type, row) {
                        return '<strong>' + data + '</strong> <small class="text-muted">v' + row.version + '</small>';
                    }
                },
                {
                    title: l('Title'),
                    data: 'title.en',
                    render: function (data, type, row) {
                        return '<div class="framework-title">' + 
                               '<div>' + data + '</div>' +
                               (row.description && row.description.en ? '<small class="text-muted">' + row.description.en.substring(0, 80) + '...</small>' : '') +
                               '</div>';
                    }
                },
                {
                    title: l('Regulator'),
                    data: 'regulator.nameEn',
                    render: function (data, type, row) {
                        return '<div class="regulator-cell">' +
                               '<div>' + data + '</div>' +
                               '<small class="text-muted">' + row.regulator.code + '</small>' +
                               '</div>';
                    }
                },
                {
                    title: l('Category'),
                    data: 'category',
                    render: function (data) {
                        var badgeClass = {
                            'Cybersecurity': 'bg-primary',
                            'DataPrivacy': 'bg-info',
                            'Financial': 'bg-success',
                            'Healthcare': 'bg-danger',
                            'Quality': 'bg-warning'
                        }[data] || 'bg-secondary';
                        return '<span class="badge ' + badgeClass + '">' + l('Category:' + data) + '</span>';
                    }
                },
                {
                    title: l('Controls'),
                    data: 'totalControls',
                    render: function (data) {
                        return '<span class="badge bg-secondary">' + data + ' ' + l('Controls') + '</span>';
                    }
                },
                {
                    title: l('Mandatory'),
                    data: 'isMandatory',
                    render: function (data) {
                        return data 
                            ? '<i class="fas fa-check-circle text-success"></i> ' + l('Yes')
                            : '<i class="fas fa-times-circle text-muted"></i> ' + l('No');
                    }
                },
                {
                    title: l('EffectiveDate'),
                    data: 'effectiveDate',
                    render: function (data) {
                        return luxon.DateTime.fromISO(data).toLocaleString();
                    }
                },
                {
                    title: l('Status'),
                    data: 'status',
                    render: function (data) {
                        var badgeClass = {
                            'Active': 'bg-success',
                            'Draft': 'bg-warning',
                            'Deprecated': 'bg-danger'
                        }[data] || 'bg-secondary';
                        return '<span class="badge ' + badgeClass + '">' + l('Status:' + data) + '</span>';
                    }
                },
                {
                    title: l('Actions'),
                    data: null,
                    orderable: false,
                    render: function (data, type, row) {
                        var actions = '<div class="btn-group btn-group-sm">';
                        actions += '<a href="/FrameworkLibrary/Details?id=' + row.id + '" class="btn btn-primary" title="' + l('ViewDetails') + '">' +
                                  '<i class="fas fa-eye"></i></a>';
                        if (row.officialDocumentUrl) {
                            actions += '<a href="' + row.officialDocumentUrl + '" target="_blank" class="btn btn-info" title="' + l('OfficialDocument') + '">' +
                                      '<i class="fas fa-file-pdf"></i></a>';
                        }
                        actions += '</div>';
                        return actions;
                    }
                }
            ]
        })
    );

    // Filter change events
    $('#RegulatorFilter, #CategoryFilter, #StatusFilter, #MandatoryFilter').on('change', function () {
        dataTable.ajax.reload();
    });

    // Refresh button
    $('#RefreshButton').on('click', function () {
        dataTable.ajax.reload();
    });

    // Load regulators for filter
    grc.frameworkLibrary.domain.regulators.regulator.getAll()
        .then(function (result) {
            var select = $('#RegulatorFilter');
            result.items.forEach(function (regulator) {
                select.append('<option value="' + regulator.id + '">' + regulator.nameEn + ' (' + regulator.code + ')</option>');
            });
        });

    // Load statistics and charts
    function loadStatistics() {
        grc.frameworkLibrary.application.contracts.frameworks.framework.getList({
            maxResultCount: 1000,
            skipCount: 0
        }).then(function (result) {
            // Update statistics cards
            $('#TotalFrameworks').text(result.totalCount);
            var activeCount = result.items.filter(f => f.status === 'Active').length;
            $('#ActiveFrameworks').text(activeCount);
            
            // Get total controls count
            var totalControls = result.items.reduce((sum, f) => sum + (f.controlsCount || 0), 0);
            $('#TotalControls').text(totalControls);
            
            // Get unique regulators count
            var uniqueRegulators = [...new Set(result.items.map(f => f.regulator?.id))].filter(Boolean);
            $('#TotalRegulators').text(uniqueRegulators.length);
            
            // Render charts
            renderCharts(result.items);
            
            // Render card view
            renderCardView(result.items);
        });
    }

    // Render charts
    function renderCharts(frameworks) {
        // Category Chart
        var categoryData = {};
        frameworks.forEach(f => {
            var cat = f.category || 'Other';
            categoryData[cat] = (categoryData[cat] || 0) + 1;
        });
        
        new Chart(document.getElementById('categoryChart'), {
            type: 'pie',
            data: {
                labels: Object.keys(categoryData),
                datasets: [{
                    data: Object.values(categoryData),
                    backgroundColor: ['#0d6efd', '#198754', '#ffc107', '#dc3545', '#6c757d', '#0dcaf0']
                }]
            },
            options: {
                responsive: true,
                maintainAspectRatio: false,
                plugins: {
                    legend: { position: 'bottom' }
                }
            }
        });

        // Regulator Chart
        var regulatorData = {};
        frameworks.forEach(f => {
            var reg = f.regulator?.nameEn || 'Unknown';
            regulatorData[reg] = (regulatorData[reg] || 0) + 1;
        });
        
        new Chart(document.getElementById('regulatorChart'), {
            type: 'bar',
            data: {
                labels: Object.keys(regulatorData),
                datasets: [{
                    label: 'Frameworks',
                    data: Object.values(regulatorData),
                    backgroundColor: '#0d6efd'
                }]
            },
            options: {
                responsive: true,
                maintainAspectRatio: false,
                scales: {
                    y: { beginAtZero: true }
                }
            }
        });

        // Status Chart
        var statusData = {};
        frameworks.forEach(f => {
            var status = f.status || 'Unknown';
            statusData[status] = (statusData[status] || 0) + 1;
        });
        
        new Chart(document.getElementById('statusChart'), {
            type: 'doughnut',
            data: {
                labels: Object.keys(statusData),
                datasets: [{
                    data: Object.values(statusData),
                    backgroundColor: ['#198754', '#ffc107', '#dc3545']
                }]
            },
            options: {
                responsive: true,
                maintainAspectRatio: false,
                plugins: {
                    legend: { position: 'bottom' }
                }
            }
        });

        // Controls Distribution Chart
        var controlsData = frameworks.map(f => ({
            label: f.code,
            controls: f.controlsCount || 0
        })).sort((a, b) => b.controls - a.controls).slice(0, 10);
        
        new Chart(document.getElementById('controlsChart'), {
            type: 'bar',
            data: {
                labels: controlsData.map(d => d.label),
                datasets: [{
                    label: 'Controls Count',
                    data: controlsData.map(d => d.controls),
                    backgroundColor: '#ffc107'
                }]
            },
            options: {
                responsive: true,
                maintainAspectRatio: false,
                indexAxis: 'y',
                scales: {
                    x: { beginAtZero: true }
                }
            }
        });
    }

    // Render card view
    function renderCardView(frameworks) {
        var container = $('#frameworkCards');
        container.empty();
        
        frameworks.forEach(function(framework) {
            var statusColor = framework.status === 'Active' ? 'success' : framework.status === 'Draft' ? 'warning' : 'secondary';
            var mandatoryBadge = framework.isMandatory ? '<span class="badge bg-danger">Mandatory</span>' : '';
            
            var card = `
                <div class="col-md-4 mb-3">
                    <div class="card h-100 border-${statusColor}">
                        <div class="card-header bg-${statusColor} text-white">
                            <h6 class="mb-0"><strong>${framework.code}</strong> <small>v${framework.version}</small></h6>
                        </div>
                        <div class="card-body">
                            <h5 class="card-title">${framework.title?.en || 'No Title'}</h5>
                            <p class="card-text text-muted small">${(framework.description?.en || '').substring(0, 100)}...</p>
                            <div class="mb-2">
                                <small class="text-muted"><i class="fas fa-landmark"></i> ${framework.regulator?.nameEn || 'N/A'}</small>
                            </div>
                            <div class="mb-2">
                                <span class="badge bg-primary">${framework.category || 'Other'}</span>
                                <span class="badge bg-${statusColor}">${framework.status}</span>
                                ${mandatoryBadge}
                            </div>
                            <div class="d-flex justify-content-between align-items-center">
                                <small><i class="fas fa-shield-alt"></i> ${framework.controlsCount || 0} Controls</small>
                                <div>
                                    <a href="/FrameworkLibrary/Details?id=${framework.id}" class="btn btn-sm btn-primary">
                                        <i class="fas fa-eye"></i> View
                                    </a>
                                </div>
                            </div>
                        </div>
                        ${framework.effectiveDate ? `<div class="card-footer text-muted small">
                            <i class="fas fa-calendar"></i> Effective: ${new Date(framework.effectiveDate).toLocaleDateString()}
                        </div>` : ''}
                    </div>
                </div>
            `;
            container.append(card);
        });
    }

    // Load statistics on page load
    loadStatistics();
    
    // Reload statistics when filters change
    $('#RegulatorFilter, #CategoryFilter, #StatusFilter, #MandatoryFilter').on('change', function () {
        loadStatistics();
    });
});

