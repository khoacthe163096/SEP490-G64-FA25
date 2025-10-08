// APMMS - Auto Parts Management & Maintenance System JavaScript

$(document).ready(function () {
    // Sidebar toggle functionality
    $('#sidebarCollapse').on('click', function () {
        $('#sidebar').toggleClass('active');
        $('#content').toggleClass('active');
    });

    // Auto-hide sidebar on mobile
    $(window).on('resize', function () {
        if ($(window).width() <= 768) {
            $('#sidebar').removeClass('active');
            $('#content').removeClass('active');
        }
    });

    // Smooth scrolling for anchor links
    $('a[href^="#"]').on('click', function (event) {
        var target = $(this.getAttribute('href'));
        if (target.length) {
            event.preventDefault();
            $('html, body').stop().animate({
                scrollTop: target.offset().top - 100
            }, 1000);
        }
    });

    // Initialize tooltips
    var tooltipTriggerList = [].slice.call(document.querySelectorAll('[data-bs-toggle="tooltip"]'));
    var tooltipList = tooltipTriggerList.map(function (tooltipTriggerEl) {
        return new bootstrap.Tooltip(tooltipTriggerEl);
    });

    // Initialize popovers
    var popoverTriggerList = [].slice.call(document.querySelectorAll('[data-bs-toggle="popover"]'));
    var popoverList = popoverTriggerList.map(function (popoverTriggerEl) {
        return new bootstrap.Popover(popoverTriggerEl);
    });

    // Dashboard animations
    $('.dashboard-card').each(function (index) {
        $(this).css('animation-delay', (index * 0.1) + 's');
    });

    // Table row click handlers
    $('.table-modern tbody tr').on('click', function () {
        $(this).addClass('table-active').siblings().removeClass('table-active');
    });

    // Search functionality
    $('#searchInput').on('keyup', function () {
        var value = $(this).val().toLowerCase();
        $('.table-modern tbody tr').filter(function () {
            $(this).toggle($(this).text().toLowerCase().indexOf(value) > -1);
        });
    });

    // Notification handling
    $('.notification-item').on('click', function () {
        $(this).removeClass('bg-light');
        $(this).addClass('bg-white');
    });

    // Form validation
    $('.needs-validation').on('submit', function (event) {
        if (this.checkValidity() === false) {
            event.preventDefault();
            event.stopPropagation();
        }
        $(this).addClass('was-validated');
    });

    // Auto-refresh dashboard data every 30 seconds
    setInterval(function () {
        // This would typically make an AJAX call to refresh data
        console.log('Refreshing dashboard data...');
    }, 30000);

    // Print functionality
    $('.btn-print').on('click', function () {
        window.print();
    });

    // Export functionality
    $('.btn-export').on('click', function () {
        var format = $(this).data('format');
        console.log('Exporting data as:', format);
        // Implement export logic here
    });

    // Modal handling
    $('.modal').on('show.bs.modal', function (event) {
        var button = $(event.relatedTarget);
        var modal = $(this);
        
        // Load data into modal if needed
        if (button.data('load')) {
            var url = button.data('load');
            modal.find('.modal-body').load(url);
        }
    });

    // Confirmation dialogs
    $('.btn-delete').on('click', function (e) {
        if (!confirm('Bạn có chắc chắn muốn xóa mục này?')) {
            e.preventDefault();
        }
    });

    // Loading states
    $('.btn-loading').on('click', function () {
        var btn = $(this);
        var originalText = btn.html();
        
        btn.html('<i class="fas fa-spinner fa-spin"></i> Đang xử lý...');
        btn.prop('disabled', true);
        
        // Simulate loading
        setTimeout(function () {
            btn.html(originalText);
            btn.prop('disabled', false);
        }, 2000);
    });

    // Chart initialization (if Chart.js is available)
    if (typeof Chart !== 'undefined') {
        // Initialize charts here
        console.log('Chart.js is available');
    }

    // Real-time updates simulation
    function updateStats() {
        $('.dashboard-card h3').each(function () {
            var currentValue = parseInt($(this).text().replace(/[^\d]/g, ''));
            var newValue = currentValue + Math.floor(Math.random() * 3) - 1;
            $(this).text(newValue.toLocaleString());
        });
    }

    // Update stats every 5 minutes
    setInterval(updateStats, 300000);

    // Initialize date pickers
    $('.datepicker').datepicker({
        format: 'dd/mm/yyyy',
        autoclose: true,
        todayHighlight: true
    });

    // Initialize time pickers
    $('.timepicker').timepicker({
        showMeridian: false,
        minuteStep: 15
    });

    // File upload handling
    $('.file-upload').on('change', function () {
        var fileName = $(this).val().split('\\').pop();
        $(this).siblings('.file-name').text(fileName);
    });

    // Auto-save functionality
    $('.auto-save').on('input', function () {
        var field = $(this);
        var timeout;
        
        clearTimeout(timeout);
        timeout = setTimeout(function () {
            // Implement auto-save logic here
            field.addClass('saving');
            setTimeout(function () {
                field.removeClass('saving').addClass('saved');
                setTimeout(function () {
                    field.removeClass('saved');
                }, 2000);
            }, 1000);
        }, 2000);
    });

    // Keyboard shortcuts
    $(document).on('keydown', function (e) {
        // Ctrl + N for new item
        if (e.ctrlKey && e.keyCode === 78) {
            e.preventDefault();
            $('.btn-new').click();
        }
        
        // Ctrl + S for save
        if (e.ctrlKey && e.keyCode === 83) {
            e.preventDefault();
            $('.btn-save').click();
        }
        
        // Escape to close modals
        if (e.keyCode === 27) {
            $('.modal').modal('hide');
        }
    });

    // Initialize all components
    console.log('APMMS Frontend initialized successfully');
});