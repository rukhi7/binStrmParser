using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Markup;

namespace nvmParser
{
    [ContentPropertyAttribute("Fields")]
    public class BitStructure : BaseBinaryFied
    {
        protected List<BitField> chlds = new List<BitField>();
        public List<BitField> Fields
        {
            get { return chlds; }
        }
        internal override bool initInTree()
        {
            if ((parent.nowPos & 7) != 0)
                throw new NotImplementedException($"int Fld place position:{parent.nowPos} error");
            parent.nowPos += bitLen;

            return true;
        }
        internal override BaseBinValue CreateObj(ReadContext readContext)
        {
            BitStructureValue res = new BitStructureValue(this, readContext.arr, readContext.bitPos);
            readContext.addLeafToValueTree(res, bitLen);
            valueObj = res;
            return res;
        }
    }
    public class BitField : BaseBinaryFied
    {
        public int StartBit { get; set; }

    }

    public class BitStructureValue:  parentBinValue
    {
        intFromBytes intVal;
        internal int readVal;
        internal BitStructureValue(BaseBinaryFied descrpt,
                    byte[] nvmArr,
                    int bitPos) : base(descrpt,
                    nvmArr,
                    bitPos)
        {
            intVal = new intFromBytes(descrpt, nvmArr, bitPos);
        }
        public override object getValue()
        //                internal override int setValue(byte[] nvmArr, int bitPos)
        {
            readVal = (int)intVal.getValue();
            if (chlds.Count > 0) return $"{readVal:X8}";
            BitStructure str = descrpt as BitStructure;
            int bPos = 0;
            foreach (BitField fld in str.Fields)
            {
                BitOfIntValue bf = new BitOfIntValue(fld, this, bPos);
                bPos += fld.bitLen;
                chlds.Add(bf);
            }
            return $"{readVal:X2}";
        }
        public override ObservableCollection<object> Children
        {
            get => chlds;
            //            set { chlds  }
        }
    }

    public class BitOfIntValue : BaseBinValue
    {
        internal BitOfIntValue(BaseBinaryFied descrpt,
            BitStructureValue prnt,
            int bitPos)
        {
            this.descrpt = descrpt;
            this.parent = prnt;
            this.bitPos = bitPos;
        }
        public override object getValue()
        //                internal override int setValue(byte[] nvmArr, int bitPos)
        {
            int BitLen = descrpt.bitLen;
            int val = ((BitStructureValue)parent).readVal;
            uint x = (uint)val;
            return (x  >> (bitPos)) & ((1 << BitLen)-1);
        }
 //       public object fldVal { get => $"{getValue():X2}"; }
    }

}
