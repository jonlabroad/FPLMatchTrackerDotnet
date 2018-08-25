using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NLog;

public class Fixture
{
    private static Logger _log = LogManager.GetCurrentClassLogger();
    public int id{get;set;}
    
    public string kickoff_time_formatted{get;set;}
    
    public bool started{get;set;}
    
    public int event_day{get;set;}
    
    public string deadline_time{get;set;}
    
    public string deadline_time_formatted{get;set;}
    
    public JArray stats {get; set;}
    
    public EventStats parsedStats{get;set;}
    
    public int code{get;set;}
    
    public string kickoff_time{get;set;}
    
    public int team_h_score {get;set;}
    
    public int team_a_score{get;set;}
    
    public bool finished{get;set;}
    
    public int minutes{get;set;}
    
    public bool provisional_start_time{get;set;}
    
    public bool finished_provisional{get;set;}

    [JsonProperty(PropertyName="event")]
    public int eventId {get;set;}
    
    public int team_a{get;set;}
    
    public int team_h{get;set;}

    public EventStats getStats()
    {
        EventStats parsedStats = new EventStats();
        for (int i = 0; i < stats.Count; i++) {
            var statObject = (JObject) stats[i] as JObject;
            foreach (var prop in statObject.Properties())
            {
                var statName = prop.Name;
                var stat = prop.Value;
                var parsedExplain = JsonConvert.DeserializeObject<HomeAwayStats>(stat.ToString(), new JsonSerializerSettings {
                                                            NullValueHandling = NullValueHandling.Ignore
                                                        });
                setField(parsedStats, statName, parsedExplain);
            }
        }
        return parsedStats;
    }

    private void setField(EventStats element, String fieldName, HomeAwayStats explain)
    {
        try {
            var field = typeof(EventStats).GetProperty(fieldName);
            field.SetValue(element, explain);
        } catch (Exception e) {
            _log.Error(e);
        }
    }
}
