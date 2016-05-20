/* ====================================================================
   Licensed To the Apache Software Foundation (ASF) under one or more
   contributor license agreements.  See the NOTICE file distributed with
   this work for Additional information regarding copyright ownership.
   The ASF licenses this file To You under the Apache License, Version 2.0
   (the "License"); you may not use this file except in compliance with
   the License.  You may obtain a copy of the License at

       http://www.apache.org/licenses/LICENSE-2.0

   Unless required by applicable law or agreed To in writing, software
   distributed under the License is distributed on an "AS IS" BASIS,
   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
   See the License for the specific language governing permissions and
   limitations under the License.
==================================================================== */

namespace NPOI.SS.Formula
{

    using System;
    using System.Text;
    using System.Collections;
    using System.Text.RegularExpressions;

    using NPOI.HSSF.Record;
    using NPOI.HSSF.Record.Formula;
    using NPOI.HSSF.Record.Formula.Function;
    
    using NPOI.SS.Util;
    using NPOI.HSSF.Record.Constant;
    using NPOI.HSSF.UserModel;

    /**
     * This class Parses a formula string into a List of Tokens in RPN order.
     * Inspired by
     *           Lets Build a Compiler, by Jack Crenshaw
     * BNF for the formula expression is :
     * <expression> ::= <term> [<addop> <term>]*
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
     *  @author Josh Micich
     */
    public class FormulaParser
    {
        private class Identifier
        {
            private String _name;
            private bool _isQuoted;

            public Identifier(String name, bool IsQuoted)
            {
                _name = name;
                _isQuoted = IsQuoted;
            }
            public String Name
            {
                get
                {
                    return _name;
                }
            }
            public bool IsQuoted
            {
                get
                {
                    return _isQuoted;
                }
            }
            public override String ToString()
            {
                StringBuilder sb = new StringBuilder(64);
                sb.Append(GetType().Name);
                sb.Append(" [");
                if (_isQuoted)
                {
                    sb.Append("'").Append(_name).Append("'");
                }
                else
                {
                    sb.Append(_name);
                }
                sb.Append("]");
                return sb.ToString();
            }
        }

        /// <summary>
        /// Specific exception thrown when a supplied formula does not Parse properly.
        ///  Primarily used by test cases when testing for specific parsing exceptions.
        /// </summary>
        public class FormulaParseException : Exception
        {
            /// <summary>
            ///This class was given package scope until it would become Clear that it is useful to general client code.
            /// </summary>
            /// <param name="msg"></param>
            public FormulaParseException(String msg)
                : base(msg)
            {

            }
        }


        private String formulaString;
        private int formulaLength;
        private int pointer;
        private static SpreadsheetVersion _ssVersion;

        private ParseNode _rootNode;

        private static char TAB = '\t';

        /**
         * Lookahead Character.
         * Gets value '\0' when the input string is exhausted
         */
        private char look;

        private FormulaParsingWorkbook book;

        private int _sheetIndex;

        /**
         * Create the formula Parser, with the string that is To be
         *  Parsed against the supplied workbook.
         * A later call the Parse() method To return ptg list in
         *  rpn order, then call the GetRPNPtg() To retrive the
         *  Parse results.
         * This class is recommended only for single threaded use.
         *
         * If you only have a usermodel.HSSFWorkbook, and not a
         *  model.Workbook, then use the convenience method on
         *  usermodel.HSSFFormulaEvaluator
         */
        public FormulaParser(String formula, FormulaParsingWorkbook book, int sheetIndex)
        {
            formulaString = formula;
            pointer = 0;
            this.book = book;

            _ssVersion = book == null ? SpreadsheetVersion.EXCEL97 : book.GetSpreadsheetVersion();
            formulaLength = formulaString.Length;
            _sheetIndex = sheetIndex;
        }

        public static Ptg[] Parse(String formula, FormulaParsingWorkbook book)
        {
            return Parse(formula, book, FormulaType.CELL);
        }


        /**
         * Parse a formula into a array of tokens
         *
         * @param formula	 the formula to parse
         * @param workbook	the parent workbook
         * @param formulaType the type of the formula, see {@link FormulaType}
         * @param sheetIndex  the 0-based index of the sheet this formula belongs to.
         * The sheet index is required to resolve sheet-level names. <code>-1</code> means that
         * the scope of the name will be ignored and  the parser will match names only by name
         *
         * @return array of parsed tokens
         * @throws FormulaParseException if the formula is unparsable
         */
        public static Ptg[] Parse(String formula, FormulaParsingWorkbook workbook, FormulaType formulaType, int sheetIndex)
        {
            FormulaParser fp = new FormulaParser(formula, workbook, sheetIndex);
            fp.Parse();
            return fp.GetRPNPtg(formulaType);
        }

        public static Ptg[] Parse(String formula, FormulaParsingWorkbook workbook, FormulaType formulaType)
        {
            return Parse(formula, workbook, formulaType, -1);
        }

