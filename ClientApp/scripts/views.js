document.addEventListener("DOMContentLoaded", () => {

    // query account
    const customerSection = document.getElementById("query-customer-main");
    const staffSection = document.getElementById("query-staff-main");
    const partnerSection = document.getElementById("query-partner-main");

    if (customerSection || staffSection || partnerSection) {
        const session = requireSession();
        if (!session) return;

        const role = (session.role || "").toLowerCase().trim();

        const userDisplay = document.getElementById("user-display");
        if (userDisplay) userDisplay.textContent = session.fullName;

        // hide the other fields based on role
        if (role === "customer") {
            if (customerSection) customerSection.style.display = "block";
            if (staffSection) staffSection.style.display = "none";
            if (partnerSection) partnerSection.style.display = "none";
        } else if (role === "staff") {
            if (customerSection) customerSection.style.display = "none";
            if (staffSection) staffSection.style.display = "block";
            if (partnerSection) partnerSection.style.display = "none";
        } else if (role === "partner") {
            if (customerSection) customerSection.style.display = "none";
            if (staffSection) staffSection.style.display = "none";
            if (partnerSection) partnerSection.style.display = "block";
        } else {
            window.location.href = "index.html";
            return;
        }

        // customer btns
        document.getElementById("viewBillBtn")?.addEventListener("click", () => {
            goToAccountView("custAccountNo", "custMessage", "view-bill.html");
        });
        document.getElementById("viewPaymentsBtn")?.addEventListener("click", () => {
            goToAccountView("custAccountNo", "custMessage", "view-payment-history.html");
        });
        document.getElementById("viewUsageBtn")?.addEventListener("click", () => {
            goToAccountView("custAccountNo", "custMessage", "view-usage.html");
        });
        document.getElementById("viewAddressBtn")?.addEventListener("click", () => {
            checkAddressWithNicPrompt("custAccountNo", "custMessage");
        });

        // staff btns
        document.getElementById("getDetailsBtn")?.addEventListener("click", async () => {
            const accountNo = requireValue(document.getElementById("staffAccountNo").value);

            if (!accountNo) {
                showMessage("staffMessage", "Please enter an account number.");
                return;
            }

            const result = await apiGet(`/views/staff-account-details/${encodeURIComponent(accountNo)}`);

            if (!result.ok) {
                showMessage("staffMessage", result.data.message || "No account found with that number.");
                return;
            }

            setSelectedAccount(accountNo);
            window.location.href = "view-acc-details.html";
        });

        document.getElementById("staffViewUsageBtn")?.addEventListener("click", () => {
            goToAccountView("staffAccountNo", "staffMessage", "view-usage.html");
        });
        document.getElementById("staffViewPaymentsBtn")?.addEventListener("click", () => {
            goToAccountView("staffAccountNo", "staffMessage", "view-payment-history.html");
        });
        

        // partner btns
        document.getElementById("partnerViewBillBtn")?.addEventListener("click", () => {
            goToAccountView("partnerAccountNo", "partnerMessage", "view-bill.html");
        });
        document.getElementById("partnerViewAddressBtn")?.addEventListener("click", () => {
            checkAddressWithNicPrompt("partnerAccountNo", "partnerMessage");
        });
    }

    // view bill
    if (document.getElementById("bill-id-display") && !document.getElementById("cardNumber")) {
        loadBill();
    }

    // view acc details by staff
    if (document.getElementById("connectionNo")) {
        loadStaffAccountDetails();
    }

    // view payment history
    if (document.getElementById("payment-rows")) {
        loadPaymentHistory();
    }

    // view usage
    if (document.getElementById("usage-rows")) {
        loadUsage();
    }

    // generate billl
    if (document.getElementById("unitsUsed")) {
        initGenerateBill();
    }

    // view water connection accounts
    if (document.body.dataset.list === "water-accounts") {
        loadWaterAccounts(1);
    }

    // customers list pagination
    if (document.body.dataset.list === "customers") {
        loadCustomers(1);
    }

    // partners list
    if (document.body.dataset.list === "partners") {
        loadPartners(1);
    }

    // pay bill
    if (document.getElementById("cardNumber")) {
        initPayBill();
    }
});

// helper method to format text into dynamic pgs
function setText(elementId, value) {
    const el = document.getElementById(elementId);
    if (el) el.textContent = value;
}

// MAIN FUNCTIONS..

