using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;


class Customer
{
    public int AccountNumber;
    public string Aadhar;
    public string Name;
    public string Email;
    public string Address;
    public string Contact;
    public string Pan;
    public string Password;
    public double Balance;
}

class Program
{
    static List<Customer> customers = new List<Customer>();
    static Random rnd = new Random();
    const int minBalance = 500;
    const string adminID = "admin";
    const string adminPwd = "Admin@123";
    const int maxNameLength = 50;
    const int minNameLength = 2;
    const int maxAddressLength = 100;
    const int minAddressLength = 5;
    const int contactLength = 10;
    const int panLength = 10;
    const int aadharLength = 12;
    const int minPasswordLength = 8;
    static void Main()
    {
        while (true)
        {
            Console.WriteLine("\n1. Admin Login\n2. Customer Login\n3. Exit");
            string choice = Console.ReadLine();

            if (choice == "1") AdminLogin();
            else if (choice == "2") CustomerLogin();
            else break;
        }
    }

    static void AdminLogin()
    {
        Console.Write("User Id: ");
        string id = Console.ReadLine();
        Console.Write("Password: ");
        string pwd = Console.ReadLine();

        if (id == adminID && pwd == adminPwd)
        {
            Console.WriteLine("Login Successful");
            AdminMenu();
        }
        else Console.WriteLine("Invalid Credentials");
    }

    static void AdminMenu()
    {
        while (true)
        {
            Console.WriteLine("\n1.Create Account\n2.View Account\n3.Update Account\n4.Delete Account\n5.Logout");
            string ch = Console.ReadLine();

            if (ch == "1") CreateAccount();
            else if (ch == "2") ViewAccount();
            else if (ch == "3") UpdateAccount();
            else if (ch == "4") DeleteAccount();
            else {
                break;
            }
        }
    }

    static bool IsValidEmail(string email)
    {
        string pattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
        return Regex.IsMatch(email, pattern);
    }

    static bool IsValidAadhar(string aadhar)
    {
        // Aadhar must be a 12-digit number.
        string pattern = @"^\d{12}$";
        return Regex.IsMatch(aadhar, pattern);
    }
    static bool IsValidPassword(string password)
{
    string pattern = $@"^(?=.*[a-z])(?=.*[A-Z])(?=.*[\W]).{{{minPasswordLength},}}$";
    return Regex.IsMatch(password, pattern);
}

    static bool IsValidPAN(string pan)
    {
        if(pan.Length != panLength) return false;
        // PAN must be a 10-character alphanumeric string, [5 characters +4 digits + 1 character].
        string pattern = @"^[A-Z]{5}\d{4}[A-Z]$";
        return Regex.IsMatch(pan, pattern);
    }

    static bool IsValidContact(string contact)
    {
        // Contact must be a 10-digit number.
        string pattern = @"^\d{contactLength}$";
        return Regex.IsMatch(contact, pattern);
    }

    static bool IsValidName(string name)
    {
        if(string.IsNullOrWhiteSpace(name)) return false;
        if(name.Length < minNameLength || name.Length > maxNameLength) return false;
        // Name must contain only letters and spaces.
        string pattern = @"^[a-zA-Z\s]+$";
        return Regex.IsMatch(name, pattern);
    }

    static bool IsValidAddress(string address)
    {
        if(string.IsNullOrWhiteSpace(address)) return false;
        if(address.Length < minAddressLength || address.Length > maxAddressLength) return false;
        return true;
    }

    static void CreateAccount()
    {
        Customer c = new Customer();

        Console.Write("Aadhar: ");
        c.Aadhar = Console.ReadLine();

        Console.Write("Name: ");
        c.Name = Console.ReadLine();

        Console.Write("Email: ");
        c.Email = Console.ReadLine();

        Console.Write("Address: ");
        c.Address = Console.ReadLine();

        Console.Write("Contact: ");
        c.Contact = Console.ReadLine();

        Console.Write("PAN: ");
        c.Pan = Console.ReadLine();

        Console.Write("Initial Deposit: ");
        c.Balance = Convert.ToDouble(Console.ReadLine());
        if (c.Balance < minBalance)
        {
            Console.WriteLine("Minimum deposit is {0}", minBalance);
            return;
        }

        Console.Write("Password: ");
        string p1 = Console.ReadLine();
        Console.Write("Confirm Password: ");
        string p2 = Console.ReadLine();

        if (p1 != p2 ){
            Console.WriteLine("Passwords do not match");
            return;
        }
        if(!IsValidPassword(p1)){
            Console.WriteLine("Password must be at least {0} characters long and include uppercase, lowercase, and special character.", minPasswordLength);
            return;
        }

        c.Password = p1;

        if(!IsValidEmail(c.Email)){
            Console.WriteLine("Invalid Email Format.");
            return;
        }
        if(!IsValidAadhar(c.Aadhar)){
            Console.WriteLine("Invalid Aadhar Format.\nAadhar should be a {0}-digit number.", aadharLength);
            return;
        }
        if(!IsValidPAN(c.Pan)){
            Console.WriteLine("Invalid PAN Format.");
            return;
        }
        if(!IsValidContact(c.Contact)){
            Console.WriteLine("Invalid Contact Format.\nContact should be a {0}-digit number.", contactLength);
            return;
        }
        if(!IsValidName(c.Name)){
            Console.WriteLine("Invalid Name Format.\nOnly characters and spaces are allowed.\nName should be between {0} and {1} characters long and contain only letters and spaces.", minNameLength, maxNameLength);
            return;
        }
        if(!IsValidAddress(c.Address)){
            Console.WriteLine("Invalid Address Format.\nAddress should be between {0} and {1} characters long.", minAddressLength, maxAddressLength);

            return;
        }

        if(customers.Exists(x => x.Aadhar == c.Aadhar)){
            Console.WriteLine("Account with this Aadhar already exists.");
            return;
        }

        if(customers.Exists(x => x.Pan == c.Pan)){
            Console.WriteLine("Account with this PAN already exists.");
            return;
        }

        do
        {
            c.AccountNumber = rnd.Next(1000, 9999);
        } while (customers.Exists(x => x.AccountNumber == c.AccountNumber));


        customers.Add(c);
        Console.WriteLine("Account Created Successfully");
        Console.WriteLine("Account Number: " + c.AccountNumber);
    }

