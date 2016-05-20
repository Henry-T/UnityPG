
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
    using NPOI.Util.IO;

    /**
     * Title:        Write Access Record
     * Description:  Stores the username of that who owns the spReadsheet generator
     *               (on Unix the user's login, on Windoze its the name you typed when
     *                you installed the thing)
     * REFERENCE:  PG 424 Microsoft Excel 97 Developer's Kit (ISBN: 1-57231-498-2)
     * @author Andrew C. Oliver (acoliver at apache dot org)
     * @version 2.0-pre
     */

    public class WriteAccessRecord : StandardRecord
    {
        public const short sid = 0x5c;
        private String field_1_username;
        
	    private const byte PAD_CHAR = (byte) ' ';
	    private const int DATA_SIZE = 112;
        	/** this record is always padded to a constant length */
	    private static byte[] PADDING = new byte[DATA_SIZE];

        static WriteAccessRecord()
        {
            Arrays.Fill(PADDING, PAD_CHAR);
        }

        public WriteAccessRecord()
        {
        }

        /**
         * Constructs a WriteAccess record and Sets its fields appropriately.
         * @param in the RecordInputstream to Read the record from
         */

        public WriteAccessRecord(RecordInputStream in1)
        {
            byte[] data = in1.ReadRemainder();
            //The string Is always 112 Chars (pAdded with spaces), therefore
            //this record can not be continued.

            //What a wierd record, it Is not really a Unicode string because the
            //header doesnt provide a correct size indication.???
            //But the header Is present, so we need to skip over it.
            //Odd, Odd, Odd ;-)
            field_1_username = StringUtil.GetFromCompressedUnicode(data, 3, data.Length - 3);

        }


        /**
         * Get the username for the user that Created the report.  HSSF uses the logged in user.  On
         * natively Created M$ Excel sheet this would be the name you typed in when you installed it
         * in most cases.
         * @return username of the user who  Is logged in (probably "tomcat" or "apache")
         */

        public String Username
        {
            get
            {
                return field_1_username;
            }
            set { field_1_username = value; }
        }

        public override String ToString()
        {
            StringBuilder buffer = new StringBuilder();

            buffer.Append("[WriteACCESS]\n");
            buffer.Append("    .name            = ")
                .Append(field_1_username.ToString()).Append("\n");
            buffer.Append("[/WriteACCESS]\n");
            return buffer.ToString();
        }

        public override void Serialize(LittleEndianOutput out1)
        {
            String username = Username;
            bool is16bit = StringUtil.HasMultibyte(username);

            out1.WriteShort(username.Length);
            out1.WriteByte(is16bit ? 0x01 : 0x00);
            if (is16bit)
            {
                StringUtil.PutUnicodeLE(username, out1);
            }
            else
            {
                StringUtil.PutCompressedUnicode(username, out1);
            }
            int encodedByteCount = 3 + username.Length * (is16bit ? 2 : 1);
            int paddingSize = DATA_SIZE - encodedByteCount;
            out1.Write(PADDING, 0, paddingSize);
        }

        protected override int DataSize
        {
            get
            {
                return DATA_SIZE;
            }
        }

        public override short Sid
        {
            get { return sid; }
        }
    }
}