// courses.js
$(function () {
    // Select all courses
    $('#selectAllCourses').on('change', function () {
        $('.course-select').prop('checked', this.checked);
    });

    // Add Course
    $('#addCourseBtn').on('click', function () {
        $('#courseEditModalLabel').text('Add Course');
        $('#courseEditForm')[0].reset();
        $('#courseEditModal').modal('show');
    });

    // AJAX add/edit course
    $('#courseEditForm').on('submit', function (e) {
        e.preventDefault();
        var $form = $(this);
        var data = $form.serialize();
        var isEdit = $('#Course_CourseId').val() && $('#Course_CourseId').val() !== '0';
        var handler = isEdit ? 'EditCourseAjax' : 'AddCourseAjax';
        $.post('/Courses/Index?handler=' + handler, data, function (course) {
            if (isEdit) {
                // Update the row in the table
                var $row = $('#coursesTable tbody tr[data-course-id="' + course.courseId + '"]');
                $row.find('td').eq(1).text(course.title);
                $row.find('td').eq(2).text(course.courseDescription);
            } else {
                // Add new row to table
                $('#coursesTable tbody').append(
                    '<tr data-course-id="' + course.courseId + '">' +
                        '<td><input type="checkbox" class="course-select" value="' + course.courseId + '" /></td>' +
                        '<td>' + course.title + '</td>' +
                        '<td>' + course.courseDescription + '</td>' +
                    '</tr>'
                );
            }
            $('#courseEditModal').modal('hide');
            showCourseToast(isEdit ? 'Course updated successfully!' : 'Course added successfully!');
            setupCoursesPagination();
        }).fail(function () {
            showCourseToast('Failed to save course. Please check your input.');
        });
    });

    // Edit Course
    $('#editCourseBtn').on('click', function () {
        var selected = $('.course-select:checked');
        if (selected.length !== 1) {
            showCourseToast('Please select exactly one course to edit.');
            return;
        }
        var row = selected.closest('tr');
        var title = row.find('td').eq(1).text().trim();
        var desc = row.find('td').eq(2).text().trim();
        // Set values in modal
        $('#courseEditModalLabel').text('Edit Course');
        $('#courseEditForm')[0].reset();
        $("#Course_CourseId").val(selected.val());
        $("#Course_Title").val(title);
        $("#Course_CourseDescription").val(desc);
        $('#courseEditModal').modal('show');
    });

    // Delete Course(s)
    $('#deleteCourseBtn').on('click', function () {
        var selected = $('.course-select:checked');
        if (selected.length === 0) {
            showCourseToast('Please select at least one course to delete.');
            return;
        }
        // Collect selected IDs and titles
        var ids = selected.map(function () { return this.value; }).get().join(',');
        var titles = selected.closest('tr').map(function () {
            return $(this).find('td').eq(1).text();
        }).get().join(', ');
        $('#deleteCourseIds').val(ids);
        $('#courseDeleteModal .modal-body span').text(titles);
        $('#courseDeleteModal').modal('show');
    });

    // Delete Courses (AJAX)
    $('#courseDeleteForm').off('submit').on('submit', function (e) {
        e.preventDefault();
        var ids = $('#deleteCourseIds').val().split(',');
        var token = $('input[name="__RequestVerificationToken"]').val();
        var requests = ids.map(function(id) {
            return $.post('/Courses/Index?handler=DeleteCourseAjax', { courseId: id, __RequestVerificationToken: token });
        });
        Promise.all(requests).then(function(results) {
            results.forEach(function(res) {
                var courseId = res.courseId || (res[0] && res[0].courseId);
                $('#coursesTable tbody tr[data-course-id="' + courseId + '"]').remove();
            });
            $('#courseDeleteModal').modal('hide');
            showCourseToast('Course(s) deleted successfully!');
            setupCoursesPagination();
        }).catch(function() {
            showCourseToast('Failed to delete course(s).');
        });
    });

    // Toast helper
    window.showCourseToast = function (message) {
        $('#courseToast .toast-body').text(message);
        var toast = new bootstrap.Toast(document.getElementById('courseToast'));
        toast.show();
    };

    // --- Pagination logic ---
    const coursesPerPage = 10;
    let currentPage = 1;
    let coursesRows = [];

    function renderCoursesPage(page) {
        const start = (page - 1) * coursesPerPage;
        const end = start + coursesPerPage;
        coursesRows.forEach((row, idx) => {
            row.style.display = (idx >= start && idx < end) ? '' : 'none';
        });
    }

    function renderCoursesPagination() {
        const totalPages = Math.ceil(coursesRows.length / coursesPerPage);
        const pagination = document.getElementById('coursesPagination');
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
                renderCoursesPage(currentPage);
                renderCoursesPagination();
            };
            li.appendChild(a);
            pagination.appendChild(li);
        }
    }

    function setupCoursesPagination() {
        coursesRows = Array.from(document.querySelectorAll('#coursesTable tbody tr'));
        currentPage = 1;
        renderCoursesPage(currentPage);
        renderCoursesPagination();
    }

    // --- Title filter logic ---
    const titleFilterInput = document.querySelector('input[name="TitleFilter"]');
    if (titleFilterInput) {
        titleFilterInput.addEventListener('input', async function () {
            const filterValue = this.value.toLowerCase();
            const response = await fetch(`/Courses/Index?handler=FilterCoursesByName&name=${encodeURIComponent(this.value)}`);
            const filteredCourses = await response.json();
            const tableBody = document.querySelector('#coursesTable tbody');
            tableBody.innerHTML = '';
            filteredCourses.forEach(course => {
                // Compare using lowercase for case-insensitive search
                if (course.title.toLowerCase().includes(filterValue)) {
                    const row = document.createElement('tr');
                    row.setAttribute('data-course-id', course.courseId);
                    row.innerHTML = `
                        <td><input type="checkbox" class="course-select" value="${course.courseId}" /></td>
                        <td>${course.title}</td>
                        <td>${course.courseDescription || course.description || ''}</td>
                    `;
                    tableBody.appendChild(row);
                }
            });
            setupCoursesPagination();
        });
    }

    // Re-setup pagination after filtering or table update if needed
    window.setupCoursesPagination = setupCoursesPagination;

    $(document).ready(function () {
        setupCoursesPagination();
    });
});
