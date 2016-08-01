﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;

namespace WpfPasswordManager
{
    class SQLiteDbHelper
    {
        private static SQLiteDbHelper instance = null;
        SQLiteConnection dbConnection;
        // Account Table
        String create_accounts_table = "CREATE TABLE accounts (id INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL, user_id INTEGER NOT NULL, title VARCHAR(20), username VARCHAR(20), password VARCHAR(20))";
        String insert_account_into_table = "INSERT INTO accounts (user_id, title, username, password) VALUES";
        String select_titles = "SELECT id, title FROM accounts WHERE user_id=";
        String select_account_with_id = "SELECT * FROM accounts WHERE id=";
        String update_account = "UPDATE accounts SET ";
        String delete_account = "DELETE FROM accounts WHERE id=";

        // PassManager Table
        String create_users_table = "CREATE TABLE users (id INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL, username VARCHAR(20), hash TEXT, salt TEXT)";
        String insert_user = "INSERT INTO users (username, hash, salt) VALUES";
        String select_user = "SELECT * FROM users WHERE username=";

        public static SQLiteDbHelper getInstance()
        {
            if (SQLiteDbHelper.instance == null)
            {
                SQLiteDbHelper.instance = new SQLiteDbHelper();
            }
            return SQLiteDbHelper.instance;
        }

        private SQLiteDbHelper()
        {
            //if file doesn't exist
            SQLiteConnection.CreateFile("MyDatabase.sqlite");

            // connect to database
            this.dbConnection = new SQLiteConnection("Data Source=MyDatabase.sqlite;Version=3;");
            this.dbConnection.Open();

            // create tables
            SQLiteCommand command = new SQLiteCommand(create_accounts_table, dbConnection);
            command.ExecuteNonQuery();

            command = new SQLiteCommand(create_users_table, dbConnection);
            command.ExecuteNonQuery();

            this.dbConnection.Close();
        }

        public void insert(long userId, String title, String username, String password)
        {
            this.dbConnection.Open();

            String insertSql = this.insert_account_into_table + "('" + userId + "','" + title + "','" + username + "','" + password + "')"; 
            SQLiteCommand command = new SQLiteCommand(insertSql, dbConnection);
            command.ExecuteNonQuery();

           this.dbConnection.Close();
        }

        public List<AccountTitle> selectTitles(long userId)
        {
            this.dbConnection.Open();

            String selectTitlesSql = this.select_titles + userId;

            SQLiteCommand command = new SQLiteCommand(selectTitlesSql, dbConnection);
            SQLiteDataReader reader = command.ExecuteReader();
            List<AccountTitle> accountTitles = new List<AccountTitle>();
            while (reader.Read())
            {
                AccountTitle accountTitle = new AccountTitle((long)reader["id"], (String)reader["title"]);
                accountTitles.Add(accountTitle);
            }
            this.dbConnection.Close();
            return accountTitles;
        }

        public AccountDetails selectWithId(long id, long userId)
        {
            this.dbConnection.Open();

            String sql = this.select_account_with_id + id + " AND user_id=" + userId;
            SQLiteCommand command = new SQLiteCommand(sql, dbConnection);
            SQLiteDataReader reader = command.ExecuteReader();
            AccountDetails accountDetails = new AccountDetails();
            while (reader.Read())
            {
                accountDetails.Id = (long)reader["id"];
                accountDetails.Title = (String)reader["title"];
                accountDetails.Username = (String)reader["username"];
                accountDetails.Password = (String)reader["password"];
            }
            this.dbConnection.Close();
            return accountDetails;
        }

        public void update(AccountDetails accountDetails, long userId)
        {
            this.dbConnection.Open();

            String updateSql = this.update_account + "title='" + accountDetails.Title + "',username='" + accountDetails.Username + "',password='" + accountDetails.Password + "' WHERE id=" + accountDetails.Id + " AND user_id=" + userId;
            SQLiteCommand command = new SQLiteCommand(updateSql, dbConnection);
            command.ExecuteNonQuery();
            this.dbConnection.Close();
        }

        public void delete(long id, long userId)
        {
            this.dbConnection.Open();

            String deleteSql = this.delete_account + id + " AND user_id=" + userId;
            SQLiteCommand command = new SQLiteCommand(deleteSql, dbConnection);
            command.ExecuteNonQuery();
            this.dbConnection.Close();
        }

        public void insertUser(String username, byte[] hash, byte[] salt)
        {
            this.dbConnection.Open();

            String hashString = Convert.ToBase64String(hash);
            String saltString = Convert.ToBase64String(salt);

            String insertSql = this.insert_user + "('" + username + "','" + hashString + "','" + saltString + "')";
            SQLiteCommand command = new SQLiteCommand(insertSql, dbConnection);
            command.ExecuteNonQuery();

            this.dbConnection.Close();
        }

        public List<User> selectUser(String username)
        {
            this.dbConnection.Open();

            String sql = this.select_user + "'" + username + "'";
            SQLiteCommand command = new SQLiteCommand(sql, dbConnection);
            SQLiteDataReader reader = command.ExecuteReader();
            List<User> users = new List<User>();
            while (reader.Read())
            {
                User user = new User();
                user.Id = (long)reader["id"];
                user.Username = (String)reader["username"];
                user.Hash = (String)reader["hash"];
                user.Salt = (String)reader["salt"];
                users.Add(user);
            }
            this.dbConnection.Close();
            return users;
        }
    }
}
