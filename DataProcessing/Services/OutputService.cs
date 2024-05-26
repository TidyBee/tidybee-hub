using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using DataProcessing.Models;

public class OutputService
{
    public OutputService()
    {

    }

    public string getTextWidgetUnused(List<DataProcessing.Models.Input.File> files)
    {
        var data = new
        {
            title = "unused",
            types = "Number",
            data = new
            {
                percentage = "+0",
                value = files.Count(file => file.PerishedScore != 'A'),
                status = true
            }
        };
        var jsonData = JsonConvert.SerializeObject(data);
        return jsonData;
    }

    public string getGradeWidget(List<DataProcessing.Models.Input.File> files)
    {
        Dictionary<char, int> gradeToValue = new Dictionary<char, int>
        {
            { 'A', 5 },
            { 'B', 4 },
            { 'C', 3 },
            { 'D', 2 },
            { 'E', 1 },
            { 'U', 1 } // Treat 'U' as 'E'
        };

        Dictionary<int, char> valueToGrade = new Dictionary<int, char>
        {
            { 5, 'A' },
            { 4, 'B' },
            { 3, 'C' },
            { 2, 'D' },
            { 1, 'E' }
        };

        List<int> numericalScores = files.Select(file => gradeToValue[file.GlobalScore]).ToList();

        double averageValue = numericalScores.Average();

        int roundedAverageValue = (int)Math.Round(averageValue);


        var data = new
        {
            grade = valueToGrade[roundedAverageValue]
        };
        var jsonData = JsonConvert.SerializeObject(data);
        return jsonData;
    }

    public string getTotalMonitored(List<DataProcessing.Models.Input.File> files)
    {
        var data = new
        {
            title = "total",
            types = "Number",
            data = new
            {
                percentage = "+0",
                value = files.Count,
                status = true
            }
        };
        var jsonData = JsonConvert.SerializeObject(data);
        return jsonData;
    }

    public string getGraphWidget(List<DataProcessing.Models.Input.File> files)
    {
        List<char> adjustedFiles = files.Select(file => file.GlobalScore == 'U' ? 'E' : file.GlobalScore).ToList();

        Dictionary<char, int> gradeCounts = new Dictionary<char, int>
        {
            { 'A', 0 },
            { 'B', 0 },
            { 'C', 0 },
            { 'D', 0 },
            { 'E', 0 }
        };

        foreach (char file in adjustedFiles)
        {
            if (gradeCounts.ContainsKey(file))
            {
                gradeCounts[file]++;
            }
        }

        int totalCount = adjustedFiles.Count;

        double[] gradePercentages = new double[5];
        char[] grades = new[] { 'A', 'B', 'C', 'D', 'E' };
        for (int i = 0; i < grades.Length; i++)
        {
            char grade = grades[i];
            gradePercentages[i] = (double)gradeCounts[grade] / totalCount * 100;
        }
        var data = new
        {
            series = gradePercentages
        };
        var jsonData = JsonConvert.SerializeObject(data);
        return jsonData;
    }

    public string getTextWidgetbadname(List<DataProcessing.Models.Input.File> files)
    {
        var data = new
        {
            title = "badname",
            types = "Number",
            data = new
            {
                percentage = "+0",
                value = files.Count(file => file.MisnamedScore != 'A'),
                status = true
            }
        };
        var jsonData = JsonConvert.SerializeObject(data);
        return jsonData;
    }

    public string getTextWidgetduplicate(List<DataProcessing.Models.Input.File> files)
    {
        var data = new
        {
            title = "duplicate",
            types = "Number",
            data = new
            {
                percentage = "+0",
                value = files.Count(file => file.DuplicatedScore != 'A'),
                status = true
            }
        };
        var jsonData = JsonConvert.SerializeObject(data);
        return jsonData;
    }

    public string getTextWidgetstorage(List<DataProcessing.Models.Input.File> files)
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

    public string getOverviewAll(List<DataProcessing.Models.Input.File> files)
    {
        List<Overview> overviews = new List<Overview>();

        foreach (var file in files)
        {
            var dateTimeOffset = new DateTimeOffset(file.LastModified);
            var secsSinceEpoch = dateTimeOffset.ToUnixTimeSeconds();
            var nanosSinceEpoch = (dateTimeOffset.Ticks % TimeSpan.TicksPerSecond) * 100;

            var overview = new Overview
            {
                pretty_path = file.Name,
                size = file.Size,
                last_modified = new LastModified
                {
                    secs_since_epoch = secsSinceEpoch,
                    nanos_since_epoch = (int)nanosSinceEpoch
                },
                tidy_score = new TidyScore
                {
                    grade = file.GlobalScore == 'U' ? 'E' : file.GlobalScore,
                    misnamed = new Misnamed
                    {
                        grade = file.MisnamedScore == 'U' ? 'E' : file.MisnamedScore
                    },
                    unused = new Unused
                    {
                        grade = file.PerishedScore == 'U' ? 'E' : file.PerishedScore
                    },
                    duplicated = new Duplicated
                    {
                        grade = file.DuplicatedScore == 'U' ? 'E' : file.DuplicatedScore
                    }
                }
            };

            overviews.Add(overview);
        }

        var jsonData = JsonConvert.SerializeObject(overviews);
        return jsonData;
    }

