﻿using System;
using System.IO;
using System.Globalization;
using System.Text;
using System.Linq;
using System.Collections.Generic;

namespace passwordCreator
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < 3; i++ ){
                var list = ReadAllLinesFromRandomFileInDirectory("./sourceFiles/");
                var random = new Random();
                int index = random.Next(list.Count);
                sb.Append(list[index]);
            }
            Console.WriteLine(sb.ToString());
            TextCopy.ClipboardService.SetText(sb.ToString());
        }
    
        static void RemoveAccentuationFromFile (string filePath) {
            int linesRemoved = 0;
            String line;
            try
            {
                StreamReader sr = new StreamReader(filePath, Encoding.GetEncoding("iso-8859-1")); // Latin 1; Western European (ISO) Encoding, otherwise it get opened in UTF8
                string tempFile = Path.GetTempFileName();
                StreamWriter sw = new StreamWriter(tempFile);
                line = sr.ReadLine();
                while (line != null)
                {
                    string normalizedLine = line.Normalize(NormalizationForm.FormD); // https://stackoverflow.com/questions/9349608/how-to-check-if-unicode-character-has-diacritics-in-net It separate the diacritics from the preceding letter
                    int lineLength = normalizedLine.Length; // So that we can know where we're in our line & get the last letter
                    int currentPos = 0;
                    foreach (char c in normalizedLine)
                    {
                        switch (CharUnicodeInfo.GetUnicodeCategory(c))
                        {
                            case UnicodeCategory.UppercaseLetter: // I don't want countries names or people in my password
                                Console.WriteLine(line);
                                linesRemoved++;
                                break;
                            case UnicodeCategory.DashPunctuation:   // so that we don't get words with a dash in it ex: demi-dieu, semi-automatique, sous-bois
                                Console.WriteLine(line);
                                linesRemoved++;
                                break;
                            case UnicodeCategory.NonSpacingMark: // With the Normalize method we separate our accent from our letter
                                Console.WriteLine(line);
                                linesRemoved++;
                                break;
                            default:
                                ++currentPos;
                                break;
                        }

                        if (currentPos == lineLength) {
                            switch (CharUnicodeInfo.GetUnicodeCategory(c))
                            {
                                case UnicodeCategory.NonSpacingMark:
                                    Console.WriteLine(line);
                                    linesRemoved++;
                                    break;
                                default:
                                    sw.WriteLine(line);
                                    break;
                            }
                        }
                    }
                    line = sr.ReadLine();
                }
                sr.Close();
                Console.WriteLine($"Removed {linesRemoved} lines from file");
                File.Delete(filePath);
                File.Move(tempFile, filePath);
            }
            catch (System.Exception e)
            {
                Console.WriteLine("Exception: " + e.Message);
            }
        }

        static void AddUpperCaseLetter (string filePath) {
            try
            {
                StreamReader sr = new StreamReader(filePath);
                string tempFile = Path.GetTempFileName();

                using (StreamWriter sw = new StreamWriter(tempFile))
                {
                    String line = sr.ReadLine();
                    while (line != null)
                    {
                        sw.WriteLine(line.First().ToString().ToUpper() + line.Substring(1));
                        line = sr.ReadLine();
                    }
                }
                sr.Close();
                File.Delete(filePath);
                File.Move(tempFile, filePath);
            }
            catch (System.Exception e)
            {
                Console.WriteLine($"Exception: {e.Message}");
            }
        }

        static void RemoveWords (string filePath, int minLength, int maxLength) {
            try
            {
                StreamReader sr = new StreamReader(filePath);
                string tempFile = Path.GetTempFileName();
                String line;
                using (StreamWriter sw = new StreamWriter(tempFile))
                {
                    line = sr.ReadLine();
                    int lineLength;
                    while (line != null)
                    {
                        lineLength = line.Length;
                        if (lineLength > minLength && lineLength < maxLength) {
                            sw.WriteLine(line);
                            line = sr.ReadLine();
                        } else {
                            line = sr.ReadLine();
                        }

                    }
                }
                sr.Close();
                File.Delete(filePath);
                File.Move(tempFile,filePath);
            }
            catch (System.Exception e)
            {
                Console.WriteLine($"Exception: {e.Message}");
            }
        }

        static List<string> ReadAllLinesFromRandomFileInDirectory (string directoryPath) {
            int fCount = Directory.GetFiles(directoryPath, "*.txt", SearchOption.TopDirectoryOnly).Length;
            var random = new Random();
            int fileIndex = random.Next(fCount);
            string[] files = Directory.GetFiles(directoryPath, "*.txt", SearchOption.TopDirectoryOnly);
            string filePath = files[fileIndex];
            return File.ReadAllLines(filePath).ToList();
        }
    }
}
