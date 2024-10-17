using System;
using System.Collections.Generic;
using System.Net;
using Microsoft.EntityFrameworkCore;
using HSBGHelper.Server.Models;
using HSBGHelper.Server.Data;
using Microsoft.Extensions.Configuration;
using PuppeteerSharp;
using Microsoft.Identity.Client;

namespace HSBGHelper.Utilities
{
    public class Program
    {
        public Program()
        {
            Console.WriteLine("HSBGHelper.Utilities");
            Console.WriteLine("Starting Program");
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
                context.Users.RemoveRange(context.Users);

                var program = new Program();

                string input = "";

                Console.WriteLine("Emtpy and Rebuild/Scrape Database? (y/n)");
                input = Console.ReadLine();

                if (input.ToUpper() == "Y")
                {
                    context.Users.Add(new User()
                    {
                        Name = "admin",
                        Email = "zackmorgenthaler@gmail.com",
                        Password = "Password_1234"
                    });

                    context.SaveChanges();
                    await program.ScrapeAllHeroInformation(context);
                    await program.SetHeroMode(context);
                    await program.ScrapeMinions(context);
                    await program.SetMinionMode(context);
                    await program.ScrapeSpells(context);
                    await program.SetSpellMode(context);
                    await program.ScrapeGreaterTrinkets(context);
                    await program.ScrapeLesserTrinkets(context);
                    await program.SetTrinketMode(context);
                }
                else
                {
                    Console.WriteLine("Add admin account?");
                    Console.WriteLine("Y/N");
                    input = Console.ReadLine();

                    if (input.ToUpper() == "Y")
                    {
                        context.Users.Add(new User()
                        {
                            Name = "admin",
                            Email = "zackmorgenthaler@gmail.com",
                            Password = "Ibanez_RG550"
                        });

                        context.SaveChanges();
                    }

                    Console.WriteLine("Scraping all minion information?");
                    Console.WriteLine("Y/N");

                    input = Console.ReadLine();

                    if (input.ToUpper() == "Y")
                    {
                        await program.ScrapeMinions(context);
                        await program.SetMinionMode(context);
                    }

                    Console.WriteLine("Scraping all hero information?");
                    Console.WriteLine("Y/N");

                    input = Console.ReadLine();

                    if (input.ToUpper() == "Y")
                    {
                        await program.ScrapeAllHeroInformation(context);
                        await program.SetHeroMode(context);
                    }

                    Console.WriteLine("Scraping all spell information?");
                    Console.WriteLine("Y/N");

                    input = Console.ReadLine();

                    if (input.ToUpper() == "Y")
                    {
                        await program.ScrapeSpells(context);
                        await program.SetSpellMode(context);
                    }

                    Console.WriteLine("Scraping all trinket information?");
                    Console.WriteLine("Y/N");

                    input = Console.ReadLine();

                    if (input.ToUpper() == "Y")
                    {
                        await program.ScrapeGreaterTrinkets(context);
                        await program.ScrapeLesserTrinkets(context);
                        await program.SetTrinketMode(context);
                    }
                }
            }
        }


