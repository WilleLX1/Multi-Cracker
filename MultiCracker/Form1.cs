using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Security.Cryptography;
using Discord;
using Discord.WebSocket;
using System.Threading.Tasks;
using Microsoft.VisualBasic.Logging;
using Discord.Commands;

namespace MultiCracker
{
    public partial class Form1 : Form
    {
        bool SomeoneElseFoundPassword = false;
        bool FoundCorrectPass = false;
        string elsesPassword = "N/A";
        string elsesHash = "N/A";

        // Communication
        string BOT_TOKEN = "";

        int counter = 0;
        // Create timer
        System.Timers.Timer timer1 = new System.Timers.Timer();

        // List of non-working passwords
        List<string> nonWorkingPasswords = new List<string>();

        public Form1()
        {
            InitializeComponent();
            InitializeAsync();
        }

        // Discord
        private DiscordSocketClient _client;
        private async void InitializeAsync()
        {
            await InitializeBot();
        }
        private async Task InitializeBot()
        {
            _client = new DiscordSocketClient();
            _client.Log += Log;

            await _client.LoginAsync(TokenType.Bot, BOT_TOKEN);
            await _client.StartAsync();

            _client.MessageReceived += HandleMessage;
        }

        private Task Log(LogMessage arg)
        {
            // Handle logging (e.g., display in a TextBox)
            txtOutput.AppendText(arg + "\r\n");
            return Task.CompletedTask;
        }

        private async Task HandleMessage(SocketMessage arg)
        {
            Log(new LogMessage(LogSeverity.Info, "Message", "Message event triggered."));

            if (arg is not IUserMessage message || message.Author.IsBot)
                return;

            // Trim the message content to remove leading and trailing whitespaces
            string trimmedContent = message.Content.Trim();

            if (!string.IsNullOrEmpty(trimmedContent))
            {
                Log(new LogMessage(LogSeverity.Info, "Message", $"Received message: {trimmedContent}"));
                // Your other message processing logic...
            }
            else
            {
                Log(new LogMessage(LogSeverity.Info, "Message", "Received empty or whitespace-only message."));
            }
        }










        // Form
        public void PassToHash(string pass)
        {
            string hash = "";
            byte[] data = Encoding.ASCII.GetBytes(pass);
            data = new SHA256Managed().ComputeHash(data);
            foreach (byte b in data)
            {
                hash += b.ToString("x2");
            }

            // Debug
            counter++;
            txtOutput.AppendText("Attempt #" + counter + ": " + pass + "\r\n");
            //txtOutput.AppendText("Hashed password: " + hash + "\r\n");

            txtGuessedPassword.Text = hash;
        }

        private void btnStartCracking_Click(object sender, EventArgs e)
        {
            string finalPassword = "";

            // Start timer
            timer1.Start();

            while (!FoundCorrectPass)
            {
                // Generate a random password containing 5 numbers
                Random rnd = new Random();
                int pass = rnd.Next(1000, 9999);
                // If password is in the list of non-working passwords, generate a new one
                while (nonWorkingPasswords.Contains(pass.ToString()))
                {
                    pass = rnd.Next(1000, 9999);
                }
                string currentPassword = pass.ToString();
                txtGuessedPassword.Text = currentPassword;

                // Hash the password and compare it to the target hash
                PassToHash(currentPassword);

                if (txtGuessedPassword.Text == txtTargetHash.Text || SomeoneElseFoundPassword)
                {
                    if (SomeoneElseFoundPassword)
                    {
                        // Stop timer
                        timer1.Stop();
                        FoundCorrectPass = true;

                        txtOutput.AppendText("Someone else found the password first!\r\n");
                        txtOutput.AppendText("Password found: (" + elsesHash + ":" + elsesPassword + ")\r\n");

                        // Print how long it took to find the password
                        TimeSpan time = TimeSpan.FromMilliseconds(timer1.Interval * counter);
                        txtOutput.AppendText(time.ToString(@"hh\:mm\:ss\:fff") + "\r\n");
                    }
                    else
                    {
                        // Stop timer
                        timer1.Stop();
                        FoundCorrectPass = true;
                        finalPassword = pass.ToString();

                        txtOutput.AppendText("Password Found! (" + txtGuessedPassword.Text + ":" + finalPassword + ")\r\n");

                        // rint how long it took to find the password
                        TimeSpan time = TimeSpan.FromMilliseconds(timer1.Interval * counter);
                        txtOutput.AppendText(time.ToString(@"hh\:mm\:ss\:fff") + "\r\n");
                    }
                }
                else
                {
                    // Add hash/password to a list of non-working passwords
                    nonWorkingPasswords.Add(txtGuessedPassword.Text);
                }
            }

            // The loop is done, and you don't need to redeclare 'password' here.
            // If you want to use the target hash as the final password, you can do this:
            txtDonePassword.Text = finalPassword.ToString();
        }
    }

}