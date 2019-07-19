using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;

namespace ConsoleToolsLibrary
{
    public abstract class Menu
    {
        public readonly List<MenuThing> MenuGlobal = new List<MenuThing>();
        /// <summary>
        /// Returns LINQ query that finds all menu items in <see cref="MenuGlobal"/>.
        /// </summary>
        public IEnumerable<MenuThing> MenuItemsQuery
        {
            get
            {
                return
                from thing in MenuGlobal
                where thing is MenuItem
                select thing;
            }
        }
        /// <summary>
        /// Returns LINQ query that finds all menu items in <see cref="MenuGlobal"/>.
        /// </summary>
        public IEnumerable<MenuThing> MenuBarsQuery
        {
            get
            {
                return
                from thing in MenuGlobal
                where thing is MenuBar
                select thing;
            }
        }

        /// <summary>
        /// Background color of the 'Menu' object.
        /// </summary>
        private ConsoleColor MenuBackgroundColor = ConsoleColor.Black;
        /// <summary>
        /// Foreground (text) color of the 'Menu' object.
        /// </summary>
        private ConsoleColor MenuForegroundColor = ConsoleColor.White;

        /// <summary>
        /// 0 argument constructor.
        /// </summary>
        public Menu() {
            /* All Other Constructors Must Be Defined In Inheriting Class */
            /* ########################################################## */
        }

        /// <summary>
        /// Constructs a 'Menu' object with the colors of your choosing.
        /// </summary>
        /// <param name="menuBackgroundColor">Background color of the menu</param>
        /// <param name="menuForegroundColor">Foreground color of the menu</param>
        public Menu(ConsoleColor menuBackgroundColor, ConsoleColor menuForegroundColor)
        {
            MenuBackgroundColor = menuBackgroundColor;
            MenuForegroundColor = menuForegroundColor;
        }

        /// <summary>
        /// Sets the 'Menu' object's background color.
        /// </summary>
        /// <param name="menuBackgroundColor">New background color of menu</param>
        public void SetMenuBackgroundColor(ConsoleColor menuBackgroundColor)
        {
            Console.BackgroundColor = menuBackgroundColor;
        }

        /// <summary>
        /// Sets the 'Menu' object's foreground color. (Note: foreground color = text color)
        /// </summary>
        /// <param name="menuForegroundColor">New foreground color of menu</param>
        public void SetMenuForegroundColor(ConsoleColor menuForegroundColor)
        {
            Console.ForegroundColor = menuForegroundColor;
        }

        /// <summary>
        /// Adds a menu item to the 'Menu' object and associates an action with that item. Both the menu item and
        /// the associated action are placed at the end of each's dedicated list.
        /// </summary>
        /// <param name="label">Label of menu item</param>
        /// <param name="action">Method or delegate executed when menu item is activated</param>
        public Menu AddItem(string label, Action action = null)
        {
            MenuGlobal.Add(new MenuItem(label, action));
            return this;
        }

        /// <summary>
        /// Adds menu item at a specific index to the 'Menu' object and associates an action with that choice. The associated action
        /// is placed at the same index, but in the 'ActionList' list (contains all actions in a 'Menu' object).
        /// </summary>
        /// <param name="index">Index where menu item is added</param>
        /// <param name="label">Label of menu item inserted</param>
        /// <param name="action">Method or delegate executed when menu item is activated</param>
        public Menu Insert(int index, string label, Action action)
        {
            if (index > MenuGlobal.Count && index >= 0)
                MenuGlobal.Insert(index, new MenuItem(label, action));
            return this;
        }

        /// <summary>
        /// Adds a visual separator for menu items after the last menu item added.
        /// </summary>
        /// <example>
        /// <code>
        /// Menu menu1 = new Menu();
        /// menu1
        ///     .AddItem(
        ///         label:"menu item 1")
        ///     .AddItem(
        ///         label:"menu item 2",
        ///         action: ()=>{})
        ///     .AddBar()
        ///     .AddItem(
        ///         label:"menu item 3",
        ///         action: ()=>{ /* item action */ });
        /// </code>
        /// Produces the following menu:
        /// ______________________________
        /// |--> menu item 1              |
        /// |     menu item 2             |
        /// |----------------------       |
        /// |     menu item 3             |
        /// |_____________________________|
        /// </example>
        /// <returns></returns>
        public Menu AddBar()
        {
            MenuGlobal.Add(
                new MenuBar(MenuBar.DefaultBar)
                );
            return this;
        }

