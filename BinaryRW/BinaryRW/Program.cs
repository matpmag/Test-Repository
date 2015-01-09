
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace BinaryRW
{
    internal class Program
    {
        static void Main()
        {
            BinaryWorker.WriteScore();
            BinaryWorker.ReadScore();
        }
    }

    public static class BinaryWorker
    {
        #region Fields

        private static byte[] binaryData;
        private static IList<Byte> byteList = new List<byte>();
        private static StreamReader FileReader;
        private static StreamWriter FileWriter;
        private static FileStream inFile;
        private static IList<int> intList = new List<int>();
        private static string pathProgressOutputFile = "../../Stuff/userdata/outputbin.bin";
        private static string pathProgressTemporaryFile = "../../Stuff/userdata/progressTempFile.temp";

        public static int userID = 5;
        public static int userZoneUnlock = 3;
        public static int userScoreManna = 5000;
        public static int userScoreQuail = 200;

        #endregion Fields

        #region Methods

        public static void ReadScore()
        {
            int[] progress = new int[4];
            ReadBinary(userID, out progress);
            userID = progress[0];
            userZoneUnlock = progress[1];
            userScoreManna = progress[2];
            userScoreQuail = progress[3];
            string output = string.Format("ID: {0}\nZone: {1}\nManna: {2}\nQuail: {3}",userID.ToString(),userZoneUnlock.ToString(),userScoreManna.ToString(),userScoreQuail.ToString());
            Console.Write(output);
            Console.ReadLine();
        }

        public static void ResetScore()
        {
            int[] progress = new int[]
            {
                userID,
                0,
                0,
                0
            };
            WriteBinary(progress);
        }

        public static void WriteScore()
        {
            int[] progress = new int[]
            {
                userID,
                userZoneUnlock,
                userScoreManna,
                userScoreQuail
            };
            WriteBinary(progress);
        }

        private static string binaryToText(char[] input)
        {
            int result;
            int bitVal;
            char[] digitAsBinaryCharArr = new char[8];
            int[] digitAsBinaryIntArr = new int[8];
            List<char> translatedList = new List<char>();
            for (int i = 0; i < 64; i += 8)
            {
                result = 0;
                bitVal = 1;
                Array.Copy(input, i, digitAsBinaryCharArr, 0, 8);
                for (int j = 0; j < digitAsBinaryCharArr.Length; j++)
                {
                    digitAsBinaryIntArr[j] = int.Parse(digitAsBinaryCharArr[j].ToString());
                }

                for (int m = digitAsBinaryIntArr.Length; m > 0; m--)
                {
                    result += digitAsBinaryIntArr[m - 1] * bitVal;
                    bitVal *= 2;
                }
                translatedList.Add((char)result);
            }
            return string.Join("", translatedList.ToArray());
        }

        private static void ReadBinary(int userID, out int[] userProgress)
        {
            userProgress = new int[4];
            int numberOfRecords = File.ReadAllLines(pathProgressOutputFile).Length;
            using (FileReader = new StreamReader(pathProgressOutputFile))
            {
                for (int i = 0; i < numberOfRecords; i++)
                {
                    string record = FileReader.ReadLine();
                    char[] recordCharArray = record.ToCharArray();
                    char[] userIDAsBinary = new char[64];
                    Array.Copy(recordCharArray, 0, userIDAsBinary, 0, 64);
                    int recoveredUserID = int.Parse(binaryToText(userIDAsBinary));
                    if (userID == recoveredUserID)
                    {
                        char[] furthestUnlockAsBinary = new char[64];
                        char[] mannaAsBinary = new char[64];
                        char[] quailAsBinary = new char[64];
                        Array.Copy(recordCharArray, 64, furthestUnlockAsBinary, 0, 64);
                        Array.Copy(recordCharArray, 128, mannaAsBinary, 0, 64);
                        Array.Copy(recordCharArray, 192, quailAsBinary, 0, 64);

                        int recoveredFurthestUnlock = int.Parse(binaryToText(furthestUnlockAsBinary));
                        int recoveredManna = int.Parse(binaryToText(mannaAsBinary));
                        int recoveredQuail = int.Parse(binaryToText(quailAsBinary));

                        userProgress[0] = recoveredUserID;
                        userProgress[1] = recoveredFurthestUnlock;
                        userProgress[2] = recoveredManna;
                        userProgress[3] = recoveredQuail;
                    }
                }
            }
        }

        private static void WriteBinary(int[] input)
        {
            #region Remove Any Past Info
            if (File.Exists(pathProgressTemporaryFile))
            {
                File.Delete(pathProgressTemporaryFile);                
            }
            byteList.Clear();

            #endregion Remove Any Past Info

            #region Prepare Scores For Conversion

            string progressToWrite = string.Format("{0,8:D8}{1,8:D8}{2,8:D8}{3,8:D8}", input[0], input[1], input[2], input[3]);

            using (FileWriter = new StreamWriter(pathProgressTemporaryFile))
            {
                FileWriter.Write(progressToWrite);
            }

            #endregion Prepare Scores For Conversion

            #region Read From Temporary File, Then Delete

            using (inFile = new FileStream(pathProgressTemporaryFile, FileMode.Open))
            {
                binaryData = new byte[inFile.Length];
                long bytesRead = inFile.Read(binaryData, 0, (int)inFile.Length);
            }
            File.Delete(pathProgressTemporaryFile);

            #endregion Read From Temporary File, Then Delete

            #region Convert To Binary

            foreach (byte val in binaryData)
            {
                byteList.Add(val);
            }
            string[] bitStringArray = new string[byteList.Count];
            for (int i = 0; i < byteList.Count; i++)
            {
                bitStringArray[i] = Convert.ToString(byteList[i], 2).PadLeft(8, '0');
            }

            #endregion Convert To Binary

            #region Write Bits To File

            string binaryToWrite = string.Join("", bitStringArray);
            binaryToWrite = binaryToWrite.Replace(" ", "");

            using (FileWriter = new StreamWriter(pathProgressOutputFile, true))
            {
                FileWriter.WriteLine(binaryToWrite);
            }

            #endregion Write Bits To File
        }

        #endregion Methods
    }
}
