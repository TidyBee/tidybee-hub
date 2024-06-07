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
        public async Task<IActionResult> GetTextWidgetunused()
        {
            return Ok(_outputService.getTextWidgetUnused(await _inputService.getFiles()));
        }

        [HttpGet("getGradeWidget")]
        public async Task<IActionResult> getGradeWidget()
        {
            return Ok(_outputService.getGradeWidget(await _inputService.getFiles()));
        }

        [HttpGet("getTotalMonitored")]
        public async Task<IActionResult> getTotalMonitored()
        {
            return Ok(_outputService.getTotalMonitored(await _inputService.getFiles()));
        }

        [HttpGet("getGraphWidget")]
        public async Task<IActionResult> getGraphWidget()
        {
            return Ok(_outputService.getGraphWidget(await _inputService.getFiles()));
        }

        [HttpGet("getTextWidgetbadname")]
        public async Task<IActionResult> GetTextWidgetbadname()
        {
            return Ok(_outputService.getTextWidgetbadname(await _inputService.getFiles()));
        }

        [HttpGet("getTextWidgetduplicate")]
        public async Task<IActionResult> GetTextWidgetduplicate()
        {
            return Ok(_outputService.getTextWidgetduplicate(await _inputService.getFiles()));
        }

        [HttpGet("getTextWidgetstorage")]
        public async Task<IActionResult> GetTextWidgetstorage()
        {
            return Ok(_outputService.getTextWidgetstorage(await _inputService.getFiles()));
        }


        [HttpGet("getOverviewAll")]
        public async Task<IActionResult> GetOverviewAll()
        {
            return Ok(_outputService.getOverviewAll(await _inputService.getFiles()));
        }

        [HttpGet("getOverviewMisnamed")]
        public async Task<IActionResult> GetOverviewMisnamed()
        {
            return Ok(_outputService.getOverviewMisnamed(await _inputService.getFiles(), await _inputService.getRules()));
        }

        [HttpGet("getOverviewDuplicate")]
        public async Task<IActionResult> GetOverviewDuplicate()
        {
            return Ok(_outputService.getOverviewDuplicate(await _inputService.getFiles(), await _inputService.getRules()));
        }

        [HttpGet("getOverviewUnused")]
        public async Task<IActionResult> GetOverviewUnused()
        {
            return Ok(_outputService.getOverviewUnused(await _inputService.getFiles(), await _inputService.getRules()));
        }

        [HttpGet("getTidyRules")]
        public async Task<IActionResult> GetTidyRules()
        {
            return Ok(_outputService.getTidyRules(await _inputService.getRules()));
        }
    }
}