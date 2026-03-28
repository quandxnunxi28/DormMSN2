 <script>
     alert("JS inline chạy OK");
     console.log("🚀 app.js loaded");
     alert(" app.js loaded kakakaka");
</script>
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
                localStorage.removeItem("fullName");
                localStorage.removeItem("role");
                window.location.href = "/login.html";
            });
    });
}
// Hàm giải mã JWT (giữ nguyên vì bắt buộc phải có để đọc token)
function parseJwt(token) {
    try {
        const base64Url = token.split('.')[1];
        const base64 = base64Url.replace(/-/g, '+').replace(/_/g, '/');
        const jsonPayload = decodeURIComponent(window.atob(base64).split('').map(function (c) {
            return '%' + ('00' + c.charCodeAt(0).toString(16)).slice(-2);
        }).join(''));
        return JSON.parse(jsonPayload);
    } catch (e) {
        return null;
    }
}

document.addEventListener("DOMContentLoaded", function () {
    // 1. Lấy token từ localStorage
    const token = localStorage.getItem('token');
    let isAdmin = false;

    // 2. Giải mã và kiểm tra role
    if (token) {
        const decodedToken = parseJwt(token);
        
        if (decodedToken) {
            // Đọc Claim Role từ C# gen ra
            const role = decodedToken["http://schemas.microsoft.com/ws/2008/06/identity/claims/role"];
            console.log("Role giải mã được:", role); // Cắm cờ kiểm tra số 2
            // Nếu role là admin thì set cờ true
            if (role && role.toLowerCase() === 'admin') {
                isAdmin = true;
            }
        }
    }

    // 3. Xử lý DOM (giao diện)
    const adminMenu = document.getElementById('adminMenu');
    // Nếu có menu này trên trang
    if (adminMenu) {
        if (isAdmin) {
            adminMenu.style.display = 'block'; // Mở lên cho Admin
        } else {
            adminMenu.remove(); // Xóa bay màu luôn nếu là Student (hoặc chưa đăng nhập)
        }
    }
    const btnAdd = document.getElementById('btn-add-news');
    if (btnAdd) {
        if (isAdmin) {
            btnAdd.style.display = 'block'; 
        } else {
            btnAdd.remove(); 
        }
    }
    const btnEdit = document.getElementById('btn-edit-news');
    if (btnEdit) {
        if (isAdmin) {
            btnEdit.style.display = 'block';
        } else {
            btnEdit.remove();
        }
    }
    const btnDelete = document.getElementById('btn-delete-news');
    if (btnDelete) {
        if (isAdmin) {
            btnDelete.style.display = 'block';
        } else {
            btnDelete.remove();
        }
    }

});



// ==========================
// SIGNALR GLOBAL
// ==========================

// Tạo kết nối tới server


// ==========================
// UI Toast
// ==========================
function showToast(msg) {
    const div = document.createElement("div");
    div.innerText = msg;

    div.style.position = "fixed";
    div.style.top = "20px";
    div.style.right = "20px";
    div.style.background = "#4caf50";
    div.style.color = "white";
    div.style.padding = "10px";
    div.style.borderRadius = "5px";

    document.body.appendChild(div);

    setTimeout(() => div.remove(), 3000);
}