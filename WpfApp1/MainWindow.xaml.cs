using RestSharp;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WpfApp1.API;
using WpfApp1.Model;

namespace WpfApp1 {
	/// <summary>
	/// Lógica de interacción para MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window {

		public MainWindow() {
			InitializeComponent();

            GridSumary.Visibility = Visibility.Hidden;
            GridRaidProgress.Visibility = Visibility.Hidden;
            GridMythicProgression.Visibility = Visibility.Hidden;

            Console.WriteLine(AppDomain.CurrentDomain.BaseDirectory);
        }

        private void Button_Click(object sender, RoutedEventArgs e) {

            #region Validar datos
            SolidColorBrush danger = new SolidColorBrush(Color.FromRgb(196,48,47));
            SolidColorBrush normal = new SolidColorBrush(Color.FromRgb(255,255,255));

            if (STbName.Text == "") 
            {
                Console.WriteLine("Ingrese un nombre");
                SLbName.Foreground = danger;
                return;
            }
            SLbName.Foreground = normal;

            if (SCbRegion.Text == "") 
            {
                Console.WriteLine("Seleccione una region");
                SLbRegion.Foreground = danger;
                return;
            }
            SLbRegion.Foreground = normal;

            if (STbReino.Text == "") 
            {
                Console.WriteLine("Seleccione un reino");
                SLbReino.Foreground = danger;
                return;
            }
            SLbReino.Foreground = normal;

            string region = SCbRegion.Text;
            string realm = STbReino.Text;
            string name = STbName.Text;
            #endregion

            //Obtener datos de la api de raider io
            var charapter_raw = RaiderIoApi.GetCharacterInfo(region, realm, name);
            JObject charapter = JObject.Parse(charapter_raw);
            
            //Si no se encuentran datos del personaje
            if ((string)charapter["statusCode"] == "400")
            {
                Console.WriteLine("No se encontro el pj");
                return;
            }

            #region Obtener colores para los Scores
            var scoreColor_raw = RaiderIoApi.GetScoreTiersColors();
            JObject scoreColor = JObject.Parse(scoreColor_raw);

            SolidColorBrush scoreColorBrush = new SolidColorBrush();

            foreach (var color in scoreColor["colors"])
            {
                Console.WriteLine(color);
                double score = (int)color["score"];
                double pScore = (int)charapter["mythic_plus_scores_by_season"][0]["scores"]["all"];
                
                Console.WriteLine($"score = {score}, pScore = {pScore}");

                if (pScore >= score)
                {
                    byte r = (byte)color["rgbInteger"][0];
                    byte g = (byte)color["rgbInteger"][1];
                    byte b = (byte)color["rgbInteger"][2];
                    scoreColorBrush = new SolidColorBrush(Color.FromRgb(r,g,b));
                    break;
                }
            }
            #endregion

            GridSumary.Visibility = Visibility.Visible;
            GridRaidProgress.Visibility = Visibility.Visible;
            GridMythicProgression.Visibility = Visibility.Visible;

            #region Colocar imagen
            BitmapImage render = new BitmapImage();
            render.BeginInit();
            render.UriSource = new Uri((string)charapter["thumbnail_url"]);
            render.EndInit();
            ImgSumaryRender.Source = render;
            #endregion

            #region GridSumary
            double aaa = (double)charapter["mythic_plus_scores_by_season"][0]["scores"]["all"];
            int bbb = (int)Math.Ceiling(aaa);
            TbMythicScore.Text = bbb.ToString();
            TbMythicScore.Foreground = scoreColorBrush;
            TbRaidProgress.Text = (string)charapter["raid_progression"]["castle-nathria"]["summary"];
            LbCharapterName.Content = (string)charapter["name"];

            try {
                LbCharapterGuild.Content = $"<{(string)charapter["guild"]["name"]}>"; 
                LbCharapterGuildRealm.Content = $"({region}) {(string)charapter["realm"]}";
            }
            catch (Exception)
            {
                LbCharapterGuild.Content = $"({region}) {(string)charapter["realm"]}";
                LbCharapterGuildRealm.Content = "";
            }

            LbCharapterRace.Text = (string)charapter["race"];
            LbCharapterSpec.Text = $" {(string)charapter["active_spec_name"]} {(string)charapter["class"]}";

            if ((string)charapter["class"] == "Death Knight") LbCharapterSpec.Foreground = ClassColors.DeathKnight;
            if ((string)charapter["class"] == "Demon Hunter") LbCharapterSpec.Foreground = ClassColors.DemonHunter;
            if ((string)charapter["class"] == "Druid") LbCharapterSpec.Foreground = ClassColors.Druid;
            if ((string)charapter["class"] == "Hunter") LbCharapterSpec.Foreground = ClassColors.Hunter;
            if ((string)charapter["class"] == "Mage") LbCharapterSpec.Foreground = ClassColors.Mage;
            if ((string)charapter["class"] == "Monk") LbCharapterSpec.Foreground = ClassColors.Monk;
            if ((string)charapter["class"] == "Paladin") LbCharapterSpec.Foreground = ClassColors.Paladin;
            if ((string)charapter["class"] == "Priest") LbCharapterSpec.Foreground = ClassColors.Priest;
            if ((string)charapter["class"] == "Rogue") LbCharapterSpec.Foreground = ClassColors.Rogue;
            if ((string)charapter["class"] == "Shaman") LbCharapterSpec.Foreground = ClassColors.Shaman;
            if ((string)charapter["class"] == "Warlock") LbCharapterSpec.Foreground = ClassColors.Warlock;
            if ((string)charapter["class"] == "Warrior") LbCharapterSpec.Foreground = ClassColors.Warrior;
            #endregion

            #region GridRaidProgress
            int total_bosses = (int)charapter["raid_progression"]["castle-nathria"]["total_bosses"];
            int mythic_kills = (int)charapter["raid_progression"]["castle-nathria"]["mythic_bosses_killed"];
            int heroic_kills = (int)charapter["raid_progression"]["castle-nathria"]["heroic_bosses_killed"];
            int normal_kills = (int)charapter["raid_progression"]["castle-nathria"]["normal_bosses_killed"];

            TbRaidMythicProgress.Text = $"{mythic_kills}/{total_bosses} M";
            TbRaidHeroicProgress.Text = $"{heroic_kills}/{total_bosses} H";
            TbRaidNormalProgress.Text = $"{normal_kills}/{total_bosses} N";
            #endregion

            #region GridMythicProgress
            JArray items = (JArray)charapter["mythic_plus_best_runs"];

            if (items.Count >= 1) LbDungeon1.Content = (string)charapter["mythic_plus_best_runs"][0]["dungeon"];
            else LbDungeon1.Content = "";
            if (items.Count >= 2) LbDungeon2.Content = (string)charapter["mythic_plus_best_runs"][1]["dungeon"];
            else LbDungeon2.Content = "";
            if (items.Count >= 3) LbDungeon3.Content = (string)charapter["mythic_plus_best_runs"][2]["dungeon"];
            else LbDungeon3.Content = "";
            if (items.Count >= 4) LbDungeon4.Content = (string)charapter["mythic_plus_best_runs"][3]["dungeon"];
            else LbDungeon4.Content = "";
            if (items.Count >= 5) LbDungeon5.Content = (string)charapter["mythic_plus_best_runs"][4]["dungeon"];
            else LbDungeon5.Content = "";
            if (items.Count >= 6) LbDungeon6.Content = (string)charapter["mythic_plus_best_runs"][5]["dungeon"];
            else LbDungeon6.Content = "";
            if (items.Count >= 7) LbDungeon7.Content = (string)charapter["mythic_plus_best_runs"][6]["dungeon"];
            else LbDungeon7.Content = "";
            if (items.Count >= 8) LbDungeon8.Content = (string)charapter["mythic_plus_best_runs"][7]["dungeon"];
            else LbDungeon8.Content = "";

            // Fila de nivel de piedra
            string[] mythicLevel = new string[8];
            for (int i = 0; i < items.Count; i++)
            {
                if ((int)charapter["mythic_plus_best_runs"][i]["num_keystone_upgrades"] == 0)
                {
                    mythicLevel[i] = (string)charapter["mythic_plus_best_runs"][i]["mythic_level"];
                } 
                else if ((int)charapter["mythic_plus_best_runs"][i]["num_keystone_upgrades"] == 1)
                {
                    mythicLevel[i] = (string)charapter["mythic_plus_best_runs"][i]["mythic_level"] + "+";
                }
                else if ((int)charapter["mythic_plus_best_runs"][i]["num_keystone_upgrades"] == 2)
                {
                    mythicLevel[i] = (string)charapter["mythic_plus_best_runs"][i]["mythic_level"] + "++";
                }
            }
            if (items.Count >= 1) LbDungeonLevel1.Content = mythicLevel[0];
            else LbDungeonLevel1.Content = "";
            if (items.Count >= 2) LbDungeonLevel2.Content = mythicLevel[1];
            else LbDungeonLevel2.Content = "";
            if (items.Count >= 3) LbDungeonLevel3.Content = mythicLevel[2];
            else LbDungeonLevel3.Content = "";
            if (items.Count >= 4) LbDungeonLevel4.Content = mythicLevel[3];
            else LbDungeonLevel4.Content = "";
            if (items.Count >= 5) LbDungeonLevel5.Content = mythicLevel[4];
            else LbDungeonLevel5.Content = "";
            if (items.Count >= 6) LbDungeonLevel6.Content = mythicLevel[5];
            else LbDungeonLevel6.Content = "";
            if (items.Count >= 7) LbDungeonLevel7.Content = mythicLevel[6];
            else LbDungeonLevel7.Content = "";
            if (items.Count >= 8) LbDungeonLevel8.Content = mythicLevel[7];
            else LbDungeonLevel8.Content = "";

            // Fila de tiempo
            string[] timeOut = new string[8];
            for (int i = 0; i < items.Count; i++)
            {
                int time = (int)charapter["mythic_plus_best_runs"][i]["clear_time_ms"];
                int sec = time / 1000;
                int min = sec / 60;
                int hou = min / 60;

                while (sec > 60) 
                {
                    sec = sec - 60;
                }

                timeOut[i] = $"{hou}:{min}:{sec}";
            }
            
            if (items.Count >= 1) LbDungeonTime1.Content = timeOut[0];
            else LbDungeonTime1.Content = "";
            if (items.Count >= 2) LbDungeonTime2.Content = timeOut[1];
            else LbDungeonTime2.Content = "";
            if (items.Count >= 3) LbDungeonTime3.Content = timeOut[2];
            else LbDungeonTime3.Content = "";
            if (items.Count >= 4) LbDungeonTime4.Content = timeOut[3];
            else LbDungeonTime4.Content = "";
            if (items.Count >= 5) LbDungeonTime5.Content = timeOut[4];
            else LbDungeonTime5.Content = "";
            if (items.Count >= 6) LbDungeonTime6.Content = timeOut[5];
            else LbDungeonTime6.Content = "";
            if (items.Count >= 7) LbDungeonTime7.Content = timeOut[6];
            else LbDungeonTime7.Content = "";
            if (items.Count >= 8) LbDungeonTime8.Content = timeOut[7];
            else LbDungeonTime8.Content = "";

            string[,] affixes = new string[8,4];
            int[] mythicLevelInt = new int[8];

            for (int i = 0; i < items.Count; i++)
            {
                mythicLevelInt[i] = (int)charapter["mythic_plus_best_runs"][i]["mythic_level"];
                for (int ii = 0; ii < 4; ii++)
                {
                    switch (ii)
                    {
                        case 0:
                            affixes[i,ii] = (string)charapter["mythic_plus_best_runs"][i]["affixes"][ii]["name"];
                            break;

                        case 1:
                            if (mythicLevelInt[i] >= 5)
                            {
                                affixes[i,ii] = ", " + (string)charapter["mythic_plus_best_runs"][i]["affixes"][ii]["name"];
                            }
                            break;

                        case 2:
                            if (mythicLevelInt[i] >= 7)
                            {
                                affixes[i,ii] = ", " + (string)charapter["mythic_plus_best_runs"][i]["affixes"][ii]["name"];
                            }
                            break;

                        case 3:
                            if (mythicLevelInt[i] >= 10)
                            {
                                affixes[i,ii] = ", " + (string)charapter["mythic_plus_best_runs"][i]["affixes"][ii]["name"];
                            }
                            break;
                    }
                }
            }

            if (items.Count >= 1) LbDungeonAffixes1.Content = $"{affixes[0,0]}{affixes[0,1]}{affixes[0,2]}{affixes[0,3]}";
            else LbDungeonAffixes1.Content = "";
            if (items.Count >= 2) LbDungeonAffixes2.Content = $"{affixes[1,0]}{affixes[1,1]}{affixes[1,2]}{affixes[1,3]}";
            else LbDungeonAffixes2.Content = "";
            if (items.Count >= 3) LbDungeonAffixes3.Content = $"{affixes[2,0]}{affixes[2,1]}{affixes[2,2]}{affixes[2,3]}";
            else LbDungeonAffixes3.Content = "";
            if (items.Count >= 4) LbDungeonAffixes4.Content = $"{affixes[3,0]}{affixes[3,1]}{affixes[3,2]}{affixes[3,3]}";
            else LbDungeonAffixes4.Content = "";
            if (items.Count >= 5) LbDungeonAffixes5.Content = $"{affixes[4,0]}{affixes[4,1]}{affixes[4,2]}{affixes[4,3]}";
            else LbDungeonAffixes5.Content = "";
            if (items.Count >= 6) LbDungeonAffixes6.Content = $"{affixes[5,0]}{affixes[5,1]}{affixes[5,2]}{affixes[5,3]}";
            else LbDungeonAffixes6.Content = "";
            if (items.Count >= 7) LbDungeonAffixes7.Content = $"{affixes[6,0]}{affixes[6,1]}{affixes[6,2]}{affixes[6,3]}";
            else LbDungeonAffixes7.Content = "";
            if (items.Count >= 8) LbDungeonAffixes8.Content = $"{affixes[7,0]}{affixes[7,1]}{affixes[7,2]}{affixes[7,3]}";
            else LbDungeonAffixes8.Content = "";

            if (items.Count >= 1) LbDungeonScore1.Content = (string)charapter["mythic_plus_best_runs"][0]["score"];
            else LbDungeonScore1.Content = "";
            if (items.Count >= 2) LbDungeonScore2.Content = (string)charapter["mythic_plus_best_runs"][1]["score"];
            else LbDungeonScore2.Content = "";
            if (items.Count >= 3) LbDungeonScore3.Content = (string)charapter["mythic_plus_best_runs"][2]["score"];
            else LbDungeonScore3.Content = "";
            if (items.Count >= 4) LbDungeonScore4.Content = (string)charapter["mythic_plus_best_runs"][3]["score"];
            else LbDungeonScore4.Content = "";
            if (items.Count >= 5) LbDungeonScore5.Content = (string)charapter["mythic_plus_best_runs"][4]["score"];
            else LbDungeonScore5.Content = "";
            if (items.Count >= 6) LbDungeonScore6.Content = (string)charapter["mythic_plus_best_runs"][5]["score"];
            else LbDungeonScore6.Content = "";
            if (items.Count >= 7) LbDungeonScore7.Content = (string)charapter["mythic_plus_best_runs"][6]["score"];
            else LbDungeonScore7.Content = "";
            if (items.Count >= 8) LbDungeonScore8.Content = (string)charapter["mythic_plus_best_runs"][7]["score"];
            else LbDungeonScore8.Content = "";

            for (int i = 1; i < 9; i++)
            {
                if (i <= items.Count) { GridMythicProgression.RowDefinitions[i].Height = new GridLength(26); Console.WriteLine($"i={i} | items.Count={items.Count} | Entro en IF"); }
                else { GridMythicProgression.RowDefinitions[i].Height = new GridLength(0); Console.WriteLine($"i={i} | items.Count={items.Count} | Entro en ELSE"); }
            }
            #endregion

        }
    }
}
