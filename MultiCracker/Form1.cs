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
using System.Runtime.InteropServices;

namespace MultiCracker
{
    public partial class Form1 : Form
    {
        bool EmergencySTOP = false;

        bool SomeoneElseFoundPassword = false;
        bool FoundCorrectPass = false;
        string elsesPassword = "N/A";
        string elsesHash = "N/A";
        string currentPassword = "";

        string hash;
        string password;

        // Hash settings
        string Algorithm = "SHA256";
        bool UseNumbers = true;
        bool UseLetters = true;
        bool UseSymbols = true;
        bool UseCapitals = true;
        int MaxLength = 11;
        int MinLength = 11;
        // NeoMeyer20!


        // Communication
        string BOT_TOKEN = "BOT-HERE";

        int counter = 0;

        // Create timer
        System.Timers.Timer timer1 = new System.Timers.Timer();

        // List of non-working passwords
        List<string> nonWorkingPasswords = new List<string>();

        public Form1()
        {
            InitializeComponent();
            Task.Run(() => DiscordThread());
        }

        // Discord
        private DiscordSocketClient _client;
        private async Task DiscordThread()
        {
            var config = new DiscordSocketConfig
            {
                LogLevel = LogSeverity.Info,
                GatewayIntents = GatewayIntents.All // Enable all intents
            };

            _client = new DiscordSocketClient(config);
            _client.Log += Log;

            await _client.LoginAsync(TokenType.Bot, BOT_TOKEN);
            await _client.StartAsync();

            _client.MessageReceived += HandleMessage;
        }

        private Task Log(LogMessage arg)
        {
            // Handle logging (e.g., display in a TextBox)
            Invoke(new Action(() =>
            {
                txtOutput.AppendText(arg + "\r\n");
            }));

            return Task.CompletedTask;
        }

