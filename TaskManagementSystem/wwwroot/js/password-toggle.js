// Simple password visibility toggle
document.addEventListener('DOMContentLoaded', function() {
    // Create and insert toggle buttons for all password fields
    document.querySelectorAll('.password-toggle-field').forEach(function(passwordField) {
        // Create the eye icon button
        const toggleBtn = document.createElement('button');
        toggleBtn.type = 'button';
        toggleBtn.className = 'btn password-toggle-btn';
        toggleBtn.innerHTML = '<i class="bi bi-eye"></i>';
        toggleBtn.style.position = 'absolute';
        toggleBtn.style.right = '10px';
        toggleBtn.style.top = '50%';
        toggleBtn.style.transform = 'translateY(-50%)';
        toggleBtn.style.zIndex = '10';
        toggleBtn.style.background = 'transparent';
        toggleBtn.style.border = 'none';
        toggleBtn.style.color = '#63b3ed';
        
        // Find the parent container
        const container = passwordField.parentElement;
        
        // Make sure container is relatively positioned
        container.style.position = 'relative';
        
        // Add the button
        container.appendChild(toggleBtn);
        
        // Add toggle functionality
        toggleBtn.addEventListener('click', function() {
            if (passwordField.type === 'password') {
                passwordField.type = 'text';
                this.innerHTML = '<i class="bi bi-eye-slash"></i>';
            } else {
                passwordField.type = 'password';
                this.innerHTML = '<i class="bi bi-eye"></i>';
            }
        });
    });
}); 