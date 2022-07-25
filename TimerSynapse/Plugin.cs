using MEC;
using System.Linq;
using Synapse;
using Synapse.Api;
using Synapse.Api.Plugin;
using System;
using System.Collections.Generic;
using Synapse.Translation;

namespace TimerSynapse
{
    [PluginInformation(
        Name = "TimerSynapse",
        Author = "Antoniofo",
        Description = "Respawn Timer in Synapse",
        LoadPriority = 0,
        SynapseMajor = SynapseController.SynapseMajor,
        SynapseMinor = SynapseController.SynapseMinor,
        SynapsePatch = SynapseController.SynapsePatch,
        Version = "v.2.1.0"
        )]
    public class Plugin : AbstractPlugin
    {
        [SynapseTranslation]
        public static SynapseTranslation<Translations> Translations { get; set; }

        public override void Load()
        {
            Server.Get.Events.Round.RoundStartEvent += OnRoundStart;
            Translations.AddTranslation(new Translations());
            Translations.AddTranslation(new Translations
            {
                And = "et",
                CI = "<color=#004d00>Insurgé du Chaos</color>",
                NTF = "<color=blue>Nine-Tailed Fox</color>",
                Min = "Minute",
                Sec = "seconde",
                YoullRes = "<color=#ffa31a>Vous allez respawn dans:</color>",
                YoullBe = "<color=yellow>Vous allez respawn en:</color>",
                AutoS = true

            }, "FRENCH");
            Translations.AddTranslation(new Translations
            {
                And = "und",
                CI = "<color=#004d00>Chaos-Aufstand</color>",
                NTF = "<color=blue>Nine-Tailed Fox</color>",
                Min = "Minuten",
                Sec = "Sekunde",
                YoullRes = "<color=#ffa31a>Du wirst wieder spawnen:</color>",
                YoullBe = "<color=yellow>Du wirst wieder spawnen:</color>",
                AutoS = false

            }, "GERMAN");
            base.Load();
        }

        CoroutineHandle TimerHandler { get; set; }
        public void OnRoundStart()
        {
            if (TimerHandler.IsRunning)
            {
                Timing.KillCoroutines(TimerHandler);
            }
            TimerHandler = Timing.RunCoroutine(TimerShow());
        }

        private IEnumerator<float> TimerShow()
        {
            while (Round.Get.RoundIsActive) 
            {
                yield return Timing.WaitForSeconds(1);

                try
                {
                    int timeRespawn = (int)Math.Round(Map.Get.Round.NextRespawn), timeMinute = timeRespawn / 60, timeSecond = timeRespawn % 60; 
                    bool autoS = Translations.ActiveTranslation.AutoS;
                    string message;

                    if (timeSecond == 0)
                    {
                        message = $"{Translations.ActiveTranslation.YoullRes}\n{timeMinute} {Translations.ActiveTranslation.Min}{(timeRespawn <= 1 && autoS ? "" : "s")}";
                        if ((int)Respawning.RespawnManager.Singleton.NextKnownTeam == 1)
                        {
                            message += "\n";
                            message += $"{Translations.ActiveTranslation.YoullBe} {Translations.ActiveTranslation.CI}";
                        }
                        else if ((int)Respawning.RespawnManager.Singleton.NextKnownTeam == 2)
                        {
                            message += "\n";
                            message += $"{Translations.ActiveTranslation.YoullBe} {Translations.ActiveTranslation.NTF}";
                        }
                    }
                    else if (timeRespawn / 60 > 0)
                    {
                        message = $"{Translations.ActiveTranslation.YoullRes}\n{timeMinute} {Translations.ActiveTranslation.Min}{(timeRespawn <= 1 && autoS ? "" : "s")} {Translations.ActiveTranslation.And} {timeSecond} {Translations.ActiveTranslation.Sec}{(timeSecond <= 1 && autoS ? "" : "s")}";
                        if ((int)Respawning.RespawnManager.Singleton.NextKnownTeam == 1)
                        {
                            message += "\n";
                            message += $"{Translations.ActiveTranslation.YoullBe} {Translations.ActiveTranslation.CI}";
                        }
                        else if ((int)Respawning.RespawnManager.Singleton.NextKnownTeam == 2)
                        {
                            message += "\n";
                            message += $"{Translations.ActiveTranslation.YoullBe} {Translations.ActiveTranslation.NTF}";
                        }
                    }
                    else
                    {
                        message = $"{Translations.ActiveTranslation.YoullRes}\n{timeSecond} {Translations.ActiveTranslation.Sec}{(timeSecond <= 1 && autoS ? "" : "s")}";
                        if ((int)Respawning.RespawnManager.Singleton.NextKnownTeam == 1)
                        {
                            message += "\n";
                            message += $"{Translations.ActiveTranslation.YoullBe} {Translations.ActiveTranslation.CI}";
                        }
                        else if ((int)Respawning.RespawnManager.Singleton.NextKnownTeam == 2)
                        {
                            message += "\n";
                            message += $"{Translations.ActiveTranslation.YoullBe} {Translations.ActiveTranslation.NTF}";
                        }
                    }
                    foreach (var player in Server.Get.Players.Where(x => (int)x.RoleType == 2))
                    {
                        player.GiveTextHint($"{message}", 1.2f);
                    }
                }
                catch (Exception err)
                {
                    SynapseController.Server.Logger.Error($"Error: {err}");
                    continue;
                }
            }
        }
    }
}
