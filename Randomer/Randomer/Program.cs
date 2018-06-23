using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Randomer
{
    class Program
    {
        static void Main(string[] args)
        {
            /*
            Bitmap myimg = new Bitmap("hotmap.png");
            Kuwahara(myimg, 9);
            myimg.Save("kwhm.png");
            */
            //AverageCounter("output/out", ".png", 10000, 1000, 1000).Save("hotmap.png");
            SomeRandomSamples("srs.wav");
            //SoundDrawer("srs.wav");
            Console.ReadKey();
        }
        static void Kuwahara(Bitmap image, int filter_size)
        {
            BitmapData dt = image.LockBits(new Rectangle(0, 0, image.Width, image.Height), ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);
            int ln = dt.Stride * dt.Height;
            int bpp = dt.Stride / dt.Width;
            byte[] data = new byte[ln];
            byte[] data2 = new byte[ln];
            Marshal.Copy(dt.Scan0, data, 0, ln);
            Parallel.For(0, dt.Height, (y) =>
            {
                for (int x = 0; x < dt.Stride; x += bpp)
                {
                    int maxdisp = 0, mindisp = 10000, divval = 0; int[] avgclrs = new int[bpp];
                    #region top_left
                    {
                        int[] __avg = new int[bpp]; int total = 0, minsum = 10000, maxsum = 0;
                        for (int dy = 0; dy >= -filter_size; dy--)
                        {
                            if (y + dy < 0) break;
                            for (int idx = 0; idx >= -filter_size; idx--)
                            {
                                int dx = idx * bpp;
                                if (x + dx < 0) break;
                                int pos = (y + dy) * dt.Stride + (x + dx);
                                int sum = 0;
                                for (int i = 0; i < bpp; i++)
                                {
                                    __avg[i] += data[pos + i];
                                    sum += data[pos + i];
                                }
                                if (minsum > sum) minsum = sum;
                                if (maxsum < sum) maxsum = sum;
                                total++;
                            }
                        }
                        int disp = maxsum - minsum;
                        if (disp < mindisp)
                        {
                            mindisp = disp;
                            avgclrs = __avg;
                            divval = total;
                        }
                    }
                    #endregion
                    #region top_right
                    {
                        int[] __avg = new int[bpp]; int total = 0, minsum = 10000, maxsum = 0;
                        for (int dy = 0; dy >= -filter_size; dy--)
                        {
                            if (y + dy < 0) break;
                            for (int idx = 0; idx <= filter_size; idx++)
                            {
                                int dx = idx * bpp;
                                if (x + dx >= dt.Stride) break;
                                int pos = (y + dy) * dt.Stride + (x + dx);
                                int sum = 0;
                                for (int i = 0; i < bpp; i++)
                                {
                                    __avg[i] += data[pos + i];
                                    sum += data[pos + i];
                                }
                                if (minsum > sum) minsum = sum;
                                if (maxsum < sum) maxsum = sum;
                                total++;
                            }
                        }
                        int disp = maxsum - minsum;
                        if (disp < mindisp)
                        {
                            mindisp = disp;
                            avgclrs = __avg;
                            divval = total;
                        }
                    }
                    #endregion
                    #region down_right
                    {
                        int[] __avg = new int[bpp]; int total = 0, minsum = 10000, maxsum = 0;
                        for (int dy = 0; dy <= filter_size; dy++)
                        {
                            if (y + dy >= dt.Height) break;
                            for (int idx = 0; idx <= filter_size; idx++)
                            {
                                int dx = idx * bpp;
                                if (x + dx >= dt.Stride) break;
                                int pos = (y + dy) * dt.Stride + (x + dx);
                                int sum = 0;
                                for (int i = 0; i < bpp; i++)
                                {
                                    __avg[i] += data[pos + i];
                                    sum += data[pos + i];
                                }
                                if (minsum > sum) minsum = sum;
                                if (maxsum < sum) maxsum = sum;
                                total++;
                            }
                        }
                        int disp = maxsum - minsum;
                        if (disp < mindisp)
                        {
                            mindisp = disp;
                            avgclrs = __avg;
                            divval = total;
                        }
                    }
                    #endregion
                    #region down_left
                    {
                        int[] __avg = new int[bpp]; int total = 0, minsum = 10000, maxsum = 0;
                        for (int dy = 0; dy <= filter_size; dy++)
                        {
                            if (y + dy >= dt.Height) break;
                            for (int idx = 0; idx >= -filter_size; idx--)
                            {
                                int dx = idx * bpp;
                                if (x + dx < 0) break;
                                int pos = (y + dy) * dt.Stride + (x + dx);
                                int sum = 0;
                                for (int i = 0; i < bpp; i++)
                                {
                                    __avg[i] += data[pos + i];
                                    sum += data[pos + i];
                                }
                                if (minsum > sum) minsum = sum;
                                if (maxsum < sum) maxsum = sum;
                                total++;
                            }
                        }
                        int disp = maxsum - minsum;
                        if (disp < mindisp)
                        {
                            mindisp = disp;
                            avgclrs = __avg;
                            divval = total;
                        }
                    }
                    #endregion
                    int position = y * dt.Stride + x;
                    for (int i = 0; i < bpp; i++)
                    {
                        data2[position + i] = (byte)(avgclrs[i] / (divval == 0 ? 1 : divval));
                    }
                }
            });

            Marshal.Copy(data2, 0, dt.Scan0, ln);
            image.UnlockBits(dt);
        }
        static Bitmap RandomImg(int width, int height, int input = 1, int seed = 0, int MAX_AVB = 100, int MAX_OK = 49)
        {
            Random r = new Random(seed);
            Bitmap bmp = new Bitmap(width, height, PixelFormat.Format8bppIndexed);
            BitmapData bd = bmp.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.ReadWrite, PixelFormat.Format8bppIndexed);
            byte[] gento = new byte[width * height];

            Queue<Pair<int, int>> nexts = new Queue<Pair<int, int>>();
            for (int ijk = 0; ijk < input; ijk++)
            {
                nexts.Enqueue(new Pair<int, int>(r.Next(0, width), r.Next(0, height)));
            }
            while (nexts.Count > 0)
            {
                var nxt = nexts.Dequeue();
                if (nxt.first < 0 || nxt.first >= width || nxt.second < 0 || nxt.second >= height || gento[nxt.second * width + nxt.first] > 0) continue;
                gento[nxt.second * width + nxt.first] = 255;
                if (r.Next(0, MAX_AVB) <= MAX_OK) nexts.Enqueue(new Pair<int, int>(nxt.first - 1, nxt.second));
                if (r.Next(0, MAX_AVB) <= MAX_OK) nexts.Enqueue(new Pair<int, int>(nxt.first + 1, nxt.second));
                if (r.Next(0, MAX_AVB) <= MAX_OK) nexts.Enqueue(new Pair<int, int>(nxt.first, nxt.second - 1));
                if (r.Next(0, MAX_AVB) <= MAX_OK) nexts.Enqueue(new Pair<int, int>(nxt.first, nxt.second + 1));
            }

            Marshal.Copy(gento, 0, bd.Scan0, width * height);
            bmp.UnlockBits(bd);
            return bmp;
        }
        static Bitmap AverageCounter(string prefix, string postfix, int count, int width, int height)
        {
            int[] sum = new int[width * height];
            int total = 0;
            Parallel.For(0, count, (i) =>
            {
                Bitmap curr = new Bitmap(prefix + i + postfix);
                var a = curr.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);
                byte[] bytes = new byte[a.Stride * a.Height];
                Marshal.Copy(a.Scan0, bytes, 0, bytes.Length);
                int bpp = a.Stride / a.Width;
                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < a.Width; x++)
                    {
                        for (int k = 0; k < 3; k++)
                        {
                            sum[y * width + x] += bytes[y * a.Stride + x * bpp + k];
                        }
                    }
                }
                curr.UnlockBits(a);
                Console.Write("\r{0}", ++total);
            });
            Bitmap hotmap = new Bitmap(width, height, PixelFormat.Format24bppRgb);
            var hmd = hotmap.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.WriteOnly, PixelFormat.Format24bppRgb);
            byte[] rb = new byte[hmd.Stride * hmd.Height];
            for (int y = 0; y < hmd.Height; y++)
            {
                for (int x = 0; x < hmd.Width; x++)
                {
                    int pos = y * hmd.Stride + x * 3;
                    rb[pos] = rb[pos + 1] = rb[pos + 2] = (byte)(sum[y * width + x] * 255 / count);
                }
            }
            Marshal.Copy(rb, 0, hmd.Scan0, hmd.Stride * hmd.Height);
            hotmap.UnlockBits(hmd);
            return hotmap;
        }
        static void SoundDrawer(string path)
        {
            BinaryReader br = new BinaryReader(new FileStream(path, FileMode.Open, FileAccess.Read), Encoding.ASCII);

            string chunkId = "";
            for (int i = 0; i < 4; i++)
            {
                chunkId += br.ReadChar();
            }
            Console.WriteLine("Chunk Id: {0}", chunkId);
            if (chunkId != "RIFF")
            {
                Console.WriteLine("Invalid chunkId");
                return;
            }

            uint ChunkSize = br.ReadUInt32();
            Console.WriteLine("Chunk Size: {0}", ChunkSize);

            string format = "";
            for (int i = 0; i < 4; i++)
            {
                format += br.ReadChar();
            }
            Console.WriteLine("Format: {0}", format);
            if (format != "WAVE")
            {
                Console.WriteLine("Format not supported");
                return;
            }

            string subchunk1id = "";
            for (int i = 0; i < 4; i++)
            {
                subchunk1id += br.ReadChar();
            }
            Console.WriteLine("SubChunk 1 Id: {0}", subchunk1id);
            if (subchunk1id != "fmt ")
            {
                Console.WriteLine("Invalid SubChunk 1 Id");
            }

            uint subchunk1size = br.ReadUInt32();
            Console.WriteLine("SubChunk 1 Size: {0} bytes", subchunk1size);

            ushort audioformatid = br.ReadUInt16();
            Console.WriteLine("AudioFormat: {0}", audioformatid);
            if (audioformatid != 1)
            {
                Console.WriteLine("Only PAC format supported");
                return;
            }

            ushort numChannels = br.ReadUInt16();
            Console.WriteLine("Number of Channels: {0}", numChannels);

            uint samplerate = br.ReadUInt32();
            Console.WriteLine("Sample rate: {0}Hz", samplerate);

            uint byterate = br.ReadUInt32();
            Console.WriteLine("Byte rate: {0}bps", byterate);

            ushort blockalign = br.ReadUInt16();
            Console.WriteLine("Bloack align: {0} bytes", blockalign);

            ushort bitsPerSample = br.ReadUInt16();
            Console.WriteLine("Bits Per Sample: {0}bit", bitsPerSample);
            while (br.BaseStream.Position < br.BaseStream.Length)
            {
                string subchunk2id = "";
                for (int i = 0; i < 4; i++)
                {
                    subchunk2id += br.ReadChar();
                }
                Console.WriteLine("SubChunk Id: {0}", subchunk2id);

                uint subchunk2size = br.ReadUInt32();
                Console.WriteLine("SubChunk Size: {0} bytes", subchunk2size);

                switch (subchunk2id)
                {
                    case "LIST":
                        {
                            string listtypeid = "";
                            for (int i = 0; i < 4; i++)
                            {
                                listtypeid += br.ReadChar();
                            }
                            Console.WriteLine("List Type ID: {0}", listtypeid);
                            switch (listtypeid)
                            {
                                case "INFO":
                                    {
                                        Console.WriteLine("List data:");
                                        for (uint ijk = 4; ijk < subchunk2size;)
                                        {
                                            string __dataid = "";
                                            for (int i = 0; i < 4; i++) __dataid += br.ReadChar();
                                            ijk += 4;
                                            uint __size = br.ReadUInt32();
                                            ijk += 4 + __size;
                                            string __dt = "";
                                            for (int i = 0; i < __size; i++)
                                            {
                                                __dt += br.ReadChar();
                                            }
                                            Console.WriteLine("\t{0} : {1}", __dataid, __dt);
                                        }
                                    }
                                    break;
                                default:
                                    Console.WriteLine("List type not supported");
                                    return;
                            }
                        }
                        break;
                    case "data":
                        {
                            for(uint i = 0; i < subchunk2size; i+= blockalign)
                            {
                                for(int j = 0; j < blockalign; j++)
                                    Console.Write("{0} ", br.ReadByte());
                                Console.WriteLine();
                            }
                        }
                        break;
                    default:
                        Console.WriteLine("Subchunk type not supported");
                        break;
                }
            }
            br.Close();
        }
        static void SomeRandomSamples(string path)
        {
            BinaryWriter bw = new BinaryWriter(new FileStream(path, FileMode.Create, FileAccess.Write), Encoding.ASCII);

            bw.BaseStream.Position = 0;
            bw.Write(new byte[] { 82,73,70,70});

            bw.BaseStream.Position = 4;
            bw.Write((uint)5090036); //here

            bw.BaseStream.Position = 8;
            bw.Write(new byte[] { 87,65,86,69,102,109,116,32});
            bw.Write((uint)5090024); //here
            bw.Write((ushort)1); //format
            bw.Write((ushort)1); //channels
            bw.Write((uint)8000); //samplerate
            bw.Write((uint)8000); //byterate
            bw.Write((ushort)1); //block size
            bw.Write((ushort)8); //bitrate
            bw.Write(new byte[] { 100, 97, 116, 97 });
            bw.Write((uint)5090000); //here
            for(int ijk = 0; ijk < 10000; ijk++)
            {
                int total = 0;
                for (byte smp = 0; smp < byte.MaxValue; smp++)
                {
                    total++;
                    bw.Write(smp);
                }
                for (byte smp = byte.MaxValue - 1; smp > 0; smp--)
                {
                    total++;
                    bw.Write(smp);
                }
                Console.Write("\r{0} {1}",ijk,total);
            }
            bw.Close();
        }
    }
    class Pair<T, V>
    {
        public T first;
        public V second;

        public Pair(T first, V second)
        {
            this.first = first;
            this.second = second;
        }
    }
}
