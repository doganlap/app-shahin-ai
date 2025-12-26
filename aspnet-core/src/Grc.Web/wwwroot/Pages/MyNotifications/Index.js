(function () {
    $(document).ready(function () {
        // Mark individual notification as read
        $('.mark-read-btn').on('click', function () {
            var $btn = $(this);
            var $row = $btn.closest('tr');
            
            $row.removeClass('table-info');
            $btn.closest('td').html('<span class="text-muted"><i class="fas fa-check"></i> Marked</span>');
            
            abp.notify.success('Notification marked as read');
        });

        // Mark all as read
        $('#MarkAllReadBtn').on('click', function () {
            $('.table-info').removeClass('table-info');
            $('.mark-read-btn').closest('td').html('<span class="text-muted"><i class="fas fa-check"></i> Marked</span>');
            
            abp.notify.success('All notifications marked as read');
        });
    });
})();

