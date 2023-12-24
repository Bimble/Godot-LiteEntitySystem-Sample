using Sample.Server;
using Sample.Shared;

ServerLogic gameServer = new();
gameServer.Awake();
using PeriodicTimer timer = new(TimeSpan.FromSeconds(1d / NetworkGeneral.GameFPS));
while (await timer.WaitForNextTickAsync())
    gameServer.Update();