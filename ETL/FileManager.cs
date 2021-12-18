using ETL.Utils;
using ETL.Utils.Models;
using Microsoft.Extensions.Options;
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

        public FileManager(IOptions<Directories> directories, ILogger logger)
        {
            _newLogsDirectory = directories.Value.NewLogsDirectory;
            _handeledLogsDirectory = directories.Value.HandeledLogsDirectory;
            _faultLogsDirectory = directories.Value.FaultLogsDirectory;
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
                _logger.Info($"{files.Length} new logs have been found", new Dictionary<LogEntry, string>() { { LogEntry.Component, GetType().Name } });
                return new Result<string[]>(true, files);
            }
            catch (Exception e)
            {
                _logger.Warning("Error raised while pulling new logs", e, new Dictionary<LogEntry, string>() { { LogEntry.Component, GetType().Name } });
                return new Result<string[]>(false, null, e.Message);
            }
        }
        /// <summary>
        /// Reading data from given file and return as string
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public Result<string> ReadFromFile(string filePath)
        {
            try
            {
                var data = File.ReadAllText(filePath);
                return new Result<string>(true, data);
            }
            catch (Exception e)
            {
                _logger.Warning($"Error raise while reading {filePath}", e, new Dictionary<LogEntry, string>() { { LogEntry.Component, GetType().Name } });
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
        private Result<bool> MoveFileToDirectory(string srcFileName, string dstFileName)
        {
            try
            {
                File.Move(srcFileName, dstFileName);
                _logger.Info($"{Path.GetFileName(srcFileName)} moved to {Path.GetDirectoryName(dstFileName)}", new Dictionary<LogEntry, string>() { { LogEntry.Component, GetType().Name } });
                return new Result<bool>(true, true);
            }
            catch (Exception e)
            {
                _logger.Warning($"Error raise while moving {srcFileName} to {dstFileName}", e, new Dictionary<LogEntry, string>() { { LogEntry.Component, GetType().Name } });
                return new Result<bool>(false, false, e.Message);
            }
        }
    }
}
