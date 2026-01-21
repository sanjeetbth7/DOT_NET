const USERS_KEY = "bank_users";
const SESSION_KEY = "bank_session";
const MINIMUM_BALANCE = 500;

const showRegisterBtn = document.querySelector("#showRegisterBtn");
const showLoginBtn = document.querySelector("#showLoginBtn");
const logoutBtn = document.querySelector("#logoutBtn");

const addHidden = (el) => el && el.classList.add("hidden");
const removeHidden = (el) => el && el.classList.remove("hidden");

const safeParse = (raw, fallback) => {
  try { return JSON.parse(raw) || fallback; } 
  catch { return fallback; }
};

const getUsers = () => safeParse(localStorage.getItem(USERS_KEY), []);
const saveUsers = (users) => localStorage.setItem(USERS_KEY, JSON.stringify(users));
const getSession = () => safeParse(localStorage.getItem(SESSION_KEY), null);
const saveSession = (accNo) => localStorage.setItem(SESSION_KEY, JSON.stringify({ accountNo: accNo }));
const clearSession = () => localStorage.removeItem(SESSION_KEY);



const generateAccountNumber = () => {
    const users = getUsers();
    let isUnique = false;
    let newAcc;
    while(!isUnique) {
        newAcc = String(Math.floor(100000 + Math.random() * 900000));
        isUnique = !users.some(u => u.accountNo === newAcc);
    }
    return newAcc;
};

function showView(view) {
  const views = {
    register: document.querySelector("#registerCard"),
    login: document.querySelector("#loginCard"),
    dashboard: document.querySelector("#dashboard")
  };
  
  Object.values(views).forEach(v => addHidden(v));

  if (views[view]) removeHidden(views[view]);
  else views["login"] && removeHidden(views["login"]);

  if (view === "dashboard") {
    removeHidden(logoutBtn);
    addHidden(showRegisterBtn);
    addHidden(showLoginBtn);
  } else {
    addHidden(logoutBtn);
    removeHidden(showRegisterBtn);
    removeHidden(showLoginBtn);
  }
}

const validateContact = (num) => /^[6-9]\d{9}$/.test(num); // It return true if 10 digit and starts with 6-9 else false
const validateAdhar = (num) => /^[1-9]\d{11}$/.test(num); // It return true if 12 digit and starts with 1-9 else false
const checkPassword = (pwd) => /[A-Z]/.test(pwd) && /[a-z]/.test(pwd) && /[0-9]/.test(pwd) && pwd.length >= 8; 

function updateDashboardUser(updatedUser) {
  const users = getUsers();
  const index = users.findIndex(u => u.accountNo === updatedUser.accountNo);
  if (index >=0) {
    users[index] = updatedUser;
    saveUsers(users);
    renderDashboard();
  }else {
    alert("Error: User not found.");
    window.location.href = "/"; // Redirect to home/login
  }
}

function renderDashboard() {
  const session = getSession();
  if (!session) return showView("login");

  const users = getUsers();
  const user = users.find(u => u.accountNo === session.accountNo);
  if (!user) return doLogout();

  showView("dashboard");
  document.querySelector("#dashAcc").innerText = user.accountNo;
  document.querySelector("#dashName").innerText = `${user.firstName} ${user.lastName}`;
  document.querySelector("#dashEmail").innerText = user.email;
  document.querySelector("#dashBalance").innerText = user.balance;
  document.querySelector("#updAddress").value = user.address;
  document.querySelector("#updContact").value = user.contact;
}