        /// <summary>
        /// Adds a custom visual separator for menu items after the last menu item added.
        /// </summary>
        /// <code>
        /// Menu menu1 = new Menu();
        /// menu1
        ///     .AddItem(
        ///         label:"menu item 1")
        ///     .AddItem(
        ///         label:"menu item 2",
        ///         action: ()=>{})
        ///     .AddBar("######################")
        ///     .AddItem(
        ///         label:"menu item 3",
        ///         action: ()=>{ /* item action */ });
        /// </code>
        /// Produces the following menu:
        /// ______________________________
        /// |--> menu item 1              |
        /// |     menu item 2             |
        /// |######################       |
        /// |     menu item 3             |
        /// |_____________________________|
        /// </example>
        /// <param name="customBar">Visual separator added to menu</param>
        /// <returns></returns>
        public Menu AddBar(string customBar)
        {
            MenuGlobal.Add(
                new MenuBar(customBar)
                );
            return this;
        }

        /// <summary>
        /// Sets the default bar used to visually separate groups of menu items.
        /// </summary>
        /// <param name="bar">Bar used as visual separator</param>
        /// <returns></returns>
        public Menu SetDefaultBar(string bar)
        {
            MenuBar.DefaultBar = bar;
            return this;
        }

        /// <summary>
        /// Removes the last type of menu thing (i.e. MenuThing, MenuItem, MenuBar) added to the menu.
        /// </summary>
        /// <example>
        /// <code>
        /// MyMenu.RemoveLast(MenuThing) // where MenuThing is a Type, not an instance
        /// MyMenu.RemoveLast(MenuItem) // where MenuItem is a Type, not an instance
        /// MyMenu.RemoveLast(MenuBar) // where MenuBar is a Type, not an instance
        /// </code>
        /// </example>
        /// <returns></returns>
        public Menu RemoveLast(Type menuThingType)
        {
            //Stop if no such menu things in list
            if (MenuGlobal.Count == 0
                && (menuThingType.BaseType == typeof(MenuThing)
                || menuThingType == typeof(MenuThing)))
                return this;

            MenuGlobal.Remove(MenuGlobal.Last(thing => thing.GetType() == menuThingType));
            return this;
        }

        /// <summary>
        /// Removes a menu item at the requested index.
        /// </summary>
        /// <param name="index">Index of menu item to remove</param>
        public Menu Remove(int index)
        {
            if (index < MenuGlobal.Count && index >= 0)
                this.MenuGlobal.RemoveAt(index);
            return this;
        }

        public Menu Remove(MenuThing menuThing)
        {
            this.MenuGlobal.Remove(menuThing);
            return this;
        }

        /// <summary>
        /// Swaps location of 2 menu items. Nothing will happen
        /// if indexes are out of range or equal.
        /// </summary>
        /// <param name="menuThingIndex"></param>
        /// <param name="newMenuThingIndex"></param>
        public Menu Swap(int menuThingIndex, int newMenuThingIndex)
        {
            if (menuThingIndex < MenuGlobal.Count && menuThingIndex >= 0
                && newMenuThingIndex < MenuGlobal.Count && newMenuThingIndex >= 0
                && menuThingIndex != newMenuThingIndex)
            {
                MenuThing menuThing1 = MenuGlobal[menuThingIndex];
                MenuThing menuThing2 = MenuGlobal[newMenuThingIndex];
                MenuGlobal.Remove(menuThing1);
                MenuGlobal.Remove(menuThing2);
                MenuGlobal.Insert(newMenuThingIndex, menuThing1);
                MenuGlobal.Insert(menuThingIndex, menuThing2);
            }
            return this;
        }

        /// <summary>
        /// Swaps location of 2 menu items. Nothing will happen if
        /// 'MenuItem' objects are the same.
        /// </summary>
        /// <param name="menuThing1"></param>
        /// <param name="menuThing2"></param>
        /// <returns></returns>
        public Menu Swap(MenuThing menuThing1, MenuThing menuThing2)
        {
            Swap(
                MenuGlobal.IndexOf(menuThing1),
                MenuGlobal.IndexOf(menuThing2)
                );
            return this;
        }

        /// <summary>
        /// Displays all menu items and bars as defined by the 'Menu' object.
        /// However, this does not include the navigation prompt at the very bottom (if any).
        /// </summary>
        /// <remarks>
        /// Behavior must be implemented
        /// </remarks>
        protected abstract void WriteMenu();

