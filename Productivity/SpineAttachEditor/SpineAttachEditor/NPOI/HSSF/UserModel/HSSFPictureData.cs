/* ====================================================================
   Copyright 2002-2004   Apache Software Foundation

   Licensed Under the Apache License, Version 2.0 (the "License");
   you may not use this file except in compliance with the License.
   You may obtain a copy of the License at

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
    using NPOI.DDF;

    /// <summary>
    /// Represents binary data stored in the file.  Eg. A GIF, JPEG etc...
    /// @author Daniel Noll
    /// </summary>
    public class HSSFPictureData
    {
        // MSOBI constants for various formats.
        public const short MSOBI_WMF = 0x2160;
        public const short MSOBI_EMF = 0x3D40;
        public const short MSOBI_PICT = 0x5420;
        public const short MSOBI_PNG = 0x6E00;
        public const short MSOBI_JPEG = 0x46A0;
        public const short MSOBI_DIB = 0x7A80;
        // Mask of the bits in the options used to store the image format.
        public static short FORMAT_MASK = unchecked((short)0xFFF0);

        /**
         * Underlying escher blip record containing the bitmap data.
         */
        private EscherBlipRecord blip;

        /// <summary>
        /// Constructs a picture object.
        /// </summary>
        /// <param name="blip">the underlying blip record containing the bitmap data.</param>
        public HSSFPictureData(EscherBlipRecord blip)
        {
            this.blip = blip;
        }

        /// <summary>
        /// Gets the picture data.
        /// </summary>
        /// <value>the picture data.</value>
        public byte[] Data
        {
            get { return blip.PictureData; }
        }
        /// <summary>
        /// gets format of the picture.
        /// </summary>
        /// <value>The format.</value>
        public int Format
        {
            get
            {
                return blip.RecordId - unchecked((short)0xF018);
            }
        }
        /// <summary>
        /// Suggests a file extension for this image.
        /// </summary>
        /// <returns>the file extension.</returns>
        public String SuggestFileExtension()
        {
            switch (blip.RecordId)
            {
                case EscherMetafileBlip.RECORD_ID_WMF:
                    return "wmf";
                case EscherMetafileBlip.RECORD_ID_EMF:
                    return "emf";
                case EscherMetafileBlip.RECORD_ID_PICT:
                    return "pict";
                case EscherBitmapBlip.RECORD_ID_PNG:
                    return "png";
                case EscherBitmapBlip.RECORD_ID_JPEG:
                    return "jpeg";
                case EscherBitmapBlip.RECORD_ID_DIB:
                    return "dib";
                default:
                    return "";
            }
        }
    }
}