        public async Task ScrapeMinions(HSBGDb context)
        {
            Console.WriteLine("Scraping minions");
            var minions = new List<Minion>();

            context.Minions.RemoveRange(context.Minions);
            context.SaveChanges();

            for (int i = 1; i <= 7; i++)
            {
                await Task.Delay(1000);
                var path = $"https://hearthstone.blizzard.com/en-us/battlegrounds?bgCardType=minion&tier={i}";
                minions.AddRange(await scrapeMinionsOnPage(path, i));
            }

            // await DownloadImagesAsync(minions);

            context.Minions.AddRange(minions);
            context.SaveChanges();
        }
        public async Task<List<Minion>> scrapeMinionsOnPage(string path, int tier)
        {
            // we gotta literally download chrome to start this up
            var browserFetcher = new BrowserFetcher();
            await browserFetcher.DownloadAsync();
            await using var Browser = await Puppeteer.LaunchAsync(new LaunchOptions { Headless = false });

            // prepare minion list
            var minions = new List<Minion>();

            // create a new page
            var page = await Browser.NewPageAsync();

            // go to the page and wait for the selector to load
            await page.GoToAsync(path);

            // wait till dom content is loaded
            

            await page.WaitForSelectorAsync("body");
            await Task.Delay(5000);
            
            await page.WaitForSelectorAsync("#MainCardGrid .CardImage");
            var minionNodes = await page.QuerySelectorAllAsync("#MainCardGrid .CardImage");


            foreach (var minionNode in minionNodes)
            {
                var name = await minionNode.EvaluateFunctionAsync<string>("e => e.alt");
                var image = await minionNode.EvaluateFunctionAsync<string>("e => e.src");

                minionNode.ClickAsync().Wait();
                await Task.Delay(1000);
                // wait a second to avoid whatever the fuck is happening 
                
                await page.WaitForSelectorAsync("[class*=CardDetailsLayout__CardFlavorText]");

                var descriptionNode = await page.QuerySelectorAsync("[class*=CardDetailsLayout__CardFlavorText]");
                var description = await descriptionNode.EvaluateFunctionAsync<string>("e => e.innerHTML");

                var keywords = new List<string>();
                try
                {
                    // await page.WaitForSelectorAsync("[class*=CardKeywords__KeywordsList] a");
                    var keywordsNodes = await page.QuerySelectorAllAsync("[class*=CardKeywords__KeywordsList] a");
                    foreach (var keywordNode in keywordsNodes)
                    {
                        var keyword = await keywordNode.EvaluateFunctionAsync<string>("e => e.innerText");
                        keywords.Add(keyword);
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine("Exception found: " + e.ToString());
                    Console.WriteLine("No keywords found");
                }

                var type = "Neutral";

                try
                {
                    // find type via .value selector
                    await page.WaitForSelectorAsync(".DPOYe li:nth-child(3) .value");
                    var typeNode = await page.QuerySelectorAsync(".DPOYe li:nth-child(3) .value");

                    type = await typeNode.EvaluateFunctionAsync<string>("e => e.innerText");
                }
                catch (Exception e)
                {
                    Console.WriteLine("No type found - labeled Neutral");
                }


                Console.WriteLine("Minion found: " + name + ", Type: " + type);

                minions.Add(new Minion()
                {
                    Name = name,
                    Image = image,
                    Type = type,
                    Tier = tier,
                    Description = description,
                    HtmlGuide = "",
                    heroSynergies = new List<Hero>(),
                    minionSynergies = new List<Minion>(),
                    spellSynergies = new List<Spell>(),
                    Keywords = keywords,
                    inDuosMode = false,
                    inSoloMode = false
                });

                // close the modal
                await page.WaitForSelectorAsync("[class*=Modal__ModalContent] [class*=CloseButton]");
                await page.ClickAsync("[class*=Modal__ModalContent] [class*=CloseButton]");
            }


            Browser.CloseAsync().Wait();

            return minions;
        }
        private async Task SetMinionMode(HSBGDb context)
        {
            string solosPath = "https://hearthstone.blizzard.com/en-us/battlegrounds?bgCardType=minion&bgGameMode=solos";
            string duosPath = "https://hearthstone.blizzard.com/en-us/battlegrounds?bgCardType=minion&bgGameMode=duos";

            List<string> soloMinionNames = new List<string>();
            List<string> duoMinionNames = new List<string>();

            // get name of all solo minions

            var browserFetcher = new BrowserFetcher();
            await browserFetcher.DownloadAsync();
            await using var Browser = await Puppeteer.LaunchAsync(new LaunchOptions { Headless = true });

            var page = await Browser.NewPageAsync();


            // Get solo mode heroes
            await page.GoToAsync(solosPath);
            await page.WaitForSelectorAsync("body");

            await page.WaitForSelectorAsync("img.CardImage");

            var soloMinionNodes = await page.QuerySelectorAllAsync("img.CardImage");

            foreach (var soloMinionNode in soloMinionNodes)
            {
                var name = await soloMinionNode.EvaluateFunctionAsync<string>("e => e.alt");
                soloMinionNames.Add(name);
            }

            // Get duo mode heroes
            await page.GoToAsync(duosPath);

            await page.WaitForSelectorAsync("body");
            await page.WaitForSelectorAsync("img.CardImage");

            var duoMinionNodes = await page.QuerySelectorAllAsync("img.CardImage");

            foreach (var duoMinionNode in duoMinionNodes)
            {
                var name = await duoMinionNode.EvaluateFunctionAsync<string>("e => e.alt");
                duoMinionNames.Add(name);
            }

            var minions = context.Minions.ToList();

            foreach (var minion in minions)
            {
                if (soloMinionNames.Contains(minion.Name))
                {
                    Console.WriteLine(minion.Name + " is in both modes");
                    minion.inSoloMode = true;
                }
                else if (duoMinionNames.Contains(minion.Name))
                {
                    Console.WriteLine(minion.Name + " is in duo mode");
                    minion.inDuosMode = true;
                }
                else
                {
                    Console.WriteLine(minion.Name + " not found in either mode");
                }
            }

            context.Minions.UpdateRange(minions);
            await context.SaveChangesAsync();
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
            await Task.Delay(5000);

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

                LesserTrinkets.Add(new LesserTrinket() { Name = name, Description = description, Image = image, HtmlGuide = "", Cost = 0, Tier = 'F' });
            }
            context.LesserTrinkets.AddRange(LesserTrinkets);
            context.SaveChanges();

            await Browser.CloseAsync();
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

                GreaterTrinkets.Add(new GreaterTrinket() { Name = name, Description = description, Image = image, HtmlGuide = "", Cost = 0, Tier = 'F' });

                await page.ClickAsync(".knbYrP");
            }
            await Browser.CloseAsync();

