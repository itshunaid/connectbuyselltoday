// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

// Toggle Favorite - Add/Remove ad from user's favorites
async function toggleFavorite(adId) {
    try {
        const response = await fetch('/Ads/ToggleFavorite', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/x-www-form-urlencoded',
                'X-Requested-With': 'XMLHttpRequest'
            },
            body: `adId=${encodeURIComponent(adId)}`
        });

        const data = await response.json();
        
        if (data.success) {
            // Update the heart icon based on favorite status
            updateHeartIcon(adId, data.isFavorite);
            
            // Show toast notification
            showToast(
                data.isFavorite ? 'Added to favorites!' : 'Removed from favorites',
                data.isFavorite ? 'success' : 'info'
            );
        } else {
            showToast(data.message || 'Error updating favorite', 'error');
            if (data.message && data.message.includes('login')) {
                window.location.href = '/Account/Login';
            }
        }
    } catch (error) {
        console.error('Error toggling favorite:', error);
        showToast('An error occurred. Please try again.', 'error');
    }
}

// Update the heart icon based on favorite status
function updateHeartIcon(adId, isFavorite) {
    const heartButton = document.querySelector(`button[onclick*="'${adId}'"]`);
    if (heartButton) {
        const heartIcon = heartButton.querySelector('i');
        if (heartIcon) {
            if (isFavorite) {
                heartIcon.classList.remove('bi-heart');
                heartIcon.classList.add('bi-heart-fill');
            } else {
                heartIcon.classList.remove('bi-heart-fill');
                heartIcon.classList.add('bi-heart');
            }
        }
    }
}

// Show toast notification
function showToast(message, type = 'info') {
    // Remove existing toast if any
    const existingToast = document.querySelector('.favorite-toast');
    if (existingToast) {
        existingToast.remove();
    }

    // Create toast element
    const toast = document.createElement('div');
    toast.className = `favorite-toast toast-notification position-fixed top-0 end-0 m-3`;
    toast.style.cssText = 'z-index: 9999; animation: slideIn 0.3s ease;';
    
    const bgColor = type === 'success' ? '#28a745' : type === 'error' ? '#dc3545' : '#17a2b8';
    
    toast.innerHTML = `
        <div class="toast show" role="alert" aria-live="assertive" aria-atomic="true">
            <div class="toast-header" style="background-color: ${bgColor}; color: white;">
                <strong class="me-auto">${type === 'success' ? 'Success' : type === 'error' ? 'Error' : 'Info'}</strong>
                <button type="button" class="btn-close btn-close-white" data-bs-dismiss="toast" aria-label="Close"></button>
            </div>
            <div class="toast-body">
                ${message}
            </div>
        </div>
    `;
    
    document.body.appendChild(toast);
    
    // Auto remove after 3 seconds
    setTimeout(() => {
        toast.remove();
    }, 3000);
}

// Initialize favorite icons on page load
document.addEventListener('DOMContentLoaded', function() {
    // Check favorite status for each ad card
    const adButtons = document.querySelectorAll('button[onclick*="toggleFavorite"]');
    adButtons.forEach(button => {
        const match = button.getAttribute('onclick').match(/'([a-f0-9-]{36})'/);
        if (match) {
            const adId = match[1];
            checkFavoriteStatus(adId);
        }
    });
});

// Check favorite status and update icon
async function checkFavoriteStatus(adId) {
    try {
        const response = await fetch(`/Ads/IsFavorite?adId=${adId}`);
        const data = await response.json();
        
        if (data.isFavorite) {
            updateHeartIcon(adId, true);
        }
    } catch (error) {
        console.error('Error checking favorite status:', error);
    }
}

// Get user's location using the browser's Geolocation API
function getUserLocation() {
    if (!navigator.geolocation) {
        showToast('Geolocation is not supported by your browser', 'error');
        return;
    }

    showToast('Getting your location...', 'info');

    navigator.geolocation.getCurrentPosition(
        function(position) {
            const lat = position.coords.latitude;
            const lon = position.coords.longitude;
            
            // Set the hidden input values
            document.getElementById('userLat').value = lat;
            document.getElementById('userLong').value = lon;
            
            showToast('Location found! Now search with your location.', 'success');
        },
        function(error) {
            let errorMessage = 'Unable to get your location';
            switch(error.code) {
                case error.PERMISSION_DENIED:
                    errorMessage = 'Location permission denied. Please enable location access.';
                    break;
                case error.POSITION_UNAVAILABLE:
                    errorMessage = 'Location information is unavailable.';
                    break;
                case error.TIMEOUT:
                    errorMessage = 'Location request timed out.';
                    break;
            }
            showToast(errorMessage, 'error');
        },
        {
            enableHighAccuracy: true,
            timeout: 10000,
            maximumAge: 0
        }
    );
}