async function goToAccountView(inputId, messageId, page) {
    const accountNoInput = document.getElementById(inputId);
    if (!accountNoInput) return;

    const accountNo = requireValue(accountNoInput.value);

    if (!accountNo) {
        showMessage(messageId, "Please enter an account number.");
        return;
    }

    const result = await apiGet(`/views/account-exists/${encodeURIComponent(accountNo)}`);

    if (!result.ok || !result.data.exists) {
        showMessage(messageId, "No account found with that number.");
        return;
    }

    setSelectedAccount(accountNo);
    window.location.href = page;
}

async function checkAddressWithNicPrompt(inputId, messageId) {
    const session = requireSession();
    if (!session) return;

    const accountNoInput = document.getElementById(inputId);
    if (!accountNoInput) return;

    const accountNo = requireValue(accountNoInput.value);

    if (!accountNo) {
        showMessage(messageId, "Please enter an account number.");
        return;
    }

    const nic = prompt("Please enter the NIC associated with this account:");

    if (nic === null) return; // if user cancels

    const trimmedNic = requireValue(nic);

    if (!trimmedNic) {
        showMessage(messageId, "NIC is required to view the address.");
        return;
    }

    const result = await apiPost("/views/account-details", {
        accountNo,
        role: session.role,
        callerId: parseInt(session.userId),
        nic: trimmedNic
    });

    if (!result.ok) {
        showMessage(messageId, result.data.message || "Account not found, access denied, or NIC does not match.");
        return;
    }

    clearMessage(messageId);

    const account = result.data;
    alert(
        `Account No: ${account.accountNo}\n` +
        `Address: ${account.address}\n` +
        `Status: ${account.status}`
    );
}

// ** VIEW BILL **
async function loadBill() {
    const session = requireSession();
    if (!session) return;

    const accountNo = session.accountNo;
    if (!accountNo) {
        window.location.href = getHomePage();
        return;
    }

    setText("acc-no-display", accountNo);

    const result = await apiPost("/views/bill", {
        accountNo,
        role: session.role,
        callerId: parseInt(session.userId)
    });

    if (!result.ok) {
        const card = document.querySelector(".bill-card");
        if (card) card.innerHTML = `<p>${result.data.message || "Could not fetch bill."}</p>`;
        return;
    }

    const bill = result.data;

    setText("bill-id-display", bill.billId);
    setText("issued-date-display", formatDate(bill.issuedDate));
    setText("due-date-display", formatDate(bill.dueDate));
    setText("units-used-display", bill.unitsUsed);
    setText("total-payable-display", formatCurrency(bill.totalPayable));

    const payBtn = document.querySelector(".view-bill.primary-btn");
    if (payBtn) {
        if (bill.status === "Paid") {
            payBtn.disabled = true;
            payBtn.textContent = "Already Paid";
        } else {
            payBtn.addEventListener("click", () => {
                window.location.href = "pay-bill.html";
            });
        }
    }

    document.getElementById("back-btn")?.addEventListener("click", () => {
        window.location.href = getHomePage();
    });
}

function getHomePage() {
    const role = (localStorage.getItem("role") || "").toLowerCase();
    if (role === "customer") return "query-acc.html";
    return "dashboard.html";
}

// ** STAFF VIEWING ACC DETAILS **
async function loadStaffAccountDetails() {
    const session = requireSession();
    if (!session) return;

    const accountNo = session.accountNo;
    if (!accountNo) {
        window.location.href = "dashboard.html";
        return;
    }

    const result = await apiGet(`/views/staff-account-details/${encodeURIComponent(accountNo)}`);

    if (!result.ok) {
        setText("connectionNo", "Not found");
        return;
    }

    const account = result.data;

    setText("connectionNo", account.accountNo);
    setText("addressDisplay", account.address);
    setText("statusDisplay", account.status);
    setText("customerNameDisplay", account.customerName);
    setText("phoneEmailDisplay", `${account.contactPhone} / ${account.contactEmail}`);
}

// ** VIEW PAYMENT HISTORY **
async function loadPaymentHistory() {
    const session = requireSession();
    if (!session) return;

    const role = (session.role || "").toLowerCase();
    const rowsContainer = document.getElementById("payment-rows");
    const accNoDisplay = document.getElementById("acc-no-display");

    let result;

    if (role === "partner") {
        if (accNoDisplay) accNoDisplay.style.display = "none";
        result = await apiGet(`/views/partner-payments/${session.userId}`);
    } else {
        const accountNo = session.accountNo;
        if (!accountNo) {
            window.location.href = getHomePage();
            return;
        }
        if (accNoDisplay) accNoDisplay.textContent = accountNo;
        result = await apiPost("/views/payment-history", {
            accountNo,
            role: session.role,
            callerId: parseInt(session.userId)
        });
    }

    if (!result.ok) {
        rowsContainer.innerHTML = `<p>${result.data.message || "Could not fetch payment history."}</p>`;
        return;
    }

    const payments = result.data;

    if (!payments || payments.length === 0) {
        rowsContainer.innerHTML = `<p>No payment history found.</p>`;
        return;
    }

    rowsContainer.innerHTML = payments.map(p => `
        <div class="payment-table-row">
            <span>${p.billId}</span>
            <span>${formatDate(p.date)}</span>
            <span>${formatCurrency(p.amount)}</span>
        </div>
    `).join("");
}

