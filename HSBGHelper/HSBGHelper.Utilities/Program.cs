using System;
using System.Collections.Generic;
using System.Net;
using Microsoft.EntityFrameworkCore;
using HSBGHelper.Server.Models;
using HSBGHelper.Server.Data;
using Microsoft.Extensions.Configuration;
using PuppeteerSharp;
using Azure.Core;
using Microsoft.Identity.Client;

namespace HSBGHelper.Utilities
{
    public class Program
    {
        public Program()
        {
        }

        public static async Task Main(string[] args)
        {
            Console.WriteLine("Start HSBGHelper.Utilities");

            // Setup database context options here
            var options = new DbContextOptionsBuilder<HSBGDb>()
                .UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=HSBGHelper;Trusted_Connection=True;MultipleActiveResultSets=true")
                .Options;

            using (HSBGDb context = new HSBGDb(options))
            {
                context.Database.EnsureCreated();
                context.Heroes.RemoveRange(context.Heroes);
                context.Minions.RemoveRange(context.Minions);
                // context.Buddies.RemoveRange(context.Buddies);
                context.Spells.RemoveRange(context.Spells);
                context.SaveChanges();

                var program = new Program();

                // context.Remove();

                await program.ScrapeMinions(context);
                await program.ScrapeHeroInformation(context);
                await program.ScrapeSpells(context);
                await program.ScrapeGreaterTrinkets(context);
                await program.ScrapeLesserTrinkets(context);
            }
        }
        private async Task ScrapeLesserTrinkets(HSBGDb context)
        {
            string lesserTrinketLink = "https://hearthstone.blizzard.com/en-us/battlegrounds?bgCardType=trinket&spellSchool=lesser_trinket";
            var browserFetcher = new BrowserFetcher();
            await browserFetcher.DownloadAsync();
            await using var Browser = await Puppeteer.LaunchAsync(new LaunchOptions { Headless = true });
            // https://hearthstone.blizzard.com/en-us/battlegrounds?bgCardType=trinket&spellSchool=greater_trinket
            // https://hearthstone.blizzard.com/en-us/battlegrounds?bgCardType=trinket&spellSchool=lesser_trinket
            var LesserTrinkets = new List<LesserTrinket>();

            var page = await Browser.NewPageAsync();
            await page.GoToAsync(lesserTrinketLink);
            await Task.Delay(1000);

            await page.WaitForSelectorAsync("#MainCardGrid .CardImage");

            var lesserTrinketNodes = await page.QuerySelectorAllAsync("#MainCardGrid .CardImage");

            foreach (var lesserTrinketNode in lesserTrinketNodes)
            {
                var name = await lesserTrinketNode.EvaluateFunctionAsync<string>("e => e.alt");
                var image = await lesserTrinketNode.EvaluateFunctionAsync<string>("e => e.src");
                Console.WriteLine("Greater Trinket Found: " + name);
                
                await lesserTrinketNode.ClickAsync();
                await page.WaitForSelectorAsync(".jWOOrt");
                var descriptionNode = await page.QuerySelectorAsync(".jWOOrt");

                var description = await descriptionNode.EvaluateFunctionAsync<string>("e => e.innerText");
                Console.WriteLine(description);

                await page.ClickAsync(".knbYrP");

                LesserTrinkets.Add(new LesserTrinket() { Name = name, Description=description, Image = image, HtmlGuide = "", Cost = 0 });
            }
            await Browser.CloseAsync();

            context.LesserTrinkets.AddRange(LesserTrinkets);
            context.SaveChanges();
        }
        private async Task ScrapeGreaterTrinkets(HSBGDb context)
        {
            var browserFetcher = new BrowserFetcher();
            await browserFetcher.DownloadAsync();
            await using var Browser = await Puppeteer.LaunchAsync(new LaunchOptions { Headless = true });
            // https://hearthstone.blizzard.com/en-us/battlegrounds?bgCardType=trinket&spellSchool=greater_trinket
            // https://hearthstone.blizzard.com/en-us/battlegrounds?bgCardType=trinket&spellSchool=lesser_trinket
            var GreaterTrinkets = new List<GreaterTrinket>();

            string greaterTrinketLink = "https://hearthstone.blizzard.com/en-us/battlegrounds?bgCardType=trinket&spellSchool=greater_trinket";

            var page = await Browser.NewPageAsync();
            await page.GoToAsync(greaterTrinketLink);
            await Task.Delay(1000);


            await page.WaitForSelectorAsync("#MainCardGrid .CardImage");
            var greaterTrinketNodes = await page.QuerySelectorAllAsync("#MainCardGrid .CardImage");

            foreach (var greaterTrinketNode in greaterTrinketNodes)
            {
                var name = await greaterTrinketNode.EvaluateFunctionAsync<string>("e => e.alt");
                var image = await greaterTrinketNode.EvaluateFunctionAsync<string>("e => e.src");
                Console.WriteLine("Greater Trinket Found: " + name);

                await greaterTrinketNode.ClickAsync();
                
                await page.WaitForSelectorAsync(".jWOOrt");
                var descriptionNode = await page.QuerySelectorAsync(".jWOOrt");

                var description = await descriptionNode.EvaluateFunctionAsync<string>("e => e.innerText");
                Console.WriteLine(description);

                GreaterTrinkets.Add(new GreaterTrinket() { Name = name, Description=description, Image = image, HtmlGuide = "", Cost = 0  });
                
                await page.ClickAsync(".knbYrP");
            }
            await Browser.CloseAsync();

            context.GreaterTrinkets.AddRange(GreaterTrinkets);
            context.SaveChanges();
            
        }   
 
