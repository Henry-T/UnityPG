/* ====================================================================
   Licensed to the Apache Software Foundation (ASF) Under one or more
   contributor license agreements.  See the NOTICE file distributed with
   this work for Additional information regarding copyright ownership.
   The ASF licenses this file to You Under the Apache License, Version 2.0
   (the "License"); you may not use this file except in compliance with
   the License.  You may obtain a copy of the License at

       http://www.apache.org/licenses/LICENSE-2.0

   Unless required by applicable law or agreed to in writing, software
   distributed Under the License is distributed on an "AS Is" BASIS,
   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
   See the License for the specific language governing permissions and
   limitations Under the License.
==================================================================== */

namespace NPOI.HSSF.UserModel
{
    using System;
    using System.IO;
    using System.Collections;

    using NPOI.HSSF.Record;
    using NPOI.HSSF.Model;
    using NPOI.Util;

    /// <summary>
    /// Rich text Unicode string.  These strings can have fonts applied to
    /// arbitary parts of the string.
    /// @author Glen Stampoultzis (glens at apache.org)
    /// @author Jason Height (jheight at apache.org)
    /// </summary> 
    [Serializable]
    public class HSSFRichTextString : IComparable,NPOI.SS.UserModel.RichTextString
    {
        /** Place holder for indicating that NO_FONT has been applied here */
        public const short NO_FONT = 0;

        private UnicodeString str;
        private Workbook book;
        private LabelSSTRecord record;

