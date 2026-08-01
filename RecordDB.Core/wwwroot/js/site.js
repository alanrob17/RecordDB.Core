// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
// Fix nested dropdowns in Bootstrap 5
document.addEventListener('DOMContentLoaded', function () {
    const dropdownSubmenus = document.querySelectorAll('.dropdown-submenu');

    dropdownSubmenus.forEach(submenu => {
        const toggle = submenu.querySelector('.dropdown-toggle');
        const menu = submenu.querySelector('.dropdown-menu');

        if (!toggle || !menu) return;

        // Show submenu on hover (desktop)
        submenu.addEventListener('mouseenter', function () {
            menu.classList.add('show');
            toggle.setAttribute('aria-expanded', 'true');
        });

        // Hide submenu when leaving (desktop)
        submenu.addEventListener('mouseleave', function () {
            menu.classList.remove('show');
            toggle.setAttribute('aria-expanded', 'false');
        });

        // Toggle on click (mobile/touch)
        toggle.addEventListener('click', function (e) {
            e.preventDefault();
            e.stopPropagation();
            menu.classList.toggle('show');
        });
    });

    // Close all submenus when clicking outside
    document.addEventListener('click', function (e) {
        if (!e.target.closest('.dropdown-submenu')) {
            dropdownSubmenus.forEach(submenu => {
                const menu = submenu.querySelector('.dropdown-menu');
                if (menu) {
                    menu.classList.remove('show');
                }
            });
        }
    });
});