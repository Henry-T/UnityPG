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

namespace NPOI.HSSF.Record.Formula
{

    using System;
    using System.IO;
    using System.Runtime.Serialization.Formatters.Binary;
    using System.Collections;
    using System.Collections.Generic;

    using NPOI.Util;
    using NPOI.HSSF.Record;

    using NPOI.SS.Util;
    using NPOI.Util.IO;
    
    /**
     * <tt>Ptg</tt> represents a syntactic token in a formula.  'PTG' is an acronym for 
     * '<b>p</b>arse <b>t</b>hin<b>g</b>'.  Originally, the name referred to the single 
     * byte identifier at the start of the token, but in POI, <tt>Ptg</tt> encapsulates
     * the whole formula token (initial byte + value data).
     * 
     * 
     * <tt>Ptg</tt>s are logically arranged in a tree representing the structure of the
     * Parsed formula.  However, in BIFF files <tt>Ptg</tt>s are written/Read in 
     * <em>Reverse-Polish Notation</em> order. The RPN ordering also simplifies formula
     * evaluation logic, so POI mostly accesses <tt>Ptg</tt>s in the same way.
     *
     * @author  andy
     * @author avik
     * @author Jason Height (jheight at chariot dot net dot au)
     */
    [Serializable]
    public abstract class Ptg:ICloneable
    {
        public static Ptg[] EMPTY_PTG_ARRAY = { };

        /**
         * Reads <tt>size</tt> bytes of the input stream, to Create an array of <tt>Ptg</tt>s.
         * Extra data (beyond <tt>size</tt>) may be Read if and <tt>ArrayPtg</tt>s are present.
         */
        public static Ptg[] ReadTokens(int size, LittleEndianInput in1)
        {
		    ArrayList temp = new ArrayList(4 + size / 2);
		    int pos = 0;
		    ArrayList arrayPtgs = null;
		    while (pos < size) {
			    Ptg ptg = Ptg.CreatePtg( in1 );
			    if (ptg is ArrayPtg) {
				    if (arrayPtgs == null) {
					    arrayPtgs = new ArrayList(5);
				    }
				    arrayPtgs.Add(ptg);
				    pos += ArrayPtg.PLAIN_TOKEN_SIZE;
			    } else {
				    pos += ptg.Size;
			    }
			    temp.Add( ptg );
		    }
		    if(pos != size) {
			    throw new Exception("Ptg array size mismatch");
		    }
		    if (arrayPtgs != null) {
			    for (int i=0;i<arrayPtgs.Count;i++) {
				    ArrayPtg p = (ArrayPtg)arrayPtgs[i];
				    p.ReadTokenValues(in1);
			    }
		    }
		    return ToPtgArray(temp);
        }
        public static bool DoesFormulaReferToDeletedCell(Ptg[] ptgs)
        {
            for (int i = 0; i < ptgs.Length; i++)
            {
                if (IsDeletedCellRef(ptgs[i]))
                {
                    return true;
                }
            }
            return false;
        }
        private static bool IsDeletedCellRef(Ptg ptg)
        {
            if (ptg == ErrPtg.REF_INVALID)
            {
                return true;
            }
            if (ptg is DeletedArea3DPtg)
            {
                return true;
            }
            if (ptg is DeletedRef3DPtg)
            {
                return true;
            }
            if (ptg is AreaErrPtg)
            {
                return true;
            }
            if (ptg is RefErrorPtg)
            {
                return true;
            }
            return false;
        }
        public static Ptg CreatePtg(LittleEndianInput in1)
        {
            byte id = (byte)in1.ReadByte();

            if (id < 0x20)
            {
                return CreateBasePtg(id, in1);
            }

            Ptg retval = CreateClassifiedPtg(id, in1);

            if (id > 0x60)
            {
                retval.PtgClass = CLASS_ARRAY;
            }
            else if (id > 0x40)
            {
                retval.PtgClass = CLASS_VALUE;
            }
            else
            {
                retval.PtgClass = CLASS_REF;
            }

            return retval;
        }

        /** Write this Ptg to a byte array*/
        public abstract void WriteBytes(byte[] array, int offset);

