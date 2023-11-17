using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nvmParser
{
    public class PaddingBinaryFld: BaseBinaryFied
    {
 //       int startPos;
        protected int areaSize;
        internal PaddingBinaryFld()
        {
        }
        internal PaddingBinaryFld(ComplexField pParent)
        {
            parent = pParent;
 //           startPos = parent.nowPos;
            Name = "Padding";
            areaSize = parent.nowPos;
            parent.sectorBitSizeConst = true;

        }
        internal override bool initInTree()
        {
            if (parent.sectorBitSizeConst)
            {
                areaSize = parent.nowPos;// - startPos;
                return true;
            }
            else
                return false;
        }

        internal override BaseBinValue CreateObj(ReadContext readContext)
        {
            int expectEnd = parent.startPos + areaSize;
            if (readContext.bitPos < expectEnd)
            { bitLen = expectEnd - readContext.bitPos; }
            else if (readContext.bitPos == expectEnd)
                return null;
            else throw new NotImplementedException("internal data size out of predefined buffer!");

            ByteArrayValue res = new ByteArrayValue(this, readContext.arr, readContext.bitPos);
            readContext.addLeafToValueTree(res, bitLen);
            valueObj = res;
            return res;
        }

    }
    public class FloatPaddingBinaryFld : PaddingBinaryFld
    {
        public long ByteSize { get; set; }

        internal override bool initInTree()
        {
            int addLen = parent.nowPos;
            if (areaSize == 0)
            {
                if ((addLen & 7) != 0)
                    throw new NotImplementedException($"padding addition {addLen} is in bits - only bytes allowed!");
                areaSize = (int)ByteSize * 8 + addLen;
                //                parent.Children.IndexOf(this);
                parent.Children.Add(this);
            }
            else
            {
                if (areaSize < addLen)
                {
                    throw new NotImplementedException($"padding size:{areaSize} doesn't cover embeded data size:{addLen} !");
                }
                parent.Children.Remove(this);
            }
            //ParseContext cntxt
            return true;
        }

    }
}
