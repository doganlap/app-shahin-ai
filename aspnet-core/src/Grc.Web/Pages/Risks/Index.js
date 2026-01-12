$(function () {
    var l = abp.localization.getResource('Grc');
    
    // Risk Matrix 5x5
    var matrix = '<table class="table table-bordered text-center">';
    matrix += '<tr><th></th><th colspan="5">' + l('Impact') + '</th></tr>';
    matrix += '<tr><th>' + l('Likelihood') + '</th><th>1</th><th>2</th><th>3</th><th>4</th><th>5</th></tr>';
    
    for (var i = 5; i >= 1; i--) {
        matrix += '<tr><th>' + i + '</th>';
        for (var j = 1; j <= 5; j++) {
            var risk = i * j;
            var color = risk <= 5 ? 'success' : risk <= 12 ? 'warning' : 'danger';
            matrix += '<td class="risk-cell bg-' + color + ' bg-opacity-25"><small>' + risk + '</small></td>';
        }
        matrix += '</tr>';
    }
    matrix += '</table>';
    $('#RiskMatrix').html(matrix);

    var dataTable = $('#RisksTable').DataTable(
        abp.libs.datatables.normalizeConfiguration({
            serverSide: true,
            paging: true,
            order: [[0, 'asc']],
            ajax: abp.libs.datatables.createAjax(grc.risk.application.contracts.risks.risk.getList),
            columnDefs: [
                { title: l('RiskCode'), data: 'riskCode' },
                { title: l('Title'), data: 'title' },
                { title: l('Category'), data: 'category' },
                { title: l('Likelihood'), data: 'likelihood' },
                { title: l('Impact'), data: 'impact' },
                {
                    title: l('RiskLevel'),
                    data: null,
                    render: function (data, type, row) {
                        var level = row.likelihood * row.impact;
                        var color = level <= 5 ? 'success' : level <= 12 ? 'warning' : 'danger';
                        return '<span class="badge bg-' + color + '">' + level + '</span>';
                    }
                },
                { title: l('Treatment'), data: 'treatment' },
                { title: l('Owner'), data: 'ownerName' },
                {
                    title: l('Actions'),
                    data: null,
                    orderable: false,
                    render: function (data, type, row) {
                        return '<a href="/Risks/Details?id=' + row.id + '" class="btn btn-sm btn-primary"><i class="fas fa-eye"></i></a>';
                    }
                }
            ]
        })
    );
});

