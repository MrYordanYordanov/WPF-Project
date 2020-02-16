using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using BCrypt.Net;

namespace PointsOfInterest
{
    /// <summary>
    /// Interaction logic for Register.xaml
    /// </summary>
    public partial class Register : Window
    {
        public Register()
        {
            InitializeComponent();
        }

        private void button2_Click(object sender, RoutedEventArgs e)
        {
            Login log = new Login();
            log.Show();
            this.Hide();
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            if (txtemail.Text.Trim() != "" && txtname.Text.Trim() != "" && txtpassword.Password.Trim() != "")
            {
                try
                {
                    using (var db = new PointsOfInterestContext())
                    {
                        var email = txtemail.Text.Trim();
                        var name = txtname.Text.Trim();
                        var password = txtpassword.Password.Trim();

                        var user = db.Users.SingleOrDefault(x => x.Email == email);

                        if (user == null)
                        {
                            user = new User();
                            user.Email = email;
                            user.Name = name;
                            
                            string passwordHashed = BCrypt.Net.BCrypt.HashPassword(password, 
                                BCrypt.Net.BCrypt.GenerateSalt());
                            user.Password = passwordHashed;
                            user.IsAdmin = false;

                            db.Users.Add(user);
                            db.SaveChanges();

                            MessageBox.Show("Registration Successfull.");
                            Login log = new Login();
                            log.Show();
                            this.Close();
                        }
                        else
                        {
                            MessageBox.Show("This email is already register");
                        }
                    }
                }

                catch (Exception ex)
                {
                    throw ex;
                }
            }
            else
            {
                MessageBox.Show("Please fill all details");
            }
        }
    }
}