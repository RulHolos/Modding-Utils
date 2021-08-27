using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Modding_Utils
{
    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            WorkingDirPath.Text = Properties.Settings.Default.WorkingDirPath;
            WhereIsFlagUsedInput.Text = "Please enter the ID of the flag";
            WhatLinksInput.Text = "Please enter the ID of the event (without leading zeros)";
        }

        private void WorkingDirPath_TextChanged(object sender, TextChangedEventArgs e)
        {
            Properties.Settings.Default.WorkingDirPath = WorkingDirPath.Text;
            Properties.Settings.Default.Save();
        }

        private void ReloadBtn_Click(object sender, RoutedEventArgs e)
        {
            Gconsole.Text = "";
            try
            {
                Utils.CheckWorkingDir();
            }
            catch (WorkingDirEmptyException)
            {
                MessageBox.Show("You haven't selected a working directory, you cannot do that.");
                return;
            }

            Utils.mentries.Clear();
            Utils.StoreMentries();
            Utils.StoreEventsPaths();

            Gconsole.Text += "Done.\n";
            Utils.loaded = true;
        }

        /// <summary>
        /// Deux output possibles. L'event est load via un autre event avec un 7, donc on va lire tout les events
        /// Sinon, il est load via un event de map, et dans ce cas là, on va lire tout les events de chaque map.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void WhatLinksInput_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter && Utils.loaded == true)
            {
                try
                {
                    Utils.CheckWorkingDir();
                }
                catch (WorkingDirEmptyException)
                {
                    MessageBox.Show("You haven't selected a working directory, you cannot do that.");
                    return;
                }

                string FinalText = "";
                int results = 0;

                // Lecture des events de maps
                foreach (Mentries mentrie in Utils.mentries)
                {
                    for (int i = 0; i < mentrie.entries.Count; i++)
                    {
                        if (mentrie.entries[i].event_arg == WhatLinksInput.Text)
                        {
                            if (mentrie.entries[i].event_arg != "0" &&
                                Convert.ToInt32(mentrie.entries[i].event_index) > 1 &&
                                Convert.ToInt32(mentrie.entries[i].event_index) < 255)
                            {
                                results++;
                                FinalText += $"Found at the map ID n°{mentrie.mapID} at the event index n°{mentrie.entries[i].event_index}.\n";
                            }
                        }
                    }
                }

                // Lecture des events appelés par d'autres events
                foreach (EventInfo evs in Utils.events)
                {
                    for (int i = 0; i < evs.lines.Length; i++)
                    {
                        string[] line = evs.lines[i].Split(',');
                        if (line[2] == $" {WhatLinksInput.Text}" && line[0] == "7")
                        {
                            results++;
                            FinalText += $"Called from {System.IO.Path.GetFileName(evs.path)}\n";
                        }
                    }
                }

                WhatLinksResult.Text = $"{results} result(s) found.\n{FinalText}";
            }
        }

        private void WhatLinksInput_GotFocus(object sender, RoutedEventArgs e)
        {
            if (WhatLinksInput.Text == "Please enter the ID of the event (without leading zeros)")
            {
                WhatLinksInput.Text = "";
            }
        }

        private void WhatLinksInput_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(WhatLinksInput.Text))
                WhatLinksInput.Text = "Please enter the ID of the event (without leading zeros)";
        }

        private void WhereIsFlagUsedInput_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter && Utils.loaded == true)
            {
                try
                {
                    Utils.CheckWorkingDir();
                }
                catch (WorkingDirEmptyException)
                {
                    MessageBox.Show("You haven't selected a working directory, you cannot do that.");
                    return;
                }

                int results = 0;
                string FinalText = "";

                // Lecture des events
                foreach (EventInfo evs in Utils.events)
                {
                    for (int i = 0; i < evs.lines.Length; i++)
                    {
                        string[] line = evs.lines[i].Split(',');
                        if (line[1] == $" {WhereIsFlagUsedInput.Text}" && line[0] == "6")
                        {
                            bool enabled = false;
                            if (line[2] == " 1")
                                enabled = true;
                            else
                                enabled = false;
                            results++;
                            string en = enabled ? "Enabled" : "Disabled";
                            FinalText += $"{en} in {System.IO.Path.GetFileName(evs.path)}\n";
                        }
                        else if (line[0] == "9" && line[1] == $" {WhereIsFlagUsedInput.Text}")
                        {
                            results++;
                            FinalText += $"Read from {System.IO.Path.GetFileName(evs.path)}\n";
                        }
                    }
                }

                WhereIsFlagUsedResult.Text = $"{results} result(s) found.\n{FinalText}";
            }
        }

        private void WhereIsFlagUsedInput_GotFocus(object sender, RoutedEventArgs e)
        {
            if (WhereIsFlagUsedInput.Text == "Please enter the ID of the flag")
            {
                WhereIsFlagUsedInput.Text = "";
            }
        }

        private void WhereIsFlagUsedInput_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(WhereIsFlagUsedInput.Text))
                WhereIsFlagUsedInput.Text = "Please enter the ID of the flag";
        }
    }
}
