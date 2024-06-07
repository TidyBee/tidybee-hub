using System;
using System.Collections.Generic;
namespace DataProcessing.Models
{
    public class Overview
    {
        public string? pretty_path { get; set; }
        public int size { get; set; }
        public LastModified? last_modified { get; set; }
        public TidyScore? tidy_score { get; set; }
    }

    public class LastModified
    {
        public long secs_since_epoch { get; set; }
        public long nanos_since_epoch { get; set; }
    }

    public class TidyScore
    {
        public char grade { get; set; }
        public Misnamed? misnamed { get; set; }
        public Unused? unused { get; set; }
        public Duplicated? duplicated { get; set; }
    }

    public class Misnamed
    {
        public char grade { get; set; }
        public List<Configuration>? configurations { get; set; }
    }

    public class Configuration
    {
        public string? name { get; set; }
        public char grade { get; set; }
        public double weight { get; set; }
        public string? regex { get; set; }
        public int? limitInt { get; set; }
        public string? limitISO { get; set; }
        public string? description { get; set; }
    }

    public class Unused
    {
        public char grade { get; set; }
        public List<Configuration>? configurations { get; set; }
    }

    public class Duplicated
    {
        public char grade { get; set; }
        public List<Configuration>? configurations { get; set; }
    }

    public class TidyRule
    {
        public List<Rule>? rules { get; set; }
    }

    public class Rule
    {
        public string? name { get; set; }
        public string? description { get; set; }
        public double? weight { get; set; }
        public List<Configuration>? configurations { get; set; }
    }
}