        /// <summary>
        /// Prompts the user a series of navigation options. The prompt will take a certain action dependent on the user's choice.
        /// </summary>
        /// <remarks>
        /// The menu's user navigation is handled here. Behavior must be implemented
        /// </remarks>
        /// <seealso cref="RunMenu"/>
        protected abstract void PromptNav();

        /// <summary>
        /// Keeps the menu running as the user navigates through the menu/ menus
        /// </summary>
        private void RunMenu()
        {
            Console.CursorVisible = false;
            WriteMenu();
            PromptNav();
        }

        /// <summary>
        /// Console is cleared and the program is shut down.
        /// </summary>
        public static void Exit()
        {
            Console.Clear();
            System.Environment.Exit(0);
        }

        /// <summary>
        /// Sends user to a different 'Menu'. This is the recommended method to transverse between menus.
        /// Using this method makes the console cursor invisible. If one needs the cursor visible
        /// afterward, then it must be manually done.
        /// </summary>
        /// <param name="target">Menu user is sent to</param>
        public virtual void GoTo()
        {
            /* Override Code Here */
            /* ################## */
            Console.Clear();
            Console.BackgroundColor = this.MenuBackgroundColor;
            Console.ForegroundColor = this.MenuForegroundColor;
            this.RunMenu();
        }

        /// <summary>
        /// The console screen is cleared of the currently displayed menu, and 
        /// then the message is displayed in the destination menu's colors.
        /// After that, pressing any key will bring the user back to the menu ('Menu' object) from
        /// which the instance method was invoked from. 
        /// Using this method makes the console cursor invisible. If one needs the cursor visible
        /// afterward, then it must be manually done.
        /// </summary>
        /// <param name="target">Target menu user is sent to</param>
        /// <param name="message">Message displayed before user is sent to target</param>
        public virtual void GoTo(string message)
        {
            /* Override Code Here */
            /* ################## */
            Console.Clear();
            Console.BackgroundColor = this.MenuBackgroundColor;
            Console.ForegroundColor = this.MenuForegroundColor;
            Console.WriteLine(message);
            Console.ReadKey(true);
            this.RunMenu();
        }

        public abstract class MenuThing
        {
            /// <summary>
            /// Text of menu thing shown on the menu.
            /// </summary>
            public string Text = "";
            /// <summary>
            /// Action performed if menu thing is selected in the menu.
            /// </summary>
            public Action Action;
        }

        /// <summary>
        /// Represents a menu item shown in a selector menu.
        /// </summary>
        public class MenuItem : MenuThing
        {
            public MenuItem(string label, Action action)
            {
                Text = label;
                Action = action;
            }
        }

        public class MenuBar : MenuThing
        {
            private const string DEFAULT_BAR = "---------------------------";
            private const string DEFAULT_BAR_UTF8 = " ──────────────────────────────────";
            private static string _DefaultBarValue = DEFAULT_BAR;
            /// <summary>
            /// String used to visually separate menu items
            /// </summary>
            public static string DefaultBar
            {
                get
                {
                    if (Console.OutputEncoding.EncodingName == "Unicode (UTF-8)"
                        && _DefaultBarValue != DEFAULT_BAR)
                    {
                        return _DefaultBarValue;
                    }
                    else
                    {
                        _DefaultBarValue = DEFAULT_BAR_UTF8;
                        return _DefaultBarValue;
                    }
                }
                set
                {
                    _DefaultBarValue = value;
                }
            }

            public MenuBar(string bar)
            {
                Text = bar;
            }
        }
    }

    namespace MenuUtilities
    {
        public static class Settings
        {
            #if false
            /// <summary>
            /// Shows the user the old value of the setting in question and
            /// then prompts the user for the new value.
            /// </summary>
            /// <remarks>
            /// Doesn't work yet. Theres an absence of a base class used for .Net settings objects (i.e. Properties.SomeSettings.Setting)
            /// It's called "ApplicationSettingsBase". If found, replace "ConfigurationProperty" with it.
            /// </remarks>
            /// <param name="settingKey">Name of the setting that is prompted for change</param>
            public static void PromptSettingChange(this ConfigurationProperty settings,string settingKey)
            {
                try
                {
                    Console.WriteLine($"Setting: {settingKey}");
                    Console.WriteLine("Old Value: " + settings.[settingKey]  /*Properties.CommandLineToolsSettings.Default.PropertyValues[settingKey].PropertyValue */);
                    Console.Write("New Value: ");
                    settings[settingKey]. = Console.ReadLine(); /* Properties.CommandLineToolsSettings.Default.PropertyValues[settingKey].PropertyValue = Console.ReadLine(); */
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
            #endif
        }
    }
}
