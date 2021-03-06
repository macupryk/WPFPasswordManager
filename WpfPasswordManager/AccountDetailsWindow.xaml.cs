﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace WpfPasswordManager
{
    /// <summary>
    /// Interaction logic for AccountDetailsWindow.xaml
    /// </summary>
    public partial class AccountDetailsWindow : Window
    {
        State currentState;
        public enum State {Add, Edit};
        public AccountDetails accountDetails;
        long userId;
        String hash;

        public AccountDetailsWindow(State state, long userId)
        {
            InitializeComponent();
            this.currentState = state;
            this.userId = userId;
            SQLiteDbHelper sqLiteDbHelper = SQLiteDbHelper.getInstance();
            this.hash = sqLiteDbHelper.getUserHash(this.userId);
        }

        public AccountDetailsWindow(State state, long accountId, long userId)
        {
            InitializeComponent();
            this.currentState = state;
            this.userId = userId;
            if(this.currentState == State.Edit)
            {
                SQLiteDbHelper sqLiteDbHelper = SQLiteDbHelper.getInstance();
                this.accountDetails = sqLiteDbHelper.selectWithId(accountId, userId);
                this.hash = sqLiteDbHelper.getUserHash(this.userId);
                this.fillFields(accountDetails);
            }
        }

        public void onCancelBtnClicked(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        public void onSaveBtnClicked(object sender, RoutedEventArgs e)
        {
            SQLiteDbHelper sqLiteDbHelper = SQLiteDbHelper.getInstance();
            String passwordText = this.getPasswordFieldText();
            byte[] salt = CryptoHelper.generateSalt();
            String saltString = Convert.ToBase64String(salt);
            String encryptedPassword = CryptoHelper.EncryptStringAES(passwordText, hash, salt);
            if (this.currentState == State.Add)
            {
                sqLiteDbHelper.insert(this.userId, titleField.Text, usernameField.Text, encryptedPassword, saltString);
            }
            else
            {
                AccountDetails updatedAccountDetails = new AccountDetails(this.accountDetails.Id, titleField.Text, usernameField.Text, encryptedPassword, saltString);
                sqLiteDbHelper.update(updatedAccountDetails, this.userId);
            }
            
            this.Close();
        }

        public void fillFields(AccountDetails accountDetails)
        {
            titleField.Text = accountDetails.Title;
            usernameField.Text = accountDetails.Username;
            accountDetails.decryptPassword(this.hash);
            passwordField.Password = accountDetails.Password;
        }

        public void onCheckBoxClicked(object sender, RoutedEventArgs e)
        {
            if (passwordCheckBox.IsChecked.HasValue)
            {
                if ((bool)passwordCheckBox.IsChecked)
                {
                    textPasswordField.Text = passwordField.Password;
                    passwordField.Visibility = Visibility.Hidden;
                    textPasswordField.Visibility = Visibility.Visible;
                }
                else
                {
                    passwordField.Password = textPasswordField.Text;
                    textPasswordField.Visibility = Visibility.Hidden;
                    passwordField.Visibility = Visibility.Visible;
                }
            }
        }

        public String getPasswordFieldText()
        {
            if (passwordCheckBox.IsChecked.HasValue && (bool)passwordCheckBox.IsChecked)
            {
                return textPasswordField.Text;                
            }
            else
            {
                return passwordField.Password;
            }
        }
    }
}
