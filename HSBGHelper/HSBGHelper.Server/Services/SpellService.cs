using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using HSBGHelper.Server.Data;
using HSBGHelper.Server.Models;
using Microsoft.EntityFrameworkCore;

namespace HSBGHelper.Server.Services 
{
    public class SpellService
    {
        private readonly HSBGDb _context;
        public SpellService(HSBGDb context)
        {
            _context = context;
        }

        public async Task<Spell> GetSpellById(int id)
        {
            return _context.Spells.Find(id);
        }

        public async Task<List<Spell>> GetSpells()
        {
            return _context.Spells.ToList();
        }
        public async Task SetSpellTierById(int id, int tier) {
            var spell = await GetSpellById(id);
            spell.Tier = tier;
            _context.Spells.Update(spell);
            await _context.SaveChangesAsync();
        }
        public async Task SetHtmlGuideById(int id, string htmlGuide) {
            var spell = await GetSpellById(id);
            spell.HtmlGuide = htmlGuide;
            _context.Spells.Update(spell);
            await _context.SaveChangesAsync();
        }
    }
}