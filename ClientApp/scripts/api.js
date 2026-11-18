const API_BASE_URL = "http://localhost:5142/api";

// get the necessary details to authorize a user
function getSession() {
    return {
        userId: localStorage.getItem("userId"),
        fullName: localStorage.getItem("fullName"),
        role: localStorage.getItem("role"),
        accountNo: localStorage.getItem("accountNo")
    };
}

// if log in for a partciular session is needed
function requireSession() {
    const session = getSession();

    if (!session.userId || !session.role) {
        window.location.href = "index.html";
        return null;
    }

    return session;
}

// store the obtained authorization details
function saveSession(authResponse) {
    localStorage.setItem("userId", authResponse.userId);
    localStorage.setItem("fullName", authResponse.fullName);
    localStorage.setItem("role", authResponse.role);
}

// for logout
function clearSession() {
    localStorage.removeItem("userId");
    localStorage.removeItem("fullName");
    localStorage.removeItem("role");
    localStorage.removeItem("accountNo");
}

function setSelectedAccount(accountNo) {
    localStorage.setItem("accountNo", accountNo);
}

// creates the request sent to backend and get the response back
async function apiRequest(method, path, body = null) {
    try {
        const options = {
            method,
            headers: { "Content-Type": "application/json" }
        };

        if (body !== null) {
            options.body = JSON.stringify(body);
        }

        const response = await fetch(`${API_BASE_URL}${path}`, options);
        const data = await response.json().catch(() => ({}));

        return { ok: response.ok, status: response.status, data };

    } catch (error) {
        console.error(`API error [${method} ${path}]:`, error);
        return {
            ok: false,
            status: 0,
            data: { message: "Unable to connect to server." }
        };
    }
}

// helper for get requests
function apiGet(path) {
    return apiRequest("GET", path);
}

// helper for post requests
function apiPost(path, body) {
    return apiRequest("POST", path, body);
}

// ui helper functions

function statusBadge(status) {
    if (!status) return "";
    const cls = String(status).toLowerCase();
    return `<span class="status-badge status-${cls}">${status}</span>`;
}

function showMessage(elementId, text, isError = true) {
    const el = document.getElementById(elementId);
    if (!el) return;

    el.textContent = text;
    el.className = isError ? "form-message error" : "form-message success";
}

function clearMessage(elementId) {
    const el = document.getElementById(elementId);
    if (!el) return;

    el.textContent = "";
    el.className = "form-message";
}

function formatCurrency(value) {
    const num = Number(value);
    if (Number.isNaN(num)) return "Rs. 0.00";
    return `Rs. ${num.toFixed(2)}`;
}

function formatDate(isoString) {
    if (!isoString) return "";
    const date = new Date(isoString);
    if (Number.isNaN(date.getTime())) return isoString;

    return date.toLocaleDateString("en-GB", {
        day: "2-digit",
        month: "short",
        year: "numeric"
    });
}

// idempotency key generator for payments
function generateIdempotencyKey() {
    if (crypto?.randomUUID) return crypto.randomUUID();
    // fallback for older browsers
    return "idem-" + Date.now() + "-" + Math.random().toString(36).slice(2);
}

// empty field validation
function requireValue(value) {
    const trimmed = (value ?? "").toString().trim();
    return trimmed.length > 0 ? trimmed : null;
}