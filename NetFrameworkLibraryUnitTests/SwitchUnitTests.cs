using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ConsoleTools.MainArgsUtil;

namespace NetFrameworkLibraryUnitTests
{
    [TestClass]
    public class SwitchUnitTests
    {
        //=== CONSTANTS ===
        //=================================================================================
        /// <summary>
        /// Stands for "long form prefix"
        /// </summary>
        const string LfpLinux = "--";
        /// <summary>
        /// Stands for "short form prefix Linux"
        /// </summary>
        const string SfpLinux = "-";
        /// <summary>
        /// Stands for "short form prefix DOS"
        /// </summary>
        const string SfpDos = "/";

        //*** TestSwitchAlpha ***
        //***********************
        /* CONSTANT VALUES:
         * 1# TestSwitchAlpha
         * 2# --TestSwitchAlpha
         * 3# A
         * 4# -A
         * 5# /A
         */
        /// <summary>
        /// Stands for "TestSwitchAlpha long form"
        /// </summary>
        const string TestSwitchAlphaLf = "TestSwitchAlpha";
        /// <summary>
        /// Stands for "TestSwitchAlpha long form prefixed"
        /// </summary>
        const string TestSwitchAlphaLfpLinux = LfpLinux + TestSwitchAlphaLf;
        /// <summary>
        /// Stands for "TestSwitchAlpha short form"
        /// </summary>
        const string TestSwitchAlphaSf = "A";
        /// <summary>
        /// Stands for "TestSwitchAlpha short form prefixed Linux"
        /// </summary>
        const string TestSwitchAlphaSfpLinux = SfpLinux + TestSwitchAlphaSf;
        /// <summary>
        /// Stands for "TestSwitchAlpha short form prefixed DOS"
        /// </summary>
        const string TestSwitchAlphaSfpDos = SfpDos + TestSwitchAlphaSf;

        //*** TestSwitchBravo ***
        //***********************
        /* CONSTANT VALUES:
         * 1# TestSwitchBravo
         * 2# --TestSwitchBravo
         * 3# B
         * 4# -B
         * 5# /B
         */
        /// <summary>
        /// Stands for "TestSwitchBravo long form"
        /// </summary>
        const string TestSwitchBravoLf = "TestSwitchBravo";
        /// <summary>
        /// Stands for "TestSwitchBravo long form prefixed Linux"
        /// </summary>
        const string TestSwitchBravoLfpLinux = LfpLinux + TestSwitchBravoLf;
        /// <summary>
        /// Stands for "TestSwitchBravo short form"
        /// </summary>
        const string TestSwitchBravoSf = "B";
        /// <summary>
        /// Stands for "TestSwitchBravo short form prefixed Linux"
        /// </summary>
        const string TestSwitchBravoSfpLinux = SfpLinux + TestSwitchBravoSf;
        /// <summary>
        /// Stands for "TestSwitchBravo short form prefixed DOS"
        /// </summary>
        const string TestSwitchBravoSfpDos = SfpDos + TestSwitchBravoSf;

        //*** Test Arguments ***
        //**********************
        /// <summary>
        /// Stands for "test argument [constant number here]"
        /// </summary>
        const string tArg = "Test Argument";
        /// <summary>
        /// Stands for "test argument [constant number here]"
        /// </summary>
        const string tArg1 = "Test Argument 1";
        /// <summary>
        /// Stands for "test argument [constant number here]"
        /// </summary>
        const string tArg2 = "Test Argument 2";
        /// <summary>
        /// Stands for "test argument [constant number here]"
        /// </summary>
        const string tArg3 = "Test Argument 3";
        /// <summary>
        /// Stands for "test argument [constant number here]"
        /// </summary>
        const string tArg4 = "Test Argument 4";
        /// <summary>
        /// Stands for "test argument [constant number here]"
        /// </summary>
        const string tArg5 = "Test Argument 5";
        /// <summary>
        /// Stands for "test argument [constant number here]"
        /// </summary>
        const string tArg6 = "Test Argument 6";
        /// <summary>
        /// Stands for "test argument [constant number here]"
        /// </summary>
        const string tArg7 = "Test Argument 7";
        /// <summary>
        /// Stands for "test argument [constant number here]"
        /// </summary>
        const string tArg8 = "Test Argument 8";
        /// <summary>
        /// Stands for "test argument [constant number here]"
        /// </summary>
        const string tArg9 = "Test Argument 9";

