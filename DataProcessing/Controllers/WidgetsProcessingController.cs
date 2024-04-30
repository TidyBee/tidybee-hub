using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using DataProcessing.Models;

namespace WidgetController.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WidgetController : ControllerBase
    {
        [HttpGet("getTextWidgetunused")]
        public IActionResult GetTextWidgetunused()
        {
            var data = new
            {
                title = "unused",
                types = "Number",
                data = new
                {
                    percentage = "+8",
                    value = "408",
                    status = false
                }
            };
            var jsonData = JsonConvert.SerializeObject(data);
            return Ok(jsonData);
        }

        [HttpGet("getGradeWidget")]
        public IActionResult getGradeWidget()
        {
            var data = new
            {
                grade = "B"
            };
            var jsonData = JsonConvert.SerializeObject(data);
            return Ok(jsonData);
        }

        [HttpGet("getTotalMonitored")]
        public IActionResult getTotalMonitored()
        {
            var data = new {
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
            return Ok(jsonData);
        }

        [HttpGet("getGraphWidget")]
        public IActionResult getGraphWidget()
        {
            var data = new
            {
                series = new[] { 20, 32, 23, 15, 10 }
            };
            var jsonData = JsonConvert.SerializeObject(data);
            return Ok(jsonData);
        }

        [HttpGet("getTextWidgetbadname")]
        public IActionResult GetTextWidgetbadname()
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
            return Ok(jsonData);
        }

        [HttpGet("getTextWidgetduplicate")]
        public IActionResult GetTextWidgetduplicate()
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
            return Ok(jsonData);
        }

        [HttpGet("getTextWidgetstorage")]
        public IActionResult GetTextWidgetstorage()
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
            return Ok(jsonData);
        }


        [HttpGet("getOverviewAll")]
        public IActionResult GetOverviewAll()
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
            return Ok(jsonData);
        }

        [HttpGet("getOverviewMisnamed")]
        public IActionResult GetOverviewMisnamed()
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
            return Ok(jsonData);
        }

        [HttpGet("getOverviewDuplicate")]
        public IActionResult GetOverviewDuplicate()
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
            return Ok(jsonData);
        }

        [HttpGet("getOverviewUnused")]
        public IActionResult GetOverviewUnused()
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
            return Ok(jsonData);
        }

        [HttpGet("getTidyRules")]
        public IActionResult GetTidyRules()
        {
            var data = new TidyRule
            {
                rules = new List<Rule>
                {
                    new Rule
                    {
                        name = "misnamed",
                        configurations = new List<Configuration>
                        {
                            new Configuration
                            {
                                name = "date",
                                weight = 3,
                                description = "The name of the file need to have a date",
                                regex = @"r'_\d{4}\.'"
                            },
                            new Configuration
                            {
                                name = "valid separator",
                                weight = 1.8,
                                description = "The name of the file need to have 4 separators _",
                                regex = @"r'^[^_]*(_[^_]*){3}$'"
                            }
                        }
                    },
                    new Rule
                    {
                        name = "duplicate",
                        configurations = new List<Configuration>
                        {
                            new Configuration
                            {
                                name = "occurrence",
                                weight = 1,
                                description = "The file need to be unique",
                                limitInt = 1
                            }
                        }
                    },
                    new Rule
                    {
                        name = "unused",
                        configurations = new List<Configuration>
                        {
                            new Configuration
                            {
                                name = "perished",
                                weight = 1,
                                description = "The file need to be recent enough",
                                limitISO = "2024-04-12T00:00:00Z"
                            }
                        }
                    }
                }
            };

            var jsonData = JsonConvert.SerializeObject(data);
            return Ok(jsonData);
        }
    }
}