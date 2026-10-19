document.addEventListener("DOMContentLoaded", function () {
    const toggle = document.getElementById("togglePassword");
    const passwordInput = document.getElementById("Password");
    if (!toggle || !passwordInput) {
        return;
    }

    function flip() {
        const showing = passwordInput.type === "password";
        passwordInput.type = showing ? "text" : "password";
        toggle.setAttribute("aria-label", showing ? "Hide password" : "Show password");
    }

    toggle.addEventListener("click", flip);
    toggle.addEventListener("keydown", function (e) {
        if (e.key === "Enter" || e.key === " ") {
            e.preventDefault();
            flip();
        }
    });
});