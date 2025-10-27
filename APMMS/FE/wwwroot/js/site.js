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

    // Initialize date pickers (commented out - requires jQuery UI)
    // $('.datepicker').datepicker({
    //     format: 'dd/mm/yyyy',
    //     autoclose: true,
    //     todayHighlight: true
    // });

    // Initialize time pickers (commented out - requires timepicker library)
    // $('.timepicker').timepicker({
    //     showMeridian: false,
    //     minuteStep: 15
    // });

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

    // Authentication and Profile Management
    console.log('Initializing authentication...');
    initializeAuth();
    
    // Initialize all components
    console.log('APMMS Frontend initialized successfully');
    
    // Initialize Floating Tab
    initializeFloatingTab();
});

// Authentication and Profile Management Functions
function initializeAuth() {
    console.log('initializeAuth called');
    // Check if user is logged in on page load
    checkLoginStatus();
    
    // Handle login form submission
    console.log('Binding login form submit handler...');
    console.log('Found login forms:', $('.login-form').length);
    $('.login-form').on('submit', function(e) {
        console.log('Form submitted, preventing default');
        console.log('Form element:', this);
        console.log('Form ID:', $(this).attr('id'));
        e.preventDefault();
        e.stopPropagation();
        
        // Check if it's login or register form
        if ($(this).find('#username').length > 0) {
            console.log('Calling handleLogin');
            handleLogin();
        } else {
            console.log('Calling handleRegister');
            handleRegister();
        }
        return false;
    });
    
    // Handle logout
    $('#logoutBtn').on('click', function(e) {
        e.preventDefault();
        handleLogout();
    });
}

async function checkLoginStatus() {
    try {
        // Check token first
        const token = localStorage.getItem('authToken');
        console.log('Token in localStorage:', token ? 'Found' : 'Not found');
        
        if (!token) {
            console.log('No token found, showing login button');
            showLoginButton();
            return;
        }
        
        // Check server for login status
        const response = await fetch('/Auth/GetUserInfo', {
            method: 'GET',
            headers: {
                'Content-Type': 'application/json',
            }
        });
        
        const result = await response.json();
        
        if (result.isLoggedIn) {
            const userInfo = {
                username: result.username,
                fullName: result.username,
                email: 'user@example.com',
                role: 'Auto Owner'
            };
            showProfileDropdown(userInfo);
        } else {
            showLoginButton();
        }
    } catch (error) {
        console.error('Check login status error:', error);
        // Fallback to authToken
        const token = localStorage.getItem('authToken');
        
        if (token) {
            try {
                // Decode token để lấy username
                const payload = JSON.parse(atob(token.split('.')[1]));
                const username = payload['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name'];
                
                if (username) {
                    showProfileDropdown({
                        username: username,
                        fullName: username,
                        email: 'user@example.com',
                        role: 'User'
                    });
                } else {
                    showLoginButton();
                }
            } catch (e) {
                showLoginButton();
            }
        } else {
            showLoginButton();
        }
    }
}

