using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using PokemonReviewApp.Data;
using PokemonReviewApp.Dto;
using PokemonReviewApp.Interfaces;
using PokemonReviewApp.Models;

namespace PokemonReviewApp.Controllers;

[Route("api/[controller]")]
[ApiController]

public class OwnerController: Controller
{
    private readonly IOwnerRepository _ownerRepository;
    private readonly IMapper _mapper;
    private readonly ICountryRepository _countryRepository;

    public OwnerController(IOwnerRepository ownerRepository,ICountryRepository countryRepository, IMapper mapper)
    {
        _ownerRepository = ownerRepository;
        _countryRepository = countryRepository;
        _mapper = mapper;
    }

    [HttpGet]
    [ProducesResponseType(200, Type = typeof(IEnumerable<Owner>))]
    public IActionResult GetOwners()
    {
        var owners = _mapper.Map<List<OwnerDto>>(_ownerRepository.GetOwners());
        if (!ModelState.IsValid)
            return BadRequest(ModelState);
        return Ok(owners);
    }

    [HttpGet("{ownerId}")]
    [ProducesResponseType(200, Type = typeof(Owner))]
    public IActionResult GetOwner(int ownerId)
    {
        if (!_ownerRepository.OwnerExists(ownerId))
            return NotFound();
        var owner = _mapper.Map<OwnerDto>(_ownerRepository.GetOwner(ownerId));
        
        if (!ModelState.IsValid)
            return BadRequest(ModelState);
        return Ok(owner);
    }
    
    [HttpGet("pokemon/{ownerId}")]
    [ProducesResponseType(200, Type = typeof(IEnumerable<Pokemon>))]
    [ProducesResponseType(400)]
    public IActionResult GetPokemonsByOwner(int ownerId)
    {
        var pokemons= _mapper.Map<List<PokemonDto>>(
            _ownerRepository.GetPokemonsByOwner(ownerId));
        
        if (!ModelState.IsValid)
            return BadRequest(ModelState);
        return Ok(pokemons);
    }
    
    [HttpGet("owner/{pokeId}")]
    [ProducesResponseType(200, Type = typeof(IEnumerable<Owner>))]
    [ProducesResponseType(400)]
    public IActionResult GetOwnerOfAPokemon(int pokeId)
    {
        var owner= _mapper.Map<List<OwnerDto>>(
            _ownerRepository.GetOwnerOfAPokemon(pokeId));
        
        if (!ModelState.IsValid)
            return BadRequest(ModelState);
        return Ok(owner);
    }

    [HttpPost]
    [ProducesResponseType(204)]
    [ProducesResponseType(400)]
    public IActionResult CreateOwner([FromQuery] int countryId, [FromBody] OwnerDto ownerCreate)
    {
        if (ownerCreate == null)
            return BadRequest(ModelState);
        var owner = _ownerRepository
            .GetOwners()
            .FirstOrDefault(c => c.FirstName.Trim().ToUpper() == ownerCreate.FirstName.TrimEnd().ToUpper());

        if (owner != null)
        {
            ModelState.AddModelError("", "Owner already exists");
            return StatusCode(422, ModelState);
        }

        if (!ModelState.IsValid)
            return BadRequest(ModelState);
        var ownerMap = _mapper.Map<Owner>(ownerCreate);
        ownerMap.Country = _countryRepository.GetCountry(countryId);
        if (!_ownerRepository.CreateOwner(ownerMap))
        {
            ModelState.AddModelError("", "Something went wrong with saving owner");
            return StatusCode(500, ModelState);
        }

        return Ok("Successfully created owner");
    }

    [HttpPut]
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]
    [ProducesResponseType(204)]
    public IActionResult UpdateOwner([FromQuery] int ownerId, [FromBody] OwnerDto updatedOwner)

    {
        if (updatedOwner == null)
                return BadRequest(ModelState);
        if (ownerId != updatedOwner.Id)
            return BadRequest(ModelState);
        if (!_ownerRepository.OwnerExists(ownerId))
            return NotFound();
        if (!ModelState.IsValid)
            return BadRequest();
        var ownerMap = _mapper.Map<Owner>(updatedOwner);
        if (!_ownerRepository.UpdateOwner(ownerMap))
        {
            ModelState.AddModelError("","Something went wrong while updating owner");
            return StatusCode(500, ModelState);
        }

        return NoContent();
    }
    
}
    
