using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;

namespace TechArray.ZXSpectrum.FileSupport
{

    #region All of the frames of a sequence / animation
    public class FrameSequence
    {

    }
    #endregion

    #region One entire frame
    public class Frame
    {
        private List<BlockBank> frame = new List<BlockBank>();
        public void AddBlockBank(BlockBank bb)
        {
            this.frame.Add(bb);
        }
        public List<BlockBank> GetFrame()
        {
            return this.frame;
        }
    }
    #endregion

    #region Data for an entire horizontal row of bit blocks
    public class BlockBank
    {
        private List<BinaryBlock> blockBank = new List<BinaryBlock>();
        public void AddBitBlock(BinaryBlock bb)
        {
            this.blockBank.Add(bb);
        }
        public List<BinaryBlock> GetBlockBank()
        {
            return this.blockBank;
        }
        public BinaryBlock GetBlock(byte position)
        {
            return this.blockBank[position];
        }

    }
    #endregion

    #region An 8x8 bit block
    public class BinaryBlock
    {
        private byte[] bits = { 0, 0, 0, 0, 0, 0, 0, 0 };
        public void SetByte(byte position, byte value)
        {
            this.bits[position] = value;
        }
        public byte GetByte(byte position)
        {
            return this.bits[position];
        }
        public byte[] GetBits(byte position)
        {
            int i = 0;
            byte value = this.bits[position];
            byte[] bitBin = { 128, 64, 32, 16, 8, 4, 2, 1 };
            byte[] block = { 0, 0, 0, 0, 0, 0, 0, 0 };
            foreach (byte v in bitBin)
            {
                if (value >= v)
                {
                    value -= v;
                    block[i] = 1;
                }
                i++;
            }
            return block;
        }
    }
    #endregion

    public class SEVSprite
    {
        const string FILE_ID = "Sev";
        private readonly Boolean valid = false;

        private readonly int width;
        private readonly int height;
        private readonly int nframes;

        private readonly List<Frame> frames = new List<Frame>();

        public SEVSprite(string filename)
        {
            byte[] sD;
            try
            {
                sD = File.ReadAllBytes(filename);
            }
            catch (FileNotFoundException e)
            {
                MessageBox.Show(e.Message);
                return;
            }
            // get the file id
            string id = getStringData(sD, 0, 3);
            this.valid = (id == FILE_ID);
            if (this.valid)
            {
                // if we are here the file is good
                int minor;
                int major;
                minor = (int)sD[4];
                major = (int)sD[5];
                int properties = (int)sD[7] * 256 + (int)sD[6];
                int nframes = (int)sD[9] * 256 + (int)sD[8];
                int sizex = (int)sD[11] * 256 + (int)sD[10];
                int sizey = (int)sD[13] * 256 + (int)sD[12];
                this.width = sizex;
                this.height = sizey;
                this.nframes = nframes;
                int bytesPerFrame = (sizex / 8) * (sizey / 8) * 9;
                // we are now ready to start getting the pixel data

                int pixelDataArea = 14;
                int pos = pixelDataArea;

                for (int frameData = 0; frameData < nframes + 1; frameData++)
                {
                    Frame thisFrame = new Frame();

                    for (int y = 0; y < (sizey / 8); y++)
                    {
                        BlockBank lineData = new BlockBank();

                        for (int x = 0; x < (sizex / 8); x++)
                        {
                            BinaryBlock block = new BinaryBlock();
                            for (byte i = 0; i < 8; i++)
                            {
                                block.SetByte(i, (byte)sD[pos]);
                                pos++;
                            }
                            pos++;
                            lineData.AddBitBlock(block);
                        }
                        thisFrame.AddBlockBank(lineData);
                    }
                    this.frames.Add(thisFrame);
                }

            }
        }

        public List<Frame> GetFrames()
        {
            return this.frames;
        }

        internal string getStringData(byte[] data, int starts, int length)
        {
            string str = String.Empty;
            for (int i = starts; i < starts + length; i++)
            {
                str += ((char)data[i]).ToString();
            }
            return str;
        }
    }

}
