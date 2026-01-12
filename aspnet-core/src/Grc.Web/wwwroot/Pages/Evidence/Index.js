$(function () {
    var l = abp.localization.getResource('Grc');
    var uploadModal = new abp.ModalManager('#UploadModal');

    // Open upload modal
    $('#UploadButton, #UploadButtonEmpty').on('click', function () {
        uploadModal.open();
    });

    // Drag & Drop functionality
    var dropZone = $('#DropZone');
    var fileInput = $('#FileInput');

    dropZone.on('drag dragstart dragend dragover dragenter dragleave drop', function (e) {
        e.preventDefault();
        e.stopPropagation();
    });

    dropZone.on('dragover dragenter', function () {
        $(this).addClass('drag-over');
    });

    dropZone.on('dragleave dragend drop', function () {
        $(this).removeClass('drag-over');
    });

    dropZone.on('drop', function (e) {
        var files = e.originalEvent.dataTransfer.files;
        if (files.length > 0) {
            fileInput[0].files = files;
            showSelectedFile(files[0]);
        }
    });

    fileInput.on('change', function () {
        if (this.files.length > 0) {
            showSelectedFile(this.files[0]);
        }
    });

    function showSelectedFile(file) {
        var sizeInMB = (file.size / 1024 / 1024).toFixed(2);
        $('#DropZone').html(
            '<i class="fas fa-file-alt fa-3x text-success mb-3"></i>' +
            '<h5>' + file.name + '</h5>' +
            '<p class="text-muted">' + sizeInMB + ' MB</p>' +
            '<button type="button" class="btn btn-sm btn-outline-danger" id="ClearFileBtn">' +
            '<i class="fas fa-times"></i> ' + l('Remove') +
            '</button>'
        );
    }

    $(document).on('click', '#ClearFileBtn', function () {
        fileInput.val('');
        resetDropZone();
    });

    function resetDropZone() {
        $('#DropZone').html(
            '<i class="fas fa-cloud-upload-alt fa-3x text-muted mb-3"></i>' +
            '<h5>' + l('DragDropFiles') + '</h5>' +
            '<p class="text-muted">' + l('Or') + ' <label for="FileInput" class="text-primary" style="cursor: pointer;">' + l('ClickToSelect') + '</label></p>' +
            '<small class="text-muted">' + l('MaxFileSize') + ': 50 MB</small>'
        );
    }

    // Save button
    uploadModal.onResult(function () {
        var formData = new FormData();
        var file = fileInput[0].files[0];
        
        if (!file) {
            abp.message.error(l('PleaseSelectFile'));
            return;
        }

        // Check file size (50 MB max)
        if (file.size > 50 * 1024 * 1024) {
            abp.message.error(l('FileTooLarge'));
            return;
        }

        formData.append('file', file);
        formData.append('description', $('#DescriptionInput').val());
        formData.append('controlAssessmentId', $('#AssessmentSelect').val() || '');

        // Show progress
        $('#UploadProgress').show();
        $('#ProgressBar').css('width', '0%');

        abp.ajax({
            url: '/Evidence/Index?handler=Upload',
            type: 'POST',
            data: formData,
            processData: false,
            contentType: false,
            xhr: function () {
                var xhr = new window.XMLHttpRequest();
                xhr.upload.addEventListener('progress', function (e) {
                    if (e.lengthComputable) {
                        var percentComplete = (e.loaded / e.total) * 100;
                        $('#ProgressBar').css('width', percentComplete + '%');
                        $('#ProgressText').text(Math.round(percentComplete) + '% ' + l('Uploaded'));
                    }
                }, false);
                return xhr;
            },
            success: function (result) {
                if (result.success) {
                    abp.notify.success(result.message);
                    uploadModal.close();
                    location.reload(); // Reload to show new evidence
                } else {
                    abp.message.error(result.message);
                }
                $('#UploadProgress').hide();
            },
            error: function () {
                $('#UploadProgress').hide();
                abp.message.error(l('UploadFailed'));
            }
        });
    });

    uploadModal.onClose(function () {
        fileInput.val('');
        $('#DescriptionInput').val('');
        $('#AssessmentSelect').val('');
        $('#TagsInput').val('');
        $('#UploadProgress').hide();
        resetDropZone();
    });

    // View evidence
    $('.view-btn').on('click', function () {
        var id = $(this).data('id');
        abp.message.info('View evidence: ' + id);
        // TODO: Implement document viewer modal
    });

    // Download evidence
    $('.download-btn').on('click', function () {
        var id = $(this).data('id');
        window.location.href = '/api/evidence/' + id + '/download';
    });

    // Delete evidence
    $('.delete-btn').on('click', function () {
        var id = $(this).data('id');
        abp.message.confirm(
            l('EvidenceDeleteConfirmation'),
            l('AreYouSure'),
            function (confirmed) {
                if (confirmed) {
                    grc.evidence.evidence.delete(id)
                        .then(function () {
                            abp.notify.success(l('SuccessfullyDeleted'));
                            location.reload();
                        });
                }
            }
        );
    });

    // Filters
    $('#ApplyFiltersButton').on('click', function () {
        abp.message.info('Filters applied (not yet implemented)');
    });

    $('#SearchInput').on('keyup', function (e) {
        if (e.keyCode === 13) {
            $('#ApplyFiltersButton').click();
        }
    });
});

