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
            for (int i = 0; i < ImportedObjects.Count(); i++)
            {
                var importedObject = ImportedObjects.ToArray()[i];
                foreach (var impObj in ImportedObjects)
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

            foreach (var database in ImportedObjects)
            {
                if (database.Type == "DATABASE")
                {
                    Console.WriteLine($"Database '{database.Name}' ({database.NumberOfChildren} tables)");

                    // print all database's tables
                    foreach (var table in ImportedObjects)
                    {
                        if (table.ParentType.ToUpper() == database.Type)
                        {
                            if (table.ParentName == database.Name)
                            {
                                Console.WriteLine($"\tTable '{table.Schema}.{table.Name}' ({table.NumberOfChildren} columns)");

                                // print all table's columns
                                foreach (var column in ImportedObjects)
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

            Console.ReadLine();
        }

        private IEnumerable<ImportedObject> ImportObjects(string fileToImport)
        {
            var importedObjects = new List<ImportedObject>();

            var streamReader = new StreamReader(fileToImport);

            var importedLines = new List<string>();
            while (!streamReader.EndOfStream)
            {
                var line = streamReader.ReadLine();
                importedLines.Add(line);
            }

            for (int i = 0; i <= importedLines.Count; i++)
            {
                var importedLine = importedLines[i];
                var values = importedLine.Split(';');
                var importedObject = new ImportedObject();
                importedObject.Type = values[0];
                importedObject.Name = values[1];
                importedObject.Schema = values[2];
                importedObject.ParentName = values[3];
                importedObject.ParentType = values[4];
                importedObject.DataType = values[5];
                importedObject.IsNullable = values[6];

                importedObjects.Add(importedObject);
            }

            return importedObjects;
        }

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
