(function () {
  abp.modals.FrameworkLibraryEditModal = function () {
    var modalManager;

    function unwrapItems(res) {
      return (res && (res.items || (res.result && res.result.items))) || res || [];
    }

    function regulatorText(r) {
      return r.name || r.displayName || r.title || r.code || r.id || '(no-name)';
    }

    function loadRegulators(selectedId) {
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

        if (selectedId) $sel.val(selectedId);
      });
    }

    function normalizeTitleEn(obj) {
      return obj.titleEn || (obj.title && (obj.title.en || obj.title.En)) || obj.nameEn || '';
    }
    function normalizeTitleAr(obj) {
      return obj.titleAr || (obj.title && (obj.title.ar || obj.title.Ar)) || obj.nameAr || '';
    }

    function loadFramework(id) {
      return abp.ajax({
        url: abp.appPath + 'api/app/framework-library/framework/' + id,
        type: 'GET'
      }).then(function (f) {
        $('#Code').val(f.code || '');
        $('#Version').val(f.version || '');
        $('#Status').val(f.status || 'Active');
        $('#TitleEn').val(normalizeTitleEn(f));
        $('#TitleAr').val(normalizeTitleAr(f));
        $('#IsMandatory').prop('checked', !!(f.isMandatory || f.mandatory));

        var dateVal = f.effectiveDate || f.effective || null;
        if (dateVal) {
          // If server returns ISO datetime, keep first 10 chars.
          $('#EffectiveDate').val(String(dateVal).substring(0, 10));
        }

        var regulatorId = f.regulatorId || (f.regulator && f.regulator.id) || null;
        return loadRegulators(regulatorId);
      });
    }

    this.init = function (_modalManager) {
      modalManager = _modalManager;

      var id = $('#Id').val();
      if (!id) {
        abp.message.error('Missing framework id in route.');
        return;
      }

      modalManager.setBusy(true);
      loadFramework(id).always(function () {
        modalManager.setBusy(false);
      });
    };

    this.save = function () {
      var id = $('#Id').val();

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
        url: abp.appPath + 'api/app/framework-library/framework/' + id,
        type: 'PUT',
        contentType: 'application/json',
        data: JSON.stringify(payload)
      }).then(function () {
        abp.notify.success('Framework updated');
        modalManager.close();
        modalManager.setResult(true);
      }).always(function () {
        modalManager.setBusy(false);
      });
    };
  };
})();
