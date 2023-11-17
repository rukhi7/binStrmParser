using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design.Serialization;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Markup;

namespace nvmParser
{
    public class BaseBinaryFied //: DependencyObject 
    {
        internal BaseBinValue valueObj;
        internal ComplexField parent;

        public virtual string Name { get; set; }
        public int bitLen { get; set; }
        public string FldLen 
        { 
            get
            {
                if((bitLen & 7) == 0)
                {
                    return $":{bitLen>>3}";
                }
                else
                    return $" Bits:{bitLen}";
            }
        }
        public virtual List<object> Children
        {
            get { return null; }
            set { }
        }

        internal virtual BaseBinValue CreateObj(ReadContext readContext)
        {
            if (bitLen <= 32 && (bitLen& 7) == 0)
            {
                intFromBytes res = new intFromBytes(this, readContext.arr, readContext.bitPos);
                readContext.addLeafToValueTree(res, bitLen);
                valueObj = res;
                return res;
            }
            throw new NotImplementedException($"bad bitLen:{bitLen} for intFromBytes");
        }

        internal virtual bool initInTree()
        {
            if ((parent.nowPos & 7) != 0)
                throw new NotImplementedException($"int Fld place position:{parent.nowPos} error");
            parent.nowPos += bitLen;
            //            throw new NotImplementedException();
            if (Name == "DataValid")
            {
                if (bitLen == 16)
                    new ValidBinaryFld(this);
                else
                    return false;
            }
            if (Name == "NumDataBytes")
                new NumDataBytes(this);


            return true;
        }

        //to do
        //void initFieldMethod(Context obj);
        static internal string FieldWholeName(BaseBinaryFied res)
        {
            StringBuilder sb = new StringBuilder();
            var obj = res;
            while (obj != null)
            {
                sb.Insert(0, obj.Name);
                sb.Insert(0, "/");
                obj = obj.parent;
            }
            return sb.ToString();
        }
    }

    public class FieldsArr : ComplexField
    {
        public int ElementsCount { get; set; }
        internal override bool initInTree()
        {
            //            throw new NotImplementedException();
            if (ElementsCount == 0)
            {
                if (ElementSize != 0)
                    ElementsCount = AllElementsSize / ElementSize;
                if (ElementsCount == 0)
                    return false;
            }
            nowPos = 0;// parent.nowPos;
            return true;
        }
        internal override BaseBinaryFied FirstChild()
        {
            if (ElementsCount == 0)
                return null;
            return internalFirstChild();
        }

        internal override BaseBinaryFied NextChild()
        {
            ++index;
            if (index < ElementsCount)
                return (BaseBinaryFied)chlds[0];
            return null;
        }
        internal override bool BranchNodeClose()
        {
            if ((nowPos & 7) != 0)
                throw new NotImplementedException($"Complrex Fld end bit size:{nowPos} error");
            if (sectorBitSizeConst == false)
                throw new NotImplementedException($"FieldsArr content size is not constant error");
            if (parent != null)
            {
                parent.sectorBitSizeConst &= sectorBitSizeConst;
                parent.nowPos += nowPos * ElementsCount;
            }
            bitLen = nowPos;
            return true;
        }
        public int ElementSize { set; get; }
        public int AllElementsSize { set; get; }
    }

}
