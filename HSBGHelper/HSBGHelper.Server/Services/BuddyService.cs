using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using HSBGHelper.Server.Data;
using HSBGHelper.Server.Models;

namespace HSBGHelper.Server.Services
{
    public class BuddyService
    {

        private readonly HSBGDb _context;

        public BuddyService(HSBGDb context)
        {
            _context = context;
        }

        public async Task<Buddy> GetBuddyById(int id)
        {
            return _context.Buddies.Find(id);
        }

        public async Task<List<Buddy>> GetBuddies()
        {
            return _context.Buddies.ToList();
        }

        public async Task SetBuddyTierById(int id, int tier) {
            var buddy = await GetBuddyById(id);
            buddy.Tier = tier;
            _context.Buddies.Update(buddy);
            await _context.SaveChangesAsync();
        }
        
        public async Task SetBuddyTypeById(int id, String type) {
            var buddy = await GetBuddyById(id);
            buddy.Type = type;
            _context.Buddies.Update(buddy);
            await _context.SaveChangesAsync();
        }

        public async Task SetBuddyNameById(int id, String name) {
            var buddy = await GetBuddyById(id);
            buddy.Name = name;
            _context.Buddies.Update(buddy);
            await _context.SaveChangesAsync();
        }

        public async Task SetBuddyImageById(int id, String image) {
            var buddy = await GetBuddyById(id);
            buddy.Image = image;
            _context.Buddies.Update(buddy);
            await _context.SaveChangesAsync();
        }
        public async Task SetBuddyHtmlGuide(int id, String htmlGuide) {
            var buddy = await GetBuddyById(id);
            buddy.HtmlGuide = htmlGuide;
            _context.Buddies.Update(buddy);
            await _context.SaveChangesAsync();
        }
    }
}