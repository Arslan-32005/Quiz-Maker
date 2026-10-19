function getFileInput(id) {
    const fileInput = document.getElementById(id);
    if (fileInput == null) {
        return "File input not found.";
    }
    const file = fileInput.files[0];
    if (file == null) {
        return "Please select a file.";
    }
    const name = file.name.toLowerCase();
    if (!name.endsWith(".pdf") && !name.endsWith(".docx")) {
        return "Please select a valid file (PDF or DOCX).";
    }
    if (file.size > 5 * 1024 * 1024) {
        return "File size must be 5 MB or less.";
    }
    return "";
}
document.getElementById("uploadForm").addEventListener("submit", function (e) {
    const errorBox = document.getElementById("errorBox");
    errorBox.textContent = "";

    const fileError = getFileInput("file");
    if (fileError !== "") {
        e.preventDefault();
        errorBox.textContent = fileError;
        return;
    }

    const title = document.getElementById("Title").value.trim();
    if (title === "") {
        e.preventDefault();
        errorBox.textContent = "Title cannot be only spaces.";
        return;
    }

    const countInput = document.getElementById("TotalQuestions");
    const count = Number(countInput.value);
    if (countInput.value.trim() === "" || !Number.isInteger(count) || count < 3 || count > 15) {
        e.preventDefault();
        errorBox.textContent = "Total questions must be a whole number between 3 and 15.";
        return;
    }

    const btn = document.getElementById("uploadBtn");
    btn.disabled = true;
    btn.textContent = "Generating...";
});