        private async Task ScrapeSpells(HSBGDb context)
        {

            // we gotta literally download chrome to start this up
            var browserFetcher = new BrowserFetcher();
            await browserFetcher.DownloadAsync();
            await using var Browser = await Puppeteer.LaunchAsync(new LaunchOptions { Headless = true });

            var spells = new List<Spell>();

            for (int i = 1; i <= 6; i++)
            {

                string path = $"https://hearthstone.blizzard.com/en-us/battlegrounds?bgCardType=spell&tier={i}";
                var page = await Browser.NewPageAsync();
                await page.GoToAsync(path);
                await page.WaitForSelectorAsync("#MainCardGrid .CardImage");
                var spellNodes = await page.QuerySelectorAllAsync("#MainCardGrid .CardImage");
                foreach (var spellNode in spellNodes)
                {
                    var name = await spellNode.EvaluateFunctionAsync<string>("e => e.alt");
                    var image = await spellNode.EvaluateFunctionAsync<string>("e => e.src");
                    Console.WriteLine("Spell Found: " + name);
                    spells.Add(new Spell() { Name = name, Image = image, Tier = i, HtmlGuide = "", spellSynergies = new List<Spell>(), minionSynergies = new List<Minion>(), heroSynergies = new List<Hero>() });
                }
            }
            
            Browser.CloseAsync().Wait();

            context.Spells.AddRange(spells);
            context.SaveChanges();
        }
        // private async Task DownloadImageAsyncIfNotExists(string path, string fileName, Uri uri)
        // {
        //     using var httpClient = new HttpClient();
        //     // await Task.Delay(500);

        //     // Get the file extension
        //     var uriWithoutQuery = uri.GetLeftPart(UriPartial.Path);
        //     var fileExtension = Path.GetExtension(uriWithoutQuery);

        //     // Create file path and ensure directory exists
        //     var path = Path.Combine($"{fileName.ToLower().Replace(" ","")}{fileExtension}");
        //     // Directory.CreateDirectory(directoryPath);

        //     // check if the file exists
        //     if (File.Exists(path)) {
        //         return;
        //     } else {
        //     // Download the image and write to the file
        //     var imageBytes = await httpClient.GetByteArrayAsync(uri);
        //     await File.WriteAllBytesAsync(path, imageBytes);
        //     }
        // }

