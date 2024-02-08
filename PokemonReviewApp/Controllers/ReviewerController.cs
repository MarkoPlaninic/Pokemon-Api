using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using PokemonReviewApp.Dto;
using PokemonReviewApp.Interfaces;
using PokemonReviewApp.Models;

namespace PokemonReviewApp.Controllers;
[Route("api/[controller]")]
[ApiController]
public class ReviewerController: Controller
{
    private readonly IReviewerRepository _reviewerRepository;
    private readonly IMapper _mapper;
 


    public ReviewerController(IReviewerRepository reviewerRepository, IMapper mapper)
    {
        _reviewerRepository = reviewerRepository;
        _mapper = mapper;
    }
    
    [HttpGet]
    [ProducesResponseType(200, Type = typeof(IEnumerable<Reviewer>))]
    public IActionResult GetReviewers()
    {
        var reviewers = _mapper.Map<List<ReviewerDto>>(_reviewerRepository.GetReviewers());
        if (!ModelState.IsValid)
            return BadRequest(ModelState);
        return Ok(reviewers);
    }

    [HttpGet("{reviewerId}")]
    [ProducesResponseType(200, Type = typeof(Reviewer))]
    public IActionResult GetReviewer(int reviewerId)
    {
        if (!_reviewerRepository.ReviewerExists(reviewerId))
            return NotFound();
        var reviewer = _mapper.Map<ReviewerDto>(_reviewerRepository.GetReviewer(reviewerId));

        if (!ModelState.IsValid)
            return BadRequest(ModelState);
        return Ok(reviewer);
    }
    [HttpGet("reviews/{reviewerId}")]
    [ProducesResponseType(200, Type = typeof(IEnumerable<Review>))]
    [ProducesResponseType(400)]
    public IActionResult GetReviewsByReviewer(int reviewerId)
    {     
        if (!_reviewerRepository.ReviewerExists(reviewerId))
            return NotFound();
        var reviews= _mapper.Map<List<ReviewDto>>(
            _reviewerRepository.GetReviewsByReviewer(reviewerId));
            
        if (!ModelState.IsValid)
            return BadRequest(ModelState);
        return Ok(reviews);
    }
    
    [HttpPost]
    [ProducesResponseType(204)]
    [ProducesResponseType(400)]
    public IActionResult CreateReviewer([FromBody] ReviewerDto reviewerCreate)
    {
        if (reviewerCreate == null)
            return BadRequest(ModelState);
        var reviewer = _reviewerRepository
            .GetReviewers()
            .FirstOrDefault(c => c.FirstName.Trim().ToUpper()== reviewerCreate.LastName.TrimEnd().ToUpper());

        if (reviewer != null)
        {
            ModelState.AddModelError("", "Reviewer already exists");
            return StatusCode(422, ModelState);
        }
        if  (!ModelState.IsValid)
            return BadRequest(ModelState);
        var reviewerMap = _mapper.Map<Reviewer>(reviewerCreate);
        if (!_reviewerRepository.CreateReviewer(reviewerMap))
        {
            ModelState.AddModelError("", "Something went wrong with saving reviewer");
            return StatusCode(500, ModelState);
        }
        return Ok("Successfully created reviewer");
    }
    
    [HttpPut]
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]
    [ProducesResponseType(204)]
    public IActionResult UpdateReviewer([FromQuery] int reviewerId, [FromBody] ReviewerDto updatedReviewer)

    {
        if (updatedReviewer == null)
            return BadRequest(ModelState);
        if (reviewerId != updatedReviewer.Id)
            return BadRequest(ModelState);
        if (!_reviewerRepository.ReviewerExists(reviewerId))
            return NotFound();
        if (!ModelState.IsValid)
            return BadRequest();
        var reviewerMap = _mapper.Map<Reviewer>(updatedReviewer);
        if (!_reviewerRepository.UpdateReviewer(reviewerMap))
        {
            ModelState.AddModelError("","Something went wrong while updating reviewer");
            return StatusCode(500, ModelState);
        }

        return NoContent();
    }
    
    [HttpDelete]
    [ProducesResponseType(204)]
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]
    public IActionResult DeleteReviewer (int reviewerId)
    {
        if (!_reviewerRepository.ReviewerExists(reviewerId))
            return NotFound();
        if (!ModelState.IsValid)
            return BadRequest(ModelState);
        
        var reviewer = _reviewerRepository.GetReviewer(reviewerId);
        if (!_reviewerRepository.DeleteReviewer(reviewer))
        {
            ModelState.AddModelError("", "Something went wrong while deleting reviewer");
            return StatusCode(500, ModelState);
        }
        return NoContent();
    }
    
}