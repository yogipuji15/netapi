using belajarnetapi.Models;

namespace belajarnetapi.Repository
{
    public interface IReviewRepository
    {
        public ICollection<Review> GetReviews();
        public Review GetReviewById(int id);
        public ICollection<Review> GetReviewsByPokemon(int pokemonId);
        public bool ReviewExists(int id);
        public bool CreateReview(Review review);
        public bool UpdateReview(Review review);
        public bool DeleteReview(Review review);
        public bool Save();
    }
}