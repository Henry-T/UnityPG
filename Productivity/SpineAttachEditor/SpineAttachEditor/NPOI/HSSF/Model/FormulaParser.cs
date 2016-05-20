/* ====================================================================
   Licensed To the Apache Software Foundation (ASF) Under one or more
   contributor license agreements.  See the NOTICE file distributed with
   this work for Additional information regarding copyright ownership.
   The ASF licenses this file To You Under the Apache License, Version 2.0
   (the "License"); you may not use this file except in compliance with
   the License.  You may obtain a copy of the License at

       http://www.apache.org/licenses/LICENSE-2.0

   Unless required by applicable law or agreed To in writing, software
   distributed Under the License Is distributed on an "AS Is" BASIS,
   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
   See the License for the specific language governing permissions and
   limitations Under the License.
==================================================================== */

namespace NPOI.HSSF.Model
{
    using System;
    using System.Text;
    using System.Collections;
    using System.Text.RegularExpressions;

    using NPOI.HSSF.Record.Formula;
    using NPOI.HSSF.Record.Formula.Function;
    using NPOI.HSSF.UserModel;

    //import PTG's .. since we need everything, import *

    /**
     * This class Parses a formula string into a List of Tokens in RPN order.
     * Inspired by
     *           Lets Build a Compiler, by Jack Crenshaw
     * BNF for the formula expression Is :
     * <expression> ::= <term> [<Addop> <term>]*
     * <term> ::= <factor>  [ <mulop> <factor> ]*
     * <factor> ::= <number> | (<expression>) | <cellRef> | <function>
     * <function> ::= <functionName> ([expression [, expression]*])
     *
     *  @author Avik Sengupta <avik at apache dot org>
     *  @author Andrew C. oliver (acoliver at apache dot org)
     *  @author Eric Ladner (eladner at goldinc dot com)
     *  @author Cameron Riley (criley at ekmail.com)
     *  @author Peter M. Murray (pete at quantrix dot com)
     *  @author Pavel Krupets (pkrupets at palmtreebusiness dot com)
     */
    public class FormulaParser
    {

        /**
         * Specific exception thrown when a supplied formula does not Parse properly.<br/>
         * Primarily used by test cases when testing for specific parsing exceptions.</p>
         *
         */
        class FormulaParseException : Exception
        {
            // This class was given package scope Until it would become clear that it Is useful To
            // general client code.
            public FormulaParseException(String msg):base(msg)
            {
                
            }
        }

        public const int FORMULA_TYPE_CELL = 0;
        public const int FORMULA_TYPE_SHARED = 1;
        public const int FORMULA_TYPE_ARRAY = 2;
        public const int FORMULA_TYPE_CONDFOMRAT = 3;
        public const int FORMULA_TYPE_NAMEDRANGE = 4;

        private String formulaString;
        private int formulaLength;
        private int pointer;

        private ParseNode _rootNode;

        /**
         * Used for spotting if we have a cell reference,
         *  or a named range
         */
        private static string CELL_REFERENCE_PATTERN = "(?:('?)[^:\\\\/\\?\\*\\[\\]]+\\1!)?\\$?[A-Za-z]+\\$?[\\d]+";

        private static char TAB = '\t';

        /**
         * Lookahead Char.
         * Gets value '\0' when the input string Is exhausted
         */
        private char look;

        private HSSFWorkbook book;


        /**
         * Create the formula Parser, with the string that Is To be
         *  Parsed against the supplied workbook.
         * A later call the Parse() method To return ptg list in
         *  rpn order, then call the GetRPNPtg() To retrive the
         *  Parse results.
         * This class Is recommended only for single thReaded use.
         *
         * If you only have a usermodel.HSSFWorkbook, and not a
         *  model.Workbook, then use the convenience method on
         *  usermodel.HSSFFormulaEvaluator
         */
        public FormulaParser(String formula, HSSFWorkbook book)
        {
            formulaString = formula;
            pointer = 0;
            this.book = book;
            formulaLength = formulaString.Length;
        }

