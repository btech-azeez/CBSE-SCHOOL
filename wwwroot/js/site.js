// CBSE School Management System - Global JavaScript & AJAX Utility Functions

/**
 * 1. Fetch Student Details via AJAX GET (fetch API)
 * @param {number} studentId 
 * @param {function} callback 
 */
function fetchStudentProfileAjax(studentId, callback) {
    if (!studentId) return;

    fetch(`/Student/GetStudentDetailsJson?id=${studentId}`)
        .then(response => {
            if (!response.ok) throw new Error('Network response error');
            return response.json();
        })
        .then(data => {
            if (data.success && typeof callback === 'function') {
                callback(data);
            }
        })
        .catch(error => console.error('Error fetching student profile:', error));
}

/**
 * 2. Calculate CBSE Grade in Real-Time via AJAX GET
 * @param {number} marks 
 * @param {number} maxMarks 
 * @param {function} callback 
 */
function calculateGradeAjax(marks, maxMarks, callback) {
    if (marks === undefined || marks === null || marks < 0) return;

    fetch(`/ExamMarks/CalculateGradeJson?marksObtained=${marks}&maxMarks=${maxMarks || 100}`)
        .then(response => response.json())
        .then(data => {
            if (data.success && typeof callback === 'function') {
                callback(data);
            }
        })
        .catch(error => console.error('Error calculating grade:', error));
}

/**
 * 3. Dynamic Subject Dropdown Population by Class ID
 * @param {number} classId 
 * @param {string} selectElementId 
 */
function loadSubjectsByClassAjax(classId, selectElementId) {
    const selectElem = document.getElementById(selectElementId);
    if (!selectElem || !classId) return;

    fetch(`/Student/GetSubjectsByClassJson?classId=${classId}`)
        .then(response => response.json())
        .then(subjects => {
            selectElem.innerHTML = '<option value="">-- Select Subject --</option>';
            subjects.forEach(sub => {
                const opt = document.createElement('option');
                opt.value = sub.id;
                opt.textContent = `[${sub.subjectCode}] ${sub.subjectName} (Pass: ${sub.passMarks})`;
                selectElem.appendChild(opt);
            });
        })
        .catch(error => console.error('Error loading subjects:', error));
}
