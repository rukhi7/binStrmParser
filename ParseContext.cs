using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nvmParser
{
    class ParseContext
    {
        public bool InitParseContext(object obj)
        {
            BaseBinaryFied rootElmnt = obj as BaseBinaryFied;
            if (rootElmnt != null && rootElmnt.Children != null)
            {
                return true;
            }
            return false;
        }

        internal BaseBinaryFied getNextFieldWithParent(BaseBinaryFied obj, out int levelInc)
        {
            BaseBinaryFied res = null;
            levelInc = 0;
            if (obj.Children != null)
            {
                levelInc ++;
                res = (BaseBinaryFied)obj.Children[0];
                res.parent = (ComplexField)obj;
                return res;
            }
            ComplexField parent = obj.parent;
            while (parent != null)
            {
                int indx = ++parent.index;
                if(indx < parent.Children.Count)
                {
                    res = (BaseBinaryFied)parent.Children[indx];
                    res.parent = parent;
                    return res;
                }
                else 
                {
                    parent.BranchNodeClose();
                    levelInc --;
                    obj = parent;
                    parent = obj.parent;
                }
            }
            return null;
        }
        

    }
}