        public async Task ScrapeHeroInformation(HSBGDb context)
        {
            var heroesPage = "https://hearthstone.blizzard.com/en-us/battlegrounds?bgCardType=hero";

            // we gotta literally download chrome to start this up
            var browserFetcher = new BrowserFetcher();
            await browserFetcher.DownloadAsync();
            await using var Browser = await Puppeteer.LaunchAsync(new LaunchOptions { Headless = true });

            // prepare minion list
            var heros = new List<Hero>();

            // create a new page
            var page = await Browser.NewPageAsync();

            // go to the page and wait for the selector to load
            await page.GoToAsync(heroesPage);
            await page.WaitForSelectorAsync(".CardWrap.hero");

            // var heroImageNode = await page.QuerySelectorAllAsync(".CardImage.hero");
            var heroLinks = await page.QuerySelectorAllAsync(".CardWrap.hero");

            foreach (var heroNode in heroLinks)
            {
                // find the hero image
                var heroLinksImage = await heroNode.QuerySelectorAsync(".CardImage.hero");

                var name = await heroLinksImage.EvaluateFunctionAsync<string>("e => e.alt");
                var image = await heroLinksImage.EvaluateFunctionAsync<string>("e => e.src");

                Console.WriteLine(name);
                // click the hero portrait
                await heroNode.ClickAsync();

                // get the hero power description
                var heroPowerDescriptionNode = await page.QuerySelectorAsync("p[class^='CardDetailsLayout__CardText']");
                var heroPowerDescription = await heroPowerDescriptionNode.EvaluateFunctionAsync<string>("e => e.innerText");
                Console.WriteLine(heroPowerDescription);

                // get the hero power image
                await page.WaitForSelectorAsync("[class^='HeroPowers__HeroPowerGrid'] img");
                var heroPowerImageNode = await page.QuerySelectorAsync("[class^='HeroPowers__HeroPowerGrid'] img");
                var heroPowerImage = await heroPowerImageNode.EvaluateFunctionAsync<string>("e => e.src");
                var heroPowerName = await heroPowerImageNode.EvaluateFunctionAsync<string>("e => e.alt");

                var heroPower = new HeroPower()
                {
                    Name = heroPowerName,
                    Description = heroPowerDescription,
                    Image = heroPowerImage,
                    HtmlGuide = "",
                    spellSynergies = new List<Spell>(),
                    minionSynergies = new List<Minion>(),
                    heroSynergies = new List<Hero>()
                };


                // add the hero information to the hero list
                heros.Add(new Hero()
                {
                    Name = name,
                    Image = image,
                    heroPower = heroPower,
                    TierHSReplay = "",
                    TierJeef = "",
                    HtmlGuide = "",
                    spellSynergies = new List<Spell>(),
                    minionSynergies = new List<Minion>(),
                    heroSynergies = new List<Hero>()
                });

                // close the modal
                await page.ClickAsync(".knbYrP");
            }
            context.Heroes.AddRange(heros);
            context.SaveChanges();

            Browser.CloseAsync().Wait();
        }

        public async Task ScrapeMinions(HSBGDb context)
        {
            Console.WriteLine("Scraping minions");
            var minions = new List<Minion>();

            minions.AddRange(await ScrapeBeasts());
            minions.AddRange(await ScrapeDemons());
            minions.AddRange(await ScrapeDragons());
            minions.AddRange(await ScrapeElementals());
            minions.AddRange(await ScrapeMechs());
            minions.AddRange(await ScrapeMurlocs());
            minions.AddRange(await ScrapeNaga());
            minions.AddRange(await ScrapePirates());
            minions.AddRange(await ScrapeQuilboar());
            minions.AddRange(await ScrapeUndead());
            
            // await DownloadImagesAsync(minions);

            minions.AddRange(await ScrapeNeutral(minions));

            context.Minions.AddRange(minions);
            context.SaveChanges();
        }

