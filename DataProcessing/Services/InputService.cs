using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using DataProcessing.Models.Input;
using DataProcessing.Context;
using Microsoft.EntityFrameworkCore;

public class InputService
{
    private readonly DatabaseContext _context;
    public InputService(DatabaseContext context)
    {
        _context = context;
    }

    public async Task<List<DataProcessing.Models.Input.Rule>> getRules()
    {
        List<DataProcessing.Models.Input.Rule> rules = await _context.Rules.ToListAsync();
        return rules;
    }

    public async Task<List<DataProcessing.Models.Input.File>> getFiles()
    {
        List<DataProcessing.Models.Input.File> rules = await _context.Files.ToListAsync();
        return rules;
    }

}