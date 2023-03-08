using System;
namespace Beatshop.Models;

public class Beat
{
    public string Id { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public string Genre { get; set; } = string.Empty;

    public string CreatedById { get; set; } = string.Empty;
}

