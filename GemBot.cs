using Sfs2X.Entities;
using Sfs2X.Entities.Data;
namespace bot;

public class GemBot : BaseBot
{
    private bool IsBotTurn => botPlayer.Id == currentPlayerId;
    internal void Load()
    {
        Console.WriteLine("Bot.Load()");
    }

    internal void Update(TimeSpan gameTime)
    {
        Console.WriteLine("Bot.Update()");
    }

    protected override void StartGame(ISFSObject gameSession, Room room)
    {
        // Assign Bot player & enemy player
        AssignPlayers(room);

        // Player & Heroes
        var objBotPlayer = gameSession.GetSFSObject(botPlayer.Name);
        var objEnemyPlayer = gameSession.GetSFSObject(enemyPlayer.Name);

        var botPlayerHero = objBotPlayer.GetSFSArray("heroes");
        var enemyPlayerHero = objEnemyPlayer.GetSFSArray("heroes");

        for (var i = 0; i < botPlayerHero.Size(); i++)
        {
            var hero = new Hero(botPlayerHero.GetSFSObject(i));
            botPlayer.Heroes.Add(hero);
            Console.WriteLine($"{hero.Name} - {hero.PrintGemType()} - {hero.Hp}");
        }

        for (var i = 0; i < enemyPlayerHero.Size(); i++)
        {
            enemyPlayer.Heroes.Add(new Hero(enemyPlayerHero.GetSFSObject(i)));
        }

        // Gems
        grid = new Grid(gameSession.GetSFSArray("gems"), botPlayer.GetRecommendGemType());
        currentPlayerId = gameSession.GetInt("currentPlayerId");
        log("StartGame ");

        // SendFinishTurn(true);
        //taskScheduler.schedule(new FinishTurn(true), new Date(System.currentTimeMillis() + delaySwapGem));
        TaskSchedule(delaySwapGem, _ => SendFinishTurn(true));
    }

    protected override void SwapGem(ISFSObject paramz)
    {
        var isValidSwap = paramz.GetBool("validSwap");
        if (!isValidSwap)
        {
            return;
        }

        HandleGems(paramz);
    }

    protected override void HandleGems(ISFSObject paramz)
    {
        var gameSession = paramz.GetSFSObject("gameSession");
        currentPlayerId = gameSession.GetInt("currentPlayerId");
        //get last snapshot
        var snapshotSfsArray = paramz.GetSFSArray("snapshots");
        var lastSnapshot = snapshotSfsArray.GetSFSObject(snapshotSfsArray.Size() - 1);
        var needRenewBoard = paramz.ContainsKey("renewBoard");
        // update information of hero
        HandleHeroes(lastSnapshot);
        if (needRenewBoard)
        {
            grid.UpdateGems(paramz.GetSFSArray("renewBoard"));
            TaskSchedule(delaySwapGem, _ => SendFinishTurn(false));
            return;
        }

        // update gem
        grid.GemTypes = botPlayer.GetRecommendGemType();
        grid.UpdateGems(lastSnapshot.GetSFSArray("gems"));
        TaskSchedule(delaySwapGem, _ => SendFinishTurn(false));
    }

    private void HandleHeroes(ISFSObject paramz)
    {
        var heroesBotPlayer = paramz.GetSFSArray(botPlayer.Name);
        for (var i = 0; i < botPlayer.Heroes.Count; i++)
        {
            botPlayer.Heroes[i].updateHero(heroesBotPlayer.GetSFSObject(i));
            Console.WriteLine();
        }

        var heroesEnemyPlayer = paramz.GetSFSArray(enemyPlayer.Name);
        for (var i = 0; i < enemyPlayer.Heroes.Count; i++)
        {
            enemyPlayer.Heroes[i].updateHero(heroesEnemyPlayer.GetSFSObject(i));
        }
    }

    protected override void StartTurn(ISFSObject paramz)
    {
        currentPlayerId = paramz.GetInt("currentPlayerId");
        if (IsBotTurn)
        {
            return;
        }

        var heroFullMana = botPlayer.FirstHeroFullMana;
        if (heroFullMana != null)
        {
            TaskSchedule(delaySwapGem, _ => SendCastSkill(heroFullMana));
            return;
        }

        TaskSchedule(delaySwapGem, _ => SendSwapGem());
    }
}
