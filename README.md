# HSBGHelper
A Blazor app that helps Hearthstone: Battlegrounds players make intelligent decisions about how to play.


[https://hsbghelper.com/](https://hsbghelper.com/)

# Minder's Notes
- remove modal -> go straight to information/guide pages
- 3 tiers of trinkets - use color coding
    - standard? neutral and always offered
    - pivot: dictated your build
        - ex automaton
        - bomb
    - tribe based trinkets
- note: https://docs.google.com/spreadsheets/d/1Cg5OU_IYcXDLiqZ8CCcF0L4quyeP28CZftv77XR3UDs/edit?gid=153329801#gid=153329801
- make calculator (https://hs-tools.github.io/bg-probabilities/)
- finally get around to laying out desktop
- add tiers for heros, trinkets, etc. on bottom
- seperate duos, solos for heros
    - solos default


## Note:
In development for the newest season of Battlegrounds. (Trinket meta)

## How to run:
- Clone this repository and open the directory in powershell.
- `cd "./HSBGHelper/HSBGHelper.Utilities"`
- `dotnet restore`
- `dotnet run`
- This may take a while as it scrapes official hearthstone servers for all the data they have.
- `cd ..`
- `cd "./HSBGHelper/HSBGHelper.Server"`
- `dotnet restore`
- `dotnet run`
- Check the ip address and port listed in the cmd to view the site!

## Administration
- `/admin/edit-heroes`
- `/admin/edit-buddies`
- `/admin/edit-minions`
- `/admin/edit-spells`

You can use these pages to add data to the the different database elements. ...There are hundreds of them! Have fun!

## Overview
- Based off of JeefHS's [HSBGGuide](hsbgguide.com.)
- Contains all scrapable data from [Hearthstone Official site](https://hearthstone.blizzard.com/en-us/battlegrounds).
- Stretch Goal:
    - Will try and include data from HSReplay's battleground [website](https://hsreplay.net/battlegrounds/heroes/) if I get that far. 

## To Do:
- Handle synergies and useful information about each card in the game

## About HSBGHelper
- uses WASM as well as Blazor Interactive Server-side methods

### Developer Notes
- https://learn.microsoft.com/en-us/ef/core/get-started/overview/first-app?tabs=netcore-cli
    - entity framework scrits are here for reference.