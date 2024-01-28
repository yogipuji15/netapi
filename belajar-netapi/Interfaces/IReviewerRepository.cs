using belajarnetapi.Models;

namespace belajarnetapi.Interfaces
{
    public interface IReviewerRepository
    {
        public ICollection<Reviewer> GetReviewers();
        public Reviewer GetReviewerById(int id);
        public ICollection<Review> GetReviewsByReviewer(int reviewerId);
        public bool ReviewerExists(int id);
        public bool CreateReviewer(Reviewer reviewer);
        public bool UpdateReviewer(Reviewer reviewer);
        public bool Save();
    }
}