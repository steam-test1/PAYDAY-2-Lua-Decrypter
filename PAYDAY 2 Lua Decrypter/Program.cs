using System;
using System.IO;
using System.Text;
using System.Windows.Forms;
using Microsoft.Win32;
using Ookii.Dialogs.Wpf;

namespace Pd2LuaDecrypterCS
{
    class Program
    {
        // Key since PD2 U35 - Below that version please supply the argument < useOldKey >
        static string Pd2EncryptionKey = "asljfowk4_5u2333crj";
        static string encryptedFilesPath;
        static string decryptedFilesPath;

        [STAThread]
        static void Main(string[] args)
        {
            Console.Title = "PAYDAY 2 Lua Decrypter";
            Console.WriteLine("PD2 Lua Decrypter - Written by Sora and Modified by Test1\n\n");

            foreach (string argument in args)
            {
                if (argument == "useOldKey")
                {
                    Pd2EncryptionKey = ">S4?fo%k4_5u2(3_=cRj";
                    Console.WriteLine("Now using the old decryption key...");
                }
            }

            Console.WriteLine("Awaiting input.\nFirst select the folder where the encrypted files are.");

            VistaFolderBrowserDialog fbdEncrypt = new();
            Console.WriteLine("First select the folder where the encrypted files are.");
            if (fbdEncrypt.ShowDialog() == true)
            {
                encryptedFilesPath = fbdEncrypt.SelectedPath;
                Console.WriteLine("Encrypted folder set: " + encryptedFilesPath);
            }

            if (encryptedFilesPath == null)
            {
                errorPathIncorrect();
                return;
            }

            VistaFolderBrowserDialog fbdDecrypt = new();
            Console.WriteLine("Next, select the folder where the decrypted files will be.");
            fbdDecrypt.SelectedPath = encryptedFilesPath;
            if (fbdDecrypt.ShowDialog() == true)
            {
                decryptedFilesPath = fbdDecrypt.SelectedPath;
                Console.WriteLine("Dncrypted folder set: " + encryptedFilesPath);
            }

            if (decryptedFilesPath == null)
            {
                errorPathIncorrect();
                return;
            }

            Console.WriteLine("Beginning task.");

            string[] encryptedFiles = Directory.GetFiles(encryptedFilesPath, "*", SearchOption.AllDirectories);

            foreach (string filePath in encryptedFiles)
            {
                string newFilePath = filePath.Replace(encryptedFilesPath, "");

                Console.WriteLine("debug - " + newFilePath);

                Stream file = File.OpenRead(filePath);
                new FileInfo(decryptedFilesPath + newFilePath).Directory.Create();
                Stream oFile = File.Create(decryptedFilesPath + newFilePath);

                decrypt(file, oFile);

                file.Close();
                oFile.Close();

                Console.WriteLine("File " + newFilePath + " saved to " + decryptedFilesPath + newFilePath);
            }

            Console.WriteLine("\n\nTask ended. You can now close this program.");
            Console.ReadLine();
        }

        static void decrypt(Stream inputStream, Stream outputStream)
        {
            byte[] key = Encoding.ASCII.GetBytes(Pd2EncryptionKey);
            byte[] data = new byte[inputStream.Length];

            inputStream.Read(data, 0, data.Length);

            for (int i = 0; i < data.Length; i++)
            {
                int keyIndex = ((data.Length + i) * 7) % key.Length;
                data[i] ^= (byte)(key[keyIndex] * (data.Length - i));
            }

            outputStream.Write(data, 0, data.Length);
        }

        static void errorPathIncorrect()
        {
            Console.WriteLine("Error. Paths are incorrect.");
            Console.ReadLine();
        }
    }
}
