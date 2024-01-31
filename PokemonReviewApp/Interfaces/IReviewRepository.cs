using PokemonReviewApp.Models;

namespace PokemonReviewApp.Interfaces;

public interface IReviewRepository
{
    ICollection<Review> GetReviews();
    Review GetReview(int reviewId);
    ICollection<Review> GetReviewsOfAPokemon(int pokeId);
    bool ReviewExists(int id);
    bool CreateReview(Review review);
    bool UpdateReview(Review review);
    bool Save();

}