    static void ViewAccount()
    {
        Console.Write("Enter Account Number or ALL: ");
        string input = Console.ReadLine();

        if (input.ToUpper() == "ALL")
        {
            foreach (var c in customers)
                Display(c);
        }
        else
        {
            int acc = Convert.ToInt32(input);
            var c = customers.Find(x => x.AccountNumber == acc);
            if (c != null) Display(c);
            else Console.WriteLine("Account Not Found");
        }
    }

    static void UpdateAccount()
    {
        Console.Write("Account Number: ");
        int acc = Convert.ToInt32(Console.ReadLine());
        var c = customers.Find(x => x.AccountNumber == acc);

        if (c == null)
        {
            Console.WriteLine("Account Not Found");
            return;
        }

        Console.Write("Confirm Update (Y/N): ");
        if (Console.ReadLine().ToUpper() != "Y") return;

        Console.Write("New Email: ");
        c.Email = Console.ReadLine();
        Console.Write("New Contact: ");
        c.Contact = Console.ReadLine();
        Console.Write("New Address: ");
        c.Address = Console.ReadLine();

        Console.WriteLine("Updated Successfully");
    }

    static void DeleteAccount()
    {
        Console.Write("Account Number: ");
        int acc = Convert.ToInt32(Console.ReadLine());
        var c = customers.Find(x => x.AccountNumber == acc);

        if (c == null)
        {
            Console.WriteLine("Account Not Found");
            return;
        }

        Console.Write("Confirm Delete (Y/N): ");
        if (Console.ReadLine().ToUpper() == "Y")
        {
            customers.Remove(c);
            Console.WriteLine("Account Deleted");
        }
    }

    static void CustomerLogin()
    {
        Console.Write("Account Number: ");
        int acc = Convert.ToInt32(Console.ReadLine());
        Console.Write("Password: ");
        string pwd = Console.ReadLine();

        var c = customers.Find(x => x.AccountNumber == acc && x.Password == pwd);
        if (c != null)
        {
            Console.WriteLine("Login Successful");
            CustomerMenu(c);
        }
        else Console.WriteLine("Invalid Login");
    }

    static void CustomerMenu(Customer c)
    {
        while (true)
        {
            Console.WriteLine("\n1.Deposit\n2.Withdraw\n3.Balance\n4.Logout");
            string ch = Console.ReadLine();

            if (ch == "1") Deposit(c);
            else if (ch == "2") Withdraw(c);
            else if (ch == "3") Console.WriteLine("Balance: " + c.Balance);
            else break;
        }
    }

    static void Deposit(Customer c)
    {
        Console.Write("Amount: ");
        double amt = Convert.ToDouble(Console.ReadLine());
        if (amt <= 0)
        {
            Console.WriteLine("Invalid Amount");
            return;
        }
        c.Balance += amt;
        Console.WriteLine("Updated Balance: " + c.Balance);
    }

    static void Withdraw(Customer c)
    {
        Console.Write("Amount: ");
        double amt = Convert.ToDouble(Console.ReadLine());
        double remainingBalance = c.Balance - amt;
        if (remainingBalance < minBalance)
        {
            Console.WriteLine("Minimum balance of {0} must be maintained", minBalance);
            return;
        }
        c.Balance -= amt;
        Console.WriteLine("Updated Balance: " + c.Balance);
    }

    static void Display(Customer c)
    {
        Console.WriteLine("Acc No \t Name \t Email \t Balance");
        Console.WriteLine($"{c.AccountNumber} \t {c.Name} \t {c.Email} \t {c.Balance}");
    }
}