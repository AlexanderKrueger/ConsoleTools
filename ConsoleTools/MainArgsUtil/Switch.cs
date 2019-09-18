using System;
using System.Runtime.CompilerServices;
using System.Collections.Generic;
using System.Globalization;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Text;

namespace ConsoleTools
{
    namespace MainArgsUtil {
    /// <summary>
    /// Switches modify the behavior of the command line function.
    /// 
    /// When 'c' represents any character but '/' or '-' (adjacent 'c's can be different values),
    /// then:
    ///     Single-character form of a switch (-c, /c)
    ///     Multi-character form of a switch (--cc, --ccc, --ccc-ccc, --ccc-ccc-ccc etc.)
    /// </summary>
    /// <remarks>
    /// <list type="bullet">
    /// <item>
    /// <term>Name Enforcement</term>
    /// <description>
    /// The "Switch" constructor
    /// enforces the rules regarding the properties of all switches.
    /// Of the enforced properties,
    /// no Switch may have a multi-character-name or single-character-name
    /// that's already in use by a created Switch
    /// (found in List<T> AllSwitches).
    /// However, if a Switch is type of "multi-character-name only" (defined by setting<see cref="Name1Char"/> = null)
    /// , the rules are limited as such that:
    /// no Switch may have a multi-character-name that's already in use by a created Switch
    /// (again, found in List<T> AllSwitches).
    /// </description>
    /// </item>
    /// <item>
    /// <term>Switch Prefixes</term>
    /// <description>
    /// On the command line, only switch prefix "--" can
    /// invoke a switch by its multi-character-name (i.e. "ClTool --help", "ClTool --yo", "ClTool --yipyap").
    /// As for the single-character-name of a switch, it can only be invoked on the command line with
    /// switch prefixes "-" and "/" (i.e. "ClTool -?", "ClTool /?", "ClTool -a", "ClTool /a").
    /// </description>
    /// </item>
    /// <item>
    /// <term>Switch Arguments</term>
    /// <description>
    /// "Switch" objects representing switches for a command line tool can carry arguments
    /// of the switch.
    /// </description>
    /// </item>
    /// </list>
    /// </remarks>
    /// <seealso cref="Switch.AssimilateArgs(string[])"/>
    public class Switch
    {
        //=== SWITCH: VARIABLES & CONSTANTS ===
        //=============================================================================
        //*** Object Variables ***
        //************************
        /// <summary>
        /// List of all 'Switch' objects made for command line tool.
        /// </summary>
        public static SwitchList AllSwitches = new SwitchList();
        /// <summary>
        /// List of 'Switch' objects representing all switches used with the command line tool.
        /// </summary>
        public static SwitchList UsedSwitches = new SwitchList();
        /// <summary>
        /// Contains all command line tool arguments before the first
        /// command line tool switch used.
        /// </summary>
        public static List<string> StartStandAloneArgs = new List<string>();
        /// <summary>
        /// Contains all command line tool arguments after the last
        /// command line tool switch's last accepted argument.
        /// </summary>
        public static List<string> EndStandAloneArgs = new List<string>();
        /// <summary>
        /// A list of references that's a concatenation of lists
        /// , in said order, 'StartStandAloneArgs' and 'EndStandAloneArgs'.
        /// Changes to items in either contributing list will be visible in this list.
        /// Likewise, changes to items in this list should affect either 'StartStandAloneArgs'
        /// or 'EndStandAloneArgs'.
        /// However, the adding and removing of items in either this list or the contributing lists
        /// isn't reflected in the other list(s).
        /// </summary>
        /// <remarks>
        /// As a reminder, C# keyword "string" is an alias for object 'String'
        /// , and objects are passed by reference, not value.
        /// </remarks>
        public static List<string> AllStandAloneArgs = new List<string>();
        /// <summary>
        /// SmartArg version of <see cref="StartStandAloneArgs"/>.
        /// </summary>
        /// <seealso cref="SmartArg"/>
        public static List<SmartArg> StartStandAloneSmartArgs = new List<SmartArg>();
        /// <summary>
        /// SmartArg version of <see cref="EndStandAloneArgs"/>.
        /// </summary>
        /// <seealso cref="SmartArg"/>
        public static List<SmartArg> EndStandAloneSmartArgs = new List<SmartArg>();
        /// <summary>
        /// SmartArg version of <see cref="AllStandAloneArgs"/>.
        /// </summary>
        /// <seealso cref="SmartArg"/>
        public static List<SmartArg> AllStandAloneSmartArgs = new List<SmartArg>();
        /// <summary>
        /// Multi-character-name of the switch.
        /// Expect a completely lower-case value with no switch prefix 
        /// (i.e. help, copy, alert).
        /// </summary>
        public readonly string Name = "";
        /// <summary>
        /// Single-character-name of the switch.
        /// Expect a completely lower-case value with no switch prefix 
        /// (correct: h, c, a).
        /// If this name was not explicitly defined (value is "")
        /// , then it's automatically set as the 1st character of the field <see cref="Name"/>.
        /// </summary>
        public readonly string Name1Char = "";
        /// <summary>
        /// Contains 
        /// </summary>
        public List<SwitchParameter> Parameters = new List<SwitchParameter>();
        /// <summary>
        /// Maximum arguments the switch can accept.
        /// </summary>
        public readonly int MaxArgs = Int32.MaxValue;
        /// <summary>
        /// Minimum arguments the switch requires. If this
        /// requirement is not satisfied, then an error is thrown.
        /// </summary>
        public readonly int MinArgs = 0;
        /// <summary>
        /// Indicates if the switch was used in the command line tool.
        /// </summary>
        public bool IsUsed = false;
        /// <summary>
        /// Arguments the switch has accepted.
        /// </summary>
        public List<string> Args = new List<string>();
        /// <summary>
        /// Arguments the switch has accepted, but wrapped in "SmartArg" objects
        /// for added convenience. If the goal is avoiding string to data type conversions,
        /// (i.e. string --> int, string --> bool) consider using a "SmartArg" object.
        /// 
        /// </summary>
        public List<SmartArg> SmartArgs = new List<SmartArg>();
        public string Summary = "";
        public string Remarks = "";

