using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using PokemonReviewApp.Dto;
using PokemonReviewApp.Interfaces;
using PokemonReviewApp.Models;

namespace PokemonReviewApp.Controllers;

[Route("api/[controller]")]
[ApiController]

public class CountryController : Controller
{
    private readonly ICountryRepository _countryRepository;
    private readonly IMapper _mapper;

    public CountryController(ICountryRepository countryRepository, IMapper mapper)
    {
        _countryRepository = countryRepository;
        _mapper = mapper;
    }

    [HttpGet]
    [ProducesResponseType(200, Type = typeof(IEnumerable<Country>))]
    public IActionResult GetCountries()
    {
        var countries = _mapper.Map<List<CountryDto>>(_countryRepository.GetCountries());
        if (!ModelState.IsValid)
            return BadRequest(ModelState);
        return Ok(countries);
    }
    
    [HttpGet("{countryId}")]
    [ProducesResponseType(200, Type = typeof(Country))]
    [ProducesResponseType(400)]
    public IActionResult GetCategory(int countryId)
    {
        if (!_countryRepository.CountryExists(countryId))
            return NotFound();
        var country = _mapper.Map<CountryDto>(_countryRepository.GetCountry(countryId));
        
        if (!ModelState.IsValid)
            return BadRequest(ModelState);
        return Ok(country);
    }

    [HttpGet("country/{ownerId}")]
    [ProducesResponseType(200, Type = typeof(Country))]
    [ProducesResponseType(400)]
    public IActionResult GetCountryByOwner(int ownerId)
    {
        if (!_countryRepository.CountryExists(ownerId))
            return NotFound();
        var country = _mapper.Map<CountryDto>(_countryRepository.GetCountry(ownerId));
        
        if (!ModelState.IsValid)
            return BadRequest(ModelState);
        return Ok(country);
    }

    [HttpGet("owner/{countryId}")]
    [ProducesResponseType(200, Type = typeof(IEnumerable<Owner>))]
    [ProducesResponseType(400)]
    public IActionResult GetOwnersByCountryId(int countryId)
    {
        var owners = _mapper.Map<List<OwnerDto>>(
            _countryRepository.GetOwnersByCountry(countryId));
        
        if (!ModelState.IsValid)
            return BadRequest(ModelState);
        return Ok(owners);
    }
    
    [HttpPost]
    [ProducesResponseType(204)]
    [ProducesResponseType(400)]
    public IActionResult CreateCountry([FromBody] CountryDto? countryCreate)
    {
        if (countryCreate == null)
            return BadRequest(ModelState);
        var category = _countryRepository
            .GetCountries()
            .FirstOrDefault(c => c.Name.Trim().ToUpper()== countryCreate.Name.TrimEnd().ToUpper());

        if (category != null)
        {
            ModelState.AddModelError("", "Country already exists");
            return StatusCode(422, ModelState);
        }
        if  (!ModelState.IsValid)
            return BadRequest(ModelState);
        var countryMap = _mapper.Map<Country>(countryCreate);
        if (!_countryRepository.CreateCountry(countryMap))
        {
            ModelState.AddModelError("", "Something went wrong with saving country");
            return StatusCode(500, ModelState);
        }
        return Ok("Successfully created country");
    }
    [HttpPut]
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]
    [ProducesResponseType(204)]
    public IActionResult UpdateCountry([FromQuery] int countryId, [FromBody] CountryDto updatedCountry)

    {
        if (updatedCountry == null)
            return BadRequest(ModelState);
        if (countryId != updatedCountry.Id)
            return BadRequest(ModelState);
        if (!_countryRepository.CountryExists(countryId))
            return NotFound();
        if (!ModelState.IsValid)
            return BadRequest();
        var countryMap = _mapper.Map<Country>(updatedCountry);
        if (!_countryRepository.UpdateCountry(countryMap))
        {
            ModelState.AddModelError("","Something went wrong while updating country");
            return StatusCode(500, ModelState);
        }

        return NoContent();
    }
    
    [HttpDelete]
    [ProducesResponseType(204)]
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]
    public IActionResult DeleteCountry(int countryId)
    {
        if (!_countryRepository.CountryExists(countryId))
            return NotFound();
        if (!ModelState.IsValid)
            return BadRequest(ModelState);
        var country = _countryRepository.GetCountry(countryId);
        if (!_countryRepository.DeleteCountry(country))
        {
            ModelState.AddModelError("", "Something went wrong while deleting country");
            return StatusCode(500, ModelState);
        }
        return NoContent();
    }
}
    

