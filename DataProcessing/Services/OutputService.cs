using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using DataProcessing.Models;

public class OutputService
{
    public OutputService()
    {

    }

    public string getTextWidgetUnused()
    {
        var data = new
        {
            title = "unused",
            types = "Number",
            data = new
            {
                percentage = "+8",
                value = "407",
                status = false
            }
        };
        var jsonData = JsonConvert.SerializeObject(data);
        return jsonData;
    }

    public string getGradeWidget()
    {
        var data = new
        {
            grade = "B"
        };
        var jsonData = JsonConvert.SerializeObject(data);
        return jsonData;
    }

    public string getTotalMonitored()
    {
        var data = new
        {
            title = "total",
            types = "Number",
            data = new
            {
                percentage = "+2",
                value = "105",
                status = true
            }
        };
        var jsonData = JsonConvert.SerializeObject(data);
        return jsonData;
    }

    public string getGraphWidget()
    {
        var data = new
        {
            series = new[] { 20, 32, 23, 15, 10 }
        };
        var jsonData = JsonConvert.SerializeObject(data);
        return jsonData;
    }

    public string getTextWidgetbadname()
    {
        var data = new
        {
            title = "badname",
            types = "Number",
            data = new
            {
                percentage = "-12",
                value = "259",
                status = true
            }
        };
        var jsonData = JsonConvert.SerializeObject(data);
        return jsonData;
    }

    public string getTextWidgetduplicate()
    {
        var data = new
        {
            title = "duplicate",
            types = "Number",
            data = new
            {
                percentage = "+19",
                value = "124",
                status = false
            }
        };
        var jsonData = JsonConvert.SerializeObject(data);
        return jsonData;
    }

    public string getTextWidgetstorage()
    {
        var data = new
        {
            title = "storage",
            types = "Graph",
            data = new
            {
                percentage = "+4",
                value = "237/512GB",
                valuePercentage = "28.32",
                status = false
            }
        };
        var jsonData = JsonConvert.SerializeObject(data);
        return jsonData;
    }

    public string getOverviewAll()
    {
        var data = new[]
        {
                new Overview
                {
                    pretty_path = "src/my_files.rs",
                    size = 21782,
                    last_modified = new LastModified
                    {
                        secs_since_epoch = 1706651511,
                        nanos_since_epoch = 396799014
                    },
                    tidy_score = new TidyScore
                    {
                        grade = 'B',
                        misnamed = new Misnamed
                        {
                            grade = 'A'
                        },
                        unused = new Unused
                        {
                            grade = 'A'
                        },
                        duplicated = new Duplicated
                        {
                            grade = 'B'
                        }
                    }
                }
            };

        var jsonData = JsonConvert.SerializeObject(data);
        return jsonData;
    }

    public string getOverviewMisnamed()
    {
        var data = new[]
        {
                new Overview
                {
                    pretty_path = "src/my_files.rs",
                    size = 21782,
                    last_modified = new LastModified
                    {
                        secs_since_epoch = 1706651511,
                        nanos_since_epoch = 396799014
                    },
                    tidy_score = new TidyScore
                    {
                        grade = 'A',
                        misnamed = new Misnamed
                        {
                            grade = 'A',
                            configurations = new List<Configuration>
                            {
                                new Configuration
                                {
                                    name = "date",
                                    grade = 'A',
                                    weight = 3
                                },
                                new Configuration
                                {
                                    name = "valid separator",
                                    grade = 'A',
                                    weight = 1.8
                                }
                            }
                        }
                    }
                }
            };

        var jsonData = JsonConvert.SerializeObject(data);
        return jsonData;
    }

    public string getOverviewDuplicate()
    {
        var data = new[]
        {
                new Overview
                {
                    pretty_path = "src/my_files.rs",
                    size = 21782,
                    last_modified = new LastModified
                    {
                        secs_since_epoch = 1706651511,
                        nanos_since_epoch = 396799014
                    },
                    tidy_score = new TidyScore
                    {
                        grade = 'B',
                        duplicated = new Duplicated
                        {
                            grade = 'B',
                            configurations = new List<Configuration>
                            {
                                new Configuration
                                {
                                    name = "occurrence",
                                    grade = 'B',
                                    weight = 1,
                                    description = "The file need to be unique",
                                    limitInt = 1
                                }
                            }
                        }
                    }
                }
            };

        var jsonData = JsonConvert.SerializeObject(data);
        return jsonData;
    }

    public string getOverviewUnused()
    {
        var data = new[]
        {
                new Overview
                {
                    pretty_path = "src/my_files.rs",
                    size = 21782,
                    last_modified = new LastModified
                    {
                        secs_since_epoch = 1706651511,
                        nanos_since_epoch = 396799014
                    },
                    tidy_score = new TidyScore
                    {
                        grade = 'C',
                        unused = new Unused
                        {
                            grade = 'C',
                            configurations = new List<Configuration>
                            {
                                new Configuration
                                {
                                    name = "perished",
                                    grade = 'C',
                                    weight = 1,
                                    description = "The file need to be recent enough",
                                    limitISO = "2024-04-12T00:00:00Z"
                                }
                            }
                        }
                    }
                }
            };

        var jsonData = JsonConvert.SerializeObject(data);
        return jsonData;
    }

    public string getTidyRules(List<InputRule> rules)
    {
        var tidyRules = new List<Rule>();

        foreach (var inputRule in rules)
        {
            var configurations = new List<Configuration>();

            foreach (var inputConfiguration in inputRule.configurations!)
            {
                var configuration = new Configuration
                {
                    name = inputConfiguration.name!,
                    weight = double(inputConfiguration.weight),
                    description = inputConfiguration.description!
                };

                if (inputConfiguration.limitInt != null)
                {
                    configuration.limitInt = inputConfiguration.limitInt!;
                }
                else if (inputConfiguration.limitISO != null)
                {
                    configuration.limitISO = inputConfiguration.limitISO!;
                }
                else
                {
                    configuration.regex = inputConfiguration.regex!;
                }

                configurations.Add(configuration);
            }

            var rule = new Rule
            {
                name = inputRule.name!,
                configurations = configurations
            };

            tidyRules.Add(rule);
        }

        var data = new TidyRule
        {
            rules = tidyRules
        };

        var jsonData = JsonConvert.SerializeObject(data);
        return jsonData;
    }


}