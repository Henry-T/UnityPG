/* ====================================================================
   Licensed to the Apache Software Foundation (ASF) under one or more
   contributor license agreements.  See the NOTICE file distributed with
   this work for Additional inFormation regarding copyright ownership.
   The ASF licenses this file to You under the Apache License, Version 2.0
   (the "License"); you may not use this file except in compliance with
   the License.  You may obtain a copy of the License at

       http://www.apache.org/licenses/LICENSE-2.0

   Unless required by applicable law or agreed to in writing, software
   distributed under the License is distributed on an "AS IS" BASIS,
   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
   See the License for the specific language governing permissions and
   limitations under the License.
==================================================================== */

namespace NPOI.HSSF.UserModel
{
    using System;
    using System.Collections;
    using System.Text;
    using System.Text.RegularExpressions;

    using NPOI.HSSF.Util;
    using NPOI.DDF;
    using NPOI.HSSF.Model;
    using NPOI.HSSF.Record;




    /**
     * HSSFDataFormatter contains methods for Formatting the value stored in an
     * HSSFCell. This can be useful for reports and GUI presentations when you
     * need to display data exactly as it appears in Excel. Supported Formats
     * include currency, SSN, percentages, decimals, dates, phone numbers, zip
     * codes, etc.
     * 
     * Internally, Formats will be implemented using subclasses of {@link FormatBase}
     * such as {@link DecimalFormat} and {@link SimpleDateFormat}. Therefore the
     * Formats used by this class must obey the same pattern rules as these FormatBase
     * subclasses. This means that only legal number pattern characters ("0", "#",
     * ".", "," etc.) may appear in number formats. Other characters can be
     * inserted <em>before</em> or <em> after</em> the number pattern to form a
     * prefix or suffix.
     * 
     * 
     * For example the Excel pattern <c>"$#,##0.00 "USD"_);($#,##0.00 "USD")"
     * </c> will be correctly Formatted as "$1,000.00 USD" or "($1,000.00 USD)".
     * However the pattern <c>"00-00-00"</c> is incorrectly Formatted by
     * DecimalFormat as "000000--". For Excel Formats that are not compatible with
     * DecimalFormat, you can provide your own custom {@link FormatBase} implementation
     * via <c>HSSFDataFormatter.AddFormat(String,FormatBase)</c>. The following
     * custom Formats are already provided by this class:
     * 
     * <pre>
     * <ul><li>SSN "000-00-0000"</li>
     *     <li>Phone Number "(###) ###-####"</li>
     *     <li>Zip plus 4 "00000-0000"</li>
     * </ul>
     * </pre>
     * 
     * If the Excel FormatBase pattern cannot be Parsed successfully, then a default
     * FormatBase will be used. The default number FormatBase will mimic the Excel General
     * FormatBase: "#" for whole numbers and "#.##########" for decimal numbers. You
     * can override the default FormatBase pattern with <c>
     * HSSFDataFormatter.setDefaultNumberFormat(FormatBase)</c>. <b>Note:</b> the
     * default FormatBase will only be used when a FormatBase cannot be Created from the
     * cell's data FormatBase string.
     *
     * @author James May (james dot may at fmr dot com)
     *
     */
    public class HSSFDataFormatter
    {

        /** Pattern to find a number FormatBase: "0" or  "#" */
        private static string numPattern = "[0#]+";

        /** Pattern to find days of week as text "ddd...." */
        private static string daysAsText = "([d]{3,})";

        /** Pattern to find "AM/PM" marker */
        private static string amPmPattern = "((A|P)[M/P]*)";

        /** A regex to find patterns like [$$-1009] and [$�-452]. */
        private static string specialPatternGroup = "(\\[\\$[^-\\]]*-[0-9A-Z]+\\])";

        /** <em>General</em> FormatBase for whole numbers. */
        private static DecimalFormat generalWholeNumFormat = new DecimalFormat("0");

        /** <em>General</em> FormatBase for decimal numbers. */
        private static DecimalFormat generalDecimalNumFormat = new DecimalFormat("#.##########");

        /** A default FormatBase to use when a number pattern cannot be Parsed. */
        private FormatBase defaultNumFormat;

