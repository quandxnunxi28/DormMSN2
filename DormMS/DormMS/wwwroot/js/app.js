const toggleBtn = document.getElementById("toggleBtn");
const sidebar = document.getElementById("sidebar");
const content = document.getElementById("content");

// ✅ Check null để tránh crash
if (toggleBtn && sidebar && content) {
    toggleBtn.addEventListener("click", () => {
        sidebar.classList.toggle("collapsed");
        content.classList.toggle("expand");
    });
}

const logoutBtn = document.getElementById("logoutBtn");

if (logoutBtn) {
    logoutBtn.addEventListener("click", function () {

        const token = localStorage.getItem("token");

        fetch("/api/Auth/logout", {
            method: "POST",
            headers: {
                "Authorization": "Bearer " + token
            }
        })
            .finally(() => {
                localStorage.removeItem("token");
                window.location.href = "/login.html";
            });
    });
}