        public static Ptg[] Parse(String formula, HSSFWorkbook book)
        {
            FormulaParser fp = new FormulaParser(formula, book);
            fp.Parse();
            return fp.GetRPNPtg();
        }

        /** Read New Char From Input Stream */
        private void GetChar()
        {
            // Check To see if we've walked off the end of the string.
            if (pointer > formulaLength)
            {
                throw new Exception("too far");
            }
            if (pointer < formulaLength)
            {
                look = formulaString[pointer];
            }
            else
            {
                // Just return if so and Reset 'look' To something To keep
                // SkipWhitespace from spinning
                look = (char)0;
            }
            pointer++;
            //Console.WriteLine("Got char: "+ look);
        }

        /** Report What Was Expected */
        private Exception expected(String s)
        {
            String msg = "Parse error near char " + (pointer - 1) + " '" + look + "'"
                + " in specified formula '" + formulaString + "'. Expected "
                + s;
            return new FormulaParseException(msg);
        }

        /** Recognize an Alpha Char */
        private bool IsAlpha(char c)
        {
            return Char.IsLetter(c) || c == '$' || c == '_';
        }

        /** Recognize a Decimal Digit */
        private bool IsDigit(char c)
        {
            return Char.IsDigit(c);
        }

        /** Recognize an Alphanumeric */
        private bool IsAlNum(char c)
        {
            return (IsAlpha(c) || IsDigit(c));
        }

        /** Recognize White Space */
        private bool IsWhite(char c)
        {
            return (c == ' ' || c == TAB);
        }

        /** Skip Over Leading White Space */
        private void SkipWhite()
        {
            while (IsWhite(look))
            {
                GetChar();
            }
        }

        /**
         *  Consumes the next input Char if it Is equal To the one specified otherwise throws an
         *  UnChecked exception. This method does <b>not</b> consume whitespace (before or after the
         *  matched Char).
         */
        private void Match(char x)
        {
            if (look != x)
            {
                throw expected("'" + x + "'");
            }
            GetChar();
        }

        /** Get an Identifier */
        private String Name
        {
            get
            {
                StringBuilder Token = new StringBuilder();
                if (!IsAlpha(look) && look != '\'')
                {
                    throw expected("Name");
                }
                if (look == '\'')
                {
                    Match('\'');
                    bool done = look == '\'';
                    while (!done)
                    {
                        Token.Append(look);
                        GetChar();
                        if (look == '\'')
                        {
                            Match('\'');
                            done = look != '\'';
                        }
                    }
                }
                else
                {
                    while (IsAlNum(look))
                    {
                        Token.Append(look);
                        GetChar();
                    }
                }
                return Token.ToString();
            }
        }

        /** Get a Number */
        private String GetNum()
        {
            StringBuilder value = new StringBuilder();

            while (IsDigit(this.look))
            {
                value.Append(this.look);
                GetChar();
            }
            return value.Length == 0 ? null : value.ToString();
        }

