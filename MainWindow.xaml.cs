using System;
using System.Collections.Generic;
using System.Speech.Synthesis;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Bot
{
    public partial class MainWindow : Window
    {
        public delegate string TopicResponseDelegate();

        private Dictionary<string, TopicResponseDelegate> topicDelegates;

        private string userName = "";
        private string userInterest = "";
        private string lastTopic = "";

        private Dictionary<string, string> simpleResponses = new Dictionary<string, string>()
        {
            { "how are you", "I'm doing great! Ready to help you stay safe online." },
            { "what can you do", "I can teach you about passwords, phishing, privacy, scams, and more!" },
            { "thank", "You're welcome! Stay vigilant." },
            { "bye", "Goodbye! Remember to use strong passwords." }
        };

        private Dictionary<string, List<string>> randomTips = new Dictionary<string, List<string>>()
        {
            { "password", new List<string> {
                "Use at least 12 characters with uppercase, lowercase, numbers, and symbols.",
                "Never reuse passwords across sites. Use a password manager!",
                "Avoid dictionary words or personal info like your birthday."
            }},
            { "phishing", new List<string> {
                "Always check the sender's email address before clicking links.",
                "Hover over links to see the real URL before clicking.",
                "Look for urgent language or spelling errors - common in phishing emails."
            }},
            { "privacy", new List<string> {
                "Review app permissions regularly - they might access more than needed.",
                "Use a VPN on public Wi-Fi to encrypt your data.",
                "Check social media privacy settings - limit who sees your info."
            }},
            { "scam", new List<string> {
                "Hang up on unsolicited calls asking for personal info.",
                "Never pay upfront for a prize - real winnings don't ask for fees.",
                "If an offer seems too good to be true, it probably is."
            }}
        };

        private Dictionary<string, string> sentimentMap = new Dictionary<string, string>()
        {
            { "worried", "worried" }, { "scared", "worried" }, { "anxious", "worried" },
            { "curious", "curious" }, { "interested", "curious" },
            { "frustrated", "frustrated" }, { "confused", "frustrated" }
        };

        private SpeechSynthesizer speech = new SpeechSynthesizer();
        private Random rand = new Random();

        public MainWindow()
        {
            InitializeComponent();
            InitializeTopicDelegates();
            Loaded += MainWindow_Loaded;
        }

        private void InitializeTopicDelegates()
        {
            topicDelegates = new Dictionary<string, TopicResponseDelegate>
            {
                { "password", () => GetRandomTip("password") },
                { "passcode", () => GetRandomTip("password") },
                { "passphrase", () => GetRandomTip("password") },
                { "phishing", () => GetRandomTip("phishing") },
                { "privacy", () => GetRandomTip("privacy") },
                { "private", () => GetRandomTip("privacy") },
                { "data", () => GetRandomTip("privacy") },
                { "scam", () => GetRandomTip("scam") },
                { "fraud", () => GetRandomTip("scam") }
            };
        }

        private async void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            AddBotMessage("Hello! I'm your Cybersecurity Awareness Bot.");
            await VoiceGreeting(); 
            AddBotMessage("What's your name?");
        }

        
        private void ProcessInput()
        {
            string input = InputBox.Text.Trim();
            if (string.IsNullOrEmpty(input)) return;

            AddUserMessage(input);
            InputBox.Clear();

            string response = GetBotResponse(input);
            AddBotMessage(response);
            
        }

        private void Send_Click(object sender, RoutedEventArgs e) => ProcessInput();

        private void InputBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter) ProcessInput();
        }

        private string GetBotResponse(string input)
        {
            string lower = input.ToLower();

            if (lower == "exit" || lower == "quit")
                return "Stay safe online! Goodbye!";

            if (string.IsNullOrEmpty(userName) && !lower.Contains("my name is"))
            {
                userName = input;
                TxtName.Text = userName;
                return $"Nice to meet you, {userName}! What cybersecurity topic would you like to learn about? (Try: passwords, phishing, privacy, scams)";
            }
            else if (lower.Contains("my name is"))
            {
                string[] parts = input.Split(new[] { "my name is" }, StringSplitOptions.None);
                if (parts.Length > 1)
                {
                    userName = parts[1].Trim();
                    TxtName.Text = userName;
                    return $"Thanks {userName}! I'll remember you. Now, ask me about cybersecurity!";
                }
            }

            if (lower.Contains("i'm interested in") || lower.Contains("i like") || lower.Contains("my favorite"))
            {
                if (lower.Contains("password")) userInterest = "password";
                else if (lower.Contains("phishing")) userInterest = "phishing";
                else if (lower.Contains("privacy")) userInterest = "privacy";
                else if (lower.Contains("scam")) userInterest = "scam";

                if (!string.IsNullOrEmpty(userInterest))
                {
                    TxtInterest.Text = userInterest;
                    return $"Great! I'll remember that you're interested in {userInterest}. " + GetRandomTip(userInterest);
                }
            }

            if (lower.Contains("another tip") || lower.Contains("tell me more") || lower.Contains("explain more"))
            {
                if (!string.IsNullOrEmpty(lastTopic) && topicDelegates.ContainsKey(lastTopic))
                    return $"Sure! Here's another tip about {lastTopic}:\n{topicDelegates[lastTopic]()}";
                else
                    return "Please ask me about a specific topic first (like passwords or phishing).";
            }

            string sentiment = DetectSentiment(lower);
            string prefix = GetEmpathyPrefix(sentiment);

            foreach (var kvp in topicDelegates)
            {
                if (lower.Contains(kvp.Key))
                {
                    lastTopic = kvp.Key;
                    return prefix + kvp.Value();
                }
            }

            foreach (var kvp in simpleResponses)
            {
                if (lower.Contains(kvp.Key))
                    return prefix + kvp.Value;
            }

            if (!string.IsNullOrEmpty(userInterest) && (lower.Contains("help") || lower.Contains("suggest")))
            {
                return $"Since you're interested in {userInterest}, here's a tip: {GetRandomTip(userInterest)}";
            }

            return "I'm not sure I understand. Try asking about: passwords, phishing, privacy, or scams.";
        }

        private string GetRandomTip(string topic)
        {
            if (!randomTips.ContainsKey(topic)) return GetBasicInfo(topic);
            var tips = randomTips[topic];
            return tips[rand.Next(tips.Count)];
        }

        private string GetBasicInfo(string topic)
        {
            switch (topic)
            {
                case "password": return "Use strong, unique passwords for every account. Consider a password manager.";
                case "phishing": return "Phishing attacks trick you via fake emails. Always verify the sender.";
                case "scam": return "Scammers create urgency. Never share personal info over the phone.";
                case "privacy": return "Protect your privacy by limiting what you share online and using VPNs.";
                default: return "Cybersecurity is about protecting your data. Start with strong passwords!";
            }
        }

        private string DetectSentiment(string lower)
        {
            foreach (var kvp in sentimentMap)
                if (lower.Contains(kvp.Key))
                    return kvp.Value;
            return "neutral";
        }

        private string GetEmpathyPrefix(string sentiment)
        {
            if (sentiment == "worried")
                return "It's completely understandable to feel worried. ";
            else if (sentiment == "curious")
                return "Great question! ";
            else if (sentiment == "frustrated")
                return "I know this can be confusing. Let me help: ";
            return "";
        }

        private void AddUserMessage(string msg)
        {
            ChatList.Items.Add($" You: {msg}");
            ChatList.ScrollIntoView(ChatList.Items[^1]);
        }

        private void AddBotMessage(string msg)
        {
            ChatList.Items.Add($"Bot: {msg}");
            ChatList.ScrollIntoView(ChatList.Items[^1]);
        }

        
        private async Task VoiceGreeting()
        {
            try
            {
                await Task.Run(() => speech.SpeakAsync("Hello! Welcome to the Cybersecurity Awareness Bot."));
            }
            catch
            {
                
            }
        }
    }
}