        public async Task DownloadImagesAsync(List<Minion> minions)
        {
            foreach (var minion in minions)
            {
                using var httpClient = new HttpClient();
                var uri = new Uri(minion.Image);
                var uriWithoutQuery = uri.GetLeftPart(UriPartial.Path);
                var fileExtension = Path.GetExtension(uriWithoutQuery);
                var path = Path.Combine($"{minion.Name.ToLower().Replace(" ", "")}{fileExtension}");

                if (File.Exists(path))
                {
                    continue;
                }
                else
                {
                    var imageBytes = await httpClient.GetByteArrayAsync(uri);
                    await File.WriteAllBytesAsync(path, imageBytes);
                }
            }
        }

        public async Task<List<Minion>> scrapeMinionsOnPage(string path, string type, int tier)
        {
            // we gotta literally download chrome to start this up
            var browserFetcher = new BrowserFetcher();
            await browserFetcher.DownloadAsync();
            await using var Browser = await Puppeteer.LaunchAsync(new LaunchOptions { Headless = true });

            // prepare minion list
            var minions = new List<Minion>();

            // create a new page
            var page = await Browser.NewPageAsync();

            // go to the page and wait for the selector to load
            await page.GoToAsync(path);
            await page.WaitForSelectorAsync("#MainCardGrid .CardImage");
            var minionNodes = await page.QuerySelectorAllAsync("#MainCardGrid .CardImage");

            foreach (var minionNode in minionNodes)
            {
                var name = await minionNode.EvaluateFunctionAsync<string>("e => e.alt");
                var image = await minionNode.EvaluateFunctionAsync<string>("e => e.src");

                Console.WriteLine(name);

                minions.Add(new Minion() { 
                    Name = name, 
                    Image = image, 
                    Type = type, 
                    Tier = tier, 
                    HtmlGuide = "",
                    heroSynergies = new List<Hero>(),
                    minionSynergies = new List<Minion>(),
                    spellSynergies = new List<Spell>()  
                });
            }
            Browser.CloseAsync().Wait();

            return minions;
        }
        public async Task<List<Minion>> scrapeNeutralMinionsOnPage(List<Minion> minionsInDb, int tier) {
            Console.WriteLine($"Scraping Neutral Minions: Tier {tier}");
            var pageUrl = $"https://hearthstone.blizzard.com/en-us/battlegrounds?bgCardType=minion&tier={tier}";

            // we gotta literally download chrome to start this up
            var browserFetcher = new BrowserFetcher();
            browserFetcher.DownloadAsync().Wait();
            using var Browser = Puppeteer.LaunchAsync(new LaunchOptions { Headless = true }).Result;

            // prepare minion list
            var minions = new List<Minion>();

            // create a new page
            var page = Browser.NewPageAsync().Result;

            // go to the page and wait for the selector to load
            page.GoToAsync(pageUrl).Wait();
            page.WaitForSelectorAsync("#MainCardGrid .CardImage").Wait();
            var minionNodes = page.QuerySelectorAllAsync("#MainCardGrid .CardImage").Result;

            foreach (var minionNode in minionNodes)
            {
                var name = minionNode.EvaluateFunctionAsync<string>("e => e.alt").Result;
                var image = minionNode.EvaluateFunctionAsync<string>("e => e.src").Result;

                // if minion is already in db
                if (!minionsInDb.Any(m => m.Name == name)) {
                    Console.WriteLine(name);
                    minions.Add(new Minion() 
                    { 
                        Name = name, 
                        Image = image, 
                        Type = "Neutral", 
                        Tier = tier, 
                        HtmlGuide = "", 
                        heroSynergies = new List<Hero>(), 
                        minionSynergies = new List<Minion>(), 
                        spellSynergies = new List<Spell>()  
                    });
                }
            }
            Browser.CloseAsync().Wait();
            return minions;
        }
        public async Task<List<Minion>> ScrapeNeutral(List<Minion> minionsInDb) {
            var minions = new List<Minion>();

            for (int i=1; i<=7; i++) {
                minions.AddRange(await scrapeNeutralMinionsOnPage(minionsInDb, i));
            }
            return minions;
        }
        
