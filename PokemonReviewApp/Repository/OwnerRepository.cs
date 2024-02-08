using PokemonReviewApp.Data;
using PokemonReviewApp.Interfaces;
using PokemonReviewApp.Models;

namespace PokemonReviewApp.Repository;

public class OwnerRepository: IOwnerRepository
{
    private readonly DataContext _context;

    public OwnerRepository(DataContext context)
    {
        _context = context;
    }
    public ICollection<Owner> GetOwners()
    {
        return _context.Owners.OrderBy(o => o.Id).ToList();
    }

    public Owner GetOwner(int id)
    {
        return _context.Owners.Where(o => o.Id == id).FirstOrDefault();
    }
    
    public ICollection<Pokemon> GetPokemonsByOwner(int ownerId)
    {
        return _context.PokemonOwners.Where(c=>c.OwnerId == ownerId).Select(p => p.Pokemon).ToList();
    }

    public ICollection<Owner> GetOwnerOfAPokemon(int pokeId)
    {
        return _context.PokemonOwners.Where(p => p.PokemonId == pokeId).Select(o => o.Owner).ToList();
    }

    public bool CreateOwner(Owner owner)
    {
        _context.Owners.Add(owner);
        return Save();
    }

    public bool UpdateOwner(Owner owner)
    {
        _context.Update(owner);
        return Save();
    }

    public bool DeleteOwner(Owner owner)
    {
        _context.Remove(owner);
        return Save();
    }

    public bool Save()
    {
        var saved = _context.SaveChanges();
        return saved > 0 ? true : false;
    }

    public bool OwnerExists(int id)
    {
        return _context.Owners.Any(o =>o.Id == id);
    }
   
}