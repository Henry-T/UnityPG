/* ====================================================================
   Licensed to the Apache Software Foundation (ASF) under one or more
   contributor license agreements.  See the NOTICE file distributed with
   this work for additional information regarding copyright ownership.
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

namespace NPOI.HSSF.Record.PivotTable
{
    using System;
    using System.Text;
    using NPOI.HSSF.Record;
    using NPOI.Util;
    using NPOI.Util.IO;

    /**
     * SXVD - View Fields (0x00B1)<br/>
     * 
     * @author Patrick Cheng
     */
    public class ViewFieldsRecord : StandardRecord
    {
        public static short sid = 0x00B1;

        /** the value of the <tt>cchName</tt> field when the name is not present */
        private static int STRING_NOT_PRESENT_LEN = -1;

        private int sxaxis;
        private int cSub;
        private int grbitSub;
        private int cItm;

        private String name = null;

        /**
         * values for the {@link ViewFieldsRecord#sxaxis} field
         */
        private static class Axis
        {
            public static int NO_AXIS = 0;
            public static int ROW = 1;
            public static int COLUMN = 2;
            public static int PAGE = 4;
            public static int DATA = 8;
        }

        public ViewFieldsRecord(RecordInputStream in1)
        {
            sxaxis = in1.ReadShort();
            cSub = in1.ReadShort();
            grbitSub = in1.ReadShort();
            cItm = in1.ReadShort();

            int cchName = in1.ReadShort();
            if (cchName != STRING_NOT_PRESENT_LEN)
            {
                name = in1.ReadCompressedUnicode(cchName);
            }
        }


        public override void Serialize(LittleEndianOutput out1)
        {

            out1.WriteShort(sxaxis);
            out1.WriteShort(cSub);
            out1.WriteShort(grbitSub);
            out1.WriteShort(cItm);

            if (name != null)
            {
                StringUtil.WriteUnicodeString(out1, name);
            }
            else
            {
                out1.WriteShort(STRING_NOT_PRESENT_LEN);
            }
        }


        protected override int DataSize
        {
            get
            {

                int cchName = 0;
                if (name != null)
                {
                    cchName = name.Length;
                }
                return 2 + 2 + 2 + 2 + 2 + cchName;
            }
        }


        public override short Sid
        {
            get
            {
                return sid;
            }
        }


        public override string ToString()
        {
            StringBuilder buffer = new StringBuilder();
            buffer.Append("[SXVD]\n");
            buffer.Append("    .sxaxis    = ").Append(HexDump.ShortToHex(sxaxis)).Append('\n');
            buffer.Append("    .cSub      = ").Append(HexDump.ShortToHex(cSub)).Append('\n');
            buffer.Append("    .grbitSub  = ").Append(HexDump.ShortToHex(grbitSub)).Append('\n');
            buffer.Append("    .cItm      = ").Append(HexDump.ShortToHex(cItm)).Append('\n');
            buffer.Append("    .name      = ").Append(name).Append('\n');

            buffer.Append("[/SXVD]\n");
            return buffer.ToString();
        }
    }
}