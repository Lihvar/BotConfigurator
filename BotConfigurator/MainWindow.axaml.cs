using Avalonia;
using Avalonia.Controls;
using Avalonia.Dialogs;
using Avalonia.Interactivity;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using Avalonia.Styling;
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Avalonia.Threading;
using System.Timers;
using System.Net;

namespace BotConfigurator
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            SaveConfigBtn.IsVisible = false;
        }

        Color checkboxTheme; //Currently useless, supposed to be a theme color for Checkbox buttons to match expansions, too lazy to mess with XAML

        string[] botConfFile;

        List<ConfigItem> configItems = new List<ConfigItem>();

        List<string> existingLines = new List<string>();

        private List<ConfigItem> LoadConfigFile(string filePath)
        {
            configItems.Clear();

            // Read in existing configuration data from file
            existingLines = File.ReadAllLines(botConfFile[0]).ToList();
            //if (File.Exists(botConfFile[0]))
            //{
            //    existingLines = File.ReadAllLines(botConfFile[0]).ToList();
            //}

            var lines = File.ReadAllLines(filePath);

            bool isEnabled;

            foreach (var line in lines)
            {
                isEnabled = true;

                if (line.Contains("# AiPlayerbot") || line.Contains("#AiPlayerbot") || line.Contains("# "))
                {
                    isEnabled = false;
                }
                if (line.Contains("<=") || line.Contains("=>"))
                {
                    continue;
                }

                var parts = line.Split('=');

                if (parts.Length == 2)
                {
                    var key = parts[0].Trim();
                    var value = parts[1].Trim();

                    var configItem = new ConfigItem { Key = key, Value = value, IsEnabled = isEnabled };
                    configItems.Add(configItem);
                }
            }

            return configItems;
        }

        private void CompareToGit()
        {
            string vanilla = "https://raw.githubusercontent.com/celguar/mangosbot-bots/master/playerbot/aiplayerbot.conf.dist.in";
            string tbc = "https://raw.githubusercontent.com/celguar/mangosbot-bots/master/playerbot/aiplayerbot.conf.dist.in.tbc";
            string wotlk = "https://raw.githubusercontent.com/celguar/mangosbot-bots/master/playerbot/aiplayerbot.conf.dist.in.wotlk";
            // Download the text from the remote URL
            var client = new WebClient();
            var remoteData = client.DownloadString(vanilla);
            string currentXpackLink = vanilla;

            if (botConfFile[0].Contains("tbc"))
            {
                remoteData = client.DownloadString(tbc);
                currentXpackLink = tbc;
            }
            else if (botConfFile[0].Contains("wotlk"))
            {
                remoteData = client.DownloadString(wotlk);
                currentXpackLink = wotlk;
            }

            var remoteLines = remoteData.Split('\n').ToList();

            if (botConfFile[0].Contains("aiplayerbot") && botConfFile[0].Contains("vanilla") || botConfFile[0].Contains("aiplayerbot") && botConfFile[0].Contains("tbc") || botConfFile[0].Contains("aiplayerbot") && botConfFile[0].Contains("wotlk"))
            {
                // Check for new lines in the remote data
                var newLines = remoteLines.Except(existingLines);
                var newLinesCount = newLines.Count();
                var messageBoxStandardWindow = MessageBox.Avalonia.MessageBoxManager.GetMessageBoxStandardWindow("Config Updates", $"There are {newLinesCount} updates/new lines for the config\nGet them from\n{currentXpackLink}\n");
                messageBoxStandardWindow.Show();
            }

            //--------Unfinished func - add new lines from Git to current config(probably shouldnt be here but it is what it is)----------
            // Add any new lines to the existing configuration data
            //existingLines.AddRange(newLines);
        }

        private void ReloadGrid()
        {
            LoadConfigFile(botConfFile[0]);

            ConfigDataGrid.Children.Clear();
            ConfigDataGrid.RowDefinitions.Clear();

            for (int i = 0; i < configItems.Count; i++)
            {
                var keyTextBlock = new TextBlock { Text = configItems[i].Key };
                var valueTextBox = new TextBox { Text = configItems[i].Value };
                var isEnabledCheckBox = new CheckBox { IsChecked = configItems[i].IsEnabled };


                ConfigDataGrid.RowDefinitions.Add(new RowDefinition());

                Grid.SetRow(keyTextBlock, i);
                Grid.SetColumn(keyTextBlock, 0);

                Grid.SetRow(valueTextBox, i);
                Grid.SetColumn(valueTextBox, 1);

                Grid.SetRow(isEnabledCheckBox, i);
                Grid.SetColumn(isEnabledCheckBox, 2);

                ConfigDataGrid.Children.Add(keyTextBlock);
                ConfigDataGrid.Children.Add(valueTextBox);
                ConfigDataGrid.Children.Add(isEnabledCheckBox);
            }
        }

        private void SetTheme()
        {
            var assets = AvaloniaLocator.Current.GetService<IAssetLoader>();
            ImageBrush imageBrush = new ImageBrush();

            if (botConfFile != null)
            {
                if (botConfFile[0].Contains("vanilla"))
                {
                    imageBrush.Source = new Bitmap(assets.Open(new Uri("avares://BotConfigurator/Resource/DruidResto.jpg")));
                    imageBrush.Opacity = 0.6;
                    imageBrush.Stretch = Stretch.UniformToFill;
                    this.Background = imageBrush;
                    ExpansionTxt.Text = "Vanilla";
                    checkboxTheme = Colors.Yellow;
                }

                else if (botConfFile[0].Contains("tbc"))
                {
                    imageBrush.Source = new Bitmap(assets.Open(new Uri("avares://BotConfigurator/Resource/DemonHunterHavoc.jpg")));
                    imageBrush.Opacity = 0.6;
                    imageBrush.Stretch = Stretch.UniformToFill;
                    this.Background = imageBrush;
                    ExpansionTxt.Text = "The Burning Crusade";
                    checkboxTheme = Colors.Green;
                }

                else if (botConfFile[0].Contains("wotlk"))
                {
                    imageBrush.Source = new Bitmap(assets.Open(new Uri("avares://BotConfigurator/Resource/DkFrost.jpg")));
                    imageBrush.Opacity = 0.6;
                    imageBrush.Stretch = Stretch.UniformToFill;
                    this.Background = imageBrush;
                    ExpansionTxt.Text = "Wrath of the Lich King";
                    checkboxTheme = Colors.Blue;
                }

                else
                {
                    imageBrush.Source = new Bitmap(assets.Open(new Uri("avares://BotConfigurator/Resource/WarriorFury.jpg")));
                    imageBrush.Opacity = 0.6;
                    imageBrush.Stretch = Stretch.UniformToFill;
                    this.Background = imageBrush;
                    ExpansionTxt.Text = "";
                    checkboxTheme = Colors.Red;
                }
            }
        }

        public async void OpenConfigBtn_click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog
            {
                Title = "Open Bot .conf file",
                /*CHANGE THIS BACK*/ InitialDirectory = "C:\\Users\\todor\\OneDrive\\Work\\source\\repos\\#RND\\BotConfigTest\\",//AppDomain.CurrentDomain.BaseDirectory,
                Filters = new List<FileDialogFilter> { new FileDialogFilter { Name = "Conf files", Extensions = new List<string> { "conf" } } }
            };

            var result = await ofd.ShowAsync(this);

            if (result != null)
            {
                botConfFile = result;
                SetTheme();
                var configItems = LoadConfigFile(result[0]);

                ConfigDataGrid.Children.Clear();
                ConfigDataGrid.RowDefinitions.Clear();

                for (int i = 0; i < configItems.Count; i++)
                {
                    var keyTextBlock = new TextBlock { Text = configItems[i].Key };
                    var valueTextBox = new TextBox { Text = configItems[i].Value };
                    var checkBox = new CheckBox { IsChecked = configItems[i].IsEnabled };

                    ConfigDataGrid.RowDefinitions.Add(new RowDefinition());

                    Grid.SetRow(keyTextBlock, i);
                    Grid.SetColumn(keyTextBlock, 0);

                    Grid.SetRow(valueTextBox, i);
                    Grid.SetColumn(valueTextBox, 1);

                    Grid.SetRow(checkBox, i);
                    Grid.SetColumn(checkBox, 2);

                    ConfigDataGrid.Children.Add(keyTextBlock);
                    ConfigDataGrid.Children.Add(valueTextBox);
                    ConfigDataGrid.Children.Add(checkBox);
                }

                OpenConfigTxt.IsVisible = false;
                SaveConfigBtn.IsVisible = true;
                CompareToGit();
            }
        }

        private void SaveConfigBtn_click(object sender, RoutedEventArgs e)
        {
            var lines = new List<string>();

            foreach (var child in ConfigDataGrid.Children)
            {
                if (child is TextBlock keyTextBlock)
                {
                    var valueTextBox = ConfigDataGrid.Children
                        .OfType<TextBox>()
                        .FirstOrDefault(tb => Grid.GetRow(tb) == Grid.GetRow(keyTextBlock));

                    var isEnabledCheckBox = ConfigDataGrid.Children
                        .OfType<CheckBox>()
                        .FirstOrDefault(cb => Grid.GetRow(cb) == Grid.GetRow(keyTextBlock));

                    var isEnabled = isEnabledCheckBox.IsChecked ?? true;

                    // Check if this key already exists in the configuration file
                    //var existingLine = existingLines.FirstOrDefault(line => line.IndexOf($"{keyTextBlock.Text}", StringComparison.OrdinalIgnoreCase) != -1);
                    var existingLine = existingLines.FirstOrDefault(line => line.Contains($"{keyTextBlock.Text}"));

                    if (existingLine != null && existingLine.Contains('='))
                    {
                        // If the key already exists, update the value
                        var existingValue = existingLine.Split('=')[1];
                        var lineToAdd = $"{(isEnabled ? keyTextBlock.Text + " = " + valueTextBox.Text : "# " + (keyTextBlock.Text + " = " + valueTextBox.Text))}";
                        if (existingLine.Trim().StartsWith("#") && !isEnabled)
                        {
                            // If the existing line already starts with '#', update the line without adding '#'
                            existingLines[existingLines.IndexOf(existingLine)] = $"{keyTextBlock.Text} = {valueTextBox.Text}";
                        }

                        else if (existingLine.Trim().StartsWith("#") && isEnabled)
                        {
                            // If the existing line already starts with '#', update the line removing '#'
                            var trimmedText = keyTextBlock.Text.Trim('#');
                            existingLines[existingLines.IndexOf(existingLine)] = $"{trimmedText} = {valueTextBox.Text}";
                        }

                        else
                        {
                            // Add the line with or without '#'
                            existingLines[existingLines.IndexOf(existingLine)] = lineToAdd;
                        }
                    }
                }
            }

            // Add any new lines to the existing configuration data
            existingLines.AddRange(lines);

            // Write the updated configuration data back to the file
            File.WriteAllLines(botConfFile[0], existingLines);

            SaveSuccessTxt.Text = "Config saved successfully!";

            // Set a timer to clear the text after 5 seconds
            var timer = new System.Timers.Timer(5000);
            timer.Elapsed += (s, args) =>
            {
                Dispatcher.UIThread.InvokeAsync(new Action(() => {
                    SaveSuccessTxt.Text = "";
                }));
                timer.Dispose(); 
            };
            timer.Start();

            ReloadGrid();
        }
    }
}