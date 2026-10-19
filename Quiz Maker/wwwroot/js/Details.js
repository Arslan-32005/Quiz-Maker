document.addEventListener("DOMContentLoaded", function () {
    const btn = document.getElementById("copyLinkBtn");
    const input = document.getElementById("shareLink");
    const status = document.getElementById("copyStatus");
    if (!btn || !input) {
        return;
    }

    btn.addEventListener("click", function () {
        navigator.clipboard.writeText(input.value).then(function () {
            status.textContent = "Link Copied";
        }).catch(function () {
            input.select();
            status.textContent = "Press Ctrl+C to copy the link";
        });
    });
});