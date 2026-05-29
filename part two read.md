# Cybersecurity Awareness Bot

A WPF‑based desktop chatbot that teaches users about cybersecurity topics such as **passwords**, **phishing**, **privacy**, and **scams**. The bot remembers user details, detects sentiment, provides random tips, and supports follow‑up questions. Voice greeting is included only on startup.

---


##  Features

- **Graphical User Interface** – Dark theme, ASCII logo, memory panel, chat log, and input area.
- **Keyword Recognition** – Detects cybersecurity topics using a delegate‑based dictionary.
- **Random Responses** – Each topic has multiple tips; the bot randomly selects one.
- **Follow‑up Support** – “another tip”, “tell me more”, “explain more” continues the current topic.
- **Memory & Recall** – Stores user name and favourite topic; personalises responses.
- **Sentiment Detection** – Recognises *worried*, *curious*, *frustrated* and responds empathetically.
- **Error Handling** – Graceful fallback for unrecognised input.
- **Voice Greeting** – Speaks “Hello! Welcome to the Cybersecurity Awareness Bot.” only once at startup.
- **Code Optimisation** – Uses generic collections (`Dictionary`, `List`) and custom delegates for extensibility.

---

## Technologies Used

- **.NET Framework 4.7.2** (or .NET Core / .NET 5+ with `System.Speech` package)
- **WPF (Windows Presentation Foundation)** – for the GUI
- **C#** – application logic
- **System.Speech** – text‑to‑speech for the welcome greeting

---

##  Project Structure
AwarenessBotGUI/
├── MainWindow.xaml # GUI layout (ASCII art, chat list, input)
├── MainWindow.xaml.cs # Bot logic, delegates, sentiment, memory
├── App.xaml / App.xaml.cs # Application entry point
├── ascii_logo.txt # (Optional) ASCII art file
└── README.md # This file

### Prerequisites

- **Visual Studio 2019/2022** (Community edition is fine)
- **.NET Framework 4.7.2** or later (or .NET 5/6/8 with `System.Speech` NuGet package)
- Windows OS (for `System.Speech`)
