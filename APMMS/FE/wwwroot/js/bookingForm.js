// bookingForm.js - logic cho modal đặt lịch công khai / đăng nhập

(function () {
    const REQUIRED_DATA_KEY = 'original-required';

    function getUserIdFromToken(token) {
        if (!token) return null;
        try {
            const payload = JSON.parse(atob(token.split('.')[1]));
            return payload['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier'] ||
                   payload['UserId'] ||
                   payload['nameid'];
        } catch (error) {
            console.error('Error decoding token:', error);
            return null;
        }
    }

    function ensureLicensePlateInput() {
        let field = $('#licensePlate');
        if (!field.length) {
            field = $('<input type="text" class="form-control" id="licensePlate" placeholder="VD: 30A-12345" autocomplete="off">');
            $('#bookingForm .modal-body .row .col-md-6').last().prepend(field);
        }
        if (field.is('select')) {
            const newInput = $('<input type="text" class="form-control" id="licensePlate" placeholder="VD: 30A-12345" autocomplete="off">');
            field.replaceWith(newInput);
            field = newInput;
        }
        return field;
    }

    function ensureLicensePlateSelect(cars = []) {
        let field = $('#licensePlate');
        if (field.is('input')) {
            const newSelect = $('<select class="form-select" id="licensePlate" required></select>');
            field.replaceWith(newSelect);
            field = newSelect;
        }
        field.empty();
        if (cars.length > 0) {
            field.append('<option value="">-- Chọn biển số xe --</option>');
            cars.forEach(car => {
                const plate = car.licensePlate || 'Chưa có biển số';
                const name = car.carName ? ` - ${car.carName}` : '';
                field.append(`<option value="${car.id}" data-car-id="${car.id}">${plate}${name}</option>`);
            });
            field.prop('disabled', false);
        } else {
            field.append('<option value="">-- Bạn chưa có xe nào đã bảo dưỡng --</option>');
            field.prop('disabled', true);
        }
        field.prop('required', true);
        return field;
    }

    function toggleGuestFields(isLoggedIn) {
        $('.guest-field').each(function () {
            const $section = $(this);
            const $inputs = $section.find('input,select,textarea');

            if (isLoggedIn) {
                $inputs.each(function () {
                    const $input = $(this);
                    if ($input.data(REQUIRED_DATA_KEY) === undefined) {
                        $input.data(REQUIRED_DATA_KEY, $input.prop('required'));
                    }
                    $input.prop('required', false);
                });
                $section.hide();
            } else {
                $inputs.each(function () {
                    const $input = $(this);
                    const originalRequired = $input.data(REQUIRED_DATA_KEY);
                    if (originalRequired !== undefined) {
                        $input.prop('required', originalRequired);
                    }
                });
                $section.show();
            }
        });
    }

    // ----------------------------------------------------------
    // API helpers
    // ----------------------------------------------------------

    window.loadBranches = async function loadBranches() {
        const token = localStorage.getItem('authToken');
        if (token) {
            // Logged-in user không cần chọn chi nhánh thủ công
            return;
        }

        try {
            const apiBaseUrl = window.API_BASE_URL || 'https://localhost:7173/api';
            const select = $('#branch');
            if (!select.length) {
                console.error('❌ Branch select element not found!');
                return;
            }

            select.html('<option value="">-- Đang tải danh sách chi nhánh --</option>');

            const response = await fetch(`${apiBaseUrl}/Branch`, {
                method: 'GET',
                headers: {
                    'Content-Type': 'application/json',
                    'Accept': 'application/json'
                },
                mode: 'cors'
            });

            if (!response.ok) {
                const errorText = await response.text();
                throw new Error(`HTTP error! status: ${response.status}, message: ${errorText}`);
            }

            const result = await response.json();
            const branches = result.success && Array.isArray(result.data) ? result.data : (Array.isArray(result) ? result : []);

            select.empty();
            select.append('<option value="">-- Chọn chi nhánh --</option>');

            if (!branches.length) {
                select.append('<option value="">-- Không có chi nhánh nào --</option>');
                return;
            }

            branches.forEach(branch => {
                select.append(`<option value="${branch.id}">${branch.name || 'N/A'}</option>`);
            });
        } catch (error) {
            console.error('❌ Error loading branches:', error);
            const select = $('#branch');
            if (select.length) {
                select.empty();
                select.append('<option value="">-- Lỗi tải danh sách chi nhánh --</option>');
            }
        }
    };

    async function loadUserCars() {
        const token = localStorage.getItem('authToken');
        if (!token) {
            return;
        }

        let userId = null;
        try {
            const userInfoStr = localStorage.getItem('userInfo');
            if (userInfoStr) {
                const userInfo = JSON.parse(userInfoStr);
                userId = userInfo.userId || userInfo.id;
            }
            if (!userId) {
                userId = getUserIdFromToken(token);
            }
        } catch (error) {
            console.error('Error getting userId:', error);
            return;
        }

        if (!userId) {
            return;
        }

        try {
            const apiBaseUrl = window.API_BASE_URL || 'https://localhost:7173/api';
            const response = await fetch(`${apiBaseUrl}/CarOfAutoOwner/user/${userId}/serviced`, {
                headers: {
                    'Authorization': `Bearer ${token}`,
                    'Content-Type': 'application/json'
                }
            });

            if (!response.ok) {
                console.warn('Failed to load user cars:', response.status);
                return;
            }

            const result = await response.json();
            const cars = result.success && Array.isArray(result.data) ? result.data : (Array.isArray(result) ? result : []);

            ensureLicensePlateSelect(cars);
        } catch (error) {
            console.error('Error loading serviced cars:', error);
        }
    }

    function getDefaultBranchId() {
        try {
            const userInfoStr = localStorage.getItem('userInfo');
            if (userInfoStr) {
                const userInfo = JSON.parse(userInfoStr);
                if (userInfo.branchId) {
                    return parseInt(userInfo.branchId, 10);
                }
            }
        } catch (error) {
            console.warn('Could not get branch from user info:', error);
        }
        return 1; // fallback chi nhánh mặc định
    }

    // ----------------------------------------------------------
    // Form state helpers
    // ----------------------------------------------------------

    function updateBookingFormForUser() {
        const token = localStorage.getItem('authToken');
        const isLoggedIn = !!token;

        toggleGuestFields(isLoggedIn);

        if (isLoggedIn) {
            ensureLicensePlateSelect();
            $('#mileage').val('');
            $('#message').val('');
        } else {
            const plateInput = ensureLicensePlateInput();
            plateInput.val('');
            $('#mileage').val('');
            $('#message').val('');
            $('#serviceType').val('');
            $('#appointmentTime').val('');
            $('#branch').val('');
        }
    }

    async function handleBookingSubmission() {
        const form = $('#bookingForm');
        const submitBtn = $('#submitBooking');

        if (!form[0].checkValidity()) {
            form[0].reportValidity();
            return;
        }

        const selectedDate = $('#appointmentTime').val();
        const appointmentTime = selectedDate ? new Date(selectedDate) : null;
        if (!appointmentTime || appointmentTime <= new Date()) {
            showAlert('Vui lòng chọn thời gian trong tương lai', 'warning');
            return;
        }

        const token = localStorage.getItem('authToken');
        const isLoggedIn = !!token;
        const apiBaseUrl = window.API_BASE_URL || 'https://localhost:7173/api';

        let bookingData;
        let endpoint;
        const headers = { 'Content-Type': 'application/json' };

        if (isLoggedIn) {
            let userId = null;
            try {
                const userInfoStr = localStorage.getItem('userInfo');
                if (userInfoStr) {
                    const userInfo = JSON.parse(userInfoStr);
                    userId = userInfo.userId || userInfo.id;
                }
                if (!userId) {
                    userId = getUserIdFromToken(token);
                }
            } catch (error) {
                console.error('Error getting userId:', error);
                showAlert('Không thể xác định người dùng. Vui lòng đăng nhập lại.', 'danger');
                return;
            }

            const licenseField = $('#licensePlate');
            let carId = null;
            let usePublicFlow = false;

            if (licenseField.is('select')) {
                if (licenseField.prop('disabled')) {
                    showAlert('Bạn chưa có xe nào đã bảo dưỡng. Vui lòng liên hệ tư vấn để thêm xe trước khi đặt lịch.', 'warning');
                    return;
                }
                carId = licenseField.val();
                if (!carId) {
                    showAlert('Vui lòng chọn biển số xe', 'warning');
                    return;
                }
            } else {
                usePublicFlow = true;
            }

            if (!usePublicFlow) {
                const branchId = getDefaultBranchId();
                bookingData = {
                    userId: parseInt(userId, 10),
                    carId: parseInt(carId, 10),
                    scheduledDate: appointmentTime.toISOString(),
                    branchId: branchId,
                    statusCode: 'PENDING'
                };
                endpoint = `${apiBaseUrl}/ServiceSchedule`;
                headers['Authorization'] = `Bearer ${token}`;
            } else {
                // Không có xe đã bảo dưỡng -> fallback sang public booking flow
                bookingData = buildPublicBookingPayload(appointmentTime);
                endpoint = `${apiBaseUrl}/ServiceSchedule/public-booking`;
            }
        } else {
            bookingData = buildPublicBookingPayload(appointmentTime);
            endpoint = `${apiBaseUrl}/ServiceSchedule/public-booking`;
        }

        const originalText = submitBtn.html();
        submitBtn.html('<i class="fas fa-spinner fa-spin me-2"></i>Đang xử lý...');
        submitBtn.prop('disabled', true);

        try {
            const response = await fetch(endpoint, {
                method: 'POST',
                headers,
                body: JSON.stringify(bookingData)
            });

            const result = await response.json();
            if (result.success) {
                showAlert(result.message || 'Đặt lịch thành công! Chúng tôi sẽ liên hệ lại với bạn sớm nhất.', 'success');
                $('#bookingModal').modal('hide');
                form[0].reset();
                updateBookingFormForUser();
            } else {
                showAlert(result.message || 'Có lỗi xảy ra khi đặt lịch. Vui lòng thử lại sau.', 'danger');
            }
        } catch (error) {
            console.error('Booking error:', error);
            showAlert('Có lỗi xảy ra khi đặt lịch. Vui lòng thử lại sau.', 'danger');
        } finally {
            submitBtn.html(originalText);
            submitBtn.prop('disabled', false);
        }
    }

    function buildPublicBookingPayload(appointmentTime) {
        return {
            fullName: $('#fullName').val(),
            email: $('#email').val() || null,
            phone: $('#phone').val(),
            carName: $('#vehicleType').val() || 'Chưa xác định',
            licensePlate: $('#licensePlate').val() || null,
            carModel: $('#vehicleType').val() || null,
            mileage: $('#mileage').val() ? parseInt($('#mileage').val(), 10) : null,
            scheduledDate: appointmentTime.toISOString(),
            branchId: parseInt($('#branch').val() || getDefaultBranchId(), 10),
            message: $('#message').val() || null,
            serviceType: $('#serviceType').val() || null
        };
    }

    // ----------------------------------------------------------
    // Event wiring
    // ----------------------------------------------------------

    $(document).ready(function () {
        $('#bookingModal').on('show.bs.modal', function () {
            updateBookingFormForUser();
            loadBranches();
            loadUserCars();

            const tomorrow = new Date();
            tomorrow.setDate(tomorrow.getDate() + 1);
            $('#appointmentTime').attr('min', tomorrow.toISOString().slice(0, 16));
        });

        $('#bookingModal').on('shown.bs.modal', function () {
            updateBookingFormForUser();
            if (!localStorage.getItem('authToken') && $('#branch option').length <= 1) {
                loadBranches();
            }
        });

        $('#bookingModal').on('hidden.bs.modal', function () {
            updateBookingFormForUser();
            $('#bookingForm')[0].reset();
        });

        $('#submitBooking').on('click', function (e) {
            e.preventDefault();
            handleBookingSubmission();
        });

        $('#bookingForm').on('submit', function (e) {
            e.preventDefault();
            handleBookingSubmission();
        });

        $(document).on('click', '[data-bs-target="#bookingModal"], .floating-tab-item[data-action="booking"]', function () {
            updateBookingFormForUser();
            loadBranches();
            loadUserCars();
        });
    });

    // Expose helpers for debugging if needed
    window.updateBookingFormForUser = updateBookingFormForUser;
    window.loadUserCars = loadUserCars;

})();