        //=== ARGS: SOLO TESTS ===
        //==================================================================================
        public static string[] args1_Switch = new string[] { TestSwitchAlphaLfpLinux };
        public static string[] args1_Arg = new string[] { tArg };
        //=== ARGS: ADJACENT TESTS ===
        //==================================================================================
        public static string[] args2_AdjacentSwitches = new string[] { TestSwitchAlphaLfpLinux, TestSwitchBravoLfpLinux };
        public static string[] args2_AdjacentArgs = new string[] { tArg1, tArg2 };
        public static string[] args2_SwitchThenArg = new string[] { TestSwitchAlphaLfpLinux, tArg1 };
        public static string[] args2_ArgThenSwitch = new string[] { tArg1, TestSwitchAlphaLfpLinux };
        //=== ARGS: MEGA TESTS ===
        //==================================================================================
        public static string[] args5_ItHasEverything = new string[] { tArg1, tArg2, TestSwitchAlphaLfpLinux, tArg3, tArg4};        
        //=== ARGS: CAPACITY TESTS ===
        //==================================================================================
        //TODO: Figure Out Capacity Tests
        public static string[] argsEmpty = new string[0];
        public static string[] argsForSwitchA = new string[0];
        public static string[] argsForSwitchB = new string[0];
        public static string[] argsForSwitchC = new string[0];

        //=== TEST CLASSES ===
        //==================================================================================

        [TestClass]
        public class Method_AssimilateArgs : AutoTestTasks {
            /// <summary>
            /// Tests 'raw' arguments that have been 'identified' as arguments.
            /// Tests focus on ensuring 'stand-alone' arguments
            /// </summary>
            [TestClass]
            public class IdentifiedArgs : AutoTestTasks {
                //Mindly: IdentifiedArgumentsBeforeFirstSwitch
                [TestMethod]
                public void IdentifiedArgs_Before1stSwitch__Is_AddedTo__StartStandAloneArgs() {
                    var testSwitch = new Switch(
                        name: TestSwitchAlphaLf,
                        name1Char: TestSwitchAlphaSf);

                    Switch.AssimilateArgs(args5_ItHasEverything);

                    //If either or both first 2 arguments aren't in List StartStandAloneArgs, then fail test 
                    if (Switch.StartStandAloneArgs[0] != args5_ItHasEverything[0] ||
                       Switch.StartStandAloneArgs[1] != args5_ItHasEverything[1]
                       )
                        Assert.Fail("StartStandAloneArgs does not contain the start-stand-alone args");

                    //If List StartStandAloneArgs contains Switch, then fail test
                    if (Switch.StartStandAloneArgs.Contains(TestSwitchAlphaLfpLinux))
                        Assert.Fail("StartStandAloneArgs contains a switch");

                    //If List StartStandAloneArgs contained anything but the first 2 arguments, then fail test
                    foreach (var arg in Switch.StartStandAloneArgs)
                        if (arg != args5_ItHasEverything[0]
                            && arg != args5_ItHasEverything[1])
                            Assert.Fail("StartStandAloneArgs contains nothing of what it's suppose to have");
                }

                //Mindly: IdentifiedArgumentsAfterLastArgumentOfLastSwitch
                [TestMethod]
                public void IdentifiedArgs_AfterLastArgOfLastSwitch__Is_AddedTo__EndStandAloneArgs() {
                    var testSwitch = new Switch(
                        name: TestSwitchAlphaLf,
                        name1Char: TestSwitchAlphaSf,
                        maxArgs: 0);

                    Switch.AssimilateArgs(args5_ItHasEverything);

                    //If either or both first 2 arguments aren't in List EndStandAloneArgs, then fail test 
                    if (Switch.EndStandAloneArgs[0] != args5_ItHasEverything[3] ||
                        Switch.EndStandAloneArgs[1] != args5_ItHasEverything[4]
                       )
                        Assert.Fail("EndStandAloneArgs does not contain the end-stand-alone args");

                    //If List EndStandAloneArgs contains Switch, then fail test
                    if (Switch.StartStandAloneArgs.Contains(TestSwitchAlphaLfpLinux))
                        Assert.Fail("EndStandAloneArgs contains a switch");

                    //If List EndStandAloneArgs contained anything but the first 2 arguments, then fail test
                    foreach (var arg in Switch.EndStandAloneArgs)
                        if (   arg != args5_ItHasEverything[3]
                            && arg != args5_ItHasEverything[4])
                            Assert.Fail("EndStandAloneArgs contains nothing of what it's suppose to have");
                }

