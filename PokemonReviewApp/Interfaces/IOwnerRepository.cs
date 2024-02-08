using PokemonReviewApp.Dto;
using PokemonReviewApp.Models;

namespace PokemonReviewApp.Interfaces;

public interface IOwnerRepository
{
    ICollection<Owner> GetOwners();
    Owner GetOwner(int id); 
    bool OwnerExists(int id);
    ICollection<Pokemon> GetPokemonsByOwner(int ownerId);
    
    ICollection<Owner> GetOwnerOfAPokemon(int pokeId);
    
    bool CreateOwner (Owner owner);
    
    bool UpdateOwner(Owner owner);
    
    bool DeleteOwner(Owner owner);
    
    bool Save();
}