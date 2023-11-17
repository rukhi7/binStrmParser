using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nvmParser
{
    public class StreamBitsFld : BaseBinaryFied
    {
        internal override BaseBinValue CreateObj(ReadContext readContext)
        {
            if (bitLen <= 32)
            {
                intFromBits res = new intFromBits(this, readContext.arr, readContext.bitPos);
                readContext.addLeafToValueTree(res, bitLen);
                valueObj = res;
                return res;
            }
            throw new NotImplementedException();
        }
        internal override bool initInTree()
        {
            parent.nowPos += bitLen;
            //            throw new NotImplementedException();
            return true;
        }
    }
}
