using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace BinaryRW
{
    class Program
    {
        private static byte[] binaryData;
        private static IList<Byte> byteList = new List<byte>();
        private static StreamWriter FileWriter;
        private static FileStream inFile;
        private static string pathProgressTemporaryFile = "../../progressTempFile.temp";

        static void Main(string[] args)
        {
            int[] Scores = new int[] { 1, 54, 300, 100 };
            WriteBinary(Scores);
        }

        static void WriteBinary(int[] input)
        {
            string progressToWrite = string.Format("{0,8:D8} {1,8:D8} {2,8:D8} {3,8:D8}", input[0], input[1], input[2], input[3]);

            using (FileWriter = new StreamWriter(pathProgressTemporaryFile))
            {
                FileWriter.Write(progressToWrite);
            }

            using (inFile = new FileStream(pathProgressTemporaryFile, FileMode.Open))
            {
                binaryData = new byte[inFile.Length];
                long bytesRead = inFile.Read(binaryData, 0, (int)inFile.Length);
            }
            File.Delete(pathProgressTemporaryFile);

            foreach (byte val in binaryData)
            {
                byteList.Add(val);
            }
            string[] bitStringArray = new string[byteList.Count];
            for (int i = 0; i < byteList.Count; i++)
            {
                bitStringArray[i] = Convert.ToString(byteList[i], 2).PadLeft(8, '0');
            }

            string binaryToWrite = string.Join(" ", bitStringArray);

            using (FileWriter = new StreamWriter("../../outputbin.bin", true))
            {
                FileWriter.WriteLine(binaryToWrite);
            }
        }
    }
}
