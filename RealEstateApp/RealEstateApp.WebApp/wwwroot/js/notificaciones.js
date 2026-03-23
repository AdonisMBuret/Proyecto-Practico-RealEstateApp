// =====================================================
// SISTEMA DE NOTIFICACIONES PARA AGENTES
// =====================================================

(function () {
    'use strict';

    // Configuración
    const CONFIG = {
        updateInterval: 30000, // 30 segundos
        apiEndpoint: '/api/notificaciones',
        maxNotificacionesVisible: 5
    };

    // Estado
    let notificacionesState = {
        total: 0,
        mensajes: 0,
        ofertas: 0,
        lista: []
    };

    /**
     * Inicializar el sistema de notificaciones
     */
    function init() {
        if (!isAgente()) return;

        cargarNotificaciones();
        
        // Actualizar periódicamente
        setInterval(cargarNotificaciones, CONFIG.updateInterval);
        
        // Eventos
        setupEventListeners();
        
        console.log('? Sistema de notificaciones inicializado');
    }

    /**
     * Verificar si el usuario es agente
     */
    function isAgente() {
        const notifBadge = document.getElementById('notificacionesCount');
        return notifBadge !== null;
    }

    /**
     * Cargar notificaciones desde el servidor
     */
    async function cargarNotificaciones() {
        try {
            const response = await fetch('/Agente/GetNotificaciones');
            
            if (!response.ok) {
                throw new Error('Error al cargar notificaciones');
            }
            
            const data = await response.json();
            actualizarNotificaciones(data);
            
        } catch (error) {
            console.error('Error cargando notificaciones:', error);
        }
    }

    /**
     * Actualizar UI con nuevas notificaciones
     */
    function actualizarNotificaciones(data) {
        notificacionesState = {
            total: data.totalNoLeidas || 0,
            mensajes: data.mensajesNuevos || 0,
            ofertas: data.ofertasNuevas || 0,
            lista: data.ultimasNotificaciones || []
        };

        // Actualizar badges
        actualizarBadges();
        
        // Actualizar lista de notificaciones
        renderizarNotificaciones();
        
        // Mostrar notificación del navegador si hay nuevas
        if (notificacionesState.total > 0 && document.hidden) {
            mostrarNotificacionNavegador();
        }
    }

    /**
     * Actualizar badges de notificación
     */
    function actualizarBadges() {
        // Badge total de notificaciones
        const notifBadge = document.getElementById('notificacionesCount');
        if (notifBadge) {
            notifBadge.setAttribute('data-count', notificacionesState.total);
        }

        // Badge de mensajes
        const mensajesBadge = document.getElementById('mensajesCount');
        if (mensajesBadge) {
            mensajesBadge.setAttribute('data-count', notificacionesState.mensajes);
        }
    }

    /**
     * Renderizar lista de notificaciones
     */
    function renderizarNotificaciones() {
        const container = document.getElementById('listaNotificaciones');
        const noNotificaciones = document.getElementById('noNotificaciones');
        
        if (!container) return;

        // Si no hay notificaciones
        if (notificacionesState.lista.length === 0) {
            container.innerHTML = '';
            if (noNotificaciones) {
                noNotificaciones.style.display = 'block';
            }
            return;
        }

        // Ocultar mensaje de "no hay notificaciones"
        if (noNotificaciones) {
            noNotificaciones.style.display = 'none';
        }

        // Renderizar notificaciones
        const html = notificacionesState.lista
            .slice(0, CONFIG.maxNotificacionesVisible)
            .map(notif => crearItemNotificacion(notif))
            .join('');

        container.innerHTML = html;
    }

    /**
     * Crear HTML para un item de notificación
     */
    function crearItemNotificacion(notif) {
        const iconoClase = notif.icono || 'bi-bell';
        const tipoClase = getTipoClase(notif.tipo);
        
        return `
            <li>
                <a class="dropdown-item notification-item ${notif.esLeida ? 'leida' : 'no-leida'}" 
                   href="${notif.url || '#'}" 
                   data-id="${notif.id}">
                    <div class="d-flex align-items-start gap-3">
                        <div class="notification-icon ${tipoClase}">
                            <i class="${iconoClase}"></i>
                        </div>
                        <div class="notification-content flex-grow-1">
                            <h6 class="notification-title mb-1">${escapeHtml(notif.titulo)}</h6>
                            <p class="notification-desc mb-1">${escapeHtml(notif.descripcion)}</p>
                            <small class="notification-time text-muted">
                                <i class="bi bi-clock"></i> ${notif.tiempoTranscurrido}
                            </small>
                        </div>
                        ${!notif.esLeida ? '<span class="badge bg-primary rounded-circle p-1"></span>' : ''}
                    </div>
                </a>
            </li>
        `;
    }

    /**
     * Obtener clase CSS según tipo de notificación
     */
    function getTipoClase(tipo) {
        switch (tipo) {
            case 'Mensaje':
                return 'notification-mensaje';
            case 'Oferta':
                return 'notification-oferta';
            default:
                return 'notification-sistema';
        }
    }

    /**
     * Escapar HTML para prevenir XSS
     */
    function escapeHtml(text) {
        const div = document.createElement('div');
        div.textContent = text;
        return div.innerHTML;
    }

    /**
     * Mostrar notificación del navegador
     */
    function mostrarNotificacionNavegador() {
        if (!('Notification' in window)) return;

        if (Notification.permission === 'granted') {
            new Notification('Real Estate App', {
                body: `Tienes ${notificacionesState.total} notificación(es) nueva(s)`,
                icon: '/images/logo.png',
                badge: '/images/badge.png',
                tag: 'real-estate-notification'
            });
        } else if (Notification.permission !== 'denied') {
            Notification.requestPermission().then(permission => {
                if (permission === 'granted') {
                    mostrarNotificacionNavegador();
                }
            });
        }
    }

    /**
     * Configurar event listeners
     */
    function setupEventListeners() {
        // Solicitar permiso para notificaciones del navegador
        document.addEventListener('DOMContentLoaded', () => {
            if ('Notification' in window && Notification.permission === 'default') {
                setTimeout(() => {
                    Notification.requestPermission();
                }, 3000); // Esperar 3 segundos antes de solicitar
            }
        });

        // Marcar como leída al hacer clic
        document.addEventListener('click', (e) => {
            const notifItem = e.target.closest('.notification-item');
            if (notifItem) {
                marcarComoLeida(notifItem.dataset.id);
            }
        });

        // Ver todas las notificaciones
        const verTodas = document.getElementById('verTodasNotificaciones');
        if (verTodas) {
            verTodas.addEventListener('click', (e) => {
                e.preventDefault();
                window.location.href = '/Agente/Notificaciones';
            });
        }
    }

    /**
     * Marcar notificación como leída
     */
    async function marcarComoLeida(notifId) {
        try {
            await fetch(`/Agente/MarcarNotificacionLeida/${notifId}`, {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json'
                }
            });
            
            // Recargar notificaciones
            cargarNotificaciones();
            
        } catch (error) {
            console.error('Error marcando notificación como leída:', error);
        }
    }

    // Inicializar cuando el DOM esté listo
    if (document.readyState === 'loading') {
        document.addEventListener('DOMContentLoaded', init);
    } else {
        init();
    }

    // Exponer API pública
    window.NotificacionesApp = {
        recargar: cargarNotificaciones,
        obtenerEstado: () => ({ ...notificacionesState })
    };

})();

