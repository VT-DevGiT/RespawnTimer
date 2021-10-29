using MEC;
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
        SynapseMajor = 2,
        SynapseMinor = 7,
        SynapsePatch = 2,
        Version = "v.2.1.0"
        )]
    public class Plugin : AbstractPlugin
    {
        [SynapseTranslation]
        public static SynapseTranslation<Translations> Trlte { get; set; }

        public override void Load()
        {
            Server.Get.Events.Round.RoundStartEvent += OnRoundStart;
            Trlte.AddTranslation(new Translations());
            Trlte.AddTranslation(new Translations
            {
                and = "et",
                CI = "<color=army_green>Insurgé du Chaos</color>",
                NTF = "<color=blue>Nine-Tailed Fox</color>",
                min = "Minute",
                sec = "seconde",
                YoullRes = "<color=#ffa31a>Vous allez respawn dans:</color>",
                YoullBe = "<color=yellow>Vous allez respawn en:</color>"

            }, "FRENCH");
            Trlte.AddTranslation(new Translations
            {
                and = "und",
                CI = "<color=army_green>Chaos-Aufstand</color>",
                NTF = "<color=blue>Nine-Tailed Fox</color>",
                min = "Minuten",
                sec = "Sekunde",
                YoullRes = "<color=#ffa31a>Du wirst wieder spawnen:</color>",
                YoullBe = "<color=yellow>Du wirst wieder spawnen:</color>",
                AutoS = false

            }, "GERMAN");
            base.Load();

        }
        public void OnRoundStart()
        {
            var timerCoroutine = Timing.RunCoroutine(TimerShow());
            if (timerCoroutine.IsRunning)
            {
                Timing.KillCoroutines(timerCoroutine);
            }
            timerCoroutine = Timing.RunCoroutine(TimerShow());
        }
        private IEnumerator<float> TimerShow()
        {
            while (Round.Get.RoundIsActive)
            {
                yield return Timing.WaitForSeconds(1);

                try
                {
                    int res = (int)Math.Round(Map.Get.Round.NextRespawn);
                    string text;
                    bool boolean = Trlte.ActiveTranslation.AutoS;
                    if (res % 60 == 0)
                    {
                        text = $"{Trlte.ActiveTranslation.YoullRes} \n {res / 60} {Trlte.ActiveTranslation.min}{(res / 60 <= 1 || boolean ? "" : "s")}";
                        if ((int)Respawning.RespawnManager.Singleton.NextKnownTeam == 1)
                        {
                            text += "\n";
                            text += $"{Trlte.ActiveTranslation.YoullBe} {Trlte.ActiveTranslation.CI}";
                        }
                        else if ((int)Respawning.RespawnManager.Singleton.NextKnownTeam == 2)
                        {
                            text += "\n";
                            text += $"{Trlte.ActiveTranslation.YoullBe} {Trlte.ActiveTranslation.NTF}";
                        }
                    }
                    else if (res / 60 > 0)
                    {
                        text = $"{Trlte.ActiveTranslation.YoullRes} \n {(res / 60)} {Trlte.ActiveTranslation.min}{(res / 60 <= 1 || boolean ? "" : "s")} {Trlte.ActiveTranslation.and} { res % 60} {Trlte.ActiveTranslation.sec}{(res % 60 <= 1 || boolean ? "" : "s")}";
                        if ((int)Respawning.RespawnManager.Singleton.NextKnownTeam == 1)
                        {
                            text += "\n";
                            text += $"{Trlte.ActiveTranslation.YoullBe} {Trlte.ActiveTranslation.CI}";
                        }
                        else if ((int)Respawning.RespawnManager.Singleton.NextKnownTeam == 2)
                        {
                            text += "\n";
                            text += $"{Trlte.ActiveTranslation.YoullBe} {Trlte.ActiveTranslation.NTF}";
                        }
                    }
                    else
                    {
                        text = $"{Trlte.ActiveTranslation.YoullRes} \n { res % 60} {Trlte.ActiveTranslation.sec}{(res % 60 <= 1 || boolean ? "" : "s")}";
                        if ((int)Respawning.RespawnManager.Singleton.NextKnownTeam == 1)
                        {
                            text += "\n";
                            text += $"{Trlte.ActiveTranslation.YoullBe} {Trlte.ActiveTranslation.CI}";
                        }
                        else if ((int)Respawning.RespawnManager.Singleton.NextKnownTeam == 2)
                        {
                            text += "\n";
                            text += $"{Trlte.ActiveTranslation.YoullBe} {Trlte.ActiveTranslation.NTF}";
                        }
                    }
                    foreach (Player player in RoleType.Spectator.GetPlayers())
                    {
                        player.GiveTextHint($"{text}", 1);
                    }

                }
                catch (Exception err)
                {
                    SynapseController.Server.Logger.Warn($"Error: {err}");
                    continue;

                }
            }
        }

    }
}