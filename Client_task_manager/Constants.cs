using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client_task_manager
{
    internal class Constants
    {
        static Constants()
        {
            Error = "Error";
            ServerIP = "127.0.0.1";
            //ServerIP = "77.122.231.142";
            EmailPattern = @"\b[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,}\b";
            PasswordPattern = @"\w{8,}";
            Login = "Login";
            Registration = "Registration";
            UserTask = "UserTask";
            CompletedUserTask = "CompletedUserTask";
            GiveUserTask = "GiveUserTask";
            EnterEmail = "Please enter your email!";
            EnterPassword = "Please enter your password!";
            IncorrectEmail = "Incorrect email! You may have made a mistake.";
            IncorrectEmailSignIn = "The user with this email does not exist!";
            IncorrectPassword = "Password must contain at least 8 characters!";
            IncorrectPasswordSignIn = "Wrong password. Try again!";
            Email = "Email";
            Password = "Password";
            RepeatPassword = "Repeat password";
            PasswordNotMatch = "The passwords don't match.";
            MustBeFilled = "The field must be filled in!";
            High = "High";
            Low = "Low";
            Normal = "Normal";
            FirstName = "First name";
            LastName = "Last name";

            Port1024 = 1024;
        }

        public static string Error { get; private set; }

        public static string ServerIP { get; private set; }

        public static string EmailPattern { get; private set; }

        public static string PasswordPattern { get; private set; }

        public static string Login { get; private set; }

        public static string Registration { get; private set; }

        public static string UserTask { get; private set; }

        public static string CompletedUserTask { get; private set; }

        public static string GiveUserTask { get; private set; }

        public static string EnterEmail { get; private set; }

        public static string EnterPassword { get; private set; }

        public static string IncorrectEmail { get; private set; }

        public static string IncorrectEmailSignIn { get; private set; }

        public static string IncorrectPassword { get; private set; }

        public static string IncorrectPasswordSignIn { get; private set; }

        public static string Email { get; private set; }

        public static string Password { get; private set; }

        public static string RepeatPassword { get; private set; }

        public static string PasswordNotMatch { get; private set; }

        public static string MustBeFilled { get; private set; }

        public static string High { get; private set; }

        public static string Low { get; private set; }

        public static string Normal { get; private set; }

        public static string FirstName { get; private set; }

        public static string LastName { get; private set; }

        public static int Port1024 { get; private set; }
    }
}
