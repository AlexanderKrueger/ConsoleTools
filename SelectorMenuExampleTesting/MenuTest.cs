using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ConsoleTools.Menus;

namespace SelectorMenuExampleTesting
{
    class MenuTest
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;
            //*** Initialize Menus ***
            //************************
            SelectorMenu Menu1 = new SelectorMenu();
            SelectorMenu Menu2 = new SelectorMenu(ConsoleColor.Black, ConsoleColor.Green);
            SelectorMenu Menu3 = new SelectorMenu(" \u03A9 ");
            Menu3.Selector.BackgroundColor = ConsoleColor.Black;
            Menu3.Selector.ForegroundColor = ConsoleColor.Red;
            SelectorMenu Menu4 = new SelectorMenu();
            BasicMenu MenuBasic = new BasicMenu();

            //*** Build Menus ***
            //*******************
            Menu1
                .AddItem("Does Nothing")
                .AddItem("GoTo A Message", () => Menu1.GoTo("Press any key: go back."))
                .AddItem("GoTo A Message, then arrive at Menu2", () => Menu2.GoTo("Press any key: go to Menu2"))
                .AddItem("GoTo Menu2 (Expecting Green Text)", () => Menu2.GoTo())
                .AddItem("GoTo Menu3 (Swap Testing)", () => Menu3.GoTo())
                .AddItem("GoTo Menu4 (Multi-Line Bar Testing)", () => Menu4.GoTo())
                .AddItem("GoTo MenuBasic (Is BasicMenu)", () => MenuBasic.GoTo())
                .AddItem("EXIT", () => SelectorMenu.Exit());

            Menu2
                .AddItem(
                    label: "A) I'm the real option",
                    action: () => { })
                .AddItem(
                    label: "B) No I'm the real option",
                    action: () => { })
                .AddBar()
                .AddItem(
                    label: "C) Uh...",
                    action: () =>
                    {
                        Menu2.GoTo("I'm the real option?");
                    })
                .AddItem(
                    label: "GoTo Menu1",
                    action: () => Menu1.GoTo())
                .AddItem(
                    label: "GoTo Menu3",
                    action: () => Menu3.GoTo())
                .AddItem(
                    label: "EXIT",
                    action: () => SelectorMenu.Exit());

            Menu3
                .AddItem("A) Menu3 Option1",delegate () { })
                .AddItem("B) Menu3 Option2", delegate () { })
                .AddItem("C) Menu3 Option3", delegate () { })
                .AddItem("D) Swap Options \"B\" and \"D\"", delegate () { Menu3.Swap(3, 1); })
                .AddBar()
                .AddItem("GoTo Menu1", delegate () { Menu1.GoTo(); })
                .AddItem("GoTo Menu2", delegate () { Menu2.GoTo(); })
                .AddItem("GoTo Menu4", delegate () { Menu4.GoTo(); })
                .AddItem("EXIT", delegate () { SelectorMenu.Exit(); });

            Menu4
                .AddItem("Option1) Be Bill Nye", delegate () { Menu4.GoTo("He's the science guy."); })
                .AddItem("Option2) Be Bill Murray", delegate () { Menu4.GoTo("He's an actor"); })
                .AddItem("Option3) Be Bill Gates", delegate () { Menu4.GoTo("Microsoft. Enough said..."); })
                .AddItem("Option4) Be Bill (the overly generic person)", delegate () { Menu4.GoTo("Hi guys."); })
                .AddBar("-----------------------------\n-----------------------------\n-----------------------------")
                .AddItem("GoTo Menu1", delegate () { Menu1.GoTo(); })
                .AddItem("GoTo Menu2", delegate () { Menu2.GoTo(); })
                .AddItem("GoTo Menu3", delegate () { Menu3.GoTo(); })
                .AddItem("Exit", delegate () { SelectorMenu.Exit(); });

            MenuBasic
                .AddItem("GoTo Menu1", () => Menu1.GoTo())
                .AddItem("GoTo Menu2", () => Menu2.GoTo())
                .AddBar("###########################################################\n"+
                        "#### (Bar Testing) Bars Don't Break Menu Item Selection ###\n"+
                        "###########################################################")
                .AddItem("GoTo Menu3", () => Menu3.GoTo())
                .AddItem("GoTo Menu4", () => Menu4.GoTo())
                .AddBar()
                .AddItem("Exit", () => Menu.Exit());

            //*** Show Starting Menu ***
            //**************************
            Menu1.GoTo();

            //*** Console Exit Code ***
            //*************************
            /* Note:
             * If the menu was started by an actual program, then
             * control will be returned to that program; Action won't be
             * required. 
             */
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
        }
    }
}