// **  VIEW USAGE **
async function loadUsage() {
    const session = requireSession();
    if (!session) return;

    const accountNo = session.accountNo;
    if (!accountNo) {
        window.location.href = getHomePage();
        return;
    }

    setText("acc-no-display", accountNo);

    const result = await apiPost("/views/usage", {
        accountNo,
        role: session.role,
        callerId: parseInt(session.userId)
    });

    const rowsContainer = document.getElementById("usage-rows");

    if (!result.ok) {
        rowsContainer.innerHTML = `<p>${result.data.message || "Could not fetch usage details."}</p>`;
        return;
    }

    const readings = result.data.readings;

    if (readings.length === 0) {
        rowsContainer.innerHTML = `<p>No readings found.</p>`;
        return;
    }

    const mostRecentDate = new Date(Math.max(...readings.map(r => new Date(r.date))));
    const cutoff = new Date(mostRecentDate);
    cutoff.setDate(cutoff.getDate() - 30);

    const recent = readings.filter(r => new Date(r.date) >= cutoff);

    if (recent.length === 0) {
        rowsContainer.innerHTML = `<p>No readings in the last 30 days.</p>`;
        return;
    }

    rowsContainer.innerHTML = recent.map(r => `
        <div class="usage-table-row">
            <span>${formatDate(r.date)}</span>
            <span>${r.value}</span>
        </div>
    `).join("");
}

// ** GENERATE BILL - STAFF **
let generateBillAccountNo = null;

async function initGenerateBill() {
    const session = requireSession();
    if (!session) return;

    const genBtn = document.querySelector(".generate-bill-card .primary-btn");
    genBtn.disabled = true;

    // avoid reuse of previous accountNo
    generateBillAccountNo = prompt("Enter account number to generate a bill for:");

    if (!generateBillAccountNo || !generateBillAccountNo.trim()) {
        window.location.href = "dashboard.html";
        return;
    }

    generateBillAccountNo = generateBillAccountNo.trim();

    // clear previous values and load new ones
    setText("accNo", generateBillAccountNo);
    setText("unitsUsed", "Loading...");
    document.getElementById("dueDate").value = "";
    document.getElementById("totalAmount").value = "";

    const result = await apiGet(`/views/staff-bill-prep/${encodeURIComponent(generateBillAccountNo)}`);

    if (!result.ok) {
        setText("unitsUsed", "Account not found.");
        return; // genBtn stays disabled
    }

    const prep = result.data;
    const readings = prep.readings;

    if (readings.length < 2) {
        setText("unitsUsed", "Not enough reading history.");
        return;
    }

    // obtain most-recent reading minus oldest reading
    const mostRecent = readings[0];
    const earliest = readings[readings.length - 1];
    const unitsUsed = (mostRecent.value - earliest.value).toFixed(0);
    setText("unitsUsed", unitsUsed);

    genBtn.disabled = false;
    genBtn.replaceWith(genBtn.cloneNode(true));
    document.querySelector(".generate-bill-card .primary-btn")
        .addEventListener("click", submitGenerateBill);
}

async function submitGenerateBill() {
    const session = requireSession();
    if (!session) return;

    const dueDate = requireValue(document.getElementById("dueDate").value);
    const totalAmount = requireValue(document.getElementById("totalAmount").value);

    if (!dueDate || !totalAmount) {
        alert("Please enter both a due date and total amount.");
        return;
    }

    const amountNum = parseFloat(totalAmount);
    if (Number.isNaN(amountNum) || amountNum <= 0) {
        alert("Total amount must be a positive number.");
        return;
    }

    const dueDateObj = new Date(dueDate);
    if (Number.isNaN(dueDateObj.getTime())) {
        alert("Please enter a valid due date (e.g. YYYY-MM-DD).");
        return;
    }

    const result = await apiPost("/bills/generate", {
        accountNo: generateBillAccountNo,
        totalPayable: amountNum,
        dueDate: dueDateObj.toISOString(),
        staffId: parseInt(session.userId)
    });

    if (!result.ok) {
        alert(result.data.message || "Could not generate bill. The account may already have an unpaid bill.");
        return;
    }

    alert(`Bill #${result.data.billId} generated successfully.`);
    window.location.href = "dashboard.html";
}