            await context.GreaterTrinkets.AddRangeAsync(GreaterTrinkets);
            await context.SaveChangesAsync();
        }
        private async Task SetTrinketMode(HSBGDb context)
        {
            string solosGreaterPath = "https://hearthstone.blizzard.com/en-us/battlegrounds?bgCardType=trinket&bgGameMode=solos&spellSchool=greater_trinket";
            string solosLesserPath = "https://hearthstone.blizzard.com/en-us/battlegrounds?bgCardType=trinket&bgGameMode=solos&spellSchool=lesser_trinket";
            string duosGreaterPath = "https://hearthstone.blizzard.com/en-us/battlegrounds?bgCardType=trinket&bgGameMode=duos&spellSchool=greater_trinket";
            string duosLesserPath = "https://hearthstone.blizzard.com/en-us/battlegrounds?bgCardType=trinket&bgGameMode=duos&spellSchool=lesser_trinket";

            var browserFetcher = new BrowserFetcher();
            await browserFetcher.DownloadAsync();
            await using var Browser = await Puppeteer.LaunchAsync(new LaunchOptions { Headless = true });

            var page = await Browser.NewPageAsync();

            // Get solo mode greater trinkets
            var soloGreaterTrinketNames = await GetTrinketNamesFromPage(page, solosGreaterPath);

            // Get duo mode greater trinkets
            var duoGreaterTrinketNames = await GetTrinketNamesFromPage(page, duosGreaterPath);

            // Set greater trinket mode info
            var greaterTrinkets = context.GreaterTrinkets.ToList();
            foreach (var greaterTrinket in greaterTrinkets)
            {
                greaterTrinket.IsInDuos = duoGreaterTrinketNames.Contains(greaterTrinket.Name);
                greaterTrinket.IsInSolos = soloGreaterTrinketNames.Contains(greaterTrinket.Name);
                Console.WriteLine($"{greaterTrinket.Name} | Solo: {greaterTrinket.IsInSolos}, Duo: {greaterTrinket.IsInDuos}");
            }

            // Get solo mode lesser trinkets
            var soloLesserTrinketNames = await GetTrinketNamesFromPage(page, solosLesserPath);

            // Get duo mode lesser trinkets
            var duoLesserTrinketNames = await GetTrinketNamesFromPage(page, duosLesserPath);

            // Set lesser trinket mode info
            var lesserTrinkets = context.LesserTrinkets.ToList();
            foreach (var lesserTrinket in lesserTrinkets)
            {
                lesserTrinket.IsInDuos = duoLesserTrinketNames.Contains(lesserTrinket.Name);
                lesserTrinket.IsInSolos = soloLesserTrinketNames.Contains(lesserTrinket.Name);
                Console.WriteLine($"{lesserTrinket.Name} | Solo: {lesserTrinket.IsInSolos}, Duo: {lesserTrinket.IsInDuos}");
            }
            context.LesserTrinkets.UpdateRange(lesserTrinkets);
            context.GreaterTrinkets.UpdateRange(greaterTrinkets);

            await context.SaveChangesAsync();

            await Browser.CloseAsync();
        }
        private async Task<List<string>> GetTrinketNamesFromPage(IPage page, string url)
        {
            await page.GoToAsync(url);
            await page.WaitForSelectorAsync("img.CardImage");

            var trinketNodes = await page.QuerySelectorAllAsync("img.CardImage");
            var trinketNames = new List<string>();

            foreach (var trinketNode in trinketNodes)
            {
                var name = await trinketNode.EvaluateFunctionAsync<string>("e => e.alt");
                trinketNames.Add(name);
            }

            return trinketNames;
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
                await Task.Delay(3000);

                await page.WaitForSelectorAsync("#MainCardGrid .CardImage");
                var spellNodes = await page.QuerySelectorAllAsync("#MainCardGrid .CardImage");
                foreach (var spellNode in spellNodes)
                {
                    var name = await spellNode.EvaluateFunctionAsync<string>("e => e.alt");
                    var image = await spellNode.EvaluateFunctionAsync<string>("e => e.src");
                    Console.WriteLine("Spell Found: " + name);
                    spells.Add(new Spell()
                    {
                        Name = name,
                        Image = image,
                        Tier = i,
                        HtmlGuide = "",
                        spellSynergies = new List<Spell>(),
                        minionSynergies = new List<Minion>(),
                        heroSynergies = new List<Hero>(),
                        isInSoloMode = false,
                        isInDuosMode = false
                    });
                }
            }

            Browser.CloseAsync().Wait();

            context.Spells.AddRange(spells);
            context.SaveChanges();
        }
        private async Task SetSpellMode(HSBGDb context)
        {
            string solosPath = "https://hearthstone.blizzard.com/en-us/battlegrounds?bgCardType=spell&bgGameMode=solos";
            string duosPath = "https://hearthstone.blizzard.com/en-us/battlegrounds?bgCardType=spell&bgGameMode=duos";

            List<string> soloSpellNames = new List<string>();
            List<string> duoSpellNames = new List<string>();

            // get name of all solo minions

            var browserFetcher = new BrowserFetcher();
            await browserFetcher.DownloadAsync();
            await using var Browser = await Puppeteer.LaunchAsync(new LaunchOptions { Headless = true });

            var page = await Browser.NewPageAsync();

            // Get solo mode heroes
            await page.GoToAsync(solosPath);

            await page.WaitForSelectorAsync("img.CardImage");

            var soloSpellNodes = await page.QuerySelectorAllAsync("img.CardImage");

            foreach (var soloSpellNode in soloSpellNodes)
            {
                var name = await soloSpellNode.EvaluateFunctionAsync<string>("e => e.alt");
                soloSpellNames.Add(name);
            }

            // Get duo mode heroes
            await page.GoToAsync(duosPath);

            await page.WaitForSelectorAsync("img.CardImage");

            var duoSpellNodes = await page.QuerySelectorAllAsync("img.CardImage");

            foreach (var duoSpellNode in duoSpellNodes)
            {
                var name = await duoSpellNode.EvaluateFunctionAsync<string>("e => e.alt");
                duoSpellNames.Add(name);
            }

            var spells = context.Spells.ToList();

            foreach (var spell in spells)
            {
                if (soloSpellNames.Contains(spell.Name))
                {
                    Console.WriteLine(spell.Name + " is in both modes");
                    spell.isInSoloMode = true;
                }
                else if (duoSpellNames.Contains(spell.Name))
                {
                    Console.WriteLine(spell.Name + " is in solo mode");
                    spell.isInDuosMode = true;
                }
            }
        }
        public async Task ScrapeAllHeroInformation(HSBGDb context)
        {
            var heroesPage = "https://hearthstone.blizzard.com/en-us/battlegrounds?bgCardType=hero";

            // we gotta literally download chrome to start this up
            var browserFetcher = new BrowserFetcher();
            await browserFetcher.DownloadAsync();
            await using var Browser = await Puppeteer.LaunchAsync(new LaunchOptions { Headless = false  });

            // prepare minion list
            var heros = new List<Hero>();

            // create a new page
            var page = await Browser.NewPageAsync();

            // go to the page and wait for the selector to load
            await page.GoToAsync(heroesPage);
            await page.WaitForSelectorAsync("body");

            await Task.Delay(5000);

            await page.WaitForSelectorAsync(".CardWrap.hero");
            

            var heroLinks = await page.QuerySelectorAllAsync(".CardWrap.hero");

            foreach (var heroNode in heroLinks)
            {
                // find the hero image
                await page.WaitForSelectorAsync(".CardImage.hero");
                var heroLinksImage = await heroNode.QuerySelectorAsync(".CardImage.hero");

                var name = await heroLinksImage.EvaluateFunctionAsync<string>("e => e.alt");
                var image = await heroLinksImage.EvaluateFunctionAsync<string>("e => e.src");

                Console.WriteLine(name);
                // click the hero portrait
                await heroLinksImage.ClickAsync();

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
                    heroSynergies = new List<Hero>(),
                    Armor = 0,
                    inSoloMode = false,
                    inDuosMode = false
                });

                // add a delay
                await Task.Delay(250);

                // close the modal
                await page.WaitForSelectorAsync(".knbYrP");
                await page.ClickAsync(".knbYrP");
            }
            context.Heroes.AddRange(heros);
            await context.SaveChangesAsync();

            Browser.CloseAsync().Wait();
        }
        private async Task SetHeroMode(HSBGDb context)
        {
            var herosNoMode = context.Heroes.ToList();

            string solosPath = "https://hearthstone.blizzard.com/en-us/battlegrounds?bgCardType=hero&bgGameMode=solos";
            string duosPath = "https://hearthstone.blizzard.com/en-us/battlegrounds?bgCardType=hero&bgGameMode=duos";

            var browserFetcher = new BrowserFetcher();
            await browserFetcher.DownloadAsync();
            await using var Browser = await Puppeteer.LaunchAsync(new LaunchOptions { Headless = true });

            var page = await Browser.NewPageAsync();

            // Get solo mode heroes
            await page.GoToAsync(solosPath);
            await page.WaitForSelectorAsync("img.hero");

            var soloHeroNodes = await page.QuerySelectorAllAsync("img.hero");

            var soloHeroNames = new List<string>();
            foreach (var soloHeroNode in soloHeroNodes)
            {
                var name = await soloHeroNode.EvaluateFunctionAsync<string>("e => e.alt");
                Console.WriteLine("Solo Hero Found: " + name);
                soloHeroNames.Add(name);
            }

            // Get duo mode heroes
            await page.GoToAsync(duosPath);
            await page.WaitForSelectorAsync("img.hero");

            var duosHeroNodes = await page.QuerySelectorAllAsync("img.hero");

            var duoHeroNames = new List<string>();
            foreach (var duoHeroNode in duosHeroNodes)
            {
                var name = await duoHeroNode.EvaluateFunctionAsync<string>("e => e.alt");
                Console.WriteLine("Duo Hero Found: " + name);
                duoHeroNames.Add(name); // Add to the correct list
            }

            // Categorize heroes into three modes: Solo, Duo, or Both
            foreach (var hero in herosNoMode)
            {
                if (soloHeroNames.Contains(hero.Name))
                {
                    Console.WriteLine(hero.Name + " is in both modes");
                    hero.inSoloMode = true;
                }
                else if (duoHeroNames.Contains(hero.Name))
                {
                    Console.WriteLine(hero.Name + " is in solo mode only");
                    hero.inDuosMode = true;
                }
            }

            // Update heroes in the database
            context.Heroes.UpdateRange(herosNoMode);
            await context.SaveChangesAsync();

            // Close the browser
            await Browser.CloseAsync();
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
    }
}
