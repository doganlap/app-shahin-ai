$(function () {
    // ABP Modal Manager for Create/Edit
    var createModal = new abp.ModalManager(abp.appPath + 'Assessments/CreateModal');
    var editModal = new abp.ModalManager(abp.appPath + 'Assessments/EditModal');

    // New Assessment Button
    $('#NewAssessmentButton').click(function (e) {
        e.preventDefault();
        abp.message.info(
            'Assessment creation form will be available here!',
            'Coming Soon'
        );
        // When ready: createModal.open();
    });

    // Export Button
    $('#ExportButton').click(function (e) {
        e.preventDefault();
        abp.message.confirm(
            'Export all assessments to Excel?',
            'Export Assessments',
            function (confirmed) {
                if (confirmed) {
                    abp.ui.setBusy();
                    setTimeout(function () {
                        abp.ui.clearBusy();
                        abp.notify.success('Export completed!', 'Success');
                    }, 2000);
                }
            }
        );
    });

    // Search and Filter
    $('#SearchButton').click(function () {
        var status = $('#StatusFilter').val();
        var framework = $('#FrameworkFilter').val();
        var dateFrom = $('#DateFromFilter').val();

        abp.ui.setBusy($('#AssessmentsTable'));
        
        // Simulate filtering
        setTimeout(function () {
            abp.ui.clearBusy($('#AssessmentsTable'));
            if (status || framework || dateFrom) {
                abp.notify.info('Filters applied: Status=' + status + ', Framework=' + framework, 'Filtering');
            }
        }, 1000);
    });

    $('#ResetButton').click(function () {
        $('#StatusFilter').val('');
        $('#FrameworkFilter').val('');
        $('#DateFromFilter').val('');
        abp.notify.info('Filters cleared', 'Reset');
    });

    // Select All Checkbox
    $('#SelectAll').change(function () {
        var isChecked = $(this).is(':checked');
        $('.assessment-checkbox').prop('checked', isChecked);
        updateBulkActionsPanel();
    });

    $('.assessment-checkbox').change(function () {
        updateBulkActionsPanel();
    });

    function updateBulkActionsPanel() {
        var selectedCount = $('.assessment-checkbox:checked').length;
        $('#SelectedCount').text(selectedCount);
        
        if (selectedCount > 0) {
            $('#BulkActionsPanel').slideDown();
        } else {
            $('#BulkActionsPanel').slideUp();
        }
    }

    // Bulk Delete
    $('#BulkDeleteButton').click(function () {
        var selectedIds = $('.assessment-checkbox:checked').map(function () {
            return $(this).val();
        }).get();

        abp.message.confirm(
            'Delete ' + selectedIds.length + ' selected assessments?',
            'Confirm Deletion',
            function (confirmed) {
                if (confirmed) {
                    abp.ui.setBusy();
                    setTimeout(function () {
                        abp.ui.clearBusy();
                        abp.notify.success(selectedIds.length + ' assessments deleted!', 'Deleted');
                        $('.assessment-checkbox:checked').closest('tr').fadeOut();
                    }, 1500);
                }
            }
        );
    });

    // Bulk Export
    $('#BulkExportButton').click(function () {
        var selectedCount = $('.assessment-checkbox:checked').length;
        abp.notify.info('Exporting ' + selectedCount + ' assessments...', 'Export');
    });

    // Row Actions
    $('.start-assessment').click(function (e) {
        e.preventDefault();
        var id = $(this).data('id');
        
        abp.message.confirm(
            'Start this assessment now?',
            'Start Assessment',
            function (confirmed) {
                if (confirmed) {
                    abp.ui.setBusy();
                    setTimeout(function () {
                        abp.ui.clearBusy();
                        abp.notify.success('Assessment started successfully!', 'Started');
                    }, 1000);
                }
            }
        );
    });

    $('.generate-report').click(function (e) {
        e.preventDefault();
        var id = $(this).data('id');
        
        abp.message.info(
            'Report generation will download a PDF report with assessment details.',
            'Generate Report'
        );
    });

    $('.delete-assessment').click(function (e) {
        e.preventDefault();
        var id = $(this).data('id');
        var $row = $(this).closest('tr');
        var name = $row.find('strong').first().text();

        abp.message.confirm(
            'Are you sure you want to delete "' + name + '"?',
            'Delete Assessment',
            function (confirmed) {
                if (confirmed) {
                    abp.ui.setBusy();
                    setTimeout(function () {
                        abp.ui.clearBusy();
                        $row.fadeOut(400, function () {
                            $(this).remove();
                            abp.notify.success('Assessment deleted successfully!', 'Deleted');
                        });
                    }, 1000);
                }
            }
        );
    });

    // ABP JavaScript API Examples
    console.log('Assessment page loaded');
    console.log('ABP Version:', abp.appPath);
    console.log('Current User:', abp.currentUser);
});
