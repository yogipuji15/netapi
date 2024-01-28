using belajarnetapi.Models;

namespace belajarnetapi.Interfaces
{
    public interface ICountryRepository
    {
        public List<Country> GetCountries();
        public Country GetCountryById(int id);
        public Country GetCountryByOwner(int ownerId);
        public ICollection<Owner> GetOwnersByCountry(int countryId);
        public bool CountryExists(int id);
        public bool CreateCountry(Country country);
        public bool UpdateCountry(Country country);
        public bool Save();
    }
}