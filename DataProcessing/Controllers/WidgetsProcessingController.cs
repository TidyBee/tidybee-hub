using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using DataProcessing.Models;

namespace WidgetController.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WidgetController : ControllerBase
    {
        private OutputService _outputService;
        private InputService _inputService;

        public WidgetController(OutputService outputService, InputService inputService)
        {
            _outputService = outputService;
            _inputService = inputService;
        }

        [HttpGet("getTextWidgetunused")]
        public IActionResult GetTextWidgetunused()
        {
            return Ok(_outputService.getTextWidgetUnused());
        }

        [HttpGet("getGradeWidget")]
        public async Task<IActionResult> getGradeWidget()
        {
            return Ok(_outputService.getGradeWidget(await _inputService.getFiles()));
        }

        [HttpGet("getTotalMonitored")]
        public IActionResult getTotalMonitored()
        {
            return Ok(_outputService.getTotalMonitored());
        }

        [HttpGet("getGraphWidget")]
        public async Task<IActionResult> getGraphWidget()
        {
            return Ok(_outputService.getGraphWidget(await _inputService.getFiles()));
        }

        [HttpGet("getTextWidgetbadname")]
        public IActionResult GetTextWidgetbadname()
        {
            return Ok(_outputService.getTextWidgetbadname());
        }

        [HttpGet("getTextWidgetduplicate")]
        public IActionResult GetTextWidgetduplicate()
        {
            return Ok(_outputService.getTextWidgetduplicate());
        }

        [HttpGet("getTextWidgetstorage")]
        public IActionResult GetTextWidgetstorage()
        {
            return Ok(_outputService.getTextWidgetstorage());
        }


        [HttpGet("getOverviewAll")]
        public async Task<IActionResult> GetOverviewAll()
        {
            return Ok(_outputService.getOverviewAll(await _inputService.getFiles()));
        }

        [HttpGet("getOverviewMisnamed")]
        public IActionResult GetOverviewMisnamed()
        {
            return Ok(_outputService.getOverviewMisnamed());
        }

        [HttpGet("getOverviewDuplicate")]
        public IActionResult GetOverviewDuplicate()
        {
            return Ok(_outputService.getOverviewDuplicate());
        }

        [HttpGet("getOverviewUnused")]
        public IActionResult GetOverviewUnused()
        {
            return Ok(_outputService.getOverviewUnused());
        }

        [HttpGet("getTidyRules")]
        public async Task<IActionResult> GetTidyRules()
        {
            return Ok(_outputService.getTidyRules(await _inputService.getRules()));
        }
    }
}