        private ParseNode ParseFunctionOrIdentifier()
        {
            String name = Name;
            if (look == '(')
            {
                //This Is a function
                return function(name);
            }
            return new ParseNode(ParseIdentifier(name));
        }
        private Ptg ParseIdentifier(String name)
        {

            if (look == ':' || look == '.')
            { // this Is a AreaReference
                GetChar();

                while (look == '.')
                { // formulas can have . or .. or ... instead of :
                    GetChar();
                }

                String first = name;
                String second = Name;
                return new AreaPtg(first + ":" + second);
            }

            if (look == '!')
            {
                Match('!');
                String sheetName = name;
                String first = Name;
                short externIdx = book.GetExternalSheetIndex(book.GetSheetIndex(sheetName));
                if (look == ':')
                {
                    Match(':');
                    String second = Name;
                    if (look == '!')
                    {
                        //The sheet name was included in both of the areas. Only really
                        //need it once
                        Match('!');
                        String third = Name;

                        if (!sheetName.Equals(second))
                            throw new Exception("Unhandled double sheet reference.");

                        return new Area3DPtg(first + ":" + third, externIdx);
                    }
                    return new Area3DPtg(first + ":" + second, externIdx);
                }
                return new Ref3DPtg(first, externIdx);
            }
            if (name.Equals("TRUE",StringComparison.InvariantCultureIgnoreCase)
                || name.Equals("FALSE", StringComparison.InvariantCultureIgnoreCase))
            {
                return new BoolPtg(name.ToUpper());
            }

            // This can be either a cell ref or a named range
            // Try To spot which it Is
            Regex regex=new Regex(CELL_REFERENCE_PATTERN);
            bool cellRef = regex.IsMatch(name);

            if (cellRef)
            {
                return new RefPtg(name);
            }

            for (int i = 0; i < book.NumberOfNames; i++)
            {
                // named range name matching Is case insensitive
                if (book.GetNameAt(i).NameName.ToLower().Equals(name.ToLower()))
                {
                    return new NamePtg(name, book);
                }
            }
            throw new FormulaParseException("Found reference To named range \""
                        + name + "\", but that named range wasn't defined!");
        }

        /**
         * Note - Excel function names are 'case aware but not case sensitive'.  This method may end
         * up creating a defined name record in the workbook if the specified name Is not an internal
         * Excel function, and has not been encountered before.
         *
         * @param name case preserved function name (as it was entered/appeared in the formula).
         */
        private ParseNode function(String name)
        {
            NamePtg nameToken = null;
            // Note regarding parameter -
            if (!AbstractFunctionPtg.IsInternalFunctionName(name))
            {
                // external functions Get a Name Token which points To a defined name record
                nameToken = new NamePtg(name, this.book);

                // in the Token tree, the name Is more or less the first argument
            }

            Match('(');
            ParseNode[] args = Arguments();
            Match(')');

            return GetFunction(name, nameToken, args);
        }

        /**
         * Generates the variable function ptg for the formula.
         * <p>
         * For IF Formulas, Additional PTGs are Added To the Tokens
         * @param name
         * @param numArgs
         * @return Ptg a null Is returned if we're in an IF formula, it needs extreme manipulation and Is handled in this function
         */
        private ParseNode GetFunction(String name, NamePtg namePtg, ParseNode[] args)
        {

            FunctionMetadata fm = FunctionMetadataRegistry.GetFunctionByName(name.ToUpper());
            int numArgs = args.Length;
            if (fm == null)
            {
                if (namePtg == null)
                {
                    throw new InvalidOperationException("NamePtg must be supplied for external functions");
                }
                // must be external function
                ParseNode[] allArgs = new ParseNode[numArgs + 1];
                allArgs[0] = new ParseNode(namePtg);
                Array.Copy(args, 0, allArgs, 1, numArgs);
                return new ParseNode(new FuncVarPtg(name, (byte)(numArgs + 1)), allArgs);
            }

            if (namePtg != null)
            {
                throw new InvalidOperationException("NamePtg no applicable To internal functions");
            }
            bool IsVarArgs = !fm.HasFixedArgsLength;
            int funcIx = fm.Index;
            ValidateNumArgs(args.Length, fm);

            AbstractFunctionPtg retval;
            if (IsVarArgs)
            {
                retval = new FuncVarPtg(name, (byte)numArgs);
            }
            else
            {
                retval = new FuncPtg(funcIx);
            }
            return new ParseNode(retval, args);
        }

