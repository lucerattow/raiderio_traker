using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp1.API
{
    class RaiderIoApi
    {
        /*  Consume la api oficial de Raider.IO.
         *
         *  Esta api devuelve un Json con los datos completos de los personajes buscados.
         */
        public static string GetCharacterInfo(string region, string realm, string name) 
        {
            string fields = "gear,guild,covenant,raid_progression,mythic_plus_scores_by_season:current,";
            fields = fields + "mythic_plus_ranks,mythic_plus_recent_runs,mythic_plus_best_runs,mythic_plus_highest_level_runs,";
            fields = fields + "mythic_plus_weekly_highest_level_runs,mythic_plus_previous_weekly_highest_level_runs,";
            fields = fields + "previous_mythic_plus_ranks,raid_achievement_meta,raid_achievement_curve";

            string url = "https://raider.io/api/v1/characters/profile?region=" + region + "&realm=" + realm + "&name=" + name + "&fields=" + fields;
            var client = new RestClient(url);
            client.Timeout = -1;
            var request = new RestRequest(Method.GET);
            request.AddParameter("text/plain", "",  ParameterType.RequestBody);
            IRestResponse response = client.Execute(request);
            
            return response.Content.ToString();
        }

        public static string GetScoreTiersColors()
        {
            string url = "https://raider.io/api/v1/mythic-plus/score-tiers?season=current";
            var client = new RestClient(url);
            client.Timeout = -1;
            var request = new RestRequest(Method.GET);
            request.AddParameter("text/plain", "",  ParameterType.RequestBody);
            IRestResponse response = client.Execute(request);

            string str = response.Content.ToString();
            str = "{\"colors\":" + str + "}";

            return str;
        }
    }
}
