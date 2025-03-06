// Starry Night Background Animation
document.addEventListener('DOMContentLoaded', function() {
    // Only run the animation if we have the dark-bg-wrapper class
    const darkBgWrapper = document.querySelector('.dark-bg-wrapper');
    if (!darkBgWrapper) return;
    
    // Create stars
    createStars(darkBgWrapper);
    
    // Add stars to the wavy background section
    const wavyBg = document.querySelector('.wavy-background');
    if (wavyBg) {
        createStars(wavyBg, 50, 0.8); // More dense stars in the hero section
    }
});

function createStars(container, count = 200, opacity = 0.5) {
    // Set container to position relative if it's not already
    const computedStyle = window.getComputedStyle(container);
    if (computedStyle.position === 'static') {
        container.style.position = 'relative';
    }
    
    const containerWidth = container.offsetWidth;
    const containerHeight = container === document.querySelector('.dark-bg-wrapper') 
        ? Math.max(document.documentElement.scrollHeight, document.body.scrollHeight) 
        : container.offsetHeight;
    
    for (let i = 0; i < count; i++) {
        const star = document.createElement('div');
        star.classList.add('star');
        
        // Random position
        const left = Math.random() * containerWidth;
        const top = Math.random() * containerHeight;
        
        // Random size (1-3px)
        const size = Math.random() * 2 + 1;
        
        // Random opacity variation
        const starOpacity = Math.random() * 0.5 + opacity;
        
        // Apply styles
        star.style.left = `${left}px`;
        star.style.top = `${top}px`;
        star.style.width = `${size}px`;
        star.style.height = `${size}px`;
        star.style.opacity = starOpacity;
        
        // Add twinkle animation with random delay
        if (Math.random() > 0.3) { // 70% of stars will twinkle
            star.classList.add('twinkle');
            star.style.animationDelay = `${Math.random() * 5}s`;
            star.style.animationDuration = `${Math.random() * 3 + 2}s`; // 2-5s duration
        }
        
        container.appendChild(star);
    }
} 