        private async Task HandleMessage(SocketMessage arg)
        {
            if (arg is not IUserMessage message || message.Author.IsBot)
                return;

            bool heavyLoging = true;
            if (heavyLoging)
            {
                Log(new LogMessage(LogSeverity.Info, "Message", "Message event triggered."));

                // Check the type of the message
                Log(new LogMessage(LogSeverity.Info, "Message", $"Message type: {arg.Type}"));

                // Check for embeds or attachments
                if (message.Embeds.Any())
                    Log(new LogMessage(LogSeverity.Info, "Message", "Message contains embeds."));
                if (message.Attachments.Any())
                    Log(new LogMessage(LogSeverity.Info, "Message", "Message contains attachments."));

                // Log the raw content of the message
                Log(new LogMessage(LogSeverity.Info, "Message", $"Raw message content: {message.Content}"));
            }
            
            // Trim the message content to remove leading and trailing whitespaces
            string trimmedContent = message.Content.Trim();

            // Example of using Invoke for UI updates
            Invoke(new Action(() =>
            {
                txtOutput.AppendText($"Received message: {trimmedContent}\r\n");
            }));

            if (!string.IsNullOrEmpty(trimmedContent))
            {
                Log(new LogMessage(LogSeverity.Info, "Message", $"Received message: {trimmedContent}"));
                // Your other message processing logic...
                if (trimmedContent == "!help")
                {
                    // Send a message to the channel
                    string help_message = "Hello! I am a bot that can crack passwords. Here are my commands:\r\n!help - Displays this message.\r\n!crack - Starts cracking the password.\r\n!stop - Stops cracking the password.\r\n!setHash - Sets the hash to crack.\r\n!setHash - Sets the hash to crack.\r\n!status - Sends status.\r\n!info - Sends info.\r\n!reset - Resets the bot.\r\n!setHash - Sets the hash to crack.\r\n!setMax - Sets the passwords max length.\r\n!setMin - Sets the passwords min length.\r\n!setLetters - Sets the password to use letters.\r\n!setNumbers - Sets the password to use numbers.\r\n!setSymbols - Sets the password to use symbols.\r\n!setCapitals - Sets the password to use capitals.\r\n!status - Sends status.\r\n\r\n";
                    await message.Channel.SendMessageAsync(help_message);

                    // Hello! I am a bot that can crack passwords.
                    // Here are my commands:
                    // !help - Displays this message.

                    // DONE !crack - Starts cracking the password.
                    // DONE !stop - Stops cracking the password.

                    // DONE !setHash - Sets the hash to crack.
                    // DONE !status - Sends status.
                    // DONE !info - Sends info.
                    // DONE !reset - Resets the bot.

                    // DONE !setMax <length> - Sets the passwords max length.
                    // DONE !setMin <length> - Sets the passwords min length.

                    // DONE !setLetters <true/false> - Sets the password to use letters.
                    // DONE !setNumbers <true/false> - Sets the password to use numbers.
                    // DONE !setSymbols <true/false> - Sets the password to use symbols.
                    // DONE !setCapitals <true/false> - Sets the password to use capitals.

                }
                else if (trimmedContent.StartsWith("!crack"))
                {
                    // Simulate a button press of btnStartCracking
                    btnStartCracking_Click(null, null);
                    await message.Channel.SendMessageAsync("Cracking started... Use **!status** it see its progress!");
                }
                else if (trimmedContent.StartsWith("!stop"))
                {
                    btnStopCracking_Click(null, null);
                    await message.Channel.SendMessageAsync("Pushed Emergency Eutton!!! This will halt any cracking.");
                }
                else if (trimmedContent.StartsWith("!setHash"))
                {
                    // Find the argument after !sethash
                    string argument = trimmedContent.Substring(9);
                    hash = argument;
                    txtTargetHash.Text = hash;

                    await message.Channel.SendMessageAsync($"Set hash to: **{hash}**");
                }
                else if (trimmedContent.StartsWith("!setMax"))
                {
                    // Find the argument after !setMaxLength
                    string maxLength = trimmedContent.Substring(7);
                    MaxLength = Convert.ToInt32(maxLength);
                    txtMax.Text = maxLength;

                    // Return responce with "Max length set to: <length>"
                    await message.Channel.SendMessageAsync($"Max length set to: **{maxLength}**");

                    Log(new LogMessage(LogSeverity.Info, "Password", $"Max length set to: {maxLength}"));
                }
                else if (trimmedContent.StartsWith("!setMin"))
                {
                    // Find the argument after !setMinLength
                    string minLength = trimmedContent.Substring(7);
                    MinLength = Convert.ToInt32(minLength);
                    txtMin.Text = minLength;

                    // Return responce with "Min length set to: <length>"
                    await message.Channel.SendMessageAsync($"Min length set to: **{minLength}**");

                    Log(new LogMessage(LogSeverity.Info, "Password", $"Min length set to: {minLength}"));
                }
                else if (trimmedContent == "!status")
                {
                    Invoke(new Action(() =>
                    {
                        string status = GetStatus();
                        message.Channel.SendMessageAsync(status);
                        Log(new LogMessage(LogSeverity.Info, "Status", $"Status: {status}"));
                    }));
                }
                else if (trimmedContent == "!info")
                {
                    string info = GetInfo();
                    await message.Channel.SendMessageAsync(info);
                    Log(new LogMessage(LogSeverity.Info, "Info", $"Info: {info}"));
                }
                else if (trimmedContent == "!reset")
                {
                    Algorithm = "SHA256";
                    UseNumbers = false;
                    UseLetters = false;
                    UseSymbols = false;
                    UseCapitals = false;
                    MaxLength = 8;
                    MinLength = 5;
                    counter = 0;
                    await message.Channel.SendMessageAsync("Reset settings.");
                    Log(new LogMessage(LogSeverity.Info, "Message", "Reset settings."));
                }
                else if (trimmedContent.StartsWith("!setLetters"))
                {
                    // Find the argument after !sethash
                    string argument = trimmedContent.Substring(12);
                    
                    if (argument == "true")
                    {
                        UseLetters = true;
                        await message.Channel.SendMessageAsync("Letters enabled.");
                    }
                    else if (argument == "false")
                    {
                        UseLetters = false;
                        await message.Channel.SendMessageAsync("Letters disabled.");
                    }
                    else
                    {
                        await message.Channel.SendMessageAsync("Invalid argument. Please use \"true\" or \"false\".");
                    }
                }
                else if (trimmedContent.StartsWith("!setNumbers"))
                {
                    // Find the argument after !sethash
                    string argument = trimmedContent.Substring(12);

                    if (argument == "true")
                    {
                        UseNumbers = true;
                        await message.Channel.SendMessageAsync("Numbers enabled.");
                    }
                    else if (argument == "false")
                    {
                        UseNumbers = false;
                        await message.Channel.SendMessageAsync("Numbers disabled.");
                    }
                    else
                    {
                        await message.Channel.SendMessageAsync("Invalid argument. Please use \"true\" or \"false\".");
                    }
                }
                else if (trimmedContent.StartsWith("!setCapitals"))
                {
                    // Find the argument after !sethash
                    string argument = trimmedContent.Substring(13);

                    if (argument == "true")
                    {
                        UseCapitals = true;
                        await message.Channel.SendMessageAsync("Capitals enabled.");
                    }
                    else if (argument == "false")
                    {
                        UseCapitals = false;
                        await message.Channel.SendMessageAsync("Capitals disabled.");
                    }
                    else
                    {
                        await message.Channel.SendMessageAsync("Invalid argument. Please use \"true\" or \"false\".");
                    }
                }
                else if (trimmedContent.StartsWith("!setSymbols"))
                {
                    // Find the argument after !sethash
                    string argument = trimmedContent.Substring(12);

                    if (argument == "true")
                    {
                        UseSymbols = true;
                        await message.Channel.SendMessageAsync("Symbols enabled.");
                    }
                    else if (argument == "false")
                    {
                        UseSymbols = false;
                        await message.Channel.SendMessageAsync("Symbols disabled.");
                    }
                    else
                    {
                        await message.Channel.SendMessageAsync("Invalid argument. Please use \"true\" or \"false\".");
                    }
                }
                else if (trimmedContent.StartsWith("!"))
                {
                    await message.Channel.SendMessageAsync("Invalid command. Please use !help for a list of commands.");
                    Log(new LogMessage(LogSeverity.Info, "Message", $"Invalid command: {trimmedContent}"));
                }
            }
            else
            {
                Log(new LogMessage(LogSeverity.Info, "Message", "Received empty or whitespace-only message."));
            }
        }





