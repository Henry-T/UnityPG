
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
     * Title:        Style Record
     * Description:  Describes a builtin to the gui or user defined style
     * REFERENCE:  PG 390 Microsoft Excel 97 Developer's Kit (ISBN: 1-57231-498-2)
     * @author Andrew C. Oliver (acoliver at apache dot org)
     * @author aviks : string fixes for UserDefined Style
     * @version 2.0-pre
     */

    public class StyleRecord
       : Record
    {
        public const short sid = 0x293;
        public static short STYLE_USER_DEFINED = 0;
        public static short STYLE_BUILT_IN = 1;

        // shared by both user defined and builtin styles
        private short field_1_xf_index;   // TODO: bitfield candidate

        // only for built in styles
        private byte field_2_builtin_style;
        private byte field_3_outline_style_level;

        // only for user defined styles
        private short field_2_name_length; //OO doc says 16 bit Length, so we believe
        private byte field_3_string_options;
        private BitField fHighByte;
        private String field_4_name;

        public StyleRecord()
        {
        }

        /**
         * Constructs a Style record and Sets its fields appropriately.
         * @param in the RecordInputstream to Read the record from
         */

        public StyleRecord(RecordInputStream in1)
        {
            fHighByte = BitFieldFactory.GetInstance(0x01); //have to init here, since we are being called
            //from base, and class level init hasnt been done. 
            field_1_xf_index = in1.ReadShort();
            if (Type == STYLE_BUILT_IN)
            {
                field_2_builtin_style = (byte)in1.ReadByte();
                field_3_outline_style_level = (byte)in1.ReadByte();
            }
            else if (Type == STYLE_USER_DEFINED)
            {
                field_2_name_length = in1.ReadShort();

                // Some files from Crystal Reports lack
                //  the remaining fields, which Is naughty
                if (in1.Remaining > 0)
                {
                    field_3_string_options = (byte)in1.ReadByte();

                    byte[] str = in1.ReadRemainder();
                    if (fHighByte.IsSet(field_3_string_options))
                    {
                        field_4_name = StringUtil.GetFromUnicodeBE(str, 0, field_2_name_length);
                    }
                    else
                    {
                        field_4_name = StringUtil.GetFromCompressedUnicode(str, 0, field_2_name_length);
                    }
                }
            }

            // todo sanity Check exception to make sure we're one or the other
        }

        // bitfields for field 1

        // field 1

        /**
         * Get the entire index field (including the type) (see bit Getters that reference this method)
         *  @return bitmask
         */

        public short Index
        {
            get { return field_1_xf_index; }
            set { field_1_xf_index = value; }
        }

        // bitfields for field 1

        /**
         * Get the type of the style (builtin or user-defined)
         * @see #STYLE_USER_DEFINED
         * @see #STYLE_BUILT_IN
         * @return type of style (userdefined/builtin)
         * @see #Index
         */

        public short Type
        {
            get
            {
                return (short)((field_1_xf_index & 0x8000) >> 15);
            }
            set
            {
                field_1_xf_index = SetField(field_1_xf_index, value, 0x8000, 15);
            }
        }

        /**
         * Get the actual index of the style extended format record
         * @see #Index
         * @return index of the xf record
         */

        public short XFIndex
        {
            get { return (short)(field_1_xf_index & 0x1FFF); }
            set { field_1_xf_index = SetField(field_1_xf_index, value, 0x1FFF, 0); }
        }

        // end bitfields
        // only for user defined records

        /**
         * if this Is a user defined record Get the Length of the style name
         * @return Length of the style's name
         * @see #Name
         */

        public short NameLength
        {
            get { return field_2_name_length; }
            set { field_2_name_length = value; }
        }

        /**
         * Get the style's name
         * @return name of the style
         * @see #NameLength
         */

        public String Name
        {
            get { return field_4_name; }
            set { 
                field_4_name = value;
                //TODO Set name Length and string options
            }
        }

        // end user defined
        // only for buildin records

        /**
         * if this Is a builtin style Get the number of the built in style
         * @return  builtin style number (0-7)
         *
         */

        public byte Builtin
        {
            get
            {
                return field_2_builtin_style;
            }
            set { field_2_builtin_style = value; }
        }

        /**
         * Get the row or column level of the style (if builtin 1||2)
         */

        public byte OutlineStyleLevel
        {
            get
            {
                return field_3_outline_style_level;
            }
            set { field_3_outline_style_level = value; }
        }

        // end builtin records
        public override String ToString()
        {
            StringBuilder buffer = new StringBuilder();

            buffer.Append("[STYLE]\n");
            buffer.Append("    .xf_index_raw    = ")
                .Append(StringUtil.ToHexString(Index)).Append("\n");
            buffer.Append("        .type        = ")
                .Append(StringUtil.ToHexString(Type)).Append("\n");
            buffer.Append("        .xf_index    = ")
                .Append(StringUtil.ToHexString(XFIndex)).Append("\n");
            if (Type == STYLE_BUILT_IN)
            {
                buffer.Append("    .builtin_style   = ")
                    .Append(StringUtil.ToHexString(Builtin)).Append("\n");
                buffer.Append("    .outline_level   = ")
                    .Append(StringUtil.ToHexString(OutlineStyleLevel))
                    .Append("\n");
            }
            else if (Type== STYLE_USER_DEFINED)
            {
                buffer.Append("    .name_length     = ")
                    .Append(StringUtil.ToHexString(NameLength)).Append("\n");
                buffer.Append("    .name            = ").Append(Name)
                    .Append("\n");
            }
            buffer.Append("[/STYLE]\n");
            return buffer.ToString();
        }

        private short SetField(int fieldValue, int new_value, int mask,
                               int ShiftLeft)
        {
            return (short)((fieldValue & ~mask)
                              | ((new_value << ShiftLeft) & mask));
        }

        public override int Serialize(int offset, byte [] data)
        {
            LittleEndian.PutShort(data, 0 + offset, sid);
            if (Type == STYLE_BUILT_IN)
            {
                LittleEndian.PutShort(data, 2 + offset,
                                      ((short)0x04));   // 4 bytes (8 total)
            }
            else
            {
                LittleEndian.PutShort(data, 2 + offset,
                                      ((short)(RecordSize - 4)));
            }
            LittleEndian.PutShort(data, 4 + offset, Index);
            if (Type == STYLE_BUILT_IN)
            {
                data[6 + offset] = Builtin;
                data[7 + offset] = OutlineStyleLevel;
            }
            else
            {
                LittleEndian.PutShort(data, 6 + offset, NameLength);
                data[8 + offset] = this.field_3_string_options;
                StringUtil.PutCompressedUnicode(Name, data, 9 + offset);
            }
            return RecordSize;
        }

        public override int RecordSize
        {
            get
            {
                int retval;

                if (Type== STYLE_BUILT_IN)
                {
                    retval = 8;
                }
                else
                {
                    if (fHighByte.IsSet(field_3_string_options))
                    {
                        retval = 9 + 2 * NameLength;
                    }
                    else
                    {
                        retval = 9 + NameLength;
                    }
                }
                return retval;
            }
        }


        public override short Sid
        {
            get { return sid; }
        }
    }
}