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
- **!kys** - Bot commits suicide...
- **!restart** - Restarts the bot.
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
- **!elevate** - Ask for elevation from user to admin on bot computer.
- **!version** - Print version of current multi-cracker bot.
- **!execute <vbs/powershell> <url-to-script-to-execute>** - Could be used to update the bot to your newer version of multi-cracker, or just execute vbs/powershell scripts.
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

## Examples
### Crack a SHA1 hash with one computer: (password: 12345)
#### Set the hash algorithm
1. !setAlgorithm SHA1
#### Configure settings for password (only numbers in this case)
2. !setLetters false
3. !setNumbers true
4. !setSymbols false
5. !setCapitals false
#### Configure password length (12345 = 5)
6. !setMin 5
7. !setMax 5
#### Set the hash (hash for 12345)
8. !setHash 8cb2237d0679ca88db6464eac60da96345513964
#### Last step... begin cracking!
9. !crack

### Crack MD5 with multiple computers using Auto-Crack:
#### Set algorithm
1. !setAlgorithm MD5
#### Set hash for password (12345)
2. !setHash 827ccb0eea8a706c4c34a16891f84e7b
#### Configure settings for password (only numbers in this case)
3. !setLetters false
4. !setNumbers true
5. !setSymbols false
6. !setCapitals false
#### Configure password length (12345 = 5)
7. !setMin 5
8. !setMax 5
#### Create the auto-crack settings
9. !createAuto 50



## ToDo
- Command !microphone for RAT.
- Fix token validation checker for RAT primary and secondary token
- Better filter for RAT keylogger, its good but could be better.
- Save current crack-run to temp file on disk every heartbeat. (So it can be used later in case of disconnection.)
- Save settings to profiles. (file)
- Faster splitting (splitting without needing to to through all password combinations.) for !split, (Not really needed but yeah...)

Enhance your password cracking capabilities with Multi-Cracker's collaborative approach, fostering faster results through synchronized efforts.
