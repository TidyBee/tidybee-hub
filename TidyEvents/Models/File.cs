using System;
using System.Collections.Generic;

namespace TidyEvents.Models;

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
