# Final Year Project UI - Unity Project

## Description

This project is a Unity-based User Interface (UI) for my final year project. It is designed to provide an interactive and intuitive experience for the trial participants. The puzzle game is the only interface the users interact with throughout the participant trials. The game was developed in Unity, using its respective object-oriented scripting language, C#. The goal of the game is for the experiment participants to complete several Nonogram puzzles and, when stuck or confused, ask for hints about what to do next. Hints differ depending on the progress the user has made and the level they are at.
Completing the levels involves filling in the Nonogram puzzle to fully respect the numerical clues and solution state, and also guessing the meaning of the revealed image correctly.

This project works in collaboration with the AI Asssitant I developed. The codebase can be found in the repository: https://github.com/neagualexa/LLM-chat-bot

## Features

- The game is built onto 5 main scenes : Main Menu, Levels Menu Scene, Level Scene, Create Level Screen, Tutorial Level Screen 
- The developer can create any Nonogram puzzle, of any size. (The provided puzzles loaded in the levels are of 10x10 size.)
- The levels gradually unlock as the player progresses throughout the game. The player starts with a tutorial level that introduces them to the main rules of Nonograms, and every interaction they can make with the game to pass the levels.
- The game provides the user with an AI Assistant (running on the https://github.com/neagualexa/LLM-chat-bot) that provides the players with tailored help to learn the best strategies in completing the tasks.

## Installation

To install this project, follow these steps:

1. Clone the repository: `git clone https://github.com/neagualexa/NonogramUnity.git`
2. Open the project in Unity
3. Build the project