        /// <summary>
        /// Indicates if a "Params" parameter documentation has been added.
        /// If true, exception will be thrown on the next attempt to add
        /// parameter documentation (for both types of parameters)
        /// </summary>
        private bool IsParamsAdded = false;

        //*** Error Messages ***
        //**********************
        const string ERROR_MSG_MUST_HAVE_NAME_1 = "Switch must have multi-character-name greater than or equal to 2 characters long: could not create object";
        const string ERROR_MSG_MUST_HAVE_NAME_2 = "Switch's single-character-name must be 1 character: could not create object";
        const string ERROR_MSG_NAME_HAS_INVALID_CHARACTERS_1 = "Switch's multi-character-name must not contain special characters '/' or '-': could not create object";
        const string ERROR_MSG_NAME_HAS_INVALID_CHARACTERS_2 = "Switch's single-character-name must not contain special characters '/' or '-': could not create object";
        const string ERROR_MSG_NAME_IS_NOT_PROPER = "Switch's single-character name or multi-character-name is not of proper form (refer to summary of method 'IsSwitchName' to see what constitutes a valid switch): could not create object";
        const string ERROR_MSG_MAXARGS_LESS_THAN_MINARGS_OR_ZERO = "Switch's maximum arguments allowed is unaceptable as it's either less than 0 or the minimum allowed args: could not create object";
        const string ERROR_MSG_MINARGS_IS_NEGATIVE = "Switch's minimum arguments allowed must not be negative: could not create object";
        const string ERROR_MSG_SWITCH_NOT_UNIQUE = "Switch is not unique (neither arguments 'name' or 'name1Char' can be found in any developed switch): could not create object";

        //=== SWITCH: SUB-CLASSES ===
        //=============================================================================
        public class SwitchList : List<Switch>
        {
            /// <summary>
            /// Gets Switch from AllSwitches by using either the Switch's multi-character-name or single-character-name
            /// . Switch name with switch prefix "-", "--", or "/" is optional, though, use should
            /// be appropriate for multi-character-name or single-character-name.
            /// </summary>
            /// <param name="singleOrMultiCharName"></param>
            /// <returns></returns>
            public Switch GetSwitch(string singleOrMultiCharName)
            {
                //lower-case argument
                singleOrMultiCharName = singleOrMultiCharName.ToLower();

                if (singleOrMultiCharName.Contains("--"))
                {
                    singleOrMultiCharName = singleOrMultiCharName.Replace("--", "");
                    return this.Find(s => s.Name == singleOrMultiCharName);
                }
                else if (singleOrMultiCharName.Contains("-") || singleOrMultiCharName.Contains("/"))
                {
                    singleOrMultiCharName = singleOrMultiCharName.Replace("-", "").Replace("/", "");
                    return this.Find(s => s.Name1Char == singleOrMultiCharName);
                }
                //else if no switch prefix
                else
                {
                    //remove switch prefix "-", "--", or "/" - if any - so switch can be called
                    //by command line tool argument (arg has prefix) or by name (no prefix)
                    singleOrMultiCharName = singleOrMultiCharName.Replace("/", "").Replace("-", "");
                    return this.Find(s => s.Name1Char == singleOrMultiCharName || s.Name == singleOrMultiCharName);
                }
            }

