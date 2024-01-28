# Multi-Cracker

## Overview
Multi-Cracker is a proof of concept for a distributed password cracker designed to harness the power of collaborative computing using discord as a C2.

## Usage
1. Download the latest release of the program.
2. Unzip it and open the project.
3. Find the form1.cs and do as the comments tell you (Basicly just change some variables)
4. When built the .exe file you can find it at Your-directory\Multi-Cracker\MultiCracker\bin\Debug\net6.0-windows
5. To open it you must have the .exe file and all of its .dll files in the same directory.

## Features
### Multi-Cracker
##### BASIC
- **!help** - Displays a help message that includes all commands for multi-cracker.
##### CRACKING 
- **!crack** - Starts cracking the password.
- **!crackAll** - Makes all bots (including itself) crack the password its selected.
- **!stop** - Stops cracking the password.
- **!setHash** - Sets the hash to crack.
- **!setAlgorithm** - Sets the algorithm to use.
- **!status** - Sends status.
- **!info** - Sends info.
- **!reset** - Resets the bot.
- **!setMax <length>** - Sets the passwords max length.
- **!setMin <length>** - Sets the passwords min length.
- **!setLetters <true/false>** - Sets the password to use letters.
- **!setNumbers <true/false>** - Sets the password to use numbers.
- **!setSymbols <true/false>** - Sets the password to use symbols.
- **!setCapitals <true/false>** - Sets the password to use capitals.
- **!split** - Splits the cracking between bots.
- **!resetStart** - Resets the start and end count.
##### OTHER
- **!kys** - Bot commits suicide...
- **!elevate** - Ask for elevation from user to admin on bot computer.
- **!cmd <command>** - Execute commands with CMD on bot's computer.
##### DEBUG
- **!log** - Sends entire log as .txt file.
- **!check <hash>** - Checks \"forces\" channel for missing passwords.
- **!auto** - Will trigger the auto crack feature.
- **!createAuto <Number of Splits>** - Creates a new auto-target for bots to target with current settings.
##### PERSISTENCE
- **!install** - Installs persistence.
- **!uninstall** - Uninstalls persistence.
- **!disableUAC** - Disables UAC.
##### DDoS
- **!ddos <ip> <port> <time>** - Starts a DDoS attack on the target.
- **!stopddos** - Stops all DDoS attacks.
##### RAT
- **!rat** - Starts a RAT on the target machine.

### RAT
##### BASIC
- **!help** - Displays a help message that includes all commands for RAT.
- **!kys** - Kills the RAT.
##### SURVENILLANCE
- **!screenshot** - Takes a screenshot of the screen.
- **!webcam** - Takes a picture with the webcam.
##### KEYLOGGER
- **!keylogger start** - Starts the keylogger.
- **!keylogger stop** - Stops the keylogger.
- **!keylogger dump** - Dumps the keylogger log.
##### EXECUTION
- **!cmd <command>** - Executes a command on the bot.
- **!elevate** - Elevates the bot to admin.
##### FILE SYSTEM 
- **!upload <DISCORD ATTACHMENT> <Folder\\of\\uploaded\\file>** - Uploads a file to the bot.
- **!download <filename>** - Downloads a file from the bot.
##### OTHER
- **!log** - Sends the log file to the C2.

## ToDo
- Command !microphone for RAT.
- Fix token validation checker for RAT primary and secondary token
- Better filter for RAT keylogger, its good but could be better.
- Save current crack-run to temp file on disk every heartbeat. (So it can be used later in case of disconnection.)
- Save settings to profiles. (file)
- Faster splitting (splitting without needing to to through all password combinations.) for !split, (Not really needed but yeah...)

Enhance your password cracking capabilities with Multi-Cracker's collaborative approach, fostering faster results through synchronized efforts.
