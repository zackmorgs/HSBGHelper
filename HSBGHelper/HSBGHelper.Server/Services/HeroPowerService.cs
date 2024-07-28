using HSBGHelper.Server.Data;
using HSBGHelper.Server.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HSBGHelper.Server.Services
{
    public class HeroPowerService
    {
        private readonly HSBGDb _context;
        public HeroPowerService(HSBGDb context)
        {
            _context = context;
        }
        public async Task<HeroPower> GetHeroPowerById(int id)
        {
            return _context.HeroPowers.Find(id);
        }
    }
}