        /** Read New Character From Input Stream */
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
                // Just return if so and reset 'look' To something To keep
                // SkipWhitespace from spinning
                look = (char)0;
            }
            pointer++;
            //Console.WriteLine("Got char: "+ look);
        }

        /** Report What Was Expected */
        private Exception expected(String s)
        {
            String msg;

            if (look == '=' && formulaString.Substring(0, pointer - 1).Trim().Length < 1)
            {
                msg = "The specified formula '" + formulaString
                    + "' starts with an Equals sign which is not allowed.";
            }
            else
            {
                msg = "Parse error near char " + (pointer - 1) + " '" + look + "'"
                    + " in specified formula '" + formulaString + "'. Expected "
                    + s;
            }
            return new FormulaParseException(msg);
        }

        /** Recognize an Alpha Character */
        private static bool IsAlpha(char c)
        {
            return Char.IsLetter(c) || c == '$' || c == '_';
        }

        /** Recognize a Decimal Digit */
        private static bool IsDigit(char c)
        {
            return Char.IsDigit(c);
        }

        /** Recognize an Alphanumeric */
        private static bool IsAlNum(char c)
        {
            return IsAlpha(c) || IsDigit(c);
        }

        /** Recognize White Space */
        private static bool IsWhite(char c)
        {
            return c == ' ' || c == TAB;
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
         *  Consumes the next input character if it is equal To the one specified otherwise throws an
         *  unchecked exception. This method does <b>not</b> consume whitespace (before or after the
         *  matched character).
         */
        private void Match(char x)
        {
            if (look != x)
            {
                throw expected("'" + x + "'");
            }
            GetChar();
        }
        private String ParseUnquotedIdentifier()
        {
            Identifier iden = ParseIdentifier();
            if (iden.IsQuoted)
            {
                throw expected("unquoted identifier");
            }
            return iden.Name;
        }
        /**
         * Parses a sheet name, named range name, or simple cell reference.<br/>
         * Note - identifiers in Excel can contain dots, so this method may return a String
         * which may need To be converted To an area reference.  For example, this method
         * may return a value like "A1..B2", in which case the caller must convert it To
         * an area reference like "A1:B2"
         */
        private Identifier ParseIdentifier()
        {
            StringBuilder sb = new StringBuilder();
            if (!IsAlpha(look) && look != '\'' && look != '[')
            {
                throw expected("Name");
            }
            bool IsQuoted = look == '\'';
            if (IsQuoted)
            {
                Match('\'');
                bool done = look == '\'';
                while (!done)
                {
                    sb.Append(look);
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
                // allow for any sequence of dots and identifier chars
                // special case of two consecutive dots is best treated in the calling code
                while (IsAlNum(look) || look == '.' || look == '[' || look == ']')
                {
                    sb.Append(look);
                    GetChar();
                }
            }
            return new Identifier(sb.ToString(), IsQuoted);
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
        private class SheetIdentifier
        {


            private String _bookName;
            private Identifier _sheetIdentifier;
            public SheetIdentifier(String bookName, Identifier sheetIdentifier)
            {
                _bookName = bookName;
                _sheetIdentifier = sheetIdentifier;
            }
            public String BookName
            {
                get
                {
                    return _bookName;
                }
            }
            public Identifier SheetID
            {
                get
                {
                    return _sheetIdentifier;
                }
            }
            public override String ToString()
            {
                StringBuilder sb = new StringBuilder(64);
                sb.Append(this.GetType().Name);
                sb.Append(" [");
                if (_bookName != null)
                {
                    sb.Append(" [").Append(_sheetIdentifier.Name).Append("]");
                }
                if (_sheetIdentifier.IsQuoted)
                {
                    sb.Append("'").Append(_sheetIdentifier.Name).Append("'");
                }
                else
                {
                    sb.Append(_sheetIdentifier.Name);
                }
                sb.Append("]");
                return sb.ToString();
            }
        }

        	/**
	 * A1, $A1, A$1, $A$1, A, 1
	 */
        private class SimpleRangePart
        {
            public enum PartType
            {
                CELL, ROW, COLUMN
            }

            public static PartType Get(bool hasLetters, bool hasDigits)
            {
                if (hasLetters)
                {
                    return hasDigits ? PartType.CELL : PartType.COLUMN;
                }
                if (!hasDigits)
                {
                    throw new ArgumentException("must have either letters or numbers");
                }
                return PartType.ROW;
            }

            private PartType _type;
            private String _rep;

            public SimpleRangePart(String rep, bool hasLetters, bool hasNumbers)
            {
                _rep = rep;
                _type = Get(hasLetters, hasNumbers);
            }

            public bool IsCell
            {
                get
                {
                    return _type == PartType.CELL;
                }
            }

            public bool IsRowOrColumn
            {
                get
                {
                    return _type != PartType.CELL;
                }
            }

            public CellReference getCellReference()
            {
                if (_type != PartType.CELL)
                {
                    throw new InvalidOperationException("Not applicable to this type");
                }
                return new CellReference(_rep);
            }

            public bool IsColumn
            {
                get
                {
                    return _type == PartType.COLUMN;
                }
            }

            public bool IsRow
            {
                get
                {
                    return _type == PartType.ROW;
                }
            }

            public String Rep
            {
                get
                {
                    return _rep;
                }
            }

            /**
             * @return <code>true</code> if the two range parts can be combined in an
             * {@link AreaPtg} ( Note - the explicit range operator (:) may still be valid
             * when this method returns <code>false</code> )
             */
            public bool IsCompatibleForArea(SimpleRangePart part2)
            {
                return _type == part2._type;
            }

            public override String ToString()
            {
                StringBuilder sb = new StringBuilder(64);
                sb.Append(this.GetType().Name).Append(" [");
                sb.Append(_rep);
                sb.Append("]");
                return sb.ToString();
            }
        }
        /**
 * Note - caller should reset {@link #_pointer} upon <code>null</code> result
 * @return The sheet name as an identifier <code>null</code> if '!' is not found in the right place
 */
        private SheetIdentifier ParseSheetName()
        {

            String bookName;
            if (look == '[')
            {
                StringBuilder sb = new StringBuilder();
                GetChar();
                while (look != ']')
                {
                    sb.Append(look);
                    GetChar();
                }
                GetChar();
                bookName = sb.ToString();
            }
            else
            {
                bookName = null;
            }

            if (look == '\'')
            {
                StringBuilder sb = new StringBuilder();

                Match('\'');
                bool done = look == '\'';
                while (!done)
                {
                    sb.Append(look);
                    GetChar();
                    if (look == '\'')
                    {
                        Match('\'');
                        done = look != '\'';
                    }
                }

                Identifier iden = new Identifier(sb.ToString(), true);
                // quoted identifier - can't concatenate anything more
                SkipWhite();
                if (look == '!')
                {
                    GetChar();
                    return new SheetIdentifier(bookName, iden);
                }
                return null;
            }

            // unquoted sheet names must start with underscore or a letter
            if (look == '_' || Char.IsLetter(look))
            {
                StringBuilder sb = new StringBuilder();
                // can concatenate idens with dots
                while (IsUnquotedSheetNameChar(look))
                {
                    sb.Append(look);
                    GetChar();
                }
                SkipWhite();
                if (look == '!')
                {
                    GetChar();
                    return new SheetIdentifier(bookName, new Identifier(sb.ToString(), false));
                }
                return null;
            }
            return null;
        }

        /**
 * very similar to {@link SheetNameFormatter#isSpecialChar(char)}
 */
        private bool IsUnquotedSheetNameChar(char ch)
        {
            if (Char.IsLetterOrDigit(ch))
            {
                return true;
            }
            switch (ch)
            {
                case '.': // dot is OK
                case '_': // underscore is OK
                    return true;
            }
            return false;
        }
        private void ResetPointer(int ptr)
        {
            pointer = ptr;
            if (pointer <= formulaLength)
            {
                look = formulaString[pointer - 1];
            }
            else
            {
                // Just return if so and reset 'look' to something to keep
                // SkipWhitespace from spinning
                look = (char)0;
            }
        }
        /**
 *
 * @return <code>true</code> if the specified character may be used in a defined name
 */
        private static bool IsValidDefinedNameChar(char ch)
        {
            if (Char.IsLetterOrDigit(ch))
            {
                return true;
            }
            switch (ch)
            {
                case '.':
                case '_':
                case '?':
                case '\\': // of all things
                    return true;
            }
            return false;
        }

        /**
 * Parses simple factors that are not primitive ranges or range components
 * i.e. '!', ':'(and equiv '...') do not appear
 * Examples
 * <pre>
 *   my.named...range.
 *   foo.bar(123.456, "abc")
 *   123.456
 *   "abc"
 *   true
 * </pre>
 */
        private ParseNode ParseNonRange(int savePointer)
        {
            ResetPointer(savePointer);

            if (Char.IsDigit(look))
            {
                return new ParseNode(ParseNumber());
            }
            if (look == '"')
            {
                return new ParseNode(new StringPtg(ParseStringLiteral()));
            }
            // from now on we can only be dealing with non-quoted identifiers
            // which will either be named ranges or functions
            StringBuilder sb = new StringBuilder();

            if (!Char.IsLetter(look))
            {
                throw expected("number, string, or defined name");
            }
            while (IsValidDefinedNameChar(look))
            {
                sb.Append(look);
                GetChar();
            }
            SkipWhite();
            String name = sb.ToString();
            if (look == '(')
            {
                return Function(name);
            }
            if (name.Equals("TRUE",StringComparison.InvariantCultureIgnoreCase) || name.Equals("FALSE",StringComparison.InvariantCultureIgnoreCase))
            {
                return new ParseNode(new BoolPtg(name.ToUpper()));
            }
            if (book == null)
            {
                // Only test cases omit the book (expecting it not to be needed)
                throw new InvalidOperationException("Need book to evaluate name '" + name + "'");
            }
            EvaluationName evalName = book.GetName(name, _sheetIndex);
            if (evalName == null)
            {
                throw new FormulaParseException("Specified named range '"
                        + name + "' does not exist in the current workbook.");
            }
            if (evalName.IsRange)
            {
                return new ParseNode(evalName.CreatePtg());
            }
            // TODO - what about NameX ?
            throw new FormulaParseException("Specified name '"
                    + name + "' is not a range as expected.");
        }

        /**
 * Parses area refs (things which could be the operand of ':') and simple factors
 * Examples
 * <pre>
 *   A$1
 *   $A$1 :  $B1
 *   A1 .......	C2
 *   Sheet1 !$A1
 *   a..b!A1
 *   'my sheet'!A1
 *   .my.sheet!A1
 *   my.named..range.
 *   foo.bar(123.456, "abc")
 *   123.456
 *   "abc"
 *   true
 * </pre>
 *
 */
        private ParseNode ParseRangeable()
        {
            SkipWhite();
            int savePointer = pointer;
            SheetIdentifier sheetIden = ParseSheetName();
            if (sheetIden == null)
            {
                ResetPointer(savePointer);
            }
            else
            {
                SkipWhite();
                savePointer = pointer;
            }

            SimpleRangePart part1 = ParseSimpleRangePart();
            if (part1 == null)
            {
                if (sheetIden != null)
                {
                    throw new FormulaParseException("Cell reference expected after sheet name at index "
                            + pointer + ".");
                }
                return ParseNonRange(savePointer);
            }
            bool whiteAfterPart1 = IsWhite(look);
            if (whiteAfterPart1)
            {
                SkipWhite();
            }

            if (look == ':')
            {
                int colonPos = pointer;
                GetChar();
                SkipWhite();
                SimpleRangePart part2 = ParseSimpleRangePart();
                if (part2 != null && !part1.IsCompatibleForArea(part2))
                {
                    // second part is not compatible with an area ref e.g. S!A1:S!B2
                    // where S might be a sheet name (that looks like a column name)

                    part2 = null;
                }
                if (part2 == null)
                {
                    // second part is not compatible with an area ref e.g. A1:OFFSET(B2, 1, 2)
                    // reset and let caller use explicit range operator
                    ResetPointer(colonPos);
                    if (!part1.IsCell)
                    {
                        String prefix;
                        if (sheetIden == null)
                        {
                            prefix = "";
                        }
                        else
                        {
                            prefix = "'" + sheetIden.SheetID.Name + '!';
                        }
                        throw new FormulaParseException(prefix + part1.Rep + "' is not a proper reference.");
                    }
                    return CreateAreaRefParseNode(sheetIden, part1, part2);
                }
                return CreateAreaRefParseNode(sheetIden, part1, part2);
            }

            if (look == '.')
            {
                GetChar();
                int dotCount = 1;
                while (look == '.')
                {
                    dotCount++;
                    GetChar();
                }
                bool whiteBeforePart2 = IsWhite(look);

                SkipWhite();
                SimpleRangePart part2 = ParseSimpleRangePart();
                String part1And2 = formulaString.Substring(savePointer - 1,pointer -savePointer);
                if (part2 == null)
                {
                    if (sheetIden != null)
                    {
                        throw new FormulaParseException("Complete area reference expected after sheet name at index "
                                + pointer + ".");
                    }
                    return ParseNonRange(savePointer);
                }


                if (whiteAfterPart1 || whiteBeforePart2)
                {
                    if (part1.IsRowOrColumn || part2.IsRowOrColumn)
                    {
                        // "A .. B" not valid syntax for "A:B"
                        // and there's no other valid expression that fits this grammar
                        throw new FormulaParseException("Dotted range (full row or column) expression '"
                                + part1And2 + "' must not contain whitespace.");
                    }
                    return CreateAreaRefParseNode(sheetIden, part1, part2);
                }

                if (dotCount == 1 && part1.IsRow && part2.IsRow)
                {
                    // actually, this is looking more like a number
                    return ParseNonRange(savePointer);
                }

                if (part1.IsRowOrColumn || part2.IsRowOrColumn)
                {
                    if (dotCount != 2)
                    {
                        throw new FormulaParseException("Dotted range (full row or column) expression '" + part1And2
                                + "' must have exactly 2 dots.");
                    }
                }
                return CreateAreaRefParseNode(sheetIden, part1, part2);
            }
            if (part1.IsCell && IsValidCellReference(part1.Rep))
            {
                return CreateAreaRefParseNode(sheetIden, part1, null);
            }
            if (sheetIden != null)
            {
                throw new FormulaParseException("Second part of cell reference expected after sheet name at index "
                        + pointer + ".");
            }

            return ParseNonRange(savePointer);
        }
        private static AreaReference CreateAreaRef(SimpleRangePart part1, SimpleRangePart part2)
        {
            if (!part1.IsCompatibleForArea(part2))
            {
                throw new FormulaParseException("has incompatible parts: '"
                        + part1.Rep + "' and '" + part2.Rep + "'.");
            }
            if (part1.IsRow)
            {
                return AreaReference.GetWholeRow(part1.Rep, part2.Rep);
            }
            if (part1.IsColumn)
            {
                return AreaReference.GetWholeColumn(part1.Rep, part2.Rep);
            }
            return new AreaReference(part1.getCellReference(), part2.getCellReference());
        }

        	/**
	 *
	 * @param sheetIden may be <code>null</code>
	 * @param part1
	 * @param part2 may be <code>null</code>
	 */
        private ParseNode CreateAreaRefParseNode(SheetIdentifier sheetIden, SimpleRangePart part1,
                SimpleRangePart part2)
        {

            int extIx;
            if (sheetIden == null)
            {
                extIx = Int32.MinValue;
            }
            else
            {
                String sName = sheetIden.SheetID.Name;
                if (sheetIden.BookName == null)
                {
                    extIx = book.GetExternalSheetIndex(sName);
                }
                else
                {
                    extIx = book.GetExternalSheetIndex(sheetIden.BookName, sName);
                }
            }
            Ptg ptg;
            if (part2 == null)
            {
                CellReference cr = part1.getCellReference();
                if (sheetIden == null)
                {
                    ptg = new RefPtg(cr);
                }
                else
                {
                    ptg = new Ref3DPtg(cr, extIx);
                }
            }
            else
            {
                AreaReference areaRef = CreateAreaRef(part1, part2);

                if (sheetIden == null)
                {
                    ptg = new AreaPtg(areaRef);
                }
                else
                {
                    ptg = new Area3DPtg(areaRef, extIx);
                }
            }
            return new ParseNode(ptg);
        }

        //private ParseNode ParseFunctionReferenceOrName()
        //{
        //    Identifier iden = ParseIdentifier();
        //    if (look == '(')
        //    {
        //        //This is a Function
        //        return Function(iden.Name);
        //    }
        //    if (!iden.IsQuoted)
        //    {
        //        String name = iden.Name;
        //        if (name.Equals("TRUE",StringComparison.InvariantCultureIgnoreCase)
        //            || name.Equals("FALSE", StringComparison.InvariantCultureIgnoreCase))
        //        {
        //            return new ParseNode(new BoolPtg(name.ToUpper()));
        //        }
        //    }
        //    return ParseRangeExpression(iden);
        //}

        private ParseNode ParseRangeExpression()
        {
            ParseNode result = ParseRangeable();
            bool hasRange = false;
            while (look == ':')
            {
                int pos = pointer;
                GetChar();
                ParseNode nextPart = ParseRangeable();
                // Note - no range simplification here. An expr like "A1:B2:C3:D4:E5" should be
                // grouped into area ref pairs like: "(A1:B2):(C3:D4):E5"
                // Furthermore, Excel doesn't seem to simplify
                // expressions like "Sheet1!A1:Sheet1:B2" into "Sheet1!A1:B2"

                CheckValidRangeOperand("LHS", pos, result);
                CheckValidRangeOperand("RHS", pos, nextPart);

                ParseNode[] children = { result, nextPart, };
                result = new ParseNode(RangePtg.instance, children);
                hasRange = true;
            }
            if (hasRange)
            {
                return AugmentWithMemPtg(result);
            }
            return result;
        }
        /**
 * From OOO doc: "Whenever one operand of the reference subexpression is a function,
 *  a defined name, a 3D reference, or an external reference (and no error occurs),
 *  a tMemFunc token is used"
 *
 */
        private static bool NeedsMemFunc(ParseNode root)
        {
            Ptg token = root.GetToken();
            if (token is AbstractFunctionPtg)
            {
                return true;
            }
            if (token is ExternSheetReferenceToken)
            { // 3D refs
                return true;
            }
            if (token is NamePtg || token is NameXPtg)
            { // 3D refs
                return true;
            }

            if (token is OperationPtg||token is ParenthesisPtg)
            {
                // expect RangePtg, but perhaps also UnionPtg, IntersectionPtg etc
                foreach (ParseNode child in root.GetChildren())
                {
                    if (NeedsMemFunc(child))
                    {
                        return true;
                    }
                }
                return false;
            }
            if (token is OperandPtg)
            {
                return false;
            }
            if (token is OperationPtg)
            {
                return true;
            }

            return false;
        }

        private static ParseNode AugmentWithMemPtg(ParseNode root)
        {
            Ptg memPtg;
            if (NeedsMemFunc(root))
            {
                memPtg = new MemFuncPtg(root.EncodedSize);
            }
            else
            {
                memPtg = new MemAreaPtg(root.EncodedSize);
            }
            return new ParseNode(memPtg, root);
        }

        /**
 * @param currentParsePosition used to format a potential error message
 */
        private void CheckValidRangeOperand(String sideName, int currentParsePosition, ParseNode pn)
        {
            if (!IsValidRangeOperand(pn))
            {
                throw new FormulaParseException("The " + sideName
                        + " of the range operator ':' at position "
                        + currentParsePosition + " is not a proper reference.");
            }
        }
        /**
         * @return false if sub-expression represented the specified ParseNode definitely
         * cannot appear on either side of the range (':') operator
         */
        private bool IsValidRangeOperand(ParseNode a)
        {
            Ptg tkn = a.GetToken();
            // Note - order is important for these instance-of checks
            if (tkn is OperandPtg)
            {
                // notably cell refs and area refs
                return true;
            }

            // next 2 are special cases of OperationPtg
            if (tkn is AbstractFunctionPtg)
            {
                AbstractFunctionPtg afp = (AbstractFunctionPtg)tkn;
                byte returnClass = afp.DefaultOperandClass;
                return Ptg.CLASS_REF == returnClass;
            }
            if (tkn is ValueOperatorPtg)
            {
                return false;
            }
            if (tkn is OperationPtg)
            {
                return true;
            }

            // one special case of ControlPtg
            if (tkn is ParenthesisPtg)
            {
                // parenthesis Ptg should have only one child
                return IsValidRangeOperand(a.GetChildren()[0]);
            }

            // one special case of ScalarConstantPtg
            if (tkn == ErrPtg.REF_INVALID)
            {
                return true;
            }

            // All other ControlPtgs and ScalarConstantPtgs cannot be used with ':'
            return false;
        }
        /**
         * 
         * "A1", "B3" -> "A1:B3"   
         * "sheet1!A1", "B3" -> "sheet1!A1:B3"
         * 
         * @return <c>null</c> if the range expression cannot / shouldn't be reduced.
         */
        private static Ptg ReduceRangeExpression(Ptg ptgA, Ptg ptgB)
        {
            if (!(ptgB is RefPtg))
            {
                // only when second ref is simple 2-D ref can the range 
                // expression be converted To an area ref
                return null;
            }
            RefPtg refB = (RefPtg)ptgB;

            if (ptgA is RefPtg)
            {
                RefPtg refA = (RefPtg)ptgA;
                return new AreaPtg(refA.Row, refB.Row, refA.Column, refB.Column,
                        refA.IsRowRelative, refB.IsRowRelative, refA.IsColRelative, refB.IsColRelative);
            }
            if (ptgA is Ref3DPtg)
            {
                Ref3DPtg refA = (Ref3DPtg)ptgA;
                return new Area3DPtg(refA.Row, refB.Row, refA.Column, refB.Column,
                        refA.IsRowRelative, refB.IsRowRelative, refA.IsColRelative, refB.IsColRelative,
                        refA.ExternSheetIndex);
            }
            // Note - other operand types (like AreaPtg) which probably can't evaluate 
            // do not cause validation errors at Parse time
            return null;
        }

        //private Ptg ParseNameOrCellRef(Identifier iden)
        //{

        //    if (look == '!')
        //    {
        //        GetChar();
        //        // 3-D ref
        //        // this code assumes iden is a sheetName
        //        // TODO - handle <book name> ! <named range name>
        //        int externIdx = GetExternalSheetIndex(iden.Name);
        //        String secondIden = ParseUnquotedIdentifier();
        //        AreaReference areaRef = ParseArea(secondIden);
        //        if (areaRef == null)
        //        {
        //            return new Ref3DPtg(secondIden, (short)externIdx);
        //        }
        //        // will happen if dots are used instead of colon
        //        return new Area3DPtg(areaRef.FormatAsString(), (short)externIdx);
        //    }

        //    String name = iden.Name;
        //    AreaReference areaRef2 = ParseArea(name);
        //    if (areaRef2 != null)
        //    {
        //        // will happen if dots are used instead of colon
        //        return new AreaPtg(areaRef2.FormatAsString());
        //    }
        //    // This can be either a cell ref or a named range


        //    int nameType = CellReference.ClassifyCellReference(name);
        //    if (nameType == NameType.CELL)
        //    {
        //        return new RefPtg(name);
        //    }
        //    if (look == ':')
        //    {
        //        if (nameType == NameType.COLUMN)
        //        {
        //            GetChar();
        //            String secondIden = ParseUnquotedIdentifier();
        //            if (CellReference.ClassifyCellReference(secondIden) != NameType.COLUMN)
        //            {
        //                throw new FormulaParseException("Expected full column after '" + name
        //                        + ":' but got '" + secondIden + "'");
        //            }
        //            return new AreaPtg(name + ":" + secondIden);
        //        }
        //    }
        //    if (nameType != NameType.NAMED_RANGE)
        //    {
        //        new FormulaParseException("Name '" + name
        //            + "' does not look like a cell reference or named range");
        //    }
        //    EvaluationName evalName = book.GetName(name,_sheetIndex);
        //    if (evalName == null)
        //    {
        //        throw new FormulaParseException("Specified named range '"
        //                + name + "' does not exist in the current workbook.");
        //    }
        //    if (evalName.IsRange)
        //    {
        //        return evalName.CreatePtg();
        //    }
        //    throw new FormulaParseException("Specified name '"
        //                + name + "' is not a range as expected");
        //}

        private int GetExternalSheetIndex(String name)
        {
            if (name[0]== '[')
            {
                // we have a sheet name qualified with workbook name e.g. '[MyData.xls]Sheet1'
                int pos = name.LastIndexOf(']'); // safe because sheet names never have ']'
                String wbName = name.Substring(1, pos - 1);
                String sheetName = name.Substring(pos + 1);
                return book.GetExternalSheetIndex(wbName, sheetName);
            }
            return book.GetExternalSheetIndex(name);
        }

        /**
         * @param name an 'identifier' like string (i.e. contains alphanums, and dots)
         * @return <c>null</c> if name cannot be split at a dot
         */
        private AreaReference ParseArea(String name)
        {
            int dotPos = name.IndexOf('.');
            if (dotPos < 0)
            {
                return null;
            }
            int dotCount = 1;
            while (dotCount < name.Length && name[dotPos + dotCount] == '.')
            {
                dotCount++;
                if (dotCount > 3)
                {
                    // four or more consecutive dots does not convert To ':'
                    return null;
                }
            }
            // This expression is only valid as an area ref, if the LHS and RHS of the dot(s) are both
            // cell refs.  Otherwise, this expression must be a named range name
            String partA = name.Substring(0, dotPos);
            if (!IsValidCellReference(partA))
            {
                return null;
            }
            String partB = name.Substring(dotPos + dotCount);
            if (!IsValidCellReference(partB))
            {
                return null;
            }
            CellReference topLeft = new CellReference(partA);
            CellReference bottomRight = new CellReference(partB);
            return new AreaReference(topLeft, bottomRight);
        }

        /**
         * @return <c>true</c> if the specified name is a valid cell reference
         */
        private static bool IsValidCellReference(String str)
        {
            return CellReference.ClassifyCellReference(str, _ssVersion) == NameType.CELL;
        }


        /**
         * Note - Excel Function names are 'case aware but not case sensitive'.  This method may end
         * up creating a defined name record in the workbook if the specified name is not an internal
         * Excel Function, and Has not been encountered before.
         *
         * @param name case preserved Function name (as it was entered/appeared in the formula).
         */
        private ParseNode Function(String name)
        {
            Ptg nameToken = null;
            if (!AbstractFunctionPtg.IsBuiltInFunctionName(name))
            {
                // user defined Function
                // in the Token tree, the name is more or less the first argument

                EvaluationName hName = book.GetName(name, _sheetIndex);
                if (hName == null)
                {

                    nameToken = book.GetNameXPtg(name);
                    if (nameToken == null)
                    {
                        throw new FormulaParseException("Name '" + name
                                + "' is completely unknown in the current workbook");
                    }
                }
                else
                {
                    if (!hName.IsFunctionName)
                    {
                        throw new FormulaParseException("Attempt To use name '" + name
                                + "' as a Function, but defined name in workbook does not refer To a Function");
                    }

                    // calls To user-defined Functions within the workbook
                    // Get a Name Token which points To a defined name record
                    nameToken = hName.CreatePtg();
                }
            }

            Match('(');
            ParseNode[] args = Arguments();
            Match(')');

            return GetFunction(name, nameToken, args);
        }

        /**
         * Generates the variable Function ptg for the formula.
         * 
         * For IF Formulas, Additional PTGs are Added To the Tokens
         * @param name a {@link NamePtg} or {@link NameXPtg} or <c>null</c>
         * @param numArgs
         * @return Ptg a null is returned if we're in an IF formula, it needs extreme manipulation and is handled in this Function
         */
        private ParseNode GetFunction(String name, Ptg namePtg, ParseNode[] args)
        {

            FunctionMetadata fm = FunctionMetadataRegistry.GetFunctionByName(name.ToUpper());
            int numArgs = args.Length;
            if (fm == null)
            {
                if (namePtg == null)
                {
                    throw new InvalidOperationException("NamePtg must be supplied for external Functions");
                }
                // must be external Function
                ParseNode[] allArgs = new ParseNode[numArgs + 1];
                allArgs[0] = new ParseNode(namePtg);
                System.Array.Copy(args, 0, allArgs, 1, numArgs);
                return new ParseNode(new FuncVarPtg(name, (byte)(numArgs + 1)), allArgs);
            }

            if (namePtg != null)
            {
                throw new InvalidOperationException("NamePtg no applicable To internal Functions");
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
                String msg = "Too few arguments to function '" + fm.Name + "'. ";
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
                String msg = "Too many arguments to function '" + fm.Name + "'. ";
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

        /** Get arguments To a Function */
        private ParseNode[] Arguments()
        {
            //average 2 args per Function
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
                temp.Add(ComparisonExpression());
                numArgs++;
                missedPrevArg = false;
                SkipWhite();
                if (!IsArgumentDelimiter(look))
                {
                    throw expected("',' or ')'");
                }
            }
             ParseNode[] result = (ParseNode[])temp.ToArray(typeof(ParseNode));
            return result;
        }

        /** Parse and Translate a Math Factor  */
        private ParseNode PowerFactor()
        {
            ParseNode result = PercentFactor();
            while (true)
            {
                SkipWhite();
                if (look != '^')
                {
                    return result;
                }
                Match('^');
                ParseNode other = PercentFactor();
                result = new ParseNode(PowerPtg.instance, result, other);
            }
        }

        private ParseNode PercentFactor()
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
        private string CELL_REF_PATTERN = "^(\\$?[A-Za-z]+)(\\$?[0-9]+)?";
        /**
 * Parses out a potential LHS or RHS of a ':' intended to produce a plain AreaRef.  Normally these are
 * proper cell references but they could also be row or column refs like "$AC" or "10"
 * @return <code>null</code> (and leaves {@link #_pointer} unchanged if a proper range part does not parse out
 */
        private SimpleRangePart ParseSimpleRangePart()
        {
            int ptr = pointer - 1; // TODO avoid StringIndexOutOfBounds
            bool hasDigits = false;
            bool hasLetters = false;
            while (ptr < formulaLength)
            {
                char ch = formulaString[ptr];
                if (Char.IsDigit(ch))
                {
                    hasDigits = true;
                }
                else if (Char.IsLetter(ch))
                {
                    hasLetters = true;
                }
                else if (ch == '$')
                {
                    //
                }
                else
                {
                    break;
                }
                ptr++;
            }
            if (ptr <= pointer - 1)
            {
                return null;
            }
            String rep = formulaString.Substring(pointer - 1, ptr-pointer+1);

            Regex pattern = new Regex(CELL_REF_PATTERN);

            if (!pattern.IsMatch(rep))
            {
                return null;
            }
            // Check range bounds against grid max
            if (hasLetters && hasDigits)
            {
                if (!IsValidCellReference(rep))
                {
                    return null;
                }
            }
            else if (hasLetters)
            {
                if (!CellReference.IsColumnWithnRange(rep.Replace("$", ""),_ssVersion))
                {
                    return null;
                }
            }
            else if (hasDigits)
            {
                int i;
                try
                {
                    i = Int32.Parse(rep.Replace("$", ""));
                }
                catch (FormatException e)
                {
                    return null;
                }
                if (i < 1 || i > 65536)
                {
                    return null;
                }
            }
            else
            {
                // just dollars ? can this happen?
                return null;
            }


            ResetPointer(ptr + 1); // stepping forward
            return new SimpleRangePart(rep, hasLetters, hasDigits);
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
                    return new ParseNode(ErrPtg.ValueOf(ParseErrorLiteral()));
                case '-':
                    Match('-');
                    return new ParseNode(UnaryMinusPtg.instance, PowerFactor());
                case '+':
                    Match('+');
                    return new ParseNode(UnaryPlusPtg.instance, PowerFactor());
                case '(':
                    Match('(');
                    ParseNode inside = ComparisonExpression();
                    Match(')');
                    return new ParseNode(ParenthesisPtg.instance, inside);
                case '"':
                    return new ParseNode(new StringPtg(ParseStringLiteral()));
                case '{':
                    Match('{');
                    ParseNode arrayNode = ParseArray();
                    Match('}');
                    return arrayNode;
            }
            if (IsAlpha(look) || Char.IsDigit(look) || look == '\'' || look == '[')
            {
                return ParseRangeExpression();
            }
            if (look == '.')
            {
                return new ParseNode(ParseNumber());
            }
            // else - assume number
            return new ParseNode(ParseNumber());
        }


        private ParseNode ParseArray()
        {
            ArrayList rowsData = new ArrayList();
            while (true)
            {
                Object[] singleRowData = ParseArrayRow();
                rowsData.Add(singleRowData);
                if (look == '}')
                {
                    break;
                }
                if (look != ';')
                {
                    throw expected("'}' or ';'");
                }
                Match(';');
            }
            int nRows = rowsData.Count;
            Object[][] values2d = new Object[nRows][];
            values2d=(Object[][])rowsData.ToArray();
            int nColumns = values2d[0].Length;
            CheckRowLengths(values2d, nColumns);

            return new ParseNode(new ArrayPtg(values2d));
        }
        private void CheckRowLengths(Object[][] values2d, int nColumns)
        {
            for (int i = 0; i < values2d.Length; i++)
            {
                int rowLen = values2d[i].Length;
                if (rowLen != nColumns)
                {
                    throw new FormulaParseException("Array row " + i + " Has length " + rowLen
                            + " but row 0 Has length " + nColumns);
                }
            }
        }

        private Object[] ParseArrayRow()
        {
            ArrayList temp = new ArrayList();
            while (true)
            {
                temp.Add(ParseArrayItem());
                SkipWhite();
                switch (look)
                {
                    case '}':
                    case ';':
                        break;
                    case ',':
                        Match(',');
                        continue;
                    default:
                        throw expected("'}' or ','");

                }
                break;
            }

            Object[] result = new Object[temp.Count];
            result=temp.ToArray();
            return result;
        }

        private Object ParseArrayItem()
        {
            SkipWhite();
            switch (look)
            {
                case '"': return new UnicodeString(ParseStringLiteral());
                case '#': return ErrorConstant.ValueOf(ParseErrorLiteral());
                case 'F':
                case 'f':
                case 'T':
                case 't':
                    return ParseBooleanLiteral();
            }
            // else assume number
            return ConvertArrayNumber(ParseNumber());
        }

        private Boolean ParseBooleanLiteral()
        {
            String iden = ParseUnquotedIdentifier();
            if ("TRUE".Equals(iden,StringComparison.InvariantCultureIgnoreCase))
            {
                return true;
            }
            if ("FALSE".Equals(iden,StringComparison.InvariantCultureIgnoreCase))
            {
                return false;
            }
            throw expected("'TRUE' or 'FALSE'");
        }

        private static Double ConvertArrayNumber(Ptg ptg)
        {
            if (ptg is IntPtg)
            {
                return ((IntPtg)ptg).Value;
            }
            if (ptg is NumberPtg)
            {
                return ((NumberPtg)ptg).Value;
            }
            throw new Exception("Unexpected ptg (" + ptg.GetType().Name + ")");
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
                    throw expected("int");
                }
                exponent = sign + number;
            }

            if (number1 == null && number2 == null)
            {
                throw expected("int");
            }

            return GetNumberPtgFromString(number1, number2, exponent);
        }


        private int ParseErrorLiteral()
        {
            Match('#');
            String part1 = ParseUnquotedIdentifier().ToUpper();

            switch (part1[0])
            {
                case 'V':
                    if (part1.Equals("VALUE"))
                    {
                        Match('!');
                        return HSSFErrorConstants.ERROR_VALUE;
                    }
                    throw expected("#VALUE!");
                case 'R':
                    if (part1.Equals("REF"))
                    {
                        Match('!');
                        return HSSFErrorConstants.ERROR_REF;
                    }
                    throw expected("#REF!");
                case 'D':
                    if (part1.Equals("DIV"))
                    {
                        Match('/');
                        Match('0');
                        Match('!');
                        return HSSFErrorConstants.ERROR_DIV_0;
                    }
                    throw expected("#DIV/0!");
                case 'N':
                    if (part1.Equals("NAME"))
                    {
                        Match('?');  // only one that ends in '?'
                        return HSSFErrorConstants.ERROR_NAME;
                    }
                    if (part1.Equals("NUM"))
                    {
                        Match('!');
                        return HSSFErrorConstants.ERROR_NUM;
                    }
                    if (part1.Equals("NULL"))
                    {
                        Match('!');
                        return HSSFErrorConstants.ERROR_NULL;
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
                        return HSSFErrorConstants.ERROR_NA;
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
                catch (FormatException)
                {
                    return new NumberPtg(numberStr);
                }
                catch (OverflowException)
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


        private String ParseStringLiteral()
        {
            Match('"');

            StringBuilder Token = new StringBuilder();
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
                Token.Append(look);
                GetChar();
            }
            return Token.ToString();
        }

        /** Parse and Translate a Math Term */
        private ParseNode Term()
        {
            ParseNode result = PowerFactor();
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
                ParseNode other = PowerFactor();
                result = new ParseNode(operator1, result, other);
            }
        }

        private ParseNode ComparisonExpression()
        {
            ParseNode result = ConcatExpression();
            while (true)
            {
                SkipWhite();
                switch (look)
                {
                    case '=':
                    case '>':
                    case '<':
                        Ptg comparisonToken = GetComparisonToken();
                        ParseNode other = ConcatExpression();
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
            bool IsGreater = look == '>';
            Match(look);
            if (IsGreater)
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


        private ParseNode ConcatExpression()
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
         * 
         */
        private void Parse()
        {
            pointer = 0;
            GetChar();
            _rootNode = UnionExpression();

            if (pointer <= formulaLength)
            {
                String msg = "Unused input [" + formulaString.Substring(pointer - 1)
                    + "] after attempting To Parse the formula [" + formulaString + "]";
                throw new FormulaParseException(msg);
            }
        }
        private ParseNode UnionExpression()
        {
            ParseNode result = ComparisonExpression();
            bool hasUnions = false;
            while (true)
            {
                SkipWhite();
                switch (look)
                {
                    case ',':
                        GetChar();
                        hasUnions = true;
                        ParseNode other = ComparisonExpression();
                        result = new ParseNode(UnionPtg.instance, result, other);
                        continue;
                }
                //if (hasUnions)
                //{
                //    return AugmentWithMemPtg(result);
                //}
                return result;
            }
        }


        private Ptg[] GetRPNPtg(FormulaType formulaType)
        {
            OperandClassTransformer oct = new OperandClassTransformer(formulaType);
            // RVA is for 'operand class': 'reference', 'value', 'array'
            oct.TransformFormula(_rootNode);
            return ParseNode.ToTokenArray(_rootNode);
        }
    }
}