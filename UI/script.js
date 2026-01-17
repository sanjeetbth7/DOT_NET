
// ---- Config ----
const USERS_KEY = "bank_users";
const SESSION_KEY = "bank_session";
const ACCOUNT_NO_SET_KEY = "generated_account_numbers_v1"; // version your storage for future migrations
const MINIMUM_BALANCE = 500;

// ---- DOM helpers ----
const $ = (sel) => document.querySelector(sel);
const addHidden = (el) => el && el.classList.add("hidden");
const removeHidden = (el) => el && el.classList.remove("hidden");

const registerCard = $("#registerCard");
const loginCard = $("#loginCard");
const dashboard = $("#dashboard");

// Top action buttons
const btnShowRegister = $("#showRegisterBtn");
const btnShowLogin = $("#showLoginBtn");
const btnShowLogout = $("#showLogoutBtn");
const btnLogout = $("#logoutBtn");

// Forms
const registerForm = $("#registerForm");
const loginForm = $("#loginForm");

// ---- Storage helpers ----
const safeParse = (raw, fallback) => {
  try {
    return JSON.parse(raw ?? ""); // will throw for undefined/empty
  } catch {
    return fallback;
  }
};

const getUsers = () => safeParse(localStorage.getItem(USERS_KEY), []);
const saveUsers = (users) => localStorage.setItem(USERS_KEY, JSON.stringify(Array.isArray(users) ? users : []));

const getSession = () => safeParse(localStorage.getItem(SESSION_KEY), null);
const saveSession = (accNo) => localStorage.setItem(SESSION_KEY, JSON.stringify({ accountNo: accNo }));
const clearSession = () => localStorage.removeItem(SESSION_KEY);

// ---- Persisted set of generated account numbers ----
const loadGeneratedAccountNumbers = () => {
  const raw = safeParse(localStorage.getItem(ACCOUNT_NO_SET_KEY), []);
  const arr = Array.isArray(raw) ? raw : [];
  return new Set(arr.map(String)); // normalize to string
};

const persistGeneratedAccountNumbers = (set) => {
  localStorage.setItem(ACCOUNT_NO_SET_KEY, JSON.stringify(Array.from(set)));
};

// Seed the set from storage and existing users (so previously registered accounts stay unique)
const generatedAccountNumbers = (() => {
  const set = loadGeneratedAccountNumbers();
  const existingUsers = getUsers();
  for (const u of existingUsers) {
    if (u?.accountNo) set.add(String(u.accountNo));
  }
  persistGeneratedAccountNumbers(set);
  return set;
})();

// ---- Account number generation ----
// Unique 6-digit number (100000–999999), persisted across sessions.
// Uses crypto.getRandomValues for stronger randomness than Math.random.
// Includes a max-attempt guard to avoid infinite loops if the space gets saturated.
const accountNumber = () => {
  const MAX_ATTEMPTS = 5000;
  for (let attempts = 0; attempts < MAX_ATTEMPTS; attempts++) {
    const buf = new Uint32Array(1);
    crypto.getRandomValues(buf);
    const num = 100000 + (buf[0] % 900000);
    const candidate = String(num);

    if (!generatedAccountNumbers.has(candidate)) {
      generatedAccountNumbers.add(candidate);
      persistGeneratedAccountNumbers(generatedAccountNumbers);
      return candidate;
    }
  }
  throw new Error("Unable to generate a unique account number. Please try again or expand the ID space.");
};

// --- check password condition ---
function checkPasswordCondition(pwd) {
  const hasUpper   = /[A-Z]/.test(pwd);
  const hasLower   = /[a-z]/.test(pwd);
  const hasSpecial = /[^A-Za-z0-9]/.test(pwd);
  const hasLength  = pwd.length >= 8;

  // Require ALL conditions
  return hasUpper && hasLower && hasSpecial && hasLength;
}


// ---- Lookup ----
const findUser = (accNo) => getUsers().find((u) => String(u.accountNo) === String(accNo));

// ---- View management ----
function showView(view) {
  switch (view) {
    case "register":
      removeHidden(registerCard);
      addHidden(loginCard);
      addHidden(dashboard);
      addHidden(btnShowLogout);
      break;
    case "login":
      addHidden(registerCard);
      removeHidden(loginCard);
      addHidden(dashboard);
      addHidden(btnShowLogout);
      break;
    case "dashboard":
      removeHidden(dashboard);
      removeHidden(btnShowLogout);
      addHidden(registerCard);
      addHidden(loginCard);
      break;
    default:
      showView("login");
  }
}

// ---- Initial state ----
if (getSession()) {
  showDashboard();
} else {
  showView("login");
}

// ---- Top buttons handlers ----
const doLogout = () => {
  clearSession();
  showView("login");
  alert("Logged out.");
};

btnShowRegister?.addEventListener("click", () => {
  loginForm?.reset();
  showView("register");
});

btnShowLogin?.addEventListener("click", () => {
  registerForm?.reset();
  showView("login");
});

btnShowLogout?.addEventListener("click", doLogout);
btnLogout?.addEventListener("click", doLogout);

