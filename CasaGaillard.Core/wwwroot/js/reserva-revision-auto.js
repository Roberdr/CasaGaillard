document.querySelectorAll('form[data-auto-submit-reserva] input').forEach(input => {
    input.addEventListener('change', () => input.form?.requestSubmit());
});