        private void ValidateNumArgs(int numArgs, FunctionMetadata fm)
        {
            if (numArgs < fm.MinParams)
            {
                String msg = "Too few arguments To function '" + fm.Name + "'. ";
                if (fm.HasFixedArgsLength)
                {
                    msg += "Expected " + fm.MinParams;
                }
                else
                {
                    msg += "At least " + fm.MinParams + " were expected";
                }
                msg += " but got " + numArgs + ".";
                throw new FormulaParseException(msg);
            }
            if (numArgs > fm.MaxParams)
            {
                String msg = "Too many arguments To function '" + fm.Name + "'. ";
                if (fm.HasFixedArgsLength)
                {
                    msg += "Expected " + fm.MaxParams;
                }
                else
                {
                    msg += "At most " + fm.MaxParams + " were expected";
                }
                msg += " but got " + numArgs + ".";
                throw new FormulaParseException(msg);
            }
        }

        private static bool IsArgumentDelimiter(char ch)
        {
            return ch == ',' || ch == ')';
        }

        /** Get arguments To a function */
        private ParseNode[] Arguments()
        {
            //average 2 args per function
            ArrayList temp = new ArrayList(2);
            SkipWhite();
            if (look == ')')
            {
                return ParseNode.EMPTY_ARRAY;
            }

            bool missedPrevArg = true;
            int numArgs = 0;
            while (true)
            {
                SkipWhite();
                if (IsArgumentDelimiter(look))
                {
                    if (missedPrevArg)
                    {
                        temp.Add(new ParseNode(MissingArgPtg.instance));
                        numArgs++;
                    }
                    if (look == ')')
                    {
                        break;
                    }
                    Match(',');
                    missedPrevArg = true;
                    continue;
                }
                temp.Add(comparisonExpression());
                numArgs++;
                missedPrevArg = false;
                SkipWhite();
                if (!IsArgumentDelimiter(look))
                {
                    throw expected("',' or ')'");
                }
            }
            ParseNode[] result = (ParseNode[])temp.ToArray();
            return result;
        }

        /** Parse and Translate a Math Factor  */
        private ParseNode powerFactor()
        {
            ParseNode result = percentFactor();
            while (true)
            {
                SkipWhite();
                if (look != '^')
                {
                    return result;
                }
                Match('^');
                ParseNode other = percentFactor();
                result = new ParseNode(PowerPtg.instance, result, other);
            }
        }

        private ParseNode percentFactor()
        {
            ParseNode result = ParseSimpleFactor();
            while (true)
            {
                SkipWhite();
                if (look != '%')
                {
                    return result;
                }
                Match('%');
                result = new ParseNode(PercentPtg.instance, result);
            }
        }


        /**
         * factors (without ^ or % )
         */
        private ParseNode ParseSimpleFactor()
        {
            SkipWhite();
            switch (look)
            {
                case '#':
                    return new ParseNode(ParseErrorLiteral());
                case '-':
                    Match('-');
                    return new ParseNode(UnaryMinusPtg.instance, powerFactor());
                case '+':
                    Match('+');
                    return new ParseNode(UnaryPlusPtg.instance, powerFactor());
                case '(':
                    Match('(');
                    ParseNode inside = comparisonExpression();
                    Match(')');
                    return new ParseNode(ParenthesisPtg.instance, inside);
                case '"':
                    return new ParseNode(ParseStringLiteral());
            }
            if (IsAlpha(look) || look == '\'')
            {
                return ParseFunctionOrIdentifier();
            }
            // else - assume number
            return new ParseNode(ParseNumber());
        }


        private Ptg ParseNumber()
        {
            String number2 = null;
            String exponent = null;
            String number1 = GetNum();

            if (look == '.')
            {
                GetChar();
                number2 = GetNum();
            }

            if (look == 'E')
            {
                GetChar();

                String sign = "";
                if (look == '+')
                {
                    GetChar();
                }
                else if (look == '-')
                {
                    GetChar();
                    sign = "-";
                }

                String number = GetNum();
                if (number == null)
                {
                    throw expected("Integer");
                }
                exponent = sign + number;
            }

            if (number1 == null && number2 == null)
            {
                throw expected("Integer");
            }

            return GetNumberPtgFromString(number1, number2, exponent);
        }


