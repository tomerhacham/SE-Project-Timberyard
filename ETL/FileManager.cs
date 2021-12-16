using ETL.Utils;
using ETL.Utils.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace ETL
{
    public class FileManager
    {
        private string _newLogsDirectory;
        private string _handeledLogsDirectory;
        private string _faultLogsDirectory;
        private readonly ILogger _logger;

        public FileManager(Directories directories,ILogger logger)
        {
            _newLogsDirectory = directories.NewLogsDirectory;
            _handeledLogsDirectory = directories.HandeledLogsDirectory;
            _faultLogsDirectory = directories.FaultLogsDirectory;
            _logger = logger;
        }


        /// <summary>
        /// Returns all .json files in the directory and the sub directory
        /// </summary>
        /// <returns></returns>
        public Result<string[]> GetNewLogs()
        {
            try
            {
                var files = Directory.GetFiles(_newLogsDirectory, "*.json", searchOption: SearchOption.AllDirectories);
                return new Result<string[]>(true,files);
            }
            catch (Exception e)
            {
                return new Result<string[]>(false, null, e.Message);
            }
        }
        public Result<string> ReadFromFile(string filePath)
        {
            try
            {
                var data = File.ReadAllText(filePath);
                return new Result<string>(true,data);
            }
            catch(Exception e)
            {
                return new Result<string>(false, null, e.Message);
            }

        }
        public Result<bool> MoveToHandeledLogsDirectory(string filePath)
        {
            var dst = Path.Combine(_handeledLogsDirectory, Path.GetFileName(filePath));
            return MoveFileToDirectory(filePath, dst);
        }
        public Result<bool> MoveToFaultLogsDirectory(string filePath)
        {
            var dst = Path.Combine(_faultLogsDirectory, Path.GetFileName(filePath));
            return MoveFileToDirectory(filePath, dst);
        }
        private Result<bool> MoveFileToDirectory(string srcFileName,string dstFileName)
        {
            try
            {
                File.Move(srcFileName, dstFileName);
                return new Result<bool>(true, true);
            }
            catch (Exception e)
            {
                return new Result<bool>(false, false, e.Message);
            }
        }
    }
}
