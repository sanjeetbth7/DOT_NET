
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

// Constants for validation and defaults
// used static class to group related constants as static class neither be instantiated, nor inherited.
static class Constants
{
    public const string adminID = "admin";
    public const string adminPwd = "Admin@123";
    public const int minBalance = 500;
    public const int minPasswordLength = 8;
    public const int aadharLength = 12;
    public const int panLength = 10;
    public const int contactLength = 10;
    public const int minNameLength = 3;
    public const int maxNameLength = 50;
    public const int minAddressLength = 10;
    public const int maxAddressLength = 100;
}

class Customer
{
    public int AccountNumber;
    public string Aadhar;
    public string Name;
    public string Email;
    public string Address;
    public string Contact;
    public string Pan;
    private string Password;
    private double Balance;

    public Customer(string aadhar, string name, string email, string address, string contact, string pan, int accountNumber, string password, double initialDeposit)
    {
        this.Aadhar = aadhar;
        this.Name = name;
        this.Email = email;
        this.Address = address;
        this.Contact = contact;
        this.Pan = pan;
        this.AccountNumber = accountNumber;
        this.Password = password;
        this.Balance = initialDeposit;
    }

    public bool checkPassword(string pwd)
    {
        return this.Password == pwd;
    }

    public double GetBalance()
    {
        return this.Balance;
    }

    public void Deposit(double amount)
    {
        if (amount <= 0)
            Console.WriteLine("Invalid Amount");
        else
        {
            this.Balance += amount;
            Console.WriteLine("Deposit Successful. Updated Balance: " + this.Balance);
        }

    }

    public void Withdraw(double amount)
    {
        if (amount <= 0)
            Console.WriteLine("Invalid Amount");
        else if (this.Balance - amount < Constants.minBalance)
            Console.WriteLine("Insufficient Balance. Minimum balance must be maintained: " + Constants.minBalance);
        else
        {
            this.Balance -= amount;
            Console.WriteLine("Withdrawal Successful. Updated Balance: " + this.Balance);
        }
    }


}

class Program
{
    /* Here I have not used dictionary or map because we need to search by multiple fields{Aadhar, PAN, AccountNumber} so that these data not be duplicated. If we use 3 separate maps or dictionary, we would have to maintain synchronization between them and we need more space. */
    static List<Customer> customers = new List<Customer>();
    static Random rnd = new Random();
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

