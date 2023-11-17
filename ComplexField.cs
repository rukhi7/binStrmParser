using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Markup;

namespace nvmParser
{
    [ContentPropertyAttribute("Children")]
    public class ComplexField : BaseBinaryFied
    {
        internal bool sectorBitSizeConst = true;
        internal int nowPos;
        internal int startPos;
        public int index;
        protected List<object> chlds = new List<object>();

        public override List<object> Children
        {
            get { return chlds; }
            set { }
        }
        internal override BaseBinValue CreateObj(ReadContext readContext)
        {
            parentBinValue res = new parentBinValue(this, readContext.arr, readContext.bitPos);
            readContext.addBranchToValueTree(res);
            //           readContext.bitPos += bitLen;//ruhi bitLen ?= 0!!! 
            valueObj = res;
            return res;
        }

        internal void initParentFromStream(ReadContext rc)
        {
            startPos = rc.bitPos;
        }
        protected BaseBinaryFied internalFirstChild()
        {
            index = 0;
            return (BaseBinaryFied)chlds[0];
        }
        internal virtual BaseBinaryFied FirstChild()
        {
            return internalFirstChild();
        }

        internal virtual BaseBinaryFied NextChild()
        {
            ++index;
            if (index < chlds.Count)
                return (BaseBinaryFied)chlds[index];
            return null;
        }
        internal override bool initInTree()
        {
            //            throw new NotImplementedException();

            nowPos = 0;// parent.nowPos;
            return true;
        }
        internal virtual bool BranchNodeClose()
        {
            if ((nowPos & 7) != 0)
            {
                //When there is mistake in bit-Fields size definitions
                //such that any complex field doesnt and up on byte boundary:
                throw new NotImplementedException($"Complex Fld {BaseBinaryFied.FieldWholeName(this)} end bit size:{nowPos} error. It must be multiple of 8!!!");
            }
            if (parent != null)
            {
                parent.sectorBitSizeConst &= sectorBitSizeConst;
                parent.nowPos += nowPos;
            }
            bitLen = nowPos;
            return true;
        }
    }

    public class DescriptionRoot : ComplexField
    {
        public string LoadedFile
        {
            get;
            set;
        }

    }
}