        // get status
        private string GetStatus()
        {
            string status = "";
            if (FoundCorrectPass)
            {
                status += "Password found!\r\n";
            }
            else
            {
                status += "Password not found.\r\n";
            }
            status += $"Attempts: {counter}\r\n";
            // Calculate possible combinations
            long possibleCombinations = CalculateCombinations(UseNumbers, UseLetters, UseSymbols, UseCapitals, MaxLength, MinLength);
            status += $"Possible Combinations: {CalculateCombinations(UseNumbers, UseLetters, UseSymbols, UseCapitals, MaxLength, MinLength)}\r\n";
            status += $"Guesses: {counter}/{possibleCombinations}\r\n";
            // Calculate hashes per second (counter / elapsed time in seconds)
            double elapsedTimeInSeconds = counter * (timer1.Interval / 1000.0); // Convert milliseconds to seconds
            int hashPerSec = (int)(counter / elapsedTimeInSeconds);
            status += $"Hashes per second: {hashPerSec}\r\n";
            // Calculate time elapsed
            TimeSpan time = TimeSpan.FromMilliseconds(timer1.Interval * counter);
            status += $"Time elapsed: {time.ToString(@"hh\:mm\:ss\:fff")}\r\n";
            // Calculate estimated time remaining
            // Estimated time remaining = (Possible Combinations - Attempts) / Hashes per second
            int timeRemaining = (Convert.ToInt32(CalculateCombinations(UseNumbers, UseLetters, UseSymbols, UseCapitals, MaxLength, MinLength)) - counter) / hashPerSec;
            TimeSpan timeLeft = TimeSpan.FromSeconds(timeRemaining);    
            status += $"Estimated time remaining: {timeLeft.ToString(@"hh\:mm\:ss\:fff")}\r\n";

            return status;
        }
        private string GetInfo()
        {
            string info = "";
            info += $"Hash: {hash} \r\n";
            info += $"Algorithm: {Algorithm} \r\n";
            info += $"UseNumbers: {UseNumbers} \r\n";
            info += $"UseLetters: {UseLetters} \r\n";
            info += $"UseSymbols: {UseSymbols} \r\n";
            info += $"UseCapitals: {UseCapitals} \r\n";
            info += $"Max length: {MaxLength} \r\n";
            info += $"Min length: {MinLength} \r\n";
            return info;
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
            //txtOutput.AppendText("Attempt #" + counter + ": " + pass + "\r\n");
            //txtOutput.AppendText("Hashed password: " + hash + "\r\n");

            txtGuessedPassword.Text = hash;
        }

        private async void btnStartCracking_Click(object sender, EventArgs e)
        {
            await Task.Run(() => StartCrackingAsync());
        }

