using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nvmParser
{
    public class ByteArray : BaseBinaryFied
    {
        public long ElementsCount { get; set; }

        internal override BaseBinValue CreateObj(ReadContext readContext)
        {
            ByteArrayValue res = new ByteArrayValue(this, readContext.arr, readContext.bitPos);
            readContext.addLeafToValueTree(res, bitLen);
            valueObj = res;
            return res;
        }
        internal override bool initInTree()
        {
            bitLen = (int)ElementsCount * 8;
            parent.nowPos += bitLen;
            //            throw new NotImplementedException();
            return true;
        }
    }
    public class HexBinaryFied: BaseBinaryFied
    {
        BaseBinaryFied linkedName;
        public string SizeLink { get; set; }
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

        internal override BaseBinValue CreateObj(ReadContext readContext)
        {
            bitLen = (int)linkedName.valueObj.getValue() * 8 + 8;

            ByteArrayValue res = new ByteArrayValue(this, readContext.arr, readContext.bitPos);
            readContext.addLeafToValueTree(res, bitLen);
            valueObj = res;
            return res;
//            throw new NotImplementedException();
        }
    }
}
