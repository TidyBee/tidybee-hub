using System;
using System.Collections.Generic;

namespace TidyEvents.Models;

public partial class DuplicateAssociativeTable
{
    public int Id { get; set; }

    public int OriginalFileId { get; set; }

    public int DuplicateFileId { get; set; }

    public virtual File DuplicateFile { get; set; } = null!;

    public virtual File OriginalFile { get; set; } = null!;
}