        private ErrPtg ParseErrorLiteral()
        {
            Match('#');
            String part1 = Name.ToUpper();

            switch (part1[0])
            {
                case 'V':
                    if (part1.Equals("VALUE"))
                    {
                        Match('!');
                        return ErrPtg.VALUE_INVALID;
                    }
                    throw expected("#VALUE!");
                case 'R':
                    if (part1.Equals("REF"))
                    {
                        Match('!');
                        return ErrPtg.REF_INVALID;
                    }
                    throw expected("#REF!");
                case 'D':
                    if (part1.Equals("DIV"))
                    {
                        Match('/');
                        Match('0');
                        Match('!');
                        return ErrPtg.DIV_ZERO;
                    }
                    throw expected("#DIV/0!");
                case 'N':
                    if (part1.Equals("NAME"))
                    {
                        Match('?');  // only one that ends in '?'
                        return ErrPtg.NAME_INVALID;
                    }
                    if (part1.Equals("NUM"))
                    {
                        Match('!');
                        return ErrPtg.NUM_ERROR;
                    }
                    if (part1.Equals("NULL"))
                    {
                        Match('!');
                        return ErrPtg.NULL_INTERSECTION;
                    }
                    if (part1.Equals("N"))
                    {
                        Match('/');
                        if (look != 'A' && look != 'a')
                        {
                            throw expected("#N/A");
                        }
                        Match(look);
                        // Note - no '!' or '?' suffix
                        return ErrPtg.N_A;
                    }
                    throw expected("#NAME?, #NUM!, #NULL! or #N/A");

            }
            throw expected("#VALUE!, #REF!, #DIV/0!, #NAME?, #NUM!, #NULL! or #N/A");
        }


        /**
         * Get a PTG for an integer from its string representation.
         * return Int or Number Ptg based on size of input
         */
        private static Ptg GetNumberPtgFromString(String number1, String number2, String exponent)
        {
            StringBuilder number = new StringBuilder();

            if (number2 == null)
            {
                number.Append(number1);

                if (exponent != null)
                {
                    number.Append('E');
                    number.Append(exponent);
                }

                String numberStr = number.ToString();
                int intVal;
                try
                {
                    intVal = int.Parse(numberStr);
                }
                catch (Exception)
                {
                    return new NumberPtg(numberStr);
                }
                if (IntPtg.IsInRange(intVal))
                {
                    return new IntPtg(intVal);
                }
                return new NumberPtg(numberStr);
            }

            if (number1 != null)
            {
                number.Append(number1);
            }

            number.Append('.');
            number.Append(number2);

            if (exponent != null)
            {
                number.Append('E');
                number.Append(exponent);
            }

            return new NumberPtg(number.ToString());
        }


        private StringPtg ParseStringLiteral()
        {
            Match('"');

            StringBuilder token = new StringBuilder();
            while (true)
            {
                if (look == '"')
                {
                    GetChar();
                    if (look != '"')
                    {
                        break;
                    }
                }
                token.Append(look);
                GetChar();
            }
            return new StringPtg(token.ToString());
        }

        /** Parse and Translate a Math Term */
        private ParseNode Term()
        {
            ParseNode result = powerFactor();
            while (true)
            {
                SkipWhite();
                Ptg operator1;
                switch (look)
                {
                    case '*':
                        Match('*');
                        operator1 = MultiplyPtg.instance;
                        break;
                    case '/':
                        Match('/');
                        operator1 = DividePtg.instance;
                        break;
                    default:
                        return result; // finished with Term
                }
                ParseNode other = powerFactor();
                result = new ParseNode(operator1, result, other);
            }
        }

        private ParseNode comparisonExpression()
        {
            ParseNode result = concatExpression();
            while (true)
            {
                SkipWhite();
                switch (look)
                {
                    case '=':
                    case '>':
                    case '<':
                        Ptg comparisonToken = GetComparisonToken();
                        ParseNode other = concatExpression();
                        result = new ParseNode(comparisonToken, result, other);
                        continue;
                }
                return result; // finished with predicate expression
            }
        }

