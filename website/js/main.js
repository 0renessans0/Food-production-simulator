document.addEventListener('DOMContentLoaded', function() {
    const btn = document.getElementById('startSimulator');
    if (btn) {
        btn.addEventListener('click', function() {
            window.location.href = 'excursions.html';
        });
    }
});