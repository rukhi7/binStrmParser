using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nvmParser
{
    public class BaseBinValue
    {
        internal parentBinValue parent;
        protected BaseBinaryFied descrpt;
        protected byte[] nvmArr;
        internal int bitPos;
        public int getEndPos()
        {
            int endPos = 0;
            int indx = parent.Children.IndexOf(this);
            if (indx == parent.Children.Count - 1)
            {
                endPos = parent.getEndPos();
            }
            else
            {
                BaseBinValue next = parent.Children[indx + 1] as BaseBinValue;
                if (next != null)
                    endPos = (next.bitPos - 1) / 8;
            }
            if (endPos != 0 && bitPos / 8 >= endPos)
                endPos = bitPos / 8;
            return endPos;
        }

        public virtual  object getValue()
        //                internal override int setValue(byte[] nvmArr, int bitPos)
        {

            int BitLen = descrpt.bitLen;
            int shft = bitPos & 7;
            int btPtr = bitPos >> 3;
            int endShft = BitLen & 7;

            if ((shft | endShft) != 0)
            {
                throw new Exception("Bytes array doesn't aligned to byte bound!");
            }
            var newArray = nvmArr.Skip(btPtr).Take(BitLen >> 8).ToArray();

            return newArray;
        }
        public object fldVal { get => $"0x{getValue():X2}"; }
        public string BitPos { get => $"Pos:{bitPos/8}:{bitPos%8};"; }
        public virtual ObservableCollection<object> Children
        {
            get => null;
  //          set { }
        }
        public virtual string Descript
        {
            get { return descrpt.Name; }
        }
    }

    public class parentBinValue : BaseBinValue
    {
        protected ObservableCollection<object> chlds = new ObservableCollection<object>();
        public parentBinValue(BaseBinaryFied descrpt,
        byte[] nvmArr,
        int bitPos)
        {
            this.descrpt = descrpt;
            this.nvmArr = nvmArr;
            this.bitPos = bitPos;
        }
        public override ObservableCollection<object> Children
        {
            get => chlds;
//            set { chlds  }
        }
        public override object getValue()
        //                internal override int setValue(byte[] nvmArr, int bitPos)
        {
            return chlds.Count;
        }
        public string startPos { get {
                ComplexField cf = descrpt as ComplexField;
                if (cf != null)
                {
                    if (cf.parent is FieldsArr)
                    {
                        int indx = parent.chlds.IndexOf(this);
                        return $"{ indx}";
                    }

                    //                    return $" ofs:0x{ ((ComplexField)descrpt).startPos / 8:X4}";
                    return $" ofs:0x{ bitPos / 8:X4}";
                }
                else
                {
                    BitStructureValue str = this as BitStructureValue;
                    return $" { str.readVal:X8}";
                }

            } }
    }
    public class NamedByFirstParent: parentBinValue
    {
        public NamedByFirstParent(BaseBinaryFied descrpt,
                    byte[] nvmArr,
                    int bitPos):base(descrpt,
                    nvmArr,
                    bitPos)
        {
            this.descrpt = descrpt;
            this.nvmArr = nvmArr;
            this.bitPos = bitPos;
        }
        public override string Descript
        {
            get 
            {
                string res = "";
                int indx = ((NamedByFirst)descrpt).nameAdditionIndex;
                if (indx >= chlds.Count) return "-skp-";
                BaseBinValue fld = chlds[indx] as BaseBinValue;
                if (fld != null)
                {
                    int? val = (int?)fld.getValue();
                    if (val == null)
                        res = "XX";
                    else res = val.ToString();
                    res = fld.Descript + ": " + res;
                }
                return res;
            }
        }

    }

    class ByteArrayValue : BaseBinValue
    {
        public ByteArrayValue(BaseBinaryFied descrpt,
            byte[] nvmArr,
            int bitPos)
        {
            this.descrpt = descrpt;
            this.nvmArr = nvmArr;
            this.bitPos = bitPos;
        }

        public override object getValue()
            //                internal override int setValue(byte[] nvmArr, int bitPos)
        {
            int BitLen = descrpt.bitLen;
            int btPtr = bitPos >> 3;
 //           var newArray = nvmArr.Skip(btPtr).Take(BitLen >> 8).ToArray();
            return $"len={BitLen / 8}";
        }
    }

    class intFromBits: BaseBinValue
    {
        public intFromBits(BaseBinaryFied descrpt,
        byte[] nvmArr,
        int bitPos)
        { 
            this.descrpt = descrpt;
            this.nvmArr = nvmArr;
            this.bitPos = bitPos;
        }
        public override object getValue()
        //                internal override int setValue(byte[] nvmArr, int bitPos)
        {
            int BitLen = descrpt.bitLen;
            int resVal = 0;
            int shft = bitPos & 7;
            int btPtr = bitPos >> 3;

            if (shft != 0)
            {
                resVal = nvmArr[btPtr++];
                resVal = resVal & ((1 << (8 - shft)) - 1);
            }

            int endPtr = (bitPos + BitLen + 7) >> 3;
            while (endPtr > btPtr)
            {
                resVal <<= 8;
                resVal |= nvmArr[btPtr++];
            }
            shft = (bitPos + BitLen) & 7;
            if (shft != 0)
            {
                resVal >>= (8 - shft);
            }
            return resVal;
        }
    }
    class intFromBytes : BaseBinValue
    {
        public intFromBytes(BaseBinaryFied descrpt,
        byte[] nvmArr,
        int bitPos)
        {
            this.descrpt = descrpt;
            this.nvmArr = nvmArr;
            this.bitPos = bitPos;
        }
        public override object getValue()
        //                internal override int setValue(byte[] nvmArr, int bitPos)
        {
            int bytesCnt = descrpt.bitLen >> 3;
            int resVal;
            int btPtr = bitPos >> 3;

            int endPtr = btPtr + bytesCnt;
            if(endPtr-1 > nvmArr.Length)
                return null;
            resVal = (int)nvmArr[--endPtr];
            while (endPtr > btPtr)
            {
                resVal <<= 8;
                resVal |= (int)nvmArr[--endPtr];
            }
            return resVal;
        }
    }
}