        /// <summary>
        /// Initializes a new instance of the <see cref="HSSFRichTextString"/> class.
        /// </summary>
        public HSSFRichTextString()
            : this("")
        {

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HSSFRichTextString"/> class.
        /// </summary>
        /// <param name="str">The string.</param>
        public HSSFRichTextString(String str)
        {
            if (str == null)
                str = "";
            this.str = new UnicodeString(str);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HSSFRichTextString"/> class.
        /// </summary>
        /// <param name="book">The workbook.</param>
        /// <param name="record">The record.</param>
        public HSSFRichTextString(Workbook book, LabelSSTRecord record)
        {
            SetWorkbookReferences(book, record);

            this.str = book.GetSSTString(record.SSTIndex);
        }

        /// <summary>
        /// This must be called to Setup the internal work book references whenever
        /// a RichTextString Is Added to a cell
        /// </summary>
        /// <param name="book">The workbook.</param>
        /// <param name="record">The record.</param>
        public void SetWorkbookReferences(Workbook book, LabelSSTRecord record)
        {
            this.book = book;
            this.record = record;
        }

        /// <summary>
        /// Called whenever the Unicode string Is modified. When it Is modified
        /// we need to Create a new SST index, so that other LabelSSTRecords will not
        /// be affected by Changes tat we make to this string.
        /// </summary>
        /// <returns></returns>
        private UnicodeString CloneStringIfRequired()
        {
            if (book == null)
                return str;
            UnicodeString s = (UnicodeString)str.Clone();
            return s;
        }

        /// <summary>
        /// Adds to SST if required.
        /// </summary>
        private void AddToSSTIfRequired()
        {
            if (book != null)
            {
                int index = book.AddSSTString(str);
                record.SSTIndex = (index);
                //The act of Adding the string to the SST record may have meant that
                //a extsing string was returned for the index, so update our local version
                str = book.GetSSTString(index);
            }
        }


        /// <summary>
        /// Applies a font to the specified Chars of a string.
        /// </summary>
        /// <param name="startIndex">The start index to apply the font to (inclusive).</param>
        /// <param name="endIndex">The end index to apply the font to (exclusive).</param>
        /// <param name="fontIndex">The font to use.</param>
        public void ApplyFont(int startIndex, int endIndex, short fontIndex)
        {
            if (startIndex > endIndex)
                throw new ArgumentException("Start index must be less than end index.");
            if (startIndex < 0 || endIndex > Length)
                throw new ArgumentException("Start and end index not in range.");
            if (startIndex == endIndex)
                return;

            //Need to Check what the font Is currently, so we can reapply it after
            //the range Is completed
            short currentFont = NO_FONT;
            if (endIndex != Length)
            {
                currentFont = this.GetFontAtIndex(startIndex);
            }

            //Need to clear the current formatting between the startIndex and endIndex
            str = CloneStringIfRequired();
            System.Collections.Generic.List<FormatRun> formatting = str.FormatIterator();

            ArrayList deletedFR = new ArrayList();
            if (formatting != null)
            {
                IEnumerator formats = formatting.GetEnumerator();
                while (formats.MoveNext())
                {
                    FormatRun r = (FormatRun)formats.Current;
                    if ((r.CharacterPos >= startIndex) && (r.CharacterPos < endIndex))
                    {
                        deletedFR.Add(r);
                    }
                }
            }
            foreach(FormatRun fr in deletedFR)
            {
                str.RemoveFormatRun(fr);
            }

            str.AddFormatRun(new FormatRun((short)startIndex, fontIndex));
            if (endIndex != Length)
                str.AddFormatRun(new FormatRun((short)endIndex, currentFont));

            AddToSSTIfRequired();
        }

        /// <summary>
        /// Applies a font to the specified Chars of a string.
        /// </summary>
        /// <param name="startIndex">The start index to apply the font to (inclusive).</param>
        /// <param name="endIndex"> The end index to apply to font to (exclusive).</param>
        /// <param name="font">The index of the font to use.</param>
        public void ApplyFont(int startIndex, int endIndex, NPOI.SS.UserModel.Font font)
        {
            ApplyFont(startIndex, endIndex, font.Index);
        }

        /// <summary>
        /// Sets the font of the entire string.
        /// </summary>
        /// <param name="font">The font to use.</param>
        public void ApplyFont(NPOI.SS.UserModel.Font font)
        {
            ApplyFont(0, str.CharCount, font);
        }

        /// <summary>
        /// Removes any formatting that may have been applied to the string.
        /// </summary>
        public void ClearFormatting()
        {
            str = CloneStringIfRequired();
            str.ClearFormatting();
            AddToSSTIfRequired();
        }

        /// <summary>
        /// Returns the plain string representation.
        /// </summary>
        /// <value>The string.</value>
        public String String
        {
            get { return str.String; }
        }
        /// <summary>
        /// Returns the raw, probably shared Unicode String.
        /// Used when tweaking the styles, eg updating font
        /// positions.
        /// Changes to this string may well effect
        /// other RichTextStrings too!
        /// </summary>
        /// <value>The raw unicode string.</value>
        public UnicodeString RawUnicodeString
        {
            get
            {
                return str;
            }
        }

        /// <summary>
        /// Gets or sets the unicode string.
        /// </summary>
        /// <value>The unicode string.</value>
        public UnicodeString UnicodeString
        {
            get { return CloneStringIfRequired(); }
            set { this.str = value; }
        }


        /// <summary>
        /// Gets the number of Chars in the font..
        /// </summary>
        /// <value>The length.</value>
        public int Length
        {
            get { return str.CharCount; }
        }

        /// <summary>
        /// Returns the font in use at a particular index.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <returns>The font that's currently being applied at that
        /// index or null if no font Is being applied or the
        /// index Is out of range.</returns>
        public short GetFontAtIndex(int index)
        {
            int size = str.FormatRunCount;
            FormatRun currentRun = null;
            for (int i = 0; i < size; i++)
            {
                FormatRun r = str.GetFormatRun(i);
                if (r.CharacterPos > index)
                    break;
                else currentRun = r;
            }
            if (currentRun == null)
                return NO_FONT;
            else return currentRun.FontIndex;
        }

        /// <summary>
        /// Gets the number of formatting runs used. There will always be at
        /// least one of font NO_FONT.
        /// </summary>
        /// <value>The num formatting runs.</value>
        public int NumFormattingRuns
        {
            get { return str.FormatRunCount; }
        }

        /// <summary>
        /// The index within the string to which the specified formatting run applies.
        /// </summary>
        /// <param name="index">the index of the formatting run</param>
        /// <returns>the index within the string.</returns>
        public int GetIndexOfFormattingRun(int index)
        {
            FormatRun r = str.GetFormatRun(index);
            return r.CharacterPos;
        }

        /// <summary>
        /// Gets the font used in a particular formatting run.
        /// </summary>
        /// <param name="index">the index of the formatting run.</param>
        /// <returns>the font number used.</returns>
        public short GetFontOfFormattingRun(int index)
        {
            FormatRun r = str.GetFormatRun(index);
            return r.FontIndex;
        }

        /// <summary>
        /// Compares one rich text string to another.
        /// </summary>
        /// <param name="o">The o.</param>
        /// <returns></returns>
        public int CompareTo(Object o)
        {
            HSSFRichTextString r = (HSSFRichTextString)o;
            return str.CompareTo(r.String);
        }

        /// <summary>
        /// Equalses the specified o.
        /// </summary>
        /// <param name="o">The o.</param>
        /// <returns></returns>
        public override bool Equals(Object o)
        {
            if (o is HSSFRichTextString)
            {
                return str.Equals(((HSSFRichTextString)o).String);
            }
            return false;

        }

        /// <summary>
        /// Returns a <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </returns>
        public override String ToString()
        {
            return str.ToString();
        }

        /// <summary>
        /// Applies the specified font to the entire string.
        /// </summary>
        /// <param name="fontIndex">Index of the font to apply.</param>
        public void ApplyFont(short fontIndex)
        {
            ApplyFont(0, str.CharCount, fontIndex);
        }
    }
}