// Sidebar toggle mobile
document.addEventListener('DOMContentLoaded', function () {
    const toggle = document.getElementById('sidebarToggle');
    const sidebar = document.getElementById('sidebar');
    const overlay = document.getElementById('sidebarOverlay');

    if (toggle && sidebar) {
        toggle.addEventListener('click', function () {
            sidebar.classList.toggle('show');
            overlay && overlay.classList.toggle('show');
        });
        overlay && overlay.addEventListener('click', function () {
            sidebar.classList.remove('show');
            overlay.classList.remove('show');
        });
    }

    // Mostrar toasts desde TempData vía campo oculto
    const toastData = document.getElementById('toast-data');
    if (toastData && window.toastr) {
        const raw = toastData.value;
        if (raw) {
            const idx = raw.indexOf('|');
            const tipo = raw.substring(0, idx);
            const msg = raw.substring(idx + 1);
            const opts = { timeOut: 4000, closeButton: true, progressBar: true };
            if (tipo === 'success') toastr.success(msg, 'Éxito', opts);
            else if (tipo === 'error') toastr.error(msg, 'Error', opts);
            else if (tipo === 'warning') toastr.warning(msg, 'Atención', opts);
            else toastr.info(msg, 'Info', opts);
        }
    }
});

function confirmarAccion(mensaje, formId) {
    Swal.fire({
        title: '¿Estás seguro?',
        text: mensaje,
        icon: 'warning',
        showCancelButton: true,
        confirmButtonColor: '#A8576B',
        cancelButtonColor: '#5F5E5A',
        confirmButtonText: 'Sí, continuar',
        cancelButtonText: 'Cancelar'
    }).then((result) => {
        if (result.isConfirmed) {
            document.getElementById(formId).submit();
        }
    });
}

function confirmarLogout() {
    Swal.fire({
        title: '¿Cerrar sesión?',
        text: 'Se cerrará tu sesión actual.',
        icon: 'question',
        showCancelButton: true,
        confirmButtonColor: '#A8576B',
        cancelButtonColor: '#5F5E5A',
        confirmButtonText: 'Sí, salir',
        cancelButtonText: 'Cancelar'
    }).then((result) => {
        if (result.isConfirmed) {
            document.getElementById('logoutForm').submit();
        }
    });
}