                //TODO: Redo Name
                //Mindly: IdentifiedNonCategoryArgumentsAreLeftAlone
                [TestMethod]
                public void IdentifiedArgs_NotAccepted_BeforeLastArgOfLastSwitch_IdentifiedArgumentNotAddedToAnyStandAloneList() {
                    var testSwitch1 = new Switch(
                        name: "test",
                        name1Char: "t",
                        maxArgs: 0);

                    var testSwitch2 = new Switch(
                        name: "box",
                        name1Char: "b",
                        maxArgs: 0);

                    string[] args = new string[] { "--test", "orphan1", "orphan2", "--box" };
                    Switch.AssimilateArgs(args);

                    if (Switch.StartStandAloneArgs.Contains("orphan1")
                       || Switch.StartStandAloneArgs.Contains("orphan2")
                       || Switch.EndStandAloneArgs.Contains("orphan1")
                       || Switch.EndStandAloneArgs.Contains("orphan2"))
                        Assert.Fail("Nothing was suppose to happen with non-accepted args \"orphan1\" and \"orphan2\", but it ended up in either List StartStandAloneArgs or EndStandAloneArgs");
                }
            }

            /// <summary>
            /// Tests 'raw' arguments that have been 'identified' as Switch objects.
            /// Tests focus on what happens when an 'identified' argument is either
            /// given or not given to a Switch.
            /// </summary>
            [TestClass]
            public class IdentifiedSwitches : AutoTestTasks
            {
                //Mindly: AllIdentifiedSwitchObjectsSentToUsedSwitches
                [TestMethod]
                public void IdentifiedSwitches__Are_AddedTo__UsedSwitches() {
                    var testSwitch = new Switch(
                        name: "test",
                        name1Char: "t");

                    var testSwitch2 = new Switch(
                        name: "box",
                        name1Char: "b");

                    var testSwitch3 = new Switch(
                        name: "start",
                        name1Char: "s");

                    Switch.AssimilateArgs(new string[] { "argOrphan", "--test", "argT", "--box", "argB", "--start", "argS" });

                    if (! (Switch.UsedSwitches.Contains(testSwitch)
                        && Switch.UsedSwitches.Contains(testSwitch2)
                        && Switch.UsedSwitches.Contains(testSwitch3)))
                        Assert.Fail("Identified Switches from raw arguments were not sent to List UsedSwitches");
                }
            }

            /// <summary>
            /// Tests that focus on how 'raw' arguments (from the command line tool arguments)
            /// are parsed into Switch objects and their arguments.
            /// </summary>
            /// <example>
            /// While in terminal environment...
            /// 
            /// CommandLineTool "RawArgument1" --RawArgument3ThatIsSwitch "RawArgument4"
            /// </example>
            [TestClass]
            public class RawArgs : AutoTestTasks
            {
                //Mindly: ArgumentsNotPrefixedWith_PREFIXES
                [TestMethod]
                public void RawArgs_WithNoSwitchPrefix__WhenParsed_Become__IdentifiedArgs() {
                    //TODO: Think about implementation
                }

                //Mindly: ArgumentsPrefixedWith_PREFIXES
                [TestMethod]
                public void RawArgs_WithSwitchPrefix__WhenParsed_Become__IdentifiedSwitches() {
                    //TODO: Think about implementation; Test equivalent to a test in Method_AssimilateArgs.IdentifiedArgs
                }
            }
        }

        [TestClass]
        public class Topic_SwitchNames : AutoTestTasks
        {
            /// <summary>
            /// Tests that focus on a Switch object's multi-character-name
            /// </summary>
            [TestClass]
            public class Name : AutoTestTasks
            {
                [TestMethod]
                public void RequiredArgument() {
                    //Enforced by compiler
                }

