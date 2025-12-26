(function () {
  abp.modals.FrameworkLibraryCreateModal = function () {
    var modalManager;

    function unwrapItems(res) {
      return (res && (res.items || (res.result && res.result.items))) || res || [];
    }

    function regulatorText(r) {
      return r.name || r.displayName || r.title || r.code || r.id || '(no-name)';
    }

    function loadRegulators() {
      return abp.ajax({
        url: abp.appPath + 'api/app/framework-library/regulator',
        type: 'GET'
      }).then(function (res) {
        var items = unwrapItems(res);
        var $sel = $('#RegulatorId');
        $sel.empty();
        $sel.append('<option value="">-- Select Regulator --</option>');

        items.forEach(function (r) {
          var id = r.id || r.regulatorId;
          if (!id) return;
          $sel.append('<option value="' + id + '">' + regulatorText(r) + '</option>');
        });

        if ($sel.children().length === 1) {
          $sel.empty().append('<option value="">(No regulators returned)</option>');
        }
      }).catch(function (e) {
        $('#RegulatorId').empty().append('<option value="">(Failed to load)</option>');
        abp.message.error(e && e.message ? e.message : 'Failed to load regulators');
      });
    }

    this.init = function (_modalManager) {
      modalManager = _modalManager;
      loadRegulators();
    };

    this.save = function () {
      var payload = {
        regulatorId: $('#RegulatorId').val() || null,
        code: $('#Code').val(),
        version: $('#Version').val() || null,
        status: $('#Status').val() || null,
        titleEn: $('#TitleEn').val() || null,
        titleAr: $('#TitleAr').val() || null,
        effectiveDate: $('#EffectiveDate').val() || null,
        isMandatory: $('#IsMandatory').is(':checked')
      };

      if (!payload.regulatorId) {
        abp.message.warn('Regulator is required.');
        return;
      }
      if (!payload.code) {
        abp.message.warn('Code is required.');
        return;
      }

      modalManager.setBusy(true);

      abp.ajax({
        url: abp.appPath + 'api/app/framework-library/framework',
        type: 'POST',
        contentType: 'application/json',
        data: JSON.stringify(payload)
      }).then(function () {
        abp.notify.success('Framework created');
        modalManager.close();
        modalManager.setResult(true);
      }).always(function () {
        modalManager.setBusy(false);
      });
    };
  };
})();
