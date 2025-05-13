// students.js
$(function () {
    // Toggle advanced filters
    $('#toggleAdvancedFilters').on('click', function () {
        $('#advancedFilters').toggleClass('d-none');
    });

    // Select all students
    $('#selectAllStudents').on('change', function () {
        $('.student-select').prop('checked', this.checked);
    });

    // Add Student
    $('#addStudentBtn').on('click', function () {
        $('#studentEditModalLabel').text('Add Student');
        $('#studentEditForm')[0].reset();
        $('#studentEditModal').modal('show');
    });

    // AJAX edit student
    $('#studentEditForm').on('submit', function (e) {
        e.preventDefault();
        var $form = $(this);
        var data = $form.serialize();
        var isEdit = $('#Student_StudentId').val() && $('#Student_StudentId').val() !== '0';
        var handler = isEdit ? 'EditStudentAjax' : 'AddStudentAjax';
        $.post('/Students/Index?handler=' + handler, data, function (student) {
            if (isEdit) {
                // Update the row in the table
                var $row = $('#studentsTable tbody tr[data-student-id="' + student.studentId + '"]');
                $row.find('td').eq(1).text(student.name);
                $row.find('td').eq(2).text(student.email);
                $row.find('td').eq(3).text(new Date(student.dateOfBirth).toLocaleDateString());
            } else {
                // Add new row to table
                var courses = '';
                $('#studentsTable tbody').append(
                    '<tr data-student-id="' + student.studentId + '">' +
                        '<td><input type="checkbox" class="student-select" value="' + student.studentId + '" /></td>' +
                        '<td>' + student.name + '</td>' +
                        '<td>' + student.email + '</td>' +
                        '<td>' + (student.dateOfBirth ? new Date(student.dateOfBirth).toLocaleDateString() : '') + '</td>' +
                        '<td>' + courses + '</td>' +
                        '<td class="text-center"><button class="btn btn-sm btn-success rounded-3 add-course-btn" data-student-id="' + student.studentId + '">Add Course</button></td>' +
                    '</tr>'
                );
            }
            $('#studentEditModal').modal('hide');
            showStudentToast(isEdit ? 'Student updated successfully!' : 'Student added successfully!');
            setupStudentsPagination(); // Ensure pagination is updated after adding/editing a student
        }).fail(function (xhr) {
            showStudentToast('Failed to save student. Please check your input.');
        });
    });

    // Edit Student
    $('#editStudentBtn').on('click', function () {
        var selected = $('.student-select:checked');
        if (selected.length !== 1) {
            showStudentToast('Please select exactly one student to edit.');
            return;
        }
        var row = selected.closest('tr');
        var name = row.find('td').eq(1).text().trim();
        var email = row.find('td').eq(2).text().trim();
        var dob = row.find('td').eq(3).text().trim();
        // Set values in modal
        $('#studentEditModalLabel').text('Edit Student');
        $('#studentEditForm')[0].reset();
        $("#Student_StudentId").val(selected.val());
        $("#Student_Name").val(name);
        $("#Student_Email").val(email);
        // Convert date to yyyy-MM-dd
        var parts = dob.split('/');
        var formattedDate = '';
        if (parts.length === 3) {
            // Try both dd/MM/yyyy and MM/dd/yyyy
            if (parseInt(parts[0], 10) > 12) {
                formattedDate = `${parts[2]}-${parts[1].padStart(2, '0')}-${parts[0].padStart(2, '0')}`;
            } else {
                formattedDate = `${parts[2]}-${parts[0].padStart(2, '0')}-${parts[1].padStart(2, '0')}`;
            }
        }
        $("#Student_DateOfBirth").val(formattedDate);
        $('#studentEditModal').modal('show');
    });

    // Delete Student(s)
    $('#deleteStudentBtn').on('click', function () {
        var selected = $('.student-select:checked');
        if (selected.length === 0) {
            showStudentToast('Please select at least one student to delete.');
            return;
        }
        // Collect selected IDs and names
        var ids = selected.map(function () { return this.value; }).get().join(',');
        var names = selected.closest('tr').map(function () {
            return $(this).find('td').eq(1).text();
        }).get().join(', ');
        $('#deleteStudentIds').val(ids);
        $('#studentDeleteModal .modal-body span').text(names);
        $('#studentDeleteModal').modal('show');
    });

    // Delete Students
    $('#studentDeleteForm').off('submit').on('submit', function (e) {
        e.preventDefault();
        var studentIds = $('#deleteStudentIds').val();
        var token = $('input[name="__RequestVerificationToken"]').val();
        $.post('/Students/Index?handler=DeleteStudents', { StudentIds: studentIds, __RequestVerificationToken: token }, function () {
            location.reload();
        });
    });

    // Toast helper
    window.showStudentToast = function (message) {
        $('#studentToast .toast-body').text(message);
        var toast = new bootstrap.Toast(document.getElementById('studentToast'));
        toast.show();
    };

    // Real-time search by name (client-side filtering, no pagination reset)
    $('input[name="NameFilter"]').on('input', function () {
        var filter = $(this).val().toLowerCase();
        $('#studentsTable tbody tr').each(function () {
            var name = $(this).find('td').eq(1).text().toLowerCase();
            if (!filter || name.includes(filter)) {
                $(this).show();
            } else {
                $(this).hide();
            }
        });
        // Do NOT call setupStudentsPagination here, so filtered rows remain visible
    });

    // Store initial table HTML for reset
    let initialStudentsTableHtml = '';
    if (!window._studentsTableInitialHtml) {
        window._studentsTableInitialHtml = $('#studentsTable tbody').html();
    }
    $(document).ready(function () {
        initialStudentsTableHtml = $('#studentsTable tbody').html();
        setupPagination();
    });

    // --- Pagination logic ---
    const studentsPerPage = 10;
    let currentPage = 1;
    let studentsRows = [];

    function renderStudentsPage(page) {
        const start = (page - 1) * studentsPerPage;
        const end = start + studentsPerPage;
        studentsRows.forEach((row, idx) => {
            row.style.display = (idx >= start && idx < end) ? '' : 'none';
        });
    }

    function renderPagination() {
        const totalPages = Math.ceil(studentsRows.length / studentsPerPage);
        const pagination = document.getElementById('studentsPagination');
        pagination.innerHTML = '';
        for (let i = 1; i <= totalPages; i++) {
            const li = document.createElement('li');
            li.className = 'page-item' + (i === currentPage ? ' active' : '');
            const a = document.createElement('a');
            a.className = 'page-link';
            a.href = '#';
            a.textContent = i;
            a.onclick = function (e) {
                e.preventDefault();
                currentPage = i;
                renderStudentsPage(currentPage);
                renderPagination();
            };
            li.appendChild(a);
            pagination.appendChild(li);
        }
    }

    function setupPagination() {
        studentsRows = Array.from(document.querySelectorAll('#studentsTable tbody tr'));
        currentPage = 1;
        renderStudentsPage(currentPage);
        renderPagination();
    }

    // Re-setup pagination after filtering or table update if needed
    window.setupStudentsPagination = setupPagination;

    $(document).ready(function () {
        setupPagination();
    });

    // Register course logic
    function registerCourse(studentId) {
        // Fetch available courses for this student
        fetch(`/Students/Index?handler=AvailableCourses&id=${studentId}`)
            .then(response => response.json())
            .then(courses => {
                const select = document.getElementById('availableCourses');
                select.innerHTML = '';
                if (courses.length > 0) {
                    courses.forEach(course => {
                        const option = document.createElement('option');
                        option.value = course.courseId;
                        option.textContent = course.courseName;
                        select.appendChild(option);
                    });
                } else {
                    const option = document.createElement('option');
                    option.disabled = true;
                    option.textContent = 'No available courses';
                    select.appendChild(option);
                }
                document.getElementById('registerStudentId').value = studentId;
                var modal = new bootstrap.Modal(document.getElementById('studentRegisterCourseModal'));
                modal.show();
            });
    }
    window.registerCourse = registerCourse;

    $(document).on('click', '#registerCourseSubmitBtn', function () {
        console.log('Register button clicked');
        const studentId = document.getElementById('registerStudentId').value;
        const courseId = document.getElementById('availableCourses').value;
        if (!courseId) return;
        // Get anti-forgery token if present
        const token = document.querySelector('#registerCourseForm input[name="__RequestVerificationToken"]')?.value;
        let body = new URLSearchParams({ studentId: studentId, courseId: courseId });
        if (token) body.append('__RequestVerificationToken', token);
        fetch(`/Students/Index?handler=RegisterCourse`, {
            method: 'POST',
            headers: { 'Content-Type': 'application/x-www-form-urlencoded' },
            body: body
        })
        .then(async r => {
            if (!r.ok) {
                showStudentToast('Server error: ' + r.status);
                return {};
            }
            const text = await r.text();
            try {
                return text ? JSON.parse(text) : {};
            } catch (e) {
                showStudentToast('Invalid server response');
                return {};
            }
        })
        .then(result => {
            showStudentToast(result.success ? 'Course registered successfully!' : (result.message || 'Failed to register course.'));
            var modal = bootstrap.Modal.getInstance(document.getElementById('studentRegisterCourseModal'));
            modal.hide();
            if (result.success) {
                // Refresh the student row and info modal
                updateStudentRowAndInfo(studentId);
                // Restore table background to light
                $("body").removeClass("modal-open");
                $(".modal-backdrop").remove();
                $("#studentsTable").removeClass("table-dark").addClass("table-light");
            }
        });
    });

    // Helper to update the student row and info modal after course changes
    function updateStudentRowAndInfo(studentId) {
        fetch(`/Students/Index?handler=StudentInfo&id=${studentId}`)
            .then(response => response.json())
            .then(data => {
                // Update table row
                var $row = $('#studentsTable tbody tr[data-student-id="' + studentId + '"]');
                var coursesHtml = '';
                if (Array.isArray(data.courses) && data.courses.length > 0) {
                    data.courses.forEach(function(course) {
                        coursesHtml += '<span class="badge bg-info text-dark rounded-pill me-1">' + course.courseName + '</span>';
                    });
                } else {
                    coursesHtml = '<span class="text-muted">None</span>';
                }
                $row.find('td').eq(4).html(coursesHtml);

                // If info modal is open and showing this student, update its content
                var infoModal = document.getElementById('studentInfoModal');
                if (infoModal.classList.contains('show')) {
                    showStudentInfo(studentId);
                }
            });
    }
});