                [TestMethod]
                public void AllowSingleDashBetweenCharacters()
                {
                    try
                    {
                        var testSwitch = new Switch(
                            name: "my-test-name",
                            name1Char: "m");
                    }
                    catch (Exception)
                    {
                        Assert.Fail("Failed to allow single dashes in name (i.e. some-name, a-realy-long-name)");
                    }
                }

                [TestMethod]
                public void HasNoSwitchPrefixes() {
                    try
                    {
                        var testSwitch = new Switch(
                            name: "--test",
                            name1Char: "-");
                    }
                    catch(Exception)
                    {
                        return;
                    }

                    Assert.Fail("Failed to stop Switch object creation when it's single-character-name or multi-character-name had a Switch prefix character");
                }

                [TestMethod]
                public void NameLengthGreaterThan1() {
                    try
                    {
                        var testSwitch = new Switch(
                            name: "a",
                            name1Char: "a");
                    }
                    catch (Exception)
                    {
                        return;
                    }

                    Assert.Fail("Failed to stop Switch object creation when multi-character-name was not greater than 1 character");

                    try
                    {
                        var testSwitch2 = new Switch(
                            name: "ab",
                            name1Char: "b");
                    }
                    catch(Exception)
                    {
                        Assert.Fail("Failed to allow minimum name length of 2 characters");
                    }

                    try
                    {
                        var testSwitch3 = new Switch(
                            name: "abcdefghijklmnopqrstuvwxyz",
                            name1Char: "z");
                    }
                    catch(Exception)
                    {
                        Assert.Fail("Failed to allow name length greater than 2 characters");
                    }
                }

                //TODO: Add to Mindly
                [TestMethod]
                public void StoredAsLowerCase()
                {
                    var testSwitch = new Switch(
                        name: "TeSt",
                        name1Char: "t");

                    Assert.AreEqual(testSwitch.Name, "test",
                        "Failed to store multi-character-name of Switch as lower case");
                }

                [TestClass]
                public class AllowedCharactersAnywhere : AutoTestTasks
                {
                    [TestMethod]
                    public void EmojiAllowed() {
                        try
                        {
                            var testSwitch = new Switch(
                                name: "🦄🦄🦄",
                                name1Char: "u");

                            if (testSwitch.Name != "🦄🦄🦄") throw new Exception("");
                        }
                        catch (Exception)
                        {
                            Assert.Fail("Failed to create Switch object. Emoji should be allowed in the single-character-name and multi-character-name");
                        }
                    }

                    [TestMethod]
                    public void NumbersAllowed() {
                        try
                        {
                            var testSwitch = new Switch(
                                name: "123",
                                name1Char: "1");

                            if (testSwitch.Name != "123") throw new Exception("");
                            if (testSwitch.Name1Char != "1") throw new Exception("");
                        }
                        catch (Exception)
                        {
                            Assert.Fail("Failed to create Switch object. Numbers should be allowed in the single-character-name and multi-character-name");
                        }
                    }

                    /// <remarks>
                    /// Ban of Switch prefixes in Switch name is
                    /// tested in a diferent test. Although the
                    /// method's name implies this feature is reinforced
                    /// here, for the sake of elminating redundency,
                    /// the test is excluded.
                    /// </remarks>
                    [TestMethod]
                    public void UTF8_ExceptSwitchPrefixes_Allowed() {
                        try
                        {
                            var testSwitch = new Switch(
                                name: "§§§",
                                name1Char: "§");

                            if (testSwitch.Name != "§§§") throw new Exception("");
                            if (testSwitch.Name1Char != "§") throw new Exception("");
                        }
                        catch(Exception)
                        {
                            Assert.Fail("Failed to create Switch object. Other UTF-8 Character (except Switch prefixes) should be allowed in the single-character-name and multi-character-name");
                        }
                    }
                }
            }

            /// <summary>
            /// Tests that focus on a Switch object's single-character-name
            /// </summary>
            [TestClass]
            public class Name1Char : AutoTestTasks
            {
                [TestMethod]
                public void OptionalArgument() {
                    var testSwitch = new Switch(
                        name: "test");

                    Assert.AreEqual(testSwitch.Name1Char, "t",
                        "Automatic creation of Switch object's single-character-name is not available when constructor parameter 'name1Char' is not provided an argument");
                }