        private async Task StartCrackingAsync()
        {
            Log(new LogMessage(LogSeverity.Info, "Cracking", "Cracking started..."));
            string finalPassword = "";
            FoundCorrectPass = false;
            EmergencySTOP = false;

            timer1.Start();

            foreach (var password in GeneratePasswords(MinLength, MaxLength, UseLetters, UseNumbers, UseCapitals, UseSymbols))
            {
                if (EmergencySTOP)
                {
                    break;
                }

                PassToHash(password);

                // Check if found password is the target password
                if (txtGuessedPassword.Text == txtTargetHash.Text)
                {
                    timer1.Stop();
                    FoundCorrectPass = true;
                    finalPassword = password;
                    Log(new LogMessage(LogSeverity.Info, "Cracking", "Password found! (" + txtGuessedPassword.Text + ":" + finalPassword + ")"));
                    // Log how much time it took
                    TimeSpan time = TimeSpan.FromMilliseconds(timer1.Interval * counter);
                    Log(new LogMessage(LogSeverity.Info, "Cracking", "It only took " + time.ToString(@"hh\:mm\:ss\:fff") + " to find the password!"));
                    break;
                }
            }

            // Start timer
            timer1.Start();

            // The loop is done, and you don't need to redeclare 'password' here.
            // If you want to use the target hash as the final password, you can do this:
            txtDonePassword.Text = finalPassword.ToString();
        }


        private void SetHashInfo()
        {
            // Set Hash settings
            Algorithm = "SHA256";
            UseNumbers = true;
            UseLetters = false;
            UseSymbols = false;
            UseCapitals = false;
            MaxLength = 2;
            MinLength = 1;
        }

        // Generate passwords
        public IEnumerable<string> GeneratePasswords(int minLength, int maxLength, bool useLetters, bool useNumbers, bool useCapitals, bool useSymbols)
        {
            var chars = "";
            if (useLetters) chars += "abcdefghijklmnopqrstuvwxyz";
            if (useNumbers) chars += "0123456789";
            if (useCapitals) chars += "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            if (useSymbols) chars += "!@#$%^&*()";

            // Calculate the number of combinations
            long combinations = CalculateCombinations(UseNumbers, UseLetters, UseSymbols, UseCapitals, MaxLength, MinLength);
            Log(new LogMessage(LogSeverity.Info, "Password", "Number of combinations: " + combinations));

            for (int length = minLength; length <= maxLength; length++)
            {
                foreach (var password in GenerateCombinations(chars, length))
                {
                    yield return password;
                }
            }
        }

        private IEnumerable<string> GenerateCombinations(IEnumerable<char> chars, int length)
        {
            if (length == 1) return chars.Select(c => c.ToString());

            return GenerateCombinations(chars, length - 1)
                .SelectMany(str => chars, (str, c) => str + c);
        }

        // Calculate combinations
        static long CalculateCombinations(bool useNumbers, bool useLetters, bool useSymbols, bool useCapitals, int maxLength, int minLength)
        {
            // Define the number of characters for each type
            int numNumbers = useNumbers ? 10 : 0; // 0-9
            int numLetters = useLetters ? 26 : 0; // a-z
            int numSymbols = useSymbols ? 32 : 0; // You can adjust this based on the symbols you want to include
            int numCapitals = useCapitals ? 26 : 0; // A-Z

            // Calculate the total number of characters
            int totalCharacters = numNumbers + numLetters + numSymbols + numCapitals;

            // Validate the input parameters
            if (minLength < 1 || maxLength < minLength)
            {
                throw new ArgumentException("Invalid length parameters");
            }

            // Calculate the number of combinations
            long combinations = 0;
            for (int length = minLength; length <= maxLength; length++)
            {
                combinations += (long)Math.Pow(totalCharacters, length);
            }

            return combinations;
        }


        private void txtTargetHash_TextChanged(object sender, EventArgs e)
        {
            hash = txtTargetHash.Text;
        }

        private void txtDonePassword_TextChanged(object sender, EventArgs e)
        {
            password = txtDonePassword.Text;
        }

        private void btnStopCracking_Click(object sender, EventArgs e)
        {
            EmergencySTOP = true;
            Log(new LogMessage(LogSeverity.Info, "Cracking", "Emergency stop triggered."));
        }

        private void txtMax_TextChanged(object sender, EventArgs e)
        {
            MaxLength = Convert.ToInt32(txtMax.Text);
        }

        private void txtMin_TextChanged(object sender, EventArgs e)
        {
            MinLength = Convert.ToInt32(txtMin.Text);
        }
    }

}