        private Ptg GetComparisonToken()
        {
            if (look == '=')
            {
                Match(look);
                return EqualPtg.instance;
            }
            bool isGreater = look == '>';
            Match(look);
            if (isGreater)
            {
                if (look == '=')
                {
                    Match('=');
                    return GreaterEqualPtg.instance;
                }
                return GreaterThanPtg.instance;
            }
            switch (look)
            {
                case '=':
                    Match('=');
                    return LessEqualPtg.instance;
                case '>':
                    Match('>');
                    return NotEqualPtg.instance;
            }
            return LessThanPtg.instance;
        }


        private ParseNode concatExpression()
        {
            ParseNode result = AdditiveExpression();
            while (true)
            {
                SkipWhite();
                if (look != '&')
                {
                    break; // finished with concat expression
                }
                Match('&');
                ParseNode other = AdditiveExpression();
                result = new ParseNode(ConcatPtg.instance, result, other);
            }
            return result;
        }


        /** Parse and Translate an Expression */
        private ParseNode AdditiveExpression()
        {
            ParseNode result = Term();
            while (true)
            {
                SkipWhite();
                Ptg operator1;
                switch (look)
                {
                    case '+':
                        Match('+');
                        operator1 = AddPtg.instance;
                        break;
                    case '-':
                        Match('-');
                        operator1 = SubtractPtg.instance;
                        break;
                    default:
                        return result; // finished with Additive expression
                }
                ParseNode other = Term();
                result = new ParseNode(operator1, result, other);
            }
        }

        //{--------------------------------------------------------------}
        //{ Parse and Translate an Assignment Statement }
        /**
    procedure Assignment;
    var Name: string[8];
    begin
       Name := GetName;
       Match('=');
       Expression;

    end;
         **/


        /**
         *  API call To execute the parsing of the formula
         * @deprecated use Ptg[] FormulaParser.Parse(String, HSSFWorkbook) directly
         */
        public void Parse()
        {
            pointer = 0;
            GetChar();
            _rootNode = comparisonExpression();

            if (pointer <= formulaLength)
            {
                String msg = "Unused input [" + formulaString.Substring(pointer - 1)
                    + "] after attempting To Parse the formula [" + formulaString + "]";
                throw new FormulaParseException(msg);
            }
        }


        /*********************************
         * ParseR IMPLEMENTATION ENDS HERE
         * EXCEL SPECIFIC METHODS BELOW
         *******************************/

        /** API call To retrive the array of Ptgs Created as
         * a result of the parsing
         */
        public Ptg[] GetRPNPtg()
        {
            return GetRPNPtg(FORMULA_TYPE_CELL);
        }

        public Ptg[] GetRPNPtg(int formulaType)
        {
            OperandClassTransformer oct = new OperandClassTransformer(formulaType);
            // RVA Is for 'operand class': 'reference', 'value', 'array'
            oct.TransformFormula(_rootNode);
            return ParseNode.toTokenArray(_rootNode);
        }

        /**
         * Convenience method which takes in a list then passes it To the
         *  other ToFormulaString signature.
         * @param book   workbook for 3D and named references
         * @param lptgs  list of Ptg, can be null or empty
         * @return a human Readable String
         */
        public static String ToFormulaString(HSSFWorkbook book, Stack lptgs)
        {
            String retval = null;
            if (lptgs == null || lptgs.Count == 0) return "#NAME";
            Ptg[] ptgs = new Ptg[lptgs.Count];
            ptgs = (Ptg[])lptgs.ToArray();
            retval = ToFormulaString(book, ptgs);
            return retval;
        }
        /**
         * Convenience method which takes in a list then passes it To the
         *  other ToFormulaString signature. Works on the current
         *  workbook for 3D and named references
         * @param lptgs  list of Ptg, can be null or empty
         * @return a human Readable String
         */
        public String ToFormulaString(Stack lptgs)
        {
            return ToFormulaString(book, lptgs);
        }

