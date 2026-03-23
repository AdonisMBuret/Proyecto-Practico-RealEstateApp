// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// ============================================
// REALESTATE APP - JAVASCRIPT INTERACTIONS
// Mejora UX y cumple Heurísticas de Nielsen
// ============================================

(function() {
    'use strict';

    // === INICIALIZACIÓN AL CARGAR EL DOM ===
    document.addEventListener('DOMContentLoaded', function() {
        initializeTooltips();
        initializePopovers();
        initializeFormValidation();
        initializeImagePreviews();
        initializeDeleteConfirmations();
        initializeAlertAutoDismiss();
        initializeScrollToTop();
        initializePropertyCardAnimations();
        initializeSearchDebounce();
    });

    // === TOOLTIPS (Bootstrap) ===
    function initializeTooltips() {
        const tooltipTriggerList = [].slice.call(document.querySelectorAll('[data-bs-toggle="tooltip"]'));
        tooltipTriggerList.map(function (tooltipTriggerEl) {
            return new bootstrap.Tooltip(tooltipTriggerEl);
        });
    }

    // === POPOVERS (Bootstrap) ===
    function initializePopovers() {
        const popoverTriggerList = [].slice.call(document.querySelectorAll('[data-bs-toggle="popover"]'));
        popoverTriggerList.map(function (popoverTriggerEl) {
            return new bootstrap.Popover(popoverTriggerEl);
        });
    }

    // === VALIDACIÓN DE FORMULARIOS ===
    function initializeFormValidation() {
        const forms = document.querySelectorAll('.needs-validation');
        Array.from(forms).forEach(form => {
            form.addEventListener('submit', event => {
                if (!form.checkValidity()) {
                    event.preventDefault();
                    event.stopPropagation();
                }
                form.classList.add('was-validated');
            }, false);
        });
    }

    // === PREVIEW DE IMÁGENES ===
    function initializeImagePreviews() {
        const imageInputs = document.querySelectorAll('input[type="file"][accept*="image"]');
        
        imageInputs.forEach(input => {
            input.addEventListener('change', function(e) {
                const files = e.target.files;
                const previewContainer = document.getElementById(this.dataset.previewContainer);
                
                if (previewContainer && files.length > 0) {
                    previewContainer.innerHTML = '';
                    
                    Array.from(files).forEach((file, index) => {
                        if (file.type.startsWith('image/')) {
                            const reader = new FileReader();
                            
                            reader.onload = function(event) {
                                const col = document.createElement('div');
                                col.className = 'col-md-3 mb-3';
                                col.innerHTML = `
                                    <div class="card">
                                        <img src="${event.target.result}" class="card-img-top" alt="Preview ${index + 1}" style="height: 150px; object-fit: cover;">
                                        <div class="card-body p-2 text-center">
                                            <small class="text-muted">Imagen ${index + 1}</small>
                                        </div>
                                    </div>
                                `;
                                previewContainer.appendChild(col);
                            };
                            
                            reader.readAsDataURL(file);
                        }
                    });
                }
            });
        });
    }

    // === CONFIRMACIONES DE ELIMINACIÓN ===
    function initializeDeleteConfirmations() {
        const deleteButtons = document.querySelectorAll('[data-confirm-delete]');
        
        deleteButtons.forEach(button => {
            button.addEventListener('click', function(e) {
                const message = this.dataset.confirmDelete || '¿Está seguro que desea eliminar este elemento?';
                if (!confirm(message)) {
                    e.preventDefault();
                }
            });
        });
    }

    // === AUTO-DISMISS DE ALERTAS ===
    function initializeAlertAutoDismiss() {
        const alerts = document.querySelectorAll('.alert:not(.alert-permanent)');
        
        alerts.forEach(alert => {
            setTimeout(() => {
                const bsAlert = new bootstrap.Alert(alert);
                bsAlert.close();
            }, 5000); // 5 segundos
        });
    }

    // === BOTÓN SCROLL TO TOP ===
    function initializeScrollToTop() {
        // Crear botón si no existe
        let scrollButton = document.getElementById('scrollToTopBtn');
        
        if (!scrollButton) {
            scrollButton = document.createElement('button');
            scrollButton.id = 'scrollToTopBtn';
            scrollButton.innerHTML = '<i class="bi bi-arrow-up"></i>';
            scrollButton.className = 'btn btn-primary rounded-circle position-fixed';
            scrollButton.style.cssText = 'bottom: 2rem; right: 2rem; width: 50px; height: 50px; display: none; z-index: 1000; box-shadow: 0 4px 8px rgba(0,0,0,0.3);';
            scrollButton.setAttribute('title', 'Volver arriba');
            document.body.appendChild(scrollButton);
        }
        
        // Mostrar/ocultar según scroll
        window.addEventListener('scroll', function() {
            if (window.pageYOffset > 300) {
                scrollButton.style.display = 'block';
                scrollButton.style.animation = 'fadeIn 0.3s';
            } else {
                scrollButton.style.display = 'none';
            }
        });
        
        // Scroll suave al hacer click
        scrollButton.addEventListener('click', function() {
            window.scrollTo({
                top: 0,
                behavior: 'smooth'
            });
        });
    }

    // === ANIMACIONES DE PROPERTY CARDS ===
    function initializePropertyCardAnimations() {
        const cards = document.querySelectorAll('.property-card, .card');
        
        const observer = new IntersectionObserver((entries) => {
            entries.forEach(entry => {
                if (entry.isIntersecting) {
                    entry.target.style.animation = 'fadeIn 0.6s ease-out';
                    observer.unobserve(entry.target);
                }
            });
        }, {
            threshold: 0.1
        });
        
        cards.forEach(card => {
            observer.observe(card);
        });
    }

    // === DEBOUNCE PARA BÚSQUEDAS ===
    function initializeSearchDebounce() {
        const searchInputs = document.querySelectorAll('input[type="search"], input[data-search]');
        
        searchInputs.forEach(input => {
            let timeout = null;
            
            input.addEventListener('input', function() {
                clearTimeout(timeout);
                
                timeout = setTimeout(() => {
                    // Aquí se puede disparar la búsqueda
                    console.log('Buscando:', this.value);
                }, 500); // 500ms de delay
            });
        });
    }

    // === FORMATEO DE MONEDA ===
    window.formatCurrency = function(amount) {
        return new Intl.NumberFormat('es-DO', {
            style: 'currency',
            currency: 'DOP',
            minimumFractionDigits: 0,
            maximumFractionDigits: 0
        }).format(amount);
    };

    // === LOADING SPINNER ===
    window.showLoading = function(message = 'Cargando...') {
        const loadingDiv = document.createElement('div');
        loadingDiv.id = 'globalLoading';
        loadingDiv.className = 'position-fixed top-0 start-0 w-100 h-100 d-flex align-items-center justify-content-center';
        loadingDiv.style.cssText = 'background: rgba(0,0,0,0.7); z-index: 9999;';
        loadingDiv.innerHTML = `
            <div class="text-center text-white">
                <div class="spinner-border mb-3" style="width: 3rem; height: 3rem;" role="status">
                    <span class="visually-hidden">Cargando...</span>
                </div>
                <div class="fs-5">${message}</div>
            </div>
        `;
        document.body.appendChild(loadingDiv);
    };

    window.hideLoading = function() {
        const loadingDiv = document.getElementById('globalLoading');
        if (loadingDiv) {
            loadingDiv.remove();
        }
    };

    // === CONFIRMACIÓN MEJORADA ===
    window.confirmAction = function(message, onConfirm) {
        if (confirm(message)) {
            if (typeof onConfirm === 'function') {
                onConfirm();
            }
            return true;
        }
        return false;
    };

    // === TOAST NOTIFICATIONS ===
    window.showToast = function(message, type = 'info') {
        const toastContainer = document.getElementById('toastContainer') || createToastContainer();
        
        const toastId = 'toast-' + Date.now();
        const bgClass = {
            'success': 'bg-success',
            'error': 'bg-danger',
            'warning': 'bg-warning',
            'info': 'bg-info'
        }[type] || 'bg-info';
        
        const icon = {
            'success': 'bi-check-circle-fill',
            'error': 'bi-x-circle-fill',
            'warning': 'bi-exclamation-triangle-fill',
            'info': 'bi-info-circle-fill'
        }[type] || 'bi-info-circle-fill';
        
        const toastHTML = `
            <div id="${toastId}" class="toast ${bgClass} text-white" role="alert" aria-live="assertive" aria-atomic="true">
                <div class="toast-body">
                    <i class="bi ${icon} me-2"></i>
                    ${message}
                    <button type="button" class="btn-close btn-close-white float-end" data-bs-dismiss="toast" aria-label="Close"></button>
                </div>
            </div>
        `;
        
        toastContainer.insertAdjacentHTML('beforeend', toastHTML);
        
        const toastElement = document.getElementById(toastId);
        const toast = new bootstrap.Toast(toastElement, { delay: 3000 });
        toast.show();
        
        toastElement.addEventListener('hidden.bs.toast', function() {
            toastElement.remove();
        });
    };

    function createToastContainer() {
        const container = document.createElement('div');
        container.id = 'toastContainer';
        container.className = 'toast-container position-fixed top-0 end-0 p-3';
        container.style.zIndex = '9999';
        document.body.appendChild(container);
        return container;
    }

    // === COPIAR AL PORTAPAPELES ===
    window.copyToClipboard = function(text) {
        if (navigator.clipboard) {
            navigator.clipboard.writeText(text).then(() => {
                showToast('Copiado al portapapeles', 'success');
            }).catch(() => {
                showToast('Error al copiar', 'error');
            });
        }
    };

})();
