
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
     * Title:        Header Record
     * Description:  Specifies a header for a sheet
     * REFERENCE:  PG 321 Microsoft Excel 97 Developer's Kit (ISBN: 1-57231-498-2)
     * @author Andrew C. Oliver (acoliver at apache dot org)
     * @author Shawn Laubach (slaubach at apache dot org) Modified 3/14/02
     * @author Jason Height (jheight at chariot dot net dot au)
     * @version 2.0-pre
     */

    public class HeaderRecord
       : Record
    {
        public const short sid = 0x14;
        private byte field_1_header_len;
        private byte field_2_reserved;
        private byte field_3_unicode_flag;
        private String field_4_header;

        public HeaderRecord()
        {
        }

        /**
         * Constructs an Header record and Sets its fields appropriately.
         * @param in the RecordInputstream to Read the record from
         */

        public HeaderRecord(RecordInputStream in1)
        {
            if (in1.Remaining > 0)
            {
                field_1_header_len = (byte)in1.ReadByte();
                /** These two fields are a bit odd. They are not documented*/
                field_2_reserved = (byte)in1.ReadByte();
                field_3_unicode_flag = (byte)in1.ReadByte();						// Unicode

                if (IsMultibyte)
                {
                    field_4_header = in1.ReadUnicodeLEString(LittleEndian.UByteToInt(field_1_header_len));
                }
                else
                {
                    field_4_header = in1.ReadCompressedUnicode(LittleEndian.UByteToInt(field_1_header_len));
                }
            }
        }

        /**
         * see the Unicode flag
         *
         * @return bool flag
         *  true:footer string has at least one multibyte Char
         */
        public bool IsMultibyte
        {
            get { return ((field_3_unicode_flag & 0xFF) == 1); }
        }



        /**
         * Get the Length of the header string
         *
         * @return Length of the header string
         * @see #Header
         */

        public byte HeaderLength
        {
            get { return (byte)(0xFF & field_1_header_len); } // [Tony Qu]  fixed the type from short to byte
            set { field_1_header_len = value; }
        }

        /**
         * Get the header string
         *
         * @return header string to Display
         * @see #HeaderLength
         */

        public String Header
        {
            get { return field_4_header; }
            set
            {
                field_4_header = value;
                field_3_unicode_flag =
                    (byte)(StringUtil.HasMultibyte(field_4_header) ? 1 : 0);
                // Check it'll fit into the space in the record
                if (field_4_header == null) return;
                if (field_3_unicode_flag == 1)
                {
                    if (field_4_header.Length > 127)
                    {
                        throw new ArgumentException("Header string too long (limit is 127 for unicode strings)");
                    }
                }
                else
                {
                    if (field_4_header.Length > 255)
                    {
                        throw new ArgumentException("Header string too long (limit is 255 for non-unicode strings)");
                    }
                }
            }
        }

        public override String ToString()
        {
            StringBuilder buffer = new StringBuilder();

            buffer.Append("[HEADER]\n");
            buffer.Append("    .Length         = ").Append(HeaderLength)
                .Append("\n");
            buffer.Append("    .header         = ").Append(Header)
                .Append("\n");
            buffer.Append("[/HEADER]\n");
            return buffer.ToString();
        }

        public override int Serialize(int offset, byte [] data)
        {
            int len = 4;

            if (HeaderLength != 0)
            {
                len += 3; // [Shawn] Fixed for two null bytes in the Length
            }
            short bytelen = (short)(IsMultibyte ?
                HeaderLength * 2 : HeaderLength);
            LittleEndian.PutShort(data, 0 + offset, sid);
            LittleEndian.PutShort(data, 2 + offset,
                                  (short)((len - 4) + bytelen));

            if (HeaderLength > 0)
            {
                data[4 + offset] = (byte)HeaderLength;
                data[6 + offset] = field_3_unicode_flag;
                if (IsMultibyte)
                {
                    StringUtil.PutUnicodeLE(Header, data, 7 + offset);
                }
                else
                {
                    StringUtil.PutCompressedUnicode(Header, data, 7 + offset); // [Shawn] Place the string in the correct offset
                }
            }
            return RecordSize;
        }

        public override int RecordSize
        {
            get
            {
                int retval = 4;

                if (HeaderLength != 0)
                {
                    retval += 3; // [Shawn] Fixed for two null bytes in the Length
                }
                return (IsMultibyte ?
                     (retval + HeaderLength * 2) : (retval + HeaderLength));
            }
        }

        public override short Sid
        {
            get { return sid; }
        }

        public override Object Clone()
        {
            HeaderRecord rec = new HeaderRecord();
            rec.field_1_header_len = field_1_header_len;
            rec.field_2_reserved = field_2_reserved;
            rec.field_3_unicode_flag = field_3_unicode_flag;
            rec.field_4_header = field_4_header;
            return rec;
        }
    }
}