        public async Task<List<Minion>> ScrapeBeasts()
        {
            Console.WriteLine("Scraping Beasts");
            var beasts = new List<Minion>();

            string path1 = "https://hearthstone.blizzard.com/en-us/battlegrounds?minionType=beast&tier=1";
            beasts.AddRange(await scrapeMinionsOnPage(path1, "Beast", 1));
            string path2 = "https://hearthstone.blizzard.com/en-us/battlegrounds?minionType=beast&tier=2";
            beasts.AddRange(await scrapeMinionsOnPage(path2, "Beast", 2));
            string path3 = "https://hearthstone.blizzard.com/en-us/battlegrounds?minionType=beast&tier=3";
            beasts.AddRange(await scrapeMinionsOnPage(path3, "Beast", 3));
            string path4 = "https://hearthstone.blizzard.com/en-us/battlegrounds?minionType=beast&tier=4";
            beasts.AddRange(await scrapeMinionsOnPage(path4, "Beast", 4));
            string path5 = "https://hearthstone.blizzard.com/en-us/battlegrounds?minionType=beast&tier=5";
            beasts.AddRange(await scrapeMinionsOnPage(path5, "Beast", 5));
            string path6 = "https://hearthstone.blizzard.com/en-us/battlegrounds?minionType=beast&tier=6";
            beasts.AddRange(await scrapeMinionsOnPage(path6, "Beast", 6));
            string path7 = "https://hearthstone.blizzard.com/en-us/battlegrounds?minionType=beast&tier=7";
            beasts.AddRange(await scrapeMinionsOnPage(path7, "Beast", 7));
            return beasts;
        }

        public async Task<List<Minion>> ScrapeDemons()
        {
            Console.WriteLine("Scraping Demons");
            var minions = new List<Minion>();
            string path1 = "https://hearthstone.blizzard.com/en-us/battlegrounds?minionType=demon&tier=1";
            minions.AddRange(await scrapeMinionsOnPage(path1, "Demon", 1));
            string path2 = "https://hearthstone.blizzard.com/en-us/battlegrounds?minionType=demon&tier=2";
            minions.AddRange(await scrapeMinionsOnPage(path2, "Demon", 2));
            string path3 = "https://hearthstone.blizzard.com/en-us/battlegrounds?minionType=demon&tier=3";
            minions.AddRange(await scrapeMinionsOnPage(path3, "Demon", 3));
            string path4 = "https://hearthstone.blizzard.com/en-us/battlegrounds?minionType=demon&tier=4";
            minions.AddRange(await scrapeMinionsOnPage(path4, "Demon", 4));
            string path5 = "https://hearthstone.blizzard.com/en-us/battlegrounds?minionType=demon&tier=5";
            minions.AddRange(await scrapeMinionsOnPage(path5, "Demon", 5));
            string path6 = "https://hearthstone.blizzard.com/en-us/battlegrounds?minionType=demon&tier=6";
            minions.AddRange(await scrapeMinionsOnPage(path6, "Demon", 6));
            string path7 = "https://hearthstone.blizzard.com/en-us/battlegrounds?minionType=demon&tier=7";
            minions.AddRange(await scrapeMinionsOnPage(path7, "Demon", 7));
            return minions;
        }

        public async Task<List<Minion>> ScrapeDragons()
        {
            Console.WriteLine("Scraping Dragons");
            var minions = new List<Minion>();
            string path1 = "https://hearthstone.blizzard.com/en-us/battlegrounds?minionType=dragon&tier=1";
            minions.AddRange(await scrapeMinionsOnPage(path1, "Dragon", 1));
            string path2 = "https://hearthstone.blizzard.com/en-us/battlegrounds?minionType=dragon&tier=2";
            minions.AddRange(await scrapeMinionsOnPage(path2, "Dragon", 2));
            string path3 = "https://hearthstone.blizzard.com/en-us/battlegrounds?minionType=dragon&tier=3";
            minions.AddRange(await scrapeMinionsOnPage(path3, "Dragon", 3));
            string path4 = "https://hearthstone.blizzard.com/en-us/battlegrounds?minionType=dragon&tier=4";
            minions.AddRange(await scrapeMinionsOnPage(path4, "Dragon", 4));
            string path5 = "https://hearthstone.blizzard.com/en-us/battlegrounds?minionType=dragon&tier=5";
            minions.AddRange(await scrapeMinionsOnPage(path5, "Dragon", 5));
            string path6 = "https://hearthstone.blizzard.com/en-us/battlegrounds?minionType=dragon&tier=6";
            minions.AddRange(await scrapeMinionsOnPage(path6, "Dragon", 6));
            string path7 = "https://hearthstone.blizzard.com/en-us/battlegrounds?minionType=dragon&tier=7";
            minions.AddRange(await scrapeMinionsOnPage(path7, "Dragon", 7));
            return minions;
        }

