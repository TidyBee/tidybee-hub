using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using DataProcessing.Models;

public class InputService
{
    public InputService()
    {

    }

    public List<InputRule> getRules()
    {
        var rules = new List<InputRule>
                        {
                            new InputRule
                            {
                                name = "misnamed",
                                configurations = new List<InputConfiguration>
                                {
                                    new InputConfiguration
                                    {
                                        name = "date",
                                        weight = 3,
                                        description = "The name of the file need to have a date",
                                        regex = @"r'_\d{4}\.'"
                                    },
                                    new InputConfiguration
                                    {
                                        name = "valid separator",
                                        weight = 1.8,
                                        description = "The name of the file need to have 4 separators _",
                                        regex = @"r'^[^_]*(_[^_]*){3}$'"
                                    }
                                }
                            },
                            new InputRule
                            {
                                name = "duplicate",
                                configurations = new List<InputConfiguration>
                                {
                                    new InputConfiguration
                                    {
                                        name = "occurrence",
                                        weight = 1,
                                        description = "The file need to be unique",
                                        limitInt = 1
                                    }
                                }
                            },
                            new InputRule
                            {
                                name = "unused",
                                configurations = new List<InputConfiguration>
                                {
                                    new InputConfiguration
                                    {
                                        name = "perished",
                                        weight = 1,
                                        description = "The file need to be recent enough",
                                        limitISO = "2024-04-12T00:00:00Z"
                                    }
                                }
                            }
                        }
        return rules;
    }

}