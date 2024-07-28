using HSBGHelper.Server.Data;
using HSBGHelper.Server.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

public class CompositionService
{
    private readonly HSBGDb _context;

    public CompositionService(HSBGDb context)
    {
        _context = context;
    }

    // public async Task<List<Composition>> GetCompositionsAsync()
    // {
    //     return await _context.Compositions
    //         .Include(c => c.mainMinion)
    //         .Include(c => c.AddOns)
    //         .Include(c => c.CommonEnablers)
    //         .ToListAsync();
    // }

    // public async Task<Composition> GetCompositionByIdAsync(int id)
    // {
    //     return await _context.Compositions
    //         .Include(c => c.mainMinion)
    //         .Include(c => c.AddOns)
    //         .Include(c => c.CommonEnablers)
    //         .FirstOrDefaultAsync(c => c.Id == id);
    // }

    public async Task AddCompositionAsync(Composition composition)
    {
        _context.Compositions.Add(composition);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateCompositionAsync(Composition composition)
    {
        _context.Entry(composition).State = EntityState.Modified;
        await _context.SaveChangesAsync();
    }

    public async Task DeleteCompositionAsync(int id)
    {
        var composition = await _context.Compositions.FindAsync(id);
        if (composition != null)
        {
            _context.Compositions.Remove(composition);
            await _context.SaveChangesAsync();
        }
    }
}