        /**
         * Static method To Convert an array of Ptgs in RPN order
         * To a human Readable string format in infix mode.
         * @param book  workbook for named and 3D references
         * @param ptgs  array of Ptg, can be null or empty
         * @return a human Readable String
         */
        public static String ToFormulaString(HSSFWorkbook book, Ptg[] ptgs)
        {
            if (ptgs == null || ptgs.Length == 0)
            {
                // TODO - what Is the justification for returning "#NAME" (which Is not "#NAME?", btw)
                return "#NAME";
            }
            Stack stack = new Stack();

            for (int i = 0; i < ptgs.Length; i++)
            {
                Ptg ptg = ptgs[i];
                // TODO - what about MemNoMemPtg?
                if (ptg is MemAreaPtg || ptg is MemFuncPtg || ptg is MemErrPtg)
                {
                    // marks the start of a list of area expressions which will be naturally combined
                    // by their trailing operators (e.g. UnionPtg)
                    // TODO - put comment and throw exception in ToFormulaString() of these classes
                    continue;
                }
                if (ptg is ParenthesisPtg)
                {
                    String contents = (String)stack.Pop();
                    stack.Push("(" + contents + ")");
                    continue;
                }
                if (ptg is AttrPtg)
                {
                    AttrPtg attrPtg = ((AttrPtg)ptg);
                    if (attrPtg.IsOptimizedIf|| attrPtg.IsOptimizedChoose || attrPtg.IsGoto)
                    {
                        continue;
                    }
                    if (attrPtg.IsSpace)
                    {
                        // POI currently doesn't render spaces in formulas
                        continue;
                        // but if it ever did, care must be taken:
                        // tAttrSpace comes *before* the operand it applies To, which may be consistent
                        // with how the formula text appears but Is against the RPN ordering assumed here
                    }
                    if (attrPtg.IsSemiVolatile)
                    {
                        // similar To tAttrSpace - RPN Is violated
                        continue;
                    }
                    if (attrPtg.IsSum)
                    {
                        String[] operands = GetOperands(stack, attrPtg.NumberOfOperands);
                        stack.Push(attrPtg.ToFormulaString(operands));
                        continue;
                    }
                    throw new Exception("Unexpected tAttr: " + attrPtg.ToString());
                }

                if (!(ptg is OperationPtg))
                {
                    stack.Push(ptg.ToFormulaString(book));
                    continue;
                }

                OperationPtg o = (OperationPtg)ptg;
                String[] operands1 = GetOperands(stack, o.NumberOfOperands);
                stack.Push(o.ToFormulaString(operands1));
            }
            if (stack.Count==0)
            {
                // inspection of the code above reveals that every stack.Pop() Is followed by a
                // stack.Push(). So this Is either an internal error or impossible.
                throw new InvalidOperationException("Stack Underflow");
            }
            String result = (String)stack.Pop();
            if (stack.Count!=0)
            {
                // Might be caused by some Tokens like AttrPtg and Mem*Ptg, which really shouldn't
                // put anything on the stack
                throw new InvalidOperationException("too much stuff left on the stack");
            }
            return result;
        }

        private static String[] GetOperands(Stack stack, int nOperands)
        {
            String[] operands = new String[nOperands];

            for (int j = nOperands - 1; j >= 0; j--)
            { // reverse iteration because args were pushed in-order
                if (stack.Count==0)
                {
                    String msg = "Too few arguments supplied To operation. Expected (" + nOperands
                         + ") operands but got (" + (nOperands - j - 1) + ")";
                    throw new InvalidOperationException(msg);
                }
                operands[j] = (String)stack.Pop();
            }
            return operands;
        }
        /**
         * Static method To Convert an array of Ptgs in RPN order
         *  To a human Readable string format in infix mode. Works
         *  on the current workbook for named and 3D references.
         * @param ptgs  array of Ptg, can be null or empty
         * @return a human Readable String
         */
        public String ToFormulaString(Ptg[] ptgs)
        {
            return ToFormulaString(book, ptgs);
        }
    }
}