            /// <summary>
            /// Returns list of switch objects where each switch was
            /// used by the command line tool.
            /// </summary>
            /// <returns></returns>
            public SwitchList GetUsedSwitches()
            {
                SwitchList list = new SwitchList();
                list.AddRange(this.FindAll(s => s.IsUsed == true));
                return list;
            }
        }

        //=== SWITCH: CONSTRUCTORS & DESTRUCTORS ===
        //=============================================================================
        /// <summary>
        /// Defines a "switch" the command line tool may use.
        /// The switch will act like switches from
        /// Linux (i.e. "Tool --switch", "Tool -s") and DOS (i.e. "Tool /s").
        /// At the moment, not all syntax features are supported.
        /// <list type="table">
        /// <listheader>
        /// <term>
        /// Known Unsupported Syntax
        /// </term>
        /// </listheader>
        /// <item>
        /// <description>
        /// Tool --switch=value
        /// </description>
        /// </item>
        /// <item>
        /// <description>
        /// Tool /MultiCharName
        /// </description>
        /// </item>
        /// </list>
        /// </summary>
        /// <param name="name">multi-character-name: must be 2 or more characters long (i.e. "help", "if", "1$🖖")</param>
        /// <param name="name1Char">single-character-name: must be 1 character long (i.e. "a", "$", "3", "🖖")</param>
        /// <param name="maxArgs">maximum arguments accepted</param>
        /// <param name="minArgs">minimum arguments required</param>
        /// <param name="isArgless">if true, forces Switch to not accept arguments</param>
        public Switch(string name, string name1Char = "", int maxArgs = Int32.MaxValue, int minArgs = 0, bool isArgless = false, string summary = "", string remarks = "")
        {
            //*** Detect & Correct "maxArgs" And "minArgs" Conflicts (if possible) ***
            //************************************************************************
            // if minArgs is negative, then fail Switch construction
            if (minArgs < 0)
                { Debug.WriteLine(ERROR_MSG_MINARGS_IS_NEGATIVE);
                throw new ArgumentException(ERROR_MSG_MINARGS_IS_NEGATIVE); }
            // if maxArgs is negative, then fail Switch construction
            if (maxArgs < 0)
                { Debug.WriteLine(ERROR_MSG_MAXARGS_LESS_THAN_MINARGS_OR_ZERO);
                throw new ArgumentException(ERROR_MSG_MAXARGS_LESS_THAN_MINARGS_OR_ZERO);
            }
            // if maxArgs < minArgs, then perform a correction
            if (maxArgs < minArgs)
                { maxArgs = minArgs; }

            //*** Other Operations On "maxArgs" And "minArgs" ***
            //***************************************************
            if(isArgless == true)
            {
                maxArgs = 0;
                minArgs = 0;
            }

            //*** Automatic Single-character-name Creation If Its Value Is "" ***
            //*******************************************************************
            if (name1Char == "" && !String.IsNullOrEmpty(name))
                name1Char = name[0].ToString();

            //*** Lowercase "name" And "name1Char" ***
            //****************************************
            name = name.ToLower();
            name1Char = name1Char?.ToLower();

            //*** Perform Basic Validation On Argument "name" (invalid characters & length) ***
            //*********************************************************************************
            if (String.IsNullOrEmpty(name)
                || name.GetUTF8Length() < 2) {
                Debug.WriteLine(ERROR_MSG_MUST_HAVE_NAME_1);
                throw new FormatException(ERROR_MSG_MUST_HAVE_NAME_1);
            }
            if (   name.Contains(/* invalid character */ "/")) {
                Debug.WriteLine(ERROR_MSG_NAME_HAS_INVALID_CHARACTERS_1);
                throw new FormatException(ERROR_MSG_NAME_HAS_INVALID_CHARACTERS_1);
            }

            //*** Perform Basic Validation On Argument "name1Char" (invalid characters & length) ***
            //**************************************************************************************
            if (!(name1Char is null) && name1Char.GetUTF8Length() != 1)
            {
                Debug.WriteLine(ERROR_MSG_MUST_HAVE_NAME_2);
                throw new FormatException(ERROR_MSG_MUST_HAVE_NAME_2);
            }
            if (   (name1Char is null? false: name1Char.Contains(/* invalid character */ "/") )
                || (name1Char is null? false: name1Char.Contains(/* invalid character */ "-") ) ) {
                Debug.WriteLine(ERROR_MSG_NAME_HAS_INVALID_CHARACTERS_2);
                throw new FormatException(ERROR_MSG_NAME_HAS_INVALID_CHARACTERS_2);
            }

            //*** Perform Complete Validation Of "name" And "name1Char" ***
            //*************************************************************
            if (!name.IsSwitchFormat() && (name1Char is null? true: !name1Char.IsSwitchFormat()) ) {
                Debug.WriteLine(ERROR_MSG_NAME_IS_NOT_PROPER);
                throw new FormatException(ERROR_MSG_NAME_IS_NOT_PROPER);
            }

            //*** Test If Switch Is Unique (no other switch has same value of "Name" or "Name1Char") ***
            //******************************************************************************************
            if (!Switch.IsUnique(name, name1Char)) {
                Debug.WriteLine(ERROR_MSG_SWITCH_NOT_UNIQUE);
                throw new ArgumentException(ERROR_MSG_SWITCH_NOT_UNIQUE);
            }

            //*** If No Test Fails, Create Object ***
            //***************************************
            Name = name;
            Name1Char = name1Char;
            MaxArgs = maxArgs;
            MinArgs = minArgs;
            Switch.AllSwitches.Add(this);
            Summary = summary;
            Remarks = remarks;
        }