    public string getOverviewMisnamed(List<DataProcessing.Models.Input.File> files, List<DataProcessing.Models.Input.Rule> rules)
    {
        var overviews = new List<Overview>();

        foreach (var file in files)
        {
            if (file.MisnamedScore != 'A')
            {
                var rule = rules
                    .Where(r => r.Name == "misnamed").First()!;

                var configurations = new List<Configuration>();
                dynamic temp = JsonConvert.DeserializeObject(rule.RulesConfig!)!;

                if (temp!.regex_rules != null)
                {
                    foreach (var inputConfiguration in temp.regex_rules)
                    {
                        var configuration = new Configuration
                        {
                            name = inputConfiguration.name!,
                            weight = inputConfiguration.weight!,
                            description = inputConfiguration.description!,
                            grade = file.MisnamedScore == 'U' ? 'E' : file.MisnamedScore
                        };

                        if (inputConfiguration.max_occurrences != null)
                        {
                            configuration.limitInt = inputConfiguration.max_occurrences!;
                        }
                        else if (inputConfiguration.expiration_days != null)
                        {
                            configuration.limitISO = inputConfiguration.expiration_days!;
                        }
                        else
                        {
                            configuration.regex = inputConfiguration.regex!;
                        }

                        configurations.Add(configuration);
                    }
                }
                else
                {
                    var configuration = new Configuration
                    {
                        name = rule.Name!,
                        description = rule.Description!,
                        weight = rule.Weight!,
                    };
                    if (temp.max_occurrences != null)
                    {
                        configuration.limitInt = temp.max_occurrences!;
                    }
                    else if (temp.expiration_days != null)
                    {
                        configuration.limitISO = temp.expiration_days!;
                    }
                    configurations.Add(configuration);
                }

                var overview = new Overview
                {
                    pretty_path = file.Name,
                    size = file.Size,
                    last_modified = new LastModified
                    {
                        secs_since_epoch = ((DateTimeOffset)file.LastModified).ToUnixTimeSeconds(),
                        nanos_since_epoch = file.LastModified.Millisecond * 1000000
                    },
                    tidy_score = new TidyScore
                    {
                        grade = file.GlobalScore == 'U' ? 'E' : file.GlobalScore,
                        misnamed = new Misnamed
                        {
                            grade = file.MisnamedScore == 'U' ? 'E' : file.MisnamedScore,
                            configurations = configurations
                        }
                    }
                };

                overviews.Add(overview);
            }
        }

        var jsonData = JsonConvert.SerializeObject(overviews);
        return jsonData;
    }

    public string getOverviewDuplicate(List<DataProcessing.Models.Input.File> files, List<DataProcessing.Models.Input.Rule> rules)
    {
        var overviews = new List<Overview>();

        foreach (var file in files)
        {
            if (file.DuplicatedScore != 'A')
            {
                var rule = rules
                    .Where(r => r.Name == "duplicated").First()!;

                var configurations = new List<Configuration>();
                dynamic temp = JsonConvert.DeserializeObject(rule.RulesConfig!)!;

                if (temp!.regex_rules != null)
                {
                    foreach (var inputConfiguration in temp.regex_rules)
                    {
                        var configuration = new Configuration
                        {
                            name = inputConfiguration.name!,
                            weight = inputConfiguration.weight!,
                            description = inputConfiguration.description!,
                            grade = file.DuplicatedScore == 'U' ? 'E' : 'E'
                        };

                        if (inputConfiguration.max_occurrences != null)
                        {
                            configuration.limitInt = inputConfiguration.max_occurrences!;
                        }
                        else if (inputConfiguration.expiration_days != null)
                        {
                            configuration.limitISO = inputConfiguration.expiration_days!;
                        }
                        else
                        {
                            configuration.regex = inputConfiguration.regex!;
                        }

                        configurations.Add(configuration);
                    }
                }
                else
                {
                    var configuration = new Configuration
                    {
                        name = rule.Name!,
                        description = rule.Description!,
                        weight = rule.Weight!,
                    };
                    if (temp.max_occurrences != null)
                    {
                        configuration.limitInt = temp.max_occurrences!;
                    }
                    else if (temp.expiration_days != null)
                    {
                        configuration.limitISO = temp.expiration_days!;
                    }
                    configurations.Add(configuration);
                }

                var overview = new Overview
                {
                    pretty_path = file.Name,
                    size = file.Size,
                    last_modified = new LastModified
                    {
                        secs_since_epoch = ((DateTimeOffset)file.LastModified).ToUnixTimeSeconds(),
                        nanos_since_epoch = file.LastModified.Millisecond * 1000000
                    },
                    tidy_score = new TidyScore
                    {
                        grade = file.GlobalScore == 'U' ? 'E' : file.GlobalScore,
                        duplicated = new Duplicated
                        {
                            grade = file.DuplicatedScore == 'U' ? 'E' : file.DuplicatedScore,
                            configurations = configurations
                        }
                    }
                };

                overviews.Add(overview);
            }
        }

        var jsonData = JsonConvert.SerializeObject(overviews);
        return jsonData;
    }

