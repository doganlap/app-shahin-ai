(function () {
  $(function () {
    $('#RiskCreateForm').on('submit', function (e) {
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
        url: abp.appPath + 'api/app/risk',
        type: 'POST',
        contentType: 'application/json',
        data: JSON.stringify(payload)
      }).then(function (created) {
        abp.notify.success('Risk created');

        var id = created && (created.id || created.riskId);
        if (id) {
          window.location.href = '/Risks/Edit/' + id;
        } else {
          window.location.href = '/Risks';
        }
      }).always(function () {
        abp.ui.clearBusy();
      });
    });
  });
})();