// ** WATER CONNECTIONS LIST **
let waterAccountsPage = 1;

async function loadWaterAccounts(page) {
    waterAccountsPage = page;

    const result = await apiGet(`/staff/water-connections?pageNumber=${page}&pageSize=10`);

    const rowsContainer = document.getElementById("list-rows");

    if (!result.ok) {
        document.getElementById("list-message").textContent = "Could not load water accounts.";
        return;
    }

    const pageData = result.data;

    if (pageData.items.length === 0) {
        document.getElementById("list-message").textContent = "No water accounts found.";
        rowsContainer.innerHTML = "";
    } else {
        document.getElementById("list-message").textContent = "";
        rowsContainer.innerHTML = pageData.items.map(item => `
            <div class="list-table-row cols-water" data-account="${item.accountNo}">
                <span>${item.accountNo}</span>
                <span>${statusBadge(item.status)}</span>
                <span>${item.customerName}</span>
            </div>
        `).join("");

        rowsContainer.querySelectorAll(".list-table-row").forEach(row => {
            row.addEventListener("click", () => {
                setSelectedAccount(row.dataset.account);
                window.location.href = "view-acc-details.html";
            });
        });
    }

    document.getElementById("pageInfo").textContent = `Page ${pageData.pageNumber} of ${pageData.totalPages || 1}`;
    document.getElementById("prevPageBtn").disabled = pageData.pageNumber <= 1;
    document.getElementById("nextPageBtn").disabled = pageData.pageNumber >= pageData.totalPages;
}

document.getElementById("prevPageBtn")?.addEventListener("click", () => {
    if (document.body.dataset.list === "water-accounts" && waterAccountsPage > 1) {
        loadWaterAccounts(waterAccountsPage - 1);
    }
});

document.getElementById("nextPageBtn")?.addEventListener("click", () => {
    if (document.body.dataset.list === "water-accounts") {
        loadWaterAccounts(waterAccountsPage + 1);
    }
});

// ** CUSTOMERS LIST **
let customersPage = 1;

async function loadCustomers(page) {
    customersPage = page;

    const result = await apiGet(`/staff/customers?pageNumber=${page}&pageSize=10`);

    const rowsContainer = document.getElementById("list-rows");

    if (!result.ok) {
        document.getElementById("list-message").textContent = "Could not load customers.";
        return;
    }

    const pageData = result.data;

    if (pageData.items.length === 0) {
        document.getElementById("list-message").textContent = "No customers found.";
        rowsContainer.innerHTML = "";
    } else {
        document.getElementById("list-message").textContent = "";
        rowsContainer.innerHTML = pageData.items.map(item => `
            <div class="list-table-row cols-customer">
                <span>${item.fullName}</span>
                <span>${item.phoneNo}</span>
            </div>
        `).join("");
    }

    document.getElementById("pageInfo").textContent = `Page ${pageData.pageNumber} of ${pageData.totalPages || 1}`;
    document.getElementById("prevPageBtn").disabled = pageData.pageNumber <= 1;
    document.getElementById("nextPageBtn").disabled = pageData.pageNumber >= pageData.totalPages;
}

document.getElementById("prevPageBtn")?.addEventListener("click", () => {
    if (document.body.dataset.list === "customers" && customersPage > 1) {
        loadCustomers(customersPage - 1);
    }
});

document.getElementById("nextPageBtn")?.addEventListener("click", () => {
    if (document.body.dataset.list === "customers") {
        loadCustomers(customersPage + 1);
    }
});

// ** PARTNERS LIST **
let partnersPage = 1;