                //TODO: Should be erased because it's equivalent to test method 'OptionalArgument' instead?
                [TestMethod]
                public void ValueOfNullExcludesIt() {
                    var testSwitch = new Switch(
                        name: "test",
                        name1Char: null);

                    var args = new string[] { "-t", "fake arg" };

                    Switch.AssimilateArgs(args);

                    if (Switch.UsedSwitches.Contains(testSwitch))
                        Assert.Fail("When excluding the single-character-name of a switch, the Switch can not be invoked by its short name");
                }

                [TestMethod]
                public void HasNoSwitchPrefixes() {
                    try
                    {
                        var testSwitch = new Switch(
                            name: "--tes/t",
                            name1Char: "-");

                        var testSwitch2 = new Switch(
                            name: "/test-",
                            name1Char: "/"
                            );

                        if (testSwitch.Name.Contains("-") || testSwitch.Name.Contains("/"))
                            throw new Exception("");
                        if (testSwitch.Name.Contains("-") || testSwitch.Name.Contains("/"))
                            throw new Exception("");

                        if (testSwitch.Name1Char.Contains("-") || testSwitch.Name1Char.Contains("/"))
                            throw new Exception("");
                        if (testSwitch.Name1Char.Contains("-") || testSwitch.Name1Char.Contains("/"))
                            throw new Exception("");
                    }
                    catch (Exception)
                    {
                        return;
                    }

                    Assert.Fail("Switch name may not have Switch prefixes");
                }

                [TestMethod]
                public void NameLengthEquals1() {
                    try
                    {
                        var testSwitch = new Switch(
                            name: "test",
                            name1Char: "failed");

                        if (testSwitch.Name1Char.Length != 1 && testSwitch.Name1Char != null)
                            Assert.Fail("Switch object should have failed creation if single-character-name was grater than 1 character");
                    }
                    catch(Exception)
                    {
                        return;
                    }
                }

                //TODO: Add to Mindly
                [TestMethod]
                public void StoredAsLowerCase()
                {
                    var testSwitch = new Switch(
                        name: "test",
                        name1Char: "T");

                    Assert.AreEqual(testSwitch.Name1Char, "t",
                        "Failed to store single-character-name of Switch as lower case");
                }

                [TestClass]
                public class AllowedCharactersAnywhere : AutoTestTasks
                {
                    [TestMethod]
                    public void EmojiAllowed() {
                        try
                        {
                            var testSwitch = new Switch(
                                name: "unicorn",
                                name1Char: "🦄");

                            if (testSwitch.Name1Char != "🦄") throw new Exception("");
                        }
                        catch (Exception)
                        {
                            Assert.Fail("Failed to create Switch object. Emoji should be allowed in the single-character-name and multi-character-name");
                        }
                    }

                    [TestMethod]
                    public void NumbersAllowed() {
                        try
                        {
                            var testSwitch = new Switch(
                                name: "123",
                                name1Char: "1");

                            if (testSwitch.Name != "123") throw new Exception("");
                            if (testSwitch.Name1Char != "1") throw new Exception("");
                        }
                        catch (Exception)
                        {
                            Assert.Fail("Failed to create Switch object. Numbers should be allowed in the single-character-name and multi-character-name");
                        }
                    }

                    /// <remarks>
                    /// Ban of Switch prefixes in Switch name is
                    /// tested in a different test. Although the
                    /// method's name implies this feature is reinforced
                    /// here, for the sake of eliminating redundancy,
                    /// the test is excluded.
                    /// </remarks>
                    [TestMethod]
                    public void UTF8_ExceptSwitchPrefixes_Allowed()
                    {
                        try
                        {
                            var testSwitch = new Switch(
                                name: "§§§",
                                name1Char: "§");

                            if (testSwitch.Name != "§§§") throw new Exception("");
                            if (testSwitch.Name1Char != "§") throw new Exception("");
                        }
                        catch (Exception)
                        {
                            Assert.Fail("Failed to create Switch object. Other UTF-8 Character (except Switch prefixes) should be allowed in the single-character-name and multi-character-name");
                        }
                    }
                }
            }

