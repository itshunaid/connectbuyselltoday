$(document).ready(function () {
    // Initialize variables
    let currentStep = 1;
    const totalSteps = 4;
    let uploadedFiles = [];
    const maxFiles = 5;

    // Initialize form validation
    initializeValidation();

    // Initialize event listeners
    initializeEventListeners();

    // Update next button state based on current step validity
    updateNextButtonState();


    // Category selection
    window.selectCategory = function (categoryId) {
        $('#categorySelect').val(categoryId).trigger('change');
        $('.category-card').removeClass('selected');
        $('.category-card[data-category-id="' + categoryId + '"]').addClass('selected');
    };

    // Category select change
    $('#categorySelect').on('change', function () {
        const categoryId = $(this).val();
        $('.category-card').removeClass('selected');
        if (categoryId) {
            $('.category-card[data-category-id="' + categoryId + '"]').addClass('selected');
        }
        // Trigger validation and update button state
        $(this).valid();
        updateNextButtonState();
    });


    // Title character counter and validation
    $('#adTitle').on('input', function () {
        const length = $(this).val().length;
        $('#titleCharCount').text(length);
        if (length >= 10) {
            $('#titleCharCount').removeClass('text-danger').addClass('text-success');
        } else {
            $('#titleCharCount').removeClass('text-success').addClass('text-danger');
        }
        // Trigger validation and update button state
        $(this).valid();
        updateNextButtonState();
    });


    // Price validation - ensure positive number
    $('#adPrice').on('input change', function () {
        const value = parseFloat($(this).val());
        if (value <= 0 || isNaN(value)) {
            $(this).addClass('is-invalid');
        } else {
            $(this).removeClass('is-invalid');
        }
        // Trigger validation and update button state
        $(this).valid();
        updateNextButtonState();
    });


    // Drag and drop functionality
    const dropZone = document.getElementById('dropZone');
    const fileInput = document.getElementById('fileInput');

    // Prevent default drag behaviors
    ['dragenter', 'dragover', 'dragleave', 'drop'].forEach(eventName => {
        dropZone.addEventListener(eventName, preventDefaults, false);
        document.body.addEventListener(eventName, preventDefaults, false);
    });

    function preventDefaults(e) {
        e.preventDefault();
        e.stopPropagation();
    }

    // Highlight drop zone when dragging over
    ['dragenter', 'dragover'].forEach(eventName => {
        dropZone.addEventListener(eventName, () => dropZone.classList.add('dragover'), false);
    });

    ['dragleave', 'drop'].forEach(eventName => {
        dropZone.addEventListener(eventName, () => dropZone.classList.remove('dragover'), false);
    });

    // Handle dropped files
    dropZone.addEventListener('drop', handleDrop, false);

    function handleDrop(e) {
        const dt = e.dataTransfer;
        const files = dt.files;
        handleFiles(files);
    }

    // Handle file input change
    fileInput.addEventListener('change', function () {
        handleFiles(this.files);
    });

    function handleFiles(files) {
        const filesArray = Array.from(files);
        
        if (uploadedFiles.length + filesArray.length > maxFiles) {
            alert(`You can only upload a maximum of ${maxFiles} images.`);
            return;
        }

        filesArray.forEach(file => {
            // Validate file type
            if (!file.type.startsWith('image/')) {
                alert('Only image files are allowed');
                return;
            }

            // Validate file size (5MB max)
            if (file.size > 5 * 1024 * 1024) {
                alert('File size must be less than 5MB');
                return;
            }

            uploadedFiles.push(file);
            previewFile(file);
        });

        updateFileInput();
        updateImageCount();
    }

    function previewFile(file) {
        const reader = new FileReader();
        reader.readAsDataURL(file);
        reader.onloadend = function () {
            const index = uploadedFiles.length - 1;
            const previewHtml = `
                <div class="col-6 col-md-4 col-lg-3 preview-item" data-index="${index}">
                    <img src="${reader.result}" class="preview-image" alt="Preview">
                    <div class="preview-remove" onclick="removeImage(${index})">
                        <i class="bi bi-x"></i>
                    </div>
                </div>
            `;
            $('#previewRow').append(previewHtml);
            $('#previewContainer').show();
        };
    }

    function updateFileInput() {
        // Create a new DataTransfer to update the file input
        const dataTransfer = new DataTransfer();
        uploadedFiles.forEach(file => {
            dataTransfer.items.add(file);
        });
        fileInput.files = dataTransfer.files;
    }

    function updateImageCount() {
        const count = uploadedFiles.length;
        let countText = `${count} image${count !== 1 ? 's' : ''} selected`;
        if (count >= maxFiles) {
            countText += ' (maximum reached)';
        }
        
        // Add or update count display
        let countEl = $('.image-count');
        if (countEl.length === 0) {
            $('#dropZone').after(`<p class="image-count mt-2">${countText}</p>`);
        } {
            countEl.text(countText);
        }
    }

    // Remove image function (global scope)
    window.removeImage = function (index) {
        uploadedFiles.splice(index, 1);
        // Re-render all previews
        $('#previewRow').empty();
        uploadedFiles.forEach((file, i) => {
            const reader = new FileReader();
            reader.readAsDataURL(file);
            reader.onloadend = function () {
                const previewHtml = `
                    <div class="col-6 col-md-4 col-lg-3 preview-item" data-index="${i}">
                        <img src="${reader.result}" class="preview-image" alt="Preview">
                        <div class="preview-remove" onclick="removeImage(${i})">
                            <i class="bi bi-x"></i>
                        </div>
                    </div>
                `;
                $('#previewRow').append(previewHtml);
            };
        });
        
        updateFileInput();
        updateImageCount();
        
        if (uploadedFiles.length === 0) {
            $('#previewContainer').hide();
        }
    };

    // Step navigation
    window.changeStep = function (direction) {
        // Validate current step before moving forward
        if (direction === 1 && !validateCurrentStep()) {
            return;
        }

        // Hide current step
        $('#step' + currentStep).hide();
        
        // Update step
        currentStep += direction;

        // Show new step
        $('#step' + currentStep).show();

        // Update progress bar
        const progress = (currentStep / totalSteps) * 100;
        $('#progressBar').css('width', progress + '%');
        $('.progress-text').text('Step ' + currentStep + ' of ' + totalSteps);

        // Update step labels
        $('.step-label').removeClass('active');
        $('#step' + currentStep + 'Label').addClass('active');

        // Update buttons
        updateButtons();
        
        // Update next button state for new step
        updateNextButtonState();
    };


    function updateButtons() {
        // Previous button
        if (currentStep === 1) {
            $('#prevBtn').prop('disabled', true);
        } else {
            $('#prevBtn').prop('disabled', false);
        }

        // Next/Submit buttons
        if (currentStep === totalSteps) {
            $('#nextBtn').hide();
            $('#submitBtn').show();
        } else {
            $('#nextBtn').show();
            $('#submitBtn').hide();
        }
    }

    function validateCurrentStep() {
        let isValid = true;
        
        // Use jquery.validate to check current step fields
        switch (currentStep) {
            case 1: // Category
                isValid = $('#categorySelect').valid();
                if (!isValid) {
                    $('#categorySelect').focus();
                }
                break;
                
            case 2: // Ad Details
                const titleValid = $('#adTitle').valid();
                const priceValid = $('#adPrice').valid();
                isValid = titleValid && priceValid;
                if (!isValid) {
                    if (!titleValid) {
                        $('#adTitle').focus();
                    } else {
                        $('#adPrice').focus();
                    }
                }
                break;
                
            case 3: // Images (optional, but warn if none)
                if (uploadedFiles.length === 0) {
                    if (!confirm('You haven\'t uploaded any images. Ads with photos get more views. Continue anyway?')) {
                        isValid = false;
                    }
                }
                break;
                
            case 4: // Location & Contact
                const cityValid = $('#CityName').valid();
                const phoneValid = $('#PhoneNumber').valid();
                isValid = cityValid && phoneValid;
                if (!isValid) {
                    if (!cityValid) {
                        $('#CityName').focus();
                    } else {
                        $('#PhoneNumber').focus();
                    }
                }
                break;
        }
        
        return isValid;
    }

    // Function to update next button state based on current step validity
    function updateNextButtonState() {
        let isValid = true;
        
        switch (currentStep) {
            case 1: // Category
                isValid = $('#categorySelect').val() !== '';
                break;
                
            case 2: // Ad Details
                const title = $('#adTitle').val();
                const price = parseFloat($('#adPrice').val());
                isValid = title && title.length >= 10 && price && price > 0;
                break;
                
            case 3: // Images - always allow next
                isValid = true;
                break;
                
            case 4: // Location & Contact
                const city = $('#CityName').val();
                const phone = $('#PhoneNumber').val();
                isValid = city && city.trim() !== '' && phone && /^[0-9]{10}$/.test(phone);
                break;
        }
        
        // Enable/disable next button
        if (currentStep < totalSteps) {
            $('#nextBtn').prop('disabled', !isValid);
        }
    }


    function initializeValidation() {
        // Add custom validation methods
        $.validator.addMethod("minTitleLength", function (value, element) {
            return this.optional(element) || value.length >= 10;
        }, "Title must be at least 10 characters long");

        $.validator.addMethod("positivePrice", function (value, element) {
            return this.optional(element) || (parseFloat(value) > 0);
        }, "Please enter a valid positive price");

        $.validator.addMethod("validPhone", function (value, element) {
            return this.optional(element) || /^[0-9]{10}$/.test(value);
        }, "Please enter a valid 10-digit phone number");

        // Configure form validation
        $("#postAdForm").validate({
            rules: {
                Title: {
                    required: true,
                    minlength: 10,
                    maxlength: 100
                },
                Price: {
                    required: true,
                    number: true,
                    range: [0.01, 999999999.99]
                },
                CategoryId: {
                    required: true
                },
                CityName: {
                    required: true
                },
                PhoneNumber: {
                    required: true,
                    validPhone: true
                }
            },
            messages: {
                Title: {
                    required: "Please enter a title for your ad",
                    minlength: "Title must be at least 10 characters long",
                    maxlength: "Title cannot exceed 100 characters"
                },
                Price: {
                    required: "Please enter a price",
                    number: "Please enter a valid number",
                    range: "Price must be greater than 0"
                },
                CategoryId: {
                    required: "Please select a category"
                },
                CityName: {
                    required: "Please enter your city"
                },
                PhoneNumber: {
                    required: "Please enter your phone number",
                    validPhone: "Please enter a valid 10-digit phone number"
                }
            },
            errorElement: "span",
            errorClass: "text-danger field-validation-error",
            validClass: "field-validation-valid",
            errorPlacement: function (error, element) {
                // Use the existing validation message span
                const errorSpan = element.siblings('.field-validation-valid, .field-validation-error').first();
                if (errorSpan.length) {
                    errorSpan.removeClass('field-validation-valid').addClass('field-validation-error');
                    errorSpan.html(error.html());
                } else {
                    error.insertAfter(element);
                }
            },
            highlight: function (element, errorClass, validClass) {
                $(element).addClass("is-invalid").removeClass("is-valid");
            },
            unhighlight: function (element, errorClass, validClass) {
                $(element).removeClass("is-invalid").addClass("is-valid");
            },
            success: function (label, element) {
                // Update the validation span to show valid state
                const errorSpan = $(element).siblings('.field-validation-error').first();
                if (errorSpan.length) {
                    errorSpan.removeClass('field-validation-error').addClass('field-validation-valid');
                    errorSpan.html('');
                }
            },
            submitHandler: function (form) {
                // Update file input with uploaded files before submit
                updateFileInput();
                
                // Allow form submission
                form.submit();
            }
        });
    }


    function initializeEventListeners() {
        // Initialize character count for title
        $('#titleCharCount').text($('#adTitle').val().length);
        
        // Category card click
        $('.category-card').on('click', function () {
            const categoryId = $(this).data('category-id');
            selectCategory(categoryId);
        });

        // City and Phone validation listeners
        $('#CityName, #PhoneNumber').on('input change', function () {
            $(this).valid();
            updateNextButtonState();
        });

        // Form submit handler - let jquery.validate handle it
        $('#postAdForm').on('submit', function (e) {
            // Update file input with uploaded files before submit
            updateFileInput();
        });
    }

});
