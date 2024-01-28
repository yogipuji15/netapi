using AutoMapper;
using belajarnetapi.Data;
using belajarnetapi.Interfaces;
using belajarnetapi.Models;

namespace belajarnetapi.Repository
{
    public class CountryRepository : ICountryRepository
    {
        private readonly DataContext _context;
        public CountryRepository(DataContext context)
        {
            _context = context;
        }

        public bool CountryExists(int id)
        {
            return _context.Countries.Any(c=>c.Id == id);
        }

        public List<Country> GetCountries()
        {
            return _context.Countries.OrderBy(c=>c.Name).ToList();
        }

        public Country GetCountryById(int id)
        {
            return _context.Countries.Where(c=>c.Id == id).FirstOrDefault();
        }

        public Country GetCountryByOwner(int ownerId)
        {
            return _context.Owners.Where(o=>o.Id == ownerId).Select(c=>c.Country).FirstOrDefault();
        }

        public ICollection<Owner> GetOwnersByCountry(int countryId)
        {
            return _context.Owners.Where(o=>o.Country.Id == countryId).ToList();
        }

        public bool CreateCountry(Country country)
        {
            _context.Add(country);
            return Save();
        }

        public bool UpdateCountry(Country country)
        {
            _context.Update(country);
            return Save();
        }

        public bool Save()
        {
            var saved = _context.SaveChanges();
            return saved > 0 ? true : false;
        }
    }
}