async function handleLogin() {
    console.log('handleLogin called');
    const username = $('#username').val();
    const password = $('#password').val();
    
    console.log('Username:', username);
    console.log('Password length:', password ? password.length : 0);
    console.log('Form element:', $('#loginForm')[0]);
    console.log('Username element:', $('#username')[0]);
    console.log('Password element:', $('#password')[0]);
    
    if (!username || !password) {
        showAlert('Vui lòng nhập đầy đủ thông tin đăng nhập', 'warning');
        return;
    }
    
    // Show loading state
    const loginBtn = $('.btn-login');
    const originalText = loginBtn.html();
    loginBtn.html('<i class="fas fa-spinner fa-spin"></i> Đang đăng nhập...');
    loginBtn.prop('disabled', true);
    
    try {
        console.log('Calling Frontend AuthController: /Auth/Login');
        // Call Frontend AuthController (which calls backend and sets session)
        const response = await fetch('/Auth/Login', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
            },
            body: JSON.stringify({
                username: username,
                password: password
            })
        });
        
        console.log('Response status:', response.status);
        
        if (!response.ok) {
            throw new Error(`HTTP error! status: ${response.status}`);
        }
        
        const responseText = await response.text();
        console.log('Response text:', responseText);
        
        let result;
        try {
            result = JSON.parse(responseText);
        } catch (jsonError) {
            console.error('JSON parse error:', jsonError);
            console.error('Response text:', responseText);
            throw new Error(`Invalid JSON response: ${jsonError.message}`);
        }
        
        console.log('Response result:', result);
        console.log('Success status:', result.success);
        console.log('Token:', result.token ? 'Present' : 'Missing');
        console.log('Role ID:', result.roleId);
        console.log('Redirect To:', result.redirectTo);
        
        if (result.success) {
            // Store login info
            localStorage.setItem('authToken', result.token);
            localStorage.setItem('isLoggedIn', 'true');
            localStorage.setItem('userInfo', JSON.stringify({
                username: username,
                fullName: username,
                email: 'user@example.com',
                role: getRoleName(result.roleId),
                roleId: result.roleId,
                token: result.token
            }));
            
            // Hide modal
            $('#loginModal').modal('hide');
            
            // Check if user should be redirected
            console.log('Checking redirect logic:');
            console.log('Role ID:', result.roleId);
            console.log('Redirect URL:', result.redirectTo);
            
            if (result.redirectTo && result.redirectTo !== '/') {
                console.log('Redirecting to:', result.redirectTo);
                showAlert('Đăng nhập thành công! Chuyển hướng đến Dashboard...', 'success');
                setTimeout(() => {
                    console.log('Executing redirect to:', result.redirectTo);
                    window.location.href = result.redirectTo;
                }, 1500);
            } else {
                // Show profile dropdown for Auto Owner/Guest
                showProfileDropdown({
                    username: username,
                    fullName: username,
                    email: 'user@example.com',
                    role: getRoleName(result.roleId)
                });
                showAlert('Đăng nhập thành công!', 'success');
            }
        } else {
            showAlert(result.error || 'Đăng nhập thất bại', 'danger');
        }
    } catch (error) {
        console.error('Login error:', error);
        showAlert('Lỗi kết nối đến server', 'danger');
    } finally {
        // Reset form and button
        $('.login-form')[0].reset();
        loginBtn.html(originalText);
        loginBtn.prop('disabled', false);
    }
}

async function handleLogout() {
    try {
        // Call logout API
        const response = await fetch('/Auth/Logout', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
            }
        });
        
        const result = await response.json();
        
        if (result.success) {
            // Clear login info
            localStorage.removeItem('isLoggedIn');
            localStorage.removeItem('userInfo');
            
            // Show login button
            showLoginButton();
            
            showAlert('Đã đăng xuất thành công!', 'info');
        } else {
            showAlert('Lỗi khi đăng xuất', 'warning');
        }
    } catch (error) {
        console.error('Logout error:', error);
        // Still clear local storage even if API call fails
        localStorage.removeItem('isLoggedIn');
        localStorage.removeItem('userInfo');
        showLoginButton();
        showAlert('Đã đăng xuất', 'info');
    }
}

function showProfileDropdown(userInfo) {
    $('#loginBtn').hide();
    $('#profileDropdown').show();
    
    // Update profile name
    if (userInfo.username) {
        $('#profileName').text(userInfo.username);
    }
    
    // Update profile info
    $('#profileName').text(userInfo.fullName || userInfo.username);
    
    // Update avatar if available
    if (userInfo.avatar) {
        $('#profileAvatar').attr('src', userInfo.avatar);
    }
}

