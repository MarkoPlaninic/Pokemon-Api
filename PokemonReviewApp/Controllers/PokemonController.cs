﻿using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using PokemonReviewApp.Dto;
using PokemonReviewApp.Interfaces;
using PokemonReviewApp.Models;

namespace PokemonReviewApp.Controllers;

[Route("api/[controller]")]
[ApiController]

public class PokemonController : Controller
{
    private readonly IPokemonRepository _pokemonRepository;
    private readonly IMapper _mapper;
    private readonly IReviewRepository _reviewRepository;

    public PokemonController(IPokemonRepository pokemonRepository,IReviewRepository reviewRepository, IMapper mapper)
    {
        _pokemonRepository = pokemonRepository;
        _reviewRepository = reviewRepository;
        _mapper = mapper;
    }

    [HttpGet]
    [ProducesResponseType(200, Type = typeof(IEnumerable<Pokemon>))]
    public IActionResult GetPokemons()
    {
        var pokemons = _mapper.Map<List<PokemonDto>>(_pokemonRepository.GetPokemons());
        if (!ModelState.IsValid)
            return BadRequest(ModelState);
        return Ok(pokemons);

    }

    [HttpGet("{pokeId}")]
    [ProducesResponseType(200, Type = typeof(Pokemon))]
    [ProducesResponseType(400)]
    public IActionResult GetPokemon(int pokeId)
    {
        if (!_pokemonRepository.PokemonExists(pokeId))
            return NotFound();
        var pokemon = _mapper.Map<PokemonDto>(_pokemonRepository.GetPokemon(pokeId));
        
        if (!ModelState.IsValid)
            return BadRequest(ModelState);
        return Ok(pokemon);
    }

    [HttpGet("{pokeId}/rating")]
    [ProducesResponseType(200, Type = typeof(decimal))]
    [ProducesResponseType(400)]
    public IActionResult GetPokemonRating(int pokeId)
    {
        if (!_pokemonRepository.PokemonExists(pokeId))
            return NotFound();
        var rating = _pokemonRepository.GetPokemonRating(pokeId);

        if (!ModelState.IsValid)
            return BadRequest(ModelState);
        return Ok(rating);
    }
    [HttpPost]
    [ProducesResponseType(204)]
    [ProducesResponseType(400)]
    public IActionResult CreatePokemon([FromQuery] int ownerId, [FromQuery] int categoryId,[FromBody] PokemonDto pokemonCreate)
    {
        if (pokemonCreate == null)
            return BadRequest(ModelState);
        var pokemon = _pokemonRepository
            .GetPokemons()
            .FirstOrDefault(c => c.Name.Trim().ToUpper()== pokemonCreate.Name.TrimEnd().ToUpper());

        if (pokemon != null)
        {
            ModelState.AddModelError("", "Pokemon already exists");
            return StatusCode(422, ModelState);
        }
        if  (!ModelState.IsValid)
            return BadRequest(ModelState);
        var pokemonMap = _mapper.Map<Pokemon>(pokemonCreate);
        if (!_pokemonRepository.CreatePokemon(ownerId, categoryId, pokemonMap))
        {
            ModelState.AddModelError("", "Something went wrong with saving pokemon");
            return StatusCode(500, ModelState);
        }
        return Ok("Successfully created pokemon");
    }
    
    [HttpPut]
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]
    [ProducesResponseType(204)]
    public IActionResult UpdatePokemon([FromQuery] int pokemonId,[FromQuery] int categoryId, [FromQuery]int ownerId, [FromBody] PokemonDto updatedPokemon)

    {
        if (updatedPokemon == null)
            return BadRequest(ModelState);
        if (pokemonId != updatedPokemon.Id)
            return BadRequest(ModelState);
        if (!_pokemonRepository.PokemonExists(pokemonId))
            return NotFound();
        if (!ModelState.IsValid)
            return BadRequest();
        var pokemonMap = _mapper.Map<Pokemon>(updatedPokemon);
        if (!_pokemonRepository.UpdatePokemon(pokemonMap))
        {
            ModelState.AddModelError("","Something went wrong while updating pokemon");
            return StatusCode(500, ModelState);
        }

        return NoContent();
    }
    
    [HttpDelete]
    [ProducesResponseType(204)]
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]
    public IActionResult DeletePokemon (int pokemonId)
    {
        if (!_pokemonRepository.PokemonExists(pokemonId))
            return NotFound();
        if (!ModelState.IsValid)
            return BadRequest(ModelState);
        
        var owner = _pokemonRepository.GetPokemon(pokemonId);
        var reviews = _reviewRepository.GetReviewsOfAPokemon(pokemonId);
        if (_reviewRepository.DeleteReviews(reviews.ToList()))
        {
            ModelState.AddModelError("", "Something went wrong while deleting reviews");
        }
        if (!_pokemonRepository.DeletePokemon(owner))
        {
            ModelState.AddModelError("", "Something went wrong while deleting pokemon");
            return StatusCode(500, ModelState);
        }
        return NoContent();
    }
}