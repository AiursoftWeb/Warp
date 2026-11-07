(() => {
    const labels = document.getElementById('short-link-text');
    const toast = document.getElementById('short-link-toast');
    if (!labels || !toast) return;
    const text = (key, fallback) => labels.querySelector(`[data-key="${key}"]`)?.textContent.trim() || fallback;
    const focusTarget = button => button.closest('.dropdown')?.querySelector('[data-bs-toggle="dropdown"]') || button;
    const modal = document.getElementById('short-link-modal');
    const input = document.getElementById('short-link-input');
    let returnFocus;
    let fallbackTimer;

    modal.addEventListener('shown.bs.modal', () => {
        input.focus();
        input.select();
    });
    modal.addEventListener('hidden.bs.modal', () => {
        returnFocus?.focus();
        returnFocus = null;
    });

    function showCopied(button) {
        if (window.bootstrap?.Toast) {
            window.bootstrap.Toast.getOrCreateInstance(toast, { delay: 1800 }).show();
        } else {
            window.clearTimeout(fallbackTimer);
            toast.classList.add('show');
            fallbackTimer = window.setTimeout(() => toast.classList.remove('show'), 1800);
        }
        focusTarget(button).focus();
    }

    function showManualCopy(button, link) {
        const message = text('manual', 'Automatic copying is unavailable. Copy this link manually.');
        if (!window.bootstrap?.Modal) {
            window.prompt(message, link);
            focusTarget(button).focus();
            return;
        }
        input.value = link;
        returnFocus = focusTarget(button);
        window.bootstrap.Modal.getOrCreateInstance(modal).show();
    }

    document.querySelectorAll('[data-copy-short-link]').forEach(button => {
        button.addEventListener('click', async () => {
            const link = button.dataset.copyShortLink;
            button.disabled = true;
            let copied = false;
            try {
                if (!navigator.clipboard?.writeText) throw new Error('Clipboard unavailable');
                await navigator.clipboard.writeText(link);
                copied = true;
            } catch {
                // The manual route below also works without clipboard permission.
            } finally {
                button.disabled = false;
            }
            if (copied) showCopied(button);
            else showManualCopy(button, link);
        });
    });
})();
