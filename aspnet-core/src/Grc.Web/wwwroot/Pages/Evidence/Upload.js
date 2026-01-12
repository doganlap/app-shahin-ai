(function () {
  $(function () {
    $('#EvidenceUploadForm').on('submit', function (e) {
      e.preventDefault();

      var fileInput = document.getElementById('File');
      if (!fileInput.files || !fileInput.files.length) {
        abp.message.warn('File is required.');
        return;
      }

      var fd = new FormData();

      // Common server expectations:
      // - file / File / Upload
      // Start with "file" (adjust if needed).
      fd.append('file', fileInput.files[0]);

      // Metadata (adjust keys if your backend differs)
      fd.append('evidenceType', $('#EvidenceType').val() || '');
      fd.append('description', $('#Description').val() || '');
      fd.append('relatedType', $('#RelatedType').val() || '');
      fd.append('relatedId', $('#RelatedId').val() || '');

      abp.ui.setBusy();
      abp.ajax({
        url: abp.appPath + 'api/app/evidence/upload',
        type: 'POST',
        data: fd,
        processData: false,
        contentType: false
      }).then(function () {
        abp.notify.success('Evidence uploaded');
        window.location.href = '/Evidence';
      }).always(function () {
        abp.ui.clearBusy();
      });
    });
  });
})();
