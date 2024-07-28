using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using HSBGHelper.Server.Data;
using HSBGHelper.Server.Models;

namespace HSBGHelper.Server.Services
{
    public class MinionService
    {
        // TODO: Implement your minion service logic here
        private readonly HSBGDb _context;
        public MinionService(HSBGDb context)
        {
            _context = context;
        }

        public async Task<Minion> GetMinionById(int id)
        {
            return _context.Minions.Find(id);
        }

        public async Task<List<Minion>> GetMinions()
        {
            return _context.Minions.ToList();
        }
        public async Task SetHtmlGuideById(int id, string htmlGuide)
        {
            var minion = _context.Minions.Find(id);
            minion.HtmlGuide = htmlGuide;
            _context.Minions.Update(minion);
            await _context.SaveChangesAsync();
        }
        public async Task SetTierById(int id, int tier) 
        {
            var minion = _context.Minions.Find(id);
            minion.Tier = tier;
            _context.Minions.Update(minion);
            await _context.SaveChangesAsync();
        }
    }
}