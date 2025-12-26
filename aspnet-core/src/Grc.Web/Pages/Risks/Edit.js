(function () {
  function setDate($el, val) {
    if (!val) return;
    $el.val(String(val).substring(0, 10));
  }

  function loadRisk(id) {
    return abp.ajax({
      url: abp.appPath + 'api/app/risk/' + id,
      type: 'GET'
    }).then(function (r) {
      $('#Title').val(r.title || '');
      $('#Description').val(r.description || '');
      $('#Category').val(r.category || 'Cyber');
      $('#Likelihood').val(r.likelihood ?? 3);
      $('#Impact').val(r.impact ?? 3);
      setDate($('#DueDate'), r.dueDate);
      $('#Status').val(r.status || 'Open');
    });
  }

  $(function () {
    var id = $('#Id').val();
    if (!id) {
      abp.message.error('Missing risk id in route.');
      return;
    }

    abp.ui.setBusy();
    loadRisk(id).always(function () { abp.ui.clearBusy(); });

    $('#RiskEditForm').on('submit', function (e) {
      e.preventDefault();

      var payload = {
        title: $('#Title').val(),
        description: $('#Description').val() || null,
        category: $('#Category').val() || null,
        likelihood: Number($('#Likelihood').val() || 0) || null,
        impact: Number($('#Impact').val() || 0) || null,
        dueDate: $('#DueDate').val() || null,
        status: $('#Status').val() || null
      };

      if (!payload.title) {
        abp.message.warn('Title is required.');
        return;
      }

      abp.ui.setBusy();
      abp.ajax({
        url: abp.appPath + 'api/app/risk/' + id,
        type: 'PUT',
        contentType: 'application/json',
        data: JSON.stringify(payload)
      }).then(function () {
        abp.notify.success('Risk updated');
        window.location.href = '/Risks';
      }).always(function () {
        abp.ui.clearBusy();
      });
    });
  });
})();
