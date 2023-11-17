using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nvmParser
{
    class ValidBinaryFld:BaseBinaryFied
    {
        const int DataValidTrue = 0x01;
        PaddingBinaryFld padding;
        internal ValidBinaryFld(BaseBinaryFied creator)
        {
            Name = creator.Name;
            bitLen = creator.bitLen;
            parent = creator.parent;
            int indx = parent.Children.IndexOf(creator);
            parent.Children[indx] = this;
            FloatPaddingBinaryFld endPadd;// = parent.Children.Last() as FloatPaddingBinaryFld;
            endPadd = (FloatPaddingBinaryFld )parent.Children.SkipWhile(obj => !(obj is FloatPaddingBinaryFld)).FirstOrDefault();
            if (endPadd == null)
            {
                padding = new PaddingBinaryFld(parent);
                parent.Children.Add(padding);
            }
            else
            {
 //               endPadd.initByValidFld( parent, creator.paddAddSize);
                padding = endPadd;

                //               throw new NotImplementedException("PaddingBinaryFld exist before Vilid Fld processed!");
            }

        }
        internal override BaseBinValue CreateObj(ReadContext readContext)
        {
//            int startPos = readContext.bitPos;
            intFromBytes res = (intFromBytes)base.CreateObj(readContext);
            int val = (int)res.getValue();
            if(val != DataValidTrue)
            {
                parent.index = parent.Children.Count - 2;
//                padding.insteadOfData();
            }
            return res;
        }
    }

    class NumDataBytes : BaseBinaryFied 
    {
        internal NumDataBytes(BaseBinaryFied creator)
        {
            Name = creator.Name;
            bitLen = creator.bitLen;
            parent = creator.parent;
            int indx = parent.Children.IndexOf(creator);
            parent.Children[indx] = this;
        }
        internal override BaseBinValue CreateObj(ReadContext readContext)
        {
            //            int startPos = readContext.bitPos;
            intFromBytes res = (intFromBytes)base.CreateObj(readContext);
            int val = (int)res.getValue();
            readContext.setPosLimit(val);
            return res;
        }
    }
}
