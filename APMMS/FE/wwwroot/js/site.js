  // APMMS - Auto Parts Management & Maintenance System JavaScript

// NOTE: This file was accidentally cleared. The contents below rebuild the core UI
// interactions and authentication flows that other pages depend on.

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
        const href = $(this).attr('href');
        if (!href || href === '#') {
            return;
        }

        const target = $(href);
        if (target.length) {
            event.preventDefault();
            $('html, body').stop().animate({
                scrollTop: target.offset().top - 100
            }, 1000);
        }
    });

    // Initialize tooltips & popovers
    var tooltipTriggerList = [].slice.call(document.querySelectorAll('[data-bs-toggle="tooltip"]'));
    tooltipTriggerList.map(function (tooltipTriggerEl) {
        return new bootstrap.Tooltip(tooltipTriggerEl);
    });

    var popoverTriggerList = [].slice.call(document.querySelectorAll('[data-bs-toggle="popover"]'));
    popoverTriggerList.map(function (popoverTriggerEl) {
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

    // Search filtering
    $('#searchInput').on('keyup', function () {
        var value = $(this).val().toLowerCase();
        $('.table-modern tbody tr').filter(function () {
            $(this).toggle($(this).text().toLowerCase().indexOf(value) > -1);
        });
    });

    // Notifications
    $('.notification-item').on('click', function () {
        $(this).removeClass('bg-light').addClass('bg-white');
    });

    // Global form validation helper
    $('.needs-validation').on('submit', function (event) {
        if (this.checkValidity() === false) {
            event.preventDefault();
            event.stopPropagation();
        }
        $(this).addClass('was-validated');
    });

    // Simple loading demonstration for buttons
    $('.btn-loading').on('click', function () {
        var btn = $(this);
        var originalText = btn.html();
        btn.html('<i class="fas fa-spinner fa-spin"></i> Đang xử lý...');
        btn.prop('disabled', true);

        setTimeout(function () {
            btn.html(originalText);
            btn.prop('disabled', false);
        }, 2000);
    });

    // Modal content loader
    $('.modal').on('show.bs.modal', function (event) {
        var button = $(event.relatedTarget);
        var modal = $(this);

        if (button && button.data('load')) {
            var url = button.data('load');
            modal.find('.modal-body').load(url);
        }
    });

    console.log('Initializing authentication...');
    initializeAuth();
    console.log('APMMS Frontend initialized successfully');
    initializeFloatingTab();
});

// ---------------------------------------------------------------------------
// Authentication helpers
// ---------------------------------------------------------------------------

function initializeAuth() {
    checkLoginStatus();

    $('.login-form').on('submit', function (e) {
        e.preventDefault();
        e.stopPropagation();

        if ($(this).find('#username').length > 0) {
            handleLogin();
        } else {
            handleRegister();
        }
        return false;
    });

    $('#logoutBtn').on('click', function (e) {
        e.preventDefault();
        handleLogout();
    });
}

async function checkLoginStatus() {
    const token = localStorage.getItem('authToken');

    if (!token) {
        showLoginButton();
        return;
    }

    try {
        const response = await fetch('/Auth/GetUserInfo', {
            method: 'GET',
            headers: { 'Content-Type': 'application/json' }
        });

        const result = await response.json();

        if (result.isLoggedIn) {
            const userInfo = {
                username: result.username,
                fullName: result.fullName || result.username,
                email: result.email || 'user@example.com',
                role: result.role || getRoleName(result.roleId),
                roleId: result.roleId || 0,
                userId: result.userId || result.id || null,
                id: result.userId || result.id || null
            };
            localStorage.setItem('userInfo', JSON.stringify(userInfo));
            localStorage.setItem('isLoggedIn', 'true');
            showProfileDropdown(userInfo);
        } else {
            showLoginButton();
        }
    } catch (error) {
        console.error('Check login status error:', error);
        showLoginButton();
    }
}

async function handleLogin() {
    const username = $('#username').val();
    const password = $('#password').val();

    if (!username || !password) {
        showAlert('Vui lòng nhập đầy đủ thông tin đăng nhập', 'warning');
        return;
    }

    const loginBtn = $('.btn-login');
    const originalText = loginBtn.html();
    loginBtn.html('<i class="fas fa-spinner fa-spin"></i> Đang đăng nhập...');
    loginBtn.prop('disabled', true);

    try {
        const response = await fetch('/Auth/Login', {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({ username, password })
        });

        if (!response.ok) {
            throw new Error(`HTTP error! status: ${response.status}`);
        }

        const result = await response.json();

        if (result.success) {
            let userId = result.userId;
            if (!userId && result.token) {
                userId = decodeUserIdFromToken(result.token);
            }

            localStorage.setItem('authToken', result.token);
            localStorage.setItem('isLoggedIn', 'true');
            localStorage.setItem('userInfo', JSON.stringify({
                username,
                fullName: result.fullName || username,
                email: result.email || 'user@example.com',
                role: getRoleName(result.roleId),
                roleId: result.roleId,
                userId,
                id: userId,
                token: result.token
            }));

            $('#loginModal').modal('hide');
            showProfileDropdown({
                username,
                fullName: result.fullName || username,
                email: result.email || 'user@example.com',
                role: getRoleName(result.roleId)
            });
            showAlert('Đăng nhập thành công!', 'success');

            if (result.redirectTo && result.redirectTo !== '/') {
                setTimeout(() => {
                    window.location.href = result.redirectTo;
                }, 1500);
            }
        } else {
            showAlert(result.error || 'Đăng nhập thất bại', 'danger');
        }
    } catch (error) {
        console.error('Login error:', error);
        showAlert('Lỗi kết nối đến server', 'danger');
    } finally {
        loginBtn.html(originalText);
        loginBtn.prop('disabled', false);
    }
}

