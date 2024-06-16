using System;
using System.Collections.Generic;
namespace DataProcessing.Models.Input
{
    public class FileModel
    {
        public string? pretty_path { get; set; }
        public int size { get; set; }
        public DateTime? last_modified { get; set; }
        public char grade { get; set; }
        public List<InputConfiguration>? configurations { get; set; }
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
        public List<InputConfiguration>? configurations { get; set; }
    }


    public partial class DuplicateAssociativeTable
    {
        public int Id { get; set; }

        public int OriginalFileId { get; set; }

        public int DuplicateFileId { get; set; }

        public virtual File DuplicateFile { get; set; } = null!;

        public virtual File OriginalFile { get; set; } = null!;
    }

    public partial class File
    {
        public int Id { get; set; }

        public string Name { get; set; } = null!;

        public int Size { get; set; }

        public string FileHash { get; set; } = null!;

        public DateTime LastModified { get; set; }

        public char MisnamedScore { get; set; }

        public char PerishedScore { get; set; }

        public char DuplicatedScore { get; set; }

        public char GlobalScore { get; set; }

        public virtual ICollection<DuplicateAssociativeTable> DuplicateAssociativeTableDuplicateFiles { get; set; } = new List<DuplicateAssociativeTable>();

        public virtual ICollection<DuplicateAssociativeTable> DuplicateAssociativeTableOriginalFiles { get; set; } = new List<DuplicateAssociativeTable>();
    }

    public partial class Rule
    {
        public int Id { get; set; }

        public string Name { get; set; } = null!;

        public string? Description { get; set; }

        public double Weight { get; set; }

        public string RulesConfig { get; set; } = null!;
    }
}