        /**
         * A map to cache formats.
         *  Map<String,FormatBase> Formats
         */
        private Hashtable formats;

        /**
         * Constructor
         */
        public HSSFDataFormatter()
        {
            formats = new Hashtable();

            // init built-in Formats

            FormatBase zipFormat = ZipPlusFourFormat.instance;
            AddFormat("00000\\-0000", zipFormat);
            AddFormat("00000-0000", zipFormat);

            FormatBase phoneFormat = PhoneFormat.instance;
            // allow for FormatBase string variations
            AddFormat("[<=9999999]###\\-####;\\(###\\)\\ ###\\-####", phoneFormat);
            AddFormat("[<=9999999]###-####;(###) ###-####", phoneFormat);
            AddFormat("###\\-####;\\(###\\)\\ ###\\-####", phoneFormat);
            AddFormat("###-####;(###) ###-####", phoneFormat);

            FormatBase ssnFormat = SSNFormat.instance;
            AddFormat("000\\-00\\-0000", ssnFormat);
            AddFormat("000-00-0000", ssnFormat);
        }

        /**
         * Return a FormatBase for the given cell if one exists, otherwise try to
         * Create one. This method will return <c>null</c> if the any of the
         * following is true:
         * <ul>
         * <li>the cell's style is null</li>
         * <li>the style's data FormatBase string is null or empty</li>
         * <li>the FormatBase string cannot be recognized as either a number or date</li>
         * </ul>
         *
         * @param cell The cell to retrieve a FormatBase for
         * @return A FormatBase for the FormatBase String
         */
        private FormatBase GetFormat(HSSFCell cell)
        {
            if (cell.CellStyle == null)
            {
                return null;
            }

            int formatIndex = cell.CellStyle.DataFormat;
            String formatStr = cell.CellStyle.DataFormatString;
            if (formatStr == null || formatStr.Trim().Length == 0)
            {
                return null;
            }
            return GetFormat(cell.NumericCellValue, formatIndex, formatStr);
        }

        private FormatBase GetFormat(double cellValue, int formatIndex, String formatStr)
        {
            FormatBase format = (FormatBase)formats[formatStr];
            if (format != null)
            {
                return format;
            }
            if (formatStr.Equals("General"))
            {
                if (HSSFDataFormatter.IsWholeNumber(cellValue))
                {
                    return generalWholeNumFormat;
                }
                return generalDecimalNumFormat;
            }
            format = CreateFormat(cellValue, formatIndex, formatStr);
            formats[formatStr] = format;
            return format;
        }

        /**
         * Create and return a FormatBase based on the FormatBase string from a  cell's
         * style. If the pattern cannot be Parsed, return a default pattern.
         *
         * @param cell The Excel cell
         * @return A FormatBase representing the excel FormatBase. May return null.
         */
        public FormatBase CreateFormat(HSSFCell cell)
        {

            int formatIndex = cell.CellStyle.DataFormat;
            String formatStr = cell.CellStyle.DataFormatString;
            return CreateFormat(cell.NumericCellValue, formatIndex, formatStr);
        }

        private FormatBase CreateFormat(double cellValue, int formatIndex, String sFormat)
        {
            // remove color Formatting if present
            String formatStr = Regex.Replace(sFormat, "\\[[a-zA-Z]*\\]", "");

            // try to extract special characters like currency
            
            MatchCollection matches = Regex.Matches(formatStr, specialPatternGroup);
            foreach (Match match in matches)
            {
                string matchedstring = match.Value;
                int beginpos = matchedstring.IndexOf('$') + 1;
                int endpos = matchedstring.IndexOf('-');
                String symbol = matchedstring.Substring(beginpos, endpos - beginpos + 1);
                
                if (symbol.IndexOf('$') > -1)
                {
                    StringBuilder sb = new StringBuilder();
                    sb.Append(symbol.Substring(0, symbol.IndexOf('$')));
                    sb.Append('\\');
                    sb.Append(symbol.Substring(symbol.IndexOf('$'), symbol.Length));
                    symbol = sb.ToString();
                }
                matchedstring=Regex.Replace(matchedstring,specialPatternGroup,symbol);

                formatStr = formatStr.Remove(match.Index,match.Length);
                formatStr = formatStr.Insert(match.Index,matchedstring);
            }

            if (formatStr == null || formatStr.Trim().Length == 0)
            {
                return GetDefaultFormat(cellValue);
            }


            if (NPOI.SS.UserModel.DateUtil.IsADateFormat(formatIndex, formatStr) &&
                    NPOI.SS.UserModel.DateUtil.IsValidExcelDate(cellValue))
            {
                return CreateDateFormat(formatStr, cellValue);
            }
            if (Regex.IsMatch(formatStr, numPattern))
            {
                return CreateNumberFormat(formatStr, cellValue);
            }
            // TODO - when does this occur?
            return null;
        }