            /// <summary>
            /// Tests that focus on -- during Switch object construction -- 'name conflict prevention'
            /// </summary>
            [TestClass]
            public class NameConflictPrevention : AutoTestTasks
            {
                //Mindly: When Both Names Included
                [TestClass]
                public class WhenSwitchHasBothNames : AutoTestTasks
                {
                    [TestMethod]
                    public void EnsureSingleCharacterNameIsNotTaken() {
                        try
                        {
                            var testSwitch = new Switch(
                                name: "test",
                                name1Char: "t");

                            var testSwitch2 = new Switch(
                                name: "test2",
                                name1Char: "t");
                        }
                        catch(Exception)
                        {
                            return;
                        }

                        Assert.Fail("Failed to keep the single-character-name between 2 Switch objects unique");
                    }

                    [TestMethod]
                    public void EnsureMultiCharacterNameIsNotTaken() {
                        try
                        {
                            var testSwitch = new Switch(
                                name: "test",
                                name1Char: "t");

                            var testSwitch2 = new Switch(
                                name: "test",
                                name1Char: "b");
                        }
                        catch (Exception)
                        {
                            return;
                        }

                        Assert.Fail("Failed to keep the multi-character-name between 2 Switch objects unique");
                    }
                }

                //Mindly: When Single-character-name Is Excluded
                [TestClass]
                public class WhenSwitchHasSingleCharacterNameExcluded : AutoTestTasks
                {

                    [TestMethod]
                    public void EnsureMultiCharacterNameIsNotTaken() {
                        try
                        {
                            var testSwitch = new Switch(
                                name: "test",
                                name1Char: null);

                            var testSwitch2 = new Switch(
                                name: "test",
                                name1Char: null);
                        }
                        catch (Exception)
                        {
                            return;
                        }

                        Assert.Fail("Failed to keep the multi-character-name between 2 Switch objects unique");
                    }
                }
            }
        }

        [TestClass]
        public class Topic_MaxArgsAndMinArgs : AutoTestTasks
        {
            [TestClass]
            public class DefaultArgsAcceptedIsInt32Max : AutoTestTasks
            { 
                /* Does nothing. Only Reference Link For Reader to help 
                 * construct a good mental map of the library 
                 */ }
            [TestClass]
            public class MaxArgs : AutoTestTasks
            {
                [TestMethod]
                public void NeverLessThanMinArgs() {
                    var testSwitch = new Switch(
                        name: "test",
                        name1Char: "t",
                        minArgs: 1,    
                        maxArgs: 0);

                    if(testSwitch.MaxArgs < testSwitch.MinArgs)
                    Assert.Fail("Program did not close");
                }

                [TestMethod]
                public void NeverNegative()
                {
                    try
                    {
                        var testSwitch = new Switch(
                            name: "test",
                            name1Char: "t",
                            maxArgs: -300);
                    }
                    catch(Exception)
                    { return; }

                    Assert.Fail("MaxArgs was negative");
                }

                [TestMethod]
                public void DefaultIsInt32MaxValue() {
                    var testSwitch = new Switch(
                        name: "test",
                        name1Char: "t");
                    if (testSwitch.MaxArgs != Int32.MaxValue)
                        Assert.Fail("Expected default value of a switches mamimum accepted arguments was not max value of 32 bit integer");
                }

                [TestMethod]
                public void OptionalArgument() { /* Should be true unless the designed user expirence is changed */ }

                [TestMethod]
                public void LimitsTotalSwitchArgsAccepted() {
                    var testSwitch = new Switch(
                        name: "test",
                        name1Char: "t",
                        maxArgs: 3);

                    var args = new string[] { "--test",
                        "Allies are the undead (God of War), Death Claws (Fallout), and player1 with a nano suit (Crysis; with 2 weapons of choice) ",
                        "Fraction of allies ride into glorious battle on queen Xenomorphs (Alien) equipped with rail guns", 
                        "'Environmental hazards' include actively firing imperial-class Star Destroyers (Star Wars; reason: poor aim), Reapers (Mass Effect), dragons (Skyrim), and Tribbles (Star Trek; tripping hazard... or dragon food?)",
                        "'Environmental hazards' include debri -- falling or stationary -- from inevitable intercombat of previously said 'environmental hazards'",
                        "Enemies are demons (Doom), demons (Dragon Age), and player2 as 'the demon' (Halo; with 2 weapons of choice)",
                        "Fraction of enemies ride into glorious battle on Brumaks (Gears of War)",};

                    Switch.AssimilateArgs(args);

                    if (testSwitch.Args.Count != 3)
                        Assert.Fail("Switch was not restricted to 3 arguments");
                }
            }

