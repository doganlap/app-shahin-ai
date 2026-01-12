(function () {
  function setText(field, val) {
    $('[data-field="' + field + '"]').text(val ?? '-');
  }

  function fmtDate(val) {
    if (!val) return '-';
    return String(val).substring(0, 10);
  }

  function unwrapItems(res) {
    return (res && (res.items || (res.result && res.result.items))) || res || [];
  }

  function loadAssessment(id) {
    return abp.ajax({
      url: abp.appPath + 'api/app/assessment/' + id,
      type: 'GET'
    }).then(function (a) {
      setText('name', a.name || '-');
      setText('type', a.type || '-');
      setText('status', a.status || '-');
      setText('startDate', fmtDate(a.startDate));
      setText('targetEndDate', fmtDate(a.targetEndDate));
    });
  }

  function loadControls(id) {
    return abp.ajax({
      url: abp.appPath + 'api/app/assessment/' + id + '/controls',
      type: 'GET'
    }).then(function (res) {
      var items = unwrapItems(res);
      var $tb = $('#ControlsTable tbody');
      $tb.empty();

      if (!items.length) {
        $tb.append('<tr><td colspan="4">(No controls)</td></tr>');
        return;
      }

      items.forEach(function (c, idx) {
        var name = c.controlName || c.titleEn || c.title || c.controlCode || c.controlNumber || c.id;
        var status = c.status || c.state || '-';
        var score = c.selfScore ?? c.verifiedScore ?? c.score ?? '-';

        $tb.append(
          '<tr>' +
            '<td>' + (idx + 1) + '</td>' +
            '<td>' + (name || '-') + '</td>' +
            '<td>' + status + '</td>' +
            '<td>' + score + '</td>' +
          '</tr>'
        );
      });
    }).catch(function () {
      $('#ControlsTable tbody').html('<tr><td colspan="4">(Failed to load)</td></tr>');
    });
  }

  function postAction(url, successMsg) {
    abp.ui.setBusy();
    return abp.ajax({ url: url, type: 'POST' })
      .then(function () { abp.notify.success(successMsg); })
      .always(function () { abp.ui.clearBusy(); });
  }

  $(function () {
    var id = $('#Id').val();
    if (!id) {
      abp.message.error('Missing assessment id in route.');
      return;
    }

    abp.ui.setBusy();
    $.when(loadAssessment(id), loadControls(id)).always(function () {
      abp.ui.clearBusy();
    });

    $('#StartBtn').on('click', function () {
      postAction(abp.appPath + 'api/app/assessment/' + id + '/start', 'Assessment started')
        .then(function () { return loadAssessment(id); });
    });

    $('#CompleteBtn').on('click', function () {
      postAction(abp.appPath + 'api/app/assessment/' + id + '/complete', 'Assessment completed')
        .then(function () { return loadAssessment(id); });
    });
  });
})();
