using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using HSBGHelper.Server.Data;
using HSBGHelper.Server.Models;

namespace HSBGHelper.Server.Services 
{
    public class LesserTrinketService 
    {
        private readonly HSBGDb _context;

        public LesserTrinketService(HSBGDb context)
        {
            _context = context;
        }

        public async Task<LesserTrinket> GetLesserTrinketById(int id)
        {
            return _context.LesserTrinkets.Find(id);
        }

        public async Task<List<LesserTrinket>> GetTrinkets()
        {
            return _context.LesserTrinkets.ToList();
        }
    }
}