document.querySelector("#registerForm").addEventListener("submit", function(e) {
  e.preventDefault();
  const users = getUsers();
  const email = document.querySelector("#email").value.trim();
  const pass = document.querySelector("#password").value;
  const aadhaar = document.querySelector("#aadhaar").value.trim();
  const contact = document.querySelector("#contact").value.trim();

  if (!validateAdhar(aadhaar)) return alert("Invalid Aadhaar: Must be a 12-digit number starting with 1-9.");
  if (!validateContact(contact)) return alert("Invalid Contact: Must be a 10-digit number starting with 6-9.");

  if (pass !== document.querySelector("#confirmPassword").value) return alert("Registration Failed: Passwords do not match.");
  if (!checkPassword(pass)) return alert("Weak Password: Must be 8+ chars with Uppercase, Lowercase, and a Number.");
  if (users.some(u => u.email === email)) return alert("Registration Failed: Email already exists.");

  const newUser = {
    accountNo: generateAccountNumber(),
    aadhaar: document.querySelector("#aadhaar").value,
    firstName: document.querySelector("#firstName").value,
    lastName: document.querySelector("#lastName").value,
    email: email,
    password: pass,
    address: document.querySelector("#address").value,
    contact: document.querySelector("#contact").value,
    balance: 0
  };

  users.push(newUser);
  saveUsers(users);
  alert(`Registration Successful!\nWelcome ${newUser.firstName}!\nYour Account Number is: ${newUser.accountNo}`);
  this.reset();
  showView("login");
});

document.querySelector("#loginForm").addEventListener("submit", function(e) {
  e.preventDefault();
  const acc = document.querySelector("#loginAccNo").value;
  const pass = document.querySelector("#loginPassword").value;
  const user = getUsers().find(u => u.accountNo === acc && u.password === pass);

  if (user) {
    saveSession(user.accountNo);
    alert(`Login Successful!\nWelcome back, ${user.firstName}.`);
    renderDashboard();
  } else {
    alert("Login Failed: Invalid Account Number or Password.");
  }
});

document.querySelector("#updateAddressForm").addEventListener("submit", function(e) {
  e.preventDefault();
  const session = getSession();
  const users = getUsers();
  const user = users.find(u => u.accountNo === session.accountNo);
  user.address = document.querySelector("#updAddress").value;
  updateDashboardUser(user);
  alert("Success: Address has been updated.");
});

document.querySelector("#updateContactForm").addEventListener("submit", function(e) {
  e.preventDefault();
  const session = getSession();
  const users = getUsers();
  const user = users.find(u => u.accountNo === session.accountNo);
  const newContact = document.querySelector("#updContact").value;
  if(validateContact(newContact)) {
      user.contact = newContact;
      updateDashboardUser(user);
      alert("Success: Contact number has been updated.");
  } else {
      alert("Update Failed: Please enter a valid 10-digit contact number.");
  }
});

document.querySelector("#depositForm").addEventListener("submit", function(e) {
  e.preventDefault();
  const amt = Number(document.querySelector("#depositAmount").value);
  if (isNaN(amt) || amt <= 0) return alert("Invalid amount.");

  const session = getSession();
  const users = getUsers();
  const user = users.find(u => u.accountNo === session.accountNo);
  
  user.balance += amt;
  updateDashboardUser(user);
  alert(`Deposit Successful!\nAmount: ₹${amt}\nNew Balance: ₹${user.balance}`);
  this.reset();
});

document.querySelector("#withdrawForm").addEventListener("submit", function(e) {
  e.preventDefault();
  const amt = Number(document.querySelector("#withdrawAmount").value);
  if (isNaN(amt) || amt <= 0) return alert("Invalid amount.");

  const session = getSession();
  const users = getUsers();
  const user = users.find(u => u.accountNo === session.accountNo);
  
  if (user.balance - amt < MINIMUM_BALANCE) {
      return alert(`Withdrawal Failed!\nInsufficient funds to maintain minimum balance of ₹${MINIMUM_BALANCE}.\nAvailable: ₹${user.balance}`);
  }
  
  user.balance -= amt;
  updateDashboardUser(user);
  alert(`Withdrawal Successful!\nAmount: ₹${amt}\nRemaining Balance: ₹${user.balance}`);
  this.reset();
});

const doLogout = () => { 
    clearSession(); 
    showView("login"); 
    alert("You have been logged out.");
};

document.querySelector("#logoutBtn").addEventListener("click", doLogout);
document.querySelector("#showRegisterBtn").addEventListener("click", () => showView("register"));
document.querySelector("#showLoginBtn").addEventListener("click", () => showView("login"));

if (getSession()) renderDashboard();
else showView("login");