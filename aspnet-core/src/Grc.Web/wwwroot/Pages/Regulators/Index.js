(function () {
    var l = abp.localization.getResource('Grc');
    var createModal = new abp.ModalManager(abp.appPath + 'Regulators/CreateModal');
    var editModal = new abp.ModalManager(abp.appPath + 'Regulators/EditModal');

    var dataTable = $('#RegulatorsTable').DataTable(
        abp.libs.datatables.normalizeConfiguration({
            serverSide: true,
            paging: true,
            order: [[1, "asc"]],
            searching: false,
            scrollX: true,
            ajax: abp.libs.datatables.createAjax(
                function (input) {
                    var filter = $('#RegulatorSearchInput').val();
                    var category = $('#CategoryFilter').val();
                    
                    return grc.frameworkLibrary.application.contracts.regulators.regulator.getList({
                        maxResultCount: input.length,
                        skipCount: input.start,
                        sorting: input.sorting,
                        filter: filter,
                        category: category
                    });
                }
            ),
            columnDefs: [
                {
                    title: l('Actions'),
                    rowAction: {
                        items: [
                            {
                                text: l('Regulators:Edit'),
                                visible: abp.auth.isGranted('FrameworkLibrary.Regulators.Edit'),
                                action: function (data) {
                                    editModal.open({ id: data.record.id });
                                }
                            },
                            {
                                text: l('Regulators:Delete'),
                                visible: abp.auth.isGranted('FrameworkLibrary.Regulators.Delete'),
                                confirmMessage: function (data) {
                                    return l('Regulators:ConfirmDelete');
                                },
                                action: function (data) {
                                    abp.ajax({
                                        url: '/api/app/framework-library/regulator/' + data.record.id,
                                        type: 'DELETE'
                                    }).done(function() {
                                        abp.notify.success(l('Regulators:Deleted'));
                                        dataTable.ajax.reload();
                                    });
                                }
                            }
                        ]
                    }
                },
                {
                    title: l('Regulators:Code'),
                    data: "code",
                    render: function (data, type, row) {
                        return '<span class="badge bg-secondary">' + data + '</span>';
                    }
                },
                {
                    title: l('Regulators:Name'),
                    data: "name",
                    render: function (data, type, row) {
                        var currentLang = abp.localization.currentCulture.cultureName;
                        var name = currentLang === 'ar' ? data.ar : data.en;
                        if (row.logoUrl) {
                            return '<img src="' + row.logoUrl + '" class="regulator-logo me-2" alt="' + name + '"> ' + name;
                        }
                        return name;
                    }
                },
                {
                    title: l('Regulators:Category'),
                    data: "category",
                    render: function (data) {
                        var badges = {
                            1: 'Cybersecurity',
                            2: 'Financial',
                            3: 'Healthcare',
                            4: 'Data',
                            5: 'Tax',
                            6: 'Telecommunications'
                        };
                        var categoryName = badges[data] || 'Other';
                        return '<span class="badge bg-info">' + categoryName + '</span>';
                    }
                },
                {
                    title: l('Regulators:Jurisdiction'),
                    data: "jurisdiction",
                    render: function (data, type, row) {
                        if (!data) return '';
                        var currentLang = abp.localization.currentCulture.cultureName;
                        return currentLang === 'ar' ? data.ar : data.en;
                    }
                },
                {
                    title: l('Regulators:Website'),
                    data: "website",
                    render: function (data) {
                        if (!data) return '';
                        return '<a href="' + data + '" target="_blank" class="btn btn-sm btn-link"><i class="fas fa-external-link-alt"></i></a>';
                    }
                },
                {
                    title: l('Regulators:CreationTime'),
                    data: "creationTime",
                    render: function (data) {
                        return luxon.DateTime.fromISO(data, {
                            locale: abp.localization.currentCulture.name
                        }).toLocaleString(luxon.DateTime.DATE_SHORT);
                    }
                }
            ]
        })
    );

    createModal.onResult(function () {
        dataTable.ajax.reload();
    });

    editModal.onResult(function () {
        dataTable.ajax.reload();
    });

    $('#NewRegulatorButton').click(function (e) {
        e.preventDefault();
        createModal.open();
    });

    $('#SearchButton, #RegulatorSearchInput').on('click keypress', function (e) {
        if (e.type === 'keypress' && e.which !== 13) {
            return;
        }
        dataTable.ajax.reload();
    });

    $('#CategoryFilter').change(function () {
        dataTable.ajax.reload();
    });

    $('input[name="regionFilter"]').change(function () {
        dataTable.ajax.reload();
    });

    // Load categories into filter dropdown
    $(document).ready(function () {
        var categories = [
            { value: 1, text: 'Cybersecurity' },
            { value: 2, text: 'Financial' },
            { value: 3, text: 'Healthcare' },
            { value: 4, text: 'Data' },
            { value: 5, text: 'Tax' },
            { value: 6, text: 'Telecommunications' },
            { value: 7, text: 'Digital' },
            { value: 8, text: 'Commerce' }
        ];
        
        categories.forEach(function (cat) {
            $('#CategoryFilter').append($('<option>', {
                value: cat.value,
                text: cat.text
            }));
        });
    });
})();

