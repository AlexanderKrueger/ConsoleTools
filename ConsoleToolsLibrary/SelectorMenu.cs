using System;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasierConsolePrograms
{
    /// <summary>
    /// 'SelectorMenu' class allows the creation of "selector menus". A selector menu
    /// lists its menu items vertically and a "selector" points to the
    /// menu item in focus. Whatever menu item is in focus, is the menu item selected
    /// when selecting is performed.
    /// Changing the focus up or down (menu navigation) is done via keyboard's arrow keys.
    /// Once the desired menu item is in focus, one can use the keyboard's "enter" key
    /// to select it. If selected, the menu item's code is executed.
    /// If navigation and selection aren't possible with the intended keyboard keys,
    /// for each, there is a fall-back key.
    /// 
    /// While the "selector menu" isn't quick as "push button" menus (press number --> action)
    /// , selector menus prevent accidental menu item selection
    /// using a "confirm selection philosophy" instead of "direct selection philosophy".
    /// 
    /// To start using a 'SelectorMenu':
    /// <code>
    /// //*** Define Menu Object ***
    /// SelectorMenu menu1 = new SelectorMenu();
    /// 
    /// //*** Fill Menu With Items ***
    /// menu1
    /// .AddMenuItem(
    ///     label: "item label 1",
    ///     action: ()=>{ /* item action */ }
    ///     )
    /// .AddMenuItem(
    ///     label: "item label 2",
    ///     action: ()=>{ /* item action */ }
    ///     )
    /// .AddMenuItem(
    ///     label: "item label 3",
    ///     action: ()=>{ /* item action */ }
    ///     );
    ///     
    /// //*** Send User To Menu ***
    /// menu1.GoTo();
    /// 
    /// //Menu shown as:
    /// //______________________________
    /// //|  -->  item label 1         |
    /// //|     item label 2           |
    /// //|     item label 3           |
    /// //|____________________________|
    /// </code>
    /// 
    /// If menu items need to be visually grouped, consider adding
    /// bars:
    /// <code>
    /// //*** Define Menu Object ***
    /// SelectorMenu menu1 = new SelectorMenu();
    /// 
    /// //*** Fill Menu With Items ***
    /// menu1
    /// .AddMenuItem(
    ///     label: "item label 1",
    ///     action: ()=>{ /* item action */ }
    ///     )
    /// .AddMenuItem(
    ///     label: "item label 2",
    ///     action: ()=>{ /* item action */ }
    ///     )
    /// .AddBar()
    /// .AddMenuItem(
    ///     label: "item label 3",
    ///     action: ()=>{ /* item action */ }
    ///     );
    ///     
    /// //*** Send User To Menu ***
    /// menu1.GoTo();
    /// 
    /// //Menu shown as:
    /// //______________________________
    /// //|  -->  item label 1         |
    /// //|     item label 2           |
    /// //| ------------------------   |
    /// //|     item label 3           |
    /// //|____________________________|
    /// </code>
    /// 
    /// If you don't like the default selector (an actual arrow if console encoding set to UTF8)
    /// ,then a custom selector can be defined:
    /// <code>
    /// //*** Define Menu Object ***
    /// //Quick Way
    /// //    SelectorMenu menu1 = new SelectorMenu("  <->  ");
    /// //Non-Object Way (not the only non-object way)
    /// //    SelectorMenu menu1 = new SelectorMenu("  <->  ", ConsoleColor.Red, ConsoleColor.Black);
    /// //Object way (many optional parameters not included)
    ///       SelectorMenu menu1 = new SelectorMenu(
    ///           new Selector("  <->  ",
    ///           ConsoleColor.Red,
    ///           ConsoleColor.Black);
    ///     
    /// //*** Fill Menu With Items ***
    /// menu1
    /// .AddMenuItem(
    ///     label: "item label 1",
    ///     action: ()=>{ /* item action */ }
    ///     )
    /// .AddMenuItem(
    ///     label: "item label 2",
    ///     action: ()=>{ /* item action */ }
    ///     )
    /// .AddMenuItem(
    ///     label: "item label 3",
    ///     action: ()=>{ /* item action */ }
    ///     );
    ///     
    /// //*** Send User To Menu ***
    /// menu1.GoTo();
    /// 
    /// //Menu shown as:
    /// //______________________________
    /// //|  <->  item label 1         |
    /// //|     item label 2           |
    /// //|     item label 3           |
    /// //|____________________________|
    /// </code>
    /// </summary>
    class SelectorMenu
    {
        /// <summary>
        /// String acts as a pointer inside a displayed 'SelectorMenu' object. Whatever option is being pointed to by the selector
        /// is currently selected menu item. 
        /// By design, each 'SelectorMenu' object may have it's own unique 'Selector', however, a different selector must be
        /// defined during object construction or afterward changed by other means.
        /// </summary>
        /// <remarks>
        /// The variable is called the 'Selector' since it acts as the user's selector (the thing that enables selection visually) on a menu.
        /// </remarks>
        public Selector Selector = new Selector();
        private const string DEFAULT_BAR      = "---------------------------";
        private const string DEFAULT_BAR_UTF8 = "───────────────────────────";
        private string _DefaultBarValue = DEFAULT_BAR;
        /// <summary>
        /// String used to visually separate menu items
        /// </summary>
        private string DefaultBar
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
            set {
                _DefaultBarValue = value;
            }
        }

        /// <summary>
        /// Index of menu item the selector is currently on.
        /// <example>
        /// If Selector is on the first choice then it's position equals 0 (the index of the choice)
        /// </example>
        private int SelectorPosition = 0;
        /// <summary>
        /// Background color of the 'SelectorMenu' object.
        /// </summary>
        private ConsoleColor MenuBackgroundColor = ConsoleColor.Black;
        /// <summary>
        /// Foreground (text) color of the 'SelectorMenu' object.
        /// </summary>
        private ConsoleColor MenuForegroundColor = ConsoleColor.White;
        /// <summary>
        /// Contains all the menu items -- and bars -- displayed on
        /// the menu.
        /// </summary>
        private List<MenuItem> MenuItems = new List<MenuItem>();

        /// <summary>
        /// 0 argument constructor.
        /// </summary>
        public SelectorMenu(){ }

        /// <summary>
        /// Constructs a 'SelectorMenu' object using a custom selector.
        /// </summary>
        /// <param name="selector">String used as the menu's selector</param>
        public SelectorMenu(string selector)
        {
            Selector = new Selector(selector);
        }

        /// <summary>
        /// Constructs a 'SelectorMenu' object using a custom selector.
        /// </summary>
        /// <seealso cref="EasierConsolePrograms.Selector"/>
        /// <param name="selector">Selector object used to define the menu's selector</param>
        public SelectorMenu(Selector selector)
        {
            Selector = selector;
        }

        /// <summary>
        /// Constructs a 'SelectorMenu' object with the colors of your choosing.
        /// </summary>
        /// <param name="menuBackgroundColor">Background color of the menu</param>
        /// <param name="menuForegroundColor">Foreground color of the menu</param>
        public SelectorMenu(ConsoleColor menuBackgroundColor, ConsoleColor menuForegroundColor)
        {
            MenuBackgroundColor = menuBackgroundColor;
            MenuForegroundColor = menuForegroundColor;
        }

        /// <summary>
        /// Constructs a 'SelectorMenu' object with the selector and colors of your choosing.
        /// </summary>
        /// <seealso cref="EasierConsolePrograms.Selector"/>
        /// <param name="selector">Selector object used to define the menu's selector</param>
        /// <param name="menuBackgroundColor">Background color of the menu</param>
        /// <param name="menuForegroundColor">Foreground color of the menu</param>
        public SelectorMenu(Selector selector, ConsoleColor menuBackgroundColor, ConsoleColor menuForegroundColor)
        {
            Selector = selector;
            MenuBackgroundColor = menuBackgroundColor;
            MenuForegroundColor = menuForegroundColor;
        }

        public SelectorMenu(string selector, ConsoleColor menuBackgroundColor, ConsoleColor menuForegroundColor)
        {
            Selector = new Selector(selector);
            MenuBackgroundColor = menuBackgroundColor;
            MenuForegroundColor = menuForegroundColor;
        }

        /// <summary>
        /// Sets the 'SelectorMenu' object's background color.
        /// </summary>
        /// <param name="menuBackgroundColor">New background color of menu</param>
        public void SetMenuBackgroundColor(ConsoleColor menuBackgroundColor)
        {
            Console.BackgroundColor = menuBackgroundColor;
        }

        /// <summary>
        /// Sets the 'SelectorMenu' object's foreground color. (Note: foreground color = text color)
        /// </summary>
        /// <param name="menuForegroundColor">New foreground color of menu</param>
        public void SetMenuForegroundColor(ConsoleColor menuForegroundColor)
        {
            Console.ForegroundColor = menuForegroundColor;
        }



        /// <summary>
        /// Sets menu's placeholder string that's present where the selector is not.
        /// </summary>
        /// <param name="selectorPlaceholder">String used as selector's placeholder</param>
        public void SetSelectorPlaceholder(string selectorPlaceholder)
        {
            Selector.SelectorStrPlaceholder = selectorPlaceholder;
        }

        /// <summary>
        /// Adds a choice to the 'SelectorMenu' object and associates an action with that choice. Both the choice and
        /// the associated action are placed at the end of each's dedicated list
        /// </summary>
        /// <param name="label">Label of menu item</param>
        /// <param name="action">Method or delegate executed when menu item is activated</param>
        public SelectorMenu AddMenuItem(string label, Action action)
        {
            this.MenuItems.Add(new MenuItem(label, action));
            return this;
        }

        /// <summary>
        /// Adds menu item at a specific index to the 'SelectorMenu' object and associates an action with that choice. The associated action
        /// is placed at the same index, but in the 'ActionList' list (contains all actions in a 'SelectorMenu' object).
        /// </summary>
        /// <param name="index">Index where menu item is added</param>
        /// <param name="label">Label of menu item inserted</param>
        /// <param name="action">Method or delegate executed when menu item is activated</param>
        public SelectorMenu InsertMenuItemAt(int index, string label, Action action)
        {
            if(index > this.MenuItems.Count && index >= 0)
                this.MenuItems.Insert(index, new MenuItem(label, action));
            return this;
        }

        /// <summary>
        /// Adds a visual separator for menu items after the last menu item added.
        /// </summary>
        /// <example>
        /// <code>
        /// SelectorMenu menu1 = new SelectorMenu();
        /// menu1.AddMenuItem(
        ///     label:"menu item 1",
        ///     action: ()=>{}
        ///     );
        /// menu1.AddMenuItem(
        ///     label:"menu item 2",
        ///     action: ()=>{}
        ///     );
        /// menu1.AddBar();
        /// menu1.AddMenuItem(
        ///     label:"menu item 3",
        ///     action: ()=>{}
        ///     );
        /// </code>
        /// Produces the following menu:
        /// ______________________________
        /// |--> menu item 1              |
        /// |menu item 2                  |
        /// |----------------------       |
        /// |menu item 3                  |
        /// |_____________________________|
        /// </example>
        /// <returns></returns>
        public SelectorMenu AddBar()
        {
            this.MenuItems.Add(
                new MenuItem(
                    DefaultBar,
                    delegate () { },
                    isTrueMenuItem: false)
                );
            return this;
        }

        /// <summary>
        /// Adds a custom visual separator for menu items after the last menu item added.
        /// </summary>
        ///         /// <code>
        /// SelectorMenu menu1 = new SelectorMenu();
        /// menu1.AddMenuItem(
        ///     label:"menu item 1",
        ///     action: ()=>{}
        ///     );
        /// menu1.AddMenuItem(
        ///     label:"menu item 2",
        ///     action: ()=>{}
        ///     );
        /// menu1.AddBar("######################");
        /// menu1.AddMenuItem(
        ///     label:"menu item 3",
        ///     action: ()=>{}
        ///     );
        /// </code>
        /// Produces the following menu:
        /// ______________________________
        /// |--> menu item 1              |
        /// |menu item 2                  |
        /// |######################       |
        /// |menu item 3                  |
        /// |_____________________________|
        /// </example>
        /// <param name="customBar">Visual separator added to menu</param>
        /// <returns></returns>
        public SelectorMenu AddBar(string customBar)
        {
            this.MenuItems.Add(
                new MenuItem(
                    customBar,
                    delegate () { },
                    isTrueMenuItem: false)
                );
            return this;
        }

        /// <summary>
        /// Sets the default bar used to visually separate groups of menu items.
        /// </summary>
        /// <param name="bar">Bar used as visual separator</param>
        /// <returns></returns>
        public SelectorMenu SetDefaultBar(string bar)
        {
            DefaultBar = bar;
            return this;
        }

        /// <summary>
        /// Removes the last created menu item.
        /// </summary>
        public SelectorMenu RemoveLastMenuItem()
        {
            //skip logic if no items in list
            if (MenuItems.Count == 0)
                return this;

            //initialize local variables
            int i = MenuItems.Count - 1;

            //begin list back-tracking
            while (i > -1)
            {
                //end list back-tracking if last such is found
                if (MenuItems[i].IsTrueMenuItem)
                    break;
                //else continue list back-tracking
                else
                    i--;
            }             
            
            //remove such item if 1 is found
            if(i != -1)
                MenuItems.RemoveAt(i);

            return this;
        }

        /// <summary>
        /// Removes the last bar added to the menu.
        /// </summary>
        /// <returns></returns>
        public SelectorMenu RemoveLastBar()
        {
            //skip logic if no items in list
            if (MenuItems.Count == 0)
                return this;

            //initialize local variables
            int i = MenuItems.Count - 1;

            //begin list back-tracking
            while (i > -1)
            {
                //end list back-tracking if last such is found
                if (!MenuItems[i].IsTrueMenuItem)
                    break;
                //else continue list back-tracking
                else
                    i--;
            }

            //remove such item if 1 is found
            if (i != -1)
                MenuItems.RemoveAt(i);

            return this;
        }

        /// <summary>
        /// Removes a menu item at the requested index.
        /// </summary>
        /// <param name="index">Index of menu item to remove</param>
        public SelectorMenu RemoveMenuItemAt(int index)
        {
            if(index < this.MenuItems.Count && index >= 0)
                MenuItems.RemoveAt(index);
            return this;
        }

        /// <summary>
        /// Swaps location of 2 menu items. Nothing will happen
        /// if indexes are out of range or equal.
        /// </summary>
        /// <param name="menuItemIndex"></param>
        /// <param name="newMenuItemIndex"></param>
        public SelectorMenu SwapMenuItems(int menuItemIndex, int newMenuItemIndex)
        {
            if (menuItemIndex < this.MenuItems.Count && menuItemIndex >= 0
                && newMenuItemIndex < this.MenuItems.Count && newMenuItemIndex >= 0
                && menuItemIndex != newMenuItemIndex)
            {
                MenuItem menuItem1 = MenuItems[menuItemIndex];
                MenuItem menuItem2 = MenuItems[newMenuItemIndex];
                MenuItems.Remove(menuItem1);
                MenuItems.Remove(menuItem2);
                MenuItems.Insert(newMenuItemIndex, menuItem1);
                MenuItems.Insert(menuItemIndex, menuItem2);
            }
            return this;
        }

        /// <summary>
        /// Swaps location of 2 menu items. Nothing will happen if
        /// 'MenuItem' objects are the same.
        /// </summary>
        /// <param name="menu1"></param>
        /// <param name="menu2"></param>
        /// <returns></returns>
        public SelectorMenu SwapMenuItems(MenuItem menu1, MenuItem menu2)
        {
            int menu1Index = this.MenuItems.IndexOf(menu1);
            int menu2Index = this.MenuItems.IndexOf(menu2);
            SwapMenuItems(menu1Index, menu2Index);
            return this;
        }

        /// <summary>
        /// Displays all menu items and bars as defined by the 'SelectorMenu' object.
        /// However, this does not include the navigation prompt at the very bottom.
        /// </summary>
        private void WriteMenu()
        {
            Console.Clear();

            int iterator = 0;
            foreach (var menuItem in MenuItems)
            {
                //If selector at position of menu item being written...
                if(iterator == SelectorPosition && menuItem.IsTrueMenuItem)
                {
                    //If selector highlighting is on,
                    //then write selector highlighted
                    if(Selector.IsHighlighted)
                    {
                        Console.BackgroundColor = Selector.BackgroundColor;
                        Console.ForegroundColor = Selector.ForegroundColor;
                        Console.Write(Selector.SelectorStr);
                        Console.BackgroundColor = this.MenuBackgroundColor;
                        Console.ForegroundColor = this.MenuForegroundColor;
                    }
                    //Else, write selector without highlighting
                    else { Console.Write(Selector.SelectorStr); }
                    //If selected highlighting is on,
                    //then write selected highlighted
                    if (Selector.IsTargetHighlighted)
                    {
                        Console.BackgroundColor = Selector.BackgroundColorSelected;
                        Console.ForegroundColor = Selector.ForegroundColorSelected;
                        Console.WriteLine(menuItem.Label);
                        Console.BackgroundColor = this.MenuBackgroundColor;
                        Console.ForegroundColor = this.MenuForegroundColor;
                    }
                    //Else, write label without highlighting
                    else { Console.WriteLine(menuItem.Label); }
                }
                //Else if selector not on menu item, write menu item without selector
                else if (menuItem.IsTrueMenuItem)
                {
                    Console.WriteLine(Selector.SelectorStrPlaceholder + menuItem.Label);
                }

                //If menu item is bar...
                if (menuItem.IsTrueMenuItem == false)
                    Console.WriteLine(menuItem.Label);

                iterator++;
            }
        }

        /// <summary>
        /// Prompts the user a series of navigation options. The prompt will take a certain action dependent on the user's choice.
        /// </summary>
        /// <remarks>
        /// The menu's selector position is moved here.
        /// </remarks>
        /// <seealso cref="RunMenu"/>
        private void PromptNav()
        {
            int lastMenuItemIndex = MenuItems.Count - 1;

            if (Console.OutputEncoding.EncodingName == "Unicode (UTF-8)")
            {
                Console.WriteLine("━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━");
                Console.WriteLine(" ↑/ U - Up, ↓/ D - Down, ←╯/ S - Select");
            }
            else
            {
                Console.WriteLine("----------------------------------------------------------------");
                Console.WriteLine(" Up-Arrow/ U - Up, Down-Arrow/ D - Down, Enter/ S - Select");
            }
            while (true) {
                ConsoleKeyInfo KeyPressed = Console.ReadKey(true);
                if (KeyPressed.KeyChar == 'U' || KeyPressed.KeyChar == 'u' || KeyPressed.Key == ConsoleKey.UpArrow)
                {
                    //If doable, move selector up by 1
                    if (SelectorPosition > 0)
                        SelectorPosition--;
                    else
                        continue;
                    //If selector is pointing at a non-menu-item...
                    if (MenuItems[SelectorPosition].IsTrueMenuItem == false)
                    {
                        //While selector position hits bar, keep moving selector up till it hits a true menu item
                        while (MenuItems[SelectorPosition].IsTrueMenuItem == false
                            && SelectorPosition > 0)
                            SelectorPosition--;
                        //If only bars encountered, move selector to the first menu item
                        if (MenuItems[SelectorPosition].IsTrueMenuItem == false)
                            SelectorPosition = MenuItems.FindIndex(item => item.IsTrueMenuItem == true);
                        //If no true menu items can be found, set selector position to 0
                        if (SelectorPosition == -1)
                            SelectorPosition = 0;
                    }
                    //----------------------------------------------------------------
                    goto SelectorMoved;
                }
                else if (KeyPressed.KeyChar == 'D' || KeyPressed.KeyChar == 'd' || KeyPressed.Key == ConsoleKey.DownArrow)
                {
                    //If doable, move selector down by 1
                    if (SelectorPosition < lastMenuItemIndex)
                        SelectorPosition++;
                    else
                        continue;
                    //If selector is pointing at a non-menu-item...
                    if (MenuItems[SelectorPosition].IsTrueMenuItem == false)
                    {
                        //While selector position hits bar, keep moving selector down till it hits a true menu item
                        while (MenuItems[SelectorPosition].IsTrueMenuItem == false
                            && SelectorPosition < lastMenuItemIndex)
                            SelectorPosition++;
                        //If only bars encountered, move selector to the first menu item
                        if (MenuItems[SelectorPosition].IsTrueMenuItem == false)
                            SelectorPosition = MenuItems.FindLastIndex(item => item.IsTrueMenuItem == true);
                        //If no true menu items can be found, set selector position to 0
                        if (MenuItems[SelectorPosition].IsTrueMenuItem == false)
                            SelectorPosition = 0;
                    }
                    //-----------------------------------------------------------------
                    goto SelectorMoved;
                }
                else if (KeyPressed.KeyChar == 'S' || KeyPressed.KeyChar == 's' || KeyPressed.Key == ConsoleKey.Enter)
                {
                    goto MenuItemInvoked;
                }
            }

            /* Note:
             * I get it, you don't like goto statements, and for good reason.
             * That being said, know it prevented a situation of loop build-up
             * over a series of method calls and that better options
             * , at the time, could not be found.
             */
            MenuItemInvoked:
                MenuItems[SelectorPosition].Action.DynamicInvoke();
            SelectorMoved:
                this.RunMenu();
        }

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
        /// Sends user to a different 'SelectorMenu'. This is the recommended method to transverse between menus.
        /// Using this method makes the console cursor invisible. If one needs the cursor visible
        /// afterward, then it must be manually done.
        /// </summary>
        /// <param name="target">Menu user is sent to</param>
        public void GoTo()
        {
            Console.Clear();
            this.SelectorPosition = 0;
            Console.BackgroundColor = this.MenuBackgroundColor;
            Console.ForegroundColor = this.MenuForegroundColor;
            this.RunMenu();
        }

        /// <summary>
        /// The console screen is cleared of the currently displayed menu, and 
        /// then the message is displayed in the destination menu's colors.
        /// After that, pressing any key will bring the user back to the menu ('SelectorMenu' object) from
        /// which the instance method was invoked from. 
        /// Using this method makes the console cursor invisible. If one needs the cursor visible
        /// afterward, then it must be manually done.
        /// </summary>
        /// <param name="target">Target menu user is sent to</param>
        /// <param name="message">Message displayed before user is sent to target</param>
        public void GoTo(string message)
        {
            Console.Clear();
            this.SelectorPosition = 0;
            Console.BackgroundColor = this.MenuBackgroundColor;
            Console.ForegroundColor = this.MenuForegroundColor;
            Console.WriteLine(message);
            Console.ReadKey(true);
            this.RunMenu();
        }
    }

    /// <summary>
    /// Defines properties of a menu's selector. This includes the selector's
    /// placeholder string, background color (selector & selected)
    /// , foreground color (selector & selected), and the selector
    /// string.
    /// </summary>
    public class Selector
    {
        private const string DEFAULT_UTF8_SELECTOR_STR = "  →  ";
        private const string DEFAULT_SELECTOR_STR = "  -->  ";
        /// <summary>
        /// String written if selector is present at a menu item. (5 characters)
        /// </summary>
        public string SelectorStr = DEFAULT_SELECTOR_STR;
        /// <summary>
        /// String written if selector is not present at a menu item. (4 characters)
        /// </summary>
        /// <remarks>
        /// To emphasize selection, placeholder for 
        /// </remarks>
        public string SelectorStrPlaceholder = "    ";
        /// <summary>
        /// Index of the menu item the selector pointing at. If 0, selector is
        /// pointing at the top menu item.
        /// </summary>
        public int MenuItemIndex = 0;
        /// <summary>
        /// If true, selector is highlighted by color. Otherwise, highlighting of selector is
        /// ignored.
        /// </summary>
        public bool IsHighlighted = true;
        /// <summary>
        /// If true, selector's menu item (a.k.a. selected) is highlighted by color.
        /// Otherwise, highlighting of selector is ignored.
        /// </summary>
        public bool IsTargetHighlighted = true;
        /// <summary>
        /// Selector's background color.
        /// </summary>
        public ConsoleColor BackgroundColor = ConsoleColor.Yellow;
        /// <summary>
        /// Selector's foreground color.
        /// </summary>
        public ConsoleColor BackgroundColorSelected = ConsoleColor.Yellow;
        /// <summary>
        /// Selected menu item's background color.
        /// </summary>
        public ConsoleColor ForegroundColor = ConsoleColor.Black;
        /// <summary>
        /// Selected menu item's foreground color.
        /// </summary>
        public ConsoleColor ForegroundColorSelected = ConsoleColor.Black;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="selectorStr"></param>
        /// <param name="selectorPlaceholderStr"></param>
        /// <param name="initialMenuItemIndex">If 0, the selector initially points to the top menu item</param>
        /// <param name="isHighlighted"></param>
        /// <param name="isTargetHighlighted"></param>
        /// <param name="backgroundColor"></param>
        /// <param name="backgroundColorTarget"></param>
        /// <param name="foregroundColor"></param>
        /// <param name="foregroundColorTarget"></param>
        public Selector(string selectorStr = DEFAULT_SELECTOR_STR /* default if console encoding not UTF8 */, 
            string selectorPlaceholderStr = null,
            int initialMenuItemIndex = 0, 
            bool isHighlighted = true,
            bool isTargetHighlighted = true,
            ConsoleColor backgroundColor = ConsoleColor.Yellow,
            ConsoleColor backgroundColorTarget = ConsoleColor.Yellow,
            ConsoleColor foregroundColor = ConsoleColor.Black,
            ConsoleColor foregroundColorTarget = ConsoleColor.Black)
        {
            SelectorStr = 
                   Console.OutputEncoding.EncodingName == "Unicode (UTF-8)"
                && selectorStr == DEFAULT_SELECTOR_STR
                ? DEFAULT_UTF8_SELECTOR_STR:selectorStr;
            SelectorStrPlaceholder = selectorPlaceholderStr ?? BuildSelectorPlaceholder(selectorStr);
            MenuItemIndex = initialMenuItemIndex;
            IsHighlighted = isHighlighted;
            IsTargetHighlighted = isTargetHighlighted;
            BackgroundColor = backgroundColor;
            BackgroundColorSelected = backgroundColorTarget;
            ForegroundColor = foregroundColor;
            ForegroundColorSelected = foregroundColorTarget;
        }

        /// <summary>
        /// Enables the display of most UTF8 characters on the console.
        /// Upon use, all selector menus stop using placeholder non-UTF8
        /// strings (default and hard-coded values)
        /// and start using their UTF8 counterparts.
        /// </summary>
        public static void Utf8EncodingOn() => Console.OutputEncoding = Encoding.UTF8;
        /// <summary>
        /// Turns highlighting (foreground and background) of selector on
        /// </summary>
        public void HighlightSelectorOn() => IsHighlighted = true;
        /// <summary>
        /// Turns highlighting (foreground and background) of selector off.
        /// </summary>
        public void HighlightSelectorOff() => IsHighlighted = false;
        /// <summary>
        /// Toggles whether selector highlighting is on or off.
        /// </summary>
        public void HighlightSelectorToggle() => IsHighlighted = !IsHighlighted;
        /// <summary>
        /// Turns highlighting (foreground and background) of selector's target on.
        /// </summary>
        public void HighlightTargetOn() => IsTargetHighlighted = true;
        /// <summary>
        /// Turns highlighting (foreground and background) of selector's target off.
        /// </summary>
        public void HighlightTargetOff() => IsTargetHighlighted = false;
        /// <summary>
        /// Toggles whether selector's target highlighting is on or off.
        /// </summary>
        public void HighlightTargetToggle() => IsTargetHighlighted = !IsTargetHighlighted;
        /// <summary>
        /// Sets background color of both selector and its target.
        /// </summary>
        /// <param name="backgroundColor"></param>
        public void SetAllBackgroundHighlights(ConsoleColor backgroundColor) { BackgroundColor = backgroundColor; BackgroundColorSelected = backgroundColor; }
        /// <summary>
        /// Sets foreground color of both selector and its target.
        /// </summary>
        /// <param name="foregroundColor"></param>
        public void SetAllForegroundHighlights(ConsoleColor foregroundColor) { ForegroundColor = foregroundColor; ForegroundColorSelected = foregroundColor; }

        /// <summary>
        /// Builds the string value of 'SelectorPlaceHolder' out of space characters
        /// -- 1 less than the argument's length.
        /// </summary>
        private static string BuildSelectorPlaceholder(string selectorStr)
        {
            StringBuilder spaceString = new StringBuilder();
            int loops = selectorStr.Length - 1;
            for (int i = 0; i < loops; i++)
                spaceString.Append(' ');
            return spaceString.ToString();
        }
    }

    /// <summary>
    /// Represents a menu item shown in a selector menu.
    /// </summary>
    public class MenuItem
    {
        /// <summary>
        /// Label of menu item shown on the menu.
        /// </summary>
        public string Label = "";
        /// <summary>
        /// Action menu item does when selected.
        /// </summary>
        public Action Action;
        /// <summary>
        /// If false, object is treated as a bar instead of menu item.
        /// </summary>
        public bool IsTrueMenuItem = true;

        public MenuItem(string label, Action action, bool isTrueMenuItem = true)
        {
            Label = label;
            Action = action;
            IsTrueMenuItem = isTrueMenuItem;
        }
    }
}
