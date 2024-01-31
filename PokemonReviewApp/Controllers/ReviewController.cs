using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using PokemonReviewApp.Interfaces;
using PokemonReviewApp.Models;
using PokemonReviewApp.Dto;

namespace PokemonReviewApp.Controllers;
[Route("api/[controller]")]
[ApiController]
public class ReviewController :Controller
{
    private readonly IReviewRepository _reviewRepository;
    private readonly IMapper _mapper;
    private readonly IReviewerRepository _reviewerRepository;
    private readonly IPokemonRepository _pokemonRepository;

    public ReviewController(IPokemonRepository pokemonRepository,IReviewerRepository reviewerRepository,IReviewRepository reviewRepository, IMapper mapper)
    {
        _reviewRepository = reviewRepository;
        _reviewerRepository = reviewerRepository;
        _pokemonRepository = pokemonRepository;
        _mapper = mapper;
    }
    
    [HttpGet]
    [ProducesResponseType(200, Type = typeof(IEnumerable<Review>))]
    public IActionResult GetReviews()
    {
        var reviews = _mapper.Map<List<ReviewDto>>(_reviewRepository.GetReviews());
        if (!ModelState.IsValid)
            return BadRequest(ModelState);
        return Ok(reviews);
    }
    
    [HttpGet("{reviewId}")]
    [ProducesResponseType(200, Type = typeof(Review))]
    public IActionResult GetReview(int reviewId)
    {
        if (!_reviewRepository.ReviewExists(reviewId))
            return NotFound();
        var review = _mapper.Map<ReviewDto>(_reviewRepository.GetReview(reviewId));
        
        if (!ModelState.IsValid)
            return BadRequest(ModelState);
        return Ok(review);
    }
    [HttpPost]
    [ProducesResponseType(204)]
    [ProducesResponseType(400)]
    public IActionResult CreateReview([FromQuery] int reviewerId,[FromQuery] int pokemonId,[FromBody] ReviewDto reviewCreate)
    {
        if (reviewCreate == null)
            return BadRequest(ModelState);
        var review = _reviewRepository
            .GetReviews()
            .FirstOrDefault(c => c.Title.Trim().ToUpper()== reviewCreate.Title.TrimEnd().ToUpper());

        if (review != null)
        {
            ModelState.AddModelError("", "Review already exists");
            return StatusCode(422, ModelState);
        }
        if  (!ModelState.IsValid)
            return BadRequest(ModelState);
        var reviewMap = _mapper.Map<Review>(reviewCreate);
        reviewMap.Pokemon = _pokemonRepository.GetPokemon(pokemonId);
        reviewMap.Reviewer = _reviewerRepository.GetReviewer(reviewerId);
        if (!_reviewRepository.CreateReview(reviewMap))
        {
            ModelState.AddModelError("", "Something went wrong with saving review");
            return StatusCode(500, ModelState);
        }
        return Ok("Successfully created review");
    }
    
}