// Load current user info for dashboard
async function loadCurrentUser() {
    try {
        const token = localStorage.getItem('authToken');
        if (!token) {
            // Redirect to login if no token
            window.location.href = '/Auth/Login';
            return;
        }

        const response = await fetch('/api/Auth/me', {
            headers: {
                'Authorization': `Bearer ${token}`,
                'Content-Type': 'application/json'
            }
        });

        if (response.ok) {
            const userInfo = await response.json();
            updateUserDisplay(userInfo.data);
        } else {
            // Token invalid, redirect to login
            localStorage.removeItem('authToken');
            window.location.href = '/Auth/Login';
        }
    } catch (error) {
        console.error('Error loading user info:', error);
        // Redirect to login on error
        window.location.href = '/Auth/Login';
    }
}

// Update user display in top bar
function updateUserDisplay(userInfo) {
    const displayName = userInfo.fullName || userInfo.username || 'Người dùng';
    document.getElementById('userDisplayName').textContent = displayName;
    
    // Update avatar if available
    if (userInfo.avatar) {
        const avatarIcon = document.getElementById('userAvatarIcon');
        avatarIcon.style.display = 'none';
        avatarIcon.parentElement.innerHTML = `<img src="${userInfo.avatar}" alt="Avatar" style="width: 100%; height: 100%; border-radius: 50%; object-fit: cover;">`;
    }
}

// Handle logout
async function handleLogout() {
    try {
        const token = localStorage.getItem('authToken');
        if (token) {
            await fetch('/api/Auth/logout', {
                method: 'POST',
                headers: {
                    'Authorization': `Bearer ${token}`,
                    'Content-Type': 'application/json'
                }
            });
        }
    } catch (error) {
        console.error('Logout error:', error);
    } finally {
        // Clear token and redirect
        localStorage.removeItem('authToken');
        window.location.href = '/Auth/Login';
    }
}

// Handle profile link
function handleProfile() {
    // TODO: Navigate to profile page
    alert('Chức năng Profile đang được phát triển');
}

function showLoginButton() {
    $('#profileDropdown').hide();
    $('#loginBtn').show();
}

function showAlert(message, type = 'info') {
    // Create and show Bootstrap alert
    const alertHtml = `
        <div class="alert alert-${type} alert-dismissible fade show position-fixed" 
             style="top: 20px; right: 20px; z-index: 9999; min-width: 300px;" role="alert">
            ${message}
            <button type="button" class="btn-close" data-bs-dismiss="alert"></button>
        </div>
    `;
    
    $('body').append(alertHtml);
    
    // Auto remove after 3 seconds
    setTimeout(() => {
        $('.alert').alert('close');
    }, 3000);
}

function getRoleName(roleId) {
    switch(roleId) {
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

// Modal switching functions
function showRegisterModal() {
    $('#loginModal').modal('hide');
    $('#registerModal').modal('show');
}

function showLoginModal() {
    $('#registerModal').modal('hide');
    $('#loginModal').modal('show');
}

// Register form handling
async function handleRegister() {
    const fullName = $('#regFullName').val();
    const email = $('#regEmail').val();
    const phone = $('#regPhone').val();
    const username = $('#regUsername').val();
    const password = $('#regPassword').val();
    const confirmPassword = $('#regConfirmPassword').val();
    const agreeTerms = $('#agreeTerms').is(':checked');
    
    // Validation
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
    
    // Show loading state
    const registerBtn = $('.btn-register');
    const originalText = registerBtn.html();
    registerBtn.html('<i class="fas fa-spinner fa-spin"></i> Đang đăng ký...');
    registerBtn.prop('disabled', true);
    
    try {
        const response = await fetch('/Auth/Register', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
            },
            body: JSON.stringify({
                fullName: fullName,
                email: email,
                phone: phone,
                username: username,
                password: password
            })
        });
        
        const result = await response.json();
        
        if (result.success) {
            showAlert('Đăng ký thành công! Vui lòng đăng nhập.', 'success');
            $('#registerModal').modal('hide');
            $('#loginModal').modal('show');
            // Pre-fill username
            $('#username').val(username);
        } else {
            showAlert(result.error || 'Đăng ký thất bại', 'danger');
        }
    } catch (error) {
        console.error('Register error:', error);
        showAlert('Lỗi kết nối đến server', 'danger');
    } finally {
        // Reset form and button
        $('.login-form')[1].reset();
        registerBtn.html(originalText);
        registerBtn.prop('disabled', false);
    }
}

