// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

// Inline editing: any <textarea data-inline-edit data-url="..." data-id="..."> saves on blur or Enter.
(function () {
    const token = document.querySelector('input[name="__RequestVerificationToken"]')?.value;

    // Grow/shrink the textarea so long titles wrap instead of scrolling
    function autosize(el) {
        el.style.height = 'auto';
        el.style.height = el.scrollHeight + 'px';
    }

    document.querySelectorAll('textarea[data-inline-edit]').forEach(function (el) {
        let saved = el.value; // last value known to be stored in the database
        autosize(el);

        el.addEventListener('input', function () { autosize(el); });

        el.addEventListener('keydown', function (e) {
            if (e.key === 'Enter') {
                e.preventDefault();   // no line breaks in a title
                el.blur();            // blur triggers the save below
            } else if (e.key === 'Escape') {
                el.value = saved;     // cancel the edit
                autosize(el);
                el.blur();
            }
        });

        el.addEventListener('blur', async function () {
            const title = el.value.trim();

            // Nothing to save: empty titles are not allowed, unchanged titles need no request
            if (!title || title === saved) {
                el.value = saved;
                autosize(el);
                return;
            }

            el.classList.add('saving');
            try {
                const res = await fetch(el.dataset.url, {
                    method: 'POST',
                    headers: {
                        'RequestVerificationToken': token,
                        'Content-Type': 'application/x-www-form-urlencoded'
                    },
                    body: new URLSearchParams({ id: el.dataset.id, title: title })
                });
                if (!res.ok) throw new Error('Save failed: ' + res.status);

                const data = await res.json();
                saved = data.title;   // use the value the server actually stored
                el.value = saved;
                el.classList.remove('error');
            } catch (err) {
                console.error(err);
                el.value = saved;     // put the old title back
                el.classList.add('error');
                setTimeout(function () { el.classList.remove('error'); }, 1500);
            } finally {
                el.classList.remove('saving');
                autosize(el);
            }
        });
    });
})();