        private static Ptg CreateClassifiedPtg(byte id, LittleEndianInput in1)
        {

            int baseId = id & 0x1F | 0x20;

            switch (baseId)
            {
                case ArrayPtg.sid: return new ArrayPtg(in1);    // 0x20, 0x40, 0x60
                case FuncPtg.sid: return new FuncPtg(in1);     // 0x21, 0x41, 0x61
                case FuncVarPtg.sid: return new FuncVarPtg(in1);  // 0x22, 0x42, 0x62
                case NamePtg.sid: return new NamePtg(in1);     // 0x23, 0x43, 0x63
                case RefPtg.sid: return new RefPtg(in1);      // 0x24, 0x44, 0x64
                case AreaPtg.sid: return new AreaPtg(in1);     // 0x25, 0x45, 0x65
                case MemAreaPtg.sid: return new MemAreaPtg(in1);  // 0x26, 0x46, 0x66
                case MemErrPtg.sid: return new MemErrPtg(in1);   // 0x27, 0x47, 0x67
                case MemFuncPtg.sid: return new MemFuncPtg(in1);  // 0x29, 0x49, 0x69
                case RefErrorPtg.sid: return new RefErrorPtg(in1);// 0x2a, 0x4a, 0x6a
                case AreaErrPtg.sid: return new AreaErrPtg(in1);  // 0x2b, 0x4b, 0x6b
                case RefNPtg.sid: return new RefNPtg(in1);     // 0x2c, 0x4c, 0x6c
                case AreaNPtg.sid: return new AreaNPtg(in1);    // 0x2d, 0x4d, 0x6d

                case NameXPtg.sid: return new NameXPtg(in1);    // 0x39, 0x49, 0x79
                case Ref3DPtg.sid: return new Ref3DPtg(in1);   // 0x3a, 0x5a, 0x7a
                case Area3DPtg.sid: return new Area3DPtg(in1);   // 0x3b, 0x5b, 0x7b
                case DeletedRef3DPtg.sid: return new DeletedRef3DPtg(in1);   // 0x3c, 0x5c, 0x7c
                case DeletedArea3DPtg.sid: return new DeletedArea3DPtg(in1); // 0x3d, 0x5d, 0x7d
            }
            throw new InvalidOperationException(" Unknown Ptg in Formula: 0x" +
                       StringUtil.ToHexString(id) + " (" + (int)id + ")");
        }

        private static Ptg CreateBasePtg(byte id, LittleEndianInput in1)
        {
            switch (id)
            {
                case 0x00: return new UnknownPtg(); // TODO - not a real Ptg
                case ExpPtg.sid: 
                    return new ExpPtg(in1);          // 0x01
                case TblPtg.sid: return new TblPtg(in1);          // 0x02
                case AddPtg.sid: return AddPtg.instance;         // 0x03
                case SubtractPtg.sid: return SubtractPtg.instance;    // 0x04
                case MultiplyPtg.sid: return MultiplyPtg.instance;    // 0x05
                case DividePtg.sid: return DividePtg.instance;      // 0x06
                case PowerPtg.sid: return PowerPtg.instance;       // 0x07
                case ConcatPtg.sid: return ConcatPtg.instance;      // 0x08
                case LessThanPtg.sid: return LessThanPtg.instance;    // 0x09
                case LessEqualPtg.sid: return LessEqualPtg.instance;   // 0x0a
                case EqualPtg.sid: return EqualPtg.instance;       // 0x0b
                case GreaterEqualPtg.sid: return GreaterEqualPtg.instance;// 0x0c
                case GreaterThanPtg.sid: return GreaterThanPtg.instance; // 0x0d
                case NotEqualPtg.sid: return NotEqualPtg.instance;    // 0x0e
                case IntersectionPtg.sid: return IntersectionPtg.instance;// 0x0f
                case UnionPtg.sid: return UnionPtg.instance;       // 0x10
                case RangePtg.sid: return RangePtg.instance;       // 0x11
                case UnaryPlusPtg.sid: return UnaryPlusPtg.instance;   // 0x12
                case UnaryMinusPtg.sid: return UnaryMinusPtg.instance;  // 0x13
                case PercentPtg.sid: return PercentPtg.instance;     // 0x14
                case ParenthesisPtg.sid: return ParenthesisPtg.instance; // 0x15
                case MissingArgPtg.sid: return MissingArgPtg.instance;  // 0x16

                case StringPtg.sid: return new StringPtg(in1);       // 0x17
                case AttrPtg.sid:
                case 0x1a: return new AttrPtg(in1); // 0x19
                case ErrPtg.sid: return new ErrPtg(in1);          // 0x1c
                case BoolPtg.sid: return new BoolPtg(in1);         // 0x1d
                case IntPtg.sid: return new IntPtg(in1);          // 0x1e
                case NumberPtg.sid: return new NumberPtg(in1);       // 0x1f
            }
            throw new Exception("Unexpected base token id (" + id + ")");
        }
        /**
 * Used to calculate value that should be encoded at the start of the encoded Ptg token array;
 * @return the size of the encoded Ptg tokens not including any trailing array data.
 */
        public static int GetEncodedSizeWithoutArrayData(Ptg[] ptgs)
        {
            int result = 0;
            for (int i = 0; i < ptgs.Length; i++)
            {
                Ptg ptg = ptgs[i];
                if (ptg is ArrayPtg)
                {
                    result += ArrayPtg.PLAIN_TOKEN_SIZE;
                }
                else
                {
                    result += ptg.Size;
                }
            }
            return result;
        }
        /**
         * @return a distinct copy of this <tt>Ptg</tt> if the class is mutable, or the same instance
         * if the class is immutable.
         */
        public Ptg Copy()
        {
            // TODO - all base tokens are logically immutable, but AttrPtg needs some clean-up 
            if (this is ValueOperatorPtg)
            {
                return this;
            }
            if (this is ScalarConstantPtg)
            {
                return this;
            }
            return (Ptg)Clone();
        }
        public virtual object Clone()
        {
            using (MemoryStream stream = new MemoryStream())
            {

                BinaryFormatter formatter = new BinaryFormatter();
                formatter.Serialize(stream, this);
                stream.Position = 0;
                return formatter.Deserialize(stream);
            }

        }
        private static Ptg[] ToPtgArray(ArrayList l)
        {
            if (l.Count==0)
            {
                return EMPTY_PTG_ARRAY;
            }
            
            Ptg[] result = (Ptg[])l.ToArray(typeof(Ptg));
            return result;
        }
        // TODO - several duplicates of this code should be refactored here
        public static int GetEncodedSize(Ptg[] ptgs)
        {
            int result = 0;
            for (int i = 0; i < ptgs.Length; i++)
            {
                result += ptgs[i].Size;
            }
            return result;
        }
        /**
         * Writes the ptgs to the data buffer, starting at the specified offset.  
         *
         * <br/>
         * The 2 byte encode Length field is <b>not</b> written by this method.
         * @return number of bytes written
         */
        public static int SerializePtgs(Ptg[] ptgs, byte[] array, int offset)
        {
		    int size = ptgs.Length;

            LittleEndianByteArrayOutputStream out1 = new LittleEndianByteArrayOutputStream(array, offset);

		    ArrayList arrayPtgs = null;

		    for (int k = 0; k < size; k++) {
			    Ptg ptg = ptgs[k];

			    ptg.Write(out1);
			    if (ptg is ArrayPtg) {
				    if (arrayPtgs == null) {
					    arrayPtgs = new ArrayList(5);
				    }
				    arrayPtgs.Add(ptg);
				    
			    }
		    }
		    if (arrayPtgs != null) {
			    for (int i=0;i<arrayPtgs.Count;i++) {
				    ArrayPtg p = (ArrayPtg)arrayPtgs[i];
				    p.WriteTokenValueBytes(out1);
			    }
		    }
		    return out1.WriteIndex - offset;;
        }