        ~Switch()
        {
            Switch.AllSwitches.Remove(this);
        }

        //=== SWITCH: METHODS ===
        //=============================================================================
        /// <summary>
        /// Tests if a Switch object is unique. A switch is considered unique if
        /// no other switch has same field value of "Name" or "Name1Char".
        /// However, if the Switch is a "multi-character-name only" type,
        /// then rules are limited as such that only the field "Name" is
        /// relevant.
        /// </summary>
        /// <remarks>
        /// Think of it as name conflict detection.
        /// </remarks>
        /// <param name="name">single-character-name of Switch object</param>
        /// <param name="name1Char">multi-character-name of Switch object</param>
        /// <returns></returns>
        public static bool IsUnique(string name, string name1Char)
        {
            //Lower case of arguments
            name = name.ToLower();
            name1Char = name1Char?.ToLower();

            //If Switch not type of "multi-character-name only"
            if (name1Char != null)
            {
                //If Switch's multi-character-name OR single-character-name
                //is already in use by a created Switch
                //, then fail Switch creation
                if (AllSwitches.Exists(@switch => 
                                       @switch.Name      == name
                                    || @switch.Name1Char == name1Char))
                    return false;
            }
            //else, if Switch type of "multi-character-name only", then...
            else
            {
                //If Switch's multi-character-name
                //is already in use by a created Switch
                //, then fail Switch creation
                if (AllSwitches.Exists(@switch => @switch.Name == name))
                    return false;
            }

            return true;
        }

        /// <summary>
        /// Tests if a Switch object is unique (no other switch has same value of "Name" or "Name1Char").
        /// </summary>
        /// <param name="switchArg"></param>
        /// <returns></returns>
        public static bool IsUnique(Switch switchArg)
        {
            return IsUnique(switchArg.Name, switchArg.Name1Char);
        }

        /// <summary>
        /// <para>
        /// Generates a "help page" and returns it as a string.
        /// Majority of the page uses information stored in each created
        /// Switch object. Everything else on the page is either
        /// predetermined or the title, the latter of which uses
        /// the program's file name.
        /// </para>
        /// 
        /// <para>
        /// Please consider using the more convenient method
        /// <see cref="Switch.WriteHelp"/> - if possible.
        /// </para>
        /// </summary>
        /// <remarks>
        /// This method's most reasonable use is debugging
        /// method <see cref="Switch.WriteHelp"/>.
        /// Of course, the use-case is only valid should one
        /// find re-routing the output stream of <see cref="Console"/> undesirable.
        /// </remarks>
        /// <seealso cref="Switch.WriteHelp"/>
        /// <returns></returns>
        public static string BuildHelp
            ( string toolDescription = ""
            , string copyright = ""
            , Switch settingsSwitch = null
            , Switch copyrightSwitch = null
            , Switch helpSwitch = null)
        {
            StringBuilder msgBuilder = new StringBuilder();
            string toolName = System.Reflection.Assembly.GetExecutingAssembly().GetName().Name;
            string switchObjName = "";

            msgBuilder.AppendLine("===================================");
            msgBuilder.AppendLine("=== HELP: " + toolName);
            msgBuilder.AppendLine("===================================");
            msgBuilder.AppendLine("DESCRIPTION:");
            msgBuilder.AppendLine(toolDescription);
            msgBuilder.AppendLine("-----------------------------------");
            msgBuilder.AppendLine("USAGES:");
            foreach (var @switch in Switch.AllSwitches)
            {
                switchObjName = nameof(@switch).ToLower();
                if (@switch == settingsSwitch)
                    continue;
                else if(switchObjName == "settings"
                    || switchObjName == "switchsettings"
                    || switchObjName == "settingsswitch")
                {
                    settingsSwitch = @switch;
                    continue;
                }
                if (@switch == copyrightSwitch)
                    continue;
                else if(switchObjName == "copyright"
                     || switchObjName == "switchcopyright"
                     || switchObjName == "copyrightswitch")
                {
                    copyrightSwitch = @switch;
                    continue;
                }
                if (@switch == helpSwitch)
                    continue;
                else if(switchObjName == "help"
                     || switchObjName == "switchhelp"
                     || switchObjName == "helpswitch")
                {
                    helpSwitch = @switch;
                    continue;
                }

                msgBuilder.AppendLine("\t :: " + "--" + @switch.Name);
                if(@switch.Name1Char != null)
                msgBuilder.Append(", -" + @switch.Name1Char + ", /" + @switch.Name1Char);
                if(@switch.MinArgs != 0)
                msgBuilder.AppendLine("\t :: Minimum Arguments Required: " + @switch.MinArgs);
                if(@switch.MaxArgs != Int32.MaxValue)
                msgBuilder.AppendLine("\t:: Maximum Arguments Accepted: " + @switch.MaxArgs);

                msgBuilder.AppendLine("\t" + toolName + "");
                foreach (var param in @switch.Parameters)
                    msgBuilder.Append((param.IsParams?"... ":"") + " {" + param.ParamType.Name + ":" + param.Name + "}");

                msgBuilder.AppendLine("\t SUMMARY:");
                msgBuilder.AppendLine("\t " + @switch.Summary.Replace("\n","\t\t\n"));
                msgBuilder.AppendLine("\t REMARKS:");
                msgBuilder.AppendLine("\t" + @switch.Remarks.Replace("\n", "\t\t\n"));

                if (@switch != /* is not last Switch */ AllSwitches[AllSwitches.Count - 1])
                {
                    msgBuilder.AppendLine();
                }
            }

            if (settingsSwitch != null)
            {
                msgBuilder.AppendLine("-----------------------------------");
                msgBuilder.AppendLine("SETTINGS:");
            }

            if (copyrightSwitch != null)
            {
                msgBuilder.AppendLine("-----------------------------------");
                msgBuilder.AppendLine("COPYRIGHT:");
            }

            if(helpSwitch != null) {
                msgBuilder.AppendLine("-----------------------------------");
                msgBuilder.AppendLine("HELP:");

            }

            return msgBuilder.ToString();
        }

        /// <summary>
        /// Writes a "help page" generated by <see cref="Switch.BuildHelp"/>
        /// to the console.
        /// </summary>
        public static void WriteHelp()
        {
            Console.WriteLine(BuildHelp());
        }

        /// <summary>
        /// Parses command line arguments (raw arguments) into Switch objects, their arguments, and
        /// stand-alone arguments.
        /// </summary>
        /// <seealso cref="Switch.UsedSwitches"/>
        /// <seealso cref="Switch.Args"/>
        /// <seealso cref="Switch.StartStandAloneArgs"/>
        /// <seealso cref="Switch.EndStandAloneArgs"/>
        /// <seealso cref="Switch.AllStandAloneArgs"/>
        /// <param name="commandLineArguments">Assign it argument "args" from main(string[] args)</param>
        public static void AssimilateArgs(string[] commandLineArgs)
        {
            //*** Variables ***
            //*****************
            /* IDE ISSUE:
             * For Visual Studio 2019: version 16.1.0 and before
             * , the IDE falsely identifies unused variables in this method.
             * Only delete a variable after manual confirmation.
             */
            Switch curSwitch = null;
            Switch lastSwitch = null;
            int indexOfLastSwitch = 0;
            string lastRawArgIdentifiedAsSwitch = "";
            string curSwitchStr = null;
            bool isFirstSwitchEncountered = false;
            bool isCurSwitchTheLast = false;
            bool rawArgIsSwitchArg = false;
            bool rawArgIsSwitch = false;
            bool minMet = false;
            bool maxMet = false;

            //*** Get Last Raw Argument Identified As Switch ***
            //**************************************************
            if(commandLineArgs.Length > 0)
            for(int i = commandLineArgs.Length - 1; i >= 0; i--)
                if (commandLineArgs[i].IsPrefixedNameOfSwitch())
                {
                    lastRawArgIdentifiedAsSwitch = commandLineArgs[i];
                    indexOfLastSwitch = i;
                    break;
                }

            //*** Get Last Identified Switch As Switch Object ***
            //***************************************************
            lastSwitch = AllSwitches.GetSwitch(lastRawArgIdentifiedAsSwitch);

            //*** Remember That Last Switch That Is Used ***
            //**********************************************
            //after loop, last switch is known unless no switches were used
            if (!(lastSwitch is null))
                lastSwitch.IsUsed = true;

            //*** Identify All Raw Arguments AS Switch Objects OR Arguments ***
            //*****************************************************************
            foreach (var arg in commandLineArgs)
            {
                //*** * Loop Variables ***
                rawArgIsSwitch = arg.IsPrefixedNameOfSwitch();
                rawArgIsSwitchArg = !rawArgIsSwitch;

                //*** * Identify As Switch? ***
                //if raw argument is identified as Switch, then
                if (rawArgIsSwitch)
                {
                    curSwitch = AllSwitches.GetSwitch(arg);
                    curSwitchStr = arg;

                    //fail command line tool if Switch object has already been used in command line
                    if (curSwitch.IsUsed && curSwitch != lastSwitch)
                    {
                        Console.Error.WriteLine("Switch '" + curSwitchStr + "' has already been used in command line: command line tool ended");
                        #if DEBUG
                        throw new Exception("Switch object was already used in command line");
                        #endif
                        #if RELEASE
                            Environment.Exit(0);
                        #endif
                    }

                    curSwitch.IsUsed = true;
                    isFirstSwitchEncountered = true;
                    isCurSwitchTheLast = curSwitch == lastSwitch;
                }

                //*** * Case: Raw Arg Identified As Starting Stand-Alone Argument ***
                //until first switch is encountered, add arg to StartStandAloneArgs
                if (!isFirstSwitchEncountered)
                {
                    StartStandAloneArgs.Add(arg);
                    StartStandAloneSmartArgs.Add(new SmartArg(arg));
                    continue;
                }

                //*** * Case: Raw Arg Identified As Switch Argument ***
                minMet = curSwitch.Args.Count >= curSwitch.MinArgs;
                maxMet = curSwitch.Args.Count >= curSwitch.MaxArgs;

                //obviously min args must be met, it's a requirement
                if (!minMet)
                    if (rawArgIsSwitchArg || !(isCurSwitchTheLast &&/* not last raw arg to be identified */ indexOfLastSwitch <= commandLineArgs.Length - 1 ))
                    {
                        curSwitch.Args.Add(arg);
                        curSwitch.SmartArgs.Add(new SmartArg(arg));
                    }
                    else /* ERROR */
                    {
                        Console.Error.WriteLine("Min args not met for switch '" + curSwitchStr + "': command line tool ended");
                        #if DEBUG
                            throw new Exception("Min args not met for switch");
                        #endif
                        #if RELEASE
                            Environment.Exit(0);
                        #endif
                    }
                //but if min args is met, we need logic where max args hasn't been met
                if (minMet && !maxMet)
                    if (rawArgIsSwitchArg)
                    {
                        curSwitch.Args.Add(arg);
                        curSwitch.SmartArgs.Add(new SmartArg(arg));
                    }

                //*** * Case: Raw Arg Identified As Ending Stand-Alone Argument ***
                //if max args be met, but current switch is the last switch, then add to EndStandAlone
                if (minMet && maxMet && isCurSwitchTheLast && rawArgIsSwitchArg)
                {
                    EndStandAloneArgs.Add(arg);
                    EndStandAloneSmartArgs.Add(new SmartArg(arg));
                }
            }

            //*** Create Lists AllStandAloneArgs & UsedSwitches ***
            //*****************************************************
            Switch.AllStandAloneArgs.AddRange(StartStandAloneArgs);
            Switch.AllStandAloneArgs.AddRange(EndStandAloneArgs);
            Switch.AllStandAloneSmartArgs.AddRange(StartStandAloneSmartArgs);
            Switch.AllStandAloneSmartArgs.AddRange(EndStandAloneSmartArgs);
            Switch.UsedSwitches = Switch.AllSwitches.GetUsedSwitches();
        }

        /// <summary>
        /// Adds summary documentation to Switch. Documentation is
        /// shown in the help page.
        /// </summary>
        /// <param name="summary"></param>
        public void AddSummary(string summary) => Summary = summary;

        /// <summary>
        /// Adds remarks documentation to Switch. Documentation is
        /// shown in the help page.
        /// </summary>
        /// <param name="remarks"></param>
        public void AddRemarks(string remarks) => Remarks = remarks;

        //commonly used error message
        const string CANT_ADD_PARAMETER = "Switch \"params\" documentation exists. Further parameter documentation can't be added.";
        //*** Add Single Parameter ***
        public void AddParamString(string name)
        {
            if (this.IsParamsAdded)
                throw new InvalidOperationException(CANT_ADD_PARAMETER);
            this.Parameters.Add(new SwitchParameter(
                typeof(string),
                name));
        }
        public void AddParamInt(string name)
        {
            if (this.IsParamsAdded)
                throw new InvalidOperationException(CANT_ADD_PARAMETER);
            this.Parameters.Add(new SwitchParameter(
                typeof(int),
                name));
        }
        public void AddParamDouble(string name)
        {
            if (this.IsParamsAdded)
                throw new InvalidOperationException(CANT_ADD_PARAMETER);
            this.Parameters.Add(new SwitchParameter(
                typeof(double),
                name));
        }
        public void AddParamDecimal(string name)
        {
            if (this.IsParamsAdded)
                throw new InvalidOperationException(CANT_ADD_PARAMETER);
            this.Parameters.Add(new SwitchParameter(
                typeof(decimal),
                name));
        }
        public void AddParamBool(string name)
        {
            if (this.IsParamsAdded)
                throw new InvalidOperationException(CANT_ADD_PARAMETER);
            this.Parameters.Add(new SwitchParameter(
                typeof(bool),
                name));
        }
        public void AddParamCustom(string type, string name)
        {
            if (this.IsParamsAdded)
                throw new InvalidOperationException(CANT_ADD_PARAMETER);
            this.Parameters.Add(new SwitchParameter(
                type,
                name));
        }

        //*** Add Params Parameter ***
        public void AddParamsString(string name)
        {
            if (this.IsParamsAdded)
                throw new InvalidOperationException(CANT_ADD_PARAMETER);
            this.Parameters.Add(new SwitchParameter(
                typeof(string),
                name,
                true));
            this.IsParamsAdded = true;
        }
        public void AddParamsInt(string name)
        {
            if (this.IsParamsAdded)
                throw new InvalidOperationException(CANT_ADD_PARAMETER);
            this.Parameters.Add(new SwitchParameter(
                typeof(int),
                name,
                true));
            this.IsParamsAdded = true;
        }
        public void AddParamsDouble(string name)
        {
            if (this.IsParamsAdded)
                throw new InvalidOperationException(CANT_ADD_PARAMETER);
            this.Parameters.Add(new SwitchParameter(
                typeof(double),
                name,
                true));
            this.IsParamsAdded = true;
        }
        public void AddParamsDecimal(string name)
        {
            if (this.IsParamsAdded)
                throw new InvalidOperationException(CANT_ADD_PARAMETER);
            this.Parameters.Add(new SwitchParameter(
                typeof(decimal),
                name,
                true));
            this.IsParamsAdded = true;
        }
        public void AddParamsBool(string name)
        {
            if (this.IsParamsAdded)
                throw new InvalidOperationException(CANT_ADD_PARAMETER);
            this.Parameters.Add(new SwitchParameter(
                typeof(bool),
                name,
                true));
            this.IsParamsAdded = true;
        }
        public void AddParamsCustom(string type, string name)
        {
            if (this.IsParamsAdded)
                throw new InvalidOperationException(CANT_ADD_PARAMETER);
            this.Parameters.Add(new SwitchParameter(
                type,
                name,
                true));
            this.IsParamsAdded = true;
        }

        public override bool Equals(object obj)
        {
            if (obj is Switch)
                return this.Name      == (obj as Switch).Name
                    && this.Name1Char == (obj as Switch).Name1Char;
            else
                return false;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public static bool operator ==(Switch s1, Switch s2) {
            try
            {
                return s1.Equals(s2);
            }
            catch(NullReferenceException)
            {
                return false;
            }
        }

        public static bool operator !=(Switch s1, Switch s2) {
            try
            {
                return !s1.Equals(s2);
            }
            catch (NullReferenceException)
            {
                return true;
            }
        }
    }

    public class SwitchParameter
    {
        public Type ParamType = typeof(string);
        //only used when type is user defined or not a given type
        public string ParamTypeStr = "";
        public string Name = "";
        public bool IsParams = false;

        public SwitchParameter(Type type, string name, bool isParams = false)
        {
            this.ParamType = type;
            this.Name = name;
            this.IsParams = isParams;
        }

        public SwitchParameter(string type, string name, bool isParams = false)
        {
            this.ParamTypeStr = type;
            this.Name = name;
            this.IsParams = isParams;
        }
    }

    public static class SwitchParameterExtensionMethods {
       
    }

        public static class ExtensionMethods
        {
            /// <summary>
            /// Confirms if a string is formated as a switch name.
            /// The format is defined by the second argument.
            /// </summary>
            /// <param name="str">string that's examined</param>
            /// <param name="regexPattern">Regular expression defining the switch name format</param>
            /// <returns></returns>
            /// <seealso cref="ExtensionMethods.IsSwitchFormat(string)"/>
            /// <seealso cref="ExtensionMethods.IsSwitchPrefixFormat(string)"/>
            [MethodImpl(methodImplOptions: MethodImplOptions.AggressiveInlining)]
            private static bool IsSwitchFormat_InlinedGuts(this string str, string regexPattern)
            {
                //*** Argument & Variable Prep ***
                //********************************
                //lower-case argument
                str = str.ToLower();

                Match valueMatched = Regex.Match(str, regexPattern);

                //*** Ensure Valid Switch Name Format ***
                //***************************************
                //test if string has switch prefix "-", "--", or "/" (regex match success should be false if the prior mentioned is true)
                if (valueMatched.Success == false)
                    return false;

                //prevent strings like "-a-x", "/a/ba", "--bo--bo", etc. from being considered a proper switch (i.e. "-h", "--help", "/h")
                if (!valueMatched.Value.Equals(str))
                    return false;

                //fail test if switch's name is not allowed size
                if (    /* is switch's single-character-name, with prefix "-" or "/" removed, and is not 1 characters long */ valueMatched.Value.Replace("-", "").Replace("/", "").GetUTF8Length() != 1
                    && (/* is switch's multi-character-name, with prefix "--" removed, and is less than 2 characters */ valueMatched.Value.Replace("--", "").GetUTF8Length() < 2))
                    return false;

                return true;
            }

            /// <summary>
            /// Returns true if the string is formated as a switch name (single-character-name or multi-character-name).
            /// </summary>
            /// <param name="str"></param>
            /// <returns></returns>
            public static bool IsSwitchFormat(this string str)
            {
                return str.IsSwitchFormat_InlinedGuts(@"^[^-\/\s]+(-[^-\/\s]+)*$");
            }

            /// <summary>
            /// Returns true if the string is formated as a switch name (single-character-name or multi-character-name).
            /// with a switch prefix
            /// </summary>
            /// <param name="str"></param>
            /// <returns></returns>
            public static bool IsSwitchPrefixFormat(this string str)
            {
                return str.IsSwitchFormat_InlinedGuts(@"^(--[^-\/\s]+(-[^-\/\s]+)*|(-|\/)[^-\/\s])$");
            }

            /// <summary>
            /// Returns true if string is name of existing switch (i.e. "switch", "s").
            /// </summary>
            /// <param name="name">multi-character-name or single-character-name (with or without prefix)</param>
            /// <returns></returns>
            public static bool IsNameOfSwitch(this string name)
            {
                //*** Ensure Name Is Of Preferred Format ***
                //*****************************************
                if (!name.IsSwitchFormat())
                    return false;

                name = name.ToLower();

                //*** Ensure Switch Name Is Of Existing Switch ***
                //************************************************
                //Fail test if no created switch has the name
                if (!Switch.AllSwitches.Exists(s => s.Name == name || (s.Name1Char == name && !String.IsNullOrEmpty(s.Name1Char))))
                    return false;

                return true;
            }

            /// <summary>
            /// <para>
            /// Returns true if string is name of existing switch -- with prefix (i.e. "--switch", "-s", "/s").
            /// </para>
            /// <para>
            /// Should be used directly on a program's command line arguments.
            /// </para>
            /// </summary>
            /// <remarks>
            /// Prefix is short for "switch prefix" in this method's name.
            /// </remarks>
            /// <param name="name">multi-character-name or single-character-name (with or without prefix)</param>
            /// <returns></returns>
            public static bool IsPrefixedNameOfSwitch(this string name)
            {
                //*** Ensure Name Is Of Preferred Format ***
                //*****************************************
                if (!name.IsSwitchPrefixFormat())
                    return false;

                name = name.Replace("-", "").Replace("/", "").ToLower();

                //*** Ensure Switch Name Is Of Existing Switch ***
                //************************************************
                //Fail test if no created switch has the name
                if (!Switch.AllSwitches.Exists(s => s.Name == name || (s.Name1Char == name && !String.IsNullOrEmpty(s.Name1Char))))
                    return false;

                return true;
            }

            /// <summary>
            /// Gets the total UTF-8 text elements in a string. The need for this method can be shown
            /// with "🦄". It counts as 2 characters if its length is retrieved, which is problematic if one is
            /// counting emoji. Thankfully, this method returns 1 for 1 unicorn text element.
            /// </summary>
            /// <param name="str"></param>
            /// <returns></returns>
            public static int GetUTF8Length(this string str)
            {
                StringInfo strInfo = new StringInfo(str);
                return strInfo.LengthInTextElements;
            }
        }
    }
}
