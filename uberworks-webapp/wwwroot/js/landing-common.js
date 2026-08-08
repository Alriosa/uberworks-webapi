// =====================================================================================
// FILE SUMMARY
// What it does: Shared modal/toast/ripple behavior used by both LandingPage.cshtml and
//               AllServices.cshtml — the "click a service card, see a detail modal with a
//               photo/description and a 'Pedir servicio' button" interaction lives here once
//               instead of being duplicated in each view's inline <script>. Both pages must
//               render the same #serviceModal/#toast markup (copy-pasted at the bottom of
//               each .cshtml, since Razor partials can't easily be shared with a plain JS
//               file) for these functions to find their elements.
// Entities connected: None
// Tables related: None
// =====================================================================================

// Generic modal show/hide. Assumes a "#<id>" wrapper (opacity/pointer-events toggle) with a
// "#<id>Box" inner box (scale toggle) — see #serviceModal markup on both pages.
function openModal(id) {
    const modal = document.getElementById(id);
    const box = document.getElementById(id + 'Box');
    modal.classList.remove('opacity-0', 'pointer-events-none');
    box.classList.remove('scale-95');
    box.classList.add('scale-100');
}

function closeModal(id) {
    const modal = document.getElementById(id);
    const box = document.getElementById(id + 'Box');
    modal.classList.add('opacity-0', 'pointer-events-none');
    box.classList.remove('scale-100');
    box.classList.add('scale-95');
}

// Service Modal Handler. `icon` is optional — used instead of `img` only as a defensive
// fallback, in case a photo URL ever fails to resolve.
function openServiceModal(title, desc, img, icon) {
    document.getElementById('svcTitle').innerText = title;
    document.getElementById('svcDesc').innerText = desc;

    const svcImg = document.getElementById('svcImg');
    const svcIconWrap = document.getElementById('svcIconWrap');
    const svcIcon = document.getElementById('svcIcon');

    if (img) {
        svcImg.src = img;
        svcImg.classList.remove('hidden');
        svcIconWrap.classList.add('hidden');
        svcIconWrap.classList.remove('flex');
    } else {
        svcIcon.className = `fa-solid ${icon} text-white text-5xl`;
        svcIconWrap.classList.remove('hidden');
        svcIconWrap.classList.add('flex');
        svcImg.classList.add('hidden');
    }

    openModal('serviceModal');
}

// Looks up a category by its serviceCatalog.js key and opens the same detail modal used by
// the curated cards — this is what AllServices.cshtml's 24 tiles call on click, so clicking
// any of them shows the same photo/description/"Pedir servicio" experience as the landing page.
function openServiceModalFromCatalog(key) {
    const entry = serviceCatalog.find(s => s.key === key);
    if (!entry) {
        return;
    }
    openServiceModal(entry.key, entry.desc, entry.img, entry.icon);
}

function confirmOrder() {
    closeModal('serviceModal');
    showToast('¡Solicitud enviada! Buscando al especialista más cercano...');
}

// Dynamic Toast Notification
function showToast(message) {
    const toast = document.getElementById('toast');
    const toastMsg = document.getElementById('toastMessage');
    toastMsg.innerText = message;

    toast.classList.remove('translate-y-20', 'opacity-0');
    toast.classList.add('translate-y-0', 'opacity-100');

    setTimeout(() => {
        toast.classList.remove('translate-y-0', 'opacity-100');
        toast.classList.add('translate-y-20', 'opacity-0');
    }, 3500);
}

// Ripple click effect for any ".btn-alive" button on the page.
document.addEventListener('click', function (e) {
    const button = e.target.closest('.btn-alive');
    if (button) {
        const rect = button.getBoundingClientRect();
        const circle = document.createElement('span');
        const diameter = Math.max(rect.width, rect.height);
        const radius = diameter / 2;

        circle.style.width = circle.style.height = `${diameter}px`;
        circle.style.left = `${e.clientX - rect.left - radius}px`;
        circle.style.top = `${e.clientY - rect.top - radius}px`;
        circle.classList.add('ripple');

        const existingRipple = button.querySelector('.ripple');
        if (existingRipple) {
            existingRipple.remove();
        }

        button.appendChild(circle);
    }
});
