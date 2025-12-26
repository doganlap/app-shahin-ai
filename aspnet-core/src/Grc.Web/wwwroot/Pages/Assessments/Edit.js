(function () {
  function setDate($el, val) {
    if (!val) return;
    $el.val(String(val).substring(0, 10));
  }

  function loadAssessment(id) {
    return abp.ajax({
      url: abp.appPath + 'api/app/assessment/' + id,
      type: 'GET'
    }).then(function (a) {
      $('#Name').val(a.name || '');
      $('#Type').val(a.type || 'SelfAssessment');
      $('#Status').val(a.status || 'Draft');
      setDate($('#StartDate'), a.startDate);
      setDate($('#TargetEndDate'), a.targetEndDate);

      $('#BackLink').attr('href', '/Assessments/Details/' + id);
    });
  }

  $(function () {
    var id = $('#Id').val();
    if (!id) {
      abp.message.error('Missing assessment id in route.');
      return;
    }

    abp.ui.setBusy();
    loadAssessment(id).always(function () { abp.ui.clearBusy(); });

    $('#AssessmentEditForm').on('submit', function (e) {
      e.preventDefault();

      var payload = {
        name: $('#Name').val(),
        type: $('#Type').val() || null,
        status: $('#Status').val() || null,
        startDate: $('#StartDate').val() || null,
        targetEndDate: $('#TargetEndDate').val() || null
      };

      if (!payload.name) {
        abp.message.warn('Name is required.');
        return;
      }

      abp.ui.setBusy();
      abp.ajax({
        url: abp.appPath + 'api/app/assessment/' + id,
        type: 'PUT',
        contentType: 'application/json',
        data: JSON.stringify(payload)
      }).then(function () {
        abp.notify.success('Assessment updated');
        window.location.href = '/Assessments/Details/' + id;
      }).always(function () {
        abp.ui.clearBusy();
      });
    });
  });
})();