async function loadPartners(page) {
    partnersPage = page;

    const result = await apiGet(`/staff/partners?pageNumber=${page}&pageSize=10`);

    const rowsContainer = document.getElementById("list-rows");

    if (!result.ok) {
        document.getElementById("list-message").textContent = "Could not load partners.";
        return;
    }

    const pageData = result.data;

    if (pageData.items.length === 0) {
        document.getElementById("list-message").textContent = "No partners found.";
        rowsContainer.innerHTML = "";
    } else {
        document.getElementById("list-message").textContent = "";
        rowsContainer.innerHTML = pageData.items.map(item => `
            <div class="list-table-row cols-partner" data-id="${item.partnerId}">
                <span>${item.name}</span>
                <span>${item.email}</span>
                <span>
                    <select class="partner-status-select" data-id="${item.partnerId}">
                        <option value="Activated" ${item.status === "Activated" ? "selected" : ""}>Activated</option>
                        <option value="Deactivated" ${item.status === "Deactivated" ? "selected" : ""}>Deactivated</option>
                    </select>
                </span>
            </div>
        `).join("");

        rowsContainer.querySelectorAll(".partner-status-select").forEach(select => {
            select.addEventListener("click", (e) => e.stopPropagation());
            select.addEventListener("change", async (e) => {
                const partnerId = parseInt(e.target.dataset.id);
                const newStatus = e.target.value;

                const result = await apiPost("/staff/partners/update-status", {
                    partnerId,
                    status: newStatus
                });

                if (!result.ok) {
                    alert(result.data.message || "Could not update partner status.");
                    loadPartners(partnersPage); // reload to revert the dropdown
                }
            });
        });
    }

    document.getElementById("pageInfo").textContent = `Page ${pageData.pageNumber} of ${pageData.totalPages || 1}`;
    document.getElementById("prevPageBtn").disabled = pageData.pageNumber <= 1;
    document.getElementById("nextPageBtn").disabled = pageData.pageNumber >= pageData.totalPages;
}

document.getElementById("prevPageBtn")?.addEventListener("click", () => {
    if (document.body.dataset.list === "partners" && partnersPage > 1) {
        loadPartners(partnersPage - 1);
    }
});

document.getElementById("nextPageBtn")?.addEventListener("click", () => {
    if (document.body.dataset.list === "partners") {
        loadPartners(partnersPage + 1);
    }
});

document.getElementById("addPartnerLinkBtn")?.addEventListener("click", () => {
    window.location.href = "add-partner.html";
});

// ** PAY BILL **
let currentBillForPayment = null;
let paymentIdempotencyKey = null;

async function initPayBill() {
    const session = requireSession();
    if (!session) return;

    const payBtn = document.getElementById("payBtn");
    payBtn.disabled = true; 

    const role = (session.role || "").toLowerCase();
    if (role !== "customer" && role !== "partner") {
        window.location.href = getHomePage();
        return;
    }

    const accountNo = session.accountNo;
    if (!accountNo) {
        window.location.href = getHomePage();
        return;
    }

    setText("acc-no-display", accountNo);
    paymentIdempotencyKey = generateIdempotencyKey();

    const result = await apiPost("/views/bill", {
        accountNo,
        role: session.role,
        callerId: parseInt(session.userId)
    });

    if (!result.ok) {
        alert(result.data.message || "Could not fetch bill.");
        window.location.href = getHomePage();
        return;
    }

    currentBillForPayment = result.data;

    setText("bill-id-display", currentBillForPayment.billId);
    setText("units-used-display", currentBillForPayment.unitsUsed);
    setText("total-payable-display", formatCurrency(currentBillForPayment.totalPayable));

    if (currentBillForPayment.status === "Paid") {
        payBtn.textContent = "Already Paid";
        payBtn.disabled = true;
        return;
    }

    payBtn.disabled = false;
    payBtn.addEventListener("click", submitPayment);

    document.getElementById("backBtn")?.addEventListener("click", () => {
        window.location.href = "view-bill.html";
    });
}

async function submitPayment() {
    const session = requireSession();
    if (!session || !currentBillForPayment) return;

    const cardNumber = requireValue(document.getElementById("cardNumber").value);
    const cardHolder = requireValue(document.getElementById("cardHolder").value);
    const expiryDate = requireValue(document.getElementById("expiryDate").value);
    const cvv = requireValue(document.getElementById("cvv").value);

    if (!cardNumber || !cardHolder || !expiryDate || !cvv) {
        alert("Please fill in all card details.");
        return;
    }

    if (cvv.length !== 3) {
        alert("CVV must be 3 digits.");
        return;
    }

    const payBtn = document.getElementById("payBtn");
    payBtn.disabled = true;
    payBtn.textContent = "Processing...";

    const result = await apiPost("/payments/pay", {
        billId: currentBillForPayment.billId,
        amount: currentBillForPayment.totalPayable,
        idempotencyKey: paymentIdempotencyKey,
        paymentMethod: "Card",
        role: session.role,
        callerId: parseInt(session.userId)
    });

    if (!result.ok) {
        alert(result.data.message || "Payment failed. Please try again.");
        payBtn.disabled = false;
        payBtn.textContent = "Pay";
        return;
    }

    const payment = result.data;

    if (payment.status === "Failed") {
        alert("Payment was declined. Please try again.");
        payBtn.disabled = false;
        payBtn.textContent = "Pay";
        return;
    }

    alert(`Payment successful.\nReceipt ID: ${payment.receiptId}\nReference: ${payment.pgRef}`);
    window.location.href = getHomePage();
}