        public async Task<List<Minion>> ScrapeElementals()
        {
            Console.WriteLine("Scraping Elementals");
            var minions = new List<Minion>();
            string path1 = "https://hearthstone.blizzard.com/en-us/battlegrounds?minionType=elemental&tier=1";
            minions.AddRange(await scrapeMinionsOnPage(path1, "Elemental", 1));
            string path2 = "https://hearthstone.blizzard.com/en-us/battlegrounds?minionType=elemental&tier=2";
            minions.AddRange(await scrapeMinionsOnPage(path2, "Elemental", 2));
            string path3 = "https://hearthstone.blizzard.com/en-us/battlegrounds?minionType=elemental&tier=3";
            minions.AddRange(await scrapeMinionsOnPage(path3, "Elemental", 3));
            string path4 = "https://hearthstone.blizzard.com/en-us/battlegrounds?minionType=elemental&tier=4";
            minions.AddRange(await scrapeMinionsOnPage(path4, "Elemental", 4));
            string path5 = "https://hearthstone.blizzard.com/en-us/battlegrounds?minionType=elemental&tier=5";
            minions.AddRange(await scrapeMinionsOnPage(path5, "Elemental", 5));
            string path6 = "https://hearthstone.blizzard.com/en-us/battlegrounds?minionType=elemental&tier=6";
            minions.AddRange(await scrapeMinionsOnPage(path6, "Elemental", 6));
            string path7 = "https://hearthstone.blizzard.com/en-us/battlegrounds?minionType=elemental&tier=7";
            minions.AddRange(await scrapeMinionsOnPage(path7, "Elemental", 7));
            return minions;
        }

        public async Task<List<Minion>> ScrapeMechs()
        {
            Console.WriteLine("Scraping Mechs");
            var minions = new List<Minion>();
            string path1 = "https://hearthstone.blizzard.com/en-us/battlegrounds?minionType=mech&tier=1";
            minions.AddRange(await scrapeMinionsOnPage(path1, "Mech", 1));
            string path2 = "https://hearthstone.blizzard.com/en-us/battlegrounds?minionType=mech&tier=2";
            minions.AddRange(await scrapeMinionsOnPage(path2, "Mech", 2));
            string path3 = "https://hearthstone.blizzard.com/en-us/battlegrounds?minionType=mech&tier=3";
            minions.AddRange(await scrapeMinionsOnPage(path3, "Mech", 3));
            string path4 = "https://hearthstone.blizzard.com/en-us/battlegrounds?minionType=mech&tier=4";
            minions.AddRange(await scrapeMinionsOnPage(path4, "Mech", 4));
            string path5 = "https://hearthstone.blizzard.com/en-us/battlegrounds?minionType=mech&tier=5";
            minions.AddRange(await scrapeMinionsOnPage(path5, "Mech", 5));
            string path6 = "https://hearthstone.blizzard.com/en-us/battlegrounds?minionType=mech&tier=6";
            minions.AddRange(await scrapeMinionsOnPage(path6, "Mech", 6));
            string path7 = "https://hearthstone.blizzard.com/en-us/battlegrounds?minionType=mech&tier=7";
            minions.AddRange(await scrapeMinionsOnPage(path7, "Mech", 7));
            return minions;
        }

