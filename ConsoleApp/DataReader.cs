namespace ConsoleApp
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;

    public class DataReader
    {
        public void ImportAndPrintData(string fileToImport, bool printData = true)
        {
            IEnumerable<ImportedObject> ImportedObjects;
            ImportedObjects = ImportObjects(fileToImport);

            // clear and correct imported data
            ClearAndCorrectImportedData(ref ImportedObjects);

            // assign number of children
            AssignNumberOfChildren(ref  ImportedObjects);

            PrintData(ImportedObjects);

            Pause();
        }

        /// <summary>
        /// Method for ending the printing the data-printing sequence and keeping the console window up.
        /// </summary>
        private void Pause()
        {
            // here's some space for more sophisticated way of ending the printing sequence, eg. logging what happened along the way
            Console.ReadLine();
        }

        private IEnumerable<ImportedObject> ImportObjects(string fileToImport)
        {
            var importedObjects = new List<ImportedObject>();
            var importedLines = ReadTheFile(fileToImport);

            for (int i = 0; i < importedLines.Count; i++)
            {
                var importedLine = importedLines[i];
                ImportedObject importedObject = new ImportedObject();
                var values = importedLine.Split(';');

                try
                {
                    importedObject.Type = values[0];
                    importedObject.Name = values[1];
                    importedObject.Schema = values[2];
                    importedObject.ParentName = values[3];
                    importedObject.ParentType = values[4];
                    importedObject.DataType = values[5];
                    importedObject.IsNullable = values[6];
                }
                catch
                {
                    // TODO: logic in case when reading the values and assigning it to the new object did not succeed
                    // assuming that program should try to do it anyway, logging of what went wrong can be implemented
                    // indexes, row numbers etc can be catched here out of ArgumentOutOfRangeException
                }
                finally
                {
                    if (!(importedObject is null)) importedObjects.Add(importedObject);
                }
            }

            return importedObjects;
        }

        private List<string> ReadTheFile(string fileToImport)
        {
            var importedLines = new List<string>();

            try
            {
                using (var streamReader = new StreamReader(fileToImport))
                {
                    while (!streamReader.EndOfStream)
                    {
                        var line = streamReader.ReadLine();
                        importedLines.Add(line);
                    }
                }

                return importedLines;
            }
            catch
            {
                // TODO logic for handling file reading issues
                throw; // throwing it even though it should not be re-throwed 
            }
        }

        /// <summary>
        /// Clears the imported objects out of set of the unwanted signs.
        /// </summary>
        /// <param name="importedObjects">List of imported objects</param>
        /// <returns>True if no errors were met during processing.</returns>
        private bool ClearAndCorrectImportedData(ref IEnumerable<ImportedObject> importedObjects)
        {
            if (importedObjects is null) return false;

            bool dataCorrectionOperationWasSuccessfull = true;

            foreach (var importedObject in importedObjects)
            {
                try
                {
                    importedObject.Type = importedObject.Type.Trim().Replace(" ", "").Replace(Environment.NewLine, "").ToUpper();
                    importedObject.Name = importedObject.Name.Trim().Replace(" ", "").Replace(Environment.NewLine, "");
                    importedObject.Schema = importedObject.Schema.Trim().Replace(" ", "").Replace(Environment.NewLine, "");
                    importedObject.ParentName = importedObject.ParentName.Trim().Replace(" ", "").Replace(Environment.NewLine, "");
                    importedObject.ParentType = importedObject.ParentType.Trim().Replace(" ", "").Replace(Environment.NewLine, "");
                }
                catch
                {
                    dataCorrectionOperationWasSuccessfull = false;
                }
            }

            return dataCorrectionOperationWasSuccessfull;
        }

        /// <returns>True if no errors were met during processing.</returns>
        private bool AssignNumberOfChildren(ref IEnumerable<ImportedObject> importedObjects)
        {
            bool assigningWasSuccessfullFlag = true;

            try
            {
                for (int i = 0; i < importedObjects.Count(); i++)
                {
                    var importedObject = importedObjects.ToArray()[i];
                    foreach (var impObj in importedObjects)
                    {
                        if (impObj.ParentType == importedObject.Type)
                        {
                            if (impObj.ParentName == importedObject.Name)
                            {
                                importedObject.NumberOfChildren = 1 + importedObject.NumberOfChildren;
                            }
                        }
                    }
                }
            }
            catch { assigningWasSuccessfullFlag = false; }

            return assigningWasSuccessfullFlag;
        }

        private void PrintData(IEnumerable<ImportedObject> importedObjects)
        {
            foreach (var database in importedObjects)
            {
                if (database.Type == "DATABASE")
                {
                    Console.WriteLine($"Database '{database.Name}' ({database.NumberOfChildren} tables)");

                    // print all database's tables
                    foreach (var table in importedObjects)
                    {
                        if (table.ParentType.ToUpper() == database.Type)
                        {
                            if (table.ParentName == database.Name)
                            {
                                Console.WriteLine($"\tTable '{table.Schema}.{table.Name}' ({table.NumberOfChildren} columns)");

                                // print all table's columns
                                foreach (var column in importedObjects)
                                {
                                    if (column.ParentType.ToUpper() == table.Type)
                                    {
                                        if (column.ParentName == table.Name)
                                        {
                                            Console.WriteLine($"\t\tColumn '{column.Name}' with {column.DataType} data type {(column.IsNullable == "1" ? "accepts nulls" : "with no nulls")}");
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
    }

    class ImportedObject : ImportedObjectBaseClass
    {
        public string Name
        {
            get;
            set;
        }
        public string Schema;

        public string ParentName;
        public string ParentType
        {
            get; set;
        }

        public string DataType { get; set; }
        public string IsNullable { get; set; }

        public double NumberOfChildren;
    }

    class ImportedObjectBaseClass
    {
        public string Name { get; set; }
        public string Type { get; set; }
    }
}
