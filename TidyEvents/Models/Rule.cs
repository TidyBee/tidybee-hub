using System;
using System.Collections.Generic;

namespace TidyEvents.Models;

public partial class Rule
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public double Weight { get; set; }

    public string RulesConfig { get; set; } = null!;
}