        public async Task<List<Minion>> ScrapeMurlocs()
        {
            Console.WriteLine("Scrape Murlocs");

            var minions = new List<Minion>();
            string path1 = "https://hearthstone.blizzard.com/en-us/battlegrounds?minionType=murloc&tier=1";
            minions.AddRange(await scrapeMinionsOnPage(path1, "Murloc", 1));
            string path2 = "https://hearthstone.blizzard.com/en-us/battlegrounds?minionType=murloc&tier=2";
            minions.AddRange(await scrapeMinionsOnPage(path2, "Murloc", 2));
            string path3 = "https://hearthstone.blizzard.com/en-us/battlegrounds?minionType=murloc&tier=3";
            minions.AddRange(await scrapeMinionsOnPage(path3, "Murloc", 3));
            string path4 = "https://hearthstone.blizzard.com/en-us/battlegrounds?minionType=murloc&tier=4";
            minions.AddRange(await scrapeMinionsOnPage(path4, "Murloc", 4));
            string path5 = "https://hearthstone.blizzard.com/en-us/battlegrounds?minionType=murloc&tier=5";
            minions.AddRange(await scrapeMinionsOnPage(path5, "Murloc", 5));
            string path6 = "https://hearthstone.blizzard.com/en-us/battlegrounds?minionType=murloc&tier=6";
            minions.AddRange(await scrapeMinionsOnPage(path6, "Murloc", 6));
            string path7 = "https://hearthstone.blizzard.com/en-us/battlegrounds?minionType=murloc&tier=7";
            minions.AddRange(await scrapeMinionsOnPage(path7, "Murloc", 7));
            return minions;

        }

        public async Task<List<Minion>> ScrapeNaga()
        {
            Console.WriteLine("Scraping Naga");
            var minions = new List<Minion>();
            string path1 = "https://hearthstone.blizzard.com/en-us/battlegrounds?minionType=naga&tier=1";
            minions.AddRange(await scrapeMinionsOnPage(path1, "Naga", 1));
            string path2 = "https://hearthstone.blizzard.com/en-us/battlegrounds?minionType=naga&tier=2";
            minions.AddRange(await scrapeMinionsOnPage(path2, "Naga", 2));
            string path3 = "https://hearthstone.blizzard.com/en-us/battlegrounds?minionType=naga&tier=3";
            minions.AddRange(await scrapeMinionsOnPage(path3, "Naga", 3));
            string path4 = "https://hearthstone.blizzard.com/en-us/battlegrounds?minionType=naga&tier=4";
            minions.AddRange(await scrapeMinionsOnPage(path4, "Naga", 4));
            string path5 = "https://hearthstone.blizzard.com/en-us/battlegrounds?minionType=naga&tier=5";
            minions.AddRange(await scrapeMinionsOnPage(path5, "Naga", 5));
            string path6 = "https://hearthstone.blizzard.com/en-us/battlegrounds?minionType=naga&tier=6";
            minions.AddRange(await scrapeMinionsOnPage(path6, "Naga", 6));
            string path7 = "https://hearthstone.blizzard.com/en-us/battlegrounds?minionType=naga&tier=7";
            minions.AddRange(await scrapeMinionsOnPage(path7, "Naga", 7));
            return minions;
        }

        public async Task<List<Minion>> ScrapePirates()
        {
            Console.WriteLine("Scraping Pirates");
            var minions = new List<Minion>();

            string path1 = "https://hearthstone.blizzard.com/en-us/battlegrounds?minionType=pirate&tier=1";
            minions.AddRange(await scrapeMinionsOnPage(path1, "Pirate", 1));
            string path2 = "https://hearthstone.blizzard.com/en-us/battlegrounds?minionType=pirate&tier=2";
            minions.AddRange(await scrapeMinionsOnPage(path2, "Pirate", 2));
            string path3 = "https://hearthstone.blizzard.com/en-us/battlegrounds?minionType=pirate&tier=3";
            minions.AddRange(await scrapeMinionsOnPage(path3, "Pirate", 3));
            string path4 = "https://hearthstone.blizzard.com/en-us/battlegrounds?minionType=pirate&tier=4";
            minions.AddRange(await scrapeMinionsOnPage(path4, "Pirate", 4));
            string path5 = "https://hearthstone.blizzard.com/en-us/battlegrounds?minionType=pirate&tier=5";
            minions.AddRange(await scrapeMinionsOnPage(path5, "Pirate", 5));
            string path6 = "https://hearthstone.blizzard.com/en-us/battlegrounds?minionType=pirate&tier=6";
            minions.AddRange(await scrapeMinionsOnPage(path6, "Pirate", 6));
            string path7 = "https://hearthstone.blizzard.com/en-us/battlegrounds?minionType=pirate&tier=7";
            minions.AddRange(await scrapeMinionsOnPage(path7, "Pirate", 7));

            return minions;
        }