    public string getOverviewUnused(List<DataProcessing.Models.Input.File> files, List<DataProcessing.Models.Input.Rule> rules)
    {
        var overviews = new List<Overview>();

        foreach (var file in files)
        {
            if (file.PerishedScore != 'A')
            {
                var rule = rules
                    .Where(r => r.Name == "perished").First()!;

                var configurations = new List<Configuration>();
                dynamic temp = JsonConvert.DeserializeObject(rule.RulesConfig!)!;

                if (temp!.regex_rules != null)
                {
                    foreach (var inputConfiguration in temp.regex_rules)
                    {
                        var configuration = new Configuration
                        {
                            name = inputConfiguration.name!,
                            weight = inputConfiguration.weight!,
                            description = inputConfiguration.description!,
                            grade = file.PerishedScore == 'U' ? 'E' : 'E'
                        };

                        if (inputConfiguration.max_occurrences != null)
                        {
                            configuration.limitInt = inputConfiguration.max_occurrences!;
                        }
                        else if (inputConfiguration.expiration_days != null)
                        {
                            configuration.limitISO = inputConfiguration.expiration_days!;
                        }
                        else
                        {
                            configuration.regex = inputConfiguration.regex!;
                        }

                        configurations.Add(configuration);
                    }
                }
                else
                {
                    var configuration = new Configuration
                    {
                        name = rule.Name!,
                        description = rule.Description!,
                        weight = rule.Weight!,
                    };
                    if (temp.max_occurrences != null)
                    {
                        configuration.limitInt = temp.max_occurrences!;
                    }
                    else if (temp.expiration_days != null)
                    {
                        configuration.limitISO = temp.expiration_days!;
                    }
                    configurations.Add(configuration);
                }

                var overview = new Overview
                {
                    pretty_path = file.Name,
                    size = file.Size,
                    last_modified = new LastModified
                    {
                        secs_since_epoch = ((DateTimeOffset)file.LastModified).ToUnixTimeSeconds(),
                        nanos_since_epoch = file.LastModified.Millisecond * 1000000
                    },
                    tidy_score = new TidyScore
                    {
                        grade = file.GlobalScore == 'U' ? 'E' : file.PerishedScore,
                        unused = new Unused
                        {
                            grade = file.PerishedScore == 'U' ? 'E' : file.PerishedScore,
                            configurations = configurations
                        }
                    }
                };

                overviews.Add(overview);
            }
        }

        var jsonData = JsonConvert.SerializeObject(overviews);
        return jsonData;
    }

    public string getTidyRules(List<DataProcessing.Models.Input.Rule> rules)
    {
        var tidyRules = new List<Rule>();

        foreach (var inputRule in rules)
        {
            var configurations = new List<Configuration>();
            dynamic temp = JsonConvert.DeserializeObject(inputRule.RulesConfig!)!;

            if (temp!.regex_rules != null)
            {
                foreach (var inputConfiguration in temp.regex_rules)
                {
                    var configuration = new Configuration
                    {
                        name = inputConfiguration.name!,
                        weight = inputConfiguration.weight!,
                        description = inputConfiguration.description!
                    };

                    if (inputConfiguration.max_occurrences != null)
                    {
                        configuration.limitInt = inputConfiguration.max_occurrences!;
                    }
                    else if (inputConfiguration.expiration_days != null)
                    {
                        configuration.limitISO = inputConfiguration.expiration_days!;
                    }
                    else
                    {
                        configuration.regex = inputConfiguration.regex!;
                    }

                    configurations.Add(configuration);
                }
            }
            else
            {
                var configuration = new Configuration
                {
                    name = inputRule.Name!,
                    description = inputRule.Description!,
                    weight = inputRule.Weight!,
                };
                if (temp.max_occurrences != null)
                {
                    configuration.limitInt = temp.max_occurrences!;
                }
                else if (temp.expiration_days != null)
                {
                    configuration.limitISO = temp.expiration_days!;
                }
                configurations.Add(configuration);
            }

            var rule = new DataProcessing.Models.Rule
            {
                name = inputRule.Name!,
                description = inputRule.Description!,
                weight = inputRule.Weight!,
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