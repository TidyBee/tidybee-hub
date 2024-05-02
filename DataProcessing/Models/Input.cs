using System;
using System.Collections.Generic;
namespace DataProcessing.Models
{
    public class FileModel
    {
        public string? pretty_path { get; set; }
        public int size { get; set; }
        public DateTime? last_modified { get; set; }
        public char grade { get; set; }
        public List<InputConfiguration?> configurations { get; set; }
    }

    public class InputConfiguration
    {
        public string? name { get; set; }
        public char? grade { get; set; }
        public double? weight { get; set; }
        public string? description { get; set; }
        public string? regex { get; set; }
        public string? limitISO { get; set; }
        public int? limitInt { get; set; }
    }

    public class InputRule
    {
        public string? name { get; set; }
        public List<InputConfiguration?> configurations { get; set; }
    }
}