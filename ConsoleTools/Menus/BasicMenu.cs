using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace ConsoleTools
{
    namespace Menus
    {
        /// <summary>
        /// Menu that allows up to 10 options selectable by pressing a number from 0 - 9.
        /// If less than 10 options are made, then only the made options are shown.
        /// </summary>
        public class BasicMenu : Menu
        {
            protected override void WriteMenu(int oldSelectedIndex = -1, int newSelectedIndex = -1)
            {
                int loops = Math.Min(MenuGlobal.Count, 10);
                int menuItemNum = 0;
                for (int i = 0; i < loops; i++)
                {
                    if (MenuGlobal[i].GetType() != typeof(MenuItem))
                    {
                        Console.WriteLine(MenuGlobal[i].Text);
                    }
                    else
                    {
                        Console.WriteLine($"[{menuItemNum}] " + MenuGlobal[i].Text);
                        menuItemNum++;
                    }
                }

                if (Console.OutputEncoding.EncodingName == "Unicode (UTF-8)")
                    Console.WriteLine("━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━");
                else
                    Console.WriteLine("----------------------------------------------------------------");

                Console.WriteLine("Press a number key to select an option.");
            }
            protected override void PromptNav()
            {
                List<MenuItem> allItems = (from thing in MenuGlobal
                                           where thing is MenuItem
                                           select (MenuItem)thing).ToList();
                int allItemsLength = allItems.Count;
                ConsoleKeyInfo pressedKey;
                while (true)
                {
                    pressedKey = Console.ReadKey();
                    switch (pressedKey.Key)
                    {
                        case ConsoleKey.D0:
                            if (allItemsLength < 1)
                                goto default;
                            allItems[0].Action.Invoke();
                            break;
                        case ConsoleKey.D1:
                            if (allItemsLength < 2)
                                goto default;
                            allItems[1].Action.Invoke();
                            break;
                        case ConsoleKey.D2:
                            if (allItemsLength < 3)
                                goto default;
                            allItems[2].Action.Invoke();
                            break;
                        case ConsoleKey.D3:
                            if (allItemsLength < 4)
                                goto default;
                            allItems[3].Action.Invoke();
                            break;
                        case ConsoleKey.D4:
                            if (allItemsLength < 5)
                                goto default;
                            allItems[4].Action.Invoke();
                            break;
                        case ConsoleKey.D5:
                            if (allItemsLength < 6)
                                goto default;
                            allItems[5].Action.Invoke();
                            break;
                        case ConsoleKey.D6:
                            if (allItemsLength < 7)
                                goto default;
                            allItems[6].Action.Invoke();
                            break;
                        case ConsoleKey.D7:
                            if (allItemsLength < 8)
                                goto default;
                            allItems[7].Action.Invoke();
                            break;
                        case ConsoleKey.D8:
                            if (allItemsLength < 9)
                                goto default;
                            allItems[8].Action.Invoke();
                            break;
                        case ConsoleKey.D9:
                            if (allItemsLength < 10)
                                goto default;
                            allItems[9].Action.Invoke();
                            break;

                        default: continue;
                    }

                    //if 
                    break;
                }
            }
        }
    }
}