        public async Task<List<Minion>> ScrapeQuilboar()
        {
            Console.WriteLine("Scraping Quilboar");
            var minions = new List<Minion>();

            string path1 = "https://hearthstone.blizzard.com/en-us/battlegrounds?minionType=quilboar&tier=1";
            minions.AddRange(await scrapeMinionsOnPage(path1, "Quilboar", 1));
            string path2 = "https://hearthstone.blizzard.com/en-us/battlegrounds?minionType=quilboar&tier=2";
            minions.AddRange(await scrapeMinionsOnPage(path2, "Quilboar", 2));
            string path3 = "https://hearthstone.blizzard.com/en-us/battlegrounds?minionType=quilboar&tier=3";
            minions.AddRange(await scrapeMinionsOnPage(path3, "Quilboar", 3));
            string path4 = "https://hearthstone.blizzard.com/en-us/battlegrounds?minionType=quilboar&tier=4";
            minions.AddRange(await scrapeMinionsOnPage(path4, "Quilboar", 4));
            string path5 = "https://hearthstone.blizzard.com/en-us/battlegrounds?minionType=quilboar&tier=5";
            minions.AddRange(await scrapeMinionsOnPage(path5, "Quilboar", 5));
            string path6 = "https://hearthstone.blizzard.com/en-us/battlegrounds?minionType=quilboar&tier=6";
            minions.AddRange(await scrapeMinionsOnPage(path6, "Quilboar", 6));
            string path7 = "https://hearthstone.blizzard.com/en-us/battlegrounds?minionType=quilboar&tier=7";
            minions.AddRange(await scrapeMinionsOnPage(path7, "Quilboar", 7));
            return minions;
        }

        public async Task<List<Minion>> ScrapeUndead()
        {
            Console.WriteLine("Scraping Murlocs");
            var minions = new List<Minion>();

            string path1 = "https://hearthstone.blizzard.com/en-us/battlegrounds?minionType=undead&tier=1";
            minions.AddRange(await scrapeMinionsOnPage(path1, "Undead", 1));
            string path2 = "https://hearthstone.blizzard.com/en-us/battlegrounds?minionType=undead&tier=2";
            minions.AddRange(await scrapeMinionsOnPage(path2, "Undead", 2));
            string path3 = "https://hearthstone.blizzard.com/en-us/battlegrounds?minionType=undead&tier=3";
            minions.AddRange(await scrapeMinionsOnPage(path3, "Undead", 3));
            string path4 = "https://hearthstone.blizzard.com/en-us/battlegrounds?minionType=undead&tier=4";
            minions.AddRange(await scrapeMinionsOnPage(path4, "Undead", 4));
            string path5 = "https://hearthstone.blizzard.com/en-us/battlegrounds?minionType=undead&tier=5";
            minions.AddRange(await scrapeMinionsOnPage(path5, "Undead", 5));
            string path6 = "https://hearthstone.blizzard.com/en-us/battlegrounds?minionType=undead&tier=6";
            minions.AddRange(await scrapeMinionsOnPage(path6, "Undead", 6));
            string path7 = "https://hearthstone.blizzard.com/en-us/battlegrounds?minionType=undead&tier=7";
            minions.AddRange(await scrapeMinionsOnPage(path7, "Undead", 7));

            return minions;
        }
    }
}