        /**
         * @return the encoded Length of this Ptg, including the initial Ptg type identifier byte. 
         */
        public abstract int Size { get; }
        /**
 * @return <c>false</c> if this token is classified as 'reference', 'value', or 'array'
 */
        public abstract bool IsBaseToken { get; }
        /**
 * Debug / diagnostic method to get this token's 'operand class' type.
 * @return 'R' for 'reference', 'V' for 'value', 'A' for 'array' and '.' for base tokens
 */
        public char RVAType
        {
            get
            {
                if (IsBaseToken)
                {
                    return '.';
                }
                switch (ptgClass)
                {
                    case Ptg.CLASS_REF: return 'R';
                    case Ptg.CLASS_VALUE: return 'V';
                    case Ptg.CLASS_ARRAY: return 'A';
                }
                throw new InvalidOperationException("Unknown operand class (" + ptgClass + ")");
            }
        }

        /** Write this Ptg to a byte array*/
        public abstract void Write(LittleEndianOutput out1);

        /**
         * return a string representation of this token alone
         */
        public abstract String ToFormulaString();

        /** Overridden toString method to Ensure object hash is not printed.
         * This helps Get rid of gratuitous diffs when comparing two dumps
         * Subclasses may output more relevant information by overriding this method
         **/
        public override String ToString()
        {
            return this.GetType().ToString();
        }

        public const byte CLASS_REF = 0x00;
        public const byte CLASS_VALUE = 0x20;
        public const byte CLASS_ARRAY = 0x40;

        private byte ptgClass = CLASS_REF; //base ptg

        /**
         *  @return the 'operand class' (REF/VALUE/ARRAY) for this Ptg
         */
        public byte PtgClass
        {
            get { return ptgClass; }
            set
            {
                if (IsBaseToken)
                {
                    throw new Exception("SetClass should not be called on a base token");
                }
                ptgClass = value;
            }
        }

        public abstract byte DefaultOperandClass { get; }

        #region ICloneable Members

        object ICloneable.Clone()
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}