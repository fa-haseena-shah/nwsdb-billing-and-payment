document.addEventListener("DOMContentLoaded", () => {
    
    // login form
    const loginForm = document.getElementById("loginForm");

    if (loginForm) {
        loginForm.addEventListener("submit", async (event) => {
            event.preventDefault();

            const email = requireValue(document.getElementById("loginEmail").value);
            const password = requireValue(document.getElementById("loginPassword").value);

            if (!email || !password) {
                showMessage("message", "Please enter all credentials.");
                return;
            }

            const result = await apiPost("/auth/login", { email, password });

            if (!result.ok) {
                showMessage("message", result.data.message || "Invalid email or password.");
                return;
            }

            saveSession(result.data);

            // redirect based on role
            if (result.data.role === "Customer") {
                window.location.href = "query-acc.html";
            } else {
                window.location.href = "dashboard.html";
            }
        });
    }

    // register form
    const registerForm = document.getElementById("registerForm");

    if (registerForm) {
        registerForm.addEventListener("submit", async (event) => {
            event.preventDefault();
            const sriLankanPhoneRegex = /^(?:\+94|0)\d{9}$/;
            const fullName = requireValue(document.getElementById("fullName").value);
            const email = requireValue(document.getElementById("registerEmail").value);
            const phoneNoRaw = document.getElementById("phoneNo").value;
            const phoneNo = requireValue(phoneNoRaw.replace(/[\s-]/g, ""));
            const nic = requireValue(document.getElementById("nic").value);
            const password = document.getElementById("registerPassword").value;

            if (!fullName || !email || !phoneNo || !nic || !password) {
                showMessage("registerMessage", "All fields are required.");
                return;
            }

            if (!sriLankanPhoneRegex.test(phoneNo)) {
                showMessage("registerMessage", "Please enter a valid Sri Lankan phone number.");
                return;
            }

            if (password.length < 6) {
                showMessage("registerMessage", "Password must be at least 6 characters.");
                return;
            }

            const result = await apiPost("/auth/register", {
                fullName, email, phoneNo, nic, password
            });

            if (!result.ok) {
                showMessage("registerMessage", result.data.message || "Registration failed.");
                return;
            }

            showMessage("registerMessage", "Registration successful. Redirecting to login...", false);

            setTimeout(() => {
                window.location.href = "index.html";
            }, 1200);
        });
    }

    // add partner
    const addPartnerForm = document.getElementById("addPartnerForm");

    if (addPartnerForm) {
        addPartnerForm.addEventListener("submit", async (event) => {
        event.preventDefault();

        const name = requireValue(document.getElementById("partnerName").value);
        const email = requireValue(document.getElementById("partnerEmail").value);

        if (!name || !email) {
            showMessage("addPartnerMessage", "All fields are required.");
            return;
        }

        const result = await apiPost("/auth/staff/create-partner", { name, email });

        if (!result.ok) {
            showMessage("addPartnerMessage", result.data.message || "Could not create partner.");
            return;
        }

        showMessage("addPartnerMessage", "Partner created. Credentials sent by email.", false);
        addPartnerForm.reset();
    });
    }

    // dashboard
    const staffDashboard = document.getElementById("staff-dashboard");
    const partnerDashboard = document.getElementById("partner-dashboard");

    if (staffDashboard || partnerDashboard) {

        const session = requireSession();
        if (!session) return;

        const normalizedRole = (session.role || "").toLowerCase().trim();

        // dont let customer view dashboard
        if (normalizedRole === "customer") {
            window.location.href = "query-acc.html";
            return;
        }

        const staffUserDisplay = document.getElementById("staff-user-display");
        const partnerUserDisplay = document.getElementById("partner-user-display");

        if (staffUserDisplay) staffUserDisplay.textContent = session.fullName;
        if (partnerUserDisplay) partnerUserDisplay.textContent = session.fullName;

        if (normalizedRole === "staff") {
            if (staffDashboard) staffDashboard.style.display = "block";
            if (partnerDashboard) partnerDashboard.style.display = "none";
        }
        else if (normalizedRole === "partner") {
            if (staffDashboard) staffDashboard.style.display = "none";
            if (partnerDashboard) partnerDashboard.style.display = "block";
        }
        else {
            alert("Unable to identify your account role.");
            clearSession();
            window.location.href = "index.html";
            return;
        }

        // staff dashboard btns
        document.getElementById("viewWaterAccountsBtn")?.addEventListener("click", () => {
            window.location.href = "view-water-connections.html";
        });
        document.getElementById("viewCustomersBtn")?.addEventListener("click", () => {
            window.location.href = "view-customers.html";
        });
        document.getElementById("generateBillBtn")?.addEventListener("click", () => {
            window.location.href = "generate-bill.html";
        });
        document.getElementById("managePartnersBtn")?.addEventListener("click", () => {
            window.location.href = "view-partners.html";
        });
        document.getElementById("queryAccBtn")?.addEventListener("click", () => {
            window.location.href = "query-acc.html";
        });

        // partner dashboard btns
        document.getElementById("payBillBtn")?.addEventListener("click", () => {
            window.location.href = "query-acc.html";
        });
        document.getElementById("viewPaymentsBtn")?.addEventListener("click", () => {
            window.location.href = "view-payment-history.html";
        });
    }

    // logout
    function logout() {
        clearSession();
        window.location.href = "index.html";
    }

    document.getElementById("logoutBtn")?.addEventListener("click", logout);
    document.getElementById("staffLogoutBtn")?.addEventListener("click", logout);
    document.getElementById("partnerLogoutBtn")?.addEventListener("click", logout);
});