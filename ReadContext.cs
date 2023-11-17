using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nvmParser
{
    class ReadContext
    {
        int levelInc;
        internal int bitPos;
        internal byte[] arr = { 0xf1,
/*                        0xe1, 0x10, 0x01,//Dastm
            0x05, 0x15, 0x22, 0xfc, 
            0x05, 0x65, //0x22, 0xff,
*/
// Dastm example bytes on offset 0x3B1648
0x01, 0x00, 0x03, 0x01, 
    0x14, 0x9C, 0xa, 0x00, 0x40, 0x34, 0x46, 0x01, 0x04, 0x01, 0x05, 0x01,
    0x06, 0x01, 0x07, 0x01, 0x08, 0x01, 0x09, 0x00, 0x80, 0x41, 0xC4, 0x01, 0x0A, 0x01, 0x0B, 0x01,
    0x0C, 0x01, 0x0D, 0x02, 0xC0, 0xC0, 0x83, 0x01, 0x1A, 0x01, 0x1B, 0x01, 0x1C, 0x00, 0xD0, 0x4C,
    0x3C, 0xC0, 0x84, 0x01, 0x0E, 0x01, 0x0F, 0x81, 0x10, 0x81, 0x11, 0x01, 0x40, 0x41, 0xC4, 0x01,
    0x12, 0x01, 0x13, 0x01, 0x14, 0x01, 0x15, 0x01, 0x80, 0x41, 0xC2, 0x01, 0x16, 0x01, 0x17, 0x01,
    0xC0, 0x41, 0xC4, 0x01, 0x24, 0x01, 0x25, 0x01, 0x26, 0x01, 0x27, 0x02, 0x40, 0xC3, 0xC3, 0x02,

    0xF9, 0x02, 0xFA, 0x02, 0xFB, 0x02, 0x80, 0x41, 0xC2, 0x01, 0x36, 0x01, 0x37, 0x1A, 0x40, 0xC1,
    0x01, 0x03, 0xA8, 0x1A, 0x80, 0xC1, 0x01, 0x03, 0xA9, 0x2A, 0x40, 0xC0, 0x45, 0x03, 0x2A, 0x03,
    0x2B, 0x03, 0x2C, 0x03, 0x2D, 0x03, 0x2E, 0x4B, 0x00, 0x34, 0x45, 0x01, 0x2C, 0x01, 0x2D, 0x01,
    0x2E, 0x01, 0x2F, 0x01, 0x30, 0x4D, 0x84, 0x3C, 0x42, 0x00, 0x46, 0x00, 0x47, 0x4D, 0xC0, 0x34,
    0x42, 0x01, 0x18, 0x01, 0x19, 0x4E, 0xC0, 0x5E, 0xC4, 0x01, 0x1D, 0x01, 0x1E, 0x01, 0x1F, 0x01,
    0x20, 0x50, 0x04, 0x3C, 0x41, 0x00, 0xDC, 0x52, 0x80, 0x34, 0x41, 0x01, 0x22, 0x52, 0xC0, 0x34,
    0x41, 0x01, 0x23, 0x64, 0x04, 0x3F, 0xC1, 0x00, 0x08, 0x66, 0x84, 0x3C, 0x01, 0x00, 0x0A, 0x39,
    0x94, 0x44, 0x00, 0x3C, 0x01, 0x80, 0xE6, 0x39, 0xD4, 0x44, 0x04, 0x3C, 0x01, 0x80, 0xE7, 0x3A,
    0x14, 0x44, 0x08, 0x3C, 0x01, 0x80, 0xE8, 0x3A, 0x94, 0x44, 0x0C, 0x3C, 0x01, 0x80, 0xEA, 0x3A,
    0xD4, 0x44, 0x10, 0x3C, 0x01, 0x80, 0xEB, 0x3B, 0x14, 0x44, 0x14, 0x3C, 0x01, 0x80, 0xEC, 0x3B,

            0xa1, 0x71,0xa2,    0xa3, 0x71 ,0xa4,
            0xf2, 0x01,
            0x13, 0xf1, 0x23, 0x15,
            0xf1, 0x26,
            0xf1, 0x27,

            0xcc, 0xcc,0xcc, 0xcc,0xcc, 0xcc,0xcc, 0xcc,0xcc, 0xcc,0xcc, 0xcc,0xcc, 0xcc,0xcc, 0xcc
        };
        private const long NVM_START_READ_OFFSET = 58 * 256 * 256;
        MainWindow wnd;
        internal BaseBinaryFied InitParseContext(object rootElmnt, MainWindow wnd)
        {
            this.wnd = wnd;
/*        FileStream nvmFile = File.OpenRead(@"c:\tmp\REF28Bf005-Full_4M.BIN");
        nvmFile.Seek(NVM_START_READ_OFFSET, SeekOrigin.Begin);
        byte[] bts = new byte[256*256];
        if (nvmFile.Read(bts, 0, (int)bts.Length) < bts.Length)
        throw new Exception("Not enough data to read");
        arr = bts;*/
        ComplexField rt = (ComplexField)rootElmnt;
            roootParentBin = new parentBinValue(rt, arr, bitPos);
            curparent = roootParentBin;
            rt.valueObj = roootParentBin;
            rt.initParentFromStream(this);
            BaseBinaryFied res = rt.FirstChild();
            if (res != null)
            {
                res.CreateObj(this);
            }
            curLevel = 1;
            return res;
        }

        internal BaseBinaryFied getNextFieldWithParent(BaseBinaryFied obj, out int retInc)
        {
            BaseBinaryFied res = null;
            ComplexField parent = obj as ComplexField;
            levelInc = 0;
            bool flag = true;
            if (parent != null)
            {
                parent.initParentFromStream(this);
                res = (BaseBinaryFied)parent.FirstChild();

                if (res != null)
                {
                    flag = false;
                    levelInc++;
                    curLevel++;
                }
            }

            if(flag)
            {
                //&nvmArr[0x1648]
                uint? tval = obj.valueObj?.getValue() as uint?;
                parent = obj.parent;
                while (parent != null)
                {
                    res = (BaseBinaryFied)parent.NextChild();
                    if (res != null)
                    {
                        break;
                    }
                    else
                    {
                        levelInc--;
                        curLevel--;
                        obj = parent;
                        parent = obj.parent;
                    }
                }
            }
            //synchByLevel();
            
            if (res!=null)
            {
                curparent = (parentBinValue)res.parent.valueObj;
                res.CreateObj(this);
            }
            retInc = levelInc;
            return res;

        }
        internal parentBinValue roootParentBin;
        parentBinValue curparent;
        int limitPos;
        int curLevel;
        int limitedLevel;
        internal void setPosLimit(int val)
        {
            if (limitPos != 0)
                throw new NotImplementedException("nested NumDataBytes Flds are not supported");
            limitPos = val*8 + bitPos;
            limitedLevel = curLevel;
            SendMessageToUi($"limitPos:{limitPos/8} defined in {curparent.Descript} fld! ");
        }
        static internal string FieldWholeName(BaseBinValue res)
        {
            StringBuilder sb = new StringBuilder();
            var obj = res;
            while (obj != null)
            {
                sb.Insert(0, obj.Descript);
                sb.Insert(0, "/");
                obj = obj.parent;
            }
            return sb.ToString();
        }
        void checkLimit(BaseBinValue res)
        {
            int btN = bitPos / 8;
            if (arr.Length <= btN)
            {
                SendMessageToUi($"--Stream Parse Error: fld:{FieldWholeName(res)} end pos:{btN} out of input Array limit:{arr.Length}!");
                limitPos = 0;
            }
            if (limitPos != 0)
            {
                if (curLevel < limitedLevel)
                {
                    SendMessageToUi($"--Stream Parse Error: limit:{limitPos / 8} was not riched! Upper Level FLD:{res.Descript} ");
                    limitPos = 0;
                }
                else
                if (bitPos > limitPos)
                {

                    SendMessageToUi($"--Stream Parse Error: fld:{FieldWholeName(res)} end pos:{btN}:{bitPos % 8} out of limit:{limitPos/8}!");
                    limitPos = 0;
                }
                else if (bitPos == limitPos)
                {
                    SendMessageToUi($"limitPos:{limitPos/8} was met by {res.Descript} fld! ");
                    limitPos = 0;
                }
            }
            else if (limitedLevel > 0)
            {
                if (curLevel >= limitedLevel)
                {
                    SendMessageToUi($"Stream Parse warning: -- notCovered FLD:{res.Descript} ");
                }
                limitedLevel = 0;
            }
        }
        internal void addLeafToValueTree(BaseBinValue res, int bitLen)
        {
            wnd.Dispatcher.Invoke(new Action(() =>
            {
                curparent.Children.Add(res);
            }));

            res.parent = curparent;
            bitPos += bitLen;
            checkLimit(res);

        }
        string msgString;
        private void SendMessageToUi(string v)
        {
            msgString += v;
        }

        internal string getMessage()
        {
            string v = msgString;
            msgString = "";
            return v;
        }
        internal void addBranchToValueTree(parentBinValue res)
        {
            wnd.Dispatcher.Invoke(new Action(() =>
            {
                curparent.Children.Add(res);
            }));
            res.parent = curparent;
            curparent = res;
            checkLimit(res);
        }

        internal void PrintArraySize()
        {
            SendMessageToUi($"I will parse Array[{arr.Length}]");
        }
    }
}
