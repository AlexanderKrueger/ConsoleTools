using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleToolsLibrary
{
    class SettingsMenu
    {
        private static void EditSettings()
        {
            ConsoleKey readKey;

            do
            {
                Console.WriteLine("=========================================");
                Console.WriteLine("=== CommandLineTools: Settings ==========");
                Console.WriteLine("=========================================");
                Console.WriteLine("[Press Number Key To Change Setting]");
                Console.WriteLine();
                Console.WriteLine("[0] - AuthorName = " + Properties.CommandLineToolsSettings.Default.AuthorName);
                Console.WriteLine("[1] - AuthorAlias1 = " + Properties.CommandLineToolsSettings.Default.AuthorAlias1);
                Console.WriteLine("[2] - AuthorAlias2 = " + Properties.CommandLineToolsSettings.Default.AuthorAlias2);
                Console.WriteLine("[3] - AuthorAlias3 = " + Properties.CommandLineToolsSettings.Default.AuthorAlias3);
                Console.WriteLine("[4] - AuthorAlias4 = " + Properties.CommandLineToolsSettings.Default.AuthorAlias4);
                Console.WriteLine("[5] - AuthorAlias5 = " + Properties.CommandLineToolsSettings.Default.AuthorAlias5);
                Console.WriteLine("[6] - AuthorAlias6 = " + Properties.CommandLineToolsSettings.Default.AuthorAlias6);
                Console.WriteLine("[7] - AuthorAlias7 = " + Properties.CommandLineToolsSettings.Default.AuthorAlias7);
                Console.WriteLine("[8] - AuthorAlias8 = " + Properties.CommandLineToolsSettings.Default.AuthorAlias8);
                Console.WriteLine("[9] - AuthorAlias9 = " + Properties.CommandLineToolsSettings.Default.AuthorAlias9);
                Console.WriteLine("******************************************");
                Console.WriteLine("[E] - Exit Settings");

                readKey = Console.ReadKey(true).Key;
                Console.Clear();

                switch (readKey)
                {
                    case ConsoleKey.D0:
                        Console.WriteLine("/* Notes:");
                        Console.WriteLine(" * Program looks for the users full name and its");
                        Console.WriteLine(" * smaller varients recorded in the command line tools");
                        Console.WriteLine(" * when it decides which to highlight");
                        Console.WriteLine(" *");
                        Console.WriteLine(" * In order to search for the smaller name varients");
                        Console.WriteLine(" * , it's recommended to use the largest version of");
                        Console.WriteLine(" * your name for this setting's value. ");
                        Console.WriteLine(" *");
                        Console.WriteLine(" * *** For Example *** ");
                        Console.WriteLine(" * (Min Accepted Name Format)");
                        Console.WriteLine(" *     MyOnlyName");
                        Console.WriteLine(" * (Okay)");
                        Console.WriteLine(" *     FirstName LastName");
                        Console.WriteLine(" * (Better)");
                        Console.WriteLine(" *     FirstName M. LastName");
                        Console.WriteLine(" * (Best)");
                        Console.WriteLine(" *     FirstName MiddleName LastName");
                        Console.WriteLine(" *");
                        Console.WriteLine(" * *** Remarks ***");
                        Console.WriteLine(" * 1# Searching using this setting can't be");
                        Console.WriteLine(" *    done with other name formats.");
                        Console.WriteLine(" *");
                        Console.WriteLine(" * 2# This setting is NOT required");
                        Console.WriteLine(" */");
                        Console.WriteLine();
                        PromptSettingChange("AuthorName");
                        break;
                    case ConsoleKey.D1:
                        PromptSettingChange("AuthorAlias1");
                        break;
                    case ConsoleKey.D2:
                        PromptSettingChange("AuthorAlias2");
                        break;
                    case ConsoleKey.D3:
                        PromptSettingChange("AuthorAlias3");
                        break;
                    case ConsoleKey.D4:
                        PromptSettingChange("AuthorAlias4");
                        break;
                    case ConsoleKey.D5:
                        PromptSettingChange("AuthorAlias5");
                        break;
                    case ConsoleKey.D6:
                        PromptSettingChange("AuthorAlias6");
                        break;
                    case ConsoleKey.D7:
                        PromptSettingChange("AuthorAlias7");
                        break;
                    case ConsoleKey.D8:
                        PromptSettingChange("AuthorAlias8");
                        break;
                    case ConsoleKey.D9:
                        PromptSettingChange("AuthorAlias9");
                        break;
                    case ConsoleKey.E:
                        Console.Clear();
                        Environment.Exit(0);
                        break;
                }

                Properties.CommandLineToolsSettings.Default.Save();
                Properties.CommandLineToolsSettings.Default.Reload();
            } while (true);
        }

        /// <summary>
        /// Shows the user the old value of the setting in question and
        /// then prompts the user for the new value.
        /// </summary>
        /// <param name="settingKey">Name of the setting that is prompted for change</param>
        private static void PromptSettingChange(string settingKey)
        {
            try
            {
                Console.WriteLine($"Setting: {settingKey}");
                Console.WriteLine("Old Value: " + Properties.CommandLineToolsSettings.Default.PropertyValues[settingKey].PropertyValue);
                Console.Write("New Value: ");
                Properties.CommandLineToolsSettings.Default.PropertyValues[settingKey].PropertyValue = Console.ReadLine();
                Console.Clear();
            }
            catch (Exception)
            {
                Console.WriteLine($"Sorry, setting {settingKey} either no longer exists");
                Console.WriteLine("or goes by a different name.");
                Console.WriteLine("[Press Any Key To Continue]");
                Console.ReadKey(true);
                Console.Clear();
            }
        }
    }