// =====================================================
// ESTILOS CSS PARA NOTIFICACIONES
// =====================================================
const styles = `
<style>
.notifications-dropdown {
    width: 380px;
    max-height: 500px;
    overflow-y: auto;
    box-shadow: 0 10px 40px rgba(0, 0, 0, 0.15);
}

.notification-item {
    padding: 1rem;
    border-bottom: 1px solid #f0f0f0;
    transition: background-color 0.2s ease;
}

.notification-item:hover {
    background-color: #f8f9fa;
}

.notification-item.no-leida {
    background-color: #f0f8ff;
}

.notification-icon {
    width: 40px;
    height: 40px;
    border-radius: 50%;
    display: flex;
    align-items: center;
    justify-content: center;
    font-size: 1.2rem;
    flex-shrink: 0;
}

.notification-icon.notification-mensaje {
    background-color: #e3f2fd;
    color: #1976d2;
}

.notification-icon.notification-oferta {
    background-color: #fff3e0;
    color: #f57c00;
}

.notification-icon.notification-sistema {
    background-color: #f3e5f5;
    color: #7b1fa2;
}

.notification-content {
    min-width: 0;
}

.notification-title {
    font-size: 0.9rem;
    font-weight: 600;
    color: #333;
    margin: 0;
}

.notification-desc {
    font-size: 0.85rem;
    color: #666;
    margin: 0;
    overflow: hidden;
    text-overflow: ellipsis;
    display: -webkit-box;
    -webkit-line-clamp: 2;
    -webkit-box-orient: vertical;
}

.notification-time {
    font-size: 0.75rem;
    color: #999;
}

.dropdown-header {
    font-size: 1rem;
    padding: 1rem;
    background-color: #f8f9fa;
}

@media (max-width: 768px) {
    .notifications-dropdown {
        width: 320px;
    }
}
</style>
`;

// Inyectar estilos
if (document.head) {
    document.head.insertAdjacentHTML('beforeend', styles);
}
