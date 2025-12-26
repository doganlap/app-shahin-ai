(function () {
  function unwrapItems(res) {
    return (res && (res.items || (res.result && res.result.items))) || res || [];
  }

  function frameworkText(f) {
    return f.code
      ? (f.code + ' — ' + (f.titleEn || (f.title && (f.title.en || f.title.En)) || f.name || ''))
      : (f.titleEn || f.name || f.id);
  }

  function loadFrameworksMultiSelect() {
    return abp.ajax({
      url: abp.appPath + 'api/app/framework-library/framework',
      type: 'GET'
    }).then(function (res) {
      var items = unwrapItems(res);
      var $sel = $('#FrameworkIds');
      $sel.empty();

      items.forEach(function (f) {
        var id = f.id;
        if (!id) return;
        $sel.append('<option value="' + id + '">' + frameworkText(f) + '</option>');
      });
    }).catch(function () {
      // Optional field; do not block create.
      $('#FrameworkIds').empty();
    });
  }

  $(function () {
    loadFrameworksMultiSelect();

    $('#AssessmentCreateForm').on('submit', function (e) {
      e.preventDefault();

      var selectedFrameworkIds = ($('#FrameworkIds').val() || []).filter(Boolean);

      var payload = {
        name: $('#Name').val(),
        type: $('#Type').val() || null,
        status: $('#Status').val() || null,
        startDate: $('#StartDate').val() || null,
        targetEndDate: $('#TargetEndDate').val() || null,
        frameworkIds: selectedFrameworkIds // if your API doesn't accept it, remove this line
      };

      if (!payload.name) {
        abp.message.warn('Name is required.');
        return;
      }

      abp.ui.setBusy();
      abp.ajax({
        url: abp.appPath + 'api/app/assessment',
        type: 'POST',
        contentType: 'application/json',
        data: JSON.stringify(payload)
      }).then(function (created) {
        abp.notify.success('Assessment created');

        var id = created && (created.id || created.assessmentId);
        if (id) {
          window.location.href = '/Assessments/Details/' + id;
        } else {
          window.location.href = '/Assessments';
        }
      }).always(function () {
        abp.ui.clearBusy();
      });
    });
  });
})();
