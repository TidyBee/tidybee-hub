using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using DataProcessing.Models;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
        // Pre-allocate memory for the overviews list
        List<Overview> overviews = new List<Overview>(files.Count);

        // Use Parallel.ForEach for concurrent processing
        Parallel.ForEach(files, file =>
        {
            var dateTimeOffset = new DateTimeOffset(file.LastModified);
            var secsSinceEpoch = dateTimeOffset.ToUnixTimeSeconds();
            var nanosSinceEpoch = (dateTimeOffset.Ticks % TimeSpan.TicksPerSecond) * 100;

            var overview = new Overview
            {
                pretty_path = file.Name,
                size = file.Size,
                provenance = file.provenance,
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

            // Thread-safe addition to the list
            lock (overviews)
            {
                overviews.Add(overview);
            }
        });

        var jsonData = JsonConvert.SerializeObject(overviews);
        return jsonData;
    }


    public string getOverviewMisnamed(List<DataProcessing.Models.Input.File> files, List<DataProcessing.Models.Input.Rule> rules)
    {
        // Pre-allocate memory for the overviews list
        List<Overview> overviews = new List<Overview>(files.Count);

        // Cache the rule to avoid repeated LINQ queries
        var misnamedRule = rules.FirstOrDefault(r => r.Name == "misnamed");
        if (misnamedRule == null)
        {
            throw new InvalidOperationException("Misnamed rule not found");
        }

        dynamic temp = JsonConvert.DeserializeObject(misnamedRule.RulesConfig);

        // Use Parallel.ForEach for concurrent processing
        Parallel.ForEach(files, file =>
        {
            if (file.MisnamedScore != 'A')
            {
                var configurations = new List<Configuration>();

                if (temp?.regex_rules != null)
                {
                    foreach (var inputConfiguration in temp.regex_rules)
                    {
                        var configuration = new Configuration
                        {
                            name = inputConfiguration.name,
                            weight = inputConfiguration.weight,
                            description = inputConfiguration.description,
                            grade = file.MisnamedScore == 'U' ? 'E' : file.MisnamedScore
                        };

                        if (inputConfiguration.max_occurrences != null)
                        {
                            configuration.limitInt = inputConfiguration.max_occurrences;
                        }
                        else if (inputConfiguration.expiration_days != null)
                        {
                            configuration.limitISO = inputConfiguration.expiration_days;
                        }
                        else
                        {
                            configuration.regex = inputConfiguration.regex;
                        }

                        configurations.Add(configuration);
                    }
                }
                else
                {
                    var configuration = new Configuration
                    {
                        name = misnamedRule.Name,
                        description = misnamedRule.Description,
                        weight = misnamedRule.Weight,
                    };
                    if (temp?.max_occurrences != null)
                    {
                        configuration.limitInt = temp.max_occurrences;
                    }
                    else if (temp?.expiration_days != null)
                    {
                        configuration.limitISO = temp.expiration_days;
                    }
                    configurations.Add(configuration);
                }

                var overview = new Overview
                {
                    pretty_path = file.Name,
                    size = file.Size,
                    provenance = file.provenance,
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

                // Thread-safe addition to the list
                lock (overviews)
                {
                    overviews.Add(overview);
                }
            }
        });

        var jsonData = JsonConvert.SerializeObject(overviews);
        return jsonData;
    }


    public string getOverviewDuplicate(List<DataProcessing.Models.Input.File> files, List<DataProcessing.Models.Input.Rule> rules)
    {
        // Pre-allocate memory for the overviews list
        List<Overview> overviews = new List<Overview>(files.Count);

        // Cache the rule to avoid repeated LINQ queries
        var duplicatedRule = rules.FirstOrDefault(r => r.Name == "duplicated");
        if (duplicatedRule == null)
        {
            throw new InvalidOperationException("Duplicated rule not found");
        }

        dynamic temp = JsonConvert.DeserializeObject(duplicatedRule.RulesConfig);

        // Use Parallel.ForEach for concurrent processing
        Parallel.ForEach(files, file =>
        {
            if (file.DuplicatedScore != 'A')
            {
                var configurations = new List<Configuration>();

                if (temp?.regex_rules != null)
                {
                    foreach (var inputConfiguration in temp.regex_rules)
                    {
                        var configuration = new Configuration
                        {
                            name = inputConfiguration.name,
                            weight = inputConfiguration.weight,
                            description = inputConfiguration.description,
                            grade = file.DuplicatedScore == 'U' ? 'E' : file.DuplicatedScore
                        };

                        if (inputConfiguration.max_occurrences != null)
                        {
                            configuration.limitInt = inputConfiguration.max_occurrences;
                        }
                        else if (inputConfiguration.expiration_days != null)
                        {
                            configuration.limitISO = inputConfiguration.expiration_days;
                        }
                        else
                        {
                            configuration.regex = inputConfiguration.regex;
                        }

                        configurations.Add(configuration);
                    }
                }
                else
                {
                    var configuration = new Configuration
                    {
                        name = duplicatedRule.Name,
                        description = duplicatedRule.Description,
                        weight = duplicatedRule.Weight,
                    };
                    if (temp?.max_occurrences != null)
                    {
                        configuration.limitInt = temp.max_occurrences;
                    }
                    else if (temp?.expiration_days != null)
                    {
                        configuration.limitISO = temp.expiration_days;
                    }
                    configurations.Add(configuration);
                }

                var overview = new Overview
                {
                    pretty_path = file.Name,
                    provenance = file.provenance,
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

                // Thread-safe addition to the list
                lock (overviews)
                {
                    overviews.Add(overview);
                }
            }
        });

        var jsonData = JsonConvert.SerializeObject(overviews);
        return jsonData;
    }

    public string getOverviewUnused(List<DataProcessing.Models.Input.File> files, List<DataProcessing.Models.Input.Rule> rules)
    {
        // Pre-allocate memory for the overviews list
        List<Overview> overviews = new List<Overview>(files.Count);

        // Cache the rule to avoid repeated LINQ queries
        var perishedRule = rules.FirstOrDefault(r => r.Name == "perished");
        if (perishedRule == null)
        {
            throw new InvalidOperationException("Perished rule not found");
        }

        dynamic temp = JsonConvert.DeserializeObject(perishedRule.RulesConfig);

        // Use Parallel.ForEach for concurrent processing
        Parallel.ForEach(files, file =>
        {
            if (file.PerishedScore != 'A')
            {
                var configurations = new List<Configuration>();

                if (temp?.regex_rules != null)
                {
                    foreach (var inputConfiguration in temp.regex_rules)
                    {
                        var configuration = new Configuration
                        {
                            name = inputConfiguration.name,
                            weight = inputConfiguration.weight,
                            description = inputConfiguration.description,
                            grade = file.PerishedScore == 'U' ? 'E' : file.PerishedScore
                        };

                        if (inputConfiguration.max_occurrences != null)
                        {
                            configuration.limitInt = inputConfiguration.max_occurrences;
                        }
                        else if (inputConfiguration.expiration_days != null)
                        {
                            configuration.limitISO = inputConfiguration.expiration_days;
                        }
                        else
                        {
                            configuration.regex = inputConfiguration.regex;
                        }

                        configurations.Add(configuration);
                    }
                }
                else
                {
                    var configuration = new Configuration
                    {
                        name = perishedRule.Name,
                        description = perishedRule.Description,
                        weight = perishedRule.Weight,
                    };
                    if (temp?.max_occurrences != null)
                    {
                        configuration.limitInt = temp.max_occurrences;
                    }
                    else if (temp?.expiration_days != null)
                    {
                        configuration.limitISO = temp.expiration_days;
                    }
                    configurations.Add(configuration);
                }

                var overview = new Overview
                {
                    pretty_path = file.Name,
                    provenance = file.provenance,
                    size = file.Size,
                    last_modified = new LastModified
                    {
                        secs_since_epoch = ((DateTimeOffset)file.LastModified).ToUnixTimeSeconds(),
                        nanos_since_epoch = file.LastModified.Millisecond * 1000000
                    },
                    tidy_score = new TidyScore
                    {
                        grade = file.GlobalScore == 'U' ? 'E' : file.GlobalScore,
                        unused = new Unused
                        {
                            grade = file.PerishedScore == 'U' ? 'E' : file.PerishedScore,
                            configurations = configurations
                        }
                    }
                };

                // Thread-safe addition to the list
                lock (overviews)
                {
                    overviews.Add(overview);
                }
            }
        });

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