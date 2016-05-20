/* ====================================================================
   Licensed to the Apache SoftwAre Foundation (ASF) under one or more
   contributor license agreements.  See the NOTICE file distributed with
   this work for additional information regarding copyright ownership.
   The ASF licenses this file to You under the Apache License, Version 2.0
   (the "License"); you may not use this file except in compliance with
   the License.  You may obtain a copy of the License at

       http://www.apache.org/licenses/LICENSE-2.0

   Unless required by applicable law or agreed to in writing, softwAre
   distributed under the License is distributed on an "AS IS" BASIS,
   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
   See the License for the specific language governing permissions and
   limitations under the License.
==================================================================== */

namespace NPOI.HSSF.UserModel
{
    using System;
    using System.Collections;
    using System.Text.RegularExpressions;

    /// <summary>
    /// Common class for HSSFHeader and HSSFFooter
    /// </summary>
    public abstract class HeaderFooter:NPOI.SS.UserModel.HeaderFooter
    {
        protected String left;
        protected String center;
        protected String right;

        protected bool stripFields = false;

        protected HeaderFooter(String text)
        {
            while (text != null && text.Length > 1)
            {
                int pos = text.Length;
                switch (text.Substring(1, text.Length-2>0?2:text.Length-1)[0])
                {
                    case 'L':
                        if (text.IndexOf("&C") >= 0)
                        {
                            pos = Math.Min(pos, text.IndexOf("&C"));
                        }
                        if (text.IndexOf("&R") >= 0)
                        {
                            pos = Math.Min(pos, text.IndexOf("&R"));
                        }
                        left = text.Substring(2, pos - 2);
                        text = text.Substring(pos);
                        break;
                    case 'C':
                        if (text.IndexOf("&L") >= 0)
                        {
                            pos = Math.Min(pos, text.IndexOf("&L"));
                        }
                        if (text.IndexOf("&R") >= 0)
                        {
                            pos = Math.Min(pos, text.IndexOf("&R"));
                        }
                        center = text.Substring(2, pos-2);
                        text = text.Substring(pos);
                        break;
                    case 'R':
                        if (text.IndexOf("&C") >= 0)
                        {
                            pos = Math.Min(pos, text.IndexOf("&C"));
                        }
                        if (text.IndexOf("&L") >= 0)
                        {
                            pos = Math.Min(pos, text.IndexOf("&L"));
                        }
                        right = text.Substring(2, pos -2);
                        text = text.Substring(pos);
                        break;
                    default:
                        text = null;
                        break;
                }
            }
        }

        /// <summary>
        /// Get the left side of the header or footer.
        /// </summary>
        /// <value>The string representing the left side.</value>
        public abstract String Left 
        {
            get; set; 
        }
        /// <summary>
        /// Get the center of the header or footer.
        /// </summary>
        /// <value>The string representing the center.</value>
        public abstract String Center
        {
            get;
            set;
        }

        /// <summary>
        /// Get the right side of the header or footer.
        /// </summary>
        /// <value>The string representing the right side..</value>
        public abstract String Right
        {
            get;
            set;
        }


        /// <summary>
        /// Returns the string that represents the change in font size.
        /// </summary>
        /// <param name="size">the new font size.</param>
        /// <returns>The special string to represent a new font size</returns>
        public static String FontSize(short size)
        {
            return "&" + size;
        }

        /// <summary>
        /// Returns the string that represents the change in font.
        /// </summary>
        /// <param name="font">the new font.</param>
        /// <param name="style">the fonts style, one of regular, italic, bold, italic bold or bold italic.</param>
        /// <returns>The special string to represent a new font size</returns>
        public static String Font(String font, String style)
        {
            return "&\"" + font + "," + style + "\"";
        }

        /// <summary>
        /// Returns the string representing the current page number
        /// </summary>
        /// <value>The special string for page number.</value>
        public static String Page
        {
            get
            {
                return PAGE_FIELD.sequence;
            }
        }

        /// <summary>
        /// Returns the string representing the number of pages.
        /// </summary>
        /// <value>The special string for the number of pages.</value>
        public static String NumPages
        {
            get
            {
                return NUM_PAGES_FIELD.sequence;
            }
        }

        /// <summary>
        /// Returns the string representing the current date
        /// </summary>
        /// <value>The special string for the date</value>
        public static String Date
        {
            get
            {
                return DATE_FIELD.sequence;
            }
        }

        /// <summary>
        /// Gets the time.
        /// </summary>
        /// <value>The time.</value>
        /// Returns the string representing the current time
        /// @return The special string for the time
        public static String Time
        {
            get
            {
                return TIME_FIELD.sequence;
            }
        }

        /// <summary>
        /// Returns the string representing the current file name
        /// </summary>
        /// <value>The special string for the file name.</value>
        public static String File
        {
            get{
                return FILE_FIELD.sequence;
            }
        }

