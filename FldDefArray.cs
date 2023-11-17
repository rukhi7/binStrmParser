using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nvmParser
{
    public class NamedByFirst : ComplexField
    {
        public int nameAdditionIndex { get; set; }
        public override string Name
        {
            get
            {
                string res = "";
                BaseBinaryFied fld = chlds[nameAdditionIndex] as BaseBinaryFied;
                if (fld != null)
                {
                    int? val = (int?)fld.valueObj?.getValue();
                    if (val == null)
                        res = "XX";
                    else res = val.ToString();
                    res = fld.Name + ": " + res;
                }
                return res;
            }
            set { }
        }
        internal override BaseBinValue CreateObj(ReadContext readContext)
        {
            {
                parentBinValue res = new NamedByFirstParent(this, readContext.arr, readContext.bitPos);
                readContext.addBranchToValueTree(res);
//                readContext.bitPos += bitLen;
                valueObj = res;
                return res;
            }

        }
    }
    public class FldDefArray: ComplexField
    {
        BaseBinaryFied linkedName;
        int arrLen;
        public FldDefArray()
        {

        }
        internal override bool initInTree()
        {
            ComplexField tmpParent = parent;
            tmpParent.sectorBitSizeConst = false;
            while (tmpParent != null)
            {
                foreach (BaseBinaryFied valObj in tmpParent.Children)
                {
                    if (valObj.Name == SizeLink)
                    {
                        linkedName = valObj;
                        return true;
                    }
                }
                tmpParent = tmpParent.parent;
            }
            return false;
        }
        public int ZeroBase { get; set; }
        internal override BaseBinaryFied FirstChild()
        {
            arrLen = (int)linkedName.valueObj.getValue() + ZeroBase;
            if (arrLen == 0) return null;
            return internalFirstChild();
        }

        internal override BaseBinaryFied NextChild()
        {
            ++index;
            if (index < arrLen)
                return (BaseBinaryFied)chlds[0];
            return null;
        }
        public string SizeLink { get; set; }

        internal override BaseBinValue CreateObj(ReadContext readContext)
        {
            arrLen = (int)linkedName.valueObj.getValue() + ZeroBase;
            if (arrLen > 0)
            {
                parentBinValue res = new parentBinValue(this, readContext.arr, readContext.bitPos);
                readContext.addBranchToValueTree(res);
//                readContext.bitPos += bitLen;
                valueObj = res;
                return res;
            }
            return null;
        }
    }
}
