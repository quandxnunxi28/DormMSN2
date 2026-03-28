console.log("🚀 signalr.js loaded");

(function () {

    try {
        const token = localStorage.getItem("token");

        if (!token) {
            console.warn("❌ Không có token → không connect SignalR");
            
            return;
        }

        console.log("✅ Có token → bắt đầu connect SignalR");

        const connection = new signalR.HubConnectionBuilder()
            .withUrl("/notificationHub", {
                accessTokenFactory: () => token
            })
            .withAutomaticReconnect() // 🔥 tự reconnect khi mất mạng
            .configureLogging(signalR.LogLevel.Information) // debug chi tiết
            .build();

        // ==========================
        // RECEIVE MESSAGE
        // ==========================
        connection.on("ReceiveNotification", function (message) {
            console.log("📩 Nhận message:", message);

            // nếu có showToast thì dùng
            if (typeof showToast === "function") {
                showToast(message);
            } else {
                alert("📩 " + message);
            }
        });

        // ==========================
        // START CONNECTION
        // ==========================
        connection.start()
            .then(() => {
                console.log("✅ SignalR connected");
                
            })
            .catch(err => {
                console.error("❌ SignalR start error:", err);
            });

        // ==========================
        // RECONNECT EVENTS
        // ==========================
        connection.onreconnecting(error => {
            console.warn("⚠️ Đang reconnect...", error);
        });

        connection.onreconnected(connectionId => {
            console.log("🔁 Reconnected. ID:", connectionId);
        });

        connection.onclose(error => {
            console.error("❌ Connection closed:", error);
        });

    } catch (err) {
        console.error("💥 Lỗi toàn cục SignalR:", err);
    }

})();