        /// <summary>
        /// Returns the string representing the current tab (sheet) name
        /// </summary>
        /// <value>The special string for tab name.</value>
        public static String Tab
        {
            get
            {
                return SHEET_NAME_FIELD.sequence;
            }
        }

        /// <summary>
        /// Returns the string representing the start bold
        /// </summary>
        /// <returns>The special string for start bold</returns>
        public static String StartBold
        {
            get
            {
                return BOLD_FIELD.sequence;
            }
        }

        /// <summary>
        /// Returns the string representing the end bold
        /// </summary>
        /// <value>The special string for end bold.</value>
        public static String EndBold
        {
            get
            {
                return BOLD_FIELD.sequence;
            }
        }

        /// <summary>
        /// Returns the string representing the start underline
        /// </summary>
        /// <value>The special string for start underline.</value>
        public static String StartUnderline
        {
            get
            {
                return UNDERLINE_FIELD.sequence;
            }
        }

        /// <summary>
        /// Returns the string representing the end underline
        /// </summary>
        /// <value>The special string for end underline.</value>
        public static String EndUnderline
        {
            get
            {
                return UNDERLINE_FIELD.sequence;
            }
        }

        /// <summary>
        /// Returns the string representing the start double underline
        /// </summary>
        /// <value>The special string for start double underline.</value>
        public static String StartDoubleUnderline
        {
            get
            {
                return DOUBLE_UNDERLINE_FIELD.sequence;
            }
        }

        /// <summary>
        /// Returns the string representing the end double underline
        /// </summary>
        /// <value>The special string for end double underline.</value>
        public static String EndDoubleUnderline
        {
            get
            {
                return DOUBLE_UNDERLINE_FIELD.sequence;
            }
        }


        /// <summary>
        /// Removes any fields (eg macros, page markers etc)
        /// from the string.
        /// Normally used to make some text suitable for showing
        /// to humans, and the resultant text should not normally
        /// be saved back into the document!
        /// </summary>
        /// <param name="text">The text.</param>
        /// <returns></returns>
        public static String StripFields(String text)
        {
            int pos;

            // Check we really got something to work on
            if (text == null || text.Length == 0)
            {
                return text;
            }

            // Firstly, do the easy ones which Are static
            for (int i = 0; i < Field.ALL_FIELDS.Count; i++)
            {
                String seq = ((Field)Field.ALL_FIELDS[i]).sequence;
                while ((pos = text.IndexOf(seq)) > -1)
                {
                    text = text.Substring(0, pos) +
                        text.Substring(pos + seq.Length);
                }
            }

            // Now do the tricky, dynamic ones
            // These Are things like font sizes and font names

            text = Regex.Replace(text,@"\&\d+", "");
            text = Regex.Replace(text,"\\&\".*?,.*?\"", "");

            // All done
            return text;
        }


        /// <summary>
        /// Are fields currently being Stripped from
        /// the text that this {@link HeaderStories} returns?
        /// Default is false, but can be changed
        /// </summary>
        /// <value><c>true</c> if [are fields stripped]; otherwise, <c>false</c>.</value>
        public bool AreFieldsStripped
        {
            get
            {
                return stripFields;
            }
            set 
            {
                this.stripFields = value; 
            }
        }


        public static Field SHEET_NAME_FIELD = new Field("&A");
        public static Field DATE_FIELD = new Field("&D");
        public static Field FILE_FIELD = new Field("&F");
        public static Field FULL_FILE_FIELD = new Field("&Z");
        public static Field PAGE_FIELD = new Field("&P");
        public static Field TIME_FIELD = new Field("&T");
        public static Field NUM_PAGES_FIELD = new Field("&N");

        public static Field PICTURE_FIELD = new Field("&G");

        public static PairField BOLD_FIELD = new PairField("&B");
        public static PairField ITALIC_FIELD = new PairField("&I");
        public static PairField STRIKETHROUGH_FIELD = new PairField("&S");
        public static PairField SUBSCRIPT_FIELD = new PairField("&Y");
        public static PairField SUPERSCRIPT_FIELD = new PairField("&X");
        public static PairField UNDERLINE_FIELD = new PairField("&U");
        public static PairField DOUBLE_UNDERLINE_FIELD = new PairField("&E");

        /// <summary>
        /// Represents a special field in a header or footer,
        /// eg the page number
        /// </summary>
        public class Field
        {
            public static ArrayList ALL_FIELDS = new ArrayList();
            /** The character sequence that marks this field */
            public String sequence;
            public Field(String sequence)
            {
                this.sequence = sequence;
                ALL_FIELDS.Add(this);
            }
        }
        /// <summary>
        /// A special field that normally comes in a pair, eg
        /// turn on underline / turn off underline
        /// </summary>
        public class PairField : Field
        {
            public PairField(String sequence)
                : base(sequence)
            {

            }
        }
    }
}