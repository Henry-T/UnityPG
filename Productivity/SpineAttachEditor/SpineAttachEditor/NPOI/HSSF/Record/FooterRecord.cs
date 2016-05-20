
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


namespace NPOI.HSSF.Record
{
    using System;
    using System.Text;
    using NPOI.Util;

    /**
     * Title:        Footer Record 
     * Description:  Specifies the footer for a sheet
     * REFERENCE:  PG 317 Microsoft Excel 97 Developer's Kit (ISBN: 1-57231-498-2)
     * @author Andrew C. Oliver (acoliver at apache dot org)
     * @author Shawn Laubach (slaubach at apache dot org) Modified 3/14/02
     * @author Jason Height (jheight at chariot dot net dot au)
     * @version 2.0-pre
     */

    public class FooterRecord
       : Record
    {
        public const short sid = 0x15;
        private byte field_1_footer_len;
        private byte field_2_reserved;
        private byte field_3_unicode_flag;
        private String field_4_footer;

        public FooterRecord()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FooterRecord"/> class.
        /// </summary>
        /// <param name="in1">the RecordInputstream to Read the record from</param>
        public FooterRecord(RecordInputStream in1)
        {
            if (in1.Remaining > 0)
            {
                field_1_footer_len = (byte)in1.ReadByte();
                /** These two fields are a bit odd. They are not documented*/
                field_2_reserved = (byte)in1.ReadByte();
                field_3_unicode_flag = (byte)in1.ReadByte();						// Unicode

                if (IsMultibyte)
                {
                    field_4_footer = in1.ReadUnicodeLEString(LittleEndian.UByteToInt(field_1_footer_len));
                }
                else
                {
                    field_4_footer = in1.ReadCompressedUnicode(LittleEndian.UByteToInt(field_1_footer_len));
                }
            }
        }

        /// <summary>
        /// the Unicode flag
        /// </summary>
        /// <value>
        /// 	<c>true</c> if footer string has at least one multibyte Char; otherwise, <c>false</c>.
        /// </value> 
        public bool IsMultibyte
        {
            get { return ((field_3_unicode_flag & 0xFF) == 1); }
        }




        /// <summary>
        /// Gets or sets the length of the footer string.
        /// </summary>
        /// <value>The length of the footer string.</value>
        /// <see cref="Footer"></see>
        public short FooterLength
        {
            get { return (short)(0xFF & field_1_footer_len); } // [Shawn] Fixed needing Unsigned byte
            set { field_1_footer_len = (byte)value; }
        }

        /// <summary>
        /// Gets or sets the footer string.
        /// </summary>
        /// <value>The footer string to Display.</value>
        /// <see cref="FooterLength"/>
        public String Footer
        {
            get { return field_4_footer; }
            set
            {
                field_4_footer = value;
                field_3_unicode_flag =
                    (byte)(StringUtil.HasMultibyte(field_4_footer) ? 1 : 0);
                // Check it'll fit into the space in the record

                if (field_4_footer == null) return;
                if (field_3_unicode_flag == 1)
                {
                    if (field_4_footer.Length > 127)
                    {
                        throw new ArgumentException("Footer string too long (limit is 127 for unicode strings)");
                    }
                }
                else
                {
                    if (field_4_footer.Length > 255)
                    {
                        throw new ArgumentException("Footer string too long (limit is 255 for non-unicode strings)");
                    }
                }
            }
        }

        /// <summary>
        /// Returns a <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </returns>
        public override String ToString()
        {
            StringBuilder buffer = new StringBuilder();

            buffer.Append("[FOOTER]\n");
            buffer.Append("    .footerlen      = ")
                .Append(StringUtil.ToHexString(FooterLength)).Append("\n");
            buffer.Append("    .footer         = ").Append(Footer)
                .Append("\n");
            buffer.Append("[/FOOTER]\n");
            return buffer.ToString();
        }

        /// <summary>
        /// Serializes the specified off set.
        /// </summary>
        /// <param name="offset">The off set.</param>
        /// <param name="data">The data.</param>
        /// <returns></returns>
        public override int Serialize(int offset, byte [] data)
        {
            int len = 4;

            if (FooterLength > 0)
            {
                len += 3; // [Shawn] Fixed for two null bytes in the Length
            }
            short bytelen = (short)(IsMultibyte ?
                FooterLength * 2 : FooterLength);
            LittleEndian.PutShort(data, 0 + offset, sid);
            LittleEndian.PutShort(data, 2 + offset,
                                  (short)((len - 4) + bytelen));
            if (FooterLength > 0)
            {
                data[4 + offset] = (byte)FooterLength;
                data[6 + offset] = field_3_unicode_flag;
                if (IsMultibyte)
                {
                    StringUtil.PutUnicodeLE(Footer, data, 7 + offset);
                }
                else
                {
                    StringUtil.PutCompressedUnicode(Footer, data, 7 + offset); // [Shawn] Place the string in the correct offset
                }
            }
            return RecordSize;
        }

        /// <summary>
        /// gives the current Serialized size of the record. Should include the sid and recLength (4 bytes).
        /// </summary>
        /// <value></value>

        public override int RecordSize
        {
            get
            {
                int retval = 4;

                if (FooterLength > 0)
                {
                    retval += 3; // [Shawn] Fixed for two null bytes in the Length
                }
                return (IsMultibyte ?
                    (retval + FooterLength * 2) : (retval + FooterLength));
            }
        }

        /// <summary>
        /// </summary>
        /// <value></value>
        /// return the non static version of the id for this record.
        public override short Sid
        {
            get { return sid; }
        }

        public override Object Clone()
        {
            FooterRecord rec = new FooterRecord();
            rec.field_1_footer_len = field_1_footer_len;
            rec.field_2_reserved = field_2_reserved;
            rec.field_3_unicode_flag = field_3_unicode_flag;
            rec.field_4_footer = field_4_footer;
            return rec;
        }
    }
}