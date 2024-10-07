using HSBGHelper.Server.Data;
using HSBGHelper.Server.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HSBGHelper.Server.Services
{
    public class HeroService
    {
        private readonly HSBGDb _context;

        public HeroService(HSBGDb context)
        {
            _context = context;
        }
        public async Task<int> GetHeroCount()
        {
            return await _context.Heroes.CountAsync();
        }
        public async Task<List<Hero>> GetHeroes()
        {
            return await _context.Heroes
                .Include(h => h.heroPower)  // Include related HeroPower
                .ToListAsync();
        }
        public async Task<Hero> GetHeroById(int id)
        {
            return await _context.Heroes
                .Include(h => h.heroPower)
                .FirstOrDefaultAsync(h => h.Id == id);
        }
        public async Task<Hero> GetHeroByName(string name)
        {
            return await _context.Heroes
                .Include(h => h.heroPower)
                .FirstOrDefaultAsync(h => h.Name == name);
        }
        public async Task SetHeroJeefTier(int id, string tier)
        {
            try
            {
                var hero = await GetHeroById(id);
                hero.TierJeef = tier;
                _context.Heroes.Update(hero);
                await _context.SaveChangesAsync();
            }
            catch (System.Exception e)
            {
                Console.WriteLine(e);
            }
        }
        public async Task SetHeroHSReplayTier(int id, string tier)
        {
            try
            {
                var hero = await GetHeroById(id);
                hero.TierHSReplay = tier;
                _context.Heroes.Update(hero);
                await _context.SaveChangesAsync();
            }
            catch (System.Exception e)
            {
                Console.WriteLine(e);
            }
        }
        public async Task SetHeroArmor(int id, int armor)
        {
            try
            {
                var hero = await GetHeroById(id);
                hero.Armor = armor;
                _context.Heroes.Update(hero);
                await _context.SaveChangesAsync();

            }
            catch (System.Exception e)
            {
                Console.WriteLine(e);
            }
        }
        public async Task SetHeroHtmlGuide(int id, string html) {
            try
            {
                var hero = await GetHeroById(id);
                hero.HtmlGuide = html;
                _context.Heroes.Update(hero);
                await _context.SaveChangesAsync();
            }
            catch (System.Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }
}