// Floating Tab Functions
function initializeFloatingTab() {
    console.log('Initializing floating tab...');
    
    // Toggle floating tab
    $('.floating-tab-toggle').on('click', function() {
        $('.floating-tab').toggleClass('collapsed');
        
        // Rotate arrow icon
        const arrow = $(this).find('i');
        if ($('.floating-tab').hasClass('collapsed')) {
            arrow.removeClass('fa-chevron-left').addClass('fa-chevron-right');
        } else {
            arrow.removeClass('fa-chevron-right').addClass('fa-chevron-left');
        }
    });
    
    // Handle floating tab item clicks
    $('.floating-tab-item[data-action="booking"]').on('click', function() {
        $('#bookingModal').modal('show');
    });
    
    $('.floating-tab-item[data-action="messenger"]').on('click', function() {
        // Open Messenger (replace with actual Messenger link)
        window.open('https://m.me/your-page', '_blank');
    });
    
    $('.floating-tab-item[data-action="hotline"]').on('click', function() {
        // Open phone dialer
        window.open('tel:0123456789', '_self');
    });
    
    $('.floating-tab-item[data-action="zalo"]').on('click', function() {
        // Open Zalo (replace with actual Zalo link)
        window.open('https://zalo.me/0123456789', '_blank');
    });
    
    $('.floating-tab-item[data-action="directions"]').on('click', function() {
        // Open Google Maps (replace with actual address)
        window.open('https://maps.google.com/?q=your-address', '_blank');
    });
    
    // Handle booking form submission
    $('#submitBooking').on('click', function() {
        handleBookingSubmission();
    });
    
    console.log('Floating tab initialized successfully');
}

// Handle booking form submission
async function handleBookingSubmission() {
    const form = $('#bookingForm');
    const submitBtn = $('#submitBooking');
    
    // Get form data
    const formData = {
        serviceType: $('#serviceType').val(),
        fullName: $('#fullName').val(),
        email: $('#email').val(),
        phone: $('#phone').val(),
        vehicleType: $('#vehicleType').val(),
        branch: $('#branch').val(),
        licensePlate: $('#licensePlate').val(),
        mileage: $('#mileage').val(),
        appointmentTime: $('#appointmentTime').val(),
        message: $('#message').val()
    };
    
    // Validation
    if (!formData.serviceType || !formData.fullName || !formData.phone || !formData.branch || !formData.appointmentTime) {
        showAlert('Vui lòng điền đầy đủ các trường bắt buộc (*)', 'warning');
        return;
    }
    
    // Show loading state
    const originalText = submitBtn.html();
    submitBtn.html('<i class="fas fa-spinner fa-spin me-2"></i>Đang xử lý...');
    submitBtn.prop('disabled', true);
    
    try {
        // Simulate API call (replace with actual API endpoint)
        await new Promise(resolve => setTimeout(resolve, 2000));
        
        // Success
        showAlert('Đặt lịch thành công! Chúng tôi sẽ liên hệ lại với bạn sớm nhất.', 'success');
        $('#bookingModal').modal('hide');
        form[0].reset();
        
    } catch (error) {
        console.error('Booking error:', error);
        showAlert('Có lỗi xảy ra khi đặt lịch. Vui lòng thử lại sau.', 'danger');
    } finally {
        // Reset button
        submitBtn.html(originalText);
        submitBtn.prop('disabled', false);
    }
}