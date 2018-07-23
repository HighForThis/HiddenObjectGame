using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using System;
using System.Collections.Generic;
using System.IO;

//*************************************************************************
//@header       FileManager
//@abstract     Read and write files.
//@version      v1.0.0
//@author       Felix Zhang
//@copyright    Copyright (c) 2017 FFTAI Co.,Ltd.All rights reserved.
//**************************************************************************

namespace FZ.HiddenObjectGame
{
    public class FileManager
    {
        static FileManager _instance;

        public string[] ReadAllLines(string filePath)
        {
            return File.ReadAllLines(filePath);
        }

        public static FileManager Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new FileManager();
                return _instance;
            }
        }

        #region File/Directory Name
        public List<string> GetFileName(string path,string fileType)
        {
            DirectoryInfo dir = new DirectoryInfo(path);
            List<string> fileList = new List<string>();
            foreach (FileInfo fileInfo in dir.GetFiles(fileType))
            {
                fileList.Add(fileInfo.Name);
            }
            return fileList;
        }
        public List<string> GetDirectoryName(string path)
        {
            DirectoryInfo dir = new DirectoryInfo(path);
            List<string> dirList = new List<string>();
            foreach (DirectoryInfo dirInfo in dir.GetDirectories())
            {
                dirList.Add(dirInfo.Name);
            }
            return dirList;
        }
        public string GetValue(string[] fileLines, string propertyName, string sectionName = null)
        {
            bool isSectionFound = false;

            try
            {
                for (int i = 0; i < fileLines.Length; i++)
                {
                    string line = fileLines[i].Trim();

                    if (line == string.Empty || line[0] == '#')
                        continue;

                    if (sectionName != null)
                    {
                        if (line[0] == '[')
                        {
                            if (isSectionFound)
                            {
                                // Not find property.
                                break;
                            }
                            line = line.Substring(1, line.Length - 2).Trim();
                            if (line == sectionName)
                            {
                                isSectionFound = true;
                                continue;
                            }
                        }
                        if (isSectionFound)
                        {
                            // Split by '='.
                            string[] pair = line.Split('=');
                            pair[0] = pair[0].Trim();
                            pair[1] = pair[1].Trim();
                            if (pair[0] == propertyName)
                                return pair[1];
                        }
                    }
                    else if (sectionName == null)
                    {
                        // Split by '='.
                        string[] pair = line.Split('=');
                        pair[0] = pair[0].Trim();
                        pair[1] = pair[1].Trim();
                        if (pair[0] == propertyName)
                            return pair[1];
                    }
                }

                // Not find property.
                return null;
            }
            catch (Exception e)
            {
                UnityEngine.Debug.LogError(e);
                throw;
            }
        }

        public void SetValue(string[] fileLines, string propertyName, string propertyValue, string sectionName = null)
        {
            bool isSectionFound = false;

            try
            {
                for (int i = 0; i < fileLines.Length; i++)
                {
                    string line = fileLines[i].Trim();

                    if (line == string.Empty || line[0] == '#')
                        continue;

                    if (sectionName != null)
                    {
                        if (line == string.Empty || line[0] == '[')
                        {
                            if (isSectionFound)
                            {
                                // Not find property.
                                break;
                            }
                            line = line.Substring(1, line.Length - 2).Trim();
                            if (line == sectionName)
                            {
                                isSectionFound = true;
                                continue;
                            }
                        }
                        if (isSectionFound)
                        {
                            // Split by '='.
                            string[] pair = line.Split('=');
                            pair[0] = pair[0].Trim();
                            pair[1] = pair[1].Trim();
                            if (pair[0] == propertyName)
                            {
                                pair[1] = propertyValue;
                                string newLine = pair[0] + '=' + pair[1];
                                fileLines[i] = newLine;
                                return;
                            }
                        }
                    }
                    else if (sectionName == null)
                    {
                        // Split by '='.
                        string[] pair = line.Split('=');
                        pair[0] = pair[0].Trim();
                        pair[1] = pair[1].Trim();
                        if (pair[0] == propertyName)
                        {
                            pair[1] = propertyValue;
                            string newLine = pair[0] + '=' + pair[1];
                            fileLines[i] = newLine;
                            return;
                        }
                    }
                }

                // Not find property.
                throw new KeyNotFoundException();
            }
            catch (Exception e)
            {
                UnityEngine.Debug.LogError(e);
                throw;
            }
        }

        public void SetValue(string[] fileLines, string propertyName, int propertyValue, string sectionName = null)
        {
            SetValue(fileLines, propertyName, propertyValue.ToString(), sectionName);
        }

        public int GetIntValue(string[] fileLines,string propertyName,string sectionName,int min,int max)
        {
            string data = GetValue(fileLines, propertyName, sectionName);
            int tempResult;
            int result;
            int.TryParse(data, out tempResult);
            result = tempResult < min ? min : tempResult;
            result = tempResult > max ? max : tempResult;
            return result;
        }

        public void WriteAllLines(string filePath, string[] fileLines)
        {
            File.WriteAllLines(filePath, fileLines);
        }

        #endregion File/Directory Name

        #region Read/Write Excel
        public double ReadExcelNumber(string filePath, int sheetIndex, int rowID, int colID)
        {
            ICell cell = ReadExcel(filePath, sheetIndex, rowID, colID);
            double data = cell.NumericCellValue;
            return data;
        }

        public string ReadExcelString(string filePath, int sheetIndex, int rowID, int colID)
        {
            ICell cell = ReadExcel(filePath, sheetIndex, rowID, colID);
            string data = "";
            if (cell != null)
            {
                data = cell.StringCellValue;
            }
            return data;
        }
        #endregion

        #region PrivateMethods
        ICell ReadExcel(string filePath, int sheetIndex, int rowID, int colID)
        {
            FileStream readFile = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            HSSFWorkbook hssfworkbook = new HSSFWorkbook(readFile);
            readFile.Close();
            ISheet sheet1 = hssfworkbook.GetSheetAt(sheetIndex);
            if (sheet1 == null)
                return null;
            IRow row = sheet1.GetRow(rowID);
            if (row == null)
                return null;
            ICell cell = row.GetCell(colID);
            return cell;
        }
        #endregion
    }
}
