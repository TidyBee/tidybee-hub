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

    public async Task<List<DataProcessing.Models.Rule>> getRules()
    {
        List<DataProcessing.Models.Rule> rules = await _context.Rules.ToListAsync();
        return rules;
    }

}