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

    public string getTidyRules(List<DataProcessing.Models.Input.Rule> rules)
    {
        var tidyRules = new List<Rule>();

        foreach (var inputRule in rules)
        {
            var configurations = new List<Configuration>();
            dynamic temp = JsonConvert.DeserializeObject(inputRule.RulesConfig!)!;

            if (temp!.regex_rules != null) {
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
            } else {
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