// ---- Registration ----
document.getElementById("registerForm").addEventListener("submit", function (e) {
  e.preventDefault();

  const aadhaar = aadhaarEl().value.trim();
  const firstName = firstNameEl().value.trim();
  const lastName = lastNameEl().value.trim();
  const email = emailEl().value.trim();
  const password = passwordEl().value;
  const confirmPassword = confirmPasswordEl().value;
  const address = addressEl().value.trim();
  const contact = contactEl().value.trim();

  if (password !== confirmPassword) {
    alert("Password and Confirm Password should be same.");
    return;
  }

  if (!/^\d{12}$/.test(aadhaar)) {
    alert("Please enter a valid 12-digit Aadhaar number.");
    return;
  }
  if (!/^\d{10}$/.test(contact)) {
    alert("Please enter a valid 10-digit contact number.");
    return;
  }


if (!checkPasswordCondition(passwordEl().value)) {
  alert("Password must contain at least one: uppercase, lowercase, number, or special character.");
  return;
}


  const users = getUsers();

  // --- Prevent duplicate Aadhaar / email (recommended in banking flows) ---
  if (users.some(u => String(u.aadhaar) === aadhaar)) {
    alert("An account with this Aadhaar already exists.");
    return;
  }
  if (users.some(u => String(u.email).toLowerCase() === email.toLowerCase())) {
    alert("An account with this email already exists.");
    return;
  }

  const newUser = {
    accountNo: accountNumber(),
    aadhaar,
    firstName,
    lastName,
    email,
    password,
    address,
    contact,
    balance: 0
  };

  users.push(newUser);
  saveUsers(users);

  alert("Customer Registration successful.\nAccount Number: " + newUser.accountNo);
  this.reset();
  showView("login");
});

// ---- Login ----
document.getElementById("loginForm").addEventListener("submit", function (e) {
  e.preventDefault();

  const accNo = document.getElementById("loginAccNo").value.trim();
  const pass = document.getElementById("loginPassword").value;

  const user = findUser(accNo);

  if (!user) {
    alert("Account not found.");
    return;
  }

  if (user.password !== pass) {
    alert("Invalid Password.");
    return;
  }

  saveSession(user.accountNo);
  alert("Customer login successful.");
  showDashboard();
  this.reset();
});

// ---- Dashboard population ----
function showDashboard() {
  const session = getSession();
  if (!session) return;

  const user = findUser(session.accountNo);
  if (!user) return;

  showView("dashboard");

  document.getElementById("dashAcc").innerText = user.accountNo;
  document.getElementById("dashName").innerText = user.firstName + " " + user.lastName;
  document.getElementById("dashEmail").innerText = user.email;
  document.getElementById("dashBalance").innerText = user.balance;

  document.getElementById("updAddress").value = user.address;
  document.getElementById("updContact").value = user.contact;
}

// ---- Persist updates ----
function updateDashboardUser(user) {
  const users = getUsers();
  const idx = users.findIndex(u => String(u.accountNo) === String(user.accountNo));
  if (idx >= 0) {
    users[idx] = user;
    saveUsers(users);
  }
  showDashboard();
}

// ---- Update contact/address ----
document.getElementById("updateForm").addEventListener("submit", function (e) {
  e.preventDefault();

  const session = getSession();
  const user = findUser(session.accountNo);

  user.address = document.getElementById("updAddress").value.trim();
  user.contact = document.getElementById("updContact").value.trim();

  updateDashboardUser(user);
  alert("Customer update successful.");
});

// ---- Deposit ----
document.getElementById("depositForm").addEventListener("submit", function (e) {
  e.preventDefault();

  const session = getSession();
  const user = findUser(session.accountNo);
  const amount = Number(document.getElementById("depositAmount").value);

  if (Number.isNaN(amount) || amount <= 0) {
    alert("Please enter a valid deposit amount.");
    return;
  }

  user.balance += amount;
  updateDashboardUser(user);
  alert(`Deposit successful.\nYour current balance is ${user.balance}`);
  this.reset();
});

// ---- Withdraw ----
document.getElementById("withdrawForm").addEventListener("submit", function (e) {
  e.preventDefault();

  const session = getSession();
  const user = findUser(session.accountNo);

  const amount = Number(document.getElementById("withdrawAmount").value);
  const remainingBalance = user.balance - amount;

  if (Number.isNaN(amount) || amount <= 0) {
    alert("Please enter a valid withdraw amount.");
    return;
  }

  if (remainingBalance < MINIMUM_BALANCE) {
    alert("Insufficient Balance.");
    return;
  }

  user.balance -= amount;
  updateDashboardUser(user);

  alert(`Withdraw successful.\nYour remaining balance is ${user.balance}`);
  this.reset();
});

// ---- Field getters ----
/*
used field getters
Always gets the latest element: If the DOM is dynamically updated (e.g., the element is replaced via JS), calling the function always fetches the current element.

Lazy evaluation: The element is only looked up when needed, not when the script loads. This avoids errors if the script runs before the DOM is ready.
*/
function aadhaarEl(){ return document.getElementById("aadhaar"); }
function firstNameEl(){ return document.getElementById("firstName"); }
function lastNameEl(){ return document.getElementById("lastName"); }
function emailEl(){ return document.getElementById("email"); }
function passwordEl(){ return document.getElementById("password"); }
function confirmPasswordEl(){ return document.getElementById("confirmPassword"); }
function addressEl(){ return document.getElementById("address"); }
function contactEl(){ return document.getElementById("contact"); }