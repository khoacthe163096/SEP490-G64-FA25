using AutoMapper;
using BE.vn.fpt.edu.DTOs.Feedback;
using BE.vn.fpt.edu.interfaces;
using Microsoft.AspNetCore.Mvc;

namespace BE.vn.fpt.edu.controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FeedbackController : ControllerBase
    {
        private readonly IFeedbackService _feedbackService;
        private readonly IMapper _mapper;

        public FeedbackController(IFeedbackService feedbackService, IMapper mapper)
        {
            _feedbackService = feedbackService;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var feedbacks = await _feedbackService.GetAllAsync();
            return Ok(feedbacks);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(long id)
        {
            var feedback = await _feedbackService.GetByIdAsync(id);
            if (feedback == null) return NotFound();
            return Ok(feedback);
        }

        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetByUserId(long userId)
        {
            var feedbacks = await _feedbackService.GetByUserIdAsync(userId);
            return Ok(feedbacks);
        }

        [HttpGet("ticket/{ticketId}")]
        public async Task<IActionResult> GetByTicketId(long ticketId)
        {
            var feedbacks = await _feedbackService.GetByTicketIdAsync(ticketId);
            return Ok(feedbacks);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] RequestDto request)
        {
            var created = await _feedbackService.CreateAsync(request);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(long id, [FromBody] RequestDto request)
        {
            var updated = await _feedbackService.UpdateAsync(id, request);
            if (updated == null) return NotFound();
            return Ok(updated);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(long id)
        {
            var result = await _feedbackService.DeleteAsync(id);
            if (!result) return NotFound();
            return NoContent();
        }
    }
}