            [TestClass]
            public class MinArgs : AutoTestTasks
            {
                [TestMethod]
                public void OptionalArgument() { /* Should be true unless the designed user expirence is changed */ }

                [TestMethod]
                public void NeverNegative()
                {
                    try
                    {
                        var testSwitch = new Switch(
                            name: "test",
                            name1Char: "t",
                            minArgs: -300);
                    }
                    catch (Exception)
                    { return; }

                    Assert.Fail("MinArgs was negative");
                }

                [TestMethod]
                public void TotalArgumentsRequired() {
                    var testSwitch = new Switch(
                        name: "test",
                        name1Char: "t",
                        minArgs: 2);

                    var args = new string[] { "--test" };

                    try
                    {
                        Switch.AssimilateArgs(args);
                    }
                    catch (Exception)
                    {
                        return;
                    }

                    Assert.Fail("Program was not closed despite Switch not having the required amount of arguments");
                }
            }

            [TestClass]
            public class IsArgless : AutoTestTasks
            {
                [TestMethod]
                public void OptionalArgument() { /* Should be true unless the designed user expirence is changed */ }
            }
        }

        [TestClass]
        public class Topic_SwitchObjects : AutoTestTasks
        {
            //Mindly: Created Switches Sent To AllSwitches
            [TestMethod]
            public void WhenCreated_ReferenceSentToListAllSwitches() {
                var testSwitch = new Switch(
                    name: "test",
                    name1Char: "t");

                if (!Switch.AllSwitches.Contains(testSwitch))
                    Assert.Fail("Test failed since newly created Switch was NOT added to List AllSwitches");
            }
        }

        /* Note:
         * At the moment, almost all tests that would've taken place in this class
         * are located in class "Method_AssimilateArgs.IdentifiedArgs".
         * To prevent maintenance complications, said tests have NOT
         * been copied here.
         * 
         * Only exception to this is List "AllStandAloneArgs"
         */
        [TestClass]
        public class Topic_StandAloneArguments : AutoTestTasks {
            [TestClass]
            public class StartStandAloneArgs { /* See test at Method_AssimilateArgs.IdentifiedArgs */ }

            [TestClass]
            public class EndStandAloneArgs { /* See test at Method_AssimilateArgs.IdentifiedArgs */ }

            [TestClass]
            public class AllStandAloneArgs : AutoTestTasks
            {
                [TestMethod]
                 public void IsAllStandAloneArgs()
                 {
                    var testSwitch1 = new Switch(
                        name: "test1",
                        name1Char: "a");

                    var testSwitch2 = new Switch(
                        name: "test2",
                        name1Char: "b",
                        maxArgs:0);

                    var args = new string[] { "arg1", "arg2", "--test1", "arg3", "arg4", "--test2", "arg5", "arg6"};

                    Switch.AssimilateArgs(args);

                    if (   /* arg1 */ Switch.AllStandAloneArgs[0] != Switch.StartStandAloneArgs[0]
                        || /* arg2 */ Switch.AllStandAloneArgs[1] != Switch.StartStandAloneArgs[1]
                        || /* arg5 */ Switch.AllStandAloneArgs[2] != Switch.EndStandAloneArgs[0]
                        || /* arg6 */ Switch.AllStandAloneArgs[3] != Switch.EndStandAloneArgs[1])
                        Assert.Fail("AllStandAloneArgs didn't include all from both StartStandAloneArgs and EndStandAloneArgs");
                 }
            }
        }

        //=== CLEAN UP AND INITIALIZE CORE METHODS ===
        //============================================================================================
        /// <summary>
        /// Inheriting this class provides cleanup and initialization methods
        /// that are automatically run for the inheritor
        /// </summary>
        public abstract class AutoTestTasks
        {
            [TestCleanup]
            public void TestCleanUp()
            {
                Switch.AllSwitches = new Switch.SwitchList();
                Switch.UsedSwitches = new Switch.SwitchList();
                Switch.StartStandAloneArgs = new List<string>();
                Switch.EndStandAloneArgs = new List<string>();
                Switch.AllStandAloneArgs = new List<string>();
            }
        }
    }
}
