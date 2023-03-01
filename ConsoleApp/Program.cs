namespace ConsoleApp
{
    using System;

    internal class Program
    {
        static void Main(string[] args)
        {
            string filePath = GetFilePath(args);

            bool printingFlagConverted = args.Length > 1 ? ConvertPrintingFlag(args[1]) : true;

            var reader = new DataReader();
            reader.ImportAndPrintData(filePath, printingFlagConverted);
        }

        private static string GetFilePath(string[] args)
        {
            if (args.Length == 0)
            {
                throw new ArgumentException("File not specified");
            }
            else
            {
                return args[0];
            }
        }

        private static bool ConvertPrintingFlag(string printingFlag)
        {
            try
            {
                return Convert.ToBoolean(printingFlag);
            }
            catch (FormatException fe)
            {
                Console.WriteLine($"Setting for printing was not recognized as a valid boolean value: {printingFlag}");

                throw fe;
            }
        }
    }
}