        if (id == Constants.adminID && pwd == Constants.adminPwd)
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
            else break;
        }
    }

    static bool IsValidEmail(string email)
    {
        string pattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
        return Regex.IsMatch(email, pattern);
    }

    // Aadhar must be a 12-digit number.
    static bool IsValidAadhar(string aadhar)
    {
        string pattern = $@"^\d{{{Constants.aadharLength}}}$";
        return Regex.IsMatch(aadhar, pattern);
    }
    static bool IsValidPassword(string password)
    {
        string pattern = $@"^(?=.*[a-z])(?=.*[A-Z])(?=.*[\W]).{{{Constants.minPasswordLength},}}$";
        return Regex.IsMatch(password, pattern);
    }

    // PAN must be a 10-character alphanumeric string, [5 characters +4 digits + 1 character].
    static bool IsValidPAN(string pan)
    {
        if (pan.Length != Constants.panLength) return false;
        string pattern = @"^[A-Z]{5}\d{4}[A-Z]$";
        return Regex.IsMatch(pan, pattern);
    }

    // Contact must be a 10-digit number and start with 6,7,8 or 9.
    static bool IsValidContact(string contact)
    {
        string pattern = $@"^[6-9]\d{{{Constants.contactLength - 1}}}$";
        return Regex.IsMatch(contact, pattern);
    }

    static bool IsValidName(string name)
    {
        if (string.IsNullOrWhiteSpace(name)) return false;
        if (name.Length < Constants.minNameLength || name.Length > Constants.maxNameLength) return false;
        // Name must contain only letters and spaces.
        string pattern = @"^[a-zA-Z\s]+$";
        return Regex.IsMatch(name, pattern);
    }

    static bool IsValidAddress(string address)
    {
        if (string.IsNullOrWhiteSpace(address)) return false;
        if (address.Length < Constants.minAddressLength || address.Length > Constants.maxAddressLength) return false;
        return true;
    }

    static void CreateAccount()
    {
        try
        {
            Console.Write("Aadhar: ");
            string aadhar = Console.ReadLine();

            Console.Write("Name: ");
            string name = Console.ReadLine();

            Console.Write("Email: ");
            string email = Console.ReadLine();

            Console.Write("Address: ");
            string address = Console.ReadLine();

            Console.Write("Contact: ");
            string contact = Console.ReadLine();

            Console.Write("PAN: ");
            string pan = Console.ReadLine();

            Console.Write("Password: ");
            string p1 = Console.ReadLine();
            Console.Write("Confirm Password: ");
            string p2 = Console.ReadLine();

            Console.Write("Initial Deposit: ");
            double initialDeposit = Convert.ToDouble(Console.ReadLine());

            if (p1 != p2)
            {
                Console.WriteLine("Passwords do not match");
                return;
            }

            if (!IsValidPassword(p1))
            {
                Console.WriteLine("Password must be at least {0} characters long and include uppercase, lowercase, and special character.", Constants.minPasswordLength);
                return;
            }

            if (initialDeposit < Constants.minBalance)
            {
                Console.WriteLine("Minimum deposit is {0}", Constants.minBalance);
                return;
            }

            if (!IsValidEmail(email))
            {
                Console.WriteLine("Invalid Email Format.");
                return;
            }

            if (!IsValidAadhar(aadhar))
            {
                Console.WriteLine("Invalid Aadhar Format.\nAadhar should be a {0}-digit number.", Constants.aadharLength);
                return;
            }
            if (!IsValidPAN(pan))
            {
                Console.WriteLine("Invalid PAN Format.");
                return;
            }

            if (!IsValidContact(contact))
            {
                Console.WriteLine("Invalid Contact Format.");
                return;
            }

            if (!IsValidName(name))
            {
                Console.WriteLine("Invalid Name Format.");
                return;
            }

            if (!IsValidAddress(address))
            {
                Console.WriteLine("Invalid Address Format.");
                return;
            }

            if (customers.Exists(x => x.Aadhar == aadhar))
            {
                Console.WriteLine("Account with this Aadhar already exists.");
                return;
            }

            if (customers.Exists(x => x.Pan == pan))
            {
                Console.WriteLine("Account with this PAN already exists.");
                return;
            }

            int accountNumber;
            do
            {
                accountNumber = rnd.Next(1000, 9999);
            } while (customers.Exists(x => x.AccountNumber == accountNumber));

            Customer c = new Customer(
                aadhar,
                name,
                email,
                address,
                contact,
                pan,
                accountNumber,
                p1,
                initialDeposit
            );

            customers.Add(c);

            Console.WriteLine("Account Created Successfully");
            Console.WriteLine("Account Number: " + accountNumber);
        }
        catch (Exception e)
        {
            Console.WriteLine("Error: " + e.Message);
        }
    }

    static void ViewAccount()
    {
        Console.Write("Enter Account Number or ALL: ");
        string input = Console.ReadLine();

        if (input.ToUpper() == "ALL")
        {
            Console.WriteLine("Acc No \t Name \t\t Email \t\t Balance");
            foreach (var c in customers)
                Display(c);
        }
        else
        {
            int acc = Convert.ToInt32(input);
            var c = customers.Find(x => x.AccountNumber == acc);
            if (c != null)
            {
                Console.WriteLine("Acc No \t Name \t\t Email \t\t\t Balance");
                Display(c);
            }
            else Console.WriteLine("Account Not Found");
        }
    }

    static void UpdateAccount()
    {
        try
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

            Console.WriteLine("Leave field blank to keep current value.");
            Console.Write("Name ({0}): ", c.Name);
            string name = Console.ReadLine();
            Console.Write("Email ({0}): ", c.Email);
            string email = Console.ReadLine();
            Console.Write("Address ({0}): ", c.Address);
            string address = Console.ReadLine();
            Console.Write("Contact ({0}): ", c.Contact);
            string contact = Console.ReadLine();

            if (!string.IsNullOrWhiteSpace(name))
            {
                if (!IsValidName(name))
                {
                    Console.WriteLine("Invalid Name Format.\nOnly characters and spaces are allowed.\nName should be between {0} and {1} characters long and contain only letters and spaces.", Constants.minNameLength, Constants.maxNameLength);
                    return;
                }
                c.Name = name;
            }
            if (!string.IsNullOrWhiteSpace(email))
            {
                if (!IsValidEmail(email))
                {
                    Console.WriteLine("Invalid Email Format.");
                    return;
                }
                c.Email = email;
            }
            if (!string.IsNullOrWhiteSpace(address))
            {
                if (!IsValidAddress(address))
                {
                    Console.WriteLine("Invalid Address Format.\nAddress should be between {0} and {1} characters long.", Constants.minAddressLength, Constants.maxAddressLength);
                    return;
                }
                c.Address = address;
            }
            if (!string.IsNullOrWhiteSpace(contact))
            {
                if (!IsValidContact(contact))
                {
                    Console.WriteLine("Invalid Contact Format.\nContact should be a {0}-digit number.", Constants.contactLength);
                    return;
                }
                c.Contact = contact;
            }

            Console.WriteLine("Updated Successfully");
        }
        catch (Exception e)
        {
            Console.WriteLine("Error: " + e.Message);
        }
    }

    static void DeleteAccount()
    {
        try
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
        catch (Exception e)
        {

            Console.WriteLine("Error: " + e.Message);
        }
    }

    static void CustomerLogin()
    {
        try
        {
            Console.Write("Account Number: ");
            int acc = Convert.ToInt32(Console.ReadLine());
            Console.Write("Password: ");
            string pwd = Console.ReadLine();

            var c = customers.Find(x => x.AccountNumber == acc && x.checkPassword(pwd));
            if (c != null)
            {
                Console.WriteLine("Login Successful");
                CustomerMenu(c);
            }
            else Console.WriteLine("Invalid Login");
        }
        catch (Exception e)
        {
            Console.WriteLine("Error: " + e.Message);
        }
    }

    static void CustomerMenu(Customer c)
    {
        while (true)
        {
            Console.WriteLine("\n1.Deposit\n2.Withdraw\n3.Balance\n4.Logout");
            string ch = Console.ReadLine();

            if (ch == "1")
            {
                try
                {
                    Console.Write("Amount: ");
                    double amt = Convert.ToDouble(Console.ReadLine());
                    c.Deposit(amt);
                }
                catch (Exception e)
                {
                    Console.WriteLine("Error: " + e.Message);
                }
            }
            else if (ch == "2")
            {
                try
                {
                    Console.Write("Amount: ");
                    double amt = Convert.ToDouble(Console.ReadLine());
                    c.Withdraw(amt);
                }
                catch (Exception e)
                {
                    Console.WriteLine("Error: " + e.Message);
                }
            }
            else if (ch == "3")
            {
                Console.WriteLine("Current Balance: " + c.GetBalance());
            }
            else break;
        }
    }

    static void Display(Customer c)
    {
        Console.WriteLine($"{c.AccountNumber} \t {c.Name} \t {c.Email} \t {c.GetBalance()}");
    }
}