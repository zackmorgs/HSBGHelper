using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using HSBGHelper.Server.Data;
using HSBGHelper.Server.Models;

namespace HSBGHelper.Server.Services 
{
    public class GreaterTrinketService 
    {
        private readonly HSBGDb _context;

        public GreaterTrinketService(HSBGDb context)
        {
            _context = context;
        }

        public async Task<GreaterTrinket> GetGreaterTrinketById(int id)
        {
            return _context.GreaterTrinkets.Find(id);
        }

        public async Task<List<GreaterTrinket>> GetGreaterTrinkets()
        {
            return _context.GreaterTrinkets.ToList();
        }
    }
}