        private FormatBase CreateDateFormat(String pformatStr, double cellValue)
        {
            String formatStr = pformatStr;
            formatStr = formatStr.Replace("\\\\-", "-");
            formatStr = formatStr.Replace("\\\\,", ",");
            formatStr = formatStr.Replace("\\\\ ", " ");
            formatStr = formatStr.Replace(";@", "");
            bool hasAmPm = Regex.IsMatch(formatStr, amPmPattern);
            if (hasAmPm)
            {
                formatStr = Regex.Replace(formatStr, amPmPattern, "@");
            }
            formatStr = formatStr.Replace("@", "a");


            MatchCollection match = Regex.Matches(formatStr, daysAsText);
            if (match.Count > 0)
            {
                string replacement = match[0].Groups[0].Value.ToUpper().Replace("D", "E");
                formatStr = Regex.Replace(formatStr, daysAsText, replacement);
            }


            // Convert excel date FormatBase to SimpleDateFormat.
            // Excel uses lower case 'm' for both minutes and months.
            // From Excel help:
            /*
              The "m" or "mm" code must appear immediately after the "h" or"hh"
              code or immediately before the "ss" code; otherwise, Microsoft
              Excel displays the month instead of minutes."
            */

            StringBuilder sb = new StringBuilder();
            char[] chars = formatStr.ToCharArray();
            bool mIsMonth = true;
            ArrayList ms = new ArrayList();
            for (int j = 0; j < chars.Length; j++)
            {
                char c = chars[j];
                if (c == 'h' || c == 'H')
                {
                    mIsMonth = false;
                    if (hasAmPm)
                    {
                        sb.Append('h');
                    }
                    else
                    {
                        sb.Append('H');
                    }
                }
                else if (c == 'm')
                {
                    if (mIsMonth)
                    {
                        sb.Append('M');
                        ms.Add(
                                sb.Length - 1
                        );
                    }
                    else
                    {
                        sb.Append('m');
                    }
                }
                else if (c == 's' || c == 'S')
                {
                    sb.Append('s');
                    // if 'M' precedes 's' it should be minutes ('m')
                    for (int i = 0; i < ms.Count; i++)
                    {
                        int index = (int)ms[i];
                        if (sb[index] == 'M')
                        {
                            sb[index]='m';
                        }
                    }
                    mIsMonth = true;
                    ms.Clear();
                }
                else if (Char.IsLetter(c))
                {
                    mIsMonth = true;
                    ms.Clear();
                    if (c == 'y' || c == 'Y')
                    {
                        sb.Append('y');
                    }
                    else if (c == 'd' || c == 'D')
                    {
                        sb.Append('d');
                    }
                    else
                    {
                        sb.Append(c);
                    }
                }
                else
                {
                    sb.Append(c);
                }
            }
            formatStr = sb.ToString();

            try
            {
                return new SimpleDateFormat(formatStr);
            }
            catch (ArgumentException iae)
            {

                // the pattern could not be Parsed correctly,
                // so fall back to the default number FormatBase
                return GetDefaultFormat(cellValue);
            }

        }

        private FormatBase CreateNumberFormat(String formatStr, double cellValue)
        {
            StringBuilder sb = new StringBuilder(formatStr);
            for (int i = 0; i < sb.Length; i++)
            {
                char c = sb[i];
                //handle (#,##0_);
                if (c == '(')
                {
                    int idx = sb.ToString().IndexOf(")", i);
                    if (idx > -1 && sb[idx - 1] == '_')
                    {
                        sb.Remove(idx, 1);
                        sb.Remove(idx - 1, 1);
                        sb.Remove(i, 1);
                        i--;
                    }
                }
                else if (c == ')' && i > 0 && sb[i - 1] == '_')
                {
                    sb.Remove(i, 1);
                    sb.Remove(i - 1, 1);
                    i--;
                    // remove quotes and back slashes
                }
                else if (c == '\\' || c == '"')
                {
                    sb.Remove(i, 1);
                    i--;

                    // for scientific/engineering notation
                }
                else if (c == '+' && i > 0 && sb[i - 1] == 'E')
                {
                    sb.Remove(i, 1);
                    i--;
                }
            }

            try
            {
                return new DecimalFormat(sb.ToString());
            }
            catch (ArgumentException iae)
            {

                // the pattern could not be Parsed correctly,
                // so fall back to the default number FormatBase
                return GetDefaultFormat(cellValue);
            }
        }

        /**
         * Return true if the double value represents a whole number
         * @param d the double value to check
         * @return <c>true</c> if d is a whole number
         */
        private static bool IsWholeNumber(double d)
        {
            return d == Math.Floor(d);
        }

        /**
         * Returns a default FormatBase for a cell.
         * @param cell The cell
         * @return a default FormatBase
         */
        public FormatBase GetDefaultFormat(HSSFCell cell)
        {
            return GetDefaultFormat(cell.NumericCellValue);
        }
        private FormatBase GetDefaultFormat(double cellValue)
        {
            // for numeric cells try user supplied default
            if (defaultNumFormat != null)
            {
                return defaultNumFormat;

                // otherwise use general FormatBase
            }
            if (IsWholeNumber(cellValue))
            {
                return generalWholeNumFormat;
            }
            return generalDecimalNumFormat;
        }

        /**
         * Returns the Formatted value of an Excel date as a <tt>String</tt> based
         * on the cell's <c>DataFormat</c>. i.e. "Thursday, January 02, 2003"
         * , "01/02/2003" , "02-Jan" , etc.
         *
         * @param cell The cell
         * @return a Formatted date string
         */
        private String GetFormattedDateString(HSSFCell cell)
        {
            FormatBase dateFormat = GetFormat(cell);
            DateTime d = cell.DateCellValue;
            if (dateFormat != null)
            {
                return dateFormat.Format(d);
            }
            return d.ToString();
        }

        /**
         * Returns the Formatted value of an Excel number as a <tt>String</tt>
         * based on the cell's <c>DataFormat</c>. Supported Formats include
         * currency, percents, decimals, phone number, SSN, etc.:
         * "61.54%", "$100.00", "(800) 555-1234".
         *
         * @param cell The cell
         * @return a Formatted number string
         */
        private String GetFormattedNumberString(HSSFCell cell)
        {

            FormatBase numberFormat = GetFormat(cell);
            double d = cell.NumericCellValue;
            if (numberFormat == null)
            {
                return d.ToString();
            }
            return numberFormat.Format(d);
        }

        /**
         * Formats the given raw cell value, based on the supplied
         *  FormatBase index and string, according to excel style rules.
         * @see #FormatCellValue(HSSFCell)
         */
        public String FormatRawCellContents(double value, int formatIndex, String formatString)
        {
            // Is it a date?
            if (NPOI.SS.UserModel.DateUtil.IsADateFormat(formatIndex, formatString) &&
                    NPOI.SS.UserModel.DateUtil.IsValidExcelDate(value))
            {

                FormatBase dateFormat = GetFormat(value, formatIndex, formatString);
                DateTime d = NPOI.SS.UserModel.DateUtil.GetJavaDate(value);
                if (dateFormat == null)
                {
                    return d.ToString();
                }
                return dateFormat.Format(d);
            }
            // else Number
            FormatBase numberFormat = GetFormat(value, formatIndex, formatString);
            if (numberFormat == null)
            {
                return value.ToString();
            }
            return numberFormat.Format(value);
        }

        /**
         * 
         * Returns the Formatted value of a cell as a <tt>String</tt> regardless
         * of the cell type. If the Excel FormatBase pattern cannot be Parsed then the
         * cell value will be Formatted using a default FormatBase.
         * 
         * When passed a null or blank cell, this method will return an empty
         * String (""). Formulas in formula type cells will not be evaluated.
         * 
         *
         * @param cell The cell
         * @return the Formatted cell value as a String
         */
        //public String FormatCellValue(HSSFCell cell)
        //{
        //    return FormatCellValue(cell, null);
        //}

        ///**
        // * 
        // * Returns the Formatted value of a cell as a <tt>String</tt> regardless
        // * of the cell type. If the Excel FormatBase pattern cannot be Parsed then the
        // * cell value will be Formatted using a default FormatBase.
        // * 
        // * When passed a null or blank cell, this method will return an empty
        // * String (""). Formula cells will be evaluated using the given
        // * {@link HSSFFormulaEvaluator} if the evaluator is non-null. If the
        // * evaluator is null, then the formula String will be returned. The caller
        // * is responsible for setting the currentRow on the evaluator
        // *
        // *
        // * @param cell The cell (can be null)
        // * @param evaluator The HSSFFormulaEvaluator (can be null)
        // * @return a string value of the cell
        // */
        //public String FormatCellValue(HSSFCell cell, HSSFFormulaEvaluator evaluator)
        //{

        //    if (cell == null)
        //    {
        //        return "";
        //    }

        //    HSSFCellType cellType = cell.CellType;
        //    if (evaluator != null && cellType == HSSFCellType.FORMULA)
        //    {
        //        try
        //        {
        //            cellType = evaluator.EvaluateFormulaCell(cell);
        //        }
        //        catch (Exception e)
        //        {
        //            throw new Exception("Did you forGet to set the current" +
        //                    " row on the HSSFFormulaEvaluator?", e);
        //        }
        //    }
        //    switch (cellType)
        //    {
        //        case HSSFCellType.FORMULA:
        //            // should only occur if evaluator is null
        //            return cell.CellFormula;

        //        case HSSFCellType.NUMERIC:

        //            if (HSSFDateUtil.IsCellDateFormatted(cell))
        //            {
        //                return GetFormattedDateString(cell);
        //            }
        //            return GetFormattedNumberString(cell);

        //        case HSSFCellType.STRING:
        //            return cell.RichStringCellValue.String;

        //        case HSSFCellType.BOOLEAN:
        //            return cell.BooleanCellValue.ToString().ToUpper();
        //        case HSSFCellType.BLANK:
        //            return "";
        //    }
        //    throw new Exception("Unexpected celltype (" + cellType + ")");
        //}


        /**
         * 
         * Sets a default number FormatBase to be used when the Excel FormatBase cannot be
         * Parsed successfully. <b>Note:</b> This is a fall back for when an error
         * occurs while parsing an Excel number FormatBase pattern. This will not
         * affect cells with the <em>General</em> FormatBase.
         * 
         * 
         * The value that will be passed to the FormatBase's FormatBase method (specified
         * by <c>java.text.FormatBase#FormatBase</c>) will be a double value from a
         * numeric cell. Therefore the code in the FormatBase method should expect a
         * <c>Number</c> value.
         * 
         *
         * @param FormatBase A FormatBase instance to be used as a default
         * @see java.text.FormatBase#FormatBase
         */
        public void SetDefaultNumberFormat(FormatBase format)
        {
            IEnumerator itr = formats.Keys.GetEnumerator();
            while (itr.MoveNext())
            {
                string key = (string)itr.Current;
                if (formats[key] == generalDecimalNumFormat
                        || formats[key] == generalWholeNumFormat)
                {
                    formats[key] = format;
                }
            }
            defaultNumFormat = format;
        }

        /**
         * Adds a new FormatBase to the available formats.
         * 
         * The value that will be passed to the FormatBase's FormatBase method (specified
         * by <c>java.text.FormatBase#FormatBase</c>) will be a double value from a
         * numeric cell. Therefore the code in the FormatBase method should expect a
         * <c>Number</c> value.
         * 
         * @param excelformatStr The data FormatBase string
         * @param FormatBase A FormatBase instance
         */
        public void AddFormat(String excelformatStr, FormatBase format)
        {
            formats[excelformatStr]= format;
        }

        // Some custom Formats
    }
}