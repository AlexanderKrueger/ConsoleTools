using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ConsoleToolsLibrary
{
    namespace MainArgsUtil
    {
        /// <summary>
        /// The smarter command line argument. Takes string argument and provides access to a copy of 
        /// a more suitable data-type. It should only be used for command line arguments; issues will
        /// occur if otherwise is done.
        /// </summary>
        /// <example>
        /// <code>
        /// //Command Line:
        /// // CommandLineTool 5 4 true "true" "bob"
        /// //
        /// //Command Line Arguments (in args[]):
        /// // (1) "5"
        /// // (2) "4.5"
        /// // (3) "true" (remember all arguments are treated as strings)
        /// // (4) "true"
        /// // (5) "bob"
        /// public static void Main(string[] args)
        /// {
        ///     //Declarations form #1 (Creation through implicit casting)
        ///     SmartArg firstNum = args[0] /* 5 */;
        ///     SmartArg secondNum = args[1] /* 4.5 */;
        ///     SmartArg firstBool = args[2] /* true */;
        ///     SmartArg secondBool = args[3] /* true */;
        ///     SmartArg Str = args[4] /* "bob" */;
        ///     
        ///     //Declarations form #2 (Creation: normal)
        ///     //SmartArg firstNum = new SmartArg(args[0]) /* 5 */;
        ///     //SmartArg secondNum = new SmartArg(args[1]) /* 4.5 */;
        ///     //SmartArg firstBool = new SmartArg(args[2]) /* true */;
        ///     //SmartArg secondBool = new SmartArg(args[3]) /* true */;
        ///     //SmartArg Str = new SmartArg(args[4]) /* "bob" */;
        ///     
        ///     //Declarations form #3 (No exceptions thrown: provides 'index out of range' safety for array "args")
        ///     //SmartArg firstNum = new SmartArg(args) /* 5 */;
        ///     //SmartArg secondNum = new SmartArg(args) /* 4.5 */;
        ///     //SmartArg firstBool = new SmartArg(args) /* true */;
        ///     //SmartArg secondBool = new SmartArg(args) /* true */;
        ///     //SmartArg Str = new SmartArg(args) /* "bob" */;
        ///     
        ///     //Declarations form #4 (No exceptions thrown)
        ///     //SmartArg.SetArgs(args);
        ///     //SmartArg firstNum = SmartArg.GetArg() /* 5 */;
        ///     //SmartArg secondNum = SmartArg.GetArg() /* 4.5 */;
        ///     //SmartArg firstBool = SmartArg.GetArg() /* true */;
        ///     //SmartArg secondBool = SmartArg.GetArg() /* true */;
        ///     //SmartArg Str = SmartArg.GetArg() /* "bob" */;
        ///     //
        ///     //// Example of attempted retrieval of SmartArg object not derived from any command line argument
        ///     //SmartArg NotArg = SmartArg.GetArg() /* null */;
        ///     
        ///     //Values can be easily stored - if you wish
        ///     int storage = firstNum.Value;
        ///     
        ///     //Alternatively, you can get the original string value
        ///     string storage2 = firstNum.ValueStr;
        ///     
        ///     //Changes to a SmartArg are allowed. Whether you should is a different story.
        ///     firstNum.Value = 89;
        ///     
        ///     //Changes are accessible in string form - not that you need it
        ///     Console.WriteLine(firstNum.ValueStr);
        ///     
        ///     //You don't need to do much for operations. It's recommended to
        ///     //do it as shown below
        ///     int result = firstNum + secondNum;
        ///     
        ///     bool result2A = firstBool & secondBool;
        ///     
        ///     bool result3A = firstBool | secondBool;
        ///     
        ///     //Not every operation is possible, at least without casting
        ///     //
        ///     //[not possible]
        ///     //bool result2B = firstBool && secondBool;
        ///     //
        ///     //bool result3B = firstBool || secondBool;
        ///     
        ///     /* WARNING:
        ///      * Nothing has been exhaustively tested, but most of the
        ///      * stuff seems to work.
        ///      */
        /// }
        /// </code>
        /// </example>
        public class SmartArg
        {
            /// <summary>
            /// It is what it sounds like; yes, it seems redundant; no, it should not be removed.
            /// It allows the automatic linking of SmartArg objects
            /// </summary>
            private static SmartArg PrevDeclaredSmartArg = null;
            /// <summary>
            /// SmartArg that was initialized before this SmartArg
            /// </summary>
            public SmartArg PrevSmartArg = null;
            /// <summary>
            /// SmartArg that was initialized after this SmartArg - if any
            /// </summary>
            public SmartArg NextSmartArg = null;
            /// <summary>
            /// Is this the first SmartArg initialized
            /// </summary>
            public bool IsFirstInit { get { return /* true if PrevSmartArg is null */ PrevSmartArg != (object)null ? false : true; } }
            /// <summary>
            /// Is this the last (at the moment of execution) SmartArg initialized
            /// </summary>
            public bool IsLastInit { get { return /* true if NextSmartArg is null */ NextSmartArg != (object)null ? false : true; } }
            /// <summary>
            /// Index of SmartArg in the array (though it's closer to a linked list - despite the index)
            /// </summary>
            public int Index = 0;
            private string _ValueStr = "";
            private bool IsValueString = false;
            private int _ValueInt = 0;
            private bool IsValueInt = false;
            private double _ValueDouble = 0.0;
            private bool IsValueDouble = false;
            private bool _ValueBool = false;
            private bool IsValueBool = false;

            private static Regex RegexInteger = new Regex(@"^\d+$");
            private static Regex RegexDouble = new Regex(@"^\d*[.]\d+$");
            private static Regex RegexBoolean = new Regex(@"^(true|false)$");
            private static Regex RegexSwitch = new Regex(@"^(--[a-zA-Z_][a-zA-Z_0-9]+|-[a-zA-Z_]|/[a-zA-Z_])$");

            /// <summary>
            /// Command line argument gets shoved into here as a string and then
            /// copied elsewhere as the next best data-type (int, bool, double). This, of course,
            /// is if the next best data-type isn't a string.
            /// </summary>
            public string ValueStr
            {
                get { return _ValueStr; }
                set
                {
                    this.IsValueInt = false;
                    this.IsValueDouble = false;
                    this.IsValueBool = false;
                    this.IsValueString = false;

                    Value = this.ToFreqType(value);
                    _ValueStr = value;
                }
            }

            /// <summary>
            /// If possible, the value of "ValueStr" converted to a different data-type (i.e. "4" --> 4).
            /// Data-types are limited to "string", "int", "double", and "bool"
            /// </summary>
            public object Value
            {
                get
                {
                    if (this.IsValueInt) { return this._ValueInt; }
                    if (this.IsValueDouble) { return this._ValueDouble; }
                    if (this.IsValueBool) { return this._ValueBool; }
                    else /*if this.IsValueString */ { return this._ValueStr; }
                }
                set
                {
                    this._ValueInt = 0;
                    this._ValueDouble = 0.0;
                    this._ValueBool = false;
                    this._ValueStr = "";

                    _ValueStr = value.ToString();
                    if (value is Int32) { this._ValueInt = (Int32)value; return; }
                    if (value is Double) { this._ValueDouble = (Double)value; return; }
                    if (value is Boolean) { this._ValueBool = (Boolean)value; return; }
                    //else do nothing
                }
            }

            private static List<SmartArg> ParsedArgs = new List<SmartArg>();
            private static int ParsedArgsIndex = 0;

            /// <summary>
            /// Dummy constructor for purpose of creating named reference
            /// objects. No side effects occur from use of the constructor.
            /// </summary>
            public SmartArg()
            {
                // Dummy constructor
            }

            /// <summary>
            /// Constructor that has side effects that impact previously created SmartArg objects
            /// </summary>
            /// <param name="stringArg"></param>
            public SmartArg(string stringArg)
            {
                this.PrevSmartArg = SmartArg.PrevDeclaredSmartArg;
                this.Index = (this.PrevSmartArg?.Index ?? -1) + 1;
                this.ValueStr = stringArg ?? "";

                //*** Keep These Statements Last ***
                //make this SmartArg the previously declared SmartArg's "NextSmartArg"
                if (SmartArg.PrevDeclaredSmartArg != (object)null) { SmartArg.PrevDeclaredSmartArg.NextSmartArg = this; }
                //for the next SmartArg's creation, make this SmartArg the previously declared SmartArg
                SmartArg.PrevDeclaredSmartArg = this;
            }

            public SmartArg(string[] stringArgs)
            {
                this.PrevSmartArg = SmartArg.PrevDeclaredSmartArg;
                this.Index = (this.PrevSmartArg?.Index ?? -1) + 1;
                try
                {
                    this.ValueStr = stringArgs[this.Index] ?? "";
                }
                catch (IndexOutOfRangeException)
                {
                    this.ValueStr = "";
                }

                //*** Keep These Statements Last ***
                //make this SmartArg the previously declared SmartArg's "NextSmartArg"
                if (SmartArg.PrevDeclaredSmartArg != (object)null) { SmartArg.PrevDeclaredSmartArg.NextSmartArg = this; }
                //for the next SmartArg's creation, make this SmartArg the previously declared SmartArg
                SmartArg.PrevDeclaredSmartArg = this;
            }
            /// <summary>
            /// Returns string value of SmartArg object in all upper case
            /// </summary>
            /// <returns></returns>
            public string ToUpper() { return this.ValueStr.ToUpper(); }
            /// <summary>
            /// Returns string value of SmartArg object in all lower case
            /// </summary>
            /// <returns></returns>
            public string ToLower() { return this.ValueStr.ToLower(); }
            /// <summary>
            /// Does a regular expression match on SmartArg object's string value
            /// </summary>
            /// <param name="pattern"></param>
            /// <returns></returns>
            public Match Match(string pattern) { return Regex.Match(this.ValueStr, pattern); }
            /// <summary>
            /// Short for "Match Ignore Case". Does as said in the context of regular expressions.
            /// </summary>
            /// <param name="pattern"></param>
            /// <returns></returns>
            public Match MatchIC(string pattern) { return Regex.Match(this.ValueStr, pattern, RegexOptions.IgnoreCase); }

            /// <summary>
            /// Parses the command line arguments into SmartArg objects. After this method
            /// is invoked, <see cref="GetArg()"/>
            /// can return the parsed (one per call).
            /// </summary>
            /// <seealso cref="GetArg()"/>
            /// <param name="args"></param>
            public static void SetArgs(string[] args)
            {
                foreach (var arg in args)
                {
                    SmartArg.ParsedArgs.Add(new SmartArg(arg));
                }
            }

            /// <summary>
            /// Returns the next of the parsed command line arguments after using the
            /// <see cref="SetArgs(string[])"/> method. If there is/no more SmartArgs to return, then
            /// a dummy SmartArg object is returned.
            /// </summary>
            /// <seealso cref="SetArgs(string[])"/>
            /// <returns></returns>
            public static SmartArg GetArg()
            {
                if (ParsedArgsIndex == ParsedArgs.Count())
                    return new SmartArg();

                SmartArg returned;
                returned = ParsedArgs[ParsedArgsIndex];
                ParsedArgsIndex++;
                return returned;
            }

            /// <summary>
            /// Unlinks the current SmartArg from the linked list.
            /// Unlinking a SmartArg doesn't make it available for garbage collection.
            /// </summary>
            public void UnLink()
            {
                SmartArg curSmartArg = this;

                //Link previous SmartArg object to next SmartArg object
                if (curSmartArg.HasPrevSmartArg && curSmartArg.HasNextSmartArg)
                    curSmartArg.PrevSmartArg = curSmartArg.NextSmartArg;

                //Unlink this SmartArg from the others
                this.PrevSmartArg = null;
                this.NextSmartArg = null;

                //Correct index values of afterward SmartArg objects
                while (curSmartArg.HasNextSmartArg)
                {
                    curSmartArg = curSmartArg.NextSmartArg;
                    curSmartArg.Index--;
                }
            }

            /// <summary>
            /// Shifts the value of both this SmartArg and all afterward SmartArg objects
            /// to their next SmartArg. As an intended side effect, the invoking SmartArg is unlinked
            /// from it's connected SmartArg objects.
            /// </summary>
            /// <example>
            /// if:
            /// "b" is SmartArg in question
            /// 
            /// before:
            /// [a] [b] [c] [d] [e] (SmartArg)
            /// [1] [2] [3] [4] [ ] (SmartArg value)
            /// 
            /// after:
            /// [a] --- [c] [d] [e] (SmartArg)
            /// [1] --- [2] [3] [4] (SmartArg value)
            /// 
            /// </example>
            public void Shift()
            {
                SmartArg curSmartArg = this;

                //create list of all afterward SmartArg objects
                List<SmartArg> afterwardSmartArgs = new List<SmartArg>();
                while (curSmartArg.HasNextSmartArg)
                {
                    curSmartArg = curSmartArg.NextSmartArg;
                    afterwardSmartArgs.Add(curSmartArg);
                }

                //reset curSmartArg for next enumeration
                curSmartArg = this;

                //remove last element in list
                afterwardSmartArgs.RemoveAt(afterwardSmartArgs.Count - 1);

                //perform shift
                for (int i = 0; i < afterwardSmartArgs.Count; i--)
                    afterwardSmartArgs[i].ValueStr = afterwardSmartArgs[i].PrevSmartArg.ValueStr;

                //unlink this SmartArg object from its connected SmartArg objects
                this.UnLink();
            }

            public bool HasNextSmartArg { get { return this.NextSmartArg != (object)null; } }
            public bool HasPrevSmartArg { get { return this.PrevSmartArg != (object)null; } }

            /// <summary>
            /// Returns a frequently used data-type that can be converted from the string argument.
            /// If a conversion can't be made, the argument is returned "as is".
            /// </summary>
            /// <remarks>
            /// The data-type "char" could be considered a frequently used data-type, but
            /// single characters have also been stored in strings due to need or desire.
            /// Because of this, along with the need for simple method design,
            /// and considering the simplicity of converting single character strings to "char" data-type
            /// , "char" data-types can only be derived after the method's execution
            /// </remarks>
            /// <param name="str"></param>
            /// <returns>int, double, bool, or string</returns>
            private object ToFreqType(string str)
            {
                //if string only contains integer
                if (RegexInteger.IsMatch(str))
                    try
                    {
                        this.IsValueInt = true;
                        return Convert.ToInt32(str);
                    }
                    catch (OverflowException)
                    {
                        Console.Error.WriteLine("Overflow Error: Integer " + str + "could not be stored in the \"int\" data-type");
                        Console.WriteLine("[Program Terminated]");
                        Environment.Exit(0);
                        return str;
                    }
                //if string only contains decimal
                else if (RegexDouble.IsMatch(str))
                    try
                    {
                        this.IsValueDouble = true;
                        return Convert.ToDouble(str);
                    }
                    catch (OverflowException)
                    {
                        Console.Error.WriteLine("Overflow Error: Double " + str + "could not be stored in the \"double\" data-type");
                        Console.WriteLine("[Program Terminated]");
                        Environment.Exit(0);
                        return str;
                    }
                //if string only contains boolean
                else if (RegexBoolean.IsMatch(str))
                {
                    this.IsValueBool = true;
                    return Boolean.Parse(str);
                }
                //else the best type to return is string
                else
                {
                    this.IsValueString = true;
                    return str;
                }
            }

            public static implicit operator string(SmartArg smartArg)
            {
                return smartArg.ValueStr;
            }
            public static implicit operator SmartArg(string stringArg)
            {
                return new SmartArg(stringArg);
            }

            public static implicit operator bool(SmartArg smartArg)
            {
                if (smartArg.IsValueBool) { return (bool)smartArg.Value; }
                else
                    throw new InvalidCastException();
            }
            public static implicit operator SmartArg(bool boolArg)
            {
                return new SmartArg(boolArg.ToString());
            }

            public static implicit operator int(SmartArg smartArg)
            {
                if (smartArg.IsValueInt) { return (int)smartArg.Value; }
                else
                    throw new InvalidCastException();
            }
            public static implicit operator SmartArg(int intArg)
            {
                return new SmartArg(intArg.ToString());

            }

            public static implicit operator double(SmartArg smartArg)
            {
                if (smartArg.IsValueBool) { return (double)smartArg.Value; }
                else
                    throw new InvalidCastException();
            }
            public static implicit operator SmartArg(double doubleArg)
            {
                return new SmartArg(doubleArg.ToString());
            }

            public static object operator +(SmartArg s1, SmartArg s2)
            {
                if (s1.IsValueInt && s2.IsValueInt) { return (int)s1.Value + (int)s2.Value; }
                if (s1.IsValueDouble && s2.IsValueDouble) { return (double)s1.Value + (double)s2.Value; }
                if (s1.IsValueBool && s2.IsValueBool) { return (bool)s1.Value || (bool)s2.Value; }
                if (s1.IsValueString && s2.IsValueString) { return (string)s1.Value + (string)s2.Value; }
                else
                    throw new InvalidOperationException("Operations on SmartArgs requires both objects to be of the same auto interpreted type (i.e. \"4\" --> 4; interpreted type 'int')");
            }
            public static object operator +(SmartArg s, Int32 i)
            {
                if (s.IsValueInt) { return (int)s.Value + i; }
                if (s.IsValueDouble) { return (double)s.Value + (double)i; }
                else
                    throw new InvalidOperationException();
            }
            public static object operator +(SmartArg s, Double d)
            {
                if (s.IsValueInt) { return (double)s.Value + d; }
                if (s.IsValueDouble) { return (double)s.Value + d; }
                else
                    throw new InvalidOperationException();
            }
            public static object operator +(SmartArg s, String str)
            {
                if (s.IsValueString) { return s.Value + str; }
                else
                    return s.Value.ToString() + str;
            }

            public static object operator -(SmartArg s1, SmartArg s2)
            {
                if (s1.IsValueInt && s2.IsValueInt) { return (int)s1.Value - (int)s2.Value; }
                if (s1.IsValueDouble && s2.IsValueDouble) { return (double)s1.Value - (double)s2.Value; }
                if (s1.IsValueBool && s2.IsValueBool) { return false; }
                if (s1.IsValueString && s2.IsValueString) { return (s1.Value as String).Replace((string)s2.Value, ""); }
                else
                    throw new InvalidOperationException("Operations on SmartArgs requires both objects to be of the same auto interpreted type (i.e. \"4\" --> 4; interpreted type 'int')");
            }
            public static object operator -(SmartArg s, Int32 i)
            {
                if (s.IsValueInt) { return (int)s.Value - i; }
                if (s.IsValueDouble) { return (double)s.Value - i; }
                else
                    throw new InvalidOperationException();
            }
            public static object operator -(SmartArg s, Double d)
            {
                if (s.IsValueInt) { return (double)s.Value - d; }
                if (s.IsValueDouble) { return (double)s.Value - d; }
                else
                    throw new InvalidOperationException();
            }
            //reverse
            public static object operator -(Int32 i, SmartArg s)
            {
                if (s.IsValueInt) { return i - (int)s.Value; }
                if (s.IsValueDouble) { return i - (double)s.Value; }
                else
                    throw new InvalidOperationException();
            }
            public static object operator -(Double d, SmartArg s)
            {
                if (s.IsValueInt) { return d - (double)s.Value; }
                if (s.IsValueDouble) { return d - (double)s.Value; }
                else
                    throw new InvalidOperationException();
            }

            public static object operator *(SmartArg s1, SmartArg s2)
            {
                if (s1.IsValueInt && s2.IsValueInt) { return (int)s1.Value * (int)s2.Value; }
                if (s1.IsValueDouble && s2.IsValueDouble) { return (double)s1.Value * (double)s2.Value; }
                if (s1.IsValueBool && s2.IsValueBool) { return (bool)s1.Value && (bool)s2.Value; }
                if (s1.IsValueString && s2.IsValueString)
                {
                    throw new InvalidOperationException("SmartArgs that represent strings can't be divided by SmartArgs representing the same type");
                }
                else
                    throw new InvalidOperationException("Operations on SmartArgs requires both objects to be of the same auto interpreted type (i.e. \"4\" --> 4; interpreted type 'int')");
            }
            public static object operator *(SmartArg s, Int32 i)
            {
                if (s.IsValueInt) { return (int)s.Value * i; }
                if (s.IsValueDouble) { return (double)s.Value * i; }
                else
                    throw new InvalidOperationException();
            }
            public static object operator *(SmartArg s, Double d)
            {
                if (s.IsValueInt) { return (double)s.Value * d; }
                if (s.IsValueDouble) { return (double)s.Value * d; }
                else
                    throw new InvalidOperationException();
            }
            //reverse
            public static object operator *(Int32 i, SmartArg s)
            {
                if (s.IsValueInt) { return i * (int)s.Value; }
                if (s.IsValueDouble) { return i * (double)s.Value; }
                else
                    throw new InvalidOperationException();
            }
            public static object operator *(Double d, SmartArg s)
            {
                if (s.IsValueInt) { return d * (double)s.Value; }
                if (s.IsValueDouble) { return d * (double)s.Value; }
                else
                    throw new InvalidOperationException();
            }

            public static object operator /(SmartArg s1, SmartArg s2)
            {
                if (s1.IsValueInt && s2.IsValueInt) { return (int)s1.Value / (int)s2.Value; }
                if (s1.IsValueDouble && s2.IsValueDouble) { return (double)s1.Value / (double)s2.Value; }
                if (s1.IsValueBool && s2.IsValueBool) { return (bool)s1.Value && (bool)s2.Value; }
                if (s1.IsValueString && s2.IsValueString)
                {
                    throw new InvalidOperationException("SmartArgs that represent strings can't be divided by SmartArgs representing the same type");
                }
                else
                    throw new InvalidOperationException("Operations on SmartArgs requires both objects to be of the same auto interpreted type (i.e. \"4\" --> 4; interpreted type 'int')");
            }
            public static object operator /(SmartArg s, Int32 i)
            {
                if (s.IsValueInt) { return (int)s.Value / i; }
                if (s.IsValueDouble) { return (double)s.Value / i; }
                else
                    throw new InvalidOperationException();
            }
            public static object operator /(SmartArg s, Double d)
            {
                if (s.IsValueInt) { return (double)s.Value / d; }
                if (s.IsValueDouble) { return (double)s.Value / d; }
                else
                    throw new InvalidOperationException();
            }
            //reverse
            public static object operator /(Int32 i, SmartArg s)
            {
                if (s.IsValueInt) { return i / (int)s.Value; }
                if (s.IsValueDouble) { return i / (double)s.Value; }
                else
                    throw new InvalidOperationException();
            }
            public static object operator /(Double d, SmartArg s)
            {
                if (s.IsValueInt) { return d / (double)s.Value; }
                if (s.IsValueDouble) { return d / (double)s.Value; }
                else
                    throw new InvalidOperationException();
            }

            public static object operator &(SmartArg s1, SmartArg s2)
            {
                if (s1.IsValueInt && s2.IsValueInt) { return (int)s1.Value & (int)s2.Value; }
                if (s1.IsValueBool && s2.IsValueBool) { return (bool)s1.Value & (bool)s2.Value; }
                else
                    throw new InvalidOperationException();
            }
            public static int operator &(SmartArg s, Int32 i)
            {
                if (s.IsValueInt) { return (int)s.Value & i; }
                else
                    throw new InvalidOperationException();
            }
            public static bool operator &(SmartArg s, Boolean b)
            {
                if (s.IsValueBool) { return (bool)s.Value & b; }
                else
                    throw new InvalidOperationException();
            }
            //reverse
            public static int operator &(Int32 i, SmartArg s)
            {
                if (s.IsValueInt) { return i & (int)s.Value; }
                else
                    throw new InvalidOperationException();
            }
            public static bool operator &(Boolean b, SmartArg s)
            {
                if (s.IsValueBool) { return b & (bool)s.Value; }
                else
                    throw new InvalidOperationException();
            }

            public static object operator |(SmartArg s1, SmartArg s2)
            {
                if (s1.IsValueInt && s2.IsValueInt) { return (int)s1.Value | (int)s2.Value; }
                if (s1.IsValueBool && s2.IsValueBool) { return (bool)s1.Value | (bool)s2.Value; }
                else
                    throw new InvalidOperationException();
            }
            public static int operator |(SmartArg s, Int32 i)
            {
                if (s.IsValueInt) { return (int)s.Value | i; }
                else
                    throw new InvalidOperationException();
            }
            public static bool operator |(SmartArg s, Boolean b)
            {
                if (s.IsValueBool) { return (bool)s.Value | b; }
                else
                    throw new InvalidOperationException();
            }
            //reverse
            public static int operator |(Int32 i, SmartArg s)
            {
                if (s.IsValueInt) { return i | (int)s.Value; }
                else
                    throw new InvalidOperationException();
            }
            public static bool operator |(Boolean b, SmartArg s)
            {
                if (s.IsValueBool) { return b | (bool)s.Value; }
                else
                    throw new InvalidOperationException();
            }

            public static bool operator !(SmartArg s)
            {
                if (s.IsValueBool) { return !(bool)s.Value; }
                else
                    throw new InvalidOperationException();
            }

            public static bool operator ==(SmartArg s1, SmartArg s2)
            {
                if ((object)s1 == null && (object)s2 == null)
                    return true;
                if (((object)s1 == null && (object)s2 != null) || ((object)s1 != null && (object)s2 == null))
                    return false;
                else
                    return s1.Value == s2.Value;
            }
            public static bool operator ==(SmartArg s, Int32 i)
            {
                if (s.IsValueInt) { return (int)s.Value == i; }
                if (s.IsValueDouble) { return (double)s.Value == i; }
                else
                    throw new InvalidOperationException();
            }
            public static bool operator ==(SmartArg s, Double d)
            {
                if (s.IsValueDouble) { return (double)s.Value == d; }
                if (s.IsValueInt) { return (int)s.Value == d; }
                else
                    throw new InvalidOperationException();
            }
            public static bool operator ==(SmartArg s, Boolean b)
            {
                if (s.IsValueBool) { return (bool)s.Value == b; }
                else
                    throw new InvalidOperationException();
            }
            public static bool operator ==(SmartArg s, String str)
            {
                if (s.IsValueString) { return (string)s.Value == str; }
                else
                    throw new InvalidOperationException();
            }
            //reverse
            public static bool operator ==(Int32 i, SmartArg s)
            {
                if (s.IsValueInt) { return i == (int)s.Value; }
                if (s.IsValueDouble) { return i == (double)s.Value; }
                else
                    throw new InvalidOperationException();
            }
            public static bool operator ==(Double d, SmartArg s)
            {
                if (s.IsValueDouble) { return d == (double)s.Value; }
                if (s.IsValueInt) { return d == (int)s.Value; }
                else
                    throw new InvalidOperationException();
            }
            public static bool operator ==(Boolean b, SmartArg s)
            {
                if (s.IsValueBool) { return b == (bool)s.Value; }
                else
                    throw new InvalidOperationException();
            }
            public static bool operator ==(String str, SmartArg s)
            {
                if (s.IsValueString) { return str == (string)s.Value; }
                else
                    throw new InvalidOperationException();
            }

            public static bool operator !=(SmartArg s1, SmartArg s2)
            {
                if ((object)s1 == null && (object)s2 == null)
                    return false;
                if (((object)s1 == null && (object)s2 != null) || ((object)s1 != null && (object)s2 == null))
                    return true;
                else
                    return s1.Value != s2.Value;
            }
            public static bool operator !=(SmartArg s, Int32 i)
            {
                if (s.IsValueInt) { return !((int)s.Value == i); }
                if (s.IsValueDouble) { return !((double)s.Value == i); }
                else
                    throw new InvalidOperationException();
            }
            public static bool operator !=(SmartArg s, Double d)
            {
                if (s.IsValueDouble) { return !((double)s.Value == d); }
                if (s.IsValueInt) { return !((int)s.Value == d); }
                else
                    throw new InvalidOperationException();
            }
            public static bool operator !=(SmartArg s, Boolean b)
            {
                if (s.IsValueBool) { return !((bool)s.Value == b); }
                else
                    throw new InvalidOperationException();
            }
            public static bool operator !=(SmartArg s, String str)
            {
                if (s.IsValueString) { return !((string)s.Value == str); }
                else
                    throw new InvalidOperationException();
            }
            //reverse
            public static bool operator !=(Int32 i, SmartArg s)
            {
                if (s.IsValueInt) { return !(i == (int)s.Value); }
                if (s.IsValueDouble) { return !(i == (double)s.Value); }
                else
                    throw new InvalidOperationException();
            }
            public static bool operator !=(Double d, SmartArg s)
            {
                if (s.IsValueDouble) { return !(d == (double)s.Value); }
                if (s.IsValueInt) { return !(d == (int)s.Value); }
                else
                    throw new InvalidOperationException();
            }
            public static bool operator !=(Boolean b, SmartArg s)
            {
                if (s.IsValueBool) { return !(b == (bool)s.Value); }
                else
                    throw new InvalidOperationException();
            }
            public static bool operator !=(String str, SmartArg s)
            {
                if (s.IsValueString) { return !(str == (string)s.Value); }
                else
                    throw new InvalidOperationException();
            }

            public static int operator >>(SmartArg s1, Int32 i)
            {
                if (s1.IsValueInt || s1.IsValueDouble) { return (int)s1.Value >> i; }
                else
                    throw new InvalidOperationException();
            }
            public static int operator <<(SmartArg s1, Int32 i)
            {
                if (s1.IsValueInt || s1.IsValueDouble) { return (int)s1.Value << i; }
                else
                    throw new InvalidOperationException();
            }

            public override bool Equals(object obj)
            {
                if (obj is SmartArg)
                    return this.ValueStr == (obj as SmartArg).ValueStr;
                else
                    return false;
            }

            public override int GetHashCode()
            {
                return base.GetHashCode();
            }
        }
    }
}