function decodeUserIdFromToken(token) {
    if (!token) return null;
    try {
        const payload = JSON.parse(atob(token.split('.')[1]));
        return payload['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier'] ||
               payload['UserId'] ||
               payload['nameid'] ||
               payload['sub'];
    } catch (error) {
        console.error('Error decoding token:', error);
        return null;
    }
}

async function handleRegister() {
    const fullName = $('#regFullName').val();
    const email = $('#regEmail').val();
    const phone = $('#regPhone').val();
    const username = $('#regUsername').val();
    const password = $('#regPassword').val();
    const confirmPassword = $('#regConfirmPassword').val();
    const agreeTerms = $('#agreeTerms').is(':checked');

    if (!fullName || !email || !phone || !username || !password || !confirmPassword) {
        showAlert('Vui lòng nhập đầy đủ thông tin', 'warning');
        return;
    }

    if (password !== confirmPassword) {
        showAlert('Mật khẩu xác nhận không khớp', 'warning');
        return;
    }

    if (!agreeTerms) {
        showAlert('Vui lòng đồng ý với điều khoản sử dụng', 'warning');
        return;
    }

    const registerBtn = $('.btn-register');
    const originalText = registerBtn.html();
    registerBtn.html('<i class="fas fa-spinner fa-spin"></i> Đang đăng ký...');
    registerBtn.prop('disabled', true);

    try {
        const response = await fetch('/Auth/Register', {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({ fullName, email, phone, username, password })
        });

        const result = await response.json();

        if (result.success) {
            showAlert('Đăng ký thành công! Vui lòng đăng nhập.', 'success');
            $('#registerModal').modal('hide');
            $('#loginModal').modal('show');
            $('#username').val(username);
        } else {
            showAlert(result.error || 'Đăng ký thất bại', 'danger');
        }
    } catch (error) {
        console.error('Register error:', error);
        showAlert('Lỗi kết nối đến server', 'danger');
    } finally {
        registerBtn.html(originalText);
        registerBtn.prop('disabled', false);
    }
}

async function handleLogout() {
    try {
        await fetch('/Auth/Logout', {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' }
        });
    } catch (error) {
        console.error('Logout error:', error);
    } finally {
        localStorage.removeItem('authToken');
        localStorage.removeItem('isLoggedIn');
        localStorage.removeItem('userInfo');

        showLoginButton();
        showAlert('Đã đăng xuất', 'info');

        // Đảm bảo đóng dropdown và refresh giao diện
        try {
            const dropdown = bootstrap.Dropdown.getInstance(document.getElementById('profileDropdownToggle'));
            if (dropdown) {
                dropdown.hide();
            }
        } catch (err) {
            console.warn('Không thể đóng dropdown profile:', err);
        }

        // Reload trang để đồng bộ trạng thái giữa các layout
        setTimeout(() => {
            window.location.href = '/';
        }, 300);
    }
}

function showProfileDropdown(userInfo) {
    $('#loginBtn').hide();
    $('#profileDropdown').show();
    $('#profileName').text(userInfo.fullName || userInfo.username || 'Người dùng');
}

function showLoginButton() {
    $('#profileDropdown').hide();
    $('#loginBtn').show();
}

function showAlert(message, type = 'info') {
    const alertHtml = `
        <div class="alert alert-${type} alert-dismissible fade show position-fixed"
             style="top: 20px; right: 20px; z-index: 9999; min-width: 300px;" role="alert">
            ${message}
            <button type="button" class="btn-close" data-bs-dismiss="alert"></button>
        </div>
    `;

    $('body').append(alertHtml);
    setTimeout(() => {
        $('.alert').alert('close');
    }, 3000);
}

function getRoleName(roleId) {
    switch (roleId) {
        case 1: return 'Admin';
        case 2: return 'Branch Manager';
        case 3: return 'Accountant';
        case 4: return 'Technician';
        case 5: return 'Warehouse Keeper';
        case 6: return 'Consulter';
        case 7: return 'Auto Owner';
        case 8: return 'Guest';
        default: return 'User';
    }
}

function showRegisterModal() {
    $('#loginModal').modal('hide');
    $('#registerModal').modal('show');
}

function showLoginModal() {
    $('#registerModal').modal('hide');
    $('#loginModal').modal('show');
}

// ---------------------------------------------------------------------------
// Floating tab shortcuts (booking/contact)
// ---------------------------------------------------------------------------

function initializeFloatingTab() {
    $('.floating-tab-toggle').on('click', function () {
        $('.floating-tab').toggleClass('collapsed');
        const arrow = $(this).find('i');
        if ($('.floating-tab').hasClass('collapsed')) {
            arrow.removeClass('fa-chevron-left').addClass('fa-chevron-right');
        } else {
            arrow.removeClass('fa-chevron-right').addClass('fa-chevron-left');
        }
    });

    $('.floating-tab-item[data-action="booking"]').on('click', function (e) {
        e.preventDefault();
        e.stopPropagation();
        $('#bookingModal').modal('show');
    });

    $('.floating-tab-item[data-action="messenger"]').on('click', function () {
        window.open('https://m.me/your-page', '_blank');
    });

    $('.floating-tab-item[data-action="hotline"]').on('click', function () {
        window.open('tel:0123456789', '_self');
    });

    $('.floating-tab-item[data-action="zalo"]').on('click', function () {
        window.open('https://zalo.me/0123456789', '_blank');
    });

    $('.floating-tab-item[data-action="directions"]').on('click', function () {
        window.open('https://maps.google.com/?q=your-address', '_blank');
    });
}

// Expose helpers globally when needed elsewhere
window.showRegisterModal = showRegisterModal;